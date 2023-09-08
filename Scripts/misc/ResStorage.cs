using System.Collections.Generic;

using UnityEngine;
namespace misc
{
    internal static class ResStorage
    {
        private static readonly Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
        private static readonly Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();
        private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

        public static void Clear()
        {
            Sprites.Clear();
            Prefabs.Clear();
            Sounds.Clear();
        }

        public static Sprite GetSprite(string path)
        {
            if (Sprites.ContainsKey(path))
            {
                return Sprites[path];
            }
            var s = Resources.Load<Sprite>(path);
            if (s == null)
            {
                Helper.LogError("Invalid path for icon: [" + path + "]");
            }
            Sprites.Add(path, s);
            return s;
        }

        public static JSONNode GetJson(string path)
        {
            var s = Resources.Load<TextAsset>(path);
            if (s == null || string.IsNullOrEmpty(s.text))
            {
                Helper.LogError("Invalid path for json: [" + path + "]");
                return new JSONNode();
            }
            return JSON.Parse(s.text);
        }

        public static string GetText(string path, bool showError = true)
        {
            var s = Resources.Load<TextAsset>(path);
            if (s == null || string.IsNullOrEmpty(s.text))
            {
                if (showError)
                {
                    Helper.LogError("Invalid path for text: [" + path + "]");
                }
                return string.Empty;
            }
            return s.text;
        }

        public static void RemoveCachePrefab(GameObject obj)
        {
            foreach (KeyValuePair<string, GameObject> kvp in Prefabs)
            {
                if (kvp.Value != obj)
                {
                    continue;
                }
                RemoveCachePrefab(kvp.Key);
                break;
            }
        }

        public static void RemoveCachePrefab(string key)
        {
            Prefabs.Remove(key);
        }

        public static GameObject GetPrefab(string path, bool log = true)
        {
            if (Prefabs.ContainsKey(path))
            {
                return Prefabs[path];
            }

            var s = Resources.Load<GameObject>(path);
            if (s == null)
            {
                if (log)
                {
                    Helper.LogError("Invalid path for prefab: [" + path + "]");
                }
            }

            Prefabs.Add(path, s);
            return s;
        }

        public static T GetPrefab<T>(string path, bool log = true) where T : Component
        {
            if (Prefabs.ContainsKey(path))
            {
                return Prefabs[path].GetComponent<T>();
            }

            var s = Resources.Load<GameObject>(path);
            if (s == null)
            {
                if (log)
                {
                    Helper.LogError("Invalid path for prefab: [" + path + "]");
                }
            }

            Prefabs.Add(path, s);
            return s.GetComponent<T>();
        }

        public static AudioClip GetSound(string path, bool log = true)
        {
            if (Sounds.ContainsKey(path))
            {
                return Sounds[path];
            }
            var s = Resources.Load<AudioClip>(path);
            if (s == null && log)
            {
                Helper.LogError("Invalid path for sound: [" + path + "]");
            }
            Sounds.Add(path, s);
            return s;
        }

        public static Texture GetTexture(string path)
        {
            //if (Sprites.ContainsKey(path)) return Sprites[path];
            var s = Resources.Load<Texture>(path);
            if (s == null)
            {
                Helper.LogError("Invalid path for texture: [" + path + "]");
            }
            //Sprites.Add(path, s);
            return s;
        }
    }
}
