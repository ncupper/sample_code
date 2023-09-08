using game.wagon_buildings;

using misc.managers;
namespace game.events
{
    public class TestEvent : EventData
    {
        public TestEvent(WagonBuilding building)
        {
            Building = building;
        }

        public WagonBuilding Building { get; }
    }
}
