using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Scripting;

namespace CustomVersion.Core
{
    public abstract class VersionInitialize
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeCanvasVersion()
        {
            TMP_Settings.LoadDefaultSettings();

            var jsonFile = Resources.Load<TextAsset>("version");
            if (jsonFile == null)
            {
                return;
            }

            var versionData = JsonUtility.FromJson<VersionData>(jsonFile.text);

            var canvasGo = new GameObject("CanvasVersion");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 99;
            canvas.vertexColorAlwaysGammaSpace = true;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<ShowVersion>();

            var textGo = new GameObject("VersionText");
            textGo.transform.SetParent(canvasGo.transform, false);
            var text = textGo.AddComponent<TextMeshProUGUI>();
            if (versionData.environment != "release") text.text = $"v{versionData.release}.{versionData.build}-{versionData.environment}";
            else if (versionData.environment == string.Empty) text.text = $"v{versionData.release}.{versionData.build}";
            text.fontSize = 15;
            text.alignment = TextAlignmentOptions.BottomRight;

            var rect = text.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 0);
            rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(1, 0);
            rect.anchoredPosition = new Vector2(-10, 5);
            rect.sizeDelta = new Vector2(500, 20);

            Object.DontDestroyOnLoad(canvasGo);
        }
    }
}
