using game.colony.works.funcs;
using game.wagon_buildings;

using loader.database;
namespace game.colony.works
{
    internal class CarrierWork : ColonyWork
    {
        private WagonBuilding _building;
        private DbResVal _carrierTask;

        public CarrierWork(Colony colony) : base(colony)
        {
        }

        public void Setup(WagonBuilding building, DbResVal task)
        {
            _building = building;
            _carrierTask = task;

            WagonBuilding warehouse = Colony.Buildings.GetWarehouse();

            Funcs.Add(new CarrierFunc(this, _building, warehouse, task.Id, task.Count));

            Colony.Team.PushWork(Funcs);
        }
    }
}
