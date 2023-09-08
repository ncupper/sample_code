using System.Collections.Generic;

using game.colony;

using misc;

using UnityEngine;

using Zenject;
namespace gui.crew
{
    internal class CrewPanel : ExtMonoBeh
    {
        [SerializeField] private CrewItem _sample;

        private readonly List<CrewItem> _pool = new List<CrewItem>();
        private Colony _colony;

        [Inject]
        public void Construct(Colony colony)
        {
            _colony = colony;
        }

        protected override void OnStart()
        {
            base.OnStart();

            int workersCount = _colony.Team.Count;
            Helper.ResizePool(_pool, _sample, workersCount);

            for (var i = 0; i < workersCount; ++i)
            {
                _pool[i].Setup(_colony.Team.GetWorker(i));
            }
        }
    }
}
