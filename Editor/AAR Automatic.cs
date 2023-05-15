using System;
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

        /*
        //Run when selection has changed
        public static void SelectionChanged()
        {
            if ((!activeInBackground && !EditorWindow.HasOpenInstances<AAREditor>()) || !isEnabled) return;
            if (Selection.activeGameObject == null || Selection.activeTransform.parent == null)
                return;
            
            oldParent = Selection.activeTransform.parent;
            oldName = Selection.activeGameObject.name;
            oldPath = AnimationUtility.CalculateTransformPath(Selection.activeTransform, animator.transform);
            removeLength = oldPath.Length;
        }
        */

        public static void HierarchyChanged()
        {
            bool shouldRun = automaticIsEnabled;
            shouldRun &= activeInBackground || EditorWindow.HasOpenInstances<AAREditor>();
            shouldRun &= renameActive || reparentActive;

            var root = GetRoot();
            shouldRun &= root;

            //temporary condition for current method of targetting a controller
            controller = animator?.runtimeAnimatorController as AnimatorController;
            shouldRun &= controllerSelection == 0 && controller;
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
            if (!reparentWarning || DisplayReparentDialog())
                RepathParent(controller);

            /*
            if (isEnabled && Selection.activeTransform != null && Selection.activeTransform.IsChildOf(animator.transform))
            {
                newName = Selection.activeTransform.name;
                newParent = Selection.activeTransform.parent;
                selectedTransform = Selection.activeTransform;

                if (reparentActive && newParent != null && oldParent != null)
                {
                    if (!reparentWarning) RepathParent();
                    else if (EditorUtility.DisplayDialog("Auto Repathing", "Repathing animation properties for " + selectedTransform.name + " to " + selectedTransform.parent.name, "Continue", "Cancel"))
                    {
                        RepathParent();
                    }
                }

                if (renameActive && oldName != newName && oldName != null)
                {
                    if (!renameWarning) RepathName();
                    else if (EditorUtility.DisplayDialog("Auto Repathing", "Repathing animation properties from " + oldName + " to " + selectedTransform.name, "Continue", "Cancel"))
                    {
                        RepathName();
                    }


                }
            }
        */
        }

        public static void RepathParent(AnimatorController target)
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (AnimationClip clip in controller.animationClips)
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

                    /*
                    foreach (EditorCurveBinding b in floatCurves)
                    {
                        EditorCurveBinding binding = b;
                        AnimationCurve Curve = AnimationUtility.GetEditorCurve(clip, binding);
                        
                        if (binding.path.Contains(oldPath) && binding.path != fullPath)
                        {
                            string OldPathCut = binding.path.Remove(0, removeLength);
                            fullPath = newPath + OldPathCut;

                            AnimationUtility.SetEditorCurve(clip, binding, null);
                            binding.path = fullPath;
                            AnimationUtility.SetEditorCurve(clip, binding, Curve);
                        }
                    }
                */
                }
            }
            finally { AssetDatabase.StopAssetEditing(); }
            //CONTIUE

            /*
            if (baseSelection == 0 && controller != null && oldPath != null)
            {
                newPath = AnimationUtility.CalculateTransformPath(Selection.activeTransform, animator.transform);

                try
                {
                    AssetDatabase.StartAssetEditing();
                    
                    foreach (AnimationClip clip in controller.animationClips)
                    {
                        Array curves = AnimationUtility.GetCurveBindings(clip);

                        foreach (EditorCurveBinding b in curves)
                        {
                            EditorCurveBinding binding = b;
                            AnimationCurve Curve = AnimationUtility.GetEditorCurve(clip, binding);

                            if (binding.path.Contains(oldPath) && binding.path != fullPath)
                            {
                                string OldPathCut = binding.path.Remove(0, removeLength);
                                fullPath = newPath + OldPathCut;

                                AnimationUtility.SetEditorCurve(clip, binding, null);
                                binding.path = fullPath;
                                AnimationUtility.SetEditorCurve(clip, binding, Curve);
                            }
                        }
                    }                   
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }          
            oldParent = newParent;
            oldPath = AnimationUtility.CalculateTransformPath(Selection.activeTransform, animator.transform);
            removeLength = oldPath.Length;
            fullPath = null;
        */
        }

        //public static void RepathName() => oldName = newName;

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
    }
}