using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace HyperVersion.Core
{
    public abstract class VersionInitialize
    {
        [Preserve]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeCanvasVersion()
        {
            TMP_Settings.LoadDefaultSettings();

            var jsonFile = Resources.Load<TextAsset>("version");
            if (jsonFile == null) return;
            var data = JsonUtility.FromJson<VersionData>(jsonFile.text);

            Debug.Log($"[HyperVersion] Game version: {data.release}.{data.build}");

            var settings = Resources.Load<HyperVersionSettings>("HyperVersionSettings");
            if (settings == null) settings = ScriptableObject.CreateInstance<HyperVersionSettings>();

            string versionString = $"v{data.release}";
            if (settings.showBuild)  versionString += $".{data.build}";
            if (settings.showEnvTag && !string.IsNullOrEmpty(data.environment) && data.environment != "release")
                versionString += $"-{data.environment}";
            if (settings.showDate)   versionString += $"/{data.data}";

            var canvasGo = new GameObject("CanvasVersion");
            var canvas   = canvasGo.AddComponent<Canvas>();
            canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 99;
            
            var canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.matchWidthOrHeight = 0.5f;
            if (data.environment != "release" && data.environment != "stg")
            {
                var textGo = new GameObject("VersionText", typeof(TextMeshProUGUI));
                textGo.transform.SetParent(canvasGo.transform, false);
                var text = textGo.GetComponent<TextMeshProUGUI>();
                text.text      = versionString;
                text.fontSize  = 15;
                text.alignment = TextAlignmentOptions.BottomRight;
                var rt = text.rectTransform;
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1, 0);
                rt.anchoredPosition = new Vector2(-10, 14);
                rt.sizeDelta        = new Vector2(500, 20);
            }
            
            Object.DontDestroyOnLoad(canvasGo);
        }
    }
}
