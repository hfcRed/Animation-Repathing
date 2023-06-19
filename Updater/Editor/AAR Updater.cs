using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static AutoAnimationRepath.AARVariables;

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
                using (var client = new System.Net.WebClient())
                {
                    return client.DownloadString(versionURL);
                }
            }
            catch (System.Net.WebException e)
            {
                Debug.Log(e);
            }
            return null;
        }

        public static bool CheckForNewVersion()
        {
            newestVersion = GetNewestVersion();
            currentVersion = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath("687ec5539659fd440992163de55107d7")).text;

            if (newestVersion != null && newestVersion != currentVersion)
            {
                return true;
            }
            return false;
        }

        public static void UpdateTool()
        {
            string outPath = "Assets/out.unitypackage";

            try
            {
                var client = new UnityWebRequest(downloadURL);

                client.downloadHandler = new DownloadHandlerFile(outPath);
                client.SendWebRequest().completed += AA =>
                {
                    AssetDatabase.ImportAsset(outPath);
                    AssetDatabase.ImportPackage(outPath, true);
                    AssetDatabase.importPackageStarted += AB =>
                    {
                        string[] toDelete = AssetDatabase.FindAssets("", new string[] { AssetDatabase.GUIDToAssetPath("235e4dc8aed3ccb4d810a6b68c176559") });
                        foreach (string s in toDelete)
                        {
                            if (s != null)
                            {
                                var path = AssetDatabase.GUIDToAssetPath(s);
                                AssetDatabase.DeleteAsset(path);
                            }
                        }
                    };
                    if (outPath != null) AssetDatabase.DeleteAsset(outPath);
                    client.Dispose();
                };
            }
            catch (System.Net.WebException e)
            {
                Debug.Log(e);
            }
        }
    }
}