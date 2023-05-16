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

            if (warnOnlyIfUsed && ScanAnimators())
            {
                foreach (AnimatorController a in controllers)
                {
                    RepathParent(a);
                }
            }
            else if (!warnOnlyIfUsed && (!reparentWarning || DisplayReparentDialog()))
            {
                foreach (AnimatorController a in controllers)
                {
                    RepathParent(a);
                }
            }
        }

        public static bool ScanAnimators()
        {
            return true;
        }

        public static void RepathParent(AnimatorController target)
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (AnimationClip clip in target.animationClips)
                {
                    EditorCurveBinding[] floatCurves = AnimationUtility.GetCurveBindings(clip);
                    EditorCurveBinding[] objectCurves = AnimationUtility.GetObjectReferenceCurveBindings(clip);

                    void HandleBinding(EditorCurveBinding b, bool isObjectCurve)
                    {
                        if (!changedPaths.TryGetValue(b.path, out string newPath)) return;
                        if (isObjectCurve)
                        {
                            ObjectReferenceKeyframe[] objectCurve = AnimationUtility.GetObjectReferenceCurve(clip, b);
                            AnimationUtility.SetObjectReferenceCurve(clip, b, null);
                            b.path = newPath;
                            AnimationUtility.SetObjectReferenceCurve(clip, b, objectCurve);
                        }
                        else
                        {
                            AnimationCurve floatCurve = isObjectCurve ? null : AnimationUtility.GetEditorCurve(clip, b);
                            AnimationUtility.SetEditorCurve(clip, b, null);
                            b.path = newPath;
                            AnimationUtility.SetEditorCurve(clip, b, floatCurve);
                        }
                    }

                    foreach (var fc in floatCurves) HandleBinding(fc, false);
                    foreach (var oc in objectCurves) HandleBinding(oc, true);
                }
            }
            finally { AssetDatabase.StopAssetEditing(); }
        }

        private static bool DisplayReparentDialog()
        {
            StringBuilder displayedChanges = new StringBuilder($"Repathing animation properties for:{Environment.NewLine}");

            foreach (var s in changedPaths.Keys.Zip(changedPaths.Values, (s1, s2) => $"{s1} to {s2}"))
                displayedChanges.AppendLine(s);

            return EditorUtility.DisplayDialog("Auto Repathing", displayedChanges.ToString(), "Continue", "Cancel");
        }

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