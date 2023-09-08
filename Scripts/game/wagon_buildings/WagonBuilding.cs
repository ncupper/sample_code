using System;
using System.Collections.Generic;

using game.wagon_buildings.funcs;

using loader;
using loader.database;

using misc;
using misc.components.storages;

using UnityEngine;
namespace game.wagon_buildings
{
    public class WagonBuilding
    {

        private readonly List<IWagonBuildingFunc> _funcs = new List<IWagonBuildingFunc>();
        private readonly WagonBuildingView _view;
        public readonly UniStorage BoxStorage;

        public readonly UniAsyncStorage WagonBuildingStorage;

        public WagonBuilding(DataLoader dataLoader, WagonBuildingView view, DbWagonBuilding data, DbPlayerWagonBuilding playerData)
        {
            WagonBuildingStorage = dataLoader.WagonBuildingStorage;
            BoxStorage = dataLoader.BoxStorage;

            _view = view;
            Data = data;
            PlayerData = playerData;

            MapPlacer = new WagonBuildingMapPlacer(_view.gameObject, this);
            Constructing = new WagonBuildingConstructing(dataLoader, this, _view.BuildStages);

            if (playerData != null)
            {
                var warehouseView = _view.gameObject.GetComponentInChildren<WarehouseFuncView>();
                if (warehouseView != null)
                {
                    var warehouseFunc = new WarehouseFunc(warehouseView, playerData, BoxStorage);
                    _funcs.Add(warehouseFunc);
                    StorageLimit = warehouseFunc.StorageLimit;
                }
            }

            SwitchOnOff(_view.IsOn);
            SetSelect(false);
        }
        public ExtMonoBeh View => _view;
        public DbWagonBuilding Data { get; set; }
        public DbPlayerWagonBuilding PlayerData { get; set; }
        public WagonBuildingMapPlacer MapPlacer { get; }
        public WagonBuildingConstructing Constructing { get; }

        public int StorageLimit { get; }

        public bool IsIntersect(RaycastHit[] hits, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                if (Array.IndexOf(_view.Colliders, hits[i].collider) != -1)
                {
                    return true;
                }
            }

            return Constructing.IsIntersect(hits, count);
        }

        public void SetSelect(bool val)
        {
            if (_view.Outline != null)
            {
                _view.Outline.enabled = val;
            }
        }

        public InteractPivotView GetNearestInteractPivot(Vector3 from, InteractPivotType type)
        {
            InteractPivotView result = Constructing.GetNearestInteractPivot(from, type);
            if (result == null)
            {
                for (var i = 0; i < _view.InteractPivots.Length; ++i)
                {
                    if (_view.InteractPivots[i] == null || (int)_view.InteractPivots[i].Type != (int)type
                        || _view.InteractPivots[i].Locked != null)
                    {
                        continue;
                    }
                    if (result == null ||
                        (result.Self.position - from).sqrMagnitude > (_view.InteractPivots[i].Self.position - from).sqrMagnitude)
                    {
                        result = _view.InteractPivots[i];
                    }
                }
            }

            return result;
        }

        public void UnlockPivots(WorkerView worker)
        {
            Constructing.UnlockPivots(worker);

            for (var i = 0; i < _view.InteractPivots.Length; ++i)
            {
                if (_view.InteractPivots[i] != null && _view.InteractPivots[i].Locked == worker)
                {
                    _view.InteractPivots[i].Locked = null;
                }
            }
        }

        public void SwitchOnOff(bool isOn)
        {
            for (var i = 0; _view.OnObjects != null && i < _view.OnObjects.Length; ++i)
            {
                if (_view.OnObjects[i] != null)
                {
                    _view.OnObjects[i].SetActive(isOn);
                }
            }
            for (var i = 0; _view.OnAnims != null && i < _view.OnAnims.Length; ++i)
            {
                if (_view.OnAnims[i] != null)
                {
                    if (isOn)
                    {
                        _view.OnAnims[i].Play();
                    }
                    else
                    {
                        _view.OnAnims[i].Stop();
                    }
                }
            }
        }

        public void ShowPivots(bool vis)
        {
            for (var i = 0; i < _view.InteractPivots.Length; ++i)
            {
                if (_view.InteractPivots[i] != null)
                {
                    _view.InteractPivots[i].SwitchDebugViewVisible(vis);
                }
            }
        }
    }
}
