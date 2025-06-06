using TMPro;
using UnityEngine;

namespace CustomVersion.Core
{
    public abstract class VersionInitialize
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeCanvasVersion()
        {
            var prefab = Resources.Load<GameObject>("CanvasVersion");
            if (prefab == null) return;

            var canvasInst = Object.Instantiate(prefab);
            canvasInst.name = "CanvasVersion";
            Object.DontDestroyOnLoad(canvasInst);

            var versionTextComp = canvasInst.transform.Find("Version_Text")?.GetComponent<TMP_Text>();
            if (versionTextComp == null) return;

            var loader = canvasInst.AddComponent<VersionLoader>();
            loader.Init(versionTextComp);
        }
    }
}
