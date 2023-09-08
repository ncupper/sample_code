using System;
using System.Collections.Generic;

using UnityEngine;
namespace misc.managers
{
    internal interface IHaveUpdate
    {
        float Delay { get; }
        bool IsDone { get; }

        void DoUpdate(float dt);
    }

    internal class EasyTimer : IHaveUpdate
    {
        private readonly Action _onComplete;

        public EasyTimer(float delay, Action onComplete)
        {
            Delay = delay;
            _onComplete = onComplete;
        }

        public float Delay
        {
            get;
            private set;
        }

        public void DoUpdate(float dt)
        {
            if (Delay < -0.5f)
            {
                return;
            }
            Delay -= dt;
            if (Delay < 0)
            {
                Delay = -1.0f;
                _onComplete?.Invoke();
            }
        }

        public bool IsDone => Delay < -0.5f;

        public void Restart(float delay)
        {
            Delay = delay;
        }
    }

    internal class TimersManager : MonoBehaviour
    {
        private static TimersManager _instance;
        private readonly List<IHaveUpdate> _items = new List<IHaveUpdate>(200);
        private readonly List<IHaveUpdate> _newItems = new List<IHaveUpdate>(100);
        private readonly List<IHaveUpdate> _removeItems = new List<IHaveUpdate>(100);

        public static TimersManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("TimersManager");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<TimersManager>();
                }
                return _instance;
            }
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            if (delta > 1)
            {
                delta -= (int)delta;
            }

            while (delta > 0.015f)
            {
                DoUpdate(0.015f);
                delta -= 0.015f;
            }
            DoUpdate(delta);
        }

        public static void Add(IHaveUpdate item)
        {
            if (!Instance._newItems.Contains(item) && !Instance._items.Contains(item))
            {
                Instance._newItems.Add(item);
            }
        }

        public static void Remove(IHaveUpdate item)
        {
            Instance._removeItems.Add(item);
        }

        public static void RemoveAll()
        {
            Instance._removeItems.AddRange(Instance._newItems);
            Instance._removeItems.AddRange(Instance._items);
        }

        public static void DoneAll()
        {
            Instance.DoUpdate(0);
            foreach (IHaveUpdate timer in Instance._items)
            {
                timer.DoUpdate(1 + Mathf.Abs(timer.Delay) * 2);
            }
            Instance._items.RemoveAll(x => x.IsDone);
        }

        public static void RemoveAllByType(Type type)
        {
            foreach (IHaveUpdate item in Instance._newItems)
            {
                if (item.GetType() == type)
                {
                    Instance._removeItems.Add(item);
                }
            }
            foreach (IHaveUpdate item in Instance._items)
            {
                if (item.GetType() == type)
                {
                    Instance._removeItems.Add(item);
                }
            }
        }

        private void UpdateRemove()
        {
            for (var i = 0; i < _removeItems.Count; ++i)
            {
                IHaveUpdate item = _removeItems[i];
                if (Instance._newItems.Contains(item))
                {
                    Instance._newItems.Remove(item);
                }
                if (Instance._items.Contains(item))
                {
                    Instance._items.Remove(item);
                }
            }
            _removeItems.Clear();
        }

        private void DoUpdate(float dt)
        {
            UpdateRemove();
            _items.AddRange(_newItems);
            _newItems.Clear();
            foreach (IHaveUpdate timer in _items)
            {
                timer.DoUpdate(dt);
            }
            _items.RemoveAll(x => x.IsDone);
            UpdateRemove();
        }
    }
}
