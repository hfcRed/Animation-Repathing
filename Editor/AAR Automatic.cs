using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static AutoAnimationRepath.AARVariables;

namespace AutoAnimationRepath
{
    [InitializeOnLoad]
    public class AARAutomatic
    {
        static AARAutomatic()
        {
            EditorApplication.hierarchyChanged -= HierarchyChanged;
            EditorApplication.hierarchyChanged += HierarchyChanged;
        }

        public class HierarchyTransform
        {
            public Transform target;
            public string path;

            public HierarchyTransform(Transform t, Transform root)
            {
                target = t;
                path = AnimationUtility.CalculateTransformPath(t, root);
            }
        }

        public static void HierarchyChanged()
        {
            bool shouldRun = automaticIsEnabled;
            shouldRun &= activeInBackground || EditorWindow.HasOpenInstances<AAREditor>();
            shouldRun &= renameActive || reparentActive;

            var root = GetRoot();
            shouldRun &= root;

            List<AnimatorController> controllers = GetControllers();
            shouldRun &= controllers.Count > 0;
            if (!shouldRun) return;

            changedPaths.Clear();
            for (int i = hierarchyTransforms.Count - 1; i >= 0; i--)
            {
                var ht = hierarchyTransforms[i];
                if (ht.target == null || !ht.target.IsChildOf(root))
                {
                    hierarchyTransforms.RemoveAt(i);
                    continue;
                }

                var currentPath = AnimationUtility.CalculateTransformPath(ht.target, root);
                if (ht.path != currentPath)
                    try { changedPaths.Add(ht.path, currentPath); }
                    catch
                    {
                        //ignore error
                        //only errors if key already exists
                        //i.e: two hierarchy transforms with the same name
                    }

                ht.path = currentPath;
            }

            if (changedPaths.Count == 0) return;

            if (warnOnlyIfUsed && ScanAnimators() && (!reparentWarning || EditorUtility.DisplayDialog(AARStrings.Popup.title, AARStrings.Popup.message, AARStrings.Popup.continuee, AARStrings.Popup.cancel)))
            {
                foreach (AnimatorController a in controllers)
                {
                    RepathAnimations(a);
                }
            }
            else if (!warnOnlyIfUsed && (!reparentWarning || EditorUtility.DisplayDialog(AARStrings.Popup.title, AARStrings.Popup.message, AARStrings.Popup.continuee, AARStrings.Popup.cancel)))
            {
                foreach (AnimatorController a in controllers)
                {
                    RepathAnimations(a);
                }
            }
        }

        /// <summary>
        /// Returns true if any of the hierarchy changes affect an Animation Clip
        /// in either the Animator Component OR Avatar currently targeted in the settings.
        /// </summary>
        public static bool ScanAnimators()
        {
            List<AnimatorController> controllers = GetControllers();
            bool returnValue = false;

            foreach (AnimatorController animator in controllers)
            {
                foreach (AnimationClip clip in animator.animationClips)
                {
                    EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                    EditorCurveBinding[] objectCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);

                    foreach (var fc in floatCurves) CheckBindings(fc, false);
                    foreach (var oc in objectCurves) CheckBindings(oc, true);

                    void CheckBindings(EditorCurveBinding binding, bool isObjectCurve)
                    {
                        if (changedPaths.TryGetValue(binding.path, out string newPath))
                        {
                            returnValue = true;
                        }
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Loops through all Animation Clips in an Animator Controller
        /// and changes every animation path which contains the old hierarchy path to the new hierarchy path.
        /// </summary>
        public static void RepathAnimations(AnimatorController target)
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                StringBuilder displayedChanges = new StringBuilder(AARStrings.Popup.debug);

                foreach (AnimationClip clip in target.animationClips)
                {
                    EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                    EditorCurveBinding[] objectCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);

                    foreach (var fc in floatCurves) HandleBinding(fc, false);
                    foreach (var oc in objectCurves) HandleBinding(oc, true);

                    void HandleBinding(EditorCurveBinding b, bool isObjectCurve)
                    {
                        if (!changedPaths.TryGetValue(b.path, out string newPath)) return;
                        if (isObjectCurve)
                        {
                            string s = b.path + AARStrings.Popup.to + newPath;
                            displayedChanges.AppendLine("");
                            displayedChanges.AppendLine(s);

                            ObjectReferenceKeyframe[] objectCurve = AnimationUtility.GetObjectReferenceCurve(clip, b);
                            AnimationUtility.SetObjectReferenceCurve(clip, b, null);
                            b.path = newPath;
                            AnimationUtility.SetObjectReferenceCurve(clip, b, objectCurve);
                        }
                        else
                        {
                            string s = b.path + AARStrings.Popup.to + newPath;
                            displayedChanges.AppendLine("");
                            displayedChanges.AppendLine(s);

                            AnimationCurve floatCurve = isObjectCurve ? null : AnimationUtility.GetEditorCurve(clip, b);
                            AnimationUtility.SetEditorCurve(clip, b, null);
                            b.path = newPath;
                            AnimationUtility.SetEditorCurve(clip, b, floatCurve);
                        }
                    }
                }
                Debug.Log(displayedChanges.ToString());
            }
            finally { AssetDatabase.StopAssetEditing(); }
        }

        /// <summary>
        /// Creates a window popup listing all hierarchy changes.
        /// Returns true or false depending on if the user wants to continue or cancel.
        /// </summary>
        /*private static bool DisplayReparentDialog()
        {
            StringBuilder displayedChanges = new StringBuilder(AARStrings.Popup.message + $":{Environment.NewLine}");

            foreach (var s in changedPaths.Keys.Zip(changedPaths.Values, (s1, s2) => $"\"{s1}\"" + AARStrings.Popup.to + $"\"{s2}\""))
            {
                displayedChanges.AppendLine("");
                displayedChanges.AppendLine(s);
            }
            return EditorUtility.DisplayDialog(AARStrings.Popup.title, displayedChanges.ToString(), AARStrings.Popup.continuee, AARStrings.Popup.cancel);
        }*/

        public static void OnRootChanged()
        {
            hierarchyTransforms.Clear();

            Transform root = GetRoot();
            if (!root) return;

            var allChildren = root.GetComponentsInChildren<Transform>();
            for (int i = 1; i < allChildren.Length; i++)
            {
                var t = allChildren[i];
                hierarchyTransforms.Add(new HierarchyTransform(t, root));
            }
        }
    }
}