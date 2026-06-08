using System.IO;
using UnityEngine;

namespace HyperVersion.Core
{
    public static class HyperVersionInitialize
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var versionFilePath = Path.Combine(Application.streamingAssetsPath, "version.json");

            if (!File.Exists(versionFilePath))
            {
                Debug.LogWarning("[HyperVersion] version.json not found.");
                return;
            }

            var json = File.ReadAllText(versionFilePath);
            var data = JsonUtility.FromJson<VersionData>(json);

            if (data == null)
            {
                Debug.LogWarning("[HyperVersion] version.json invalid.");
                return;
            }

            Debug.Log($"[HyperVersion] version: {data.release}.{data.build}");
        }
    }
}
