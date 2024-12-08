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
            public static string newVersion;
            public static string general;
            public static string target;
            public static string animatorComponent;
            public static string vrchatAvatar;
            public static string controllerToUse;
            public static string missingController;
            public static string avatarToUse;
            public static string missingDescriptor;
            public static string layersToUse;
            public static string useRootController;
            public static string useRootInfo;
            public static string disableTooltips;
            public static string language;
            public static string automatic;
            public static string warningPopup;
            public static string warnOnlyIfUsed;
            public static string warning;
            public static string runWhenWindowClosed;
            public static string credit;
            public static string docs;
            public static string disableLogging;
            public static string checkNewVersion;
            public static string noNewVersion;
            public static string yesNewVersion;
            public static string cantGetVersion;
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

        public static void ReloadLanguage(int languageSelection)
        {
            switch (languageSelection)
            {
                case 0:
                    LoadEnglish();
                    break;
                case 1:
                    LoadJapanese();
                    break;
                case 2:
                    LoadKorean();
                    break;
                case 3:
                    LoadSimplifiedChinese();
                    break;
                default:
                    LoadEnglish();
                    break;
            }
        }

        public static (string, string) GetLanguageCredits(int languageSelection)
        {
            switch (languageSelection)
            {
                case 1:
                    return ("Translation by Potato", "https://twitter.com/potatovrc");
                case 2:
                    return ("Translation by Neuru", "https://twitter.com/Neuru5278");
                case 3:
                    return ("Translation by ColorlessColor", null);
                default:
                    return (null, null);
            }
        }

        public static void LoadEnglish()
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
            InvalidPaths.noInvalidPaths = "You have no Invalid Paths in your Controllers";
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
            Settings.newVersion = "New Version Available!";
            Settings.general = "General";
            Settings.target = "Target";
            Settings.animatorComponent = "Animator Component";
            Settings.vrchatAvatar = "VRChat Avatar";
            Settings.controllerToUse = "Animator to use";
            Settings.missingController = "No Animator Controller assigned!";
            Settings.avatarToUse = "Avatar to use";
            Settings.missingDescriptor = "No Avatar Descriptor assigned!";
            Settings.layersToUse = "Layers to use";
            Settings.useRootController = "Use Controller from root Animator";
            Settings.useRootInfo = " This setting is slightly slower than setting the target manually";
            Settings.disableTooltips = " Disable tooltips";
            Settings.language = "Language";
            Settings.automatic = "Automatic";
            Settings.warningPopup = " Warn on hierarchy change";
            Settings.warnOnlyIfUsed = " Warn only if used";
            Settings.warning = "This setting might cause slight lag when working with big Animator Controllers";
            Settings.runWhenWindowClosed = " Run when window is closed";
            Settings.credit = "Made by hfcRed";
            Settings.docs = "Documentation";
            Settings.disableLogging = "Disable console logging";
            Settings.checkNewVersion = " Check for new version";
            Settings.noNewVersion = " No new version available";
            Settings.yesNewVersion = " New version available! Download it at the top";
            Settings.cantGetVersion = " Could not fetch newest version";

            Popup.title = "Animation Repathing";
            Popup.message = "Hierarchy changes detected, would you like to Repath your Animations?";
            Popup.to = "  -->  ";
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
            ToolTips.applyPartOfAll = "Replace the part of all Animation Paths with the specified string. \"Replace From\" can not be empty or the same as \"Replace To\"";
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
            Main.automatic = " オートマティック（自動）";
            Main.manual = " マニュアル（手動）";

            Automatic.title = "自動アニメーションリパス";
            Automatic.disabled = "無効";
            Automatic.enabled = "有効";

            Manual.title = "手動アニメーションリパス";
            Manual.fixPaths = " 無効パスの修正";
            Manual.editClips = " アニメーションクリップを修正";

            InvalidPaths.invalidPaths = "無効パス";
            InvalidPaths.apply = "適用";
            InvalidPaths.noInvalidPaths = "Controllerに無効パスがありません";
            InvalidPaths.dragAndDrop = "テキストフィールドにGameObjectをドラッグ＆ドロップすることができます";

            ClipEditing.replacePartOfAll = "すべてのパスの一部を置き換える";
            ClipEditing.from = "始点";
            ClipEditing.to = "終点";
            ClipEditing.apply = "適用";
            ClipEditing.warning = "入力した文字列がひとつ、または複数のパスに一回以上含まれております。適用した場合は全て置き換えますのでご注意ください。";
            ClipEditing.replaceIndividual = "個別にパスを置き換える";
            ClipEditing.noClipsSelected = "選択中のアニメーションクリップがありません";
            ClipEditing.dragAndDrop = "テキストフィールドにGameObjectをドラッグ＆ドロップすることができます";

            Settings.settings = "設定";
            Settings.newVersion = "新しいバージョンがあります";
            Settings.general = "一般";
            Settings.target = "ターゲット";
            Settings.animatorComponent = "Animator Component";
            Settings.vrchatAvatar = "VRChatアバター";
            Settings.controllerToUse = "Animatorの指定";
            Settings.missingController = "Animator Controllerが設定されていません！";
            Settings.avatarToUse = "使用アバター";
            Settings.missingDescriptor = "Avatar Descriptorが設定されていません！";
            Settings.layersToUse = "使用レイヤー";
            Settings.useRootController = "Root AnimatorのControllerを使用";
            Settings.useRootInfo = " この設定をご利用の場合は動作が少し遅くなります";
            Settings.disableTooltips = " ツールチップを非表示にする";
            Settings.language = "言語設定";
            Settings.automatic = "自動";
            Settings.warningPopup = " Hierarchyの変更警告";
            Settings.warnOnlyIfUsed = " 使用の際のみ警告";
            Settings.warning = "複雑なAnimator Controllerを使用する際にこの設定を有効化していると多少ラグが生じる可能性があります。";
            Settings.runWhenWindowClosed = " ウィンドウ非表示時にも実行を継続する";
            Settings.credit = "hfcRed制作";
            Settings.docs = "ドキュメンテーション";
            Settings.disableLogging = "コンソールへのログの書き出しを無効にする";
            Settings.checkNewVersion = " 更新の確認";
            Settings.noNewVersion = " 現在利用可能なバージョンアップデートはありません";
            Settings.yesNewVersion = " 新バージョンが利用可能です！上からダウンロードしてください。";
            Settings.cantGetVersion = " アップデートの取得に失敗しました";

            Popup.title = "アニメーションのリパス";
            Popup.message = "Hierarchyへの変更が検出されました。アニメーションのリパスを行いますか？";
            Popup.to = "  -->  ";
            Popup.continuee = "はい";
            Popup.cancel = "いいえ";
            Popup.debug = "アニメーションリパス:";

            ToolTips.automatic = "Hierarchyの変更が検出された際、自動的にアニメーションパスを変更";
            ToolTips.manual = "手動でアニメーションパスを変更";
            ToolTips.toggleButton = "自動アニメーションリパスの有効化・無効化";
            ToolTips.fixPaths = "設定されたAnimator内の破損しているアニメーションパス（黄色）を修復";
            ToolTips.editClips = "一つ、または複数のアニメーションクリップのアニメーションパスを直接変更";
            ToolTips.resetInvalidPaths = "設定されたAnimatorの破損しているアニメーションパスのリストの更新";
            ToolTips.resetInvalidPath = "オリジナルのアニメーションパスを復元";
            ToolTips.applyValidPath = "破損したアニメーションパスを指定したパスと置き換える。破損したパスとまったく同じパス、パスの指定なしは不可";
            ToolTips.replacePartOfAll = "選択したAnimation Clip内のアニメーションパスの一部またはすべてを指定したstringに置き換える";
            ToolTips.resetPartOfAll = "テキストフィールドを両方クリアする";
            ToolTips.replaceFrom = "全アニメーションパスにて置き換えたい部分の指定";
            ToolTips.replaceTo = "選択したアニメーションパスを置き換える内容";
            ToolTips.applyPartOfAll = "アニメーションパスの一部をすべて指定したstringに置き換える。\"始点・終点\" は空欄不可、そして \"始点\" との重複での設定も不可";
            ToolTips.replaceIndividual = "選択したAnimation Clipのアニメーションパスを丸ごと新しいものと置き換える";
            ToolTips.resetIndividual = "オリジナルのアニメーションパスを復元する";
            ToolTips.applyIndividual = "既存のアニメーションパスを指定したパスに置き換える。まったく同じパス、パスの指定なしは不可";
            ToolTips.resetSettings = "設定をデフォルトの状態に戻す";
            ToolTips.target = "ツールがターゲットするオブジェクト。GameObjectのAnimator Component内のAnimator Controller、またはVRChatアバターの全Animator Controllerを対象に設定できます。";
            ToolTips.controllerToUse = "ターゲットのAnimator Controllerが組み込まれているAnimator Componentを持ったGameObject";
            ToolTips.avatarToUse = "Animator Controllerが組み込まれているVRChatアバター";
            ToolTips.layersToUse = "VRChatのアバターに使用されているツールの対象のすべてのAnimator Controller";
            ToolTips.disableTooltips = "全ツールチップを非表示にしますか？";
            ToolTips.warningPopup = "Hierarchyの変更を感知した際、警告メッセージを表示しますか？";
            ToolTips.warnOnlyIfUsed = "Animator Controller内に変更中のGameObjectが使用されているのを感知した際に警告メッセージを表示しますか？";
            ToolTips.runWhenWindowClosed = "ウインドウを閉じてもツールの機能を有効化させますか？";

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
            Settings.newVersion = "New Version Available!";
            Settings.general = "일반 설정";
            Settings.target = "대상";
            Settings.animatorComponent = "애니메이터 컴포넌트";
            Settings.vrchatAvatar = "VRChat 아바타";
            Settings.controllerToUse = "사용할 애니메이터";
            Settings.missingController = "애니메이터 컨트롤러가 할당되지 않았습니다!";
            Settings.layersToUse = "사용할 레이어";
            Settings.avatarToUse = "사용할 아바타";
            Settings.missingDescriptor = "아바타 디스크립터가 할당되지 않았습니다!";
            Settings.useRootController = "Use Controller from root Animator";
            Settings.useRootInfo = " This setting is slightly slower than setting the target manually";
            Settings.disableTooltips = "툴팁 비활성화";
            Settings.language = "언어";
            Settings.automatic = "자동 수정 설정";
            Settings.warningPopup = "하이어라키 변경 시 경고 표시";
            Settings.warnOnlyIfUsed = "대상에 영향 줄 시에만 경고 표시";
            Settings.warning = "이 설정은 작업 중인 애니메이터 컨트롤러가 커지면 지연이 발생할 수 있습니다";
            Settings.runWhenWindowClosed = "창을 닫아도 계속 실행";
            Settings.credit = "제작자: hfcRed";
            Settings.docs = "문서";
            Settings.disableLogging = "Disable console logging";
            Settings.checkNewVersion = " Check for new version";
            Settings.noNewVersion = " No new version available";
            Settings.yesNewVersion = " New version available! Download it at the top";
            Settings.cantGetVersion = " Could not fetch newest version";

            Popup.title = "애니메이션 리패스";
            Popup.message = "하이어라키 변경이 감지되었습니다. 애니메이션 경로도 수정하시겠습니까?";
            Popup.to = "  -->  ";
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

        public static void LoadSimplifiedChinese()
        {
            Main.windowName = "动画路径重定向";
            Main.automatic = " 自动模式";
            Main.manual = " 手动模式";

            Automatic.title = "自动动画路径重定向";
            Automatic.disabled = "已禁用";
            Automatic.enabled = "已启用";

            Manual.title = "手动动画路径重定向";
            Manual.fixPaths = " 修复无效路径";
            Manual.editClips = " 编辑动画片段";

            InvalidPaths.invalidPaths = "无效路径";
            InvalidPaths.apply = "应用";
            InvalidPaths.noInvalidPaths = "Animator Controller 中没有无效路径";
            InvalidPaths.dragAndDrop = "可以将对象直接拖进文本框作为路径使用";

            ClipEditing.replacePartOfAll = "替换所有路径";
            ClipEditing.from = "源字符串";
            ClipEditing.to = "新字符串";
            ClipEditing.apply = "应用";
            ClipEditing.warning = "部分路径中包含多个指定的源字符串子串，按下应用将替换所有匹配到的子字符串！";
            ClipEditing.replaceIndividual = "单独替换路径";
            ClipEditing.noClipsSelected = "没有选中任何 Animation Clip";
            ClipEditing.dragAndDrop = "可以将对象直接拖进文本框作为路径使用";

            Settings.settings = "设置";
            Settings.newVersion = "新版本可用！";
            Settings.general = "常规设置";
            Settings.target = "目标";
            Settings.animatorComponent = "Animator 组件";
            Settings.vrchatAvatar = "VRChat Avatar";
            Settings.controllerToUse = "指定 Animator 组件";
            Settings.missingController = "指定对象的 Animator 上没有 Animator Controller！";
            Settings.avatarToUse = "指定 VCRChat Avatar";
            Settings.missingDescriptor = "指定的对象上没有 Avatar Descriptor！";
            Settings.layersToUse = "指定 Playable Layer";
            Settings.useRootController = "指定 Avatar 根 Animator 上的 Animator Controller";
            Settings.useRootInfo = " 与手动指定目标相比，该设置会造成轻微性能下降";
            Settings.disableTooltips = " 隐藏提示信息";
            Settings.language = "语言";
            Settings.automatic = "自动化";
            Settings.warningPopup = " 层级结构改变时弹出警告";
            Settings.warnOnlyIfUsed = " 按需弹出警告";
            Settings.warning = "如果 Animator Controller 很复杂，该功能会造成轻微性能下降";
            Settings.runWhenWindowClosed = " 窗口关闭时保持后台运行";
            Settings.credit = "由 hfcRed 编写";
            Settings.docs = "文档";
            Settings.disableLogging = "禁用控制台日志";
            Settings.checkNewVersion = " 检查更新";
            Settings.noNewVersion = " 没有新版本";
            Settings.yesNewVersion = " 新版本可用！在窗口顶部下载更新";
            Settings.cantGetVersion = " 无法获取新版本信息";

            Popup.title = "Animation Repathing";
            Popup.message = "检测到层级结构改变。是否修改所有受影响的动画路径？";
            Popup.to = "  -->  ";
            Popup.continuee = "是";
            Popup.cancel = "否";
            Popup.debug = "Animation Repathing:";

            ToolTips.automatic = "当层级结构改变时自动修改动画路径";
            ToolTips.manual = "手工修改动画路径";
            ToolTips.toggleButton = "打开或关闭自动动画路径重定向";
            ToolTips.fixPaths = "修复设置中指定目标包含的无效的（黄色）动画路径";
            ToolTips.editClips = "直接修改 Animation Clip 中的动画路径";
            ToolTips.resetInvalidPaths = "刷新设置中指定的 Animator Controller 中的存在的无效的动画路径列表";
            ToolTips.resetInvalidPath = "恢复原始动画路径";
            ToolTips.applyValidPath = "用指定路径替换无效的动画路径。新路径必须有效且不能为空";
            ToolTips.replacePartOfAll = "用指定字符串替换所有选中的 Animation Clip 中的所有动画路径";
            ToolTips.resetPartOfAll = "清除所有文本框";
            ToolTips.replaceFrom = "所有动画路径中需要被替换字符串";
            ToolTips.replaceTo = "所有动画路径中替换后的新字符串";
            ToolTips.applyPartOfAll = "用指定字符串替换所有动画路径。“源字符串”不能为空，也不能和“新字符串”相同";
            ToolTips.replaceIndividual = "将所有选中的 Animation Clip 中的所有的该动画路径替换为新路径";
            ToolTips.resetIndividual = "还原为原始动画路径";
            ToolTips.applyIndividual = "用指定路径替换所有的旧路径。“源字符串”不能为空，也不能和旧路径相同";
            ToolTips.resetSettings = "还原为默认设置";
            ToolTips.target = "该工具指定的目标。可以是特定对象上 Animator 组件中的 Animator Controller，也可以是 VRChat Avatar 上的所有 Animator Controller";
            ToolTips.controllerToUse = "带有设置了 Animator Controller 的 Animator 组件的对象";
            ToolTips.avatarToUse = "Playable Layer 设置中包含自定义 Animator Controller 的 VRChat Avatar";
            ToolTips.layersToUse = "VRChat Avatar 的 Playable Layer 设置中指定的 Animator Contrller";
            ToolTips.disableTooltips = "是否隐藏所有提示信息？";
            ToolTips.warningPopup = "如果层级结构有任何变化，是否应该弹出警告？";
            ToolTips.warnOnlyIfUsed = "是否在只在层级结构变化影响到设置中指定的 Animator Controller 时弹出警告？";
            ToolTips.runWhenWindowClosed = "窗口关闭后该工具是否仍然在后台工作？";

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
