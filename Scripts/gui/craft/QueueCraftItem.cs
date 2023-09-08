using gui.tool_tips;

using loader;
using loader.database;

using misc;

using Ricimi;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
namespace gui.craft
{
    internal class QueueCraftItem : CraftItem, IResourceHolder
    {
        [SerializeField] private TextMeshProUGUI _task;

        private DbCraftWork _data;

        public UnityAction<DbCraftWork> OnClickItem = delegate {};
        public UnityAction<DbCraftWork> OnRightClickItem = delegate {};

        public int ResId => _data.ResId;

        protected override void OnAwake()
        {
            base.OnAwake();

            GetComponent<CleanButton>().onClick.AddListener(() => { OnClickItem(_data); });
        }

        public void Setup(DbCraftWork data)
        {
            _data = data;
            base.Setup(DataStorage.Resources[data.ResId]);
            if ((int)_data.Condition == (int)CraftCondition.None)
            {
                _task.text = "x" + Helper.PercentToString(data.Count);
            }
            else if ((int)_data.Condition == (int)CraftCondition.Until)
            {
                _task.text = "<" + Helper.PercentToString(data.Count);
            }
            else
            {
                _task.text = "∞";
            }
        }

        public void RightMouseButtonClick()
        {
            OnRightClickItem(_data);
        }
    }
}
