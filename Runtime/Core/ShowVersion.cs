using CustomVersion.Core;
using UnityEngine;
public class ShowVersion : MonoBehaviour
{
    private void Start()
    {
        var version = Resources.Load<TextAsset>("version");
        var json = JsonUtility.FromJson<VersionData>(version.text);
    }
}