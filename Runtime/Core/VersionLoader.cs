using System.Collections;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace CustomVersion.Core
{
    public class VersionLoader : MonoBehaviour
    {
        private TMP_Text _textComp;

        public void Init(TMP_Text textComp)
        {
            _textComp = textComp;
        }

        private void Start()
        {
            StartCoroutine(LoadVersion());
        }

        private IEnumerator LoadVersion()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "version.json");
    		UnityWebRequest www = UnityWebRequest.Get(path);
    		yield return www.SendWebRequest();

    		if (www.result == UnityWebRequest.Result.ConnectionError ||
        		www.result == UnityWebRequest.Result.ProtocolError)
    		{
        		yield break;
    		}

    		VersionData data;
    		try
    		{
        		data = JsonConvert.DeserializeObject<VersionData>(www.downloadHandler.text);
    		}
    		catch
    		{
        		yield break;
    		}
    		if (data == null || data.environment == "release")
        		yield break;

    		if (data != null)
    		{
        		if (!string.IsNullOrEmpty(data.environment))
            		_textComp.text = $"v{data.release}.{data.build}-{data.environment}";
        		else
            		_textComp.text = $"v{data.release}.{data.build}";
    		}
        }
    }
}