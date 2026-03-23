using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using HyperVersion.Core;

namespace HyperVersion.Editor
{
    public class VersionJsonManager : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        private const string StreamingAssetsFolder = "Assets/StreamingAssets";
        private const string VersionAssetPath = "Assets/StreamingAssets/version.json";

        private static readonly string VersionFilePath =
            Path.Combine(Application.dataPath, "StreamingAssets", "version.json");

        public void OnPreprocessBuild(BuildReport report)
        {
            CreateOrUpdateVersionJson(initial: false);
        }

        private static void CreateOrUpdateVersionJson(bool initial)
        {
            if (!AssetDatabase.IsValidFolder(StreamingAssetsFolder))
                AssetDatabase.CreateFolder("Assets", "StreamingAssets");

            VersionData data;

            if (initial || !File.Exists(VersionFilePath))
            {
                data = new VersionData
                {
                    release = PlayerSettings.bundleVersion,
                    build = "0",
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    environment = "dev",
                    show_version_web = true
                };
            }
            else
            {
                try
                {
                    var txt = File.ReadAllText(VersionFilePath);
                    data = JsonUtility.FromJson<VersionData>(txt) ?? new VersionData();
                }
                catch
                {
                    data = new VersionData();
                }

                data.release = PlayerSettings.bundleVersion;

                if (!int.TryParse(data.build, out var b))
                    b = 0;

                data.build = (++b).ToString();
                data.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (!initial)
            {
                BuildTagSelectorWindow.Show();

                if (!BuildTagSelectorWindow.Confirmed)
                    throw new BuildFailedException("[HyperVersion] Build cancelada pelo usuário.");

                data.environment = BuildTagSelectorWindow.SelectedTag;
            }

            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(VersionFilePath, json);

            Debug.Log($"[HyperVersion] version.json atualizado em StreamingAssets:\\n{json}");

            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(VersionAssetPath, ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();
        }

        public static void ResetVersionFile()
        {
            CreateOrUpdateVersionJson(initial: true);
        }
    }
}