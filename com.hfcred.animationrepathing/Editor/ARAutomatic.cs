using static AnimationRepathing.ARVariables;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimationRepathing
{
    [InitializeOnLoad]
    public class ARAutomatic
    {
        static ARAutomatic()
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

        public static void GetAllChildren()
        {
            hierarchyTransforms.Clear();

            Transform root = GetRoot();
            if (!root) return;

            var allChildren = root.GetComponentsInChildren<Transform>(true);
            for (int i = 1; i < allChildren.Length; i++)
            {
                var t = allChildren[i];
                hierarchyTransforms.Add(new HierarchyTransform(t, root));
            }
        }

 public class Tree
        {
            public string value;
            public Transform transform;
            public Tree[] children;

            public Tree(string value, Transform transform, Tree[] children)
            {
                this.value = value;
                this.transform = transform;
                this.children = children;
            }
            
            protected bool Equals(Tree other)
            {
                return value == other.value && Equals(children, other.children);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Tree)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    if (children == null)
                    {
                        return ((value != null ? value.GetHashCode() : 0) * 397);
                    }

                    int childVal = children.Length;
                    foreach (var child in children)
                    {
                        childVal = childVal * 17 + child.GetHashCode();
                    }
                    return ((value != null ? value.GetHashCode() : 0) * 397) ^ childVal;
                }
            }

            public static Tree GetTreeFromChildren(Transform root)
            {
                Tree[] children = new Tree[root.transform.childCount];
                for (int i = 0; i < root.transform.childCount; i++)
                {
                    children[i] = GetTreeFromChildren(root.transform.GetChild(i));
                }
                return new Tree(root.name, root, children);
            }
        }
        
        
        public static void HierarchyChanged()
        {
            bool shouldRun = automaticIsEnabled;
            shouldRun &= activeInBackground || EditorWindow.HasOpenInstances<AREditor>();

            var root = GetRoot();
            shouldRun &= root;

            List<AnimatorController> controllers = GetControllers();
            shouldRun &= controllers != null;
            shouldRun &= controllers.Count > 0;
            if (!shouldRun) return;
            
            Tree tree = Tree.GetTreeFromChildren(root);

            int hashCode = tree.GetHashCode();
            if (hashCode == hierarchyHash)
            {
                return;
            }

            var childCount = root.GetComponentsInChildren<Transform>(true).Where(x => x != root).ToList();
            if (childCount.Count != hierarchyTransforms.Count)
            {
                List<HierarchyTransform> compare = new List<HierarchyTransform>();
                foreach (Transform t in childCount)
                {
                    compare.Add(new HierarchyTransform(t, root));
                }
                hierarchyTransforms.AddRange(compare.Where(x => !hierarchyTransforms.Contains(x)));
            }

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
                {
                    try
                    {
                        changedPaths.Add(ht.path, currentPath);
                    }
                    catch
                    {
                        //ignore error
                        //only errors if key already exists
                        //i.e: two hierarchy transforms with the same name
                    }
                }

                ht.path = currentPath;
            }

            if (changedPaths.Count == 0) return;

            if (warnOnlyIfUsed && ScanAnimators() && (!sendWarning || EditorUtility.DisplayDialog(ARStrings.Popup.title, ARStrings.Popup.message, ARStrings.Popup.continuee, ARStrings.Popup.cancel)))
            {
                RepathAnimations(controllers.ToArray());
            }
            else if (!warnOnlyIfUsed && (!sendWarning || EditorUtility.DisplayDialog(ARStrings.Popup.title, ARStrings.Popup.message, ARStrings.Popup.continuee, ARStrings.Popup.cancel)))
            {
                RepathAnimations(controllers.ToArray());
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

                    foreach (var fc in floatCurves) CheckBindings(fc);
                    foreach (var oc in objectCurves) CheckBindings(oc);

                    void CheckBindings(EditorCurveBinding binding)
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
        public static void RepathAnimations(AnimatorController[] targets)
        {
            StringBuilder displayedChanges = new StringBuilder(ARStrings.Popup.debug);
            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (AnimatorController target in targets)
                {
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

                            if (!displayedChanges.ToString().Contains(b.path))
                            {
                                string s = b.path + ARStrings.Popup.to + newPath;
                                displayedChanges.AppendLine("");
                                displayedChanges.AppendLine(s);
                            }
                        }
                    }
                }
            }
            finally { AssetDatabase.StopAssetEditing(); Debug.Log(displayedChanges); }
        }
    }
}