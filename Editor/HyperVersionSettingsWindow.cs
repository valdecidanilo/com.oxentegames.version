using UnityEditor;
using UnityEngine;
using HyperVersion.Core;
using System.IO;

namespace HyperVersion.Editor
{
    public class HyperVersionSettingsWindow : EditorWindow
    {
        private HyperVersionSettings settings;
        private string   preview;
        private GUIStyle previewStyle;

        [MenuItem("Tools/HyperVersion/Settings")]
        private static void Open()
        {
            var window = GetWindow<HyperVersionSettingsWindow>("HyperVersion Settings");
            window.minSize = new Vector2(320f, 80f);
        }

        private void OnEnable()
        {
            settings = Resources.Load<HyperVersionSettings>("HyperVersionSettings");

            // Try to create and reload if not found
            if (settings == null)
            {
                Debug.Log("[HyperVersion] HyperVersionSettings not found. Initializing Resources...");
                ResourcesVersionCreator.InitializeResourcesVersion();
                settings = Resources.Load<HyperVersionSettings>("HyperVersionSettings");
                if (settings != null)
                    Debug.Log("[HyperVersion] HyperVersionSettings loaded after initialization.");
                else
                    Debug.LogWarning("[HyperVersion] Failed to load HyperVersionSettings after initialization attempt.");
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

            // Guard clause: settings missing
            if (settings == null)
            {
                EditorGUILayout.HelpBox(
                    "HyperVersionSettings not found. Click 'Inicializar Resources' to create the required assets.",
                    MessageType.Warning
                );

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Inicializar Resources", GUILayout.Width(200)))
                    {
                        Debug.Log("[HyperVersion] Initialize Resources requested from settings window.");
                        ResourcesVersionCreator.InitializeResourcesVersion();
                        settings = Resources.Load<HyperVersionSettings>("HyperVersionSettings");
                        if (settings != null)
                        {
                            Debug.Log("[HyperVersion] Resources initialized and HyperVersionSettings loaded.");
                            UpdatePreview();
                            Repaint();
                        }
                        else
                        {
                            Debug.LogWarning("[HyperVersion] Initialization finished, but HyperVersionSettings is still unavailable.");
                        }
                    }
                    GUILayout.FlexibleSpace();
                }

                return; // stop drawing to avoid null usage below
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Resetar version.json"))
                {
                    if (EditorUtility.DisplayDialog("Resetar version.json?",
                        "Tem certeza que deseja resetar o version.json para build 0 e ambiente dev?",
                        "Sim", "Cancelar"))
                    {
                        Debug.Log("[HyperVersion] version.json reset requested by user.");
                        VersionJsonManager.ResetVersionFile();
                    }
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Inicializar Resources"))
                {
                    Debug.Log("[HyperVersion] Initialize Resources requested by user.");
                    ResourcesVersionCreator.InitializeResourcesVersion();
                    var before = settings;
                    settings = Resources.Load<HyperVersionSettings>("HyperVersionSettings");
                    if (settings != null && before == null)
                    {
                        Debug.Log("[HyperVersion] HyperVersionSettings created and loaded.");
                        UpdatePreview();
                    }
                    else
                    {
                        Debug.Log("[HyperVersion] Resources verified. No changes needed.");
                    }
                }
            }
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Versao Atual (version.json)", EditorStyles.boldLabel);

            var versionAsset = Resources.Load<TextAsset>("version");
            if (versionAsset != null)
            {
                var versionData = JsonUtility.FromJson<VersionData>(versionAsset.text);

                EditorGUI.BeginChangeCheck();
                versionData.release     = EditorGUILayout.TextField("Release",     versionData.release);
                versionData.build       = EditorGUILayout.TextField("Build",       versionData.build);
                versionData.data        = EditorGUILayout.TextField("Data",        versionData.data);
                versionData.environment = EditorGUILayout.TextField("Ambiente",    versionData.environment);

                if (EditorGUI.EndChangeCheck())
                {
                    string newJson = JsonUtility.ToJson(versionData, true);
                    string path = "Assets/Resources/version.json";
                    File.WriteAllText(path, newJson);
                    AssetDatabase.Refresh();
                    Debug.Log("[HyperVersion] version.json atualizado manualmente.");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("version.json nao encontrado em Resources.", MessageType.Warning);
            }
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Opcoes de exibicao", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            settings.showBuild  = EditorGUILayout.ToggleLeft("Mostrar No Build", settings.showBuild);
            settings.showEnvTag = EditorGUILayout.ToggleLeft("Mostrar Ambiente", settings.showEnvTag);
            settings.showDate   = EditorGUILayout.ToggleLeft("Mostrar Data",     settings.showDate);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                UpdatePreview();
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Preview (runtime)", previewStyle, GUILayout.Height(30));
            EditorGUILayout.HelpBox(preview, MessageType.Info);
        }

        private void UpdatePreview()
        {
            if (settings == null)
            {
                preview = "HyperVersionSettings indisponivel - inicialize os Resources.";
                if (previewStyle != null)
                {
                    previewStyle.normal.textColor = Color.gray;
                }
                return;
            }

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
