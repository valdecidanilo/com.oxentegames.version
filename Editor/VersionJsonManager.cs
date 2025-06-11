using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using CustomVersion.Core;

namespace CustomVersion.Editor
{
    public class VersionJsonManager : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

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
                    data        = DateTime.Now.ToString("yyyy-MM-dd"),
                    environment = "dev"
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
                var choice = EditorUtility.DisplayDialogComplex(
                    "Selecionar Ambiente",
                    "Escolha o ambiente para esta build:",
                    "Dev", "Release", "Stg"
                );

                data.environment = choice switch
                {
                    0 => "dev",
                    1 => "release",
                    2 => "stg",
                    _ => data.environment
                };
            }

            var json = JsonUtility.ToJson(data, true);
            try
            {
                File.WriteAllText(VersionFilePath, json);
                Debug.Log($"[VersionJsonManager] version.json {(initial ? "criado" : "atualizado")} em Resources:\n{json}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[VersionJsonManager] falha ao escrever version.json: {ex.Message}");
            }

            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset("Assets/Resources/version.json", ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.SaveAssets();
        }
        [MenuItem("Tools/Custom Version/Reset version.json")]
        public static void ResetVersionFile()
        {
            if (EditorUtility.DisplayDialog("Resetar version.json?", "Tem certeza que deseja resetar o version.json para build 0 e ambiente dev?", "Sim", "Cancelar"))
            {
                CreateOrUpdateVersionJson(initial: true);
            }
        }
    }
}
