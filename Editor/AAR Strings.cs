using System.Runtime.CompilerServices;

namespace AutoAnimationRepath
{
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
            public static string disableTooltips;
            public static string language;
            public static string automatic;
            public static string repathWhenRenamed;
            public static string repathWhenReparented;
            public static string warnWhenRenamed;
            public static string warnWhenReparented;
            public static string runWhenWindowClosed;
            public static string warnOnlyIfUsed;
            public static string warning;
            public static string credit;
        }

        public static class Popup
        {
            public static string title;
            public static string message;
            public static string to;
            public static string continuee;
            public static string cancel;
            public static string debug;
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
            public static string layersToUse;
            public static string avatarToUse;
            public static string disableTooltips;
            public static string repathWhenRenamed;
            public static string repathWhenReparented;
            public static string warnWhenRenamed;
            public static string warnWhenReparented;
            public static string runWhenWindowClosed;
            public static string warnOnlyIfUsed;
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
            InvalidPaths.noInvalidPaths = "You have no Invalid Paths in your controllers";
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
            Settings.controllerToUse = "Animator to use";
            Settings.layersToUse = "Layers to use";
            Settings.avatarToUse = "Avatar to use";
            Settings.disableTooltips = " Disable tooltips";
            Settings.language = "Language";
            Settings.automatic = "Automatic";
            Settings.repathWhenRenamed = " Repath when renamed";
            Settings.repathWhenReparented = " Repath when reparented";
            Settings.warnWhenRenamed = " Warn when renamed";
            Settings.warnWhenReparented = " Warn when reparented";
            Settings.runWhenWindowClosed = " Run when window is closed";
            Settings.warnOnlyIfUsed = " Warn only if used";
            Settings.warning = "This setting might cause slight lag when working with big Animator Controllers";
            Settings.credit = "Made by hfcRed";

            Popup.title = "Animation Repathing";
            Popup.message = "Hierarchy changes detected, would you like to repath your animations?";
            Popup.to = "  ==>  ";
            Popup.continuee = "Yes";
            Popup.cancel = "No";
            Popup.debug = "Animation Repathing:";

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
            ToolTips.layersToUse = "All of the Animator Controllers on the VRChat Avatar that the tool should target";
            ToolTips.avatarToUse = "The VRChat Avatar which holds the target Animator Controllers";
            ToolTips.disableTooltips = "Hide all Tooltips?";
            ToolTips.repathWhenRenamed = "Should the tool automatically change the Animation Paths if a Gameobject is renamed in the Hierarchy?";
            ToolTips.repathWhenReparented = "Should the tool automatically change the Animations Paths if a Gameobject is moved to a different spot in the Hierarchy?";
            ToolTips.warnWhenRenamed = "Should the tool show a pop-up message if a Gameobject is renamed in the Hierarchy?";
            ToolTips.warnWhenReparented = "Should the tool show a pop-up message if a Gameobject is moved to a different spot in the Hierarchy?";
            ToolTips.runWhenWindowClosed = "Should the tool still work even if the window has been closed?";
            ToolTips.warnOnlyIfUsed = "Should the tool only warn you if the affected GameObject is used in your Animator Controllers?";

            if (AARVariables.disableTooltips)
            {
                clearTooltips();
            }
        }

        public static void loadJapanese()
        {
            Main.windowName = "";
            Main.automatic = "";
            Main.manual = "";

            Automatic.title = "";
            Automatic.disabled = "";
            Automatic.enabled = "";

            Manual.title = "";
            Manual.fixPaths = "";
            Manual.editClips = "";

            InvalidPaths.invalidPaths = "";
            InvalidPaths.apply = "";
            InvalidPaths.noInvalidPaths = "";
            InvalidPaths.dragAndDrop = "";

            ClipEditing.replacePartOfAll = "";
            ClipEditing.from = "";
            ClipEditing.to = "";
            ClipEditing.apply = "";
            ClipEditing.warning = "";
            ClipEditing.replaceIndividual = "";
            ClipEditing.noClipsSelected = "";
            ClipEditing.dragAndDrop = "";

            Settings.settings = "";
            Settings.general = "";
            Settings.target = "";
            Settings.animatorComponent = "";
            Settings.vrchatAvatar = "";
            Settings.controllerToUse = "";
            Settings.layersToUse = "";
            Settings.avatarToUse = "";
            Settings.disableTooltips = "";
            Settings.language = "";
            Settings.automatic = "";
            Settings.repathWhenRenamed = "";
            Settings.repathWhenReparented = "";
            Settings.warnWhenRenamed = "";
            Settings.warnWhenReparented = "";
            Settings.runWhenWindowClosed = "";
            Settings.warnOnlyIfUsed = "";
            Settings.warning = "";
            Settings.credit = "";

            Popup.title = "";
            Popup.message = "";
            Popup.to = "";
            Popup.continuee = "";
            Popup.cancel = "";
            Popup.debug = "";

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
            ToolTips.layersToUse = "";
            ToolTips.avatarToUse = "";
            ToolTips.disableTooltips = "";
            ToolTips.repathWhenRenamed = "";
            ToolTips.repathWhenReparented = "";
            ToolTips.warnWhenRenamed = "";
            ToolTips.warnWhenReparented = "";
            ToolTips.runWhenWindowClosed = "";
            ToolTips.warnOnlyIfUsed = "";

            if (AARVariables.disableTooltips)
            {
                clearTooltips();
            }
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
