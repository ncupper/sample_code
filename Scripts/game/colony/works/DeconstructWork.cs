using game.colony.works.funcs;
using game.wagon_buildings;

using loader.database;
namespace game.colony.works
{
    internal class DeconstructWork : ColonyWork
    {
        private DeconstructFunc _deconstructFunc;
        private bool _isCanceling;

        public DeconstructWork(Colony colony) : base(colony)
        {
        }

        public WagonBuilding Building { get; private set; }

        public void Setup(WagonBuilding building)
        {
            Building = building;

            Building.PlayerData.LeftToBuildPercents = 100;
            _deconstructFunc = new DeconstructFunc(this, Building);
            Funcs.Add(_deconstructFunc);

            WagonBuilding warehouse = Colony.Buildings.GetWarehouse();

            foreach (DbResVal resVal in Building.PlayerData.Storage.Items)
            {
                bool isIng = Building.Data.IsCraftIngredient(resVal.Id);
                if (isIng)
                {
                    CreateCarrierFunc(Building, warehouse, resVal);
                }
            }

            for (var i = 0; i < Building.Data.Ings.Length; ++i)
            {
                CreateCarrierFunc(Building, warehouse, Building.Data.Ings[i]);
            }

            Colony.Team.PushWork(Funcs);
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            if (Funcs.Count == 0)
            {
                if (!_isCanceling)
                {
                    Building.Constructing.FinalDeconstruct();
                }
                else
                {
                    Building.Constructing.FinalCostruct();
                }
            }
        }

        public override void DoCancel()
        {
            base.DoCancel();
            _isCanceling = !_isCanceling;
            if (!_isCanceling)
            {
                int lastFuncsCount = Funcs.Count;
                WagonBuilding warehouse = Colony.Buildings.GetWarehouse();
                for (var i = 0; i < Building.Data.Ings.Length; ++i)
                {
                    CreateCarrierFunc(Building, warehouse, Building.Data.Ings[i]);
                }
                Colony.Team.PushWork(Funcs, lastFuncsCount);
            }
        }
    }
}
