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
        public static string[] BaseOptions = { "Custom Controller", "VRChat Avatar" };
        public static int LanguageSelection;
        public static string[] LanguageOptions = { "English", "日本" };

        public static AnimatorController Controller;
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

        static AnimatorController TargetController;
        static Transform Selected;
        static Transform OldParent;
        static string OldName;
        static string NewName;

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

            //Get GUID of custom controller to save as string
            if (Controller != null)
            {
                string ControllerGUID = AssetDatabase.GetAssetPath(Controller);
                EditorPrefs.SetString("AAR Controller", ControllerGUID);
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

            AARAutomatic.LoadData();
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
            Controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(FindController);

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
            Debug.Log("Selection Changed");
            if (Selection.activeGameObject == null)
            {
                return;
            }
            Selected = Selection.activeTransform;
            OldParent = Selection.activeTransform.parent;
            OldName = Selection.activeGameObject.name;
        }
        #endregion

        #region Hierarchy Change
        //Run when anything in the hierarchy changes
        public static void HierarchyChanged()
        {
            if (EditorWindow.HasOpenInstances<AAREditor>() && Toggle == true || !EditorWindow.HasOpenInstances<AAREditor>() && ActiveInBackground == true)
            {
                if (ReparentActive == true && Selected.parent != null && OldParent != Selected.parent)
                {
                    if (ReparentWarning == true)
                    {
                        if (EditorUtility.DisplayDialog("Auto Repathing", "Repathing animation properties for " + Selected.name + " to " + Selected.parent.name, "Continue", "Cancel"))
                        {

                        }
                    }
                    else
                    {

                    }
                }

                if (RenameAvtive == true && OldName != Selected.name)
                {
                    if (RenameWarning == true)
                    {
                        if (EditorUtility.DisplayDialog("Auto Repathing", "Repathing animation properties from " + OldName + " to " + Selected.name, "Continue", "Cancel"))
                        {

                        }
                    }
                    else
                    {

                    }
                }
            }
        }
        #endregion
    }
}