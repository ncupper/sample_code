using System;
using System.Collections.Generic;

using UnityEngine;
namespace misc.managers
{
    public class EventData
    {
    }

    internal interface IEventListener
    {
        bool OnEvent(EventData eventData);
    }

    internal class EventBus : MonoBehaviour
    {
        private static EventBus _instance;

        private readonly Dictionary<Type, List<IEventListener>> _listeners = new Dictionary<Type, List<IEventListener>>();

        private void Update()
        {
            foreach (KeyValuePair<Type, List<IEventListener>> listener in _listeners)
            {
                listener.Value.Remove(null);
            }
        }

        public static void AddListener<T>(IEventListener listener) where T : EventData
        {
            if (_instance == null)
            {
                var go = new GameObject("EventsManager");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<EventBus>();
            }
            if (!_instance._listeners.ContainsKey(typeof(T)))
            {
                _instance._listeners.Add(typeof(T), new List<IEventListener>());
            }
            _instance._listeners[typeof(T)].Add(listener);
        }

        public static void RemoveListener<T>(IEventListener listener) where T : EventData
        {
            if (_instance == null)
            {
                return;
            }
            if (_instance._listeners.ContainsKey(typeof(T)))
            {
                _instance._listeners[typeof(T)].Remove(listener);
            }
        }

        public static bool Invoke<T>(T eventData) where T : EventData
        {
            if (_instance == null)
            {
                return false;
            }

            if (!_instance._listeners.ContainsKey(typeof(T)))
            {
                return false;
            }

            List<IEventListener> listeners = _instance._listeners[typeof(T)];
            listeners.RemoveAll(x => x == null);
            for (var i = 0; i < listeners.Count; ++i)
            {
                if (listeners[i].OnEvent(eventData))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
