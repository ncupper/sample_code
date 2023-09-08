using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
namespace misc.network
{
    internal class NetRequest
    {

        private static byte[] _unzip = new byte[1500 * 1024];
        private float _wait;
        public Action<JSONNode> OnComplete;
        public Action OnError;

        public IEnumerator Get(string url, Dictionary<string, string> headers)
        {
            _wait = 0;
            UnityWebRequest w = UnityWebRequest.Get(url);
            foreach (KeyValuePair<string, string> header in headers)
            {
                w.SetRequestHeader(header.Key, header.Value);
            }
            Helper.Log("GET: " + url);
            yield return DoRequest(w);
        }

        public IEnumerator Post(string url, string plain, Dictionary<string, string> headers)
        {
            _wait = 0;
            UnityWebRequest w = UnityWebRequest.Post(url, plain);
            foreach (KeyValuePair<string, string> header in headers)
            {
                w.SetRequestHeader(header.Key, header.Value);
            }
            Helper.Log("POST: " + url + " [" + plain + "]");
            yield return DoRequest(w);
        }

        private IEnumerator DoRequest(UnityWebRequest w)
        {
            UnityWebRequestAsyncOperation async = w.SendWebRequest();
            while (!async.isDone)
            {
                if (_wait > 10)
                {
                    //Net.OnError("Timeout " + w.url);
                    if (OnError != null)
                    {
                        OnError();
                    }
                    yield break;
                }
                _wait += Time.deltaTime;
                yield return null;
            }
            if (w.result == UnityWebRequest.Result.ConnectionError)
            {
                //Net.OnError(w.error + " " + w.url);
                if (OnError != null)
                {
                    OnError();
                }
            }
            else
            {
                LoadComplete(w.url, w.GetResponseHeaders(), w.downloadHandler.text, w.downloadHandler.data);
            }
        }
        public void LoadComplete(string url, Dictionary<string, string> headers, string response, byte[] data)
        {
            /*bool compressed = headers != null && headers.ContainsKey("CONTENT-ENCODING") && headers["CONTENT-ENCODING"] == "gzip";
		if (Application.platform == RuntimePlatform.OSXEditor)
			compressed = false;

		if (compressed)
        {
            response = string.Empty;
            int bufSize = 1024;
            var gzip = new GZipInputStream(new MemoryStream(data));
            int offset = 0;
            while (bufSize == 1024)
            {
                bufSize = gzip.Read(_unzip, offset, bufSize);
                offset += bufSize;
                if (offset >= _unzip.Length)
                {
                    var t = _unzip;
                    _unzip = new byte[_unzip.Length + bufSize];
                    Buffer.BlockCopy(t, 0, _unzip, 0, t.Length);
                    Debug.LogError("Need resize initial unzip buffer to [" + _unzip.Length + "]");
                }
            }
            response += Encoding.UTF8.GetString(_unzip, 0, offset);
            Helper.Log("compressed");
        }*/
            if (response.Length < 20000)
            {
                Helper.Log("Request: " + url + "\nResponse: " + response);
            }
            else
            {
                Helper.Log("Request: " + url + "\nBig Response");
            }

            try
            {
                JSONNode payload = JSON.Parse(response);
                if (payload == null)
                {
                    /*Helper.LogError("Request: " + url + "\nBad Answer: " + response);
                //Net.OnError("answer parsing error");
                if (OnError != null)
                {
                    OnError();
                }*/
                    OnComplete?.Invoke(new JSONNode());
                }
                else
                {
                    OnComplete?.Invoke(payload);
                }
            }
            catch
            {
                Helper.LogError("Request: " + url + "\nBad Answer: " + response);
            }
        }
    }
}
