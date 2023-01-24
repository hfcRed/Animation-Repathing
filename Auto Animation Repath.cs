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
        bool Toggle = true;
        int BaseSelection = 0;
        int AvatarSelection = 0;
        string[] BaseOptions = { "Custom Controller", "VRChat Avatar" };
        string [] AvatarOptions = { "Animator Component", "Base Layer", "Additive Layer", "Gesture Layer", "Action Layer", "FX Layer" };

        bool RenameAvtive = true;
        bool ReparentActive = true;
        bool RenameWarning = true;
        bool ReparentWarning = true;

        Vector2 Scroll = Vector2.zero;
        AnimatorController Controller;
        GameObject Avatar;

        [MenuItem("hfcRed/Auto Repath")]
        static void ShowWindow()
        {
            GetWindow<AutoAnimationRepathWindow>(false, "AARP", true).titleContent.image = EditorGUIUtility.IconContent("AnimationClip Icon").image;
        }

        public static class UIStyles
        {
            public static GUIStyle Title = new GUIStyle(GUI.skin.label) { richText = true };
            public static GUIStyle ToggleButton = new GUIStyle(GUI.skin.button) { fontSize = 16, richText = true };
            public static GUIStyle Settings = new GUIStyle(GUI.skin.label) { fontSize = 15, richText = true };
            public static GUIStyle SettingsButtons = new GUIStyle(GUI.skin.label) { richText = true };
        }
        public static class UIContent
        {
            public static GUIContent LinkIcon = new GUIContent(EditorGUIUtility.IconContent("UnityEditor.FindDependencies"));
        }

        void OnGUI()
        {
            Scroll = GUILayout.BeginScrollView(Scroll, false, true, GUILayout.Width(position.width));
            GUILayout.Space(10);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Space(5);
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
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

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Space(5);
                using (new EditorGUILayout.VerticalScope(EditorStyles.boldLabel))
                {
                    GUILayout.Label("<b>Settings</b>", UIStyles.Settings);
                }
                GUILayout.Space(5);

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
        }
    }
}
