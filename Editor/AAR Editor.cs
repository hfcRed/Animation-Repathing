using UnityEngine;
using UnityEditor;
using System.Linq;
using VRC.SDK3.Avatars.Components;
using static AutoAnimationRepath.AARVariables;
using static AutoAnimationRepath.AARSettings;
using static WindowsUtil;
using Boo.Lang;

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
                SaveData();
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
                GUILayout.Label("Auto Animation Repathing", AARStyle.title);

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Made by hfcRed", "linkLabel"))
                    Application.OpenURL("https://hfcred.carrd.co");
                if (GUILayout.Button(AARContent.linkIcon, "label", GUILayout.Height(21)))
                    Application.OpenURL("https://hfcred.carrd.co");
            }

            using (new SqueezeScope((5, 5, 3), (5, 5, 4)))
            {
                string buttonLabel = isEnabled ? "<color=#4aff93><b>Enabled</b></color>" : "<color=#ff5263><b>Disabled</b></color>";
                isEnabled = GUILayout.Toggle(isEnabled, buttonLabel, AARStyle.toggleButton, GUILayout.Height(30));
            }
        }

        public static void DrawManual()
        {
            using (new SqueezeScope((5, 5, 3), (5, 5, 4), (0, 0, 4, EditorStyles.helpBox), (10, 10, 4), (5, 5, 3)))
            {
                GUILayout.Label("Manual Animation Repathing", AARStyle.title);

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Made by hfcRed", "linkLabel"))
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
                            invalidPathsFoldout = EditorGUILayout.Foldout(invalidPathsFoldout, "Invalid Paths", AARStyle.foldout);
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
                                if (GUILayout.Button("Apply", GUILayout.Width(75)))
                                {
                                    AARManual.InvalidPaths.RenameInvalidPaths();
                                }
                                EditorGUI.EndDisabledGroup();
                            }

                            using (new SqueezeScope((5, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                            {
                                invalidFoldouts[invalidPosition] = EditorGUILayout.Foldout(invalidFoldouts[invalidPosition], "Affected Clips" + " (" + invalidClips[invalidPosition].Count + ")");
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
                            GUILayout.Label("You have no invalid paths in your controller", AARStyle.invalidPathTip);
                        }
                        else
                        {
                            GUILayout.Label("You can drag and drop GameObjects into the text fields", AARStyle.invalidPathTip);
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
                                GUILayout.Label("<color=#ffffff>Replace part of all paths</color>", AARStyle.replacePath);
                            }

                            using (new SqueezeScope((10, 0, 4), (0, 0, 3)))
                            {
                                GUILayout.Label("", GUILayout.Width(50));
                                GUILayout.Label("From", AARStyle.replaceFromTo);
                                GUILayout.Label("To", AARStyle.replaceFromTo);
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
                                        countTotal = countTotal + sp.count;

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
                                if (GUILayout.Button("Apply", GUILayout.Width(75)))
                                {
                                    AARManual.ClipEditing.RenameClipPathsMultiple(clipsTarget);
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
                                    EditorGUILayout.HelpBox("One or more paths contain the input string multiple times within it. Pressing apply replaces all instances of the string within the paths!", MessageType.Warning);
                                }
                            }

                            using (new SqueezeScope((20, 15, 4)))
                            {
                                UnityEngine.Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(10));
                                EditorGUI.DrawRect(new UnityEngine.Rect(r.x, r.y, r.width, 2), new Color(0.5f, 0.5f, 0.5f));
                            }

                            using (new SqueezeScope((0, 10, 4), (0, 0, 3)))
                            {
                                GUILayout.Label("<color=#ffffff>Replace paths individually</color>", AARStyle.replacePath);
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
                                    if (GUILayout.Button("Apply", GUILayout.Width(75)))
                                    {
                                        AARManual.ClipEditing.RenameClipPathsSingle(sp);
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
                            GUILayout.Label("No Animation Clips selected", AARStyle.invalidPathTip);
                        }
                        else
                        {
                            GUILayout.Label("You can drag and drop GameObjects into the text fields", AARStyle.invalidPathTip);
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
                    GUILayout.Label("<b>Settings</b>", AARStyle.settings);

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(AARContent.resetIcon, GUILayout.Height(30), GUILayout.Width(30)))
                    {
                        ResetData();
                    }
                }

                using (new SqueezeScope((15, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 5, 4), (5, 5, 3)))
                    {
                        GUILayout.Label("Target", GUILayout.Width(150));
                        controllerSelection = EditorGUILayout.Popup(controllerSelection, controllerOptions);
                    }

                    if (controllerSelection == 0)
                    {
                        using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
                        {
                            GUILayout.Label("Controller to use", GUILayout.Width(150));
                            animator = (Animator)EditorGUILayout.ObjectField(animator, typeof(Animator), true);
                        }
                    }
                    else
                    {
                        using (new SqueezeScope((0, 10, 4), (5, 5, 3)))
                        {
                            GUILayout.Label("Avatar to use", GUILayout.Width(150));
                            avatar = (GameObject)EditorGUILayout.ObjectField(avatar, typeof(GameObject), true);
                        }
#if VRC_SDK_VRCSDK3
                        if (avatar != null && avatar.GetComponent<VRCAvatarDescriptor>() != null)
                        {
                            using (new SqueezeScope((-5, 10, 4), (5, 5, 3)))
                            {
                                GUILayout.Label("Layer to target", GUILayout.Width(150));
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
                        renameActive = GUILayout.Toggle(renameActive, "Repath when renamed");
                        reparentActive = GUILayout.Toggle(reparentActive, "Repath when reparented");
                    }

                    using (new SqueezeScope((5, 10, 4), (5, 5, 3)))
                    {
                        renameWarning = GUILayout.Toggle(renameWarning, "Warn when renamed");
                        reparentWarning = GUILayout.Toggle(reparentWarning, "Warn when reparented");
                    }
                }

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                    {
                        activeInBackground = GUILayout.Toggle(activeInBackground, "Run when window is closed");
                    }
                }

                using (new SqueezeScope((10, 0, 4), (0, 0, 4, EditorStyles.helpBox)))
                {
                    using (new SqueezeScope((10, 10, 4), (5, 5, 3)))
                    {
                        GUILayout.Label("Language", GUILayout.Width(150));
                        languageSelection = EditorGUILayout.Popup(languageSelection, languageOptions);
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
                    //sp.newPath = p;
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
