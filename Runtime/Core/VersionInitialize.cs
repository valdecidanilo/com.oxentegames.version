using TMPro;
using UnityEngine;

namespace CustomVersion.Core
{
    public abstract class VersionInitialize
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeCanvasVersion()
        {
            var canvasInst =new GameObject("CanvasVersion").AddComponent<Canvas>();
            canvasInst.renderMode = RenderMode.ScreenSpaceOverlay;
            Object.DontDestroyOnLoad(canvasInst);

            var versionTextComp = new GameObject("Version_Text").AddComponent<TextMeshProUGUI>();
            versionTextComp.transform.SetParent(canvasInst.transform);
            versionTextComp.alignment = TextAlignmentOptions.MidlineRight;
            var rectText = (RectTransform) versionTextComp.GetComponent<Transform>();
            rectText.anchorMin = Vector2.right;
            rectText.anchorMax = Vector2.right;
            rectText.pivot = Vector2.right;
            rectText.sizeDelta = new Vector2(1058f, 50f);
            rectText.anchoredPosition = Vector2.zero;
            if (versionTextComp == null) return;

            var loader = canvasInst.gameObject.AddComponent<VersionLoader>();
            loader.Init(versionTextComp);
        }
    }
}
