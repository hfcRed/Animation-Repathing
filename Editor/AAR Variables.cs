using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using static AutoAnimationRepath.AARAutomatic;
using static AutoAnimationRepath.AARVariables;

namespace AutoAnimationRepath
{
    public class AARVariables
    {
        public static int toolSelection;
        public static string[] tools = { "Automatic", "Manual" };
        public static bool isEnabled;
        public static int manualToolSelection;
        public static string[] manualTools = { "Fix Invalid Paths", "Edit Animation Clips" };
        public static bool invalidPathsFoldout = true;
        public static int controllerSelection;
        public static string[] controllerOptions = { "Animator Component", "VRChat Avatar" };
        public static bool renameActive = true;
        public static bool reparentActive = true;
        public static bool renameWarning = true;
        public static bool reparentWarning = true;
        public static bool activeInBackground;
        public static int languageSelection;
        public static string[] languageOptions = { "English", "日本" };
        public static bool selectedClipsFoldout;

        public static readonly List<HierarchyTransform> hierarchyTransforms = new List<HierarchyTransform>();
        public static readonly Dictionary<string, string> changedPaths = new Dictionary<string, string>();
        public static AnimatorController controller;
        public static Animator _animator;
        public static GameObject _avatar;
        public static Animator animator
        {
            get => _animator;
            set
            {
                if (_animator != value)
                {
                    _animator = value;
                    OnRootChanged();
                }
            }
        }
        public static GameObject avatar
        {
            get => _avatar;
            set
            {
                if (_avatar != value)
                {
                    _avatar = value;
                    OnRootChanged();
                }
            }
        }
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

        public static List<List<AnimationClip>> invalidClips = new List<List<AnimationClip>>();
        public static List<bool> invalidFoldouts = new List<bool>();
        public static List<string> invalidOldPaths = new List<string>();
        public static List<string> invalidNewPaths = new List<string>();
        public static int invalidPosition;

        public static List<AnimationClip> clipsClips = new List<AnimationClip>();
        public static List<SharedProperty> sharedProperties = new List<SharedProperty>();
        public static Dictionary<string, SharedProperty> pathToSharedProperty = new Dictionary<string, SharedProperty>();
        public static string clipsReplaceFrom;
        public static string clipsReplaceTo = string.Empty;
        public static bool clipsFoldout;

        public static Vector2 scroll = Vector2.zero;
    }

    public class SharedProperty
    {
        public HashSet<AnimationClip> foldoutClips = new HashSet<AnimationClip>();
        public bool foldout;
        public string oldPath;
        public string newPath;
        public int count;
        public bool warning;
    }

    public static class AARStyle
    {
        public static GUIStyle title = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        public static GUIStyle toggleButton = new GUIStyle(GUI.skin.button) { fontSize = 16, richText = true };
        public static GUIStyle foldout = new GUIStyle(EditorStyles.foldout) { fontSize = 15, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        public static GUIStyle settings = new GUIStyle(GUI.skin.label) { fontSize = 20, richText = true };
        public static GUIStyle invalidPath = new GUIStyle(GUI.skin.box) { fontSize = 12, richText = true, stretchWidth = true, alignment = TextAnchor.MiddleLeft };
        public static GUIStyle invalidPathTip = new GUIStyle(GUI.skin.label) { fontSize = 12, richText = true, stretchWidth = true, alignment = TextAnchor.MiddleCenter };
        public static GUIStyle customFoldout = new GUIStyle(GUI.skin.button) { fontSize = 11, richText = true };
        public static GUIStyle resetButton = new GUIStyle(GUI.skin.label) { };
        public static GUIStyle replaceFromTo = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        public static GUIStyle replacePath = new GUIStyle(GUI.skin.box) { fontSize = 15, fontStyle = FontStyle.Bold, stretchWidth = true, alignment = TextAnchor.MiddleCenter, richText = true };
    }

    public static class AARContent
    {
        public static GUIContent linkIcon = new GUIContent(EditorGUIUtility.IconContent("UnityEditor.FindDependencies"));
        public static GUIContent resetIcon = new GUIContent(EditorGUIUtility.IconContent("TreeEditor.Refresh")) { tooltip = "Reset settings to default values" };
        public static GUIContent warningIcon = new GUIContent(EditorGUIUtility.IconContent("console.warnicon"));
        public static GUIContent passedIcon = new GUIContent(EditorGUIUtility.IconContent("TestPassed"));
    }

    public static class AARSettings
    {
        public static void SaveData()
        {
            EditorPrefs.SetInt("AAR BaseSelection", controllerSelection);
            EditorPrefs.SetInt("AAR LanguageSelection", languageSelection);

            EditorPrefs.SetBool("AAR Toggle", isEnabled);
            EditorPrefs.SetBool("AAR RenameActive", renameActive);
            EditorPrefs.SetBool("AAR ReparentActive", reparentActive);
            EditorPrefs.SetBool("AAR RenameWarning", renameWarning);
            EditorPrefs.SetBool("AAR ReparentWarning", reparentWarning);
            EditorPrefs.SetBool("AAR ActiveInBackground", activeInBackground);
            EditorPrefs.SetString("AAR Controller", animator == null ? null : animator.gameObject.transform.name);

            EditorPrefs.SetString("AAR Avatar", avatar == null ? null : avatar.name);

            EditorPrefs.SetInt("AAR PlayableSelection", (int)PlayableSelection);
            LoadData();
        }

        public static void LoadData()
        {
            controllerSelection = EditorPrefs.GetInt("AAR BaseSelection");

            isEnabled = EditorPrefs.GetBool("AAR Toggle");
            renameActive = EditorPrefs.GetBool("AAR RenameActive");
            reparentActive = EditorPrefs.GetBool("AAR ReparentActive");
            renameWarning = EditorPrefs.GetBool("AAR RenameWarning");
            reparentWarning = EditorPrefs.GetBool("AAR ReparentWarning");
            activeInBackground = EditorPrefs.GetBool("AAR ActiveInBackground");

            string findController = EditorPrefs.GetString("AAR Controller");
            Debug.Log(findController);

            GameObject animatorHolder = GameObject.Find(findController);
            if (animatorHolder != null)
            {
                animator = animatorHolder.GetComponent(typeof(Animator)) as Animator;
                controller = animator == null ? null : animator.runtimeAnimatorController as AnimatorController;
            }

            avatar = GameObject.Find(EditorPrefs.GetString("AAR Avatar"));

            PlayableSelection = (Playables)EditorPrefs.GetInt("AAR PlayableSelection");
        }

        public static void ResetData()
        {
            controllerSelection = 0;

            animator = null;
            avatar = null;

            isEnabled = false;
            renameActive = true;
            reparentActive = true;
            renameWarning = true;
            reparentWarning = true;
            activeInBackground = false;

            PlayableSelection = Playables.all;

            SaveData();
        }
    }


}
