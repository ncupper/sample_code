using System.Collections.Generic;
using System.Linq;

using game;
using game.colony;
using game.wagon_buildings;

using loader;
using loader.database;

using misc;
using misc.components;

using TMPro;

using UnityEngine;

using Zenject;
namespace gui.craft
{
    internal class CraftNode : GuiScreen
    {
        [SerializeField] private CraftItem _sampleCraft;
        [SerializeField] private ExToggleGroup _craftGroup;
        [SerializeField] private QueueCraftItem _sampleQueue;
        [Space(10), SerializeField]
         private ResourceIconPlacer _selIcon;
        [SerializeField] private TextMeshProUGUI _selName;
        [SerializeField] private TextMeshProUGUI _selDesc;
        [SerializeField] private ReceiptItem _selReceipt;
        [Space(10), SerializeField]
         private TextMeshProUGUI _makeCondValue;

        private readonly Dictionary<int, int> _buildingCraftItemSelect = new Dictionary<int, int>();

        private readonly List<CraftItem> _craftPool = new List<CraftItem>();
        private readonly List<QueueCraftItem> _queuePool = new List<QueueCraftItem>();

        private WagonBuilding _building;
        private Colony _colony;
        private Helper.Factory _factory;
        private MouseSelectionHandler _selection;
        private DbResource _selRecData;

        private WagonBuilding Building
        {
            get => _building;
            set
            {
                if (_building != null)
                {
                    _building.PlayerData.OnQueueChanged -= OnBuildingCraftQueueChanged;
                }
                _building = value;
                if (_building != null)
                {
                    _building.PlayerData.OnQueueChanged += OnBuildingCraftQueueChanged;
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
                    if (!Building.Data.HaveCraft)
                    {
                        DelayedHideNode();
                    }
                    else
                    {
                        Setup();
                    }
                }
            }
        }

        [Inject]
        public void Construct(MouseSelectionHandler selection, Colony colony, Helper.Factory factory)
        {
            _selection = selection;
            _colony = colony;
            _factory = factory;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _craftGroup.OnChange += x =>
            {
                var item = x.GetComponent<CraftItem>();
                _buildingCraftItemSelect[Building.Data.Id] = _craftPool.IndexOf(item);
                SetupReceipt(item);
            };
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
            Helper.ResizePool(_factory, _craftPool, _sampleCraft, Building.Data.Craft.Length);

            var selIdx = 0;
            if (_buildingCraftItemSelect.ContainsKey(Building.Data.Id) && _buildingCraftItemSelect[Building.Data.Id] >= 0)
            {
                selIdx = _buildingCraftItemSelect[Building.Data.Id];
            }
            else
            {
                _buildingCraftItemSelect.Add(Building.Data.Id, 0);
            }

            for (var i = 0; i < Building.Data.Craft.Length; ++i)
            {
                DbResource res = DataStorage.Resources[Building.Data.Craft[i]];
                _craftPool[i].Setup(res);
                _craftPool[i].IsOn = selIdx == i;
            }

            SetupReceipt(_craftPool[selIdx]);

            SetupQueue();
        }

        private void SetupQueue()
        {
            if (!Visible)
            {
                return;
            }
            Helper.ResizePool(_factory, _queuePool, _sampleQueue, Building.PlayerData.Queue.Items.Count, x =>
            {
                x.OnClickItem += OnSelectQueueItem;
                x.OnRightClickItem += OnDeleteQueueItem;
            });
            for (var i = 0; i < Building.PlayerData.Queue.Items.Count; ++i)
            {
                DbCraftWork qItem = Building.PlayerData.Queue.Items[i];
                _queuePool[i].Setup(qItem);
            }

            SetupUntilButton();
        }

        private void OnSelectQueueItem(DbCraftWork work)
        {
            for (var i = 0; i < Building.Data.Craft.Length; ++i)
            {
                if (_craftPool[i].Data.Id == work.ResId)
                {
                    _craftPool[i].IsOn = true;
                    break;
                }
            }
        }

        private void OnDeleteQueueItem(DbCraftWork work)
        {
            _colony.Works.RemoveCraft(work);
            Building.PlayerData.Queue.Remove(work);
            SetupQueue();
        }

        private void SetupReceipt(CraftItem item)
        {
            _selRecData = item.Data;
            _selIcon.Setup(_selRecData);
            _selName.text = Lang.Get(_selRecData.Name);
            _selDesc.text = Lang.Get(_selRecData.Desc);

            _selReceipt.Setup(_factory, _selRecData);

            SetupUntilButton();
        }

        private void SetupUntilButton()
        {
            DbCraftWork task = Building.PlayerData.Queue.Items.FirstOrDefault(
                x => x.ResId == _selRecData.Id && x.Condition == CraftCondition.Until);
            if (task == null)
            {
                _makeCondValue.text = "<10";
            }
            else
            {
                _makeCondValue.text = "<" + Helper.PercentToString(task.Count + 1000);
            }
        }

        public void ClickMakeOne()
        {
            var qItem = new DbCraftWork
            {
                ResId = _selRecData.Id, Count = 100, Condition = CraftCondition.None
            };
            Building.PlayerData.AddTask(_colony, qItem);
            SetupQueue();
        }

        public void ClickMakeFive()
        {
            var qItem = new DbCraftWork
            {
                ResId = _selRecData.Id, Count = 500, Condition = CraftCondition.None
            };
            Building.PlayerData.AddTask(_colony, qItem);
            SetupQueue();
        }

        public void ClickInfinity()
        {
            var qItem = new DbCraftWork
            {
                ResId = _selRecData.Id, Count = 0, Condition = CraftCondition.Infinity
            };
            Building.PlayerData.AddTask(_colony, qItem);
            SetupQueue();
        }

        public void ClickCondition()
        {
            var qItem = new DbCraftWork
            {
                ResId = _selRecData.Id, Count = Helper.Parse(_makeCondValue.text) * 100, Condition = CraftCondition.Until
            };
            Building.PlayerData.AddTask(_colony, qItem);
            SetupQueue();
        }

        protected override void OnBtnClick(string btn, ClickAction action)
        {
            if (btn == "Close Button")
            {
                ((GameWnd)Parent).SelectNode.TurnOffCraft();
            }
            base.OnBtnClick(btn, action);
        }

        private void OnBuildingCraftQueueChanged(DbPlayerWagonBuilding model)
        {
            SetupQueue();
        }
    }
}
