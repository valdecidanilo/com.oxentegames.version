using UnityEditor;
using UnityEngine;
using System.IO;
using CustomVersion.Core;

[InitializeOnLoad]
public static class ResourcesVersionCreator
{
    private const string ResourcesFolderPath = "Assets/Resources";
    private const string VersionFileName    = "version.json";
    private static readonly string VersionFilePath =
        Path.Combine(Application.dataPath, "Resources", VersionFileName);

    static ResourcesVersionCreator()
    {
        EditorApplication.delayCall += EnsureResourcesVersionJson;
    }

    [MenuItem("Tools/HyperVersion/Initialize Resources")]
    public static void InitializeResourcesVersion()
    {
        EnsureResourcesVersionJson();
    }

    private static void EnsureResourcesVersionJson()
    {
        if (!AssetDatabase.IsValidFolder(ResourcesFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
            Debug.Log($"[HyperVersion] Criada pasta Resources em: {ResourcesFolderPath}");
        }

        if (!File.Exists(VersionFilePath))
        {
            var initial = new VersionData
            {
                release     = PlayerSettings.bundleVersion,
                build       = "0",
                data        = "0",
                environment = "dev"
            };
            string defaultJson = JsonUtility.ToJson(initial, true);

            try
            {
                File.WriteAllText(VersionFilePath, defaultJson);
                Debug.Log($"[HyperVersion] Criado {VersionFileName} em Resources:\n{defaultJson}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[HyperVersion] Falha ao criar {VersionFileName}: {ex.Message}");
            }
        }
        else
        {
            Debug.Log($"[HyperVersion] {VersionFileName} já existe em Resources.");
        }

        AssetDatabase.Refresh();
    }
}
