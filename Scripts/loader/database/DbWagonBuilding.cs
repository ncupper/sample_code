using System;
using System.Collections.Generic;

using game.colony;

using misc;

using UnityEngine;
namespace loader.database
{
    public enum WagonBuildingCat
    {
        None,
        Workbench,
        Crew,
        Other,
        Decor
    }

    internal enum WagonBuildingId
    {
        BuildingPlace = -1,
        None,
        Warehouse = 1,
        GarbageRecycler = 2,
        WorkbenchMetal = 3,
        WorkbenchChip = 4,
        WorkbenchChemical = 5,
        WorkbenchResearch = 6,
        BedSingle = 7
    }

    [Serializable]
    public class DbWagonBuildingMapLineCells
    {
        public int[] Line;
    }

    [Serializable]
    public class DbWagonBuildingMap
    {
        public Vector2Int Size;
        public Vector2Int Offset;
        public DbWagonBuildingMapLineCells[] Cells;
    }

    [Serializable]
    public class DbWagonBuilding : RenderIconContainer
    {
        public int Id;
        public string Name;
        public string Desc;
        public string Icon;
        public int Category;
        public int[] Craft;
        public float CraftTmCoef;
        public DbResVal[] Ings;
        public DbWagonBuildingMap Map;

        public bool HaveCraft => Craft != null && Craft.Length > 0;

        public bool IsCraftIngredient(int resId)
        {
            if (!HaveCraft)
            {
                return false;
            }
            for (var c = 0; c < Craft.Length; ++c)
            {
                DbResource res = DataStorage.Resources[Craft[c]];
                for (var i = 0; i < res.Craft.Ings.Length; ++i)
                {
                    if (resId == res.Craft.Ings[i].Id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    [Serializable]
    internal class DbWagonBuildings
    {
        public DbWagonBuilding[] Items;

        public DbWagonBuilding this[int id]
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
        public DbWagonBuilding this[WagonBuildingId id] => this[(int)id];
    }

    public enum CraftCondition
    {
        None,
        Infinity,
        Until
    }

    public class DbCraftWork
    {
        public int BuildingUid;
        public CraftCondition Condition;
        public int Count;
        public int ResId;

        public static JSONNode Save(DbCraftWork item)
        {
            var data = new JSONClass();
            data.Add("ResId", item.ResId);
            data.Add("Count", item.Count);
            data.Add("Condition", item.Condition.ToString());
            return data;
        }

        public static DbCraftWork Load(JSONNode data)
        {
            var item = new DbCraftWork
            {
                ResId = data.GetInt("ResId"),
                Count = data.GetInt("Count"),
                Condition = data.GetEnum("Condition", CraftCondition.None)
            };
            return item;
        }
    }

    public class DbCraftQueue
    {
        private List<DbCraftWork> _items = new List<DbCraftWork>();
        public IReadOnlyList<DbCraftWork> Items => _items;

        public event Action OnChanged;

        public void Save(JSONNode data, string name)
        {
            data.Add(name, _items, DbCraftWork.Save);
        }

        public void Load(JSONNode data, string name)
        {
            _items = data.GetList(name, DbCraftWork.Load);
        }

        public void Add(DbCraftWork work)
        {
            if ((int)work.Condition == (int)CraftCondition.None)
            {
                _items.RemoveAll(x => x.ResId == work.ResId && (int)x.Condition != (int)CraftCondition.None);
            }
            else
            {
                _items.RemoveAll(x => x.ResId == work.ResId);
            }
            _items.Add(work);
            OnChanged?.Invoke();
        }

        public void Remove(DbCraftWork work)
        {
            _items.Remove(work);
            OnChanged?.Invoke();
        }
    }

    public class DbPlayerWagonBuilding
    {
        public readonly DbCraftQueue Queue;

        public readonly DbResStorage Storage;
        public int Angle;
        public int Building;
        public int CellIdx;
        public int Condition;
        public int LeftToBuildPercents;
        public int Uid;
        public int WagonIdx;

        public DbPlayerWagonBuilding()
        {
            Storage = new DbResStorage();
            Storage.OnChanged += () =>
            {
                OnStorageChanged?.Invoke(this);
            };
            Queue = new DbCraftQueue();
            Queue.OnChanged += () =>
            {
                OnQueueChanged?.Invoke(this);
            };
        }

        public event Action<DbPlayerWagonBuilding> OnStorageChanged;
        public event Action<DbPlayerWagonBuilding> OnQueueChanged;

        public static JSONNode Save(DbPlayerWagonBuilding item)
        {
            var data = new JSONClass();
            data.Add("Uid", item.Building);
            data.Add("Id", item.Building);
            data.Add("WagonIdx", item.WagonIdx);
            data.Add("CellIdx", item.CellIdx);
            data.Add("Angle", item.Angle);
            data.Add("Condition", item.Condition);
            data.Add("IsBuild", item.LeftToBuildPercents);

            item.Storage.Save(data, "Storage");
            item.Queue.Save(data, "Queue");
            return data;
        }

        public static DbPlayerWagonBuilding Load(JSONNode data)
        {
            var item = new DbPlayerWagonBuilding
            {
                Uid = data.GetInt("Uid"),
                Building = data.GetInt("Id"),
                WagonIdx = data.GetInt("WagonIdx"),
                CellIdx = data.GetInt("CellIdx"),
                Angle = data.GetInt("Angle"),
                Condition = data.GetInt("Condition", 100),
                LeftToBuildPercents = data.GetInt("IsBuild")
            };
            item.Storage.Load(data, "Storage");
            item.Queue.Load(data, "Queue");
            for (var i = 0; i < item.Queue.Items.Count; ++i)
            {
                item.Queue.Items[i].BuildingUid = item.Uid;
            }
            return item;
        }

        public void AddTask(Colony colony, DbCraftWork work)
        {
            work.BuildingUid = Uid;
            if ((int)work.Condition == (int)CraftCondition.None)
            {
                foreach (DbCraftWork craftWork in Queue.Items)
                {
                    if (craftWork.ResId == work.ResId && (int)craftWork.Condition != (int)CraftCondition.None)
                    {
                        colony.Works.RemoveCraft(craftWork);
                    }
                }
            }
            else
            {
                foreach (DbCraftWork x in Queue.Items)
                {
                    if (x.ResId == work.ResId && (int)x.Condition == (int)work.Condition)
                    {
                        if ((int)work.Condition == (int)CraftCondition.Until)
                        {
                            x.Count = work.Count;
                            OnQueueChanged?.Invoke(this);
                        }
                        return;
                    }
                }

                foreach (DbCraftWork x in Queue.Items)
                {
                    if (x.ResId == work.ResId)
                    {
                        colony.Works.RemoveCraft(x);
                    }
                }
            }

            Queue.Add(work);
            colony.Works.AddCraft(work);
        }
    }
}
