using misc;
using misc.components;
using misc.managers;
using misc.tweens;

using UnityEngine;
using UnityEngine.Events;
namespace game
{
    public class WagonView : ExtMonoBeh, IEventListener
    {
        public const int Width = 34;
        public const int Height = 6;

        [SerializeField] private ClickableCollider _walk;
        [SerializeField] private WagonFloorCells _cells;
        [SerializeField] private GameObject _frontWall;

        public UnityAction<WagonView, Vector3> OnWalkClick = delegate {};

        private void OnDestroy()
        {
            EventBus.RemoveListener<TrainCameraController.Changed>(this);
        }

        public bool OnEvent(EventData eventData)
        {
            if (eventData is TrainCameraController.Changed signal)
            {
                SetFrontWallVis(signal.Zoom > 0);
            }
            return false;
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            if (_walk != null)
            {
                _walk.ClickAction += x =>
                {
                    OnWalkClick(this, x.Point);
                };
            }

            _cells.Setup(Width, Height);
            _cells.Visible = false;
        }

        protected override void OnStart()
        {
            base.OnStart();
            EventBus.AddListener<TrainCameraController.Changed>(this);
        }

        public void SetFloorCellsVis(bool vis)
        {
            _cells.Visible = vis;
        }

        public WagonFloorCell GetMouseFloorCell()
        {
            return _cells.GetMouseFloorCell();
        }

        public WagonFloorCell GetCell(int idx)
        {
            return _cells.GetCell(idx, 0, 0);
        }

        private void SetFrontWallVis(bool vis)
        {
            if (_frontWall != null)
            {
                new TweenScale(0.4f, _frontWall.transform, new Vector3(1.0f, vis ? 1.0f : 0.2f, 1.0f)).DoAlive();
            }
        }
    }
}
