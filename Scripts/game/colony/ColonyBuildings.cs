using System;
using System.Collections.Generic;

using game.wagon_buildings;

using loader;
using loader.database;

using UnityEngine;
namespace game.colony
{
    public class ColonyBuildings
    {

        private readonly List<WagonBuilding> _allBuildings = new List<WagonBuilding>();
        private readonly Dictionary<int, List<WagonBuilding>> _buildings;
        private readonly Colony _colony;
        private readonly DataLoader _dataLoader;
        private readonly List<WagonBuilding> _warehouses = new List<WagonBuilding>();

        public ColonyBuildings(Colony colony, DataLoader dataLoader)
        {
            _colony = colony;
            _dataLoader = dataLoader;

            _buildings = new Dictionary<int, List<WagonBuilding>>();
            foreach (WagonBuildingCat category in Enum.GetValues(typeof(WagonBuildingCat)))
            {
                _buildings[(int)category] = new List<WagonBuilding>();
            }
        }

        public void Setup()
        {
            LoadBuildings();
        }

        private async void LoadBuildings()
        {
            foreach (DbPlayerWagonBuilding playerWagonBuilding in DataStorage.Player.WagonBuildings)
            {
                DbWagonBuilding data = DataStorage.WagonBuildings[playerWagonBuilding.Building];
                if (data != null)
                {
                    WagonView wagon = _colony.Train.GetWagon(playerWagonBuilding.WagonIdx);
                    WagonFloorCell cell = wagon.GetCell(playerWagonBuilding.CellIdx);
                    if (cell != null)
                    {
                        var view = await _dataLoader.WagonBuildingStorage.GetAsync<WagonBuildingView>(data.Id, wagon.Self);
                        var building = new WagonBuilding(_dataLoader, view, data, playerWagonBuilding);
                        for (var r = 0; r < playerWagonBuilding.Angle; ++r)
                        {
                            building.MapPlacer.DoRotate(false);
                        }
                        building.MapPlacer.PlaceToFloorCell(cell, false);

                        AddBuilding(building);
                    }
                }
            }
        }

        public void AddBuilding(WagonBuilding building)
        {
            _allBuildings.Add(building);
            building.Constructing.Deconstructed += RemoveBuilding;

            _buildings[building.Data.Category].Add(building);
            if (building.Data.Id == (int)WagonBuildingId.Warehouse)
            {
                _warehouses.Add(building);
            }
        }

        private void RemoveBuilding(WagonBuilding building)
        {
            building.Constructing.Deconstructed -= RemoveBuilding;
            _allBuildings.Remove(building);
            _buildings[building.Data.Category].Remove(building);
            _warehouses.Remove(building);
        }

        public WagonBuilding GetBuilding(int uid)
        {
            for (var i = 0; i < _allBuildings.Count; ++i)
            {
                if (_allBuildings[i].PlayerData.Uid == uid)
                {
                    return _allBuildings[i];
                }
            }

            return null;
        }

        public WagonBuilding GetWarehouse()
        {
            return _warehouses[0];
        }

        public int GetStoredResources(int resId, WagonBuildingCat category = WagonBuildingCat.None)
        {
            var count = 0;
            List<WagonBuilding> buildings = category == WagonBuildingCat.None ? _warehouses : _buildings[(int)category];
            for (var i = 0; i < buildings.Count; ++i)
            {
                foreach (DbResVal resVal in buildings[i].PlayerData.Storage.Items)
                {
                    if (resVal.Id == resId)
                    {
                        count += resVal.Count;
                    }
                }
            }

            return count;
        }

        public WagonBuilding GetIntersect(RaycastHit[] hits, int hitCount)
        {
            for (var i = 0; i < _allBuildings.Count; ++i)
            {
                if (_allBuildings[i].IsIntersect(hits, hitCount))
                {
                    return _allBuildings[i];
                }
            }
            return null;
        }
    }
}
