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
            Settings.missingController = "";
            Settings.avatarToUse = "";
            Settings.missingDescriptor = "";
            Settings.layersToUse = "";
            Settings.disableTooltips = "";
            Settings.language = "";
            Settings.automatic = "";
            Settings.warningPopup = "";
            Settings.warnOnlyIfUsed = "";
            Settings.warning = "";
            Settings.runWhenWindowClosed = "";
            Settings.credit = "";
            Settings.docs = "";

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
            ToolTips.avatarToUse = "";
            ToolTips.layersToUse = "";
            ToolTips.disableTooltips = "";
            ToolTips.warningPopup = "";
            ToolTips.warnOnlyIfUsed = "";
            ToolTips.runWhenWindowClosed = "";

            if (ARVariables.disableTooltips)
            {
                ClearTooltips();
            }
        }

        public static void LoadKorean()
        {
            Main.windowName = "애니메이션 리패스";
            Main.automatic = " 자동 수정";
            Main.manual = " 수동 변경";

            Automatic.title = "자동 애니메이션 리패스";
            Automatic.disabled = "비활성화";
            Automatic.enabled = "활성화";

            Manual.title = "수동 애니메이션 리패스";
            Manual.fixPaths = "손상된 경로 수정";
            Manual.editClips = "애니메이션 클립 편집";

            InvalidPaths.invalidPaths = "손상된 경로";
            InvalidPaths.apply = "적용";
            InvalidPaths.noInvalidPaths = "컨트롤러에 손상된 경로가 없습니다";
            InvalidPaths.dragAndDrop = "게임 오브젝트를 텍스트 필드에 끌어다 놓을 수 있습니다";

            ClipEditing.replacePartOfAll = "모든 경로 일괄 변경";
            ClipEditing.from = "기존 문자열 (From)";
            ClipEditing.to = "변경 문자열 (To)";
            ClipEditing.apply = "적용";
            ClipEditing.warning = "기존 문자열이 여러 번 포함된 경로가 존재합니다. 적용 버튼을 누르면 경로 내의 모든 기존 문자열이 변경됩니다!";
            ClipEditing.replaceIndividual = "개별 경로 변경";
            ClipEditing.noClipsSelected = "선택된 애니메이션 클립 없음";
            ClipEditing.dragAndDrop = "게임 오브젝트를 텍스트 필드로 끌어다 놓을 수 있습니다";

            Settings.settings = "설정";
            Settings.general = "일반 설정";
            Settings.target = "대상";
            Settings.animatorComponent = "애니메이터 컴포넌트";
            Settings.vrchatAvatar = "VRChat 아바타";
            Settings.controllerToUse = "사용할 애니메이터";
            Settings.missingController = "애니메이터 컨트롤러가 할당되지 않았습니다!";
            Settings.layersToUse = "사용할 레이어";
            Settings.avatarToUse = "사용할 아바타";
            Settings.missingDescriptor = "아바타 디스크립터가 할당되지 않았습니다!";
            Settings.disableTooltips = "툴팁 비활성화";
            Settings.language = "언어";
            Settings.automatic = "자동 수정 설정";
            Settings.warningPopup = "하이어라키 변경 시 경고 표시";
            Settings.warnOnlyIfUsed = "대상에 영향 줄 시에만 경고 표시";
            Settings.warning = "이 설정은 작업 중인 애니메이터 컨트롤러가 커지면 지연이 발생할 수 있습니다";
            Settings.runWhenWindowClosed = "창을 닫아도 계속 실행";
            Settings.credit = "제작자: hfcRed";
            Settings.docs = "문서";

            Popup.title = "애니메이션 리패스";
            Popup.message = "하이어라키 변경이 감지되었습니다. 애니메이션 경로도 수정하시겠습니까?";
            Popup.to = "  ==>  ";
            Popup.continuee = "예";
            Popup.cancel = "아니오";
            Popup.debug = "애니메이션 리패스:";

            ToolTips.automatic = "하이어라키 변경 시 애니메이션 경로를 자동으로 수정합니다";
            ToolTips.manual = "애니메이션 경로를 직접 수동으로 변경합니다";
            ToolTips.toggleButton = "자동 애니메이션 리패스를 켜거나 끕니다";
            ToolTips.fixPaths = "사용 중인 애니메이터에서 손상된 (노란색) 애니메이션 경로를 수정합니다";
            ToolTips.editClips = "여러 애니메이션 클립의 애니메이션 경로를 직접 변경합니다";
            ToolTips.resetInvalidPaths = "사용 중인 애니메이터에서 손상된 애니메이션 경로의 목록을 새로고침 합니다";
            ToolTips.resetInvalidPath = "기존 애니메이션 경로 복구";
            ToolTips.applyValidPath = "손상된 애니메이션 경로를 지정한 경로로 수정합니다. 경로는 비어 있거나 손상된 경로와 동일해서는 안 됩니다";
            ToolTips.replacePartOfAll = "선택된 애니메이션 클립 경로들의 일부 문자열을 지정한 문자열로 모두 변경합니다";
            ToolTips.resetPartOfAll = "두 텍스트 필드의 모든 내용을 지웁니다";
            ToolTips.replaceFrom = "모든 애니메이션 경로에서 '변경 문자열'로 대체될 문자열";
            ToolTips.replaceTo = "모든 애니메이션 경로에서 '기존 문자열'을 대체할 문자열";
            ToolTips.applyPartOfAll = "모든 애니메이션 경로의 일부 문자열을 지정한 문자열로 변경합니다. '기존 문자열'은 비어 있거나 '변경 문자열'과 동일해서는 안 됩니다";
            ToolTips.replaceIndividual = "선택한 애니메이션 클립의 전체 애니메이션 경로를 수정한 경로로 변경합니다";
            ToolTips.resetIndividual = "기존 애니메이션 경로 복원";
            ToolTips.applyIndividual = "기존 애니메이션 경로를 지정한 경로로 변경합니다. 경로는 비어 있거나 기존 경로와 동일해서는 안 됩니다";
            ToolTips.resetSettings = "설정을 기본 값으로 초기화합니다";
            ToolTips.target = "이 툴이 타기팅할 대상. 게임 오브젝트의 애니메이터 컴포넌트에 할당된 애니메이터 컨트롤러를 대상으로 할지, VRChat 아바타의 모든 애니메이터 컨트롤러를 대상으로 할지 설정";
            ToolTips.controllerToUse = "대상 애니메이터 컨트롤러가 포함된 게임 오브젝트";
            ToolTips.avatarToUse = "대상 애니메이터 컨트롤러가 포함된 VRChat 아바타";
            ToolTips.layersToUse = "대상이 될 VRChat 아바타의 애니메이터 컨트롤러";
            ToolTips.disableTooltips = "모든 툴팁을 숨깁니까?";
            ToolTips.warningPopup = "하이어라키 변경 시 경고창을 표시할까요?";
            ToolTips.warnOnlyIfUsed = "하이어라키 변경에 영향 받는 게임 오브젝트가 대상 애니메이터 컨트롤러에서 사용될 시에만 경고창을 표시할까요?";
            ToolTips.runWhenWindowClosed = "창이 닫혀도 툴을 계속 작동시킬까요?";

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
