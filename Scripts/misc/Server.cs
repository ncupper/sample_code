using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;
namespace misc
{
    public class Server
    {

        public static string Url = "http://fudog-bbr2.appspot.com/";

        private MonoBehaviour _manager;

        public Server()
        {
            Instance = this;
        }
        public static Server Instance { get; private set; }
        private class ServerRequest
        {
            public readonly Action<JSONNode> Handler;
            public readonly UnityWebRequest Request;
            public ServerRequest(string url, Action<JSONNode> handler)
            {
                Request = new UnityWebRequest(url);
                Handler = handler;
            }

            public ServerRequest(string url, Action<JSONNode> handler, string data)
            {
                Request = new UnityWebRequest(url, data);
                Handler = handler;
            }
        }

        public void SetManager(MonoBehaviour manager)
        {
            _manager = manager;
        }

        public static int Now()
        {
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

            return currentEpochTime;
        }

        public static void GetRequest(string url, Action<JSONNode> handler)
        {
            var request = new ServerRequest(url, handler);
            Instance._manager.StartCoroutine(Instance.WaitForRequest(request));
        }

        public static void PostRequest(string url, string form, Action<JSONNode> handler)
        {
            var request = new ServerRequest(url, handler, form);
            Instance._manager.StartCoroutine(Instance.WaitForRequest(request));
        }

        private IEnumerator WaitForRequest(ServerRequest request)
        {
            yield return request.Request;

            if (request.Request.error == null)
            {
                JSONNode json = JSON.Parse(request.Request.downloadHandler.text);
                if (json != null && json["data"] != null && json["data"].GetLong("tm") > 0)
                {
                    Helper.ServerDelta = (uint)json["data"].GetLong("tm");
                }
                Helper.Log("Request: " + request.Request.url + " \nResponse: " + request.Request.downloadHandler.text);
                request.Handler?.Invoke(json);
            }
            else
            {
                Helper.LogWarning("Request: " + request.Request.url + " \nError: " + request.Request.error);
                request.Handler?.Invoke(null);
            }

        }
    }
}
