using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Animations;
using UnityEngine.Animations;
using System.Linq;
using VRC.SDK3.Avatars.Components;


namespace AutoAnimationRepath
{
    public class AAREditor : EditorWindow
    {
        internal sealed class BGColoredScope : System.IDisposable
        {
            private readonly Color ogColor;

            public BGColoredScope(bool isActive, Color active, Color inactive)
            {
                ogColor = GUI.backgroundColor;
                GUI.backgroundColor = isActive ? active : inactive;
            }
            public void Dispose()
            {
                GUI.backgroundColor = ogColor;
            }
        }

        #region Window
        //Create window
        [MenuItem("hfcRed/Auto Repath")]
        static void ShowWindow()
        {
            AAREditor Window = GetWindow<AAREditor>(false, "", true);
            Window.titleContent.image = EditorGUIUtility.IconContent("AnimationClip Icon").image;
        }
        #endregion

        #region UI variables
        //Create UI styles
        public static class UIStyles
        {
            public static GUIStyle Title = new GUIStyle(GUI.skin.label) { fontSize = 15, richText = true };
            public static GUIStyle ToggleButton = new GUIStyle(GUI.skin.button) { fontSize = 16, richText = true };
            public static GUIStyle Settings = new GUIStyle(GUI.skin.label) { fontSize = 20, richText = true };
            public static GUIStyle SettingsButtons = new GUIStyle(GUI.skin.label) { richText = true };
        }

        //Create UI contents
        public static class UIContent
        {
            public static GUIContent LinkIcon = new GUIContent(EditorGUIUtility.IconContent("UnityEditor.FindDependencies"));
            public static GUIContent ResetIcon = new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh")) { tooltip = "Reset settings to default values" };
        }

        Vector2 Scroll = Vector2.zero;
        #endregion

