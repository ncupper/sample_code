using System.Collections.Generic;
namespace game.colony
{
    public class ColonyTrain
    {
        private readonly List<WagonView> _wagons = new List<WagonView>();

        public void Setup()
        {
            _wagons.Add(Train.Instance.Locomotive);
            _wagons.Add(Train.Instance.Wagon);
        }

        public WagonView GetWagon(int index)
        {
            return _wagons[index];
        }
    }
}
