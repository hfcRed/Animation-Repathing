using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static AutoAnimationRepath.AARVariables;

namespace AutoAnimationRepath
{
    public class AARVariables
    {
        public static bool automaticIsEnabled;
        public static bool renameActive = true;
        public static bool reparentActive = true;
        public static bool renameWarning = true;
        public static bool reparentWarning = true;
        public static bool activeInBackground = false;
        public static bool disableTooltips = false;

        public static int toolSelection;
        public static int manualToolSelection;
        public static int controllerSelection;
        public static int languageSelection;

        public class InvalidSharedProperty
        {
            public HashSet<AnimationClip> foldoutClips = new HashSet<AnimationClip>();
            public bool foldout;
            public string oldPath;
            public string newPath;
            public int count;
        }
        public static List<InvalidSharedProperty> invalidSharedProperties = new List<InvalidSharedProperty>();
        public static Dictionary<string, InvalidSharedProperty> invalidPathToSharedProperty = new Dictionary<string, InvalidSharedProperty>();
        public static int invalidPosition;

        public class ClipsSharedProperty
        {
            public HashSet<AnimationClip> foldoutClips = new HashSet<AnimationClip>();
            public bool foldout;
            public string oldPath;
            public string newPath;
            public int count;
            public bool warning;
        }
        public static List<ClipsSharedProperty> clipsSharedProperties = new List<ClipsSharedProperty>();
        public static Dictionary<string, ClipsSharedProperty> clipsPathToSharedProperty = new Dictionary<string, ClipsSharedProperty>();
        public static List<AnimationClip> clipsSelected = new List<AnimationClip>();
        public static string clipsReplaceFrom = string.Empty;
        public static string clipsReplaceTo = string.Empty;
        public static bool clipsReplaceFoldout;

        public static readonly List<AARAutomatic.HierarchyTransform> hierarchyTransforms = new List<AARAutomatic.HierarchyTransform>();
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
                    AARAutomatic.OnRootChanged();
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
                    AARAutomatic.OnRootChanged();
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
    }

    [InitializeOnLoad]
    public static class AARSaveData
    {
        static AARSaveData()
        {
            if (!EditorPrefs.HasKey("AARFirstStartup"))
            {
                ResetGeneralData();
                ResetAutomaticData();
                EditorPrefs.SetBool("AARFirstStartup", true);
            }

            EditorApplication.delayCall += () => { LoadData(); };

            if (languageSelection == 0)
            {
                AARStrings.loadEnglisch();
            }
            else
            {
                AARStrings.loadJapanese();
            }
        }

        public static void SaveData()
        {
            EditorPrefs.SetBool("AAREnabled", automaticIsEnabled);
            EditorPrefs.SetBool("AARRenameActive", renameActive);
            EditorPrefs.SetBool("AARReparentActive", reparentActive);
            EditorPrefs.SetBool("AARRenameWarning", renameWarning);
            EditorPrefs.SetBool("AARReparentWarning", reparentWarning);
            EditorPrefs.SetBool("AARRunInBackground", activeInBackground);
            EditorPrefs.SetBool("AARTooltips", disableTooltips);

            EditorPrefs.SetInt("AARToolSelection", toolSelection);
            EditorPrefs.SetInt("AARManualToolSelection", manualToolSelection);
            EditorPrefs.SetInt("AARControllerSelection", controllerSelection);
            EditorPrefs.SetInt("AARLanguageSelection", languageSelection);
            EditorPrefs.SetInt("AARPlayableSelection", (int)PlayableSelection);

            EditorPrefs.SetString("AARAvatar", avatar == null ? null : avatar.name);
            EditorPrefs.SetString("AARController", animator == null ? null : animator.gameObject.name);
        }

        public static void LoadData()
        {
            automaticIsEnabled = EditorPrefs.GetBool("AAREnabled");
            renameActive = EditorPrefs.GetBool("AARRenameActive");
            reparentActive = EditorPrefs.GetBool("AARReparentActive");
            renameWarning = EditorPrefs.GetBool("AARRenameWarning");
            reparentWarning = EditorPrefs.GetBool("AARReparentWarning");
            activeInBackground = EditorPrefs.GetBool("AARRunInBackground");
            disableTooltips = EditorPrefs.GetBool("AARTooltips");

            toolSelection = EditorPrefs.GetInt("AARToolSelection");
            manualToolSelection = EditorPrefs.GetInt("AARManualToolSelection");
            controllerSelection = EditorPrefs.GetInt("AARControllerSelection");
            languageSelection = EditorPrefs.GetInt("AARLanguageSelection");
            PlayableSelection = (Playables)EditorPrefs.GetInt("AARPlayableSelection");

            avatar = GameObject.Find(EditorPrefs.GetString("AARAvatar"));
            animator = GameObject.Find(EditorPrefs.GetString("AARController"))?.GetComponent<Animator>();
            controller = animator?.runtimeAnimatorController as AnimatorController;
        }

