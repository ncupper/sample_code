using game.wagon_buildings;

using misc;
using misc.components;

using UnityEngine;
namespace game
{
    public enum WagonFloorCellColor
    {
        Default = 0,
        Blocked = 1,
        Free,
        Pivot
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class WagonFloorCell : ExtMonoBeh
    {
        public static Vector2 Size = new Vector2(1.25f, 1.25f);
        public static Vector2 HalfSize = Size * 0.5f;
        private Collider _collider;

        private ColorStorage _colors;
        private SpriteRenderer _sprite;
        public WagonBuildingView Building { get; set; }

        public WagonFloorCells Container { get; private set; }
        public int Idx { get; private set; }

        public WagonFloorCellColor Color
        {
            get;
            private set;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _sprite = GetComponent<SpriteRenderer>();
            _colors = GetComponent<ColorStorage>();
            _collider = GetComponent<Collider>();
        }
        public void Setup(WagonFloorCells container, int idx)
        {
            Container = container;
            Idx = idx;
        }

        public void SetColor(WagonFloorCellColor color)
        {
            Color = color;
            _colors.SetColorForce((int)color);
        }

        public bool Is()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (_collider.Raycast(ray, out hit, 2000))
            {
                return true;
            }

            return false;
        }

        public WagonFloorCell GetNeighbour(int dx, int dy)
        {
            return Container.GetCell(Idx, dx, dy);
        }
    }
}
