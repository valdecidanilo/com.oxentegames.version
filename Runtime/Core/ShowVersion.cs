using System;
using TMPro;
using HyperVersion.Core;
using UnityEngine;
public class ShowVersion : MonoBehaviour
{
    public static Action<bool> OnShowVersion;
    private TMP_Text _versionText;
    private void OnEnabled()
    {
        OnShowVersion += Show;
    }
    private void OnDisabled()
    {
        OnShowVersion -= Show;
    }
    public void Initialize(TMP_Text text, string typeAmbience)
    {
        _versionText = text;
        var isRelease = typeAmbience.Equals("release", StringComparison.CurrentCultureIgnoreCase) || typeAmbience.Equals("hml", StringComparison.CurrentCultureIgnoreCase);
        if(isRelease) _versionText.enabled = false;
    }
    private void EnableVersionInfo(bool isShow) => _versionText.enabled = isShow;
}