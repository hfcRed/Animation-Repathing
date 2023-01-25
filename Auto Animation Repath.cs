using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Animations;
using UnityEngine.Animations;
using System.Linq;


namespace AutoAnimationRepath
{
    public class AutoAnimationRepathWindow : EditorWindow
    {
        //Settings values
        bool Toggle;
        string[] BaseOptions = { "Custom Controller", "VRChat Avatar" };
        string [] AvatarOptions = { "Animator Component", "Base Layer", "Additive Layer", "Gesture Layer", "Action Layer", "FX Layer" };
        int BaseSelection;
        int AvatarSelection;

        bool RenameAvtive = true;
        bool ReparentActive = true;
        bool RenameWarning = true;
        bool ReparentWarning = true;

        //UI values
        Vector2 Scroll = Vector2.zero;
        AnimatorController Controller;
        GameObject Avatar;

        //Create window
        [MenuItem("hfcRed/Auto Repath")]
        static void ShowWindow()
        {
            AutoAnimationRepathWindow Window = GetWindow<AutoAnimationRepathWindow>(false, "", true);
            Window.titleContent.image = EditorGUIUtility.IconContent("AnimationClip Icon").image;
        }

        //Save settings to disk
        public void SaveData()
        {
            //Save directly
            EditorPrefs.SetBool("AAR Toggle", Toggle);
            EditorPrefs.SetInt("AAR BaseSelection", BaseSelection);
            EditorPrefs.SetInt("AAR AvatarSelection", AvatarSelection);
            EditorPrefs.SetBool("AAR RenameActive", RenameAvtive);
            EditorPrefs.SetBool("AAR ReparentActive", ReparentActive);
            EditorPrefs.SetBool("AAR RenameWarning", RenameWarning);
            EditorPrefs.SetBool("AAR ReparentWarning", ReparentWarning);

            //Get GUID of custom controller to save as string
            if(Controller != null)
            {
                string ControllerGUID = AssetDatabase.GetAssetPath(Controller);
                EditorPrefs.SetString("AAR Controller", ControllerGUID);
            }
            else
            {
                EditorPrefs.SetString("AAR Controller", null);
            }

            //Get avatar name to save as string
            if(Avatar != null)
            {
                EditorPrefs.SetString("AAR Avatar", Avatar.name);
            }
            else
            {
                EditorPrefs.SetString("AAR Avatar", null);
            }
        }

        //Load settings on window open
        public void Awake()
        {
            //Load directly
            Toggle = EditorPrefs.GetBool("AAR Toggle", Toggle);
            BaseSelection = EditorPrefs.GetInt("AAR BaseSelection", BaseSelection);
            AvatarSelection = EditorPrefs.GetInt("AAR AvatarSelection", AvatarSelection);
            RenameAvtive = EditorPrefs.GetBool("AAR RenameActive", RenameAvtive);
            ReparentActive = EditorPrefs.GetBool("AAR ReparentActive", ReparentActive);
            RenameWarning = EditorPrefs.GetBool("AAR RenameWarning", RenameWarning);
            ReparentWarning = EditorPrefs.GetBool("AAR ReparentWarning", ReparentWarning);

            //Load GUID of custom controller to load the asset
            string FindController = EditorPrefs.GetString("AAR Controller");
            Controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(FindController);

            //Load avatar name to load the gameobject
            string FindAvatar = EditorPrefs.GetString("AAR Avatar");
            Avatar = GameObject.Find(FindAvatar);

        }

        //Create UI styles
        public static class UIStyles
        {
            public static GUIStyle Title = new GUIStyle(GUI.skin.label) { richText = true };
            public static GUIStyle ToggleButton = new GUIStyle(GUI.skin.button) { fontSize = 16, richText = true };
            public static GUIStyle Settings = new GUIStyle(GUI.skin.label) { fontSize = 20, richText = true };
            public static GUIStyle SettingsButtons = new GUIStyle(GUI.skin.label) { richText = true };
        }

        //Create UI contents
        public static class UIContent
        {
            public static GUIContent LinkIcon = new GUIContent(EditorGUIUtility.IconContent("UnityEditor.FindDependencies"));
            public static GUIContent ResetIcon = new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh"));
        }

