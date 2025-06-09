using UnityEditor;
using UnityEngine;
using System.IO;
using CustomVersion.Core;

[InitializeOnLoad]
public static class StreamingAssetsCreator
{
    private static readonly string jsonPath =
        Path.Combine(Application.streamingAssetsPath, "version.json");

    static StreamingAssetsCreator()
    {
        EditorApplication.delayCall += EnsureVersionJsonExists;
    }

    [MenuItem("Tools/HyperVersion/Initialize")]
    public static void EnsureVersionJsonExists()
    {
        var saPath = Application.streamingAssetsPath;
        if (!Directory.Exists(saPath))
        {
            Directory.CreateDirectory(saPath);
            Debug.Log($"[HyperVersion] Criada pasta StreamingAssets em: {saPath}");
        }

        if (!File.Exists(jsonPath))
        {
            var initial = new VersionData
            {
                release     = "v" + PlayerSettings.bundleVersion,
                build       = "0",
                data        = "0",
                environment = "dev"
            };
            string defaultJson = JsonUtility.ToJson(initial, true);

            try
            {
                File.WriteAllText(jsonPath, defaultJson);
                Debug.Log($"[HyperVersion] Criado version.json em: {jsonPath}\n{defaultJson}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[HyperVersion] Falha ao criar version.json: {ex.Message}");
            }
        }
        else
        {
            Debug.Log($"[HyperVersion] version.json já existe em: {jsonPath}");
        }

        AssetDatabase.Refresh();
    }
}
