using System.IO;
using HyperVersion.Core;
using UnityEditor;
using UnityEngine;

namespace HyperVersion.Editor
{
    [InitializeOnLoad]
    public static class HyperVersionAssetInitializer
    {
        private const string ResourcesFolderPath = "Assets/Resources";
        private const string StreamingAssetsFolderPath = "Assets/StreamingAssets";
        private const string PluginsFolderPath = "Assets/Plugins";
        private const string WebGlPluginsFolderPath = "Assets/Plugins/WebGL";

        private const string SettingsAssetPath = "Assets/Resources/HyperVersionSettings.asset";
        private const string JslibAssetPath = "Assets/Plugins/WebGL/HyperVersionWebGL.jslib";

        private static readonly string VersionFilePath =
            Path.Combine(Application.dataPath, "StreamingAssets", "version.json");

        private static readonly string JslibFilePath =
            Path.Combine(Application.dataPath, "Plugins", "WebGL", "HyperVersionWebGL.jslib");

        static HyperVersionProjectInitializer()
        {
            EditorApplication.delayCall += EnsureProjectAssets;
        }

        public static void InitializeProject()
        {
            EnsureProjectAssets();
        }

        private static void EnsureProjectAssets()
        {
            EnsureFolder("Assets", "Resources", ResourcesFolderPath);
            EnsureFolder("Assets", "StreamingAssets", StreamingAssetsFolderPath);
            EnsureFolder("Assets", "Plugins", PluginsFolderPath);
            EnsureFolder("Assets/Plugins", "WebGL", WebGlPluginsFolderPath);

            CreateSettingsAssetIfNeeded();
            CreateVersionFileIfNeeded();
            CreateJslibIfNeeded();
        }

        private static void EnsureFolder(string parent, string name, string fullPath)
        {
            if (!AssetDatabase.IsValidFolder(fullPath))
                AssetDatabase.CreateFolder(parent, name);
        }

        private static void CreateSettingsAssetIfNeeded()
        {
            if (File.Exists(SettingsAssetPath))
                return;

            var settings = ScriptableObject.CreateInstance<HyperVersionSettings>();
            AssetDatabase.CreateAsset(settings, SettingsAssetPath);
            AssetDatabase.SaveAssets();
        }

        private static void CreateVersionFileIfNeeded()
        {
            if (File.Exists(VersionFilePath))
                return;

            var initial = new VersionData
            {
                release = PlayerSettings.bundleVersion,
                build = "0",
                date = "0",
                environment = "dev",
                show_version_web = true
            };

            File.WriteAllText(VersionFilePath, JsonUtility.ToJson(initial, true));
            AssetDatabase.Refresh();
        }

        private static void CreateJslibIfNeeded()
        {
            if (File.Exists(JslibFilePath))
                return;

            const string jslibContent =
@"mergeInto(LibraryManager.library, {
  HyperVersionShow: function () {
    if (window.HyperVersion_Show) window.HyperVersion_Show();
  },

  HyperVersionHide: function () {
    if (window.HyperVersion_Hide) window.HyperVersion_Hide();
  }
});";

            File.WriteAllText(JslibFilePath, jslibContent);
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(JslibAssetPath, ImportAssetOptions.ForceSynchronousImport);
        }
    }
}