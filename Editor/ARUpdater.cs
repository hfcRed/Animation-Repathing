using static AnimationRepathing.ARVariables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
#if VRC_RESOLVER
using VRC.PackageManagement.Core;
using VRC.PackageManagement.Core.Types;
using VRC.PackageManagement.Resolver;
#endif

namespace AnimationRepathing
{
    public class ARUpdater
    {
        public static string versionURL = "https://raw.githubusercontent.com/hfcRed/Animation-Repathing/main/package.json";
        public static string downloadURL = "https://github.com/hfcRed/Animation-Repathing/releases/latest/download/AnimationRepathing_v";
        public static string packagePath = "Assets/AROutput.unitypackage";

        public static List<string> assetsToDelete = new List<string>();
        public static List<string> metasToDelete = new List<string>();

        public static void CheckForNewVersion()
        {
            newestVersion = string.Empty;
            currentVersion = string.Empty;

            currentVersion = GetCurrentVersion()?.ToString();

            GetNewestVersion(jsonText =>
            {
                if (jsonText != null && jsonText != string.Empty)
                {
                    JsonData data = JsonUtility.FromJson<JsonData>(jsonText);
                    newestVersion = data.version;
                }
                else newestVersion = null;

                if (newestVersion != null && currentVersion != null && newestVersion != currentVersion) availableUpdate = true;
                else availableUpdate = false;
            });
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
            ScriptableObject.DestroyImmediate(editor);

            string folderPath = Path.GetDirectoryName(path);
            string parentFolder = Path.GetDirectoryName(folderPath);

            if (!File.Exists(parentFolder + "/package.json")) return null;

            string jsonText = File.ReadAllText(parentFolder + "/package.json");

            if (jsonText != null && jsonText != string.Empty)
            {
                JsonData data = JsonUtility.FromJson<JsonData>(jsonText);
                return data.version;
            }

            return null;
        }

        public static void GetNewestVersion(Action<string> callback)
        {
            fetchingVersion = true;
            fetchingFailed = false;
            string jsonText = string.Empty;

            var client = new UnityWebRequest(versionURL)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            client.SendWebRequest().completed += AA =>
            {
                if (client.isNetworkError || client.isHttpError)
                {
                    Debug.LogError("Could not get newest version for Animation Repathing! => " + client.error);
                    fetchingVersion = false;
                    fetchingFailed = true;
                    newestVersion = null;
                    callback(string.Empty);
                }
                else if (client.downloadHandler.isDone)
                {
                    jsonText = client.downloadHandler.text;
                    fetchingVersion = false;
                    callback(jsonText);
                }
            };
        }

        public static void UpdateTool()
        {
            if (newestVersion == null || currentVersion == null || newestVersion == currentVersion) return;

            AssetDatabase.Refresh();

#if VRC_RESOLVER
            if (Resolver.VPMManifestExists())
            {
                try
                {
                    var project = new UnityProject(Resolver.ProjectDir);
                    var package = Repos.GetPackageWithVersionMatch("com.hfcred.animationrepathing", newestVersion);

                    if (package != null)
                    {
                        project.UpdateVPMPackage(package);
                        AssetDatabase.Refresh();
                        return;
                    }
                }
                catch (Exception e) { Debug.Log(e); };
            }
#endif

            bool downloadSuccess = false;

            var client = new UnityWebRequest(downloadURL + newestVersion + ".unitypackage")
            {
                downloadHandler = new DownloadHandlerFile(packagePath)
            };

            client.SendWebRequest().completed += AA =>
            {
                if (client.isNetworkError || client.isHttpError)
                {
                    Debug.Log(client.error);
                }
                else if (client.downloadHandler.isDone)
                {
                    downloadSuccess = true;
                    GetAssetsToDelete();

                    AssetDatabase.ImportAsset(packagePath);
                    AssetDatabase.ImportPackage(packagePath, false);
                }

                client.Dispose();
                AssetDatabase.DeleteAsset(packagePath);

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
            ScriptableObject.DestroyImmediate(editor);

            string folderPath = Path.GetDirectoryName(path);
            string parentFolder = Path.GetDirectoryName(folderPath);
            string[] assetGUID = AssetDatabase.FindAssets("", new[] { parentFolder }).ToArray();
            string[] foldersToExclude = Directory.GetDirectories(parentFolder).Select(x => AssetDatabase.AssetPathToGUID(x)).ToArray();
            assetGUID = assetGUID.Except(foldersToExclude).ToArray();

            foreach (string s in assetGUID)
            {
                string ext = Path.GetExtension(AssetDatabase.GUIDToAssetPath(s));
                string oldPath = AssetDatabase.GUIDToAssetPath(s);
                AssetDatabase.MoveAsset(oldPath, folderPath + "/ARToDelete" + s + ext);

                assetsToDelete.Add(folderPath + "/ARToDelete" + s + ext);
                metasToDelete.Add(oldPath + ".meta");
            }
        }

        public static void DeleteAssets()
        {
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