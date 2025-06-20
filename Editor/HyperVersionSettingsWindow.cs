using UnityEditor;
using UnityEngine;
using HyperVersion.Core;

namespace HyperVersion.Editor
{
    public class HyperVersionSettingsWindow : EditorWindow
    {
        private HyperVersionSettings settings;
        private string   preview;
        private GUIStyle previewStyle;

        [MenuItem("Tools/HyperVersion/Settings")]
        private static void Open() =>
            GetWindow<HyperVersionSettingsWindow>("HyperVersion Settings");

        // ──────────────────────────────────────────────────────────────────────────
        private void OnEnable()
        {
            settings = Resources.Load<HyperVersionSettings>("HyperVersionSettings");
            if (settings == null)
            {
                settings = CreateInstance<HyperVersionSettings>();
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.CreateAsset(settings, "Assets/Resources/HyperVersionSettings.asset");
                AssetDatabase.SaveAssets();
            }

            previewStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize   = 15,
                alignment  = TextAnchor.MiddleCenter,
                normal     = { textColor = Color.cyan }
            };

            UpdatePreview();
        }

        private void OnGUI()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, 60),
                               new Color(0.2f, 0.4f, 0.7f));

            var titleStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize  = 20,
                alignment = TextAnchor.MiddleLeft,
                padding   = new RectOffset(10, 0, 10, 0),
                normal    = { textColor = Color.white }
            };

            GUILayout.Space(10);
            GUILayout.Label("HyperVersion Settings", titleStyle);
            GUILayout.Space(20);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Resetar version.json"))
                {
                    if (EditorUtility.DisplayDialog("Resetar version.json?",
                        "Tem certeza que deseja resetar o version.json para build 0 e ambiente dev?",
                        "Sim", "Cancelar"))
                    {
                        VersionJsonManager.ResetVersionFile();
                    }
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Inicializar Resources"))
                {
                    ResourcesVersionCreator.InitializeResourcesVersion();
                }
            }

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Opções de exibição", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            settings.showBuild  = EditorGUILayout.ToggleLeft("Mostrar Nº Build", settings.showBuild);
            settings.showEnvTag = EditorGUILayout.ToggleLeft("Mostrar Ambiente", settings.showEnvTag);
            settings.showDate   = EditorGUILayout.ToggleLeft("Mostrar Data",     settings.showDate);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                UpdatePreview();
            }

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Preview (runtime)", previewStyle, GUILayout.Height(30));
            EditorGUILayout.HelpBox(preview, MessageType.Info);
        }
        private void UpdatePreview()
        {
            string release = "v0.0.0";
            string build   = ".42";
            string envTag  = "-dev";
            string date    = "/2025-06-20 09:44:04";

            preview = release;
            if (settings.showBuild)  preview += build;
            if (settings.showEnvTag) preview += envTag;
            if (settings.showDate)   preview += date;

            previewStyle.normal.textColor =
                (settings.showBuild || settings.showEnvTag || settings.showDate)
                    ? Color.cyan
                    : Color.gray;
        }
    }
}