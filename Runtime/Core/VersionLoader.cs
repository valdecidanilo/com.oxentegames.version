using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HyperVersion.Core
{
    public class VersionLoader : MonoBehaviour
    {
        public static VersionData Data { get; private set; }

        private IEnumerator Start()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "version.json");

            using var request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[Version] Erro ao carregar version.json: {request.error}");
                yield break;
            }

            Data = JsonUtility.FromJson<VersionData>(request.downloadHandler.text);

            Debug.Log($"[Version] v{Data.release}.{Data.build} env:{Data.environment}");

            if (ShowVersion.OnShowVersion)
            {
                ShowInsideGame();
            }
        }

        private void ShowInsideGame()
        {
            string version = $"v{Data.release}.{Data.build}";

            if (!string.IsNullOrEmpty(Data.environment) && Data.environment != "release")
                version += $"-{Data.environment}";

            Debug.Log($"[Version UI] {version}");
            // aqui você atualiza TMP/Text/UI
        }
    }
}