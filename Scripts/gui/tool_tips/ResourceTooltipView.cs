using game.colony;

using loader;
using loader.database;

using misc;
using misc.components;

using TMPro;

using UnityEngine;

using Zenject;
namespace gui.tool_tips
{
    [RequireComponent(typeof(AutoUpdater))]
    public class ResourceTooltipView : TooltipView
    {
        private const string TooltipLangKeyStored = "tt_ResourceItemStored";
        private const string TooltipLangKeyWorkbenched = "tt_ResourceItemWorkbenched";
        private const string TooltipLangKeyCarried = "tt_ResourceItemCarried";

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _stored;
        [SerializeField] private TextMeshProUGUI _workbenched;
        [SerializeField] private TextMeshProUGUI _carried;

        private Colony _colony;
        private IResourceHolder _resHolder;
        private AutoUpdater _updater;

        [Inject]
        public void Construct(Colony colony)
        {
            _colony = colony;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _updater = GetComponent<AutoUpdater>();
            _updater.OnUpdate += UpdateData;
        }

        public override void Setup(TooltipPlace place)
        {
            _resHolder = place.GetComponent<IResourceHolder>();
            UpdateData();
        }

        private void UpdateData()
        {
            DbResource resData = DataStorage.Resources[_resHolder.ResId];
            _name.text = Lang.Get(resData.Name);

            int stored = _colony.Buildings.GetStoredResources(_resHolder.ResId);
            int workbenched = _colony.Buildings.GetStoredResources(_resHolder.ResId, WagonBuildingCat.Workbench);
            int carried = _colony.Works.GetCarriedResources(_resHolder.ResId);

            _stored.text = Lang.Get(TooltipLangKeyStored, Helper.PercentToString(stored));
            _workbenched.text = Lang.Get(TooltipLangKeyWorkbenched, Helper.PercentToString(workbenched));
            _carried.text = Lang.Get(TooltipLangKeyCarried, Helper.PercentToString(carried));
        }
    }
}
