using System;
using UnityEditor;
using UnityEngine;
using static AutoAnimationRepath.AARVariables;

namespace AutoAnimationRepath
{
    [InitializeOnLoad]
    public static class AARSaveData
    {
        static AARSaveData()
        {
            if (!EditorPrefs.HasKey("AARFirstStartup"))
            {
                EditorPrefs.SetInt("AARFirstStartup", 1);
            }

            if (!PlayerPrefs.HasKey("AARFirstProjectStartup"))
            {
                ResetGeneralData();
                ResetAutomaticData();
                PlayerPrefs.SetInt("AARFirstProjectStartup", 1);
            }

            EditorApplication.delayCall += () => { LoadData(); };
        }

        public static void SaveData()
        {
            PlayerPrefs.SetInt("AAREnabled", automaticIsEnabled ? 1 : 0);
            PlayerPrefs.SetInt("AARRenameActive", renameActive ? 1 : 0);
            PlayerPrefs.SetInt("AARReparentActive", reparentActive ? 1 : 0);
            PlayerPrefs.SetInt("AARRenameWarning", renameWarning ? 1 : 0);
            PlayerPrefs.SetInt("AARReparentWarning", reparentWarning ? 1 : 0);
            PlayerPrefs.SetInt("AARRunInBackground", activeInBackground ? 1 : 0);
            PlayerPrefs.SetInt("AARTooltips", disableTooltips ? 1 : 0);
            PlayerPrefs.SetInt("AARWarnOnlyIfUsed", warnOnlyIfUsed ? 1 : 0);

            PlayerPrefs.SetInt("AARToolSelection", toolSelection);
            PlayerPrefs.SetInt("AARManualToolSelection", manualToolSelection);
            PlayerPrefs.SetInt("AARControllerSelection", controllerSelection);
            PlayerPrefs.SetInt("AARLanguageSelection", languageSelection);
            PlayerPrefs.SetInt("AARPlayableSelection", (int)PlayableSelection);

            PlayerPrefs.SetString("AARAvatar", avatar == null ? null : avatar.name);
            PlayerPrefs.SetString("AARController", animator == null ? null : animator.gameObject.name);
        }

        public static void LoadData()
        {
            automaticIsEnabled = PlayerPrefs.GetInt("AAREnabled") != 0;
            renameActive = PlayerPrefs.GetInt("AARRenameActive") != 0;
            reparentActive = PlayerPrefs.GetInt("AARReparentActive") != 0;
            renameWarning = PlayerPrefs.GetInt("AARRenameWarning") != 0;
            reparentWarning = PlayerPrefs.GetInt("AARReparentWarning") != 0;
            activeInBackground = PlayerPrefs.GetInt("AARRunInBackground") != 0;
            disableTooltips = PlayerPrefs.GetInt("AARTooltips") != 0;
            warnOnlyIfUsed = PlayerPrefs.GetInt("AARWarnOnlyIfUsed") != 0;

            toolSelection = PlayerPrefs.GetInt("AARToolSelection");
            manualToolSelection = PlayerPrefs.GetInt("AARManualToolSelection");
            controllerSelection = PlayerPrefs.GetInt("AARControllerSelection");
            languageSelection = PlayerPrefs.GetInt("AARLanguageSelection");
            PlayableSelection = (Playables)PlayerPrefs.GetInt("AARPlayableSelection");

            avatar = GameObject.Find(PlayerPrefs.GetString("AARAvatar"));
            animator = GameObject.Find(PlayerPrefs.GetString("AARController"))?.GetComponent<Animator>();

            if (languageSelection == 0) AARStrings.loadEnglisch(); else AARStrings.loadJapanese();
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
            warnOnlyIfUsed = true;

            SaveData();
        }
    }
}
