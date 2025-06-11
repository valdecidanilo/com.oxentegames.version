using System.IO;
using CustomVersion.Core;
using UnityEditor;
using UnityEngine;

namespace CustomVersion.Editor
{
    [InitializeOnLoad]
    public static class ResourcesVersionCreator
    {
        private const string ResourcesFolderPath = "Assets/Resources";
        private const string VersionFileName    = "version.json";
        private static readonly string BootStrapPath = Path.Combine(Application.dataPath, "HyperVersion", "BootStrapVersion.cs");
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
                Directory.CreateDirectory(Path.GetDirectoryName(BootStrapPath));
                string bootstrapScript = @"// AUTO-GERADO PELO HYPERVERSION
using UnityEngine;
using CustomVersion.Core;

namespace HyperVersion.Runtime
{
    internal class BootStrapVersion : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EditorPing()
        {
            Debug.Log(""[BootStrapVersion] Forçando inclusão de CustomVersion.Core."");
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimePing()
        {
            Debug.Log(""[BootStrapVersion] Forçando inclusão de CustomVersion.Core no build."");
            var dummy = new VersionData
            {
                release = ""0"",
                build = ""0"",
                data = ""0"",
                environment = ""dev""
            };
            Debug.Log($""[BootStrapVersion] Dummy version: {dummy.release}.{dummy.build}"");
        }
    }
}
";

                File.WriteAllText(BootStrapPath, bootstrapScript);
                Debug.Log($"[HyperVersion] Criado BootStrapVersion.cs em: {BootStrapPath}");
                var defaultJson = JsonUtility.ToJson(initial, true);
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

            AssetDatabase.Refresh();
        }
    }
}
