using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace HyperVersion.Core
{
    public abstract class VersionInitialize
    {
        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeCanvasVersion()
        {
            var bootstrapGo = new GameObject("HyperVersionBootstrap");
            Object.DontDestroyOnLoad(bootstrapGo);
            bootstrapGo.hideFlags = HideFlags.HideAndDontSave;
            bootstrapGo.AddComponent<HyperVersionBootstrap>();
        }

        private class HyperVersionBootstrap : MonoBehaviour
        {
            private IEnumerator Start()
            {
                TMP_Settings.LoadDefaultSettings();

                var settings = Resources.Load<HyperVersionSettings>("HyperVersionSettings");
                if (settings == null)
                    settings = ScriptableObject.CreateInstance<HyperVersionSettings>();

                string versionFilePath = Path.Combine(Application.streamingAssetsPath, "version.json");

                using var request = UnityWebRequest.Get(versionFilePath);
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[HyperVersion] Falha ao carregar version.json: {request.error}");
                    yield break;
                }

                var data = JsonUtility.FromJson<VersionData>(request.downloadHandler.text);
                if (data == null)
                {
                    Debug.LogWarning("[HyperVersion] version.json inválido.");
                    yield break;
                }

                Debug.Log($"[HyperVersion] Game version: {data.release}.{data.build}");

                string versionString = $"v{data.release}";
                if (settings.showBuild)
                    versionString += $".{data.build}";
                if (settings.showEnvTag && !string.IsNullOrEmpty(data.environment) && data.environment != "release")
                    versionString += $"-{data.environment}";
                if (settings.showDate)
                    versionString += $"/{data.date}";

                var canvasGo = new GameObject("CanvasVersion");
                Object.DontDestroyOnLoad(canvasGo);

                var canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 99;

                var scaler = canvasGo.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                if (data.environment == "release")
                    yield break;

                var textGo = new GameObject("VersionText", typeof(TextMeshProUGUI));
                textGo.transform.SetParent(canvasGo.transform, false);

                var text = textGo.GetComponent<TextMeshProUGUI>();
                text.text = versionString;
                text.fontSize = 15;
                text.alignment = TextAlignmentOptions.BottomRight;

                var rt = text.rectTransform;
                rt.anchorMin = new Vector2(1, 0);
                rt.anchorMax = new Vector2(1, 0);
                rt.pivot = new Vector2(1, 0);
                rt.anchoredPosition = new Vector2(-10, 14);
                rt.sizeDelta = new Vector2(700, 30);

                var showVersion = textGo.AddComponent<ShowVersion>();
                showVersion.Initialize(text);

                text.enabled = data.show_version_game;
            }
        }
    }
}