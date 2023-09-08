using game.colony.works.funcs;
using game.wagon_buildings;

using loader;
using loader.database;

using UnityEngine;
namespace game.colony.works
{
    internal class CraftWork : ColonyWork
    {

        public CraftWork(Colony colony) : base(colony)
        {
        }

        public override bool IsFinished => false;

        public WagonBuilding Building
        {
            get;
            private set;
        }

        public DbCraftWork CraftData
        {
            get;
            private set;
        }

        public void Setup(WagonBuilding building, DbCraftWork work)
        {
            Building = building;
            CraftData = work;

            GenCraftFuncs();
        }

        private void GenCraftFuncs()
        {
            int craftCount = CraftData.Count;
            if (CraftData.Condition == CraftCondition.Infinity)
            {
                craftCount = 100;
            }
            else if (CraftData.Condition == CraftCondition.Until)
            {
                int haveRes = DbWarehouse.GetRes(DataStorage.Player.Resources, CraftData.ResId).Count;
                if (haveRes >= craftCount)
                {
                    return;
                }
                craftCount = Mathf.Min(100, craftCount - haveRes);
            }

            WagonBuilding warehouse = Colony.Buildings.GetWarehouse();

            DbResource resData = DataStorage.Resources[CraftData.ResId];
            int countMakes = craftCount / resData.Craft.Count;
            for (var i = 0; i < resData.Craft.Ings.Length; ++i)
            {
                DbResource ingData = DataStorage.Resources[resData.Craft.Ings[i].Id];
                int carrierCount = resData.Craft.Ings[i].Count * countMakes;
                while (carrierCount > 0)
                {
                    int count = Mathf.Min(ingData.StackSize, carrierCount);
                    Funcs.Add(new CarrierFunc(this, warehouse, Building, ingData.Id, count));
                    carrierCount -= count;
                }
            }

            Funcs.Add(new CraftFunc(this, Building, CraftData, craftCount));

            Colony.Team.PushWork(Funcs);
        }

        public override void DoUpdate(float dt)
        {
            if (CraftData != null)
            {
                if (CraftData.Condition != CraftCondition.None)
                {
                    if (Funcs.Count == 0)
                    {
                        GenCraftFuncs();
                    }
                }
            }

            base.DoUpdate(dt);
        }

        public override void DoCancel()
        {
            base.DoCancel();
            CraftData = null;
        }
    }
}
