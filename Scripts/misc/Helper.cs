using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using misc.managers;

using UnityEditor;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

using Zenject;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
namespace misc
{
    public static class Helper
    {
        private static readonly StringBuilder Builder = new StringBuilder(400);
        private static string _days;
        private static string _hours;

        private static int _lang = -1;
        private static string _minutes;
        private static string _seconds;
        private static uint _serverDelta;

        private static long _syncRnd;

        private static Camera _worldCam;
        private static Transform _worldCamSelf;

        private static DateTime _zero;

        public static long DebugDelta;
        public static uint ServerDelta
        {
            get => _serverDelta;
            set => _serverDelta = value - (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static uint TimeStamp => (uint)(RealTimeStamp + DebugDelta);

        public static uint RealTimeStamp
        {
            get
            {
                if (_zero.Year != 1970)
                {
                    _zero = new DateTime(1970, 1, 1);
                }
                long unixTimestamp = ServerDelta + (long)DateTime.UtcNow.Subtract(_zero).TotalSeconds;
                return (uint)unixTimestamp;
            }
        }

        public static long LocalTimeStampMs
        {
            get
            {
                var unixTimestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                return unixTimestamp;
            }
        }

        public static long LocalTimeStampTicks
        {
            get
            {
                long unixTimestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Ticks;
                return unixTimestamp;
            }
        }
        public static T GetChild<T>(GameObject obj, string name, bool useHidden = false) where T : Component
        {
            T[] ar = obj.GetComponentsInChildren<T>(useHidden);
            return ar.FirstOrDefault(item => item.name == name);
        }

        public static T GetChildLike<T>(GameObject obj, string name) where T : Component
        {
            T[] ar = obj.GetComponentsInChildren<T>();
            return ar.FirstOrDefault(item => item.name.StartsWith(name));
        }

        public static T RootChildLike<T>(string name) where T : Component
        {
            Object[] objs = Object.FindObjectsOfType(typeof(T));
            return (from o in objs where o.name.StartsWith(name) && o is T select o as T).FirstOrDefault();
        }

        public static GameObject GetRootGameObject(string name)
        {
            GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject o in objs)
            {
                if (o.name == name)
                {
                    return o;
                }
            }
            return null;
        }

        public static T GetRootObject<T>(string name) where T : Component
        {
            GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject o in objs)
            {
                if (o.name.StartsWith(name))
                {
                    var cmp = o.GetComponent<T>();
                    if (cmp != null)
                    {
                        return cmp;
                    }
                }
            }
            return null;
        }

        public static T GetRootObject<T>() where T : Component
        {
            GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject o in objs)
            {
                var cmp = o.GetComponent<T>();
                if (cmp != null)
                {
                    return cmp;
                }
            }
            return null;
        }

        public static T GetChild<T>(Transform parent, string name) where T : Component
        {
            T[] ar = parent.gameObject.GetComponentsInChildren<T>();
            return ar.FirstOrDefault(item => item.name == name);
        }

        public static T Init<T>(GameObject obj) where T : Component
        {
            var c = obj.GetComponent<T>();
            return c;
        }

        private static GameObject Clone(GameObject go)
        {
            GameObject clone = Object.Instantiate(go);
            if (clone != null)
            {
                clone.transform.SetParent(go.transform.parent, false);
                clone.transform.localScale = go.transform.localScale;
                clone.transform.position = go.transform.position;
            }
            return clone;
        }

        public static T Clone<T>(GameObject go, Transform parent = null) where T : Component
        {
            if (parent == null)
            {
                parent = go.transform.parent;
            }
            GameObject clone = Object.Instantiate(go);
            if (clone != null)
            {
                clone.transform.localScale = go.transform.localScale;
                clone.transform.SetParent(parent, false);
                clone.transform.localPosition = go.transform.localPosition;
            }
            return clone != null ? clone.GetComponent<T>() : null;
        }

        public static T Clone<T>(T sample, Transform parent = null) where T : Component
        {
            if (parent == null)
            {
                parent = sample.transform.parent;
            }
            var clone = Object.Instantiate(sample.gameObject, parent).GetComponent<T>();
            return clone;
        }

        public static GameObject Clone(GameObject go, Transform parent)
        {
            if (parent == null)
            {
                parent = go.transform.parent;
            }
            GameObject clone = Object.Instantiate(go);
            if (clone != null)
            {
                clone.transform.SetParent(parent, false);
            }

            return clone;
        }

