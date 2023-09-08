using gui.craft;

using loader;
using loader.database;

using misc;

using TMPro;

using UnityEngine;
namespace gui.review
{
    internal class ResItem : ExtMonoBeh
    {
        [SerializeField] private ResourceIconPlacer _icon;
        [SerializeField] private TextMeshProUGUI _val;
        private int _count;

        private DbResVal _data;
        private int _pCount;
        private DbResVal _pData;
        private int _reserved;

        private void Update()
        {
            if (_val == null || _data == null)
            {
                return;
            }

            if (_pData.Count != _pCount || _data.Count != _count || _data.Reserved != _reserved)
            {
                UpdateVal();
            }
        }

        public void Setup(int resId, DbPlayerWagonBuilding building = null)
        {
            _data = building != null ? DbWarehouse.GetRes(building.Storage.Items, resId) : null;
            if (_data == null)
            {
                _count = 0;
                _reserved = 0;
            }

            _pData = DbWarehouse.GetRes(DataStorage.Player.Resources, resId);

            _icon.Setup(DataStorage.Resources[resId]);

            UpdateVal();
        }

        public void Setup(DbResVal ing, DbPlayerWagonBuilding building = null)
        {
            _data = building != null ? DbWarehouse.GetRes(building.Storage.Items, ing.Id) : null;
            if (_data == null)
            {
                _count = 0;
                _reserved = 0;
            }

            _pData = ing;

            _icon.Setup(DataStorage.Resources[ing.Id]);

            UpdateVal();
        }

        private void UpdateVal()
        {
            if (_val != null && _data != null)
            {
                _count = _data.Count;
                _reserved = _data.Reserved;
                _pCount = _pData.Count;

                _val.text = Helper.PercentToString(_count) + "/" + Helper.PercentToString(_pCount);
            }
        }
    }
}
