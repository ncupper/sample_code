using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Zenject;

using Object = UnityEngine.Object;
namespace misc.components.storages
{
    public class UniAsyncStorage
    {
        private readonly Factory _factory;

        private readonly Dictionary<int, List<ExtMonoBeh>> _pools = new Dictionary<int, List<ExtMonoBeh>>();

        private readonly Dictionary<int, AssetReference> _prefabs = new Dictionary<int, AssetReference>();

        public UniAsyncStorage(Factory factory)
        {
            _factory = factory;
        }

        public void AddPrefab(int name, AssetReference prefab)
        {
            if (_pools.ContainsKey(name))
            {
                return;
            }
            _prefabs.Add(name, prefab);
            _pools.Add(name, new List<ExtMonoBeh>());
        }

        public async Task<ExtMonoBeh> GetAsync(int name, Transform root)
        {
            if (!_pools.ContainsKey(name))
            {
                return null;
            }
            List<ExtMonoBeh> pool = _pools[name];
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

            if (_prefabs[name] == null)
            {
                return null;
            }

            ExtMonoBeh item = await _factory.CreateASync(_prefabs[name], root);

            _pools[name].Add(item);
            item.Visible = true;

            return item;
        }

        public async Task<T> GetAsync<T>(int name, Transform root) where T : ExtMonoBeh
        {
            return await GetAsync(name, root) as T;
        }

        public async void PreCache(Transform root, int name, int count)
        {
            if (!_pools.ContainsKey(name))
            {
                return;
            }

            for (var i = 0; i < count; ++i)
            {
                await GetAsync(name, root);
            }

            List<ExtMonoBeh> pool = _pools[name];
            for (var i = 0; i < pool.Count; ++i)
            {
                pool[i].Visible = false;
            }
        }

        public void ClearAll()
        {
            _pools.Clear();
            _prefabs.Clear();
        }

        public void DestroyAll()
        {
            foreach (KeyValuePair<int, List<ExtMonoBeh>> pair in _pools)
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
            foreach (KeyValuePair<int, List<ExtMonoBeh>> pair in _pools)
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

        public class Factory : PlaceholderFactory<AssetReference, Transform, ExtMonoBeh>
        {
            private readonly DiContainer _container;
            public Factory(DiContainer container)
            {
                _container = container;
            }

            public async Task<ExtMonoBeh> CreateASync(AssetReference prefabReference, Transform root)
            {
                AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(prefabReference);
                await loadHandle.Task;

                if (loadHandle.Status == AsyncOperationStatus.Failed)
                {
                    throw new Exception("Asset load failed", loadHandle.OperationException);
                }
                var item = _container.InstantiatePrefab(loadHandle.Result, root).GetComponent<ExtMonoBeh>();
                item.name = loadHandle.Result.name;
                return item;
            }
        }
    }
}
