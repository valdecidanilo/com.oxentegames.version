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
            // Carrega o JSON dos Resources
            var ta = Resources.Load<TextAsset>("version");
            if (ta == null)
            {
                _textComp.text = "v0.0.0.0";
                return;
            }

            VersionData data;
            try
            {
                data = JsonUtility.FromJson<VersionData>(ta.text);
            }
            catch
            {
                _textComp.text = "v0.0.0.0";
                return;
            }

            if (data == null || data.environment == "release")
            {
                _textComp.text = "";
                return;
            }

            _textComp.text = !string.IsNullOrEmpty(data.environment)
                ? $"v{data.release}.{data.build}-{data.environment}"
                : $"v{data.release}.{data.build}";
        }
    }
}