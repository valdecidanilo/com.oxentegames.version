using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace HyperVersion.Core
{
    public static class HyperVersionInitialize
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            var go = new GameObject("[HyperVersion]");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<HyperVersionWebLoader>();
#else
            var versionFilePath = Path.Combine(Application.streamingAssetsPath, "version.json");

            if (!File.Exists(versionFilePath))
            {
                Debug.LogWarning("[HyperVersion] version.json not found.");
                return;
            }

            var json = File.ReadAllText(versionFilePath);
            ApplyVersionData(json);
#endif
        }

        internal static void ApplyVersionData(string json)
        {
            var data = JsonUtility.FromJson<VersionData>(json);

            if (data == null)
            {
                Debug.LogWarning("[HyperVersion] version.json invalid.");
                return;
            }

            Debug.Log($"[HyperVersion] version: {data.release}.{data.build}");
        }
    }

    internal class HyperVersionWebLoader : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(LoadVersion());
        }

        private IEnumerator LoadVersion()
        {
            var url = Path.Combine(Application.streamingAssetsPath, "version.json");

            using var req = UnityWebRequest.Get(url);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[HyperVersion] version.json not found: {req.error}");
            }
            else
            {
                HyperVersionInitialize.ApplyVersionData(req.downloadHandler.text);
            }

            Destroy(gameObject);
        }
    }
}
