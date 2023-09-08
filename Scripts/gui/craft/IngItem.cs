using loader;
using loader.database;

using misc;

using TMPro;

using UnityEngine;
namespace gui.craft
{
    internal class IngItem : ExtMonoBeh
    {
        [SerializeField] private ResourceIconPlacer _icon;
        [SerializeField] private TextMeshProUGUI _value;

        private DbResVal _data;

        private int _pCount;
        private DbResVal _pData;

        private void Update()
        {
            if (_pData.Count != _pCount)
            {
                UpdateVal();
            }
        }

        public void Setup(DbResVal data)
        {
            _data = data;
            _pData = DbWarehouse.GetRes(DataStorage.Player.Resources, _data.Id);

            _icon.Setup(_data.Params);
            UpdateVal();
        }

        private void UpdateVal()
        {
            _pCount = _pData.Count;
            _value.text = Helper.PercentToString(_data.Count) + "/" + Helper.PercentToString(_pCount);
        }
    }
}