        //Draw UI
        void OnGUI()
        {

            //Check if any settings changed, then call SaveData to save all settings
            using (var Change = new EditorGUI.ChangeCheckScope())
            {

                //Draw scrollbar
                Scroll = GUILayout.BeginScrollView(Scroll, false, true, GUILayout.Width(position.width));
                GUILayout.Space(10);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Space(5);
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {

                        //Draw title and credits
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label("<b>Auto Animation Repathing</b>", UIStyles.Title, GUILayout.Height(20));
                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("Made by hfcRed", "linkLabel"))
                            {
                                Application.OpenURL("https://hfcred.carrd.co");
                            }
                            if (GUILayout.Button(UIContent.LinkIcon, "label", GUILayout.Height(21)))
                            {
                                Application.OpenURL("https://hfcred.carrd.co");
                            }
                            GUILayout.Space(5);
                        }
                    }
                    GUILayout.Space(5);

                    //Draw toggle button
                    if (Toggle == false)
                    {
                        Toggle = GUILayout.Toggle(Toggle, "<color=#969696><b>Disabled</b></color>", UIStyles.ToggleButton, GUILayout.Height(30));
                    }
                    else
                    {
                        Toggle = GUILayout.Toggle(Toggle, "<color=#2bff80><b>Enabled</b></color>", UIStyles.ToggleButton, GUILayout.Height(30));
                    }
                    GUILayout.Space(5);
                }
                GUILayout.Space(20);

                //Draw settings
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Space(5);
                    using (new GUILayout.HorizontalScope())
                    {
                        using (new EditorGUILayout.VerticalScope(EditorStyles.boldLabel))
                        {
                            GUILayout.Space(3);
                            GUILayout.Label("<b>Settings</b>", UIStyles.Settings);
                        }

                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(UIContent.ResetIcon, GUILayout.Height(30), GUILayout.Width(30)))
                        {
                            Toggle = false;
                            BaseSelection = 0;
                            AvatarSelection = 0;

                            RenameAvtive = true;
                            ReparentActive = true;
                            RenameWarning = true;
                            ReparentWarning = true;

                            Controller = null;
                            Avatar = null;

                            SaveData();
                        }
                        GUILayout.Space(5);
                    }
                    GUILayout.Space(5);

                    //Draw controller settings
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {

                        GUILayout.Space(5);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(5);
                            GUILayout.Label("Target", GUILayout.Width(150));
                            BaseSelection = EditorGUILayout.Popup(BaseSelection, BaseOptions);
                            GUILayout.Space(5);
                        }

                        GUILayout.Space(5);
                        if (BaseSelection == 0)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.Space(5);
                                GUILayout.Label("Controller to use", GUILayout.Width(150));
                                Controller = (AnimatorController)EditorGUILayout.ObjectField(Controller, typeof(AnimatorController), true);
                                GUILayout.Space(5);
                            }
                        }
                        else
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.Space(5);
                                GUILayout.Label("Avatar to use", GUILayout.Width(150));
                                Avatar = (GameObject)EditorGUILayout.ObjectField(Avatar, typeof(GameObject), true);
                                GUILayout.Space(5);
                            }

                            GUILayout.Space(5);
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.Space(5);
                                GUILayout.Label("Layer to target", GUILayout.Width(150));
                                AvatarSelection = EditorGUILayout.Popup(AvatarSelection, AvatarOptions);
                                GUILayout.Space(5);
                            }
                        }
                        GUILayout.Space(5);
                    }

                    //Draw misc settings
                    GUILayout.Space(10);
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        GUILayout.Space(5);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(5);
                            RenameAvtive = GUILayout.Toggle(RenameAvtive, "Repath when renamed");
                            ReparentActive = GUILayout.Toggle(ReparentActive, "Repath when reparented");
                            GUILayout.Space(5);
                        }

                        GUILayout.Space(5);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Space(5);
                            RenameWarning = GUILayout.Toggle(RenameWarning, "Warn when renamed");
                            ReparentWarning = GUILayout.Toggle(ReparentWarning, "Warn when reparented");
                            GUILayout.Space(5);
                        }
                        GUILayout.Space(5);
                    }
                    GUILayout.Space(10);
                }
                GUILayout.EndScrollView();

                if(Change.changed)
                {
                    SaveData();
                }
            }            
        }
    }
}
