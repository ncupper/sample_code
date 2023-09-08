using game;
using game.wagon_buildings;

using misc;

using UnityEngine;

using Zenject;
namespace gui.selection
{
    internal class SmallButtonsGrid : ExtMonoBeh
    {
        [SerializeField] private GameObject _trash;
        [SerializeField] private GameObject _cancel;
        [SerializeField] private GameObject _move;

        private WagonBuilding _building;
        private bool _canCancel;
        private MouseSelectionHandler _selection;
        private bool _waitBuild;

        private void Update()
        {
            if (_building != _selection.CurSelect)
            {
                if (_selection.CurSelect == null)
                {
                    _building = null;
                }
                else
                {
                    _building = _selection.CurSelect;
                }
            }

            _waitBuild = _building != null && _building.Constructing.WaitConstructing;
            _canCancel = _waitBuild && _building.Constructing.CanWorkCanceled;
            _trash.SetActive(_building != null && !_waitBuild);
            _cancel.SetActive(_building != null && _canCancel);
            _move.SetActive(_building != null && !_waitBuild);
        }

        [Inject]
        public void Construct(MouseSelectionHandler selection)
        {
            _selection = selection;
        }
    }
}
