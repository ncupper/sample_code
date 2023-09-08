using System.Collections.Generic;

using gui.review;

using loader;
using loader.database;

using misc;

using TMPro;

using UnityEngine;

using Zenject;
namespace gui.tool_tips
{
    public class BuildingTooltipView : TooltipView
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _desc;
        [SerializeField] private GameObject _productsCaption;
        [SerializeField] private ResItem _sample;
        [SerializeField] private GameObject _productsGrid;

        private readonly List<ResItem> _pool = new List<ResItem>();
        private Helper.Factory _factory;

        [Inject]
        public void Construct(Helper.Factory factory)
        {
            _factory = factory;
        }

        protected override void OnStart()
        {
            base.OnStart();
            _sample.Visible = false;
        }

        public override void Setup(TooltipPlace place)
        {
            DbWagonBuilding buildingData = DataStorage.WagonBuildings[place.GetComponent<IWagonBuildingHolder>().WagonBuildingId];

            UpdateView(buildingData);
        }

        private void UpdateView(DbWagonBuilding buildingData, bool needRefreshing = true)
        {
            _name.text = Lang.Get(buildingData.Name);
            _desc.text = Lang.Get(buildingData.Desc);
            _productsCaption.SetActive(buildingData.HaveCraft);
            _productsGrid.SetActive(buildingData.HaveCraft);
            if (buildingData.HaveCraft)
            {
                Helper.ResizePool(_factory, _pool, _sample, buildingData.Craft.Length);
                for (var i = 0; i < buildingData.Craft.Length; ++i)
                {
                    _pool[i].Setup(buildingData.Craft[i]);
                }
            }
        }
    }
}
