using game.colony;
using game.wagon_buildings;

using misc;
using misc.components;

using UnityEngine;

using Zenject;
namespace game
{
    internal class MouseSelectionHandler : ExtMonoBeh
    {
        private readonly RaycastHit[] _hits = new RaycastHit[10];
        private Camera _camera;
        private Colony _colony;

        public WagonBuilding CurSelect { get; set; }
        public WagonBuilding UnderMouse { get; set; }
        public WorkerView WorkerCurSelect { get; set; }
        public WorkerView WorkerUnderMouse { get; set; }

        private void Update()
        {
            if (_camera == null || TouchCatcher.Current == null)
            {
                return;
            }

            if (UnderMouse != null)
            {
                UnderMouse.SetSelect(false);
                UnderMouse = null;
            }

            if (WorkerUnderMouse != null)
            {
                WorkerUnderMouse.SetSelect(false);
                WorkerUnderMouse = null;
            }

            if (CurSelect != null && !CurSelect.View.Visible)
            {
                CurSelect = null;
            }

            if (TouchCatcher.Current.IsOver || TouchCatcher.Current.IsDown)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                int hitCount = Physics.RaycastNonAlloc(ray, _hits, 100);
                if (hitCount > 0)
                {
                    WorkerUnderMouse = _colony.Team.GetIntersect(_hits, hitCount);
                    if (WorkerUnderMouse == null)
                    {
                        UnderMouse = _colony.Buildings.GetIntersect(_hits, hitCount);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && TouchCatcher.Current.IsDown)
            {
                if (CurSelect != null)
                {
                    CurSelect.SetSelect(false);
                }
                if (WorkerCurSelect != null)
                {
                    WorkerCurSelect.SetSelect(false);
                }
                CurSelect = UnderMouse;
                WorkerCurSelect = WorkerUnderMouse;
            }

            if (CurSelect != null && !CurSelect.View.Visible)
            {
                CurSelect = null;
            }

            if (CurSelect != null)
            {
                CurSelect.SetSelect(true);
            }
            if (WorkerCurSelect != null)
            {
                WorkerCurSelect.SetSelect(true);
            }
        }

        [Inject]
        public void Construct(Colony colony)
        {
            _colony = colony;
        }

        public void StartGame(Camera mainCamera)
        {
            _camera = mainCamera;
        }
    }
}
