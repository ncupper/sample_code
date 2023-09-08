using System.Collections.Generic;

using UnityEngine;
namespace misc
{
    public class Lang
    {
        private readonly Dictionary<string, string> _cn = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _de = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _en = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _es = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _fr = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _id = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _it = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _jp = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _ko = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _pl = new Dictionary<string, string>();

        private readonly Dictionary<string, string> _pt = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _ru = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _tr = new Dictionary<string, string>();
        private Dictionary<string, string> _lng;

        public Lang()
        {
            Instance = this;
        }

        public static Lang Instance { get; private set; }

        public SystemLanguage CurLang
        {
            get;
            private set;
        }

        public void Load(string text, SystemLanguage lang)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            JSONNode data = JSON.Parse(text);
            JSONArray ar = data.AsArray;

            Dictionary<string, string> dict = null;
            var valKey = "";
            switch (lang)
            {
            case SystemLanguage.Russian:
                dict = _ru;
                valKey = "ru";
                break;
            case SystemLanguage.English:
                dict = _en;
                valKey = "en";
                break;
            case SystemLanguage.Italian:
                dict = _it;
                valKey = "it";
                break;
            case SystemLanguage.German:
                dict = _de;
                valKey = "de";
                break;
            case SystemLanguage.French:
                dict = _fr;
                valKey = "fr";
                break;
            case SystemLanguage.Chinese:
                dict = _cn;
                valKey = "cn";
                break;
            case SystemLanguage.Japanese:
                dict = _jp;
                valKey = "jp";
                break;
            case SystemLanguage.Polish:
                dict = _pl;
                valKey = "pl";
                break;
            case SystemLanguage.Portuguese:
                dict = _pt;
                valKey = "pt";
                break;
            case SystemLanguage.Korean:
                dict = _ko;
                valKey = "ko";
                break;
            case SystemLanguage.Indonesian:
                dict = _id;
                valKey = "id";
                break;
            case SystemLanguage.Turkish:
                dict = _tr;
                valKey = "tr";
                break;
            case SystemLanguage.Spanish:
                dict = _es;
                valKey = "es";
                break;
            }

            if (ar == null || dict == null)
            {
                //Debug.LogError("Lang empty for path: " + path);
                return;
            }

            for (var i = 0; i < ar.Count; i++)
            {
                JSONNode item = ar[i];
                string key = item.GetKey("key");
                dict.Add(key, item.GetKey(valKey, "", true));
            }
        }

        public void Load(string path = "lang", string arName = null)
        {
            var lngRes = Resources.Load<TextAsset>(path);
            if (lngRes == null)
            {
                return;
            }
            JSONNode data = JSON.Parse(lngRes.text);
            JSONArray ar = string.IsNullOrEmpty(arName) ? data.AsArray : data.GetArray(arName);
            if (ar == null)
            {
                Debug.LogError("Lang empty for path: " + path);
                return;
            }
            for (var i = 0; i < ar.Count; i++)
            {
                JSONNode item = ar[i];
                string key = item.GetKey("key");
                if (_ru.ContainsKey(key))
                {
                    Debug.LogError("Lang duplicates for key: " + key);
                    continue;
                }
                _ru.Add(key, item.GetKey("ru", "", true));
                _en.Add(key, item.GetKey("en", "", true));
                _it.Add(key, item.GetKey("it", "", true));
                _de.Add(key, item.GetKey("de", "", true));
                _fr.Add(key, item.GetKey("fr", "", true));
                _cn.Add(key, item.GetKey("cn", "", true));
                _jp.Add(key, item.GetKey("jp", "", true));

                _pl.Add(key, item.GetKey("pl", "", true));
                _pt.Add(key, item.GetKey("pt", "", true));
                _ko.Add(key, item.GetKey("ko", "", true));
                _id.Add(key, item.GetKey("id", "", true));
                _tr.Add(key, item.GetKey("tr", "", true));
                _es.Add(key, item.GetKey("es", "", true));
            }
        }

        private string GetText(string key)
        {
            if (_lng != null && _lng.ContainsKey(key))
            {
                return _lng[key];
            }
            return key;
        }

        public static string Get(string key)
        {
            return Instance != null && !string.IsNullOrEmpty(key) ? Instance.GetText(key) : key;
        }

        public static string Get(string key, int val)
        {
            string fmt = Instance != null ? Instance.GetText(key) : "";
            if (fmt == key)
            {
                fmt += " {0}";
            }

            try
            {
                return Instance != null ? string.Format(fmt, val) : "";
            }
            catch
            {
                return fmt + " " + val;
            }
        }

        public static string Get(string key, int val0, int val1)
        {
            string fmt = Instance != null ? Instance.GetText(key) : "";
            if (fmt == key)
            {
                fmt += " {0}/{1}";
            }
            return Instance != null ? string.Format(fmt, val0, val1) : "";
        }

        public static string Get(string key, string val)
        {
            string fmt = Instance != null ? Instance.GetText(key) : "";
            if (fmt == key)
            {
                fmt += " {0}";
            }
            return Instance != null ? string.Format(fmt, val) : "";
        }

        public void SetLang(SystemLanguage lang)
        {
            CurLang = lang;
            if (lang == SystemLanguage.Russian)
            {
                _lng = _ru;
            }
            else if (lang == SystemLanguage.Italian)
            {
                _lng = _it;
            }
            else if (lang == SystemLanguage.French)
            {
                _lng = _fr;
            }
            else if (lang == SystemLanguage.German)
            {
                _lng = _de;
            }
            else if (lang == SystemLanguage.Polish)
            {
                _lng = _pl;
            }
            else if (lang == SystemLanguage.Portuguese)
            {
                _lng = _pt;
            }
            else if (lang == SystemLanguage.Spanish)
            {
                _lng = _es;
            }
            else if (lang == SystemLanguage.Japanese)
            {
                _lng = _jp;
            }
            else if (lang == SystemLanguage.Chinese)
            {
                _lng = _cn;
            }
            else if (lang == SystemLanguage.Korean)
            {
                _lng = _ko;
            }
            //else if (lang == SystemLanguage.Indonesian) _lng = _id;
            //else if (lang == SystemLanguage.Turkish) _lng = _tr;
            else
            {
                _lng = _en;
            }
        }

        public static Dictionary<string, string>.KeyCollection GetKeys()
        {
            return Instance._lng.Keys;
        }
    }
}
