using static AutoAnimationRepath.AARVariables;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace AutoAnimationRepath
{
    public class AARUpdater
    {
        public static string versionURL = "https://raw.githubusercontent.com/hfcRed/Animation-Repathing/main/version.txt";
        public static string downloadURL = "https://github.com/hfcRed/Animation-Repathing/releases/latest/download/Animation_Repathing.unitypackage";

        public static string GetNewestVersion()
        {
            try
            {
                var client = new System.Net.WebClient();
                return client.DownloadString(versionURL);
            }
            catch (System.Net.WebException e)
            {
                Debug.Log(e);
            }
            return null;
        }

        public static bool CheckForNewVersion()
        {
            newestVersion = GetNewestVersion()?.ToString();
            currentVersion = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath("687ec5539659fd440992163de55107d7"))?.text;

            if (newestVersion != null && newestVersion != currentVersion) return true;
            else return false;
        }

        public static void UpdateTool()
        {
            string outPath = "Assets/out.unitypackage";

            var client = new UnityWebRequest(downloadURL)
            {
                downloadHandler = new DownloadHandlerFile(outPath)
            };
            client.SendWebRequest().completed += AA =>
            {
                AssetDatabase.ImportAsset(outPath);
                AssetDatabase.ImportPackage(outPath, true);
                AssetDatabase.DeleteAsset(outPath);
                client.Dispose();
            };
        }
    }
}