using UnityEditor;
using UnityEngine;
using System.IO;

namespace CustomVersion.Core
{
    [InitializeOnLoad]
    static class StreamingAssetsCreator
    {
        private static readonly string defaultJson =
        "{\n" +
        "    \"release\": \"v0.0.0\",\n" +
        "    \"build\":   \"0\",\n" +
        "    \"date\":    \"0\"\n" +
        "    \"environment\":    \"dev\"\n" +
        "}";
        static StreamingAssetsCreator()
        {
            EditorApplication.delayCall += EnsureStreamingAssets;
        }

        private static void EnsureStreamingAssets()
        {
            var saPath = Application.streamingAssetsPath;
            if (!Directory.Exists(saPath))
            {
                Directory.CreateDirectory(saPath);
                Debug.Log($"[Hyper Version] Criada pasta: {saPath}");
            }
            if (!File.Exists(jsonPath))
            {
                try
                {
                    File.WriteAllText(jsonPath, defaultJson);
                    Debug.Log($"[Hyper Version] Criado version.json em: {jsonPath}");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[Hyper Version] Falha ao criar version.json: {ex.Message}");
                }
            }
            AssetDatabase.Refresh();
        }
    }
}