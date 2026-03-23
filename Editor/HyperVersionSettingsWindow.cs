using System.IO;
using HyperVersion.Core;
using UnityEditor;
using UnityEngine;

namespace HyperVersion.Editor
{
    public class HyperVersionSettingsWindow : EditorWindow
    {
        private HyperVersionSettings _settings;
        private const string SettingsPath = "Assets/Resources/HyperVersionSettings.asset";
        private const string VersionJsonPath = "Assets/StreamingAssets/version.json";

        [MenuItem("Tools/HyperVersion/Settings")]
        public static void ShowWindow()
        {
            GetWindow<HyperVersionSettingsWindow>("HyperVersion Settings");
        }

        private void OnEnable()
        {
            _settings = AssetDatabase.LoadAssetAtPath<HyperVersionSettings>(SettingsPath);

            if (_settings == null)
            {
                HyperVersionProjectInitializer.InitializeProject();
                _settings = AssetDatabase.LoadAssetAtPath<HyperVersionSettings>(SettingsPath);
            }
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                EditorGUILayout.HelpBox("HyperVersionSettings não encontrado.", MessageType.Warning);
                if (GUILayout.Button("Inicializar Projeto"))
                {
                    HyperVersionProjectInitializer.InitializeProject();
                    _settings = AssetDatabase.LoadAssetAtPath<HyperVersionSettings>(SettingsPath);
                }
                return;
            }

            EditorGUILayout.LabelField("Configurações Visuais", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            _settings.showBuild = EditorGUILayout.ToggleLeft("Mostrar Build", _settings.showBuild);
            _settings.showEnvTag = EditorGUILayout.ToggleLeft("Mostrar Ambiente", _settings.showEnvTag);
            _settings.showDate = EditorGUILayout.ToggleLeft("Mostrar Data", _settings.showDate);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("version.json", EditorStyles.boldLabel);

            if (File.Exists(VersionJsonPath))
            {
                var json = File.ReadAllText(VersionJsonPath);
                var versionData = JsonUtility.FromJson<VersionData>(json) ?? new VersionData();

                EditorGUI.BeginChangeCheck();
                versionData.release = EditorGUILayout.TextField("Release", versionData.release);
                versionData.build = EditorGUILayout.TextField("Build", versionData.build);
                versionData.date = EditorGUILayout.TextField("Date", versionData.date);
                versionData.environment = EditorGUILayout.TextField("Environment", versionData.environment);
                versionData.show_version_web = EditorGUILayout.ToggleLeft("Mostrar na Web", versionData.show_version_web);

                if (EditorGUI.EndChangeCheck())
                {
                    File.WriteAllText(VersionJsonPath, JsonUtility.ToJson(versionData, true));
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("version.json não encontrado em StreamingAssets.", MessageType.Warning);

                if (GUILayout.Button("Criar version.json"))
                {
                    HyperVersionProjectInitializer.InitializeProject();
                }
            }

            EditorGUILayout.Space(12);

            if (GUILayout.Button("Resetar version.json"))
            {
                VersionJsonManager.ResetVersionFile();
            }

            if (GUILayout.Button("Inicializar Projeto"))
            {
                HyperVersionProjectInitializer.InitializeProject();
            }
        }
    }
}