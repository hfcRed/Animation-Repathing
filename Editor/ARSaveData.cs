using static AnimationRepathing.ARVariables;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace AnimationRepathing
{
    [InitializeOnLoad]
    public static class ARSaveData
    {
        static ARSaveData()
        {
            if (!EditorPrefs.HasKey("ARFirstStartup"))
            {
                var client = new UnityWebRequest("https://animation-repathing.hfcred.workers.dev/putuser");
                client.SendWebRequest().completed += AA =>
                {
                    client.Dispose();
                };

                EditorPrefs.SetInt("ARFirstStartup", 1);
            }

            if (!PlayerPrefs.HasKey("ARFirstProjectStartup"))
            {
                var client = new UnityWebRequest("https://animation-repathing.hfcred.workers.dev/putproject");
                client.SendWebRequest().completed += AA =>
                {
                    client.Dispose();
                };

                ResetGeneralData();
                ResetAutomaticData();
                PlayerPrefs.SetInt("ARFirstProjectStartup", 1);
                PlayerPrefs.SetInt("ARRepathCount", 0);
            }

            if (PlayerPrefs.GetInt("ARRepathCount") != 0)
            {
                var client = new UnityWebRequest("https://animation-repathing.hfcred.workers.dev/putrepathcount/" + PlayerPrefs.GetInt("ARRepathCount"));
                client.SendWebRequest().completed += AA =>
                {
                    client.Dispose();
                    PlayerPrefs.SetInt("ARRepathCount", 0);
                };
            }

            EditorApplication.delayCall += () => { LoadData(); };
        }

        public static void SaveData()
        {
            PlayerPrefs.SetInt("AREnabled", automaticIsEnabled ? 1 : 0);
            PlayerPrefs.SetInt("ARGetAutomatically", getControllerAutomatically ? 1 : 0);
            PlayerPrefs.SetInt("ARTooltips", disableTooltips ? 1 : 0);
            PlayerPrefs.SetInt("ARSendWarning", sendWarning ? 1 : 0);
            PlayerPrefs.SetInt("ARWarnOnlyIfUsed", warnOnlyIfUsed ? 1 : 0);
            PlayerPrefs.SetInt("ARRunInBackground", activeInBackground ? 1 : 0);
            PlayerPrefs.SetInt("ARDisableDebugLogging", disableDebugLogging ? 1 : 0);

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
            getControllerAutomatically = PlayerPrefs.GetInt("ARGetAutomatically") != 0;
            disableTooltips = PlayerPrefs.GetInt("ARTooltips") != 0;
            sendWarning = PlayerPrefs.GetInt("ARSendWarning") != 0;
            warnOnlyIfUsed = PlayerPrefs.GetInt("ARWarnOnlyIfUsed") != 0;
            activeInBackground = PlayerPrefs.GetInt("ARRunInBackground") != 0;
            disableDebugLogging = PlayerPrefs.GetInt("ARDisableDebugLogging") != 0;

            toolSelection = PlayerPrefs.GetInt("ARToolSelection");
            manualToolSelection = PlayerPrefs.GetInt("ARManualToolSelection");
            controllerSelection = PlayerPrefs.GetInt("ARControllerSelection");
            languageSelection = PlayerPrefs.GetInt("ARLanguageSelection");
            PlayableSelection = (Playables)PlayerPrefs.GetInt("ARPlayableSelection");

            ARVariables.Avatar = GameObject.Find(PlayerPrefs.GetString("ARAvatar"));
            ARVariables.Animator = GameObject.Find(PlayerPrefs.GetString("ARController"))?.GetComponent<Animator>();

            ARStrings.ReloadLanguage(languageSelection);

            ARUpdater.CheckForNewVersion();
        }

        public static void ResetGeneralData()
        {
            ARVariables.Animator = null;
            ARVariables.Avatar = null;
            PlayableSelection = Playables.all;
            getControllerAutomatically = false;
            SaveData();
        }

        public static void ResetAutomaticData()
        {
            sendWarning = true;
            warnOnlyIfUsed = true;
            activeInBackground = false;
            disableDebugLogging = true;
            SaveData();
        }
    }
}
