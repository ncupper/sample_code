using game.colony.works.funcs;
using game.wagon_buildings;

using loader.database;
namespace game.colony.works
{
    internal class ConstructWork : ColonyWork
    {
        private ConstructFunc _constructFunc;
        private bool _isCanceling;

        public ConstructWork(Colony colony) : base(colony)
        {
        }

        public WagonBuilding Building { get; private set; }

        public void Setup(WagonBuilding building)
        {
            Building = building;

            WagonBuilding warehouse = Colony.Buildings.GetWarehouse();

            for (var i = 0; i < Building.Data.Ings.Length; ++i)
            {
                CreateCarrierFunc(warehouse, Building, Building.Data.Ings[i]);
            }
            _constructFunc = new ConstructFunc(this, Building);
            Funcs.Add(_constructFunc);

            Colony.Team.PushWork(Funcs);
        }

        public override void DoCancel()
        {
            base.DoCancel();

            _isCanceling = !_isCanceling;
            if (_isCanceling)
            {
                int oldFuncs = Funcs.Count;
                WagonBuilding warehouse = Colony.Buildings.GetWarehouse();
                if (!DbWarehouse.IsEmpty(Building.PlayerData.Storage.Items))
                {
                    foreach (DbResVal resVal in Building.PlayerData.Storage.Items)
                    {
                        CreateCarrierFunc(Building, warehouse, resVal);
                    }
                }
                else
                {
                    for (var i = 0; i < Building.Data.Ings.Length; ++i)
                    {
                        CreateCarrierFunc(Building, warehouse, Building.Data.Ings[i]);
                    }
                }

                Colony.Team.PushWork(Funcs, oldFuncs);
            }
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            if (_isCanceling && DbWarehouse.IsEmpty(Building.PlayerData.Storage.Items)
                && (_constructFunc.IsCompleted || _constructFunc.IsCanceled))
            {
                _isCanceling = false;
                Building.Constructing.FinalDeconstruct();
            }
        }
    }
}
