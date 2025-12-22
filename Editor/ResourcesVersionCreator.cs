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
        private const string SettingsAssetPath = "Assets/Resources/HyperVersionSettings.asset";
        private const string VersionFileName    = "version.json";
        private static readonly string VersionFilePath =
            Path.Combine(Application.dataPath, "Resources", VersionFileName);

        static ResourcesVersionCreator()
        {
            EditorApplication.delayCall += EnsureResourcesVersionJson;
        }

        public static void InitializeResourcesVersion()
        {
            EnsureResourcesVersionJson();
        }

        private static void EnsureResourcesVersionJson()
        {
            if (!AssetDatabase.IsValidFolder(ResourcesFolderPath))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                Debug.Log($"[HyperVersion] Criada pasta Resources em: {ResourcesFolderPath}");
            }
            if (!File.Exists(SettingsAssetPath))
            {
                var settings = ScriptableObject.CreateInstance<HyperVersionSettings>();
                AssetDatabase.CreateAsset(settings, SettingsAssetPath);
                AssetDatabase.SaveAssets();
                Debug.Log("[HyperVersion] Criado HyperVersionSettings.asset em Resources.");
            }
            if (!File.Exists(VersionFilePath))
            {
                var initial = new VersionData
                {
                    release     = PlayerSettings.bundleVersion,
                    build       = "0",
                    data        = "0",
                    environment = "dev"
                };
            }
        }
    }
}
