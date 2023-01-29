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
        public static int ToolSelection;
        public static string[] Tools = { "Automatic", "Manual" };
        public static int BaseSelection;
        public static string[] BaseOptions = { "Animator Component", "VRChat Avatar" };
        public static int LanguageSelection;
        public static string[] LanguageOptions = { "English", "日本" };

        public static Animator Controller;
        public static AnimatorController TargetController;
        public static GameObject Avatar;

        public static bool Toggle;
        public static bool RenameAvtive = true;
        public static bool ReparentActive = true;
        public static bool RenameWarning = true;
        public static bool ReparentWarning = true;
        public static bool ActiveInBackground;
        public static bool ShowDebug = false;

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

        static Transform SelectedTransform;
        static Transform OldParent;
        static Transform NewParent;
        static string OldName;
        static string NewName;
        static string OldPath;
        static string NewPath;
        static string FullPath;
        static int Remove;

        #region Save settings
        //Save settings to disk
        public static void SaveData()
        {
            //Save directly
            EditorPrefs.SetInt("AAR BaseSelection", BaseSelection);
            EditorPrefs.SetInt("AAR LanguageSelection", LanguageSelection);

            EditorPrefs.SetBool("AAR Toggle", Toggle);
            EditorPrefs.SetBool("AAR RenameActive", RenameAvtive);
            EditorPrefs.SetBool("AAR ReparentActive", ReparentActive);
            EditorPrefs.SetBool("AAR RenameWarning", RenameWarning);
            EditorPrefs.SetBool("AAR ReparentWarning", ReparentWarning);
            EditorPrefs.SetBool("AAR ActiveInBackground", ActiveInBackground);

            //Get controller name to save as string
            if (Controller != null)
            {
                EditorPrefs.SetString("AAR Controller", Controller.gameObject.transform.name);
            }
            else
            {
                EditorPrefs.SetString("AAR Controller", null);
            }

            //Get avatar name to save as string
            if (Avatar != null)
            {
                EditorPrefs.SetString("AAR Avatar", Avatar.name);
            }
            else
            {
                EditorPrefs.SetString("AAR Avatar", null);
            }

            EditorPrefs.SetInt("AAR PlayableSelection", (int)PlayableSelection);

            LoadData();
        }
        #endregion

        #region Load settings
        //Load settings on call
        public static void LoadData()
        {
            //Load directly
            BaseSelection = EditorPrefs.GetInt("AAR BaseSelection");

            Toggle = EditorPrefs.GetBool("AAR Toggle");
            RenameAvtive = EditorPrefs.GetBool("AAR RenameActive");
            ReparentActive = EditorPrefs.GetBool("AAR ReparentActive");
            RenameWarning = EditorPrefs.GetBool("AAR RenameWarning");
            ReparentWarning = EditorPrefs.GetBool("AAR ReparentWarning");
            ActiveInBackground = EditorPrefs.GetBool("AAR ActiveInBackground");

            //Load GUID of custom controller to load the asset
            string FindController = EditorPrefs.GetString("AAR Controller");
            GameObject AnimatorHolder = GameObject.Find(FindController);
            if (AnimatorHolder != null)
            {
                Controller = AnimatorHolder.GetComponent(typeof(Animator)) as Animator;
                RuntimeAnimatorController RuntimeController = Controller.runtimeAnimatorController;
                TargetController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(RuntimeController));
            }

            //Load avatar name to load the gameobject
            string FindAvatar = EditorPrefs.GetString("AAR Avatar");
            Avatar = GameObject.Find(FindAvatar);

            PlayableSelection = (Playables)EditorPrefs.GetInt("AAR PlayableSelection");
        }
        #endregion

        #region Selection Change
        //Run when selection has changed
        public static void SelectionChanged()
        {
            if (EditorWindow.HasOpenInstances<AAREditor>() && Toggle == true || !EditorWindow.HasOpenInstances<AAREditor>() && ActiveInBackground == true && Toggle == true)
            {
                if (Selection.activeGameObject == null || Selection.activeTransform.parent == null)
                {
                    return;
                }
                OldParent = Selection.activeTransform.parent;
                OldName = Selection.activeGameObject.name;
                OldPath = AnimationUtility.CalculateTransformPath(Selection.activeTransform, Controller.transform);
                Remove = OldPath.Length;
            }
        }
        #endregion

        #region Hierarchy Change
        //Run when anything in the hierarchy changes
        public static void HierarchyChanged()
        {
            if (EditorWindow.HasOpenInstances<AAREditor>() && Toggle == true && Selection.activeTransform != null && Selection.activeTransform.IsChildOf(Controller.transform) == true|| !EditorWindow.HasOpenInstances<AAREditor>() && ActiveInBackground == true && Toggle == true && Selection.activeTransform != null && Selection.activeTransform.IsChildOf(Controller.transform) == true)
            {
                NewName = Selection.activeTransform.name;
                NewParent = Selection.activeTransform.parent;
                SelectedTransform = Selection.activeTransform;

                if (ReparentActive == true && NewParent != null && OldParent != null)
                {
                    if (ReparentWarning == true)
                    {
                        if (EditorUtility.DisplayDialog("Auto Repathing", "Repathing animation properties for " + SelectedTransform.name + " to " + SelectedTransform.parent.name, "Continue", "Cancel"))
                        {
                            RepathParent();
                        }
                    }
                    else
                    {
                        RepathParent();
                    }
                }

                if (RenameAvtive == true && OldName != NewName && OldName != null)
                {
                    if (RenameWarning == true)
                    {
                        if (EditorUtility.DisplayDialog("Auto Repathing", "Repathing animation properties from " + OldName + " to " + SelectedTransform.name, "Continue", "Cancel"))
                        {
                            RepathName();
                        }
                    }
                    else
                    {
                        RepathName();
                    }
                }
            }
        }
        #endregion
        
        public static void RepathParent()
        {
            if (BaseSelection == 0 && TargetController != null && OldPath != null)
            {
                NewPath = AnimationUtility.CalculateTransformPath(Selection.activeTransform, Controller.transform);

                try
                {
                    AssetDatabase.StartAssetEditing();
                    
                    foreach (AnimationClip Clip in TargetController.animationClips)
                    {
                        Array Curves = AnimationUtility.GetCurveBindings(Clip);

                        foreach (EditorCurveBinding B in Curves)
                        {
                            EditorCurveBinding Binding = B;
                            AnimationCurve Curve = AnimationUtility.GetEditorCurve(Clip, Binding);

                            if (Binding.path.Contains(OldPath) && Binding.path != FullPath)
                            {
                                
                                AnimationUtility.SetEditorCurve(Clip, Binding, null);
                                string OldPathCut = Binding.path.Remove(0, Remove);
                                FullPath = NewPath + OldPathCut;
                                Binding.path = FullPath;
                                AnimationUtility.SetEditorCurve(Clip, Binding, Curve);
                            }
                        }
                    }                   
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }          
            OldParent = NewParent;
            OldPath = AnimationUtility.CalculateTransformPath(Selection.activeTransform, Controller.transform);
            Remove = OldPath.Length;
            FullPath = null;
        }

        public static void RepathName()
        {
            OldName = NewName;
        }
    }
}