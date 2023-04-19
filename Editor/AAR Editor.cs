using UnityEngine;
using UnityEditor;
using System.Linq;
using VRC.SDK3.Avatars.Components;
using static AutoAnimationRepath.AARVariables;
using Boo.Lang;
using System;

namespace AutoAnimationRepath
{
    public class AAREditor : EditorWindow
    {
        [MenuItem("hfcRed/Tools/Auto Repath")]
        static void ShowWindow() => GetWindow<AAREditor>().titleContent.image = EditorGUIUtility.IconContent("AnimationClip Icon").image;

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
            if (manualToolSelection != 1 || toolSelection == 0)
            {
                DrawSettings();
            }
            
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
                toolSelection = GUILayout.Toolbar(toolSelection, tools, GUILayout.Height(25));
            }
        }

        public static void DrawAutomatic()
        {
            using (new SqueezeScope((5, 5, 3), (5, 5, 4), (0, 0, 4, EditorStyles.helpBox), (10, 10, 4), (5, 5, 3)))
            {
                GUILayout.Label(Strings.Automatic.title, AARStyle.title);

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Strings.Automatic.credit, "linkLabel"))
                    Application.OpenURL("https://hfcred.carrd.co");
                if (GUILayout.Button(AARContent.linkIcon, "label", GUILayout.Height(21)))
                    Application.OpenURL("https://hfcred.carrd.co");
            }

            using (new SqueezeScope((5, 5, 3), (5, 5, 4)))
            {
                string buttonLabel = isEnabled ? "<color=#4aff93><b>"+ Strings.Automatic.enabled + "</b></color>" : "<color=#ff5263><b>" + Strings.Automatic.disabled + "</b></color>";
                isEnabled = GUILayout.Toggle(isEnabled, buttonLabel, AARStyle.toggleButton, GUILayout.Height(30));
            }
        }

        public static void DrawManual()
        {
            using (new SqueezeScope((5, 5, 3), (5, 5, 4), (0, 0, 4, EditorStyles.helpBox), (10, 10, 4), (5, 5, 3)))
            {
                GUILayout.Label(Strings.Manual.title, AARStyle.title);

                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Strings.Manual.credit, "linkLabel"))
                    Application.OpenURL("https://hfcred.carrd.co");
                if (GUILayout.Button(AARContent.linkIcon, "label", GUILayout.Height(21)))
                    Application.OpenURL("https://hfcred.carrd.co");
            }

            using (new SqueezeScope((5, 5, 3), (5, 5, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
            {
                using (new SqueezeScope((0, 0, 4)))
                {
                    manualToolSelection = GUILayout.Toolbar(manualToolSelection, manualTools, GUILayout.Height(25));
                }

                if (manualToolSelection == 0)
                {
                    using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
                    {
                        using (new SqueezeScope((5, 0, 4)))
                        {
                            invalidPathsFoldout = EditorGUILayout.Foldout(invalidPathsFoldout, Strings.InvalidPaths.invalidPaths, AARStyle.foldout);
                        }

                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(AARContent.resetIcon, GUILayout.Height(30), GUILayout.Width(30)))
                        {
                            invalidNewPaths.Clear();
                            AARManual.InvalidPaths.ScanInvalidPaths();
                        }
                    }
                }

                if (invalidPathsFoldout && manualToolSelection == 0)
                {
                    for (invalidPosition = 0; invalidPosition < invalidOldPaths.Count; invalidPosition++)
                    {
                        string str = invalidOldPaths[invalidPosition];

                        using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
                        {
                            GUILayout.Label("<color=#F0DE08><b>" + str + "</b></color>", AARStyle.invalidPath);

                            using (new SqueezeScope((5, 0, 4), (0, 0, 3)))
                            {
                                if (GUILayout.Button(AARContent.resetIcon, AARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    invalidNewPaths[invalidPosition] = invalidOldPaths[invalidPosition];
                                }

                                invalidNewPaths[invalidPosition] = GUILayout.TextField(invalidNewPaths[invalidPosition], GUILayout.MinWidth(1));
                                string path = DragAndDropGameobject();
                                if (path != null)
                                {
                                    invalidNewPaths[invalidPosition] = path;
                                }

                                if (invalidNewPaths[invalidPosition] == string.Empty || invalidNewPaths[invalidPosition] == invalidOldPaths[invalidPosition])
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                }
                                if (GUILayout.Button(Strings.InvalidPaths.apply, GUILayout.Width(75)))
                                {
                                    AARManual.InvalidPaths.RenameInvalidPaths();
                                }
                                EditorGUI.EndDisabledGroup();
                            }

                            using (new SqueezeScope((5, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                            {
                                invalidFoldouts[invalidPosition] = EditorGUILayout.Foldout(invalidFoldouts[invalidPosition], Strings.InvalidPaths.affectedClips + " (" + invalidClips[invalidPosition].Count + ")");
                                if (invalidFoldouts[invalidPosition] == true)
                                {
                                    GUILayout.Space(5);
                                    EditorGUI.BeginDisabledGroup(true);
                                    foreach (AnimationClip clip in invalidClips[invalidPosition])
                                    {
                                        EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);
                                    }
                                    EditorGUI.EndDisabledGroup();
                                }
                            }
                        }
                    }

                    using (new SqueezeScope((10, 0, 4)))
                    {
                        if (invalidOldPaths.Count == 0)
                        {
                            GUILayout.Label(Strings.InvalidPaths.noInvalidPaths, AARStyle.invalidPathTip);
                        }
                        else
                        {
                            GUILayout.Label(Strings.InvalidPaths.dragAndDrop, AARStyle.invalidPathTip);
                        }
                    }
                }

                if (manualToolSelection == 1)
                {
                    if (clipsClips.Count != 0)
                    {
                        using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
                        {
                            using (new SqueezeScope((0, 0, 3)))
                            {
                                GUILayout.Label("<color=#ffffff>" + Strings.ClipEditing.replacePartOfAll + "</color>", AARStyle.replacePath);
                            }

                            using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
                            {
                                GUILayout.Label("", GUILayout.Width(50));
                                GUILayout.Label(Strings.ClipEditing.from, AARStyle.replaceFromTo);
                                GUILayout.Label(Strings.ClipEditing.to, AARStyle.replaceFromTo);
                                GUILayout.Label("", GUILayout.Width(130));
                            }

                            bool warning = false;
                            int countTotal = 0;
                            List<AnimationClip> clipsTarget = new List<AnimationClip>();
                            using (new SqueezeScope((5, 0, 4), (0, 0, 3)))
                            {
                                foreach (SharedProperty sp in pathToSharedProperty.Values)
                                {
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

                                if (GUILayout.Button(AARContent.resetIcon, AARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
                                {
                                    clipsReplaceFrom = string.Empty;
                                    clipsReplaceTo = string.Empty;
                                }

                                string buttonLabel = clipsFoldout ? "<b>v</b>" : "<b>></b>";
                                clipsFoldout = GUILayout.Toggle(clipsFoldout, buttonLabel, AARStyle.customFoldout, GUILayout.Width(25));

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

                                if (clipsReplaceFrom == string.Empty || clipsReplaceFrom == clipsReplaceTo || countTotal == 0)
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                }
                                if (GUILayout.Button(Strings.ClipEditing.apply, GUILayout.Width(75)))
                                {
                                    foreach (AnimationClip clip in clipsTarget)
                                    {
                                        AARManual.ClipEditing.RenameClipPaths(clip, false, string.Empty, string.Empty);
                                    }

                                }
                                EditorGUI.EndDisabledGroup();

                                GUILayout.Label("(" + countTotal.ToString() + ")", GUILayout.Width(41));
                            }

                            if (clipsFoldout == true)
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
                                    EditorGUILayout.HelpBox(Strings.ClipEditing.warning, MessageType.Warning);
                                }
                            }

                            using (new SqueezeScope((20, 15, 4)))
                            {
                                UnityEngine.Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(10));
                                EditorGUI.DrawRect(new UnityEngine.Rect(r.x, r.y, r.width, 2), new Color(0.5f, 0.5f, 0.5f));
                            }

                            using (new SqueezeScope((0, 10, 4), (0, 0, 3)))
                            {
                                GUILayout.Label("<color=#ffffff>" + Strings.ClipEditing.replaceIndividual + "</color>", AARStyle.replacePath);
                            }

                            foreach (SharedProperty sp in pathToSharedProperty.Values)
                            {
                                string str = sp.oldPath;

                                using (new SqueezeScope((0, 0, 4), (0, 0, 3)))
                                {
                                    if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty)
                                    {
                                        sp.newPath = sp.oldPath;

                                        if (sp.warning == true)
                                        {
                                            GUILayout.Label(AARContent.warningIcon, GUILayout.Width(20), GUILayout.Height(20));
                                        }
                                        else if (sp.oldPath.Contains(clipsReplaceFrom))
                                        {
                                            GUILayout.Label(AARContent.passedIcon, GUILayout.Width(20), GUILayout.Height(20));
                                        }
                                        else
                                        {
                                            GUILayout.Label("", GUILayout.Width(20), GUILayout.Height(20));
                                        }
                                    }
                                    else if (GUILayout.Button(AARContent.resetIcon, AARStyle.resetButton, GUILayout.Width(20), GUILayout.Height(20)))
                                    {
                                        sp.newPath = sp.oldPath;
                                    }

                                    string buttonLabel = sp.foldout ? "<b>v</b>" : "<b>></b>";
                                    sp.foldout = GUILayout.Toggle(sp.foldout, buttonLabel, AARStyle.customFoldout, GUILayout.Width(25));

                                    if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty && sp.oldPath.Contains(clipsReplaceFrom))
                                    {
                                        GUI.contentColor = Color.green;
                                    }
                                    else if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty)
                                    {
                                        GUI.contentColor = Color.gray;
                                    }

                                    sp.newPath = GUILayout.TextField(sp.newPath, GUILayout.MinWidth(1));
                                    string path = DragAndDropGameobject();
                                    if (path != null)
                                    {
                                        sp.newPath = path;
                                    }

                                    if (clipsReplaceFrom != null && clipsReplaceFrom != string.Empty)
                                    {
                                        GUI.contentColor = Color.white;
                                    }

                                    if (sp.newPath == string.Empty || sp.newPath == sp.oldPath)
                                    {
                                        EditorGUI.BeginDisabledGroup(true);
                                    }
                                    if (GUILayout.Button(Strings.ClipEditing.apply, GUILayout.Width(75)))
                                    {
                                        foreach (AnimationClip clip in sp.foldoutClips)
                                        {
                                            AARManual.ClipEditing.RenameClipPaths(clip, true, sp.oldPath, sp.newPath);
                                        }
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

                    using (new SqueezeScope((10, 0, 4)))
                    {
                        if (clipsClips.Count == 0)
                        {
                            GUILayout.Label(Strings.ClipEditing.noClipsSelected, AARStyle.invalidPathTip);
                        }
                        else
                        {
                            GUILayout.Label(Strings.ClipEditing.dragAndDrop, AARStyle.invalidPathTip);
                        }
                    }
                }
            }
        }


        public static void DrawSettings()
        {
            using (new SqueezeScope((15, 15, 4)))
            {
                UnityEngine.Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(10));
                EditorGUI.DrawRect(new UnityEngine.Rect(r.x, r.y, r.width, 2), new Color(0.5f, 0.5f, 0.5f));
            }

            using (new SqueezeScope((5, 5, 3), (0, 10, 4), (0, 0, 4, EditorStyles.helpBox), (5, 5, 3), (10, 10, 4)))
            {
                using (new SqueezeScope((0, 0, 3)))
                {
                    GUILayout.Label("<b>" + Strings.Settings.settings +  "</b>", AARStyle.settings);

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(AARContent.resetIcon, GUILayout.Height(30), GUILayout.Width(30)))
                    {
                        AARSaveData.ResetData();
                    }
                }

                using (new SqueezeScope((15, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 5, 4), (5, 5, 3)))
                    {
                        GUILayout.Label(Strings.Settings.target, GUILayout.Width(150));
                        controllerSelection = EditorGUILayout.Popup(controllerSelection, controllerOptions);
                    }

                    if (controllerSelection == 0)
                    {
                        using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
                        {
                            GUILayout.Label(Strings.Settings.controllerToUse, GUILayout.Width(150));
                            animator = (Animator)EditorGUILayout.ObjectField(animator, typeof(Animator), true);
                        }
                    }
                    else
                    {
                        using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
                        {
                            GUILayout.Label(Strings.Settings.avatarToUse, GUILayout.Width(150));
                            avatar = (GameObject)EditorGUILayout.ObjectField(avatar, typeof(GameObject), true);
                        }
#if VRC_SDK_VRCSDK3
                        if (avatar != null && avatar.GetComponent<VRCAvatarDescriptor>() != null)
                        {
                            using (new SqueezeScope((-5, 10, 4), (5, 5, 3)))
                            {
                                GUILayout.Label(Strings.Settings.layersToUse, GUILayout.Width(150));
                                PlayableSelection = (Playables)EditorGUILayout.EnumFlagsField(PlayableSelection);
                            }
                        }
#endif
                    }
                }

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 0, 4), (5, 5, 3)))
                    {
                        renameActive = GUILayout.Toggle(renameActive, Strings.Settings.repathWhenRenamed);
                        reparentActive = GUILayout.Toggle(reparentActive, Strings.Settings.repathWhenReparented);
                    }

                    using (new SqueezeScope((5, 10, 4), (5, 5, 3)))
                    {
                        renameWarning = GUILayout.Toggle(renameWarning, Strings.Settings.warnWhenRenamed);
                        reparentWarning = GUILayout.Toggle(reparentWarning, Strings.Settings.warnWhenReparented);
                    }
                }

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                    {
                        activeInBackground = GUILayout.Toggle(activeInBackground, Strings.Settings.runWhenWindowClosed);
                    }
                }

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                    {
                        GUILayout.Label(Strings.Settings.language, GUILayout.Width(150));

                        EditorGUI.BeginChangeCheck();
                        languageSelection = EditorGUILayout.Popup(languageSelection, languageOptions);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (languageSelection == 0)
                                Strings.loadEnglisch();
                            else
                                Strings.loadJapanese();
                        }
                    }
                }
            }
        }

        public static string DragAndDropGameobject()
        {
            UnityEngine.Rect r = GUILayoutUtility.GetLastRect();
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
                    string p = AnimationUtility.CalculateTransformPath(go.transform, AARAutomatic.GetRoot());
                    return(p);
                }
            }
            return(null);
        }
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
