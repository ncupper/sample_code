using System.Collections.Generic;

using UnityEngine;

using Zenject;
namespace misc.components.storages
{
    public class UniStorage
    {
        private readonly Factory _factory;

        private readonly Dictionary<int, List<ExtMonoBeh>> _intPools = new Dictionary<int, List<ExtMonoBeh>>();

        private readonly Dictionary<int, ExtMonoBeh> _intPrefabs = new Dictionary<int, ExtMonoBeh>();
        private readonly Dictionary<string, List<ExtMonoBeh>> _strPools = new Dictionary<string, List<ExtMonoBeh>>();
        private readonly Dictionary<string, ExtMonoBeh> _strPrefabs = new Dictionary<string, ExtMonoBeh>();

        public UniStorage(Factory factory)
        {
            _factory = factory;
        }

        public void AddPrefab(int name, ExtMonoBeh prefab)
        {
            if (_intPools.ContainsKey(name))
            {
                return;
            }
            _intPrefabs.Add(name, prefab);
            _intPools.Add(name, new List<ExtMonoBeh>());
        }

        public void AddPrefab(string name, ExtMonoBeh prefab)
        {
            if (_strPools.ContainsKey(name))
            {
                return;
            }
            _strPrefabs.Add(name, prefab);
            _strPools.Add(name, new List<ExtMonoBeh>());
        }

        public ExtMonoBeh Get(int name, Transform root)
        {
            if (!_intPools.ContainsKey(name))
            {
                return null;
            }
            List<ExtMonoBeh> pool = _intPools[name];
            for (var i = 0; i < pool.Count; ++i)
            {
                if (pool[i] != null && !pool[i].Visible)
                {
                    pool[i].Visible = true;
                    if (root != null)
                    {
                        pool[i].transform.SetParent(root, false);
                    }

                    return pool[i];
                }
            }

            if (_intPrefabs[name] == null)
            {
                return null;
            }

            ExtMonoBeh item = _factory.Create(_intPrefabs[name], root);

            _intPools[name].Add(item);
            item.Visible = true;
            item.name = _intPrefabs[name].name;

            return item;
        }

        public ExtMonoBeh Get(string name, Transform root)
        {
            if (!_strPools.ContainsKey(name))
            {
                return null;
            }
            List<ExtMonoBeh> pool = _strPools[name];
            for (var i = 0; i < pool.Count; ++i)
            {
                if (!pool[i].Visible)
                {
                    pool[i].Visible = true;
                    if (root != null)
                    {
                        pool[i].transform.SetParent(root, false);
                    }

                    return pool[i];
                }
            }

            if (_strPrefabs[name] == null)
            {
                return null;
            }

            ExtMonoBeh item = _factory.Create(_strPrefabs[name], root);

            _strPools[name].Add(item);
            item.Visible = true;
            item.name = name;

            return item;
        }

        public T Get<T>(int name, Transform root) where T : ExtMonoBeh
        {
            return Get(name, root) as T;
        }

        public T Get<T>(string name, Transform root) where T : ExtMonoBeh
        {
            return Get(name, root) as T;
        }

        public void PreCache(Transform root, int name, int count)
        {
            if (!_intPools.ContainsKey(name))
            {
                return;
            }

            for (var i = 0; i < count; ++i)
            {
                Get(name, root);
            }

            List<ExtMonoBeh> pool = _intPools[name];
            for (var i = 0; i < pool.Count; ++i)
            {
                pool[i].Visible = false;
            }
        }

        public bool PreCache(Transform root, string name, int count)
        {
            if (!_strPools.ContainsKey(name))
            {
                return false;
            }

            for (var i = 0; i < count; ++i)
            {
                Get(name, root);
            }

            List<ExtMonoBeh> pool = _strPools[name];
            for (var i = 0; i < pool.Count; ++i)
            {
                pool[i].Visible = false;
            }

            return true;
        }

        public void ClearAll()
        {
            _intPools.Clear();
            _intPrefabs.Clear();
            _strPools.Clear();
            _strPrefabs.Clear();
        }

        public void DestroyAll()
        {
            foreach (KeyValuePair<int, List<ExtMonoBeh>> pair in _intPools)
            {
                List<ExtMonoBeh> pool = pair.Value;
                for (var i = 0; i < pool.Count; ++i)
                {
                    if (pool[i] != null)
                    {
                        Object.DestroyImmediate(pool[i].gameObject);
                    }
                }
            }
            foreach (KeyValuePair<string, List<ExtMonoBeh>> pair in _strPools)
            {
                List<ExtMonoBeh> pool = pair.Value;
                for (var i = 0; i < pool.Count; ++i)
                {
                    if (pool[i] != null)
                    {
                        Object.DestroyImmediate(pool[i].gameObject);
                    }
                }
            }
            ClearAll();
        }

        public void DestroyRedundant(int minCount)
        {
            foreach (KeyValuePair<int, List<ExtMonoBeh>> pair in _intPools)
            {
                List<ExtMonoBeh> pool = pair.Value;
                while (pool.Count > minCount)
                {
                    if (pool[minCount] != null)
                    {
                        Object.DestroyImmediate(pool[minCount].gameObject);
                    }
                    pool.RemoveAt(minCount);
                }
            }
            foreach (KeyValuePair<string, List<ExtMonoBeh>> pair in _strPools)
            {
                List<ExtMonoBeh> pool = pair.Value;
                while (pool.Count > minCount)
                {
                    if (pool[minCount] != null)
                    {
                        Object.DestroyImmediate(pool[minCount].gameObject);
                    }
                    pool.RemoveAt(minCount);
                }
            }
        }

        public class Factory : PlaceholderFactory<ExtMonoBeh, Transform, ExtMonoBeh>
        {
            private readonly DiContainer _container;
            public Factory(DiContainer container)
            {
                _container = container;
            }

            public override ExtMonoBeh Create(ExtMonoBeh prefab, Transform root)
            {
                return _container.InstantiatePrefab(prefab, root).GetComponent<ExtMonoBeh>();
            }
        }
    }
}
