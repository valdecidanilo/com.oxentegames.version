using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CustomVersion.Core
{

    public class VersionJsonManager : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        static VersionJsonManager()
        {
            UpdateReleaseInJson();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateReleaseInJson();
            UpdateBuildAndDate();
            ShowEnvironmentPopup();
        }

        private static void UpdateReleaseInJson()
        {
            var streamingAssetsDir = Application.streamingAssetsPath;
            var versionJsonPath = Path.Combine(streamingAssetsDir, "version.json");

            if (!Directory.Exists(streamingAssetsDir))
                Directory.CreateDirectory(streamingAssetsDir);

            VersionData data;

            if (!File.Exists(versionJsonPath))
            {
                data = new VersionData
                {
                    release = PlayerSettings.bundleVersion,
                    build = "0",
                    data = "0",
                    environment = "dev"
                };
            }
            else
            {
                try
                {
                    var existingJson = File.ReadAllText(versionJsonPath);
                    data = JsonUtility.FromJson<VersionData>(existingJson);
                    if (data == null)
                    {
                        data = new VersionData
                        {
                            release = PlayerSettings.bundleVersion,
                            build = "0",
                            data = "0",
                            environment = "dev"
                        };
                    }
                    else
                    {
                        data.release = PlayerSettings.bundleVersion;
                        if (string.IsNullOrEmpty(data.environment))
                            data.environment = "dev";
                    }
                }
                catch
                {
                    data = new VersionData
                    {
                        release = PlayerSettings.bundleVersion,
                        build = "0",
                        data = "0",
                        environment = "dev"
                    };
                }
            }

            var jsonToWrite = JsonUtility.ToJson(data, true);
            File.WriteAllText(versionJsonPath, jsonToWrite);
            AssetDatabase.Refresh();
        }

        private static void UpdateBuildAndDate()
        {
            var versionJsonPath = Path.Combine(Application.streamingAssetsPath, "version.json");
            VersionData data;

            try
            {
                var existingJson = File.ReadAllText(versionJsonPath);
                data = JsonUtility.FromJson<VersionData>(existingJson) ?? new VersionData();
            }
            catch
            {
                data = new VersionData();
            }

            if (!int.TryParse(data.build, out var buildCount))
                buildCount = 0;
            buildCount++;
            data.build = buildCount.ToString();

            data.data = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            data.release = PlayerSettings.bundleVersion;
            if (string.IsNullOrEmpty(data.environment))
                data.environment = "dev";

            var jsonToWrite = JsonUtility.ToJson(data, true);
            File.WriteAllText(versionJsonPath, jsonToWrite);
            AssetDatabase.Refresh();
        }

        private static void ShowEnvironmentPopup()
        {
            var versionJsonPath = Path.Combine(Application.streamingAssetsPath, "version.json");
            VersionData data;

            try
            {
                var existingJson = File.ReadAllText(versionJsonPath);
                data = JsonUtility.FromJson<VersionData>(existingJson) ?? new VersionData();
            }
            catch
            {
                data = new VersionData
                {
                    release = PlayerSettings.bundleVersion,
                    build = "0",
                    data = "0",
                    environment = "dev"
                };
            }

            int choice = EditorUtility.DisplayDialogComplex(
                "Selecionar Ambiente",
                "Escolha o ambiente para esta build:",
                "Dev",
                "Release",
                "Stg"
            );

            data.environment = choice switch
            {
                0 => "dev",
                1 => "release",
                2 => "stg",
                _ => data.environment
            };

            var jsonToWrite = JsonUtility.ToJson(data, true);
            File.WriteAllText(versionJsonPath, jsonToWrite);
            AssetDatabase.Refresh();
        }
    }
}