        #region UI
        //Draw UI
        void OnGUI()
        {

            //Check if any settings changed, then call SaveData to save all settings
            using (var Change = new EditorGUI.ChangeCheckScope())
            {
                
                //Draw scrollbar
                Scroll = GUILayout.BeginScrollView(Scroll, false, true, GUILayout.Width(position.width));

                GUILayout.Space(10);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(10);
                    AARAutomatic.ToolSelection = GUILayout.Toolbar(AARAutomatic.ToolSelection, AARAutomatic.Tools);
                    GUILayout.Space(10);
                }

                //Draw UI of the selected tool
                if(AARAutomatic.ToolSelection == 0)
                {
                    #region Automatic
                    GUILayout.Space(10);
                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        GUILayout.Space(10);
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                        {

                            //Draw title and credits
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Space(5);
                                using (new EditorGUILayout.VerticalScope())
                                {
                                    GUILayout.Space(5);
                                    GUILayout.Label("<b>Auto Animation Repathing</b>", UIStyles.Title, GUILayout.Height(20));
                                    GUILayout.Space(5);
                                }
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
                        GUILayout.Space(10);

                        //Draw toggle button                      
                        if (AARAutomatic.Toggle == false)
                        {
                            AARAutomatic.Toggle = GUILayout.Toggle(AARAutomatic.Toggle, "<color=#969696><b>Disabled</b></color>", UIStyles.ToggleButton, GUILayout.Height(30));
                        }
                        else
                        {
                            AARAutomatic.Toggle = GUILayout.Toggle(AARAutomatic.Toggle, "<color=#2bff80><b>Enabled</b></color>", UIStyles.ToggleButton, GUILayout.Height(30));
                        }
                        
                        //using (new BGColoredScope(AARAutomatic.Toggle, Color.green, Color.gray))
                        //AARAutomatic.Toggle = GUILayout.Toggle(AARAutomatic.Toggle, "<color=#2bff80><b>Enabled</b></color>", UIStyles.ToggleButton, GUILayout.Height(30));

                        GUILayout.Space(10);
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
                                GUILayout.Space(5);
                                GUILayout.Label("<b>Settings</b>", UIStyles.Settings);
                            }

                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button(UIContent.ResetIcon, GUILayout.Height(30), GUILayout.Width(30)))
                            {
                                AARAutomatic.BaseSelection = 0;

                                AARAutomatic.Controller = null;
                                AARAutomatic.Avatar = null;

                                AARAutomatic.Toggle = false;
                                AARAutomatic.RenameAvtive = true;
                                AARAutomatic.ReparentActive = true;
                                AARAutomatic.RenameWarning = true;
                                AARAutomatic.ReparentWarning = true;
                                AARAutomatic.ActiveInBackground = false;

                                AARAutomatic.PlayableSelection = AARAutomatic.Playables.all;

                                AARAutomatic.SaveData();
                            }
                            GUILayout.Space(5);
                        }
                        GUILayout.Space(15);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(5)))
                            { }
                            using (new EditorGUILayout.VerticalScope())
                            {
                                //Draw controller settings
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                                    {
                                        GUILayout.Space(5);
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            GUILayout.Space(5);
                                            GUILayout.Label("Target", GUILayout.Width(150));
                                            AARAutomatic.BaseSelection = EditorGUILayout.Popup(AARAutomatic.BaseSelection, AARAutomatic.BaseOptions);
                                            GUILayout.Space(5);
                                        }

                                        GUILayout.Space(5);
                                        if (AARAutomatic.BaseSelection == 0)
                                        {
                                            using (new EditorGUILayout.HorizontalScope())
                                            {
                                                GUILayout.Space(5);
                                                GUILayout.Label("Controller to use", GUILayout.Width(150));
                                                AARAutomatic.Controller = (Animator)EditorGUILayout.ObjectField(AARAutomatic.Controller, typeof(Animator), true);
                                                GUILayout.Space(5);
                                            }
                                        }
                                        else
                                        {
                                            using (new EditorGUILayout.HorizontalScope())
                                            {
                                                GUILayout.Space(5);
                                                GUILayout.Label("Avatar to use", GUILayout.Width(150));
                                                AARAutomatic.Avatar = (GameObject)EditorGUILayout.ObjectField(AARAutomatic.Avatar, typeof(GameObject), true);
                                                GUILayout.Space(5);
                                            }

#if VRC_SDK_VRCSDK3
                                            GUILayout.Space(5);
                                            if (AARAutomatic.Avatar != null && AARAutomatic.Avatar.GetComponent<VRCAvatarDescriptor>() != null)
                                            {
                                                using (new EditorGUILayout.HorizontalScope())
                                                {
                                                    GUILayout.Space(5);
                                                    GUILayout.Label("Layer to target", GUILayout.Width(150));
                                                    AARAutomatic.PlayableSelection = (AARAutomatic.Playables)EditorGUILayout.EnumFlagsField(AARAutomatic.PlayableSelection);
                                                    GUILayout.Space(5);

                                                    if (AARAutomatic.ShowDebug == true)
                                                    {
                                                        int PlayableSelectionAsInt;
                                                        PlayableSelectionAsInt = (int)AARAutomatic.PlayableSelection;
                                                        GUILayout.Space(5);
                                                        GUILayout.Label("Int", GUILayout.Width(45));
                                                        EditorGUILayout.IntField(PlayableSelectionAsInt);
                                                        GUILayout.Space(5);
                                                    }
                                                }
                                            }
#endif
                                        }
                                        GUILayout.Space(5);
                                    }

                                }
                                //Draw misc settings
                                GUILayout.Space(10);
                                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                                {
                                    GUILayout.Space(5);
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.Space(5);
                                        AARAutomatic.RenameAvtive = GUILayout.Toggle(AARAutomatic.RenameAvtive, "Repath when renamed");
                                        AARAutomatic.ReparentActive = GUILayout.Toggle(AARAutomatic.ReparentActive, "Repath when reparented");
                                        GUILayout.Space(5);
                                    }

                                    GUILayout.Space(5);
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.Space(5);
                                        AARAutomatic.RenameWarning = GUILayout.Toggle(AARAutomatic.RenameWarning, "Warn when renamed");
                                        AARAutomatic.ReparentWarning = GUILayout.Toggle(AARAutomatic.ReparentWarning, "Warn when reparented");
                                        GUILayout.Space(5);
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
                                        AARAutomatic.ActiveInBackground = GUILayout.Toggle(AARAutomatic.ActiveInBackground, "Run when window is closed");
                                        GUILayout.Space(5);
                                    }
                                    GUILayout.Space(5);
                                }

                                //Draw language settings
                                GUILayout.Space(10);
                                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                                {
                                    GUILayout.Space(5);
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        GUILayout.Space(5);
                                        GUILayout.Label("Language", GUILayout.Width(150));
                                        AARAutomatic.LanguageSelection = EditorGUILayout.Popup(AARAutomatic.LanguageSelection, AARAutomatic.LanguageOptions);
                                        GUILayout.Space(5);
                                    }
                                    GUILayout.Space(5);
                                }
                                GUILayout.Space(10);
                            }
                            using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(5)))
                            { }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Manual

                    #endregion
                }
                GUILayout.EndScrollView();

                if(Change.changed)
                {
                    AARAutomatic.SaveData();
                }
            }            
        }
        #endregion
    }
}

//var color = GUI.backgroundColor;
//GUI.backgroundColor = Color.green;
//GUI.backgroundColor = color;