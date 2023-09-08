using System;
using System.Collections.Generic;

using misc;

using UnityEngine;
namespace loader.database
{
    public enum ResId
    {
        None,
        GarbageMetal,
        GarbagePlastic,
        GarbageGlass,
        GarbageOrganic,
        Metal,
        Plastic,
        Glass,
        MechanicParts,
        ElectronicParts,
        Chemical,
        Heal,
        Cloth
    }

    public class RenderIconContainer
    {
        [NonSerialized] public RenderTexture Texture;
    }

    [Serializable]
    public class DbResVal
    {
        public int Id;
        public int Count;
        public int Reserved;

        [NonSerialized] private DbResource _params;
        public DbResource Params
        {
            get
            {
                if (_params == null)
                {
                    _params = DataStorage.Resources[(ResId)Id];
                }

                return _params;
            }
        }

        public static JSONNode Save(DbResVal item)
        {
            var data = new JSONClass();
            data.Add("Id", item.Id);
            data.Add("C", item.Count);
            return data;
        }

        public static DbResVal Load(JSONNode data)
        {
            var item = new DbResVal
            {
                Id = data.GetInt("Id"), Count = data.GetInt("C")
            };
            return item;
        }
    }

    [Serializable]
    public class DbReceipt
    {
        public DbResVal[] Ings;
        public int Count;
    }

    [Serializable]
    public class DbResource : RenderIconContainer
    {
        public int Id;
        public string Name;
        public string Desc;
        public string Icon;
        public int StackSize;
        public DbReceipt Craft;
    }

    [Serializable]
    internal class DbResources
    {
        public DbResource[] Items;

        public DbResource this[ResId id] => this[(int)id];

        public DbResource this[int id]
        {
            get
            {
                for (var i = 0; i < Items.Length; ++i)
                {
                    if (Items[i].Id == id)
                    {
                        return Items[i];
                    }
                }
                return null;
            }
        }
    }

    internal static class DbWarehouse
    {

        private static readonly DbResVal EmptyRes = new DbResVal();
        public static bool HaveRes(IReadOnlyList<DbResVal> storage, int resId, int count)
        {
            for (var i = 0; i < storage.Count; ++i)
            {
                if (storage[i].Id == resId)
                {
                    return storage[i].Count - storage[i].Reserved >= count;
                }
            }
            return false;
        }
        public static DbResVal GetRes(IReadOnlyList<DbResVal> storage, int resId)
        {
            for (var i = 0; i < storage.Count; ++i)
            {
                if (storage[i].Id == resId)
                {
                    return storage[i];
                }
            }

            EmptyRes.Id = resId;
            EmptyRes.Count = 0;
            return EmptyRes;
        }

        public static void GainRes(List<DbResVal> storage, int resId, int count)
        {
            DataStorage.Player.GainRes(resId, count);
            for (var i = 0; i < storage.Count; ++i)
            {
                if (storage[i].Id == resId)
                {
                    storage[i].Count += count;
                    return;
                }
            }
            storage.Add(new DbResVal
            {
                Id = resId, Count = count
            });
        }

        public static void PayRes(List<DbResVal> storage, int resId, int count)
        {
            for (var i = 0; i < storage.Count; ++i)
            {
                if (storage[i].Id == resId)
                {
                    count = Mathf.Min(count, storage[i].Count);
                    storage[i].Count -= count;
                    DataStorage.Player.PayRes(resId, count);
                    storage[i].Reserved -= Mathf.Min(count, storage[i].Reserved);
                    break;
                }
            }
        }

        public static int BoxCount(int resCount, int stackSize)
        {
            return resCount / stackSize + (resCount % stackSize > 0 ? 1 : 0);
        }

        public static int BoxCount(IReadOnlyList<DbResVal> storage, int resId = 0, int count = 0)
        {
            var boxCount = 0;
            for (var i = 0; i < storage.Count; ++i)
            {
                if (storage[i].Id == resId)
                {
                    boxCount += BoxCount(storage[i].Count + count, storage[i].Params.StackSize);
                }
                else
                {
                    boxCount += BoxCount(storage[i].Count, storage[i].Params.StackSize);
                }
            }

            return boxCount;
        }

        public static void RemoveZero(List<DbResVal> storage)
        {
            storage.RemoveAll(x => x.Count <= 0);
        }

        public static bool IsEmpty(IReadOnlyList<DbResVal> storage)
        {
            for (var i = 0; i < storage.Count; ++i)
            {
                if (storage[i].Count > 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class DbResStorage
    {
        private List<DbResVal> _items = new List<DbResVal>();
        public IReadOnlyList<DbResVal> Items => _items;

        public event Action OnChanged;

        public void PayRes(int resId, int count)
        {
            DbWarehouse.PayRes(_items, resId, count);
            OnChanged?.Invoke();
        }

        public void GainRes(int resId, int count)
        {
            DbWarehouse.GainRes(_items, resId, count);
            OnChanged?.Invoke();
        }

        public void RemoveZero()
        {
            DbWarehouse.RemoveZero(_items);
        }

        public void Save(JSONNode data, string name)
        {
            data.Add(name, _items, DbResVal.Save);
        }

        public void Load(JSONNode data, string name)
        {
            _items = data.GetList(name, DbResVal.Load);
        }
    }
}