        public static int Parse(string s)
        {
            var res = 0;
            var k = 1;
            for (int i = s.Length - 1; i >= 0; --i)
            {
                char ch = s[i];
                if (ch >= '0' && ch <= '9')
                {
                    res += (s[i] - '0') * k;
                }
                else if (ch == '-')
                {
                    res = -res;
                    break;
                }
                else
                {
                    break;
                }
                k *= 10;
            }
            return res;
        }

        public static int ParseHex(string s)
        {
            var res = 0;
            var k = 1;
            for (int i = s.Length - 1; i >= 0; --i)
            {
                char ch = s[i];
                if (ch >= '0' && ch <= '9')
                {
                    res += (s[i] - '0') * k;
                }
                else if (ch >= 'a' && ch <= 'f')
                {
                    res += (s[i] - 'a' + 10) * k;
                }
                else if (ch >= 'A' && ch <= 'F')
                {
                    res += (s[i] - 'A' + 10) * k;
                }
                else
                {
                    break;
                }
                k *= 16;
            }
            return res;
        }

        public static int ParseHex(char ch)
        {
            var res = 0;
            if (ch >= '0' && ch <= '9')
            {
                res += ch - '0';
            }
            else if (ch >= 'a' && ch <= 'f')
            {
                res += ch - 'a' + 10;
            }
            else if (ch >= 'A' && ch <= 'F')
            {
                res += ch - 'A' + 10;
            }
            return res;
        }

