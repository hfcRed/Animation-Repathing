using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using static AutoAnimationRepath.AARVariables;

namespace AutoAnimationRepath
{
    public class AAREditor : EditorWindow
    {
        [MenuItem("hfcRed/Tools/Animation Repathing")]
        static void ShowWindow() => GetWindow<AAREditor>("Animation Repathing").titleContent.image = EditorGUIUtility.IconContent("AnimationClip Icon").image;

        public static Vector2 scroll = Vector2.zero;
        public void OnGUI()
        {
            Repaint();

            EditorGUI.BeginChangeCheck();
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(position.width));

            DrawHeader();
            switch (toolSelection)
            {
                case 0: DrawAutomatic(); break;
                case 1: DrawManual(); break;
            }
            DrawSettings();

            GUILayout.EndScrollView();
            if (EditorGUI.EndChangeCheck())
            {
                AARSaveData.SaveData();
            }
        }

        public static void DrawHeader()
        {
            using (new SqueezeScope((5, 5, 3), (10, 5, 4)))
            {
                GUIContent[] content = { new GUIContent(AARStrings.Main.automatic, AARStrings.ToolTips.automatic), new GUIContent(AARStrings.Main.manual, AARStrings.ToolTips.manual) };
                toolSelection = GUILayout.Toolbar(toolSelection, content, GUILayout.Height(25));
            }
        }

        public static void DrawAutomatic()
        {
            using (new SqueezeScope((5, 5, 3), (10, 5, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
            {
                using (new SqueezeScope((0, 0, 4, GUI.skin.box), (5, 5, 4), (5, 5, 3)))
                {
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("Profiler.Memory").image, ""), GUILayout.Width(20), GUILayout.Height(25));
                    GUILayout.Label("<color=#ffffff><b>" + AARStrings.Automatic.title + "</b></color>", AARStyle.title);
                }

                using (new SqueezeScope((15, 0, 4)))
                {
                    GUIContent content = automaticIsEnabled ? new GUIContent("<color=#4aff93><b>" + AARStrings.Automatic.enabled + "</b></color>", AARStrings.ToolTips.toggleButton) : new GUIContent("<color=#ff5263><b>" + AARStrings.Automatic.disabled + "</b></color>", AARStrings.ToolTips.toggleButton);
                    automaticIsEnabled = GUILayout.Toggle(automaticIsEnabled, content, AARStyle.toggleButton, GUILayout.Height(40));
                }
            }
        }

        public static void DrawManual()
        {
            using (new SqueezeScope((5, 5, 3), (10, 5, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
            {
                using (new SqueezeScope((0, 0, 4, GUI.skin.box), (5, 5, 4), (5, 5, 3)))
                {
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("ViewToolMove").image, ""), GUILayout.Width(20), GUILayout.Height(25));
                    GUILayout.Label("<color=#ffffff><b>" + AARStrings.Manual.title + "</b></color>", AARStyle.title);
                }

                using (new SqueezeScope((15, 0, 4)))
                {
                    GUIContent[] content = { new GUIContent(AARStrings.Manual.fixPaths, AARStrings.ToolTips.fixPaths), new GUIContent(AARStrings.Manual.editClips, AARStrings.ToolTips.editClips) };
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
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    GUILayout.Label(new GUIContent(AARStrings.InvalidPaths.invalidPaths), AARStyle.foldout);
                }

                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, AARStrings.ToolTips.resetInvalidPaths), GUILayout.Height(25)))
                {
                    List<AnimatorController> animators = GetControllers();

                    foreach (AnimatorController a in animators)
                        AARManual.InvalidPaths.ScanInvalidPaths(a);
                }
            }

