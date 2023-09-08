using misc;

using UnityEngine;
namespace game.wagon_buildings
{
    internal class BuildingPlaceView : WagonBuildingView
    {
        public ExtMonoBeh Left;
        public ExtMonoBeh Top;
        public ExtMonoBeh Right;
        public ExtMonoBeh Bottom;
        public BoxCollider Collider;
        public InteractPivotView LeftPivot;
        public InteractPivotView TopPivot;
        public InteractPivotView RightPivot;
        public InteractPivotView BottomPivot;
    }
}