        public static T Parse<T>(string name, T def)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), name, true);
            }
            catch (ArgumentException)
            {
                return def;
            }
        }

        public static string RemoveAll(string s, char ch)
        {
            if (Builder.Length > 0)
            {
                Builder.Remove(0, Builder.Length);
            }
            for (var i = 0; i < s.Length; ++i)
            {
                if (s[i] != ch)
                {
                    Builder.Append(s[i]);
                }
            }
            return Builder.ToString();
        }

        public static string Signed(int val)
        {
            if (val > 0)
            {
                return "+" + val;
            }
            return val.ToString();
        }

        public static bool IsEqual(float v0, float v1, float epsilon = 0.001f)
        {
            return Mathf.Abs(v0 - v1) < epsilon;
        }

        public static Color ColorFromInt(int color)
        {
            float r = (color >> 16) % 256 / 255.0f;
            float g = (color >> 8) % 256 / 255.0f;
            float b = color % 256 / 255.0f;
            return new Color(r, g, b);
        }

        public static Color ColorFromInt(string hex)
        {
            return ColorFromInt(ParseHex(hex));
        }

        public static string ColorToHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255);
            int g = Mathf.RoundToInt(color.g * 255);
            int b = Mathf.RoundToInt(color.b * 255);
            return ((r << 16) + (g << 8) + b).ToString("X6");
        }

        public static Vector2 ResizeScrollerContext(GameObject lastItem, bool horisontal)
        {
            if (lastItem == null)
            {
                return Vector2.zero;
            }
            var rect = lastItem.GetComponent<RectTransform>();
            var content = lastItem.transform.parent.gameObject.GetComponent<RectTransform>();
            Vector2 sz = content.sizeDelta;
            if (horisontal)
            {
                sz.x = rect.anchoredPosition.x + rect.sizeDelta.x * (1.0f - rect.pivot.x);
            }
            else
            {
                sz.y = Math.Abs(rect.anchoredPosition.y) + rect.sizeDelta.y * rect.pivot.y;
            }
            content.sizeDelta = sz;
            return content.sizeDelta;
        }

        public static void Log(string msg)
        {
            if (Application.isEditor)
            {
                Debug.Log(msg);
            }
        }

        public static void LogWarning(string msg)
        {
            Debug.LogWarning(msg);
        }

        public static void LogError(string msg)
        {
            Debug.LogError(msg);
        }

        private static long GetRandom()
        {
            //algorithm MMIX by Donald Knuth
            _syncRnd = 6364136223846793005 * _syncRnd + 1442695040888963407;
            return Math.Abs(_syncRnd);
        }

        public static void SetRandomize(long r)
        {
            _syncRnd = r;
            Random.InitState((int)r);
        }

        public static int SyncRand(int min, int max = 0)
        {
            if (_syncRnd == 0)
            {
                _syncRnd = Random.Range(0, int.MaxValue);
            }

            if (max == min)
            {
                return min;
            }
            if (max == 0)
            {
                return (int)(GetRandom() % min);
            }

            return (int)(GetRandom() % (max - min) + min);
        }

        public static float SyncRand(float min, float max)
        {
            if (IsEqual(min, max))
            {
                return min;
            }
            var dist = (int)(10000.0f * (max - min));
            return GetRandom() % dist / 10000.0f + min;
        }

        public static string GetDottedTimeSpan(long uts)
        {
            if (uts < TimeStamp)
            {
                uts = (int)TimeStamp;
            }
            var value = new TimeSpan(0, 0, (int)(uts - TimeStamp));
            string duration = (value.Hours + value.Days * 24).ToString("00") + ":" + value.Minutes.ToString("00") + ":"
                + value.Seconds.ToString("00");
            return duration;
        }

        public static string GetDottedDeltaTime(long uts, bool needHours = true)
        {
            var value = new TimeSpan(0, 0, (int)uts);
            var duration = "";
            if (value.Hours > 0 || needHours)
            {
                duration += (value.Hours + value.Days * 24).ToString("00") + ":";
            }
            duration += value.Minutes.ToString("00") + ":" + value.Seconds.ToString("00");
            return duration;
        }
        public static string GetReadableDeltaTime(int deltaTime, int count = 2)
        {
            if (_lang != (int)Lang.Instance.CurLang || string.IsNullOrEmpty(_seconds))
            {
                _seconds = Lang.Get("sec");
                _minutes = Lang.Get("min");
                _hours = Lang.Get("hour");
                _days = Lang.Get("day");
                _lang = (int)Lang.Instance.CurLang;
            }

            if (Builder.Length > 0)
            {
                Builder.Remove(0, Builder.Length);
            }

            var value = new TimeSpan(0, 0, deltaTime);
            if (value.Days > 0)
            {
                Builder.Append(value.Days);
                Builder.Append(_days);
                --count;
            }
            if (count > 0 && value.Hours > 0)
            {
                if (Builder.Length > 0)
                {
                    Builder.Append(' ');
                }
                Builder.Append(value.Hours);
                Builder.Append(_hours);
                --count;
            }
            if (count > 0 && value.Minutes > 0)
            {
                if (Builder.Length > 0)
                {
                    Builder.Append(' ');
                }
                Builder.Append(value.Minutes);
                Builder.Append(_minutes);
                --count;
            }
            if (count > 0 && value.Seconds > 0)
            {
                if (Builder.Length > 0)
                {
                    Builder.Append(' ');
                }
                Builder.Append(value.Seconds);
                Builder.Append(_seconds);
                --count;
            }

            return Builder.ToString();
        }

        public static string GetReadableDeltaTimeMinutes(int deltaTime)
        {
            if (_lang != (int)Lang.Instance.CurLang || string.IsNullOrEmpty(_seconds))
            {
                _seconds = Lang.Get("sec");
                _minutes = Lang.Get("min");
                _hours = Lang.Get("hour");
                _days = Lang.Get("day");
                _lang = (int)Lang.Instance.CurLang;
            }

            if (Builder.Length > 0)
            {
                Builder.Remove(0, Builder.Length);
            }

            int value = (deltaTime + 30) / 60;
            Builder.Append(value);
            Builder.Append(_minutes);

            return Builder.ToString();
        }

        public static string GetShortInt(int val)
        {
            if (val >= 1000000)
            {
                return val / 1000000 + "M";
            }
            if (val >= 1000)
            {
                return val / 1000 + "k";
            }

            return val.ToString();
        }

        private static void AddCharToBuilder(int val, int num)
        {
            if (val > 9)
            {
                ++num;
                AddCharToBuilder(val / 10, num);
            }
            if (num > 0 && num % 3 == 0)
            {
                Builder.Append(' '); // ');
            }
            Builder.Append(val % 10);
        }
        public static string GetSeparatedStr(int val)
        {
            Builder.Clear();
            AddCharToBuilder(val, 0);
            return Builder.ToString().TrimStart();
        }

        public static void ShareScreen(string msg)
        {
            Log("Sharing: " + msg);
#if UNITY_ANDROID
        if (Application.isMobilePlatform)
        {
            new AndroidUnityNativeSharingAdapter().ShareText(msg);
            /*
            //Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
            //screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
            //screenTexture.Apply();
            //byte[] dataToSave = screenTexture.EncodeToJPG();

            //string destination = Path.Combine(Application.persistentDataPath, "tpc-" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".jpg");

            //File.WriteAllBytes(destination, dataToSave);

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            //AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            //AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            if (!string.IsNullOrEmpty(msg))
            {
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), msg);
            }
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");
            //intentObject.Call<AndroidJavaObject>("setType", "image/jpg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            // option one WITHOUT chooser:
            //currentActivity.Call("startActivity", intentObject);

            // option two WITH chooser:
            AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share link using:");
            currentActivity.Call("startActivity", jChooser);
            */
        }
#elif UNITY_IOS
        new IosUnityNativeSharingAdapter().ShareText(msg);
#else
#endif
        }

        public static T Load<T>(string path) where T : Component
        {
            var prefub = Resources.Load<GameObject>(path);
            return Clone<T>(prefub);
        }

        public static void ResizePool<T>(IList<T> pool, T sample, int count, UnityAction<T> onCreate = null,
            Transform parent = null) where T : ExtMonoBeh
        {
            for (var i = 0; i < count; ++i)
            {
                if (i >= pool.Count)
                {
                    sample.Visible = true;
                    pool.Add(sample.Clone<T>(parent));
                    onCreate?.Invoke(pool[i]);
                }
                pool[i].Visible = true;
            }
            sample.Visible = false;
            for (int i = count; i < pool.Count; ++i)
            {
                pool[i].Visible = false;
            }
        }

        public static void ResizePool<T>(Factory factory, IList<T> pool, T sample, int count, UnityAction<T> onCreate = null)
            where T : ExtMonoBeh
        {
            for (var i = 0; i < count; ++i)
            {
                if (i >= pool.Count)
                {
                    sample.Visible = true;
                    GameObject clone = factory.Create(sample.gameObject, sample.Self.parent);
                    pool.Add(clone.GetComponent<T>());
                    onCreate?.Invoke(pool[i]);
                }
                pool[i].Visible = true;
            }
            sample.Visible = false;
            for (int i = count; i < pool.Count; ++i)
            {
                pool[i].Visible = false;
            }
        }

        public static void ResizePoolComponents<T>(IList<T> pool, T sample, int count, UnityAction<T> onCreate = null)
            where T : Component
        {
            for (var i = 0; i < count; ++i)
            {
                if (i >= pool.Count)
                {
                    sample.gameObject.SetActive(true);
                    pool.Add(Clone(sample.gameObject).GetComponent<T>());
                    onCreate?.Invoke(pool[i]);
                }
                pool[i].gameObject.SetActive(true);
            }
            sample.gameObject.SetActive(false);
            for (int i = count; i < pool.Count; ++i)
            {
                pool[i].gameObject.SetActive(false);
            }
        }

        public static void ResizePoolGameObjects(IList<GameObject> pool, GameObject sample, int count,
            UnityAction<GameObject> onCreate = null)
        {
            for (var i = 0; i < count; ++i)
            {
                if (i >= pool.Count)
                {
                    sample.gameObject.SetActive(true);
                    pool.Add(Clone(sample.gameObject));
                    onCreate?.Invoke(pool[i]);
                }
                pool[i].gameObject.SetActive(true);
            }
            sample.gameObject.SetActive(false);
            for (int i = count; i < pool.Count; ++i)
            {
                pool[i].gameObject.SetActive(false);
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

        public static void SendEmail(string email, string subject, string body)
        {
            subject = UnityWebRequest.EscapeURL(subject).Replace("+", "%20");
            body = UnityWebRequest.EscapeURL(body).Replace("+", "%20");
            Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
        }

        public static void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }
        public static Vector3 WorldToScreen(Transform target, float offset, ScreenSwitcher screenSwitcher)
        {
            if (_worldCam != screenSwitcher.Canvas.worldCamera)
            {
                _worldCam = screenSwitcher.Canvas.worldCamera;
                _worldCamSelf = _worldCam.transform;
            }
            Vector2 pos = RectTransformUtility.WorldToScreenPoint(_worldCam, target.position + _worldCamSelf.up * offset);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(screenSwitcher.SelfRect, pos, _worldCam, out Vector2 local);
            return screenSwitcher.SelfRect.TransformPoint(local);
        }

        public static string DeviceUniqueIdentifier()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public static long TotalSeconds(this DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static void OpenMarket()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Application.OpenURL("https://apps.apple.com/us/app/story-of-alcana-match-3/id1418553295");
            }
            else
            {
                if (Application.identifier[0] == 'h')
                {
                    Application.OpenURL("appmarket://details?id=" + Application.identifier);
                }
                else
                {
                    Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
                }
            }
        }

        public static string PercentToString(int val)
        {
            int cents = val % 100;
            if (cents == 0)
            {
                return (val / 100).ToString();
            }

            string result = val / 100 + ".";
            if (cents < 10)
            {
                result += "0";
            }
            return result + cents;
        }

        public class Factory : PlaceholderFactory<GameObject, Transform, GameObject>
        {
            private readonly DiContainer _container;

            public Factory(DiContainer container)
            {
                _container = container;
            }

            public override GameObject Create(GameObject prefab, Transform root)
            {
                GameObject clone = _container.InstantiatePrefab(prefab, root);
                clone.name = prefab.name;
                return clone;
            }
        }
    }
}
