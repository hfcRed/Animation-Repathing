using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Animations;


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
            Selection.selectionChanged -= SelectionChanged;

            EditorApplication.hierarchyChanged += HierarchyChanged;
            Selection.selectionChanged += SelectionChanged;
        }

        //Create variables
        public static int toolSelection;
        public static string[] tools = { "Automatic", "Manual" };
        public static int baseSelection;
        public static string[] baseOptions = { "Animator Component", "VRChat Avatar" };
        public static int languageSelection;
        public static string[] languageOptions = { "English", "日本" };

        public static Animator animator;
        public static AnimatorController controller;
        public static GameObject avatar;

        public static bool toggle;
        public static bool renameActive = true;
        public static bool reparentActive = true;
        public static bool renameWarning = true;
        public static bool reparentWarning = true;
        public static bool activeInBackground;
        public static bool showDebug = false;

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

        private static Transform selectedTransform;
        private static Transform oldParent;
        private static Transform newParent;
        private static string oldName;
        private static string newName;
        private static string oldPath;
        private static string newPath;
        private static string fullPath;
        private static int removeLength;

        #region Save settings
        //Save settings to disk
        public static void SaveData()
        {
            //Save directly
            EditorPrefs.SetInt("AAR BaseSelection", baseSelection);
            EditorPrefs.SetInt("AAR LanguageSelection", languageSelection);

            EditorPrefs.SetBool("AAR Toggle", toggle);
            EditorPrefs.SetBool("AAR RenameActive", renameActive);
            EditorPrefs.SetBool("AAR ReparentActive", reparentActive);
            EditorPrefs.SetBool("AAR RenameWarning", renameWarning);
            EditorPrefs.SetBool("AAR ReparentWarning", reparentWarning);
            EditorPrefs.SetBool("AAR ActiveInBackground", activeInBackground);

            //Get controller name to save as string
            EditorPrefs.SetString("AAR Controller", animator == null ? null : animator.gameObject.transform.name );

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

            toggle = EditorPrefs.GetBool("AAR Toggle");
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

        #region Selection Change
        //Run when selection has changed
        public static void SelectionChanged()
        {
            if (EditorWindow.HasOpenInstances<AAREditor>() && toggle || !EditorWindow.HasOpenInstances<AAREditor>() && activeInBackground && toggle)
            {
                if (Selection.activeGameObject == null || Selection.activeTransform.parent == null)
                {
                    return;
                }
                oldParent = Selection.activeTransform.parent;
                oldName = Selection.activeGameObject.name;
                oldPath = AnimationUtility.CalculateTransformPath(Selection.activeTransform, animator.transform);
                removeLength = oldPath.Length;
            }
        }
        #endregion

        #region Hierarchy Change
        //Run when anything in the hierarchy changes
        public static void HierarchyChanged()
        {
            if (EditorWindow.HasOpenInstances<AAREditor>() && toggle && Selection.activeTransform != null && Selection.activeTransform.IsChildOf(animator.transform) || !EditorWindow.HasOpenInstances<AAREditor>() && activeInBackground && toggle && Selection.activeTransform != null && Selection.activeTransform.IsChildOf(animator.transform))
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
        }
        #endregion
        
        public static void RepathParent()
        {
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
        }

        public static void RepathName()
        {
            oldName = newName;
        }
    }
}