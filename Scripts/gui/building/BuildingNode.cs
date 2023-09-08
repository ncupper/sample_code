using System.Collections.Generic;

using game.colony;
using game.colony.works;
using game.wagon_buildings;

using loader;
using loader.database;

using misc;
using misc.components;

using UnityEngine;

using Zenject;
namespace gui.building
{
    internal class BuildingNode : ExtMonoBeh
    {
        [SerializeField] private GameObject _list;
        [SerializeField] private BuildingItem _sample;

        [SerializeField] private ExToggle[] _categories;
        [SerializeField] private ExToggleGroup _buildingsGroup;

        private readonly List<BuildingItem> _pool = new List<BuildingItem>();
        private Colony _colony;
        private WagonBuildingDragger _dragger;
        private Helper.Factory _factory;

        [Inject]
        public void Construct(Helper.Factory factory, WagonBuildingDragger dragger, Colony colony)
        {
            _factory = factory;
            _dragger = dragger;
            _colony = colony;
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            for (var i = 0; i < _categories.Length; ++i)
            {
                _categories[i].IsOn = false;
                _categories[i].OnChange += OnChangeSelection;
            }

            _list.SetActive(false);
            _buildingsGroup.OnChange += OnSelectBuilding;
        }

        private void OnChangeSelection(ExToggle cat)
        {
            int catIdx = -1;
            for (var i = 0; i < _categories.Length; ++i)
            {
                if (_categories[i].IsOn)
                {
                    catIdx = i;
                }
            }

            _list.SetActive(catIdx != -1);
            if (catIdx != -1)
            {
                RefreshList((WagonBuildingCat)(catIdx + 1));
            }
        }

        private void RefreshList(WagonBuildingCat cat)
        {
            DbWagonBuilding[] buildings = DataStorage.WagonBuildings.Items;
            var count = 0;
            for (var i = 0; i < buildings.Length; ++i)
            {
                if (buildings[i].Category == (int)cat)
                {
                    ++count;
                }
            }

            Helper.ResizePool(_factory, _pool, _sample, count);
            var idx = 0;
            for (var i = 0; i < buildings.Length; ++i)
            {
                if (buildings[i].Category == (int)cat)
                {
                    _pool[idx].Setup(buildings[i]);
                    ++idx;
                }
            }

            _buildingsGroup.ReInit();
        }

        private void OnSelectBuilding(ExtMonoBeh item)
        {
            var building = item.GetComponent<BuildingItem>();
            _dragger.SetDrag(building.IsOn ? building.Data : null, OnFinishDrag);
        }

        private void OnFinishDrag(WagonBuilding buildingView, bool isCanceled)
        {
            UnselectBuilding();
            if (!isCanceled)
            {
                Debug.Log("place [" + buildingView.View.name + "] to cell [" + buildingView.MapPlacer.CellIdx + "]");

                buildingView.View.Visible = true;
                buildingView.MapPlacer.PlaceToFloorCell(_colony.Train.GetWagon(1).GetCell(buildingView.MapPlacer.CellIdx), false);
                DbPlayerWagonBuilding pdata =
                    DataStorage.Player.AddBuilding(buildingView.Data.Id, 0, buildingView.MapPlacer.CellIdx,
                        buildingView.MapPlacer.Angle);
                buildingView.PlayerData = pdata;
                _colony.Buildings.AddBuilding(buildingView);

                buildingView.Constructing.StartConstruct();

                var cTask = new ConstructWork(_colony);
                cTask.Setup(buildingView);
                _colony.Works.Add(cTask);
            }
        }

        private void UnselectBuilding()
        {
            _buildingsGroup.TurnAllOff();
        }
    }
}
