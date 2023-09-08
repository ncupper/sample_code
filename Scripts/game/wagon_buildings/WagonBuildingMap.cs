using System.Collections.Generic;

using loader.database;
namespace game.wagon_buildings
{
    public class WagonBuildingMap
    {

        private readonly Dictionary<WagonFloorCell, WagonFloorCellColor> _debugFloorCells =
            new Dictionary<WagonFloorCell, WagonFloorCellColor>();
        private readonly DbWagonBuildingMap _mapData;

        public WagonBuildingMap(DbWagonBuilding data)
        {
            _mapData = data.Map;
        }

        public bool CanPlace
        {
            get;
            private set;
        }

        public void ClearMap()
        {
            foreach (KeyValuePair<WagonFloorCell, WagonFloorCellColor> cell in _debugFloorCells)
            {
                cell.Key.SetColor(cell.Value);
            }
            _debugFloorCells.Clear();
        }

        public void MarkCells(bool isDrag, WagonFloorCell placerCell, int angle)
        {
            ClearMap();

            if (isDrag)
            {
                CheckCanPlace(placerCell, angle);
            }

            if (placerCell == null)
            {
                return;
            }

            WagonFloorCell corner = placerCell.GetNeighbour(-_mapData.Size.x / 2, -_mapData.Size.y / 2);
            if (corner == null)
            {
                return;
            }

            corner = angle switch
            {
                1 => corner.GetNeighbour(0, _mapData.Size.y - 1),
                2 => corner.GetNeighbour(_mapData.Size.x - 1, _mapData.Size.y - 1),
                3 => corner.GetNeighbour(_mapData.Size.x - 1, 0),
                _ => corner
            };

            if (corner == null)
            {
                return;
            }

            for (var y = 0; y < _mapData.Cells.Length; ++y)
            {
                for (var x = 0; x < _mapData.Cells[y].Line.Length; ++x)
                {
                    int c = _mapData.Cells[y].Line[x];
                    if (c == 0)
                    {
                        continue;
                    }

                    WagonFloorCell cell = angle switch
                    {
                        0 => corner.GetNeighbour(x + _mapData.Offset.x, y + _mapData.Offset.y),
                        1 => corner.GetNeighbour(y + _mapData.Offset.y, -x - _mapData.Offset.x),
                        2 => corner.GetNeighbour(-x - _mapData.Offset.x, -y - _mapData.Offset.y),
                        _ => corner.GetNeighbour(-y - _mapData.Offset.y, x + _mapData.Offset.x)
                    };
                    if (cell != null)
                    {
                        _debugFloorCells.Add(cell, cell.Color);
                        if (isDrag)
                        {
                            if (!CanPlace)
                            {
                                c = (int)WagonFloorCellColor.Blocked;
                            }
                            else if (c == (int)WagonFloorCellColor.Blocked)
                            {
                                c = (int)WagonFloorCellColor.Free;
                            }
                        }
                        cell.SetColor((WagonFloorCellColor)c);
                    }
                }
            }
        }

        private void CheckCanPlace(WagonFloorCell placerCell, int angle)
        {
            CanPlace = false;

            if (placerCell == null)
            {
                return;
            }

            WagonFloorCell corner = placerCell.GetNeighbour(-_mapData.Size.x / 2, -_mapData.Size.y / 2);
            if (corner == null)
            {
                return;
            }

            corner = angle switch
            {
                1 => corner.GetNeighbour(0, _mapData.Size.y - 1),
                2 => corner.GetNeighbour(_mapData.Size.x - 1, _mapData.Size.y - 1),
                3 => corner.GetNeighbour(_mapData.Size.x - 1, 0),
                _ => corner
            };

            if (corner == null)
            {
                return;
            }

            var haveSomeYellow = false;
            var haveFreeYellow = false;
            for (var y = 0; y < _mapData.Cells.Length; ++y)
            {
                for (var x = 0; x < _mapData.Cells[y].Line.Length; ++x)
                {
                    int c = _mapData.Cells[y].Line[x];
                    if (c == 0)
                    {
                        continue;
                    }

                    WagonFloorCell cell = angle switch
                    {
                        0 => corner.GetNeighbour(x + _mapData.Offset.x, y + _mapData.Offset.y),
                        1 => corner.GetNeighbour(y + _mapData.Offset.y, -x - _mapData.Offset.x),
                        2 => corner.GetNeighbour(-x - _mapData.Offset.x, -y - _mapData.Offset.y),
                        _ => corner.GetNeighbour(-y - _mapData.Offset.y, x + _mapData.Offset.x)
                    };
                    if (cell != null)
                    {
                        bool isFree = (int)cell.Color == (int)WagonFloorCellColor.Default
                            || cell.Color == WagonFloorCellColor.Pivot;
                        if (c == (int)WagonFloorCellColor.Pivot)
                        {
                            haveSomeYellow = true;
                            if (isFree)
                            {
                                haveFreeYellow = true;
                            }
                        }
                        if (c == (int)WagonFloorCellColor.Blocked && !isFree)
                        {
                            return;
                        }
                    }
                    else if (c == (int)WagonFloorCellColor.Blocked)
                    {
                        return;
                    }
                }
            }

            CanPlace = !haveSomeYellow || haveFreeYellow;
        }
    }
}
