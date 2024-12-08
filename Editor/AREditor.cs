using static AnimationRepathing.ARVariables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
#if VRC_AVATARS
using VRC.SDK3.Avatars.Components;
#endif

namespace AnimationRepathing
{
    public class AREditor : EditorWindow
    {
        [MenuItem("hfcRed/Tools/Animation Repathing")]
        public static void ShowWindow() => GetWindow<AREditor>("Animation Repathing", true).titleContent.image = EditorGUIUtility.IconContent("AnimationClip Icon").image;

        public static Vector2 scroll = Vector2.zero;

        public string GetScriptPath()
        {
            var script = MonoScript.FromScriptableObject(this);
            return AssetDatabase.GetAssetPath(script);
        }

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(position.width));

            DrawHeader();
            switch (toolSelection)
            {
                case 0: DrawAutomatic(); break;
                case 1: DrawManual(); break;
            }

            DrawDivider(28, 20, 5, 5);
            DrawSettings();

            GUILayout.EndScrollView();
            if (EditorGUI.EndChangeCheck())
            {
                ARSaveData.SaveData();
            }
        }

        public static void DrawHeader()
        {
            using (new SqueezeScope((5, 5, 3), (10, 5, 4)))
            {
                GUIContent[] content = { new GUIContent(ARStrings.Main.automatic, ARStrings.ToolTips.automatic), new GUIContent(ARStrings.Main.manual, ARStrings.ToolTips.manual) };
                content[0].image = EditorGUIUtility.IconContent("Profiler.Memory").image;
                content[1].image = EditorGUIUtility.IconContent("ViewToolMove").image;

                toolSelection = GUILayout.Toolbar(toolSelection, content, GUILayout.Height(25));
            }
        }

        public static void DrawAutomatic()
        {
            using (new SqueezeScope((5, 5, 3), (10, 0, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
            {
                using (new SqueezeScope((0, 0, 4, GUI.skin.box), (5, 5, 4), (5, 5, 3)))
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("Profiler.Memory").image, ""), GUILayout.Width(20), GUILayout.Height(25));
                    GUILayout.Label("<color=#ffffff><b>" + ARStrings.Automatic.title + "</b></color>", ARStyle.title);
                    GUILayout.FlexibleSpace();
                }

                using (new SqueezeScope((15, 0, 4)))
                {
                    GUI.color = automaticIsEnabled ? Color.green : Color.grey;

                    GUIContent content = automaticIsEnabled ? new GUIContent("<color=#4aff93><b>" + ARStrings.Automatic.enabled + "</b></color>", ARStrings.ToolTips.toggleButton) : new GUIContent("<color=#ff5263><b>" + ARStrings.Automatic.disabled + "</b></color>", ARStrings.ToolTips.toggleButton);
                    automaticIsEnabled = GUILayout.Toggle(automaticIsEnabled, content, ARStyle.toggleButton, GUILayout.Height(40));

                    GUI.color = Color.white;
                }
            }
        }

        public static void DrawManual()
        {
            using (new SqueezeScope((5, 5, 3), (10, 0, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
            {
                using (new SqueezeScope((0, 0, 4, GUI.skin.box), (5, 5, 4), (5, 5, 3)))
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("ViewToolMove").image, ""), GUILayout.Width(20), GUILayout.Height(25));
                    GUILayout.Label("<color=#ffffff><b>" + ARStrings.Manual.title + "</b></color>", ARStyle.title);
                    GUILayout.FlexibleSpace();
                }

                using (new SqueezeScope((15, 0, 4)))
                {
                    GUIContent[] content = { new GUIContent(ARStrings.Manual.fixPaths, ARStrings.ToolTips.fixPaths), new GUIContent(ARStrings.Manual.editClips, ARStrings.ToolTips.editClips) };
                    content[0].image = EditorGUIUtility.IconContent("winbtn_mac_min").image;
                    content[1].image = EditorGUIUtility.IconContent("Motion Icon").image;
                    manualToolSelection = GUILayout.Toolbar(manualToolSelection, content, GUILayout.Height(25));
                }

                switch (manualToolSelection)
                {
                    case 0: DrawManualInvalidPaths(); break;
                    case 1: DrawManualClipEditing(); break;
                }
            }
        }

        public static void DrawManualInvalidPaths()
        {
            using (new SqueezeScope((15, 0, 4)))
            {
                using (new SqueezeScope((0, 5, 4), (0, 0, 4, GUI.skin.box), (5, 5, 3)))
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(ARStrings.InvalidPaths.invalidPaths), ARStyle.foldout);
                    GUILayout.FlexibleSpace();
                }

                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, ARStrings.ToolTips.resetInvalidPaths), GUILayout.Height(25)))
                {
                    ARManual.InvalidPaths.ScanInvalidPaths(GetControllers().ToArray());
                }
            }

            for (int i = 0; i < invalidPathToSharedProperty.Values.Count; i++)
            {
                InvalidSharedProperty sp = invalidPathToSharedProperty.Values.ElementAt(i);

                string str = sp.oldPath;

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, GUI.skin.box), (5, 5, 3)))
                {
                    GUILayout.Label(new GUIContent("<color=#F0DE08><b>" + str + "</b></color>"), ARStyle.invalidPath, GUILayout.MinWidth(1));
                }

                using (new SqueezeScope((0, 0, 4), (0, 0, 3)))
                {
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, ARStrings.ToolTips.resetInvalidPath), ARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        sp.newPath = sp.oldPath;
                    }

                    GUIContent content = sp.foldout ? new GUIContent(EditorGUIUtility.IconContent("IN foldout act on").image, "") : new GUIContent(EditorGUIUtility.IconContent("IN foldout act").image, "");
                    sp.foldout = GUILayout.Toggle(sp.foldout, content, new GUIStyle(GUI.skin.button), GUILayout.Width(25));

                    GUILayout.Space(5);

                    sp.newPath = GUILayout.TextField(sp.newPath, GUILayout.MinWidth(1));
                    string path = DragAndDropGameobject();
                    if (path != null)
                    {
                        sp.newPath = path;
                    }

                    GUILayout.Space(5);

                    if (sp.newPath == string.Empty || sp.newPath == sp.oldPath)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                    }
                    if (GUILayout.Button(new GUIContent(ARStrings.InvalidPaths.apply, ARStrings.ToolTips.applyValidPath), GUILayout.Width(75)))
                    {
                        ARManual.InvalidPaths.RenameInvalidPaths(sp.foldoutClips.ToArray(), sp.oldPath, sp.newPath);
                    }
                    EditorGUI.EndDisabledGroup();
                }

                if (sp.foldout == true)
                {
                    using (new SqueezeScope((5, 5, 4)))
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        foreach (AnimationClip clip in sp.foldoutClips)
                        {
                            EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                }
            }

            using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
            {
                if (invalidPathToSharedProperty.Count == 0)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("TestPassed").image, ""), GUILayout.Width(20), GUILayout.Height(21));
                    GUILayout.Label(ARStrings.InvalidPaths.noInvalidPaths, ARStyle.invalidPathTip, GUILayout.Height(20));
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("GameObject Icon").image, ""), GUILayout.Width(20), GUILayout.Height(21));
                    GUILayout.Label(ARStrings.InvalidPaths.dragAndDrop, ARStyle.invalidPathTip, GUILayout.Height(20));
                    GUILayout.FlexibleSpace();
                }
            }
        }

        public static void DrawManualClipEditing()
        {
            if (clipsSelected.Count != 0)
            {
                using (new SqueezeScope((15, 0, 4)))
                {
                    DrawReplacePartOfAll();
                    DrawDivider(20, 15, 0, 0);
                    DrawReplaceIndividual();
                }
            }

            using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
            {
                if (clipsSelected.Count == 0)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("AnimationClip Icon").image, ""), GUILayout.Width(20), GUILayout.Height(21));
                    GUILayout.Label(ARStrings.ClipEditing.noClipsSelected, ARStyle.invalidPathTip, GUILayout.Height(20));
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("GameObject Icon").image, ""), GUILayout.Width(20), GUILayout.Height(21));
                    GUILayout.Label(ARStrings.ClipEditing.dragAndDrop, ARStyle.invalidPathTip, GUILayout.Height(20));
                    GUILayout.FlexibleSpace();
                }
            }
        }

        public static void DrawReplacePartOfAll()
        {
            using (new SqueezeScope((0, 0, 4, GUI.skin.box), (5, 5, 3)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(new GUIContent(ARStrings.ClipEditing.replacePartOfAll, ARStrings.ToolTips.replacePartOfAll), ARStyle.replacePath);
                GUILayout.FlexibleSpace();
            }

            using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
            {
                GUILayout.Label("", GUILayout.Width(55));
                GUILayout.Label(new GUIContent(ARStrings.ClipEditing.from, ARStrings.ToolTips.replaceFrom), ARStyle.replaceFromTo);
                GUILayout.Label(new GUIContent(ARStrings.ClipEditing.to, ARStrings.ToolTips.replaceTo), ARStyle.replaceFromTo);
                GUILayout.Label("", GUILayout.Width(140));
            }

            bool warning = false;
            int countTotal = 0;
            List<AnimationClip> clipsTarget = new List<AnimationClip>();

            using (new SqueezeScope((5, 5, 4), (0, 0, 3)))
            {
                for (int i = 0; i < clipsPathToSharedProperty.Values.Count; i++)
                {
                    ClipsSharedProperty sp = clipsPathToSharedProperty.Values.ElementAt(i);

                    sp.warning = false;

                    if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty && sp.oldPath.Contains(clipsReplaceFrom))
                    {
                        countTotal += sp.count;

                        var checkDuplicates = sp.oldPath.IndexOf(clipsReplaceFrom);
                        if (checkDuplicates != sp.oldPath.LastIndexOf(clipsReplaceFrom))
                        {
                            warning = true;
                            sp.warning = true;
                        }

                        foreach (AnimationClip clip in sp.foldoutClips)
                        {
                            if (!clipsTarget.Contains(clip))
                            {
                                clipsTarget.Add(clip);
                            }
                        }
                    }
                }

                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, ARStrings.ToolTips.resetPartOfAll), ARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    clipsReplaceFrom = string.Empty;
                    clipsReplaceTo = string.Empty;
                }

                GUIContent content = clipsReplaceFoldout ? new GUIContent(EditorGUIUtility.IconContent("IN foldout act on").image, "") : new GUIContent(EditorGUIUtility.IconContent("IN foldout act").image, "");
                clipsReplaceFoldout = GUILayout.Toggle(clipsReplaceFoldout, content, new GUIStyle(GUI.skin.button), GUILayout.Width(25));

                GUILayout.Space(5);

                clipsReplaceFrom = EditorGUILayout.TextField(clipsReplaceFrom, GUILayout.MinWidth(1));
                string path = DragAndDropGameobject();
                if (path != null)
                {
                    clipsReplaceFrom = path;
                }

                clipsReplaceTo = EditorGUILayout.TextField(clipsReplaceTo, GUILayout.MinWidth(1));
                string path2 = DragAndDropGameobject();
                if (path2 != null)
                {
                    clipsReplaceTo = path2;
                }

                GUILayout.Space(5);

                if (clipsReplaceFrom == string.Empty || clipsReplaceTo == string.Empty || clipsReplaceFrom == clipsReplaceTo || countTotal == 0)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }
                if (GUILayout.Button(new GUIContent(ARStrings.ClipEditing.apply, ARStrings.ToolTips.applyPartOfAll), GUILayout.Width(75)))
                {
                    ARManual.ClipEditing.RenameClipPaths(clipsTarget.ToArray(), false, clipsReplaceFrom, clipsReplaceTo);
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.Label("(" + countTotal.ToString() + ")", GUILayout.Width(41));
            }

            if (clipsReplaceFoldout == true)
            {
                using (new SqueezeScope((0, 0, 4)))
                {
                    EditorGUI.BeginDisabledGroup(true);
                    foreach (AnimationClip clip in clipsTarget)
                    {
                        EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }

            using (new SqueezeScope((0, 0, 4), (0, 0, 3)))
            {
                if (warning == true)
                {
                    EditorGUILayout.HelpBox(ARStrings.ClipEditing.warning, MessageType.Warning);
                }
            }
        }

        public static void DrawReplaceIndividual()
        {
            using (new SqueezeScope((0, 10, 4), (0, 0, 4, GUI.skin.box), (5, 5, 3)))
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(new GUIContent(ARStrings.ClipEditing.replaceIndividual, ARStrings.ToolTips.replaceIndividual), ARStyle.replacePath);
                GUILayout.FlexibleSpace();
            }

            for (int i = 0; i < clipsPathToSharedProperty.Values.Count; i++)
            {
                ClipsSharedProperty sp = clipsPathToSharedProperty.Values.ElementAt(i);

                string str = sp.oldPath;

                using (new SqueezeScope((0, 0, 4), (0, 0, 3)))
                {
                    if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty)
                    {
                        sp.newPath = sp.oldPath;

                        if (sp.warning == true)
                        {
                            GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("console.warnicon")), GUILayout.Width(20), GUILayout.Height(20));
                        }
                        else if (sp.oldPath.Contains(clipsReplaceFrom))
                        {
                            GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("TestPassed")), GUILayout.Width(20), GUILayout.Height(20));
                        }
                        else
                        {
                            GUILayout.Label("", GUILayout.Width(20), GUILayout.Height(20));
                        }
                    }
                    else if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, ARStrings.ToolTips.resetIndividual), ARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        sp.newPath = sp.oldPath;
                    }

                    GUIContent content = sp.foldout ? new GUIContent(EditorGUIUtility.IconContent("IN foldout act on").image, "") : new GUIContent(EditorGUIUtility.IconContent("IN foldout act").image, "");
                    sp.foldout = GUILayout.Toggle(sp.foldout, content, new GUIStyle(GUI.skin.button), GUILayout.Width(25));

                    Color contentClr = GUI.contentColor;
                    Color rectColor = new Color(0.55f, 0.55f, 0f);

                    if (clipsReplaceFrom != string.Empty && sp.oldPath.Contains(clipsReplaceFrom))
                    {
                        string[] s = sp.oldPath.Split(new string[] { clipsReplaceFrom }, StringSplitOptions.None);
                        StringBuilder stringBuilder = new StringBuilder();

                        for (int ii = 0; ii < s.Length; ii++)
                        {
                            if (ii != 0)
                            {
                                if (s.Length < 3)
                                {
                                    stringBuilder.Append("<color=#00ffff><b>");
                                }
                                else stringBuilder.Append("<color=#ff0000><b>");
                                stringBuilder.Append(clipsReplaceFrom);
                                stringBuilder.Append("</b></color>");
                            }

                            stringBuilder.Append("<color=#00FF00>");
                            stringBuilder.Append(s[ii]);
                            stringBuilder.Append("</color>");
                        }

                        sp.newPath = stringBuilder.ToString();
                    }
                    else if (clipsReplaceFrom != string.Empty)
                    {
                        GUI.contentColor = Color.gray;
                    }
                    else if (sp.newPath.Contains("</color>"))
                    {
                        sp.newPath = sp.oldPath;
                    }

                    GUILayout.Space(5);

                    sp.newPath = GUILayout.TextField(sp.newPath, new GUIStyle(GUI.skin.textField) { richText = true }, GUILayout.MinWidth(1));
                    string path = DragAndDropGameobject();
                    if (path != null)
                    {
                        sp.newPath = path;
                    }
                    if (sp.invalid)
                    {
                        Rect r = GUILayoutUtility.GetLastRect();

                        EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, 1), rectColor);
                        EditorGUI.DrawRect(new Rect(r.x, r.height + r.y - 1, r.width, 1), rectColor);

                        EditorGUI.DrawRect(new Rect(r.x, r.y, 1, r.height), rectColor);
                        EditorGUI.DrawRect(new Rect(r.width + r.x - 1, r.y, 1, r.height), rectColor);
                    }

                    GUILayout.Space(5);

                    if (clipsReplaceFrom != string.Empty)
                    {
                        GUI.contentColor = contentClr;
                    }

                    if (sp.newPath == string.Empty || sp.newPath == sp.oldPath || clipsReplaceFrom != string.Empty)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                    }
                    if (GUILayout.Button(new GUIContent(ARStrings.ClipEditing.apply, ARStrings.ToolTips.applyIndividual), GUILayout.Width(75)))
                    {
                        ARManual.ClipEditing.RenameClipPaths(sp.foldoutClips.ToArray(), true, sp.oldPath, sp.newPath);
                    }
                    EditorGUI.EndDisabledGroup();

                    GUILayout.Label("(" + sp.count.ToString() + ")", GUILayout.Width(41));
                }

                if (sp.foldout == true)
                {
                    using (new SqueezeScope((5, 5, 4)))
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        foreach (AnimationClip clip in sp.foldoutClips)
                        {
                            EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                }
            }
        }

        public static void DrawSettings()
        {
            using (new SqueezeScope((5, 5, 3), (0, 10, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
            {
                using (new SqueezeScope((0, 0, 4, GUI.skin.box), (5, 5, 4), (5, 5, 3)))
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("SettingsIcon").image, ""), GUILayout.Width(20), GUILayout.Height(30));
                    GUILayout.Label("<color=#ffffff><b>" + ARStrings.Settings.settings + "</b></color>", ARStyle.settings, GUILayout.Height(30));
                    GUILayout.FlexibleSpace();
                }

                if (availableUpdate)
                {
                    using (new SqueezeScope((10, -5, 4), (0, 0, 4, GUI.skin.box), (5, 5, 4), (5, 5, 3)))
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(ARStrings.Settings.newVersion, GUILayout.Height(25));
                        GUILayout.Space(10);
                        if (GUILayout.Button("Update  " + currentVersion + "  ==>  " + newestVersion, GUILayout.Height(25)))
                        {
                            ARUpdater.UpdateTool();
                        }
                        GUILayout.FlexibleSpace();
                    }
                }

                DrawGeneralSettings();
                DrawAutomaticSettings();

                using (new SqueezeScope((20, 0, 4), (0, 0, 4, GUI.skin.box), (10, 10, 4)))
                {
                    using (new SqueezeScope((5, 5, 3)))
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(ARStrings.Settings.credit, EditorGUIUtility.IconContent("BuildSettings.Web.Small").image, ""), GUILayout.Width(EditorGUIUtility.currentViewWidth / 2.5f), GUILayout.Height(25)))
                        {
                            Application.OpenURL("https://hfcred.carrd.co");
                        }

                        GUILayout.Space(15);

                        if (GUILayout.Button(new GUIContent(ARStrings.Settings.docs, EditorGUIUtility.IconContent("TextAsset Icon").image, ""), GUILayout.Width(EditorGUIUtility.currentViewWidth / 2.5f), GUILayout.Height(25)))
                        {
                            Application.OpenURL("https://docs.hfcred.dev/animation-repathing/overview/");
                        }
                        GUILayout.FlexibleSpace();
                    }

                    using (new SqueezeScope((5, -5, 4), (0, 0, 3)))
                    {
                        GUILayout.FlexibleSpace();

                        Color c = GUI.color;
                        GUI.color = new Color(0.75f, 0.75f, 0.75f);
                        if (GUILayout.Button(new GUIContent(ARStrings.Settings.checkNewVersion, EditorGUIUtility.IconContent("TreeEditor.Refresh").image, ""), new GUIStyle(GUI.skin.label), GUILayout.Height(25)))
                        {
                            ARUpdater.CheckForNewVersion();
                        }
                        GUI.color = c;

                        GUILayout.FlexibleSpace();
                    }

                    using (new SqueezeScope((5, -5, 4), (5, 5, 3)))
                    {
                        if (!fetchingVersion)
                        {
                            GUILayout.FlexibleSpace();

                            Color c = GUI.color;

                            if (fetchingFailed)
                            {
                                GUI.color = new Color(1f, 0.4f, 0.4f);

                                if (GUILayout.Button(ARStrings.Settings.cantGetVersion, new GUIStyle(GUI.skin.label), GUILayout.Height(25)))
                                {
                                    fetchingVersion = true;
                                }
                            }
                            else
                            {
                                GUI.color = new Color(0.6f, 0.6f, 0.6f);

                                string text = availableUpdate ? ARStrings.Settings.yesNewVersion : ARStrings.Settings.noNewVersion;

                                if (GUILayout.Button(text, new GUIStyle(GUI.skin.label), GUILayout.Height(25)))
                                {
                                    fetchingVersion = true;
                                }
                            }

                            GUI.color = c;

                            GUILayout.FlexibleSpace();
                        }
                        else
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("", new GUIStyle(GUI.skin.label), GUILayout.Height(0)))
                            {
                            }
                            GUILayout.FlexibleSpace();
                        }
                    }
                }

                using (new SqueezeScope((5, 0, 4), (0, 0, 3)))
                {
                    GUILayout.FlexibleSpace();

                    Color c = GUI.color;
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUILayout.Label(currentVersion);
                    GUI.color = c;

                    GUILayout.FlexibleSpace();
                }
            }
        }

        public static void DrawGeneralSettings()
        {
            using (new SqueezeScope((15, 5, 4), (3, 0, 3)))
            {
                GUILayout.Label(ARStrings.Settings.general, ARStyle.settings, GUILayout.Height(30));

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, ARStrings.ToolTips.resetSettings), GUILayout.Height(30), GUILayout.Width(30)))
                {
                    ARSaveData.ResetGeneralData();
                }
            }

            if (getControllerAutomatically) EditorGUI.BeginDisabledGroup(true);
            using (new SqueezeScope((0, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
            {
                using (new SqueezeScope((10, 5, 4), (5, 5, 3)))
                {
                    GUILayout.Label(new GUIContent(ARStrings.Settings.target, ARStrings.ToolTips.target), GUILayout.MinWidth(100));

                    GUIContent[] content = { new GUIContent(ARStrings.Settings.animatorComponent), new GUIContent(ARStrings.Settings.vrchatAvatar) };
                    controllerSelection = EditorGUILayout.Popup(controllerSelection, content);
                }

                if (controllerSelection == 0)
                {
                    using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
                    {
                        GUILayout.Label(new GUIContent(ARStrings.Settings.controllerToUse, ARStrings.ToolTips.controllerToUse), GUILayout.MinWidth(100));
                        ARVariables.Animator = (Animator)EditorGUILayout.ObjectField(ARVariables.Animator, typeof(Animator), true);
                    }

                    if (ARVariables.Animator && !ARVariables.Animator.runtimeAnimatorController)
                    {
                        using (new SqueezeScope((-5, 10, 4), (5, 5, 3)))
                        {
                            EditorGUILayout.HelpBox(ARStrings.Settings.missingController, MessageType.Warning);
                        }
                    }
                }
                else
                {
                    using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
                    {
                        GUILayout.Label(new GUIContent(ARStrings.Settings.avatarToUse, ARStrings.ToolTips.avatarToUse), GUILayout.MinWidth(100));
                        ARVariables.Avatar = (GameObject)EditorGUILayout.ObjectField(ARVariables.Avatar, typeof(GameObject), true);
                    }
#if VRC_AVATARS
                    if (ARVariables.Avatar != null && ARVariables.Avatar.GetComponent<VRCAvatarDescriptor>() != null)
                    {
                        using (new SqueezeScope((-5, 10, 4), (5, 5, 3)))
                        {
                            GUILayout.Label(new GUIContent(ARStrings.Settings.layersToUse, ARStrings.ToolTips.layersToUse), GUILayout.MinWidth(100));
                            PlayableSelection = (Playables)EditorGUILayout.EnumFlagsField(PlayableSelection);
                        }
                    }
                    else if (ARVariables.Avatar != null)
                    {
                        using (new SqueezeScope((-5, 10, 4), (5, 5, 3)))
                        {
                            EditorGUILayout.HelpBox(ARStrings.Settings.missingDescriptor, MessageType.Warning);
                        }
                    }
#endif
                }
            }
            EditorGUI.EndDisabledGroup();

            using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
            {
                using (new SqueezeScope((10, 5, 4), (5, 5, 3)))
                {
                    getControllerAutomatically = GUILayout.Toggle(getControllerAutomatically, ARStrings.Settings.useRootController);
                }

                using (new SqueezeScope((0, 5, 4), (5, 5, 3)))
                {
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("d_console.infoicon.inactive.sml").image, ""), GUILayout.Height(20), GUILayout.Width(20));

                    Color c = GUI.color;
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUILayout.Label(ARStrings.Settings.useRootInfo, new GUIStyle(GUI.skin.label) { fontSize = 11 }, GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth / 1.4f));
                    GUI.color = c;
                }

                if (getControllerAutomatically)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    using (new SqueezeScope((0, 5, 4), (5, 5, 3)))
                    {
                        automaticAnimator = (Animator)EditorGUILayout.ObjectField(automaticAnimator, typeof(Animator), true);
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }

            using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
            {
                using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                {
                    EditorGUI.BeginChangeCheck();
                    disableTooltips = GUILayout.Toggle(disableTooltips, new GUIContent(ARStrings.Settings.disableTooltips, ARStrings.ToolTips.disableTooltips));
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (disableTooltips)
                        {
                            ARStrings.ClearTooltips();
                        }
                        else
                        {
                            ARStrings.ReloadLanguage(languageSelection);
                        }
                    }
                }
            }

            using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
            {
                using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                {
                    GUILayout.Label(ARStrings.Settings.language, GUILayout.MinWidth(100));

                    EditorGUI.BeginChangeCheck();
                    GUIContent[] content = { new GUIContent("English"), new GUIContent("日本"), new GUIContent("한국어"), new GUIContent("简体中文") };
                    languageSelection = EditorGUILayout.Popup(languageSelection, content);
                    if (EditorGUI.EndChangeCheck())
                    {
                        ARStrings.ReloadLanguage(languageSelection);
                    }
                }

                var (languageCreditText, languageCreditLink) = ARStrings.GetLanguageCredits(languageSelection);
                if (languageCreditText != null)
                {
                    DrawLanguageCredits(languageCreditText, languageCreditLink);
                }
            }
        }

        public static void DrawLanguageCredits(string buttonText, string link)
        {
            using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
            {
                if (GUILayout.Button(new GUIContent(buttonText, EditorGUIUtility.IconContent("BuildSettings.Web.Small").image, ""), GUILayout.Height(25)))
                {
                    if (link != null && link != string.Empty)
                    {
                        Application.OpenURL(link);
                    }
                }
            }
        }

        public static void DrawAutomaticSettings()
        {
            using (new SqueezeScope((25, 5, 4), (3, 0, 3)))
            {
                GUILayout.Label(ARStrings.Settings.automatic, ARStyle.settings, GUILayout.Height(30));

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, ARStrings.ToolTips.resetSettings), GUILayout.Height(30), GUILayout.Width(30)))
                {
                    ARSaveData.ResetAutomaticData();
                }
            }

            using (new SqueezeScope((0, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
            {
                using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                {
                    sendWarning = GUILayout.Toggle(sendWarning, new GUIContent(ARStrings.Settings.warningPopup, ARStrings.ToolTips.warningPopup));
                }
            }

            if (!sendWarning) EditorGUI.BeginDisabledGroup(true);
            using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
            {
                using (new SqueezeScope((10, 5, 4), (5, 5, 3)))
                {
                    warnOnlyIfUsed = GUILayout.Toggle(warnOnlyIfUsed, new GUIContent(ARStrings.Settings.warnOnlyIfUsed, ARStrings.ToolTips.warnOnlyIfUsed));
                }

                using (new SqueezeScope((0, 5, 4), (5, 5, 3)))
                {
                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("d_console.infoicon.inactive.sml").image, ""), GUILayout.Height(20), GUILayout.Width(20));

                    Color c = GUI.color;
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUILayout.Label(ARStrings.Settings.warning, new GUIStyle(GUI.skin.label) { fontSize = 11 }, GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth / 1.4f));
                    GUI.color = c;
                }
            }
            EditorGUI.EndDisabledGroup();

            using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
            {
                using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                {
                    activeInBackground = GUILayout.Toggle(activeInBackground, new GUIContent(ARStrings.Settings.runWhenWindowClosed, ARStrings.ToolTips.runWhenWindowClosed));
                }
            }

            using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
            {
                using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                {
                    disableDebugLogging = GUILayout.Toggle(disableDebugLogging, new GUIContent(ARStrings.Settings.disableLogging));
                }
            }
        }

        /// <summary>
        /// Draws a straight horizontal line which can be used as a divider. Parameters are used for margins.
        /// </summary>
        public static void DrawDivider(int above, int below, int left, int right)
        {
            using (new SqueezeScope((above, below, 4), (left, right, 3)))
            {
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(10));
                EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, 3), new Color(0.5f, 0.5f, 0.5f));
            }
        }

        /// <summary>
        /// Uses the previous rectangle to calculate the animation path of a GameObject that the user is dragging.
        /// </summary>
        /// <returns>
        /// String of the hierarchy path from the Avatar root or Animator GameObject to the dragged GameObject.
        /// Returns null if the Avatar, Animator or GameObject is null. 
        /// </returns>
        public static string DragAndDropGameobject()
        {
            Rect r = GUILayoutUtility.GetLastRect();
            Event e = Event.current;

            if (r.Contains(e.mousePosition))
            {
                var draggedObjects = DragAndDrop.objectReferences.Where(o => o is GameObject).Select(o => o as GameObject);

                if (e.type == EventType.DragUpdated)
                {
                    if (draggedObjects.Any())
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    }
                }


                if (e.type == EventType.DragPerform)
                {
                    var go = draggedObjects.First();
                    string p = controllerSelection == 0 ?
                    ARVariables.Animator == null ? AnimationUtility.CalculateTransformPath(go.transform, go.GetComponentInParent<Animator>()?.transform) : AnimationUtility.CalculateTransformPath(go.transform, ARVariables.Animator.transform) :
                    ARVariables.Avatar == null ? AnimationUtility.CalculateTransformPath(go.transform, go.GetComponentInParent<Animator>()?.transform) : AnimationUtility.CalculateTransformPath(go.transform, ARVariables.Avatar.transform);
                    return p;
                }
            }
            return null;
        }
    }

    public static class ARStyle
    {
        public static GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, richText = true };
        public static GUIStyle toggleButton = new GUIStyle(GUI.skin.button) { fontSize = 16, richText = true };
        public static GUIStyle foldout = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        public static GUIStyle settings = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, richText = true };
        public static GUIStyle invalidPath = new GUIStyle(GUI.skin.label) { fontSize = 12, richText = true };
        public static GUIStyle invalidPathTip = new GUIStyle(GUI.skin.label) { fontSize = 12, richText = true };
        public static GUIStyle resetButton = new GUIStyle(GUI.skin.label) { };
        public static GUIStyle replaceFromTo = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        public static GUIStyle replacePath = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
    }

    public class SqueezeScope : IDisposable
    {
        private readonly SqueezeSettings[] settings;
        //0 = none
        //1 = Horizontal
        //2 = Vertical
        //3 = EditorH
        //4 = EditorV

        public SqueezeScope(SqueezeSettings input) : this(new[] { input }) { }

        /// <summary>
        /// Method for creating a new horizontal or vertical scope with space applied before and after.
        /// Takes three inputs, first determines space before, second determines space after and third determines the scope type.
        /// Inputs can be stacked. Example: using (new SqueezeScope((10, 10, 4), (5, 5, 3))) {  }
        /// </summary>
        public SqueezeScope(params SqueezeSettings[] input)
        {
            settings = input;
            foreach (var squeezeSettings in input)
            {
                BeginSqueeze(squeezeSettings);
            }
        }

        private void BeginSqueeze(SqueezeSettings squeezeSettings)
        {
            switch (squeezeSettings.type)
            {
                case 1: GUILayout.BeginHorizontal(squeezeSettings.style); break;
                case 2: GUILayout.BeginVertical(squeezeSettings.style); break;
                case 3: EditorGUILayout.BeginHorizontal(squeezeSettings.style); break;
                case 4: EditorGUILayout.BeginVertical(squeezeSettings.style); break;
            }

            GUILayout.Space(squeezeSettings.width1);
        }

        public void Dispose()
        {
            foreach (var squeezeSettings in settings.Reverse())
            {
                GUILayout.Space(squeezeSettings.width2);
                switch (squeezeSettings.type)
                {
                    case 1: GUILayout.EndHorizontal(); break;
                    case 2: GUILayout.EndVertical(); break;
                    case 3: EditorGUILayout.EndHorizontal(); break;
                    case 4: EditorGUILayout.EndVertical(); break;
                }
            }
        }
    }

    public struct SqueezeSettings
    {
        public int width1;
        public int width2;
        public int type;
        public GUIStyle style;

        public static implicit operator SqueezeSettings((int, int) val)
        {
            return new SqueezeSettings() { width1 = val.Item1, width2 = val.Item2, type = 1, style = GUIStyle.none };
        }
        public static implicit operator SqueezeSettings((int, int, int) val)
        {
            return new SqueezeSettings() { width1 = val.Item1, width2 = val.Item2, type = val.Item3, style = GUIStyle.none };
        }

        public static implicit operator SqueezeSettings((int, int, int, GUIStyle) val)
        {
            return new SqueezeSettings() { width1 = val.Item1, width2 = val.Item2, type = val.Item3, style = val.Item4 };
        }
    }
}
