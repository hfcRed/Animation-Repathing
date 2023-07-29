using static AnimationRepathing.ARVariables;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace AnimationRepathing
{
    public class ARUpdater
    {
        public static string versionURL = "https://raw.githubusercontent.com/hfcRed/Animation-Repathing/main/com.hfcred.animationrepathing/package.json";
        public static string downloadURL = "https://github.com/hfcRed/Animation-Repathing/releases/latest/download/AnimationRepathing_v";
        public static string packagePath = "Assets/AROutput.unitypackage";

        public static List<string> assetsToDelete = new List<string>();
        public static List<string> metasToDelete = new List<string>();

        public static bool CheckForNewVersion()
        {
            newestVersion = string.Empty;
            currentVersion = string.Empty;

            currentVersion = GetCurrentVersion()?.ToString();
            newestVersion = GetNewestVersion()?.ToString();

            if (newestVersion != null && newestVersion != currentVersion) return true;
            else return false;
        }

        [System.Serializable]
        public class JsonData
        {
            public string version;
        }

        public static string GetCurrentVersion()
        {
            var editor = ScriptableObject.CreateInstance<AREditor>();
            string path = editor.GetScriptPath();

            string folderPath = Path.GetDirectoryName(path);
            string parentFolder = Path.GetDirectoryName(folderPath);
            string jsonText = File.ReadAllText(parentFolder + "/package.json");

            JsonData data = JsonUtility.FromJson<JsonData>(jsonText);
            return data.version;
        }

        public static string GetNewestVersion()
        {
            try
            {
                var client = new System.Net.WebClient();
                string jsonText = client.DownloadString(versionURL);

                JsonData data = JsonUtility.FromJson<JsonData>(jsonText);
                return data.version;
            }
            catch (System.Net.WebException e)
            {
                Debug.Log(e);
            }
            return null;
        }

        public static void UpdateTool()
        {
            AssetDatabase.Refresh();
            bool downloadSuccess = false;

            var client = new UnityWebRequest(downloadURL + newestVersion + ".unitypackage")
            {
                downloadHandler = new DownloadHandlerFile(packagePath)
            };
            client.SendWebRequest().completed += AA =>
            {
                if (client.downloadHandler.isDone)
                {
                    downloadSuccess = true;
                    GetAssetsToDelete();


                    AssetDatabase.ImportAsset(packagePath);
                    AssetDatabase.ImportPackage(packagePath, false);
                }

                client.Dispose();

                if (downloadSuccess) DeleteAssets();
                else Debug.LogError("Animation Repathing: Download could not be completed, you may have to manually update the tool through the VRChat Creator Companion or by downloading the latest release from https://github.com/hfcRed/Animation-Repathing/releases");
            };
        }

        public static void GetAssetsToDelete()
        {
            assetsToDelete.Clear();
            metasToDelete.Clear();

            var editor = ScriptableObject.CreateInstance<AREditor>();
            string path = editor.GetScriptPath();

            string folderPath = Path.GetDirectoryName(path);
            string parentFolder = Path.GetDirectoryName(folderPath);
            string[] assetGUID = AssetDatabase.FindAssets("", new[] { parentFolder }).Where(x => x != AssetDatabase.AssetPathToGUID(folderPath)).ToArray();

            foreach (string s in assetGUID)
            {
                string ext = Path.GetExtension(AssetDatabase.GUIDToAssetPath(s));
                string oldPath = AssetDatabase.GUIDToAssetPath(s);

                File.Move(oldPath, folderPath + "/ARToDelete" + s + ext);

                assetsToDelete.Add(folderPath + "/ARToDelete" + s + ext);
                metasToDelete.Add(oldPath + ".meta");
            }
        }

        public static void DeleteAssets()
        {
            AssetDatabase.DeleteAsset(packagePath);

            foreach (string s in metasToDelete)
            {
                if (File.Exists(s))
                {
                    File.Delete(s);
                }
            }

            foreach (string s in assetsToDelete)
            {
                if (File.Exists(s))
                {
                    AssetDatabase.DeleteAsset(s);
                }
            }
        }
    }
}