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

        private const string ResourcesFolder = "Assets/Resources";
        private static readonly string VersionFilePath =
            Path.Combine(Application.dataPath, "Resources", "version.json");

        public void OnPreprocessBuild(BuildReport report)
        {
            CreateOrUpdateVersionJson(initial: false);
        }

        private static void CreateOrUpdateVersionJson(bool initial)
        {
            if (!AssetDatabase.IsValidFolder(ResourcesFolder))
                AssetDatabase.CreateFolder("Assets", "Resources");

            VersionData data;

            if (initial || !File.Exists(VersionFilePath))
            {
                data = new VersionData
                {
                    release     = PlayerSettings.bundleVersion,
                    build       = "0",
                    data        = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    environment = "development"
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

                if (!int.TryParse(data.build, out var b)) b = 0;
                data.build = (++b).ToString();
                data.data  = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (!initial)
            {
                BuildTagSelectorWindow.Show();

                if (!BuildTagSelectorWindow.Confirmed)
                    throw new BuildFailedException("[HyperVersion] Build cancelada pelo usuário.");

                data.environment = BuildTagSelectorWindow.SelectedTag;
            }

            var json = JsonUtility.ToJson(data, true);
            try
            {
                File.WriteAllText(VersionFilePath, json);
                Debug.Log($"[HyperVersion] version.json {(initial ? "criado" : "atualizado")} em Resources:\n{json}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[HyperVersion] falha ao escrever version.json: {ex.Message}");
            }

            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset("Assets/Resources/version.json", ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();
        }

        public static void ResetVersionFile()
        {
            CreateOrUpdateVersionJson(initial: true);
        }
    }
}