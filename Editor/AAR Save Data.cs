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
            //controller = animator?.runtimeAnimatorController as AnimatorController;
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
}
