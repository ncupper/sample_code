using misc;

using UnityEngine;
namespace game.wagon_buildings
{
    public enum InteractPivotType
    {
        None,
        General,
        LoadUnload
    }

    public enum PivotPose
    {
        None,
        Rotate,
        Seat
    }

    public class InteractPivotView : ExtMonoBeh
    {
        [SerializeField] private InteractPivotType _type;
        [SerializeField] private GameObject _debugView;
        [SerializeField] private PivotPose _pose;

        public InteractPivotType Type => _type;
        public PivotPose Pose => _pose;

        public WorkerView Locked { get; set; }

        public WagonFloorCell FloorCell { get; set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            _debugView.SetActive(false);
        }

        public void SwitchDebugViewVisible(bool vis)
        {
            _debugView.SetActive(vis);
        }
    }
}
