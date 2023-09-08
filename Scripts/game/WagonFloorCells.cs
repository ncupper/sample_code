using System.Collections.Generic;

using misc;

using UnityEngine;
namespace game
{
    public class WagonFloorCells : ExtMonoBeh
    {
        [SerializeField] private WagonFloorCell _sample;
        private readonly List<WagonFloorCell> _pool = new List<WagonFloorCell>();

        private int _width;

        protected override void OnAwake()
        {
            base.OnAwake();
            _sample.Visible = false;
        }

        public void Setup(int width, int height)
        {
            _width = width;
            Helper.ResizePool(_pool, _sample, _width * height, OnCreateCell);
            for (var i = 0; i < _pool.Count; ++i)
            {
                int x = i % _width;
                int y = i / _width;
                _pool[i].Setup(this, i);
                _pool[i].Self.localPosition = new Vector3(x * 1.25f, 0, y * 1.25f);
                _pool[i].SetColor(WagonFloorCellColor.Default);
            }
        }

        private void OnCreateCell(WagonFloorCell cell)
        {
        }

        public WagonFloorCell GetMouseFloorCell()
        {
            for (var i = 0; i < _pool.Count; ++i)
            {
                if (_pool[i].Is())
                {
                    return _pool[i];
                }
            }
            return null;
        }

        public WagonFloorCell GetCell(int baseIdx, int dx, int dy)
        {
            int x = baseIdx % _width;
            int y = baseIdx / _width;
            if (x + dx < 0 || x + dx >= _width)
            {
                return null;
            }
            int height = _pool.Count / _width;
            if (y + dy < 0 || y + dy >= height)
            {
                return null;
            }
            return _pool[x + dx + (y + dy) * _width];
        }

        public WagonFloorCell GetCell(Vector3 pos)
        {
            return null;
        }
    }
}
