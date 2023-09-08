using game.colony.works.funcs;

using loader;
using loader.database;

using misc.tweens;

using UnityEngine;
using UnityEngine.Events;
namespace game.wagon_buildings
{
    public sealed class WagonBuildingConstructing
    {
        private readonly WagonBuilding _building;
        private readonly GameObject[] _buildStages;
        private readonly DataLoader _dataLoader;

        private BuildingPlace _constructPlace;
        private BuildingPlace _deconstructPlace;

        private IWorkFunc _work;

        public UnityAction<WagonBuilding> Deconstructed = delegate {};

        public WagonBuildingConstructing(DataLoader dataLoader, WagonBuilding building, GameObject[] buildStages)
        {
            _dataLoader = dataLoader;
            _building = building;
            _buildStages = buildStages;
        }

        public bool IsConstruct => _constructPlace != null;
        public bool IsDeconstruct => _deconstructPlace != null;
        public bool WaitConstructing => IsConstruct || IsDeconstruct;

        public bool CanWorkCanceled => _work != null && !_work.IsCompleted;

        public int BuildingStages => _buildStages?.Length ?? 0;

        public void AssignWork(IWorkFunc work)
        {
            _work = work;
        }

        public void FinalDeconstruct()
        {
            if (_constructPlace != null)
            {
                _constructPlace.View.Visible = false;
                _constructPlace = null;
            }
            if (_deconstructPlace != null)
            {
                _deconstructPlace.View.Visible = false;
                _deconstructPlace = null;
            }

            _building.MapPlacer.SetNavigationCarveEnabled(false);
            _building.MapPlacer.ClearMap();

            ResetBuildingStages();

            _building.View.Visible = false;
            Deconstructed(_building);
        }

        public void SetBuildingStage(int stage)
        {
            for (var i = 0; _buildStages != null && i < _buildStages.Length; ++i)
            {
                if (_buildStages[i] != null)
                {
                    bool prevState = _buildStages[i].activeSelf;
                    _buildStages[i].SetActive(i < stage);
                    if (i == stage - 1 && !prevState)
                    {
                        Transform tr = _buildStages[i].transform;
                        tr.localScale = new Vector3(1, 0.05f, 1);
                        new TweenScale(0.5f, tr, 1).DoAlive();
                    }
                }
            }
        }

        public async void StartConstruct()
        {
            _building.MapPlacer.PlaceToFloorCell(_building.MapPlacer.Cell, false);
            if (BuildingStages > 0)
            {
                SetBuildingStage(0);
            }

            var view = await _building.WagonBuildingStorage.GetAsync<BuildingPlaceView>((int)WagonBuildingId.BuildingPlace,
                _building.View.Self.parent);
            _constructPlace = new BuildingPlace(_dataLoader, view, _building.Data, _building.PlayerData);

            _constructPlace.View.Self.position = _building.MapPlacer.CornerCell.Self.position;
            _constructPlace.StartBuild(_building);
            _building.PlayerData.LeftToBuildPercents = 100;
        }

        public void FinalCostruct()
        {
            _building.PlayerData.LeftToBuildPercents = 0;
            ResetBuildingStages();
            if (_constructPlace != null)
            {
                _constructPlace.View.Visible = false;
                _constructPlace = null;
            }
            if (_deconstructPlace != null)
            {
                _deconstructPlace.View.Visible = false;
                _deconstructPlace = null;
            }
        }

        private void ResetBuildingStages()
        {
            if (BuildingStages > 0)
            {
                SetBuildingStage(BuildingStages);
            }
        }

        public async void StartDeconstruct()
        {
            var view = await _building.WagonBuildingStorage.GetAsync<BuildingPlaceView>((int)WagonBuildingId.BuildingPlace,
                _building.View.Self.parent);
            _deconstructPlace = new BuildingPlace(_dataLoader, view, null, _building.PlayerData);
            _deconstructPlace.View.Self.position = _building.MapPlacer.CornerCell.Self.position;
            _deconstructPlace.StartBuild(_building);
        }

        public void SwitchConstructDeconstruct()
        {
            if (_constructPlace != null)
            {
                _deconstructPlace = _constructPlace;
                _constructPlace = null;
            }
            else
            {
                _constructPlace = _deconstructPlace;
                _deconstructPlace = null;
            }
        }

        public bool IsIntersect(RaycastHit[] hits, int count)
        {
            if (_constructPlace != null)
            {
                return _constructPlace.IsIntersect(hits, count);
            }
            if (_deconstructPlace != null)
            {
                return _deconstructPlace.IsIntersect(hits, count);
            }

            return false;
        }

        public InteractPivotView GetNearestInteractPivot(Vector3 from, InteractPivotType type)
        {
            if (_constructPlace != null)
            {
                return _constructPlace.GetNearestInteractPivot(from, type);
            }
            if (_deconstructPlace != null)
            {
                return _deconstructPlace.GetNearestInteractPivot(from, type);
            }

            return null;
        }

        public void UnlockPivots(WorkerView worker)
        {
            _constructPlace?.UnlockPivots(worker);
            _deconstructPlace?.UnlockPivots(worker);
        }
    }
}
