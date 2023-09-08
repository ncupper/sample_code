using game.colony;

using loader;

using misc;

using UnityEngine;

using Zenject;
namespace game
{
    internal class GameStarter : ExtMonoBeh
    {
        [SerializeField] private Camera _mainCamera;
        private Colony _colony;
        private MouseSelectionHandler _selection;

        [Inject]
        public void Construct(Colony colony, MouseSelectionHandler selection)
        {
            _colony = colony;
            _selection = selection;
        }

        protected override void OnAwake()
        {
            if (!DataStorage.IsLoaded)
            {
                SceneSwitcher.SwitchScene(SceneSwitcher.Startup);
                return;
            }
            base.OnAwake();
        }

        protected override void OnStart()
        {
            base.OnStart();
            _colony.StartGame();
            _selection.StartGame(_mainCamera);
        }
    }
}
