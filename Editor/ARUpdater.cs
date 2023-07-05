using static AutoAnimationRepath.ARVariables;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AutoAnimationRepath
{
    public class ARUpdater
    {
        public static string versionURL = "https://raw.githubusercontent.com/hfcRed/Animation-Repathing/main/version.txt";
        public static string downloadURL = "https://github.com/hfcRed/Animation-Repathing/releases/latest/download/Animation_Repathing.unitypackage";
        public static string outputPath = "Assets/AROutput.unitypackage";
        public static List<string> assetsToDelete = new List<string>();

        public static bool CheckForNewVersion()
        {
            newestVersion = GetNewestVersion()?.ToString();
            currentVersion = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath("687ec5539659fd440992163de55107d7"))?.text;

            if (newestVersion != null && newestVersion != currentVersion) return true;
            else return false;
        }

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

        public static void UpdateTool()
        {
            var client = new UnityWebRequest(downloadURL)
            {
                downloadHandler = new DownloadHandlerFile(outputPath)
            };
            client.SendWebRequest().completed += AA =>
            {
                GetAssetsToDelete();
                AssetDatabase.ImportAsset(outputPath);
                AssetDatabase.ImportPackage(outputPath, false);
                DeleteAssets();
                client.Dispose();
            };
        }

        public static void GetAssetsToDelete()
        {
            var editor = ScriptableObject.CreateInstance<AREditor>();
            string path = editor.GetScriptPath();

            assetsToDelete.Clear();
            string folderPath = Path.GetDirectoryName(path);
            string[] assetGUID = AssetDatabase.FindAssets("", new[] { folderPath });

            foreach (string s in assetGUID)
            {
                string ext = Path.GetExtension(AssetDatabase.GUIDToAssetPath(s));
                string oldPath = AssetDatabase.GUIDToAssetPath(s);
                File.Move(oldPath, folderPath + "/ARToDelete" + s + ext);
                assetsToDelete.Add(folderPath + "/ARToDelete" + s + ext);
            }
        }

        public static void DeleteAssets()
        {
            AssetDatabase.DeleteAsset(outputPath);

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