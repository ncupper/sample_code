using game;
using game.colony;
using game.colony.works;
using game.wagon_buildings;

using loader;

using misc;
using misc.components;
using misc.managers;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenject;
namespace gui.selection
{
    internal class SelectNode : GuiScreen
    {
        [SerializeField] private RawImage _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _condition;
        [Space(10), SerializeField]
         private ExToggle _craft;
        [SerializeField] private ExToggle _review;
        [SerializeField] private ExToggle _tasks;

        private WagonBuilding _building;
        private Colony _colony;
        private WagonBuildingDragger _dragger;
        private MouseSelectionHandler _selection;
        private bool _waitConstruct;

        private void Update()
        {
            if (_building != _selection.CurSelect)
            {
                if (_selection.CurSelect == null)
                {
                    _building = null;
                    DelayedHideNode();
                }
                else
                {
                    _building = _selection.CurSelect;
                    Setup();
                    AudioManager.Instance.PlaySound("popup_open");
                }
            }
            if (_building != null && _waitConstruct != _building.Constructing.WaitConstructing)
            {
                Setup();
            }

            UpdateCondition();
        }

        [Inject]
        public void Construct(DataLoader dataLoader, MouseSelectionHandler selection, Colony colony, WagonBuildingDragger dragger)
        {
            _selection = selection;
            _colony = colony;
            _dragger = dragger;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _craft.IsOn = false;
            _review.IsOn = false;
            _tasks.IsOn = false;

            _craft.OnChange += x =>
            {
                ((GameWnd)Parent).ToggleCraft(x.IsOn);
            };
            _review.OnChange += x =>
            {
                ((GameWnd)Parent).ToggleReview(x.IsOn);
            };
        }

        protected override void OnShow()
        {
            base.OnShow();

            _building = _selection.CurSelect;
            Setup();
        }

        private void Setup()
        {
            _waitConstruct = _building.Constructing.WaitConstructing;

            _icon.texture = _building.Data.Texture;
            _name.text = Lang.Get(_building.Data.Name);

            _craft.Visible = _building.Data.HaveCraft && !_waitConstruct;
            if (_craft.Visible && _craft.IsOn && !_review.IsOn)
            {
                ((GameWnd)Parent).ToggleCraft(true);
            }
            if (_review.IsOn)
            {
                _craft.IsOn = false;
                ((GameWnd)Parent).ToggleReview(true);
            }
        }

        private void UpdateCondition()
        {
            if (_building != null)
            {
                if (_building.Constructing.IsConstruct || _building.Constructing.IsDeconstruct)
                {
                    if (_building.Constructing.IsConstruct)
                    {
                        _condition.text = 100 - _building.PlayerData.LeftToBuildPercents + "%";
                    }
                    else
                    {
                        _condition.text = _building.PlayerData.LeftToBuildPercents + "%";
                    }
                }
                else
                {
                    _condition.text = _building.PlayerData.Condition + "%";
                    if (_waitConstruct)
                    {
                        Setup();
                    }
                }
            }
        }

        public void TurnOffCraft()
        {
            _craft.IsOn = false;
        }

        public void TurnOffReview()
        {
            _review.IsOn = false;
        }

        protected override void OnBtnClick(string btn, ClickAction action)
        {
            if (btn == "ButtonCancel")
            {
                _colony.Works.CancelConstruct(_building);
            }
            else if (btn == "ButtonTrash")
            {
                _colony.Works.CancelAllFor(_building);

                _selection.CurSelect.Constructing.StartDeconstruct();

                var task = new DeconstructWork(_colony);
                task.Setup(_selection.CurSelect);
                _colony.Works.Add(task);
            }
            else if (btn == "ButtonMove")
            {
                _dragger.SetDrag(_selection.CurSelect.Data, DragFinished);
            }
            else
            {
                base.OnBtnClick(btn, action);
            }
        }
        private void DragFinished(WagonBuilding buildingView, bool isCanceled)
        {

        }
    }
}
