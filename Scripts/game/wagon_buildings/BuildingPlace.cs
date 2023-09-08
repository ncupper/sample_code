using System.Collections.Generic;

using loader;
using loader.database;

using misc;

using UnityEngine;
namespace game.wagon_buildings
{
    public class BuildingPlace : WagonBuilding
    {
        private readonly List<InteractPivotView> _pivotPool = new List<InteractPivotView>();
        private int _nowHeight;
        private int _nowWidth;

        private readonly BuildingPlaceView _placeView;

        public BuildingPlace(DataLoader dataLoader, WagonBuildingView view, DbWagonBuilding data, DbPlayerWagonBuilding playerData)
            : base(dataLoader, view, data, playerData)
        {
            _placeView = View.GetComponent<BuildingPlaceView>();
        }

        private void Resize(int width, int height)
        {
            _nowWidth = width;
            _nowHeight = height;

            var szL = 1.25f;
            float halfSzL = szL * 0.5f;

            var koef = 0.004f;
            float szWx = 0.135f - koef + _nowWidth * koef;
            float halfSzWx = szWx * 0.5f;
            float szWy = 0.135f - koef + _nowHeight * koef;
            float halfSzWy = szWy * 0.5f;

            _placeView.Left.Self.localScale = new Vector3(szWx, szL * _nowHeight - szWy, 1);
            _placeView.Right.Self.localScale = new Vector3(szWx, szL * _nowHeight - szWy, 1);
            _placeView.Left.Self.localPosition =
                new Vector3(-halfSzL + halfSzWx, 0.02f, -halfSzWy + halfSzL * _nowHeight - halfSzL);
            _placeView.Right.Self.localPosition = new Vector3(-halfSzL - halfSzWx + szL * _nowWidth, 0.02f,
                halfSzWy + halfSzL * _nowHeight - halfSzL);

            _placeView.Top.Self.localScale = new Vector3(szWy, szL * _nowWidth - szWx, 1);
            _placeView.Bottom.Self.localScale = new Vector3(szWy, szL * _nowWidth - szWx, 1);
            _placeView.Top.Self.localPosition = new Vector3(-halfSzL - halfSzWx + halfSzL * _nowWidth, 0.02f,
                -halfSzL + szL * _nowHeight - halfSzWy);
            _placeView.Bottom.Self.localPosition =
                new Vector3(-halfSzL + halfSzWx + halfSzL * _nowWidth, 0.02f, -halfSzL + halfSzWy);

            _placeView.Left.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, 4 * _nowHeight);
            _placeView.Right.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, 4 * _nowHeight);
            _placeView.Top.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, 4 * _nowWidth);
            _placeView.Bottom.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(1, 4 * _nowWidth);

            _placeView.Collider.center = new Vector3(halfSzL * (_nowWidth - 1), 0, halfSzL * (_nowHeight - 1));
            _placeView.Collider.size = new Vector3(szL * _nowWidth, 0.1f, szL * _nowHeight);
        }

        public void StartBuild(WagonBuilding building)
        {
            Resize(building.MapPlacer.Width, building.MapPlacer.Height);

            int count = (building.MapPlacer.Width + building.MapPlacer.Height) * 2;
            if (count < _pivotPool.Count)
            {
                for (int i = count; i < _pivotPool.Count; ++i)
                {
                    Object.DestroyImmediate(_pivotPool[i].gameObject);
                }
                _pivotPool.RemoveRange(count, _pivotPool.Count - count);
            }
            Helper.ResizePool(_pivotPool, _placeView.LeftPivot, count);
            for (var i = 0; i < building.MapPlacer.Width; ++i)
            {
                _pivotPool[i].Self.localPosition =
                    _placeView.BottomPivot.Self.localPosition + Vector3.right * (i * WagonFloorCell.Size.x);
                _pivotPool[i + building.MapPlacer.Width].Self.localPosition =
                    _placeView.TopPivot.Self.localPosition + Vector3.right * (i * WagonFloorCell.Size.x)
                    + Vector3.forward * ((building.MapPlacer.Height - 1) * WagonFloorCell.Size.y);
            }
            for (var i = 0; i < building.MapPlacer.Height; ++i)
            {
                int idx = i + building.MapPlacer.Width * 2;
                _pivotPool[idx].Self.localPosition =
                    _placeView.LeftPivot.Self.localPosition + Vector3.forward * (i * WagonFloorCell.Size.y);
                _pivotPool[idx + building.MapPlacer.Height].Self.localPosition =
                    _placeView.RightPivot.Self.localPosition + Vector3.forward * (i * WagonFloorCell.Size.x)
                    + Vector3.right * ((building.MapPlacer.Width - 1) * WagonFloorCell.Size.y);
            }
            _placeView.InteractPivots = _pivotPool.ToArray();

            //_placeView.Ings.ResizeLimits(new Vector3Int(building.MapPlacer.Width, _placeView.Ings.Limits.y, building.MapPlacer.Height));
        }
    }
}
