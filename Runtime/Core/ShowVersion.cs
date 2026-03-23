using System;
using TMPro;
using UnityEngine;

namespace HyperVersion.Core
{
    public class ShowVersion : MonoBehaviour
    {
        public static Action<bool> OnShowVersion;

        private TMP_Text _versionText;

        public void Initialize(TMP_Text text)
        {
            _versionText = text;
        }

        private void OnEnable()
        {
            OnShowVersion += EnableVersionInfo;
        }

        private void OnDisable()
        {
            OnShowVersion -= EnableVersionInfo;
        }

        private void EnableVersionInfo(bool isShow)
        {
            if (_versionText == null) return;
            _versionText.enabled = isShow;
        }
    }
}