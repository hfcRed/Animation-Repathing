namespace AnimationRepathing
{
    public static class ARStrings
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
            public static string missingController;
            public static string avatarToUse;
            public static string missingDescriptor;
            public static string layersToUse;
            public static string disableTooltips;
            public static string language;
            public static string automatic;
            public static string warningPopup;
            public static string warnOnlyIfUsed;
            public static string warning;
            public static string runWhenWindowClosed;
            public static string credit;
            public static string docs;
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
            public static string avatarToUse;
            public static string layersToUse;
            public static string disableTooltips;
            public static string warningPopup;
            public static string warnOnlyIfUsed;
            public static string runWhenWindowClosed;
        }

        public static void LoadEnglisch()
        {
            Main.windowName = "Animation Repathing";
            Main.automatic = " Automatic";
            Main.manual = " Manual";

            Automatic.title = "Automatic Animation Repathing";
            Automatic.disabled = "Disabled";
            Automatic.enabled = "Enabled";

            Manual.title = "Manual Animation Repathing";
            Manual.fixPaths = " Fix Invalid Paths";
            Manual.editClips = " Edit Animation Clips";

            InvalidPaths.invalidPaths = "Invalid Paths";
            InvalidPaths.apply = "Apply";
            InvalidPaths.noInvalidPaths = "You have no Invalid Paths in your controllers";
            InvalidPaths.dragAndDrop = "You can drag and drop GameObjects into the text fields";

            ClipEditing.replacePartOfAll = "Replace part of all Paths";
            ClipEditing.from = "From";
            ClipEditing.to = "To";
            ClipEditing.apply = "Apply";
            ClipEditing.warning = "One or more Paths contain the input string multiple times within them. Pressing apply replaces all instances of the input within the Paths!";
            ClipEditing.replaceIndividual = "Replace Paths individually";
            ClipEditing.noClipsSelected = "No Animation Clips selected";
            ClipEditing.dragAndDrop = "You can drag and drop GameObjects into the text fields";

            Settings.settings = "Settings";
            Settings.general = "General";
            Settings.target = "Target";
            Settings.animatorComponent = "Animator Component";
            Settings.vrchatAvatar = "VRChat Avatar";
            Settings.controllerToUse = "Animator to use";
            Settings.missingController = "No Animator Controller assigned!";
            Settings.avatarToUse = "Avatar to use";
            Settings.missingDescriptor = "No Avatar Descriptor assigned!";
            Settings.layersToUse = "Layers to use";
            Settings.disableTooltips = " Disable tooltips";
            Settings.language = "Language";
            Settings.automatic = "Automatic";
            Settings.warningPopup = " Warn on hierarchy change";
            Settings.warnOnlyIfUsed = " Warn only if used";
            Settings.warning = "This setting might cause slight lag when working with big Animator Controllers";
            Settings.runWhenWindowClosed = " Run when window is closed";
            Settings.credit = "Made by hfcRed";
            Settings.docs = "Documentation";

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
            ToolTips.avatarToUse = "The VRChat Avatar which holds the target Animator Controllers";
            ToolTips.layersToUse = "All of the Animator Controllers on the VRChat Avatar that the tool should target";
            ToolTips.disableTooltips = "Hide all Tooltips?";
            ToolTips.warningPopup = "Should the tool open a warning popup if any changes are made to the hierarchy?";
            ToolTips.warnOnlyIfUsed = "Should the tool only open the warning popup if the affected GameObject is used in your Animator Controllers?";
            ToolTips.runWhenWindowClosed = "Should the tool still work even if the window has been closed?";

            if (ARVariables.disableTooltips)
            {
                ClearTooltips();
            }
        }

        public static void LoadJapanese()
        {
            Main.windowName = "アニメーションリパス";
            Main.automatic = " 自動";
            Main.manual = " 手動";

            Automatic.title = "自動アニメーションリパス";
            Automatic.disabled = "無効";
            Automatic.enabled = "有効";

            Manual.title = "手動アニメーションリパス";
            Manual.fixPaths = " 無効なパスを修正";
            Manual.editClips = " アニメーションクリップを編集";

            InvalidPaths.invalidPaths = "無効なパス";
            InvalidPaths.apply = "適用";
            InvalidPaths.noInvalidPaths = "コントローラに無効なパスはありません";
            InvalidPaths.dragAndDrop = "テキストフィールドにゲームオブジェクトをドラッグアンドドロップできます";

            ClipEditing.replacePartOfAll = "すべてのパスの一部を置換する";
            ClipEditing.from = "From";
            ClipEditing.to = "To";
            ClipEditing.apply = "適用";
            ClipEditing.warning = "ひとつ以上のパスに、入力文字列が複数回含まれています。適用すると、パス内のすべての入力文字列が置換されます！";
            ClipEditing.replaceIndividual = "個別のパスを置換する";
            ClipEditing.noClipsSelected = "アニメーションクリップは選択されていません";
            ClipEditing.dragAndDrop = "テキストフィールドにゲームオブジェクトをドラッグアンドドロップできます";

            Settings.settings = "設定";
            Settings.general = "一般";
            Settings.target = "ターゲット";
            Settings.animatorComponent = "Animatorコンポーネント";
            Settings.vrchatAvatar = "VRChatアバター";
            Settings.controllerToUse = "使用するAnimator";
            Settings.missingController = "No Animator Controller assigned!";
            Settings.layersToUse = "使用するレイヤー";
            Settings.avatarToUse = "使用するアバター";
            Settings.missingDescriptor = "No Avatar Descriptor assigned!";
            Settings.disableTooltips = "ツールチップを無効にする";
            Settings.language = "言語";
            Settings.automatic = "自動";
            //
            Settings.warnOnlyIfUsed = " 使用される場合のみ警告する";
            Settings.warning = "この設定は、大きなAnimatorコントローラと作業する際にわずかな遅延を引き起こす可能性があります";
            Settings.runWhenWindowClosed = " ウィンドウが閉じられた時に実行する";
            Settings.credit = "hfcRed作成";
            Settings.docs = "ドキュメント";

            Popup.title = "アニメーションリパス";
            Popup.message = "階層の変更が検出されました。アニメーションのリパスを行いますか？";
            Popup.to = "  ==>  ";
            Popup.continuee = "はい";
            Popup.cancel = "いいえ";
            Popup.debug = "アニメーションリパス:";

            ToolTips.automatic = "階層内の何かが変更された際に、自動的にアニメーションパスを変更します";
            ToolTips.manual = "手動でアニメーションパスを変更します";
            ToolTips.toggleButton = "自動アニメーションリパスをオンまたはオフにします";
            ToolTips.fixPaths = "設定で使用されるAnimator内の壊れた（黄色い）アニメーションパスを修復します";
            ToolTips.editClips = "1つ以上のアニメーションクリップのアニメーションパスを直接変更します";
            ToolTips.resetInvalidPaths = "設定で使用されるAnimator内の壊れたアニメーションパスのリストを更新します";
            ToolTips.resetInvalidPath = "元のアニメーションパスを復元します";
            ToolTips.applyValidPath = "壊れたアニメーションパスを指定したパスで置き換えます。パスは空白であってはならず、壊れたパスと同じであってはなりません";
            ToolTips.replacePartOfAll = "選択したすべてのアニメーションクリップのすべてのアニメーションパスの一部を指定した文字列で置き換えます";
            ToolTips.resetPartOfAll = "両方のテキストフィールドを空にします";
            ToolTips.replaceFrom = "置換するアニメーションパスの一部";
            ToolTips.replaceTo = "アニメーションパスの一部が置換される値";
            ToolTips.applyPartOfAll = "すべてのアニメーションパスの一部を指定した文字列で置き換えます。「置換元」は空白であってはならず、置換先と同じであってはなりません";
            ToolTips.replaceIndividual = "選択したすべてのアニメーションクリップのすべてのアニメーションパスを新しいものに置き換えます";
            ToolTips.resetIndividual = "元のアニメーションパスを復元します";
            ToolTips.applyIndividual = "古いアニメーションパスを指定したパスで置き換えます。パスは空白であってはならず、古いパスと同じであってはなりません";
            ToolTips.resetSettings = "設定をデフォルト値にリセットします";
            ToolTips.target = "ツールの対象とするオブジェクト。ゲームオブジェクトのAnimator Component内のAnimator Controllerを対象とするか、VRChat Avatar内のすべてのAnimator Controllerを対象とすることができます";
            ToolTips.controllerToUse = "アニメーターコンポーネントを保持するゲームオブジェクトで、対象のAnimator Controllerが含まれています";
            ToolTips.avatarToUse = "対象のAnimator Controllerを保持するVRChat Avatar";
            ToolTips.layersToUse = "ツールの対象とするVRChat AvatarのすべてのAnimator Controller";
            ToolTips.disableTooltips = "すべてのツールチップを非表示にしますか？";
            ToolTips.warnOnlyIfUsed = "ツールは、影響を受けるゲームオブジェクトがAnimator Controllerで使用されている場合にのみ警告しますか？";
            //
            ToolTips.runWhenWindowClosed = "ウィンドウが閉じられた場合でも、ツールは動作する必要がありますか？";

            if (ARVariables.disableTooltips)
            {
                ClearTooltips();
            }
        }

        public static void ClearTooltips()
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
            ToolTips.disableTooltips = "";
            ToolTips.warningPopup = "";
            ToolTips.warnOnlyIfUsed = "";
            ToolTips.runWhenWindowClosed = "";
        }
    }
}
