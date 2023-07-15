using static AnimationRepathing.ARVariables;
using UnityEditor;
using UnityEngine;

namespace AnimationRepathing
{
    [InitializeOnLoad]
    public static class ARSaveData
    {
        static ARSaveData()
        {
            if (!EditorPrefs.HasKey("ARFirstStartup"))
            {
                EditorPrefs.SetInt("ARFirstStartup", 1);
            }

            if (!PlayerPrefs.HasKey("ARFirstProjectStartup"))
            {
                ResetGeneralData();
                ResetAutomaticData();
                PlayerPrefs.SetInt("ARFirstProjectStartup", 1);
            }

            EditorApplication.delayCall += () => { LoadData(); };
        }

        public static void SaveData()
        {
            PlayerPrefs.SetInt("AREnabled", automaticIsEnabled ? 1 : 0);
            PlayerPrefs.SetInt("ARRenameActive", renameActive ? 1 : 0);
            PlayerPrefs.SetInt("ARReparentActive", reparentActive ? 1 : 0);
            PlayerPrefs.SetInt("ARRenameWarning", renameWarning ? 1 : 0);
            PlayerPrefs.SetInt("ARReparentWarning", reparentWarning ? 1 : 0);
            PlayerPrefs.SetInt("ARRunInBackground", activeInBackground ? 1 : 0);
            PlayerPrefs.SetInt("ARTooltips", disableTooltips ? 1 : 0);
            PlayerPrefs.SetInt("ARWarnOnlyIfUsed", warnOnlyIfUsed ? 1 : 0);

            PlayerPrefs.SetInt("ARToolSelection", toolSelection);
            PlayerPrefs.SetInt("ARManualToolSelection", manualToolSelection);
            PlayerPrefs.SetInt("ARControllerSelection", controllerSelection);
            PlayerPrefs.SetInt("ARLanguageSelection", languageSelection);
            PlayerPrefs.SetInt("ARPlayableSelection", (int)PlayableSelection);

            PlayerPrefs.SetString("ARAvatar", ARVariables.Avatar?.name);
            PlayerPrefs.SetString("ARController", ARVariables.Animator?.gameObject.name);
        }

        public static void LoadData()
        {
            automaticIsEnabled = PlayerPrefs.GetInt("AREnabled") != 0;
            renameActive = PlayerPrefs.GetInt("ARRenameActive") != 0;
            reparentActive = PlayerPrefs.GetInt("ARReparentActive") != 0;
            renameWarning = PlayerPrefs.GetInt("ARRenameWarning") != 0;
            reparentWarning = PlayerPrefs.GetInt("ARReparentWarning") != 0;
            activeInBackground = PlayerPrefs.GetInt("ARRunInBackground") != 0;
            disableTooltips = PlayerPrefs.GetInt("ARTooltips") != 0;
            warnOnlyIfUsed = PlayerPrefs.GetInt("ARWarnOnlyIfUsed") != 0;

            toolSelection = PlayerPrefs.GetInt("ARToolSelection");
            manualToolSelection = PlayerPrefs.GetInt("ARManualToolSelection");
            controllerSelection = PlayerPrefs.GetInt("ARControllerSelection");
            languageSelection = PlayerPrefs.GetInt("ARLanguageSelection");
            PlayableSelection = (Playables)PlayerPrefs.GetInt("ARPlayableSelection");

            ARVariables.Avatar = GameObject.Find(PlayerPrefs.GetString("ARAvatar"));
            ARVariables.Animator = GameObject.Find(PlayerPrefs.GetString("ARController"))?.GetComponent<Animator>();

            if (languageSelection == 0) ARStrings.LoadEnglisch(); else ARStrings.LoadJapanese();

            ARAutomatic.GetAllChildren();
            availableUpdate = ARUpdater.CheckForNewVersion();
        }

        public static void ResetGeneralData()
        {
            PlayableSelection = Playables.all;
            ARVariables.Animator = null;
            ARVariables.Avatar = null;
            SaveData();
        }

        public static void ResetAutomaticData()
        {
            renameActive = true;
            reparentActive = true;
            renameWarning = true;
            reparentWarning = true;
            activeInBackground = false;
            warnOnlyIfUsed = true;
            SaveData();
        }
    }
}
