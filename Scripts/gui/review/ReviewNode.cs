using System.Collections.Generic;

using game;
using game.wagon_buildings;

using loader.database;

using misc;
using misc.components;

using TMPro;

using UnityEngine;

using Zenject;
namespace gui.review
{
    internal class ReviewNode : GuiScreen
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _desc;
        [SerializeField] private ResItem _sampleRes;

        private readonly List<ResItem> _pool = new List<ResItem>();

        private WagonBuilding _building;
        private Helper.Factory _factory;
        private bool _isBuildMode;
        private MouseSelectionHandler _selection;

        private WagonBuilding Building
        {
            get => _building;
            set
            {
                if (_building != null)
                {
                    _building.PlayerData.OnStorageChanged -= OnBuildingStorageChanged;
                }
                _building = value;
                if (_building != null)
                {
                    _building.PlayerData.OnStorageChanged += OnBuildingStorageChanged;
                }
            }
        }

        private void Update()
        {
            if (Building != _selection.CurSelect)
            {
                if (_selection.CurSelect == null)
                {
                    Building = null;
                    DelayedHideNode();
                }
                else
                {
                    Building = _selection.CurSelect;
                    Setup();
                }
            }

            if (Building != null && _isBuildMode && Building.PlayerData.LeftToBuildPercents <= 0)
            {
                Setup();
            }
        }

        [Inject]
        public void Construct(MouseSelectionHandler selection, Helper.Factory factory)
        {
            _selection = selection;
            _factory = factory;
        }

        public override void OnHide()
        {
            base.OnHide();
            Building = null;
        }

        protected override void OnShow()
        {
            base.OnShow();

            Building = _selection.CurSelect;
            Setup();
        }

        private void Setup()
        {
            _isBuildMode = Building.PlayerData.LeftToBuildPercents > 0;

            _name.text = Lang.Get(Building.Data.Name);
            _desc.text = Lang.Get(Building.Data.Desc);

            DbResStorage storage = Building.PlayerData.Storage;
            storage.RemoveZero();

            if (_isBuildMode)
            {
                Helper.ResizePool(_factory, _pool, _sampleRes, Building.Data.Ings.Length);
                for (var i = 0; i < Building.Data.Ings.Length; ++i)
                {
                    _pool[i].Setup(Building.Data.Ings[i], Building.PlayerData);
                }
            }
            else
            {
                Helper.ResizePool(_factory, _pool, _sampleRes, storage.Items.Count);
                for (var i = 0; i < storage.Items.Count; ++i)
                {
                    _pool[i].Setup(storage.Items[i].Id, Building.PlayerData);
                }
            }
        }

        private void OnBuildingStorageChanged(DbPlayerWagonBuilding model)
        {
            Setup();
        }

        protected override void OnBtnClick(string btn, ClickAction action)
        {
            if (btn == "Close Button")
            {
                ((GameWnd)Parent).SelectNode.TurnOffReview();
            }
            base.OnBtnClick(btn, action);
        }
    }
}
