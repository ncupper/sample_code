using System;
using System.Collections.Generic;

using misc;

using UnityEngine.Events;
#pragma warning disable 649

namespace loader.database
{
    [Serializable]
    internal class DbPlayer
    {
        public delegate void DbPlayerChanged();

        private const int ActualVersion = 2;
        public int Ver;
        public string Name;
        public long Tutors;

        public int WagonCount;

        public List<DbResVal> Resources = new List<DbResVal>();

        public string AppVersion;
        public long LocalSaveTimestamp; // время последнего сейва (локального)
        public List<DbPlayerWagonBuilding> WagonBuildings = new List<DbPlayerWagonBuilding>();

        public int OldVer { get; set; }
        public event DbPlayerChanged OnDbPlayerChanged = delegate {};
        public event UnityAction<string, string> OnUpdateAppVersion = delegate {};

        public void NewPlayer()
        {
            Helper.Log("new player");
            Ver = ActualVersion;
            Name = "";
            Tutors = 0;

            WagonCount = 1;
            WagonBuildings.Clear();
            PlaceStartupBuildings();

            Resources.Clear();
            WagonBuildings[0].Storage.GainRes((int)ResId.GarbageGlass, 1000);
            WagonBuildings[0].Storage.GainRes((int)ResId.GarbageMetal, 3000);
            WagonBuildings[0].Storage.GainRes((int)ResId.GarbagePlastic, 2000);
            WagonBuildings[0].Storage.GainRes((int)ResId.GarbageOrganic, 500);
            WagonBuildings[0].Storage.GainRes((int)ResId.Metal, 500);
            WagonBuildings[0].Storage.GainRes((int)ResId.Plastic, 500);
            WagonBuildings[0].Storage.GainRes((int)ResId.Glass, 500);
            WagonBuildings[0].Storage.GainRes((int)ResId.MechanicParts, 600);
            WagonBuildings[0].Storage.GainRes((int)ResId.ElectronicParts, 600);

            AppVersion = CurrentBundleVersion.Version;
            LocalSaveTimestamp = 0;
            OnDbPlayerChanged();
        }

        public int GetWagonBuildingNextUid()
        {
            if (WagonBuildings.Count > 0)
            {
                return WagonBuildings[WagonBuildings.Count - 1].Uid + 1;
            }

            return 1;
        }

        private void PlaceStartupBuildings()
        {
            AddBuilding((int)WagonBuildingId.Warehouse, 1, 141, 0);
            AddBuilding((int)WagonBuildingId.GarbageRecycler, 1, 181, 0);
        }

        public DbPlayerWagonBuilding AddBuilding(int id, int wagonIdx, int cellIdx, int angle)
        {
            var b = new DbPlayerWagonBuilding
            {
                Uid = GetWagonBuildingNextUid(),
                Building = id,
                WagonIdx = wagonIdx,
                CellIdx = cellIdx,
                Angle = angle,
                Condition = 100
            };
            WagonBuildings.Add(b);
            return b;
        }

        public void Save(JSONNode data)
        {
            data.Add("v", Ver);

            data.Add("name", Name);
            data.Add("Tutors", Tutors);

            data.Add("WagonCount", WagonCount);
            data.Add("TrainBuildings", WagonBuildings, DbPlayerWagonBuilding.Save);

            LocalSaveTimestamp = Helper.TimeStamp;
            data.Add("local_save_time", LocalSaveTimestamp);

            data.Add("app_ver", AppVersion);
        }

        public void Load(JSONNode data)
        {
            Ver = data.GetInt("v", 1);

            Name = data.GetKey("name");
            Tutors = data.GetLong("Tutors");

            WagonCount = data.GetInt("WagonCount", 1);
            WagonBuildings = data.GetList("TrainBuildings", DbPlayerWagonBuilding.Load);

            LocalSaveTimestamp = data.GetLong("local_save_time");
            AppVersion = data.GetKey("app_ver");

            OnDbPlayerChanged();
        }

        public void JobAfterLoad()
        {
            if (Ver < 2)
            {
                NewPlayer();
            }

            if (AppVersion != CurrentBundleVersion.Version)
            {
                OnUpdateAppVersion(AppVersion, CurrentBundleVersion.Version);
                AppVersion = CurrentBundleVersion.Version;
            }

            OldVer = Ver;
            Ver = ActualVersion;
        }

        public void SetName(string n)
        {
            Name = n;
        }

        public void DoAfterAllParamsLoading()
        {
        }

        public void GainRes(int resId, int count, string source = null)
        {
            for (var i = 0; i < Resources.Count; ++i)
            {
                if (Resources[i].Id == resId)
                {
                    Resources[i].Count += count;
                    return;
                }
            }

            var r = new DbResVal
            {
                Id = resId, Count = count
            };
            Resources.Add(r);
        }

        public void PayRes(int resId, int count)
        {
            for (var i = 0; i < Resources.Count; ++i)
            {
                if (Resources[i].Id == resId)
                {
                    Resources[i].Count -= count;
                    if (Resources[i].Count < 0)
                    {
                        Resources[i].Count = 0;
                    }
                    break;
                }
            }
        }
    }
}
