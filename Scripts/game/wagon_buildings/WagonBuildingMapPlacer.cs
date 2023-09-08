using UnityEngine;
using UnityEngine.AI;
namespace game.wagon_buildings
{
    public class WagonBuildingMapPlacer
    {
        private readonly int _height;
        private readonly WagonBuildingMap _map;
        private readonly NavMeshObstacle _navCarve;
        private readonly Transform _self;
        private readonly int _width;

        public WagonBuildingMapPlacer(GameObject gameObject, WagonBuilding building)
        {
            _width = building.Data.Map.Size.x;
            _height = building.Data.Map.Size.y;

            _map = new WagonBuildingMap(building.Data);
            _self = gameObject.transform;
            _navCarve = gameObject.GetComponent<NavMeshObstacle>();
        }

        public int CellIdx { get; private set; }
        public WagonFloorCell Cell
        {
            get;
            private set;
        }
        public int Angle => Mathf.RoundToInt(_self.localEulerAngles.y / 90.0f);
        public WagonFloorCell CornerCell => Cell != null ? Cell.GetNeighbour(-Width / 2, -Height / 2) : null;
        private bool IsRotated => Mathf.RoundToInt(_self.localEulerAngles.y / 90.0f) % 2 == 1;
        public int Width => IsRotated ? _height : _width;
        public int Height => IsRotated ? _width : _height;

        public bool PlaceToFloorCell(WagonFloorCell cell, bool isDrag)
        {
            if (cell == null)
            {
                return false;
            }

            WagonFloorCell corner = cell.GetNeighbour(-Width / 2, -Height / 2);
            if (corner == null)
            {
                cell = FindCornerNear(cell);
                corner = cell.GetNeighbour(-Width / 2, -Height / 2);
                if (corner == null)
                {
                    return false;
                }
            }

            if (corner.GetNeighbour(Width - 1, Height - 1) == null)
            {
                cell = FindRightCornerNear(cell);
                corner = cell.GetNeighbour(-Width / 2, -Height / 2);
                if (corner == null || corner.GetNeighbour(Width - 1, Height - 1) == null)
                {
                    return false;
                }
            }

            Cell = cell;
            CellIdx = Cell.Idx;
            _map.MarkCells(isDrag, Cell, Angle);

            Vector3 pos = Cell.Self.position;
            if (Width % 2 == 0)
            {
                pos.x -= WagonFloorCell.HalfSize.x;
            }
            if (Height % 2 == 0)
            {
                pos.z -= WagonFloorCell.HalfSize.y;
            }

            _self.position = pos;
            return true;
        }

        public bool CanPlace()
        {
            return _map.CanPlace;
        }

        public void BeginDrag()
        {
            SetNavigationCarveEnabled(false);
        }

        public void EndDrag()
        {
            SetNavigationCarveEnabled(true);

            ClearMap();
        }

        public void SetNavigationCarveEnabled(bool value)
        {
            if (_navCarve != null)
            {
                _navCarve.enabled = value;
            }
        }

        public void ClearMap()
        {
            _map.ClearMap();
        }

        private WagonFloorCell FindCornerNear(WagonFloorCell cell)
        {
            WagonFloorCell c = cell;
            var delta = 1;
            while (c != null && delta < 3)
            {
                c = cell.GetNeighbour(0, -delta);
                if (c != null && c.GetNeighbour(-Width / 2, -Height / 2))
                {
                    return c;
                }
                c = cell.GetNeighbour(0, delta);
                if (c != null && c.GetNeighbour(-Width / 2, -Height / 2))
                {
                    return c;
                }
                c = cell.GetNeighbour(delta, 0);
                if (c != null && c.GetNeighbour(-Width / 2, -Height / 2))
                {
                    return c;
                }
                c = cell.GetNeighbour(-delta, 0);
                if (c != null && c.GetNeighbour(-Width / 2, -Height / 2))
                {
                    return c;
                }
                ++delta;
            }

            return cell;
        }

        private WagonFloorCell FindRightCornerNear(WagonFloorCell cell)
        {
            WagonFloorCell c = cell;
            var delta = 1;
            while (c != null && delta < 3)
            {
                c = cell.GetNeighbour(0, -delta);
                if (c != null)
                {
                    WagonFloorCell corner = c.GetNeighbour(-Width / 2, -Height / 2);
                    if (corner != null && corner.GetNeighbour(Width - 1, Height - 1) != null)
                    {
                        return c;
                    }
                }
                c = cell.GetNeighbour(0, delta);
                if (c != null)
                {
                    WagonFloorCell corner = c.GetNeighbour(-Width / 2, -Height / 2);
                    if (corner != null && corner.GetNeighbour(Width - 1, Height - 1) != null)
                    {
                        return c;
                    }
                }
                c = cell.GetNeighbour(delta, 0);
                if (c != null)
                {
                    WagonFloorCell corner = c.GetNeighbour(-Width / 2, -Height / 2);
                    if (corner != null && corner.GetNeighbour(Width - 1, Height - 1) != null)
                    {
                        return c;
                    }
                }
                c = cell.GetNeighbour(-delta, 0);
                if (c != null)
                {
                    WagonFloorCell corner = c.GetNeighbour(-Width / 2, -Height / 2);
                    if (corner != null && corner.GetNeighbour(Width - 1, Height - 1) != null)
                    {
                        return c;
                    }
                }
                ++delta;
            }

            return cell;
        }

        private void FindPlaceNear(WagonFloorCell cell, bool isDrag)
        {
            WagonFloorCell c = cell;
            var delta = 1;
            while (c != null && delta < 3)
            {
                c = cell.GetNeighbour(0, -delta);
                if (PlaceToFloorCell(c, isDrag))
                {
                    break;
                }
                c = cell.GetNeighbour(0, delta);
                if (PlaceToFloorCell(c, isDrag))
                {
                    break;
                }
                c = cell.GetNeighbour(delta, 0);
                if (PlaceToFloorCell(c, isDrag))
                {
                    break;
                }
                c = cell.GetNeighbour(-delta, 0);
                if (PlaceToFloorCell(c, isDrag))
                {
                    break;
                }
                ++delta;
            }
        }

        public void DoRotate(bool isDrag)
        {
            Vector3 r = _self.localEulerAngles;
            r.y += 90;
            _self.localEulerAngles = r;
            if (Cell != null)
            {
                WagonFloorCell cell = Cell;
                Cell = null;
                if (!PlaceToFloorCell(cell, isDrag))
                {
                    FindPlaceNear(cell, isDrag);
                }
            }
        }
    }
}
