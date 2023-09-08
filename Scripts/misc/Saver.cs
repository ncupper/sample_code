using System;
using System.Collections.Generic;

using loader;

using UnityEngine;
namespace misc
{
    public abstract class IWantSave
    {
        public string WanterName { get; protected set; }

        public abstract void Load(JSONNode data);

        public abstract void Save(JSONNode data);
    }

    public class Saver
    {
        private readonly List<IWantSave> _wanters = new List<IWantSave>();
        private readonly JSONNode _data;
        private readonly string _fn;

        public Saver(bool reset, string fn)
        {
            _fn = fn;
            if (!reset)
            {
                try
                {
#if UNITY_WEBPLAYER || UNITY_WEBGL
                _data = JSONNode.Parse(PlayerPrefs.GetString(_fn));
                if (_data == null) _data = new JSONClass();
#else
                    _data = JSONNode.Parse(PlayerPrefs.GetString(_fn));
                    if (_data == null)
                    {
                        _data = new JSONClass();
                        _data = JSONNode.LoadFromFile(GetFileName());
                    }
#endif
                }
                catch (Exception)
                {
                    _data = new JSONClass();
                }
            }
            else
            {
                _data = new JSONClass();
            }
            Instance = this;
        }

        public static Saver Instance { get; private set; }

        public void AddWanter(IWantSave wanter)
        {
            _wanters.Add(wanter);
            JSONNode node = _data[wanter.WanterName];
            wanter.Load(node ?? new JSONClass());
        }

        private string GetFileName()
        {
            string fileName = "/" + _fn;
            fileName = Application.persistentDataPath + fileName;
            return fileName;
        }

        public void Load()
        {
            try
            {
                foreach (IWantSave wanter in _wanters)
                {
                    JSONNode node = _data[wanter.WanterName];
                    if (node != null)
                    {
                        wanter.Load(node);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void DoSave(bool updateSaveTimestamp = true)
        {
            if (updateSaveTimestamp && DataStorage.Player.LocalSaveTimestamp < DateTime.Now.Ticks)
            {
                DataStorage.Player.LocalSaveTimestamp = DateTime.Now.Ticks;
            }
            Instance.Save();
        }

        public void Save()
        {
            //var tm = DateTime.UtcNow.Ticks;
            foreach (IWantSave wanter in _wanters)
            {
                var node = new JSONClass();
                _data.Add(wanter.WanterName, node);
                wanter.Save(node);
            }
#if UNITY_EDITOR
            Helper.Log(_data.ToString());
#endif

#if UNITY_WEBPLAYER || UNITY_WEBGL
	    PlayerPrefs.SetString(_fn, _data.ToString());
	    PlayerPrefs.Save();
#else
            PlayerPrefs.SetString(_fn, _data.ToString());
            PlayerPrefs.Save();
            //_data.SaveToFile(GetFileName());
#endif
            //Debug.LogError("ticks: " + (DateTime.UtcNow.Ticks - tm));
        }
    }
}
