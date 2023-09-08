using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;
namespace misc.network
{
    internal class Net : MonoBehaviour
    {
        private static Net _instance;
        private readonly List<NetCommand> _commands = new List<NetCommand>();
        private string _deviceId;

        private string _url;

        private void Update()
        {
            if (_instance == null || _instance._commands.Count == 0)
            {
                return;
            }
            NetCommand cmd = _instance._commands[0];
            cmd.DoUpdate();
            if (cmd.IsComplete)
            {
                _instance._commands.RemoveAt(0);
                if (_instance._commands.Count > 0)
                {
                    _instance._commands[0].SendRequest();
                }
            }
            if (cmd.IsFailed)
            {
                if (_instance._commands.Count > 0)
                {
                    _instance._commands.RemoveAt(0);
                }
                foreach (NetCommand command in _instance._commands)
                {
                    command.Break();
                }
                _instance._commands.Clear();
            }
        }

        public void Init(string url)
        {
            if (_instance == null)
            {
                _instance = this;
                _deviceId = SystemInfo.deviceUniqueIdentifier;
                _url = url;
            }
        }

        public static string Md5Sum(string strToEncrypt)
        {
            var ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);
            var md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);
            var hashString = "";
            for (var i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }
            return hashString.PadLeft(32, '0');
        }

        private static void RequestToUrl(string url, Action<JSONNode> onComplete, Action onError, string postParams)
        {
            var request = new NetRequest
            {
                OnComplete = onComplete, OnError = onError
            };

            var headers = new Dictionary<string, string>();

            if (postParams != null)
            {
                headers.Add("Content-type", "application/json");
                _instance.StartCoroutine(request.Post(url, postParams, headers));
            }
            else
            {
                _instance.StartCoroutine(request.Get(url, headers));
            }
        }

        public static void Request(string req, Action<JSONNode> onComplete, string getParams = "", Action onError = null,
            string postParams = null)
        {
            if (_instance._deviceId == "n/a")
            {
                _instance._deviceId = "webgl";
            }
            //var tm = Helper.TimeStamp;
            //var token = Md5Sum("bbr2" + tm + _instance._deviceId + getParams + 17032015 + req);
            string
                url = _instance._url
                    + req /*+ "&did=" + _instance._deviceId + "&ts=" + Helper.TimeStamp + getParams + "&token=" + token*/;

            RequestToUrl(url, onComplete, onError, postParams);
        }

        public static void OnError(string ololo)
        {
            Debug.LogError(ololo);
        }

        public static void AddCommand(NetCommand cmd)
        {
            if (_instance == null)
            {
                return;
            }
            _instance._commands.Add(cmd);
            if (_instance._commands.Count == 1)
            {
                _instance._commands[0].SendRequest();
            }
        }

        public static void Clear()
        {
            _instance._commands.Clear();
        }

        public static int GetQueuePosOfCmd(NetCommand cmd)
        {
            return _instance._commands.IndexOf(cmd);
        }
    }
}
