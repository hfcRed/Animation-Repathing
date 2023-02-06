using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Animations;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AutoAnimationRepath
{
    [InitializeOnLoad]
    public class AARAutomatic
    {
        //Run on initialization
        static AARAutomatic()
        {
            LoadData();

            EditorApplication.hierarchyChanged -= HierarchyChanged;
            //Selection.selectionChanged -= SelectionChanged;

            EditorApplication.hierarchyChanged += HierarchyChanged;
            //Selection.selectionChanged += SelectionChanged;
        }

        //Create variables
        public static int toolSelection;
        public static string[] tools = { "Automatic", "Manual" };
        public static int baseSelection;
        public static string[] baseOptions = { "Animator Component", "VRChat Avatar" };
        public static int languageSelection;
        public static string[] languageOptions = { "English", "日本" };

        private static Animator _animator;

        public static Animator animator
        {
            get => _animator;
            set
            {
                if (_animator != value)
                {
                    _animator = value;
                    OnRootChanged();
                }
            }
        }
        public static AnimatorController controller;

        private static GameObject _avatar;

        public static GameObject avatar
        {
            get => _avatar;
            set
            {
                if (_avatar != value)
                {
                    _avatar = value;
                    OnRootChanged();
                }
            }
        }

        public static bool isEnabled;
        public static bool renameActive = true;
        public static bool reparentActive = true;
        public static bool renameWarning = true;
        public static bool reparentWarning = true;
        public static bool activeInBackground;
        public static bool showDebug = false;
        public static bool foldout;

        public enum Playables
        {
            Base = 1 << 0,
            Additive = 1 << 1,
            Gesture = 1 << 2,
            Action = 1 << 3,
            FX = 1 << 4,
            Sitting = 1 << 5,
            TPose = 1 << 6,
            IKPose = 1 << 7,
            all = ~0
        }
        public static Playables PlayableSelection = Playables.all;

        private static readonly List<HierarchyTransform> hierarchyTransforms = new List<HierarchyTransform>();
        private static readonly Dictionary<string, string> changedPaths = new Dictionary<string, string>();


        #region Save settings
        //Save settings to disk
        public static void SaveData()
        {
            //Save directly
            EditorPrefs.SetInt("AAR BaseSelection", baseSelection);
            EditorPrefs.SetInt("AAR LanguageSelection", languageSelection);

            EditorPrefs.SetBool("AAR Toggle", isEnabled);
            EditorPrefs.SetBool("AAR RenameActive", renameActive);
            EditorPrefs.SetBool("AAR ReparentActive", reparentActive);
            EditorPrefs.SetBool("AAR RenameWarning", renameWarning);
            EditorPrefs.SetBool("AAR ReparentWarning", reparentWarning);
            EditorPrefs.SetBool("AAR ActiveInBackground", activeInBackground);

            //Get controller name to save as string
            EditorPrefs.SetString("AAR Controller", animator == null ? null : animator.gameObject.transform.name);

            //Get avatar name to save as string
            EditorPrefs.SetString("AAR Avatar", avatar == null ? null : avatar.name);

            EditorPrefs.SetInt("AAR PlayableSelection", (int)PlayableSelection);

            LoadData();
        }
        #endregion

        #region Load settings
        //Load settings on call
        public static void LoadData()
        {
            //Load directly
            baseSelection = EditorPrefs.GetInt("AAR BaseSelection");

            isEnabled = EditorPrefs.GetBool("AAR Toggle");
            renameActive = EditorPrefs.GetBool("AAR RenameActive");
            reparentActive = EditorPrefs.GetBool("AAR ReparentActive");
            renameWarning = EditorPrefs.GetBool("AAR RenameWarning");
            reparentWarning = EditorPrefs.GetBool("AAR ReparentWarning");
            activeInBackground = EditorPrefs.GetBool("AAR ActiveInBackground");

            //Load GUID of custom controller to load the asset
            string findController = EditorPrefs.GetString("AAR Controller");
            GameObject animatorHolder = GameObject.Find(findController);
            if (animatorHolder != null)
            {
                animator = animatorHolder.GetComponent(typeof(Animator)) as Animator;
                controller = animator == null ? null : animator.runtimeAnimatorController as AnimatorController;
            }

            //Load avatar name to load the gameobject
            avatar = GameObject.Find(EditorPrefs.GetString("AAR Avatar"));

            PlayableSelection = (Playables)EditorPrefs.GetInt("AAR PlayableSelection");
        }
        #endregion

        /*
        #region Selection Change
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
        #endregion
        */

        #region Hierarchy Change
        //Run when anything in the hierarchy changes
        public static void HierarchyChanged()
        {
            bool shouldRun = isEnabled;
            shouldRun &= activeInBackground || EditorWindow.HasOpenInstances<AAREditor>();
            shouldRun &= renameActive || reparentActive;

            var root = GetRoot();
            shouldRun &= root;

            //temporary condition for current method of targetting a controller
            controller = animator.runtimeAnimatorController as AnimatorController;
            shouldRun &= baseSelection == 0 && controller;
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
        #endregion

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

        private static Transform GetRoot() => baseSelection == 0 ?
            animator == null ? null : animator.transform :
            avatar == null ? null : avatar.transform;

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