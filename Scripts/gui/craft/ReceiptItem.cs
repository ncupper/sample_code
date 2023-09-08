using System.Collections.Generic;

using loader.database;

using misc;

using TMPro;

using UnityEngine;
namespace gui.craft
{
    internal class ReceiptItem : ExtMonoBeh
    {
        [SerializeField] private ResourceIconPlacer _resultIcon;
        [SerializeField] private TextMeshProUGUI _resultValue;
        [SerializeField] private IngItem _ingSample;

        private readonly List<IngItem> _pool = new List<IngItem>();

        private DbResource _data;

        public void Setup(Helper.Factory factory, DbResource data)
        {
            _data = data;
            Helper.ResizePool(factory, _pool, _ingSample, _data.Craft.Ings.Length);

            for (var i = 0; i < _data.Craft.Ings.Length; ++i)
            {
                _pool[i].Setup(_data.Craft.Ings[i]);
            }

            _resultIcon.Setup(_data);
            _resultValue.text = Helper.PercentToString(_data.Craft.Count);
        }
    }
}