        public static void ResetGeneralData()
        {
            controllerSelection = 0;
            PlayableSelection = Playables.all;
            animator = null;
            avatar = null;

            SaveData();
        }

        public static void ResetAutomaticData()
        {
            renameActive = true;
            reparentActive = true;
            renameWarning = true;
            reparentWarning = true;
            activeInBackground = false;

            SaveData();
        }
    }

    public static class AARStrings
    {
        public static class Main
        {
            public static string windowName;
            public static string automatic;
            public static string manual;
        }

        public static class Automatic
        {
            public static string title;
            public static string disabled;
            public static string enabled;
        }

        public static class Manual
        {
            public static string title;
            public static string fixPaths;
            public static string editClips;
        }

        public static class InvalidPaths
        {
            public static string invalidPaths;
            public static string apply;
            public static string noInvalidPaths;
            public static string dragAndDrop;
        }

        public static class ClipEditing
        {
            public static string replacePartOfAll;
            public static string from;
            public static string to;
            public static string apply;
            public static string warning;
            public static string replaceIndividual;
            public static string noClipsSelected;
            public static string dragAndDrop;
        }

        public static class Settings
        {
            public static string settings;
            public static string general;
            public static string target;
            public static string animatorComponent;
            public static string vrchatAvatar;
            public static string controllerToUse;
            public static string layersToUse;
            public static string avatarToUse;
            public static string automatic;
            public static string repathWhenRenamed;
            public static string repathWhenReparented;
            public static string warnWhenRenamed;
            public static string warnWhenReparented;
            public static string runWhenWindowClosed;
            public static string disableTooltips;
            public static string language;
            public static string credit;
        }

        public static class ToolTips
        {
            public static string automatic;
            public static string manual;
            public static string toggleButton;
            public static string fixPaths;
            public static string editClips;
            public static string resetInvalidPaths;
            public static string resetInvalidPath;
            public static string applyValidPath;
            public static string replacePartOfAll;
            public static string resetPartOfAll;
            public static string replaceFrom;
            public static string replaceTo;
            public static string applyPartOfAll;
            public static string replaceIndividual;
            public static string resetIndividual;
            public static string applyIndividual;
            public static string resetSettings;
            public static string target;
            public static string controllerToUse;
            public static string avatarToUse;
            public static string layersToUse;
            public static string repathWhenRenamed;
            public static string repathWhenReparented;
            public static string warnWhenRenamed;
            public static string warnWhenReparented;
            public static string runWhenWindowClosed;
            public static string disableTooltips;
        }

