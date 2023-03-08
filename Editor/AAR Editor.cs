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
        #region Window
        //Create window
        [MenuItem("hfcRed/Auto Repath")]
        static void ShowWindow() => GetWindow<AAREditor>(false, string.Empty, true).titleContent.image = EditorGUIUtility.IconContent("AnimationClip Icon").image;
        #endregion

        #region UI variables
        //Create UI styles
        public static class UIStyles
        {
            public static GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 15, richText = true };
            public static GUIStyle toggleButton = new GUIStyle(GUI.skin.button) { fontSize = 16, richText = true };
            public static GUIStyle settings = new GUIStyle(GUI.skin.label) { fontSize = 20, richText = true };
            public static GUIStyle settingsButtons = new GUIStyle(GUI.skin.label) { richText = true };
            public static GUIStyle invalidPath = new GUIStyle(GUI.skin.box) { fontSize= 12, richText = true, stretchWidth = true, alignment = TextAnchor.MiddleLeft };
        }

        //Create UI contents
        public static class UIContent
        {
            public static GUIContent linkIcon = new GUIContent(EditorGUIUtility.IconContent("UnityEditor.FindDependencies"));
            public static GUIContent resetIcon = new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh")) { tooltip = "Reset settings to default values" };
            public static GUIContent test = new GUIContent() { tooltip = "test" };
        }

        private static Vector2 scroll = Vector2.zero;
        #endregion

        #region UI
        //Draw UI
        public void OnGUI()
        {
            //Check if any settings changed, then call SaveData to save all settings
            EditorGUI.BeginChangeCheck();

            //Draw scrollbar
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(position.width));

            GUILayout.Space(10);

            using (new SqueezeScope(10))
                AARAutomatic.toolSelection = GUILayout.Toolbar(AARAutomatic.toolSelection, AARAutomatic.tools);
     

            //Draw UI of the selected tool
            switch (AARAutomatic.toolSelection)
            {
                case 0: DrawAutomaticGUI(); break;
                case 1: DrawManualGUI(); break;
            }

            DrawSettings();

            GUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
                AARAutomatic.SaveData();
        }

        public static void DrawAutomaticGUI()
        {
            using (new SqueezeScope(10, 20, 0))
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new SqueezeScope(10, 10, 2))
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {

                    //Draw title and credits
                    using (new SqueezeScope(5))
                    {
                        using (new SqueezeScope(5, 5, 4))
                            GUILayout.Label("<b>Auto Animation Repathing</b>", UIStyles.title, GUILayout.Height(20));
                        

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Made by hfcRed", "linkLabel"))
                            Application.OpenURL("https://hfcred.carrd.co");
                        
                        if (GUILayout.Button(UIContent.linkIcon, "label", GUILayout.Height(21)))
                            Application.OpenURL("https://hfcred.carrd.co");                        
                    }
                }

                //Draw toggle button
                string buttonLabel = AARAutomatic.isEnabled ? "<color=#2bff80><b>Enabled</b></color>" : "<color=#969696><b>Disabled</b></color>";
                AARAutomatic.isEnabled = GUILayout.Toggle(AARAutomatic.isEnabled, buttonLabel, UIStyles.toggleButton, GUILayout.Height(30));

                GUILayout.Space(10);

                //using (new BGColoredScope(AARAutomatic.Toggle, Color.green, Color.gray))
                //AARAutomatic.Toggle = GUILayout.Toggle(AARAutomatic.Toggle, "<color=#2bff80><b>Enabled</b></color>", UIStyles.ToggleButton, GUILayout.Height(30));
            }

        }

        public static void DrawManualGUI()
        {
            using (new SqueezeScope(10, 10, 0))
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new SqueezeScope(10, 10, 2))
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {

                    //Draw title and credits
                    using (new SqueezeScope(5))
                    {
                        using (new SqueezeScope(5, 5, 4))
                            GUILayout.Label("<b>Manual Animation Repathing</b>", UIStyles.title, GUILayout.Height(20));


                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Made by hfcRed", "linkLabel"))
                            Application.OpenURL("https://hfcred.carrd.co");

                        if (GUILayout.Button(UIContent.linkIcon, "label", GUILayout.Height(21)))
                            Application.OpenURL("https://hfcred.carrd.co");
                    }
                }
            }

            using (new SqueezeScope(0, 20, 0))
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new SqueezeScope(5, 5, 0))
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new SqueezeScope(5, 5, 0))
                            AARAutomatic.foldout = EditorGUILayout.Foldout(AARAutomatic.foldout, "Invalid Paths");
                    }

                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new SqueezeScope(5, 5, 0))
                            if (GUILayout.Button("Scan Animator", GUILayout.Width(116)))
                            {
                                AARManual.ScanPaths();
                            }
                    }
                }
                if (AARAutomatic.foldout == true)
                {
                    AARManual.position = -1;
                    foreach (string str in AARManual.invalidPaths)
                    {
                        AARManual.position++;

                        using (new SqueezeScope(0, 10, 0))
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new SqueezeScope(5, 5, 0))
                            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                            {
                                using (new EditorGUILayout.VerticalScope())
                                {
                                    using (new SqueezeScope(5, 0, 0))
                                    {
                                       using (new EditorGUILayout.HorizontalScope())
                                       {
                                            using (new SqueezeScope(5, 5, 0))
                                            using (new EditorGUILayout.VerticalScope())
                                            {
                                                GUILayout.Label("<color=#F0DE08><b>" + str + "</b></color>", UIStyles.invalidPath);

                                                using (new SqueezeScope(5, 5, 0))
                                                using (new EditorGUILayout.HorizontalScope())
                                                {
                                                    var hover = string.Empty;

                                                    AARManual.newPaths[AARManual.position] = GUILayout.TextField(AARManual.newPaths[AARManual.position], GUILayout.MaxWidth(300));

                                                    GUILayout.FlexibleSpace();
                                                    if (GUILayout.Button("Apply", GUILayout.Width(116)) && AARManual.newPaths[AARManual.position] != string.Empty)
                                                    {
                                                        AARManual.RenamePath();
                                                    }
                                                }

                                                using (new SqueezeScope(0, 5, 0))
                                                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                                                {
                                                    using (new EditorGUILayout.VerticalScope())
                                                    {
                                                        AARManual.foldouts[AARManual.position] = EditorGUILayout.Foldout(AARManual.foldouts[AARManual.position], "Affected Clips" + " (" + AARManual.invalidClips[AARManual.position].Count + ")");
                                                        if (AARManual.foldouts[AARManual.position] == true)
                                                        {
                                                            foreach (AnimationClip clip in AARManual.invalidClips[AARManual.position])
                                                            {
                                                                EditorGUI.BeginDisabledGroup(true);
                                                                EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);
                                                                EditorGUI.EndDisabledGroup();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                       }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DrawSettings()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Space(5);
                using (new GUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope(EditorStyles.boldLabel))
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("<b>Settings</b>", UIStyles.settings);
                    }

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(UIContent.resetIcon, GUILayout.Height(30), GUILayout.Width(30)))
                    {
                        AARAutomatic.baseSelection = 0;

                        AARAutomatic.animator = null;
                        AARAutomatic.avatar = null;

                        AARAutomatic.isEnabled = false;
                        AARAutomatic.renameActive = true;
                        AARAutomatic.reparentActive = true;
                        AARAutomatic.renameWarning = true;
                        AARAutomatic.reparentWarning = true;
                        AARAutomatic.activeInBackground = false;

                        AARAutomatic.PlayableSelection = AARAutomatic.Playables.all;

                        AARAutomatic.SaveData();
                    }
                    GUILayout.Space(5);
                }

                GUILayout.Space(15);

                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(5)))
                    {
                    }

                    using (new EditorGUILayout.VerticalScope())
                    {
                        //Draw controller settings
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new SqueezeScope(5, 5, 4, EditorStyles.helpBox))
                            {
                                using (new SqueezeScope(5))
                                {
                                    GUILayout.Label("Target", GUILayout.Width(150));
                                    AARAutomatic.baseSelection = EditorGUILayout.Popup(AARAutomatic.baseSelection, AARAutomatic.baseOptions);
                                }

                                GUILayout.Space(5);
                                if (AARAutomatic.baseSelection == 0)
                                {
                                    using (new SqueezeScope(5))
                                    {
                                        GUILayout.Label("Controller to use", GUILayout.Width(150));

                                        AARAutomatic.animator = (Animator)EditorGUILayout.ObjectField(AARAutomatic.animator, typeof(Animator), true);
                                    }
                                }
                                else
                                {
                                    using (new SqueezeScope(5))
                                    {
                                        GUILayout.Label("Avatar to use", GUILayout.Width(150));

                                        AARAutomatic.avatar = (GameObject)EditorGUILayout.ObjectField(AARAutomatic.avatar, typeof(GameObject), true);

                                    }

#if VRC_SDK_VRCSDK3
                                    if (AARAutomatic.avatar != null && AARAutomatic.avatar.GetComponent<VRCAvatarDescriptor>() != null)
                                    {
                                        GUILayout.Space(5);
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            using (new SqueezeScope(5, 5, 0))
                                            {
                                                GUILayout.Label("Layer to target", GUILayout.Width(150));
                                                AARAutomatic.PlayableSelection = (AARAutomatic.Playables)EditorGUILayout.EnumFlagsField(AARAutomatic.PlayableSelection);
                                            }

                                            if (AARAutomatic.showDebug)
                                            {
                                                int PlayableSelectionAsInt = (int)AARAutomatic.PlayableSelection;

                                                using (new SqueezeScope(5, 5, 0))
                                                {
                                                    GUILayout.Label("Int", GUILayout.Width(45));
                                                    EditorGUILayout.IntField(PlayableSelectionAsInt);
                                                }
                                            }
                                        }
                                    }
#endif
                                }
                            }
                        }

                        //Draw misc settings
                        GUILayout.Space(10);
                        using (new SqueezeScope(5, 5, 4, EditorStyles.helpBox))
                        {
                            using (new SqueezeScope(5, 5, 3))
                            {
                                AARAutomatic.renameActive = GUILayout.Toggle(AARAutomatic.renameActive, "Repath when renamed");
                                AARAutomatic.reparentActive = GUILayout.Toggle(AARAutomatic.reparentActive, "Repath when reparented");
                            }

                            GUILayout.Space(5);

                            using (new SqueezeScope(5, 5, 3))
                            {
                                AARAutomatic.renameWarning = GUILayout.Toggle(AARAutomatic.renameWarning, "Warn when renamed");
                                AARAutomatic.reparentWarning = GUILayout.Toggle(AARAutomatic.reparentWarning, "Warn when reparented");
                            }
                        }

                        GUILayout.Space(10);
                        using (new SqueezeScope(5, 5, 4, EditorStyles.helpBox))
                        using (new SqueezeScope(5, 5, 3))
                            AARAutomatic.activeInBackground = GUILayout.Toggle(AARAutomatic.activeInBackground, "Run when window is closed");



                        //Draw language settings
                        GUILayout.Space(10);
                        using (new SqueezeScope(5, 5, 4, EditorStyles.helpBox))
                        {
                            using (new SqueezeScope(5, 5, 3))
                            {
                                GUILayout.Label("Language", GUILayout.Width(150));
                                AARAutomatic.languageSelection = EditorGUILayout.Popup(AARAutomatic.languageSelection, AARAutomatic.languageOptions);
                            }
                        }
                        GUILayout.Space(10);
                    }

                    using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(5)))
                    {
                    }
                }
            }
        }
        #endregion
    }

    #region GUI Classes
    public class BGColoredScope : System.IDisposable
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

    public class SqueezeScope : System.IDisposable
    {
        private readonly int width1;
        private readonly int width2;
        private readonly int type;
        //0 = none
        //1 = Horizontal
        //2 = Vertical
        //3 = EditorH
        //4 = EditorV

        public SqueezeScope(int width1, int width2, int type = 1, GUIStyle style = null)
        {
            this.width1 = width1;
            this.width2 = width2;
            this.type = type;
            BeginSqueeze(style ?? GUIStyle.none);
        }

        public SqueezeScope(int width1, int type = 1, GUIStyle style = null)
        {
            this.width1 = width1;
            this.width2 = width1;
            this.type = type;
            BeginSqueeze(style ?? GUIStyle.none);
        }

        private void BeginSqueeze(GUIStyle style = null)
        {
            switch (type)
            {
                case 1: GUILayout.BeginHorizontal(style); break;
                case 2: GUILayout.BeginVertical(style); break;
                case 3: EditorGUILayout.BeginHorizontal(style); break;
                case 4: EditorGUILayout.BeginVertical(style); break;
            }

            GUILayout.Space(width1);
        }


        public void Dispose()
        {
            GUILayout.Space(width2);

            switch (type)
            {
                case 1: GUILayout.EndHorizontal(); break;
                case 2: GUILayout.EndVertical(); break;
                case 3: EditorGUILayout.EndHorizontal(); break;
                case 4: EditorGUILayout.EndVertical(); break;
            }
        }
    }
    #endregion
}
