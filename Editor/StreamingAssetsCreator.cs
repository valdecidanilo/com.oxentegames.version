// Assets/Editor/HyperVersionInitializer.cs

using UnityEditor;
using UnityEngine;
using System.IO;
using CustomVersion.Core; // se sua VersionData ficar num namespace diferente, ajuste aqui

[InitializeOnLoad]
public static class StreamingAssetsCreator
{
    // caminho completo para Assets/StreamingAssets/version.json
    private static readonly string jsonPath =
        Path.Combine(Application.streamingAssetsPath, "version.json");

    // Executa logo após o Editor recompilar os scripts
    static HyperVersionInitializer()
    {
        EditorApplication.delayCall += EnsureVersionJsonExists;
    }

    // Também disponível em Tools → HyperVersion → Initialize
    [MenuItem("Tools/HyperVersion/Initialize")]
    public static void EnsureVersionJsonExists()
    {
        // 1) Garante que a pasta StreamingAssets exista
        var saPath = Application.streamingAssetsPath;
        if (!Directory.Exists(saPath))
        {
            Directory.CreateDirectory(saPath);
            Debug.Log($"[HyperVersion] Criada pasta StreamingAssets em: {saPath}");
        }

        // 2) Se version.json não existe, cria com valores iniciais
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

        // 3) Atualiza o Project window
        AssetDatabase.Refresh();
    }
}
