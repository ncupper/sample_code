using System;

using game.colony;

using loader;
using loader.database;

using misc;
using misc.components;
using misc.components.storages;

using UnityEngine;

using Zenject;
namespace game.wagon_buildings
{
    public class WagonBuildingDragger : ExtMonoBeh
    {
        private Colony _colony;
        private DataLoader _dataLoader;
        private WagonBuilding _drag;
        private UniAsyncStorage _wagonStorage;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                FinishDrag(true);
            }

            if (_drag != null)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    _drag.MapPlacer.DoRotate(true);
                }
                WagonFloorCell cell = Train.Instance.Wagon.GetMouseFloorCell();
                if (cell != null)
                {
                    _drag.MapPlacer.PlaceToFloorCell(cell, true);
                }

                if (Input.GetMouseButtonDown(0) && TouchCatcher.Current.IsDown && _drag.MapPlacer.CanPlace())
                {
                    FinishDrag(false);
                }
            }
        }

        private event Action<WagonBuilding, bool> DragFinished;

        [Inject]
        public void Construct(DataLoader dataLoader, Colony colony)
        {
            _dataLoader = dataLoader;
            _wagonStorage = _dataLoader.WagonBuildingStorage;
            _colony = colony;
        }

        public async void SetDrag(DbWagonBuilding buildingData, Action<WagonBuilding, bool> dragFinished)
        {
            FinishDrag(true);
            if (buildingData == null)
            {
                return;
            }

            DragFinished = dragFinished;
            WagonView wagon = _colony.Train.GetWagon(1);
            var dragView = await _wagonStorage.GetAsync<WagonBuildingView>(buildingData.Id, wagon.Self);
            _drag = new WagonBuilding(_dataLoader, dragView, buildingData, null);
            _drag.MapPlacer.BeginDrag();
            Train.Instance.SetFloorCellsVis(true);
        }

        private void FinishDrag(bool isCanceled)
        {
            if (_drag != null)
            {
                _drag.MapPlacer.EndDrag();
                _drag.View.Visible = false;
                DragFinished?.Invoke(_drag, isCanceled);
                _drag = null;
            }
            Train.Instance.SetFloorCellsVis(false);
        }
    }
}
