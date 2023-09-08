using System.Collections.Generic;

using loader.database;

using misc.components.storages;

using UnityEngine;
namespace game.wagon_buildings.funcs
{
    internal class WarehouseFunc : IWagonBuildingFunc
    {
        private readonly UniStorage _boxStorage;
        private readonly DbPlayerWagonBuilding _playerData;

        private readonly List<BoxResView> _pool = new List<BoxResView>();
        private readonly WarehouseFuncView _view;

        public WarehouseFunc(WarehouseFuncView view, DbPlayerWagonBuilding data, UniStorage boxStorage)
        {
            _view = view;
            _playerData = data;
            _boxStorage = boxStorage;

            _view.OnViewEnabled += OnViewEnabled;
            if (_view.Visible)
            {
                OnViewEnabled();
            }
            _view.OnViewDisabled += OnViewDisabled;

            SetupView(data.Storage.Items, boxStorage);
        }

        public int StorageLimit => _view.Limits.x * _view.Limits.y * _view.Limits.z;

        private void OnViewEnabled()
        {
            _playerData.OnStorageChanged += PlayerDataOnOnStorageChanged;
        }

        private void OnViewDisabled()
        {
            _playerData.OnStorageChanged -= PlayerDataOnOnStorageChanged;
        }

        private void PlayerDataOnOnStorageChanged(DbPlayerWagonBuilding playerData)
        {
            SetupView(playerData.Storage.Items, _boxStorage);
        }

        private void SetupView(IReadOnlyList<DbResVal> data, UniStorage boxStorage)
        {
            for (var i = 0; i < _pool.Count; ++i)
            {
                _pool[i].Visible = false;
            }
            _pool.Clear();

            int layerCount = _view.Limits.x * _view.Limits.z;
            for (var i = 0; i < data.Count; ++i)
            {
                int boxCount = DbWarehouse.BoxCount(data[i].Count, data[i].Params.StackSize);
                for (var j = 0; j < boxCount; ++j)
                {
                    var box = boxStorage.Get<BoxResView>(data[i].Params.Id, _view.Self);
                    int idx = _pool.Count;
                    int y = idx / layerCount;
                    int z = idx % layerCount / _view.Limits.x;
                    int x = idx % layerCount % _view.Limits.x;
                    box.Self.localPosition = _view.BoxPosCorner
                        + new Vector3(x * _view.BoxPosOffset.x, y * _view.BoxPosOffset.y, z * _view.BoxPosOffset.z);
                    _pool.Add(box);
                }
            }
        }
    }
}
