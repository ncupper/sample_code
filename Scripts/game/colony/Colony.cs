using gui;

using loader;

using misc;

using UnityEngine;

using Zenject;
namespace game.colony
{
    public class Colony : ExtMonoBeh
    {

        public ColonyWorks Works
        {
            get;
            private set;
        }
        public ColonyTrain Train
        {
            get;
            private set;
        }
        public ColonyBuildings Buildings
        {
            get;
            private set;
        }
        public ColonyTeam Team
        {
            get;
            private set;
        }

        private void Update()
        {
            Works.DoUpdate(Time.deltaTime);
        }

        private void LateUpdate()
        {
            Works.DoLateUpdate();
        }

        [Inject]
        public void Construct(DataLoader dataLoader, IconsCamera iconsCamera)
        {
            Works = new ColonyWorks(this);
            Train = new ColonyTrain();
            Buildings = new ColonyBuildings(this, dataLoader);
            Team = new ColonyTeam(iconsCamera);
        }

        public void StartGame()
        {
            Train.Setup();
            Buildings.Setup();
            Team.Setup();
        }
    }
}