        public static void loadEnglisch()
        {
            Main.windowName = "Animation Repathing";
            Main.automatic = "Automatic";
            Main.manual = "Manual";

            Automatic.title = "Automatic Animation Repathing";
            Automatic.disabled = "Disabled";
            Automatic.enabled = "Enabled";

            Manual.title = "Manual Animation Repathing";
            Manual.fixPaths = "Fix Invalid Paths";
            Manual.editClips = "Edit Animation Clips";

            InvalidPaths.invalidPaths = "Invalid Paths";
            InvalidPaths.apply = "Apply";
            InvalidPaths.noInvalidPaths = "You have no Invalid Paths in your controller";
            InvalidPaths.dragAndDrop = "You can drag and drop GameObjects into the text fields";

            ClipEditing.replacePartOfAll = "Replace part of all Paths";
            ClipEditing.from = "From";
            ClipEditing.to = "To";
            ClipEditing.apply = "Apply";
            ClipEditing.warning = "One or more Paths contain the input string multiple times within it. Pressing apply replaces all instances of the string within the Paths!";
            ClipEditing.replaceIndividual = "Replace Paths individually";
            ClipEditing.noClipsSelected = "No Animation Clips selected";
            ClipEditing.dragAndDrop = "You can drag and drop GameObjects into the text fields";

            Settings.settings = "Settings";
            Settings.general = "General";
            Settings.target = "Target";
            Settings.animatorComponent = "Animator Component";
            Settings.vrchatAvatar = "VRChat Avatar";
            Settings.controllerToUse = "Controller to use";
            Settings.layersToUse = "Layers to use";
            Settings.avatarToUse = "Avatar to use";
            Settings.automatic = "Automatic";
            Settings.repathWhenRenamed = "Repath when renamed";
            Settings.repathWhenReparented = "Repath when reparented";
            Settings.warnWhenRenamed = "Warn when renamed";
            Settings.warnWhenReparented = "Warn when reparented";
            Settings.runWhenWindowClosed = "Run when window is closed";
            Settings.disableTooltips = "Disable tooltips";
            Settings.language = "Language";
            Settings.credit = "Made by hfcRed";

            ToolTips.automatic = "Automatically change Animation Paths when something in the hierarchy gets changed";
            ToolTips.manual = "Manually change Animation Paths by hand";
            ToolTips.toggleButton = "Turn the automatic Animation Repathing on or off";
            ToolTips.fixPaths = "Repair broken (yellow) Animation Paths in the Animator used in the settings";
            ToolTips.editClips = "Directly change the Animation Paths of one or more Animation Clips";
            ToolTips.resetInvalidPaths = "Refresh the list of broken Animation Paths in the Animator used in the settings";
            ToolTips.resetInvalidPath = "Restore original Animation Path";
            ToolTips.applyValidPath = "Replace the broken Animation Path with the specified Path. Path can not be empty or the same as the broken Path";
            ToolTips.replacePartOfAll = "Replace a part of all Animation Paths in every selected Animation Clip with a specified string";
            ToolTips.resetPartOfAll = "Clear out both text fields";
            ToolTips.replaceFrom = "The part of all Animation Paths that you want to replace";
            ToolTips.replaceTo = "What you want the part of all Animation Paths to be replaced with";
            ToolTips.applyPartOfAll = "Replace the part of all Animation Paths with the specified string. \"Replace from\" can not be empty or the same as \"replace to\"";
            ToolTips.replaceIndividual = "Replace an entire Animation Path in every selected Animation Clip with a new one";
            ToolTips.resetIndividual = "Restore original Animation Path";
            ToolTips.applyIndividual = "Replace the old Animation Path with the specified Path. Path can not be empty or the same as the old Path";
            ToolTips.resetSettings = "Reset settings to their default values";
            ToolTips.target = "The object the tool should target. Can be set to either target the Animator Controller inside of the Animator Component of a Gameobject or target all Animator Controllers on a VRChat Avatar";
            ToolTips.controllerToUse = "The Gameobject which holds the Animator Component with the target Animator Controller";
            ToolTips.avatarToUse = "The VRChat Avatar which holds the target Animator Controllers";
            ToolTips.layersToUse = "All of the Animator Controllers on the VRChat Avatar that the tool should target";
            ToolTips.repathWhenRenamed = "Should the tool automatically change the Animation Paths if a Gameobject is renamed in the Hierarchy?";
            ToolTips.repathWhenReparented = "Should the tool automatically change the Animations Paths if a Gameobject is moved to a different spot in the Hierarchy?";
            ToolTips.warnWhenRenamed = "Should the tool show a pop-up message if a Gameobject is renamed in the Hierarchy?";
            ToolTips.warnWhenReparented = "Should the tool show a pop-up message if a Gameobject is moved to a different spot in the Hierarchy?";
            ToolTips.runWhenWindowClosed = "Should the tool still work even if the window has been closed?";
            ToolTips.disableTooltips = "Hide all Tooltips?";
        }

        public static void loadJapanese()
        {

        }

        public static void clearTooltips()
        {
            ToolTips.automatic = "";
            ToolTips.manual = "";
            ToolTips.toggleButton = "";
            ToolTips.fixPaths = "";
            ToolTips.editClips = "";
            ToolTips.resetInvalidPaths = "";
            ToolTips.resetInvalidPath = "";
            ToolTips.applyValidPath = "";
            ToolTips.replacePartOfAll = "";
            ToolTips.resetPartOfAll = "";
            ToolTips.replaceFrom = "";
            ToolTips.replaceTo = "";
            ToolTips.applyPartOfAll = "";
            ToolTips.replaceIndividual = "";
            ToolTips.resetIndividual = "";
            ToolTips.applyIndividual = "";
            ToolTips.resetSettings = "";
            ToolTips.target = "";
            ToolTips.controllerToUse = "";
            ToolTips.avatarToUse = "";
            ToolTips.layersToUse = "";
            ToolTips.repathWhenRenamed = "";
            ToolTips.repathWhenReparented = "";
            ToolTips.warnWhenRenamed = "";
            ToolTips.warnWhenReparented = "";
            ToolTips.runWhenWindowClosed = "";
            ToolTips.disableTooltips = "";
        }
    }
}
