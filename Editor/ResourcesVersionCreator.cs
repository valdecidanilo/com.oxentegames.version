using System.IO;
using HyperVersion.Core;
using UnityEditor;
using UnityEngine;

namespace HyperVersion.Editor
{
    [InitializeOnLoad]
    public static class ResourcesVersionCreator
    {
        private const string ResourcesFolderPath = "Assets/Resources";
        private const string StreamingAssetsFolderPath = "Assets/StreamingAssets";
        private const string SettingsAssetPath = "Assets/Resources/HyperVersionSettings.asset";

        private static readonly string VersionFilePath =
            Path.Combine(Application.dataPath, "StreamingAssets", "version.json");

        static ResourcesVersionCreator()
        {
            EditorApplication.delayCall += EnsureVersionAssets;
        }

        public static void InitializeResourcesVersion()
        {
            EnsureVersionAssets();
        }

        private static void EnsureVersionAssets()
        {
            if (!AssetDatabase.IsValidFolder(ResourcesFolderPath))
                AssetDatabase.CreateFolder("Assets", "Resources");

            if (!AssetDatabase.IsValidFolder(StreamingAssetsFolderPath))
                AssetDatabase.CreateFolder("Assets", "StreamingAssets");

            if (!File.Exists(SettingsAssetPath))
            {
                var settings = ScriptableObject.CreateInstance<HyperVersionSettings>();
                AssetDatabase.CreateAsset(settings, SettingsAssetPath);
                AssetDatabase.SaveAssets();
            }

            if (!File.Exists(VersionFilePath))
            {
                var initial = new VersionData
                {
                    release = PlayerSettings.bundleVersion,
                    build = "0",
                    date = "0",
                    environment = "dev",
                    show_version_web = true,
                    show_version_game = false
                };

                File.WriteAllText(VersionFilePath, JsonUtility.ToJson(initial, true));
                AssetDatabase.Refresh();
            }
        }
    }
}