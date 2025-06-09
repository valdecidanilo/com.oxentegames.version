using TMPro;
using UnityEngine;

namespace CustomVersion.Core
{
    public class VersionLoader : MonoBehaviour
    {
        private TMP_Text _textComp;
        public void Init(TMP_Text textComp) => _textComp = textComp;

        private void Start()
        {
            Debug.Log("[VersionLoader] Start() chamado — tentando Resources.Load<TextAsset>(\"version\")");
            var ta = Resources.Load<TextAsset>("version");
            if (ta == null)
            {
                Debug.LogError("[VersionLoader] Resources.Load retornou null! O arquivo `Assets/Resources/version.json` existe?");
                _textComp.text = "v0.0.0.0";
                return;
            }

            Debug.Log($"[VersionLoader] TextAsset carregado, comprimento = {ta.text.Length}");
            VersionData data;
            try
            {
                data = JsonUtility.FromJson<VersionData>(ta.text);
            }
            catch
            {
                Debug.LogError("[VersionLoader] JsonUtility.FromJson falhou.");
                _textComp.text = "v0.0.0.0";
                return;
            }

            if (data == null)
            {
                Debug.LogError("[VersionLoader] Desserialização retornou null.");
                _textComp.text = "v0.0.0.0";
                return;
            }

            if (data.environment == "release")
            {
                Debug.Log("[VersionLoader] Ambiente = release, não exibindo texto.");
                _textComp.text = "";
                return;
            }

            string display = !string.IsNullOrEmpty(data.environment)
                ? $"v{data.release}.{data.build}-{data.environment}"
                : $"v{data.release}.{data.build}";
            Debug.Log($"[VersionLoader] Exibindo: {display}");
            _textComp.text = display;
        }
    }
}
