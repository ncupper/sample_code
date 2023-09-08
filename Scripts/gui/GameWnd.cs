using game;
using game.wagon_buildings;

using gui.building;
using gui.craft;
using gui.review;
using gui.selection;

using loader;
using loader.database;

using misc;

using UnityEngine;

using Zenject;
namespace gui
{
    internal class GameWnd : GuiScreen
    {
        [SerializeField] private SelectNode _selectNode;
        [SerializeField] private CraftNode _craftNode;
        [SerializeField] private ReviewNode _reviewNode;
        [SerializeField] private BuildingNode _buildNode;
        private WagonBuildingDragger _dragger;

        private MouseSelectionHandler _selection;

        public SelectNode SelectNode => _selectNode;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Saver.DoSave();
            }

            if (Application.isEditor && Input.GetKeyDown(KeyCode.BackQuote))
            {
                DbWagonBuilding bdata = DataStorage.WagonBuildings[WagonBuildingId.GarbageRecycler];
                _dragger.SetDrag(bdata, null);
            }

            CheckSelection();
        }

        [Inject]
        public void Construct(MouseSelectionHandler selection, WagonBuildingDragger dragger)
        {
            _selection = selection;
            _dragger = dragger;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _selectNode = GetNode<SelectNode>();
        }

        private void CheckSelection()
        {
            if (_selectNode == null)
            {
                return;
            }

            if (!_selectNode.Visible && _selection.CurSelect != null)
            {
                ShowNode("select");
            }
        }

        public void ToggleCraft(bool vis)
        {
            if (_craftNode.Visible != vis)
            {
                if (vis)
                {
                    ShowNode(_craftNode);
                }
                else
                {
                    _craftNode.DelayedHideNode();
                }
            }
        }

        public void ToggleReview(bool vis)
        {
            if (_reviewNode.Visible != vis)
            {
                if (vis)
                {
                    ShowNode(_reviewNode);
                }
                else
                {
                    _reviewNode.DelayedHideNode();
                }
            }
        }
    }
}