            for (int i = 0; i < invalidPathToSharedProperty.Values.Count; i++)
            {
                InvalidSharedProperty sp = invalidPathToSharedProperty.Values.ElementAt(i);

                string str = sp.oldPath;

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, GUI.skin.box), (5, 5, 3)))
                {
                    GUILayout.Label(new GUIContent("<color=#F0DE08><b>" + str + "</b></color>"), AARStyle.invalidPath);
                }

                using (new SqueezeScope((0, 0, 4), (0, 0, 3)))
                {
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, AARStrings.ToolTips.resetInvalidPath), AARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        sp.newPath = sp.oldPath;
                    }

                    GUIContent content = sp.foldout ? new GUIContent(EditorGUIUtility.IconContent("IN foldout act on").image, "") : new GUIContent(EditorGUIUtility.IconContent("IN foldout act").image, "");
                    sp.foldout = GUILayout.Toggle(sp.foldout, content, new GUIStyle(GUI.skin.button), GUILayout.Width(25));

                    sp.newPath = GUILayout.TextField(sp.newPath, GUILayout.MinWidth(1));
                    string path = DragAndDropGameobject();
                    if (path != null)
                    {
                        sp.newPath = path;
                    }

                    if (sp.newPath == string.Empty || sp.newPath == sp.oldPath)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                    }
                    if (GUILayout.Button(new GUIContent(AARStrings.InvalidPaths.apply, AARStrings.ToolTips.applyValidPath), GUILayout.Width(75)))
                    {
                        try
                        {
                            AssetDatabase.StartAssetEditing();

                            foreach (AnimationClip clip in sp.foldoutClips)
                            {
                                AARManual.InvalidPaths.RenameInvalidPaths(clip, sp.oldPath, sp.newPath);
                            }
                        }
                        finally { AssetDatabase.StopAssetEditing(); }

                        List<AnimatorController> animators = GetControllers();

                        foreach (AnimatorController a in animators)
                        {
                            AARManual.InvalidPaths.ScanInvalidPaths(a);
                        }
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
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("TestPassed").image, ""), GUILayout.Width(20), GUILayout.Height(21));
                    GUILayout.Label(AARStrings.InvalidPaths.noInvalidPaths, AARStyle.invalidPathTip, GUILayout.Height(20));
                }
                else
                {
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("GameObject Icon").image, ""), GUILayout.Width(20), GUILayout.Height(21));
                    GUILayout.Label(AARStrings.InvalidPaths.dragAndDrop, AARStyle.invalidPathTip, GUILayout.Height(20));
                }
            }
        }

        public static void DrawManualClipEditing()
        {
            if (clipsSelected.Count != 0)
            {
                using (new SqueezeScope((15, 0, 4)))
                {
                    using (new SqueezeScope((0, 0, 4, GUI.skin.box), (5, 5, 3)))
                    {
                        GUILayout.Label("", GUILayout.ExpandWidth(true));

                        GUILayout.Label(new GUIContent(AARStrings.ClipEditing.replacePartOfAll, AARStrings.ToolTips.replacePartOfAll), AARStyle.replacePath);
                    }

                    using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
                    {
                        GUILayout.Label("", GUILayout.Width(55));
                        GUILayout.Label(new GUIContent(AARStrings.ClipEditing.from, AARStrings.ToolTips.replaceFrom), AARStyle.replaceFromTo);
                        GUILayout.Label(new GUIContent(AARStrings.ClipEditing.to, AARStrings.ToolTips.replaceTo), AARStyle.replaceFromTo);
                        GUILayout.Label("", GUILayout.Width(140));
                    }

                    bool warning = false;
                    int countTotal = 0;
                    List<AnimationClip> clipsTarget = new List<AnimationClip>();

                    using (new SqueezeScope((5, 0, 4), (0, 0, 3)))
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

                        if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, AARStrings.ToolTips.resetPartOfAll), AARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
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

                        if (clipsReplaceFrom == string.Empty || clipsReplaceFrom == clipsReplaceTo || countTotal == 0)
                        {
                            EditorGUI.BeginDisabledGroup(true);
                        }
                        if (GUILayout.Button(new GUIContent(AARStrings.ClipEditing.apply, AARStrings.ToolTips.applyPartOfAll), GUILayout.Width(75)))
                        {
                            try
                            {
                                AssetDatabase.StartAssetEditing();

                                foreach (AnimationClip clip in clipsTarget)
                                {
                                    AARManual.ClipEditing.RenameClipPaths(clip, false, clipsReplaceFrom, clipsReplaceTo);
                                }
                            }
                            finally { AssetDatabase.StopAssetEditing(); AARManual.ClipEditing.GetClipPaths(); }
                        }
                        EditorGUI.EndDisabledGroup();

                        GUILayout.Label("(" + countTotal.ToString() + ")", GUILayout.Width(41));
                    }

                    if (clipsReplaceFoldout == true)
                    {
                        using (new SqueezeScope((5, 0, 4)))
                        {
                            EditorGUI.BeginDisabledGroup(true);
                            foreach (AnimationClip clip in clipsTarget)
                            {
                                EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                    }

                    using (new SqueezeScope((5, 0, 4), (0, 0, 3)))
                    {
                        if (warning == true)
                        {
                            EditorGUILayout.HelpBox(AARStrings.ClipEditing.warning, MessageType.Warning);
                        }
                    }

                    DrawDivider(20, 15, 0, 0);

                    using (new SqueezeScope((0, 10, 4), (0, 0, 4, GUI.skin.box), (5, 5, 3)))
                    {
                        GUILayout.Label("", GUILayout.ExpandWidth(true));

                        GUILayout.Label(new GUIContent(AARStrings.ClipEditing.replaceIndividual, AARStrings.ToolTips.replaceIndividual), AARStyle.replacePath);
                    }

                    foreach (ClipsSharedProperty sp in clipsPathToSharedProperty.Values)
                    {
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
                            else if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, AARStrings.ToolTips.resetIndividual), AARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                sp.newPath = sp.oldPath;
                            }

                            GUIContent content = sp.foldout ? new GUIContent(EditorGUIUtility.IconContent("IN foldout act on").image, "") : new GUIContent(EditorGUIUtility.IconContent("IN foldout act").image, "");
                            sp.foldout = GUILayout.Toggle(sp.foldout, content, new GUIStyle(GUI.skin.button), GUILayout.Width(25));

                            if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty && sp.oldPath.Contains(clipsReplaceFrom))
                            {
                                GUI.contentColor = Color.green;
                            }
                            else if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty)
                            {
                                GUI.contentColor = Color.gray;
                            }

                            GUILayout.Space(5);

                            sp.newPath = GUILayout.TextField(sp.newPath, GUILayout.MinWidth(1));
                            string path = DragAndDropGameobject();
                            if (path != null)
                            {
                                sp.newPath = path;
                            }

                            GUILayout.Space(5);

                            if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty)
                            {
                                GUI.contentColor = Color.white;
                            }

                            if (sp.newPath == string.Empty || sp.newPath == sp.oldPath)
                            {
                                EditorGUI.BeginDisabledGroup(true);
                            }
                            if (GUILayout.Button(new GUIContent(AARStrings.ClipEditing.apply, AARStrings.ToolTips.applyIndividual), GUILayout.Width(75)))
                            {
                                try
                                {
                                    AssetDatabase.StartAssetEditing();

                                    foreach (AnimationClip clip in sp.foldoutClips)
                                    {
                                        AARManual.ClipEditing.RenameClipPaths(clip, true, sp.oldPath, sp.newPath);
                                    }
                                }
                                finally { AssetDatabase.StopAssetEditing(); AARManual.ClipEditing.GetClipPaths(); }
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
            }

            using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
            {
                if (clipsSelected.Count == 0)
                {
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("AnimationClip Icon").image, ""), GUILayout.Width(20), GUILayout.Height(21));
                    GUILayout.Label(AARStrings.ClipEditing.noClipsSelected, AARStyle.invalidPathTip, GUILayout.Height(20));
                }
                else
                {
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("GameObject Icon").image, ""), GUILayout.Width(20), GUILayout.Height(21));
                    GUILayout.Label(AARStrings.ClipEditing.dragAndDrop, AARStyle.invalidPathTip, GUILayout.Height(20));
                }
            }
        }

        public static void DrawSettings()
        {
            DrawDivider(23, 20, 5, 5);

            using (new SqueezeScope((5, 5, 3), (0, 10, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
            {

                using (new SqueezeScope((0, 0, 4, GUI.skin.box), (5, 5, 4), (5, 5, 3)))
                {
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("SettingsIcon").image, ""), GUILayout.Width(20), GUILayout.Height(30));
                    GUILayout.Label("<color=#ffffff><b>" + AARStrings.Settings.settings + "</b></color>", AARStyle.settings, GUILayout.Height(30));
                }

                using (new SqueezeScope((15, 5, 4), (3, 0, 3)))
                {
                    GUILayout.Label(AARStrings.Settings.general, AARStyle.settings, GUILayout.Height(30));

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, AARStrings.ToolTips.resetSettings), GUILayout.Height(30), GUILayout.Width(30)))
                    {
                        AARSaveData.ResetGeneralData();
                    }
                }

                using (new SqueezeScope((0, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 5, 4), (5, 5, 3)))
                    {
                        GUILayout.Label(new GUIContent(AARStrings.Settings.target, AARStrings.ToolTips.target), GUILayout.Width(150));

                        GUIContent[] content = { new GUIContent(AARStrings.Settings.animatorComponent), new GUIContent(AARStrings.Settings.vrchatAvatar) };
                        controllerSelection = EditorGUILayout.Popup(controllerSelection, content);
                    }

                    if (controllerSelection == 0)
                    {
                        using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
                        {
                            GUILayout.Label(new GUIContent(AARStrings.Settings.controllerToUse, AARStrings.ToolTips.controllerToUse), GUILayout.Width(150));
                            animator = (Animator)EditorGUILayout.ObjectField(animator, typeof(Animator), true);
                        }
                    }
                    else
                    {
                        using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
                        {
                            GUILayout.Label(new GUIContent(AARStrings.Settings.avatarToUse, AARStrings.ToolTips.avatarToUse), GUILayout.Width(150));
                            avatar = (GameObject)EditorGUILayout.ObjectField(avatar, typeof(GameObject), true);
                        }
#if VRC_SDK_VRCSDK3
                        if (avatar != null && avatar.GetComponent<VRCAvatarDescriptor>() != null)
                        {
                            using (new SqueezeScope((-5, 10, 4), (5, 5, 3)))
                            {
                                GUILayout.Label(new GUIContent(AARStrings.Settings.layersToUse, AARStrings.ToolTips.layersToUse), GUILayout.Width(150));
                                PlayableSelection = (Playables)EditorGUILayout.EnumFlagsField(PlayableSelection);
                            }
                        }
#endif
                    }
                }

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                    {
                        EditorGUI.BeginChangeCheck();
                        disableTooltips = GUILayout.Toggle(disableTooltips, new GUIContent(AARStrings.Settings.disableTooltips, AARStrings.ToolTips.disableTooltips));
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (disableTooltips == true)
                            {
                                AARStrings.clearTooltips();
                            }
                            else if (languageSelection == 0)
                            {
                                AARStrings.loadEnglisch();
                            }
                            else
                            {
                                AARStrings.loadJapanese();
                            }
                        }
                    }
                }

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                    {
                        GUILayout.Label(AARStrings.Settings.language, GUILayout.Width(150));

                        EditorGUI.BeginChangeCheck();
                        GUIContent[] content = { new GUIContent("English"), new GUIContent("日本") };
                        languageSelection = EditorGUILayout.Popup(languageSelection, content);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (languageSelection == 0)
                            {
                                AARStrings.loadEnglisch();
                            }
                            else
                            {
                                AARStrings.loadJapanese();
                            }
                        }
                    }
                }

                using (new SqueezeScope((15, 5, 4), (3, 0, 3)))
                {
                    GUILayout.Label(AARStrings.Settings.automatic, AARStyle.settings, GUILayout.Height(30));

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh").image, AARStrings.ToolTips.resetSettings), GUILayout.Height(30), GUILayout.Width(30)))
                    {
                        AARSaveData.ResetAutomaticData();
                    }
                }

                using (new SqueezeScope((0, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 0, 4), (5, 5, 3)))
                    {
                        renameActive = GUILayout.Toggle(renameActive, new GUIContent(AARStrings.Settings.repathWhenRenamed, AARStrings.ToolTips.repathWhenRenamed));
                        reparentActive = GUILayout.Toggle(reparentActive, new GUIContent(AARStrings.Settings.repathWhenReparented, AARStrings.ToolTips.repathWhenReparented));
                    }

                    using (new SqueezeScope((5, 0, 4), (5, 5, 3)))
                    {
                        renameWarning = GUILayout.Toggle(renameWarning, new GUIContent(AARStrings.Settings.warnWhenRenamed, AARStrings.ToolTips.warnWhenRenamed));
                        reparentWarning = GUILayout.Toggle(reparentWarning, new GUIContent(AARStrings.Settings.warnWhenReparented, AARStrings.ToolTips.warnWhenReparented));
                    }

                    using (new SqueezeScope((5, 10, 4), (5, 5, 3)))
                    {
                        activeInBackground = GUILayout.Toggle(activeInBackground, new GUIContent(AARStrings.Settings.runWhenWindowClosed, AARStrings.ToolTips.runWhenWindowClosed));
                    }
                }

                using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
                {
                    GUILayout.Label("", GUILayout.ExpandWidth(true));

                    if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("UnityEditor.FindDependencies").image, ""), new GUIStyle(GUI.skin.label), GUILayout.Width(20), GUILayout.Height(21)))
                        Application.OpenURL("https://hfcred.carrd.co");
                    if (GUILayout.Button(AARStrings.Settings.credit, new GUIStyle(EditorStyles.linkLabel), GUILayout.Height(20)))
                        Application.OpenURL("https://hfcred.carrd.co");

                    Rect r = GUILayoutUtility.GetLastRect();
                    Event e = Event.current;
                    if (r.Contains(e.mousePosition))
                    {
                        EditorGUI.DrawRect(new Rect(r.x, r.y + 17, r.width, 1), new Color(0.49f, 0.678f, 0.957f));
                    }

                    GUILayout.Label("", GUILayout.ExpandWidth(true));
                }
            }
        }

        public static void DrawDivider(int above, int below, int left, int right)
        {
            using (new SqueezeScope((above, below, 4), (left, right, 3)))
            {
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(10));
                EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, 2), new Color(0.5f, 0.5f, 0.5f));
            }
        }

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
                    animator == null ? AnimationUtility.CalculateTransformPath(go.transform, go.GetComponentInParent<Animator>()?.transform) : AnimationUtility.CalculateTransformPath(go.transform, animator.transform) :
                    avatar == null ? AnimationUtility.CalculateTransformPath(go.transform, go.GetComponentInParent<Animator>()?.transform) : AnimationUtility.CalculateTransformPath(go.transform, avatar.transform);
                    return (p);
                }
            }
            return (null);
        }
    }

    public static class AARStyle
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

    public class SqueezeScope : System.IDisposable
    {
        private SqueezeSettings[] settings;
        //0 = none
        //1 = Horizontal
        //2 = Vertical
        //3 = EditorH
        //4 = EditorV

        public SqueezeScope(SqueezeSettings input) : this(new[] { input }) { }

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
