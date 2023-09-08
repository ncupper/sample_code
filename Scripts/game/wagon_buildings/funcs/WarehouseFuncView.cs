using System;

using misc;

using UnityEngine;
namespace game.wagon_buildings.funcs
{
    internal class WarehouseFuncView : ExtMonoBeh
    {
        public Vector3Int Limits;
        public Vector3 BoxPosCorner;
        public Vector3 BoxPosOffset;

        public event Action OnViewEnabled;
        public event Action OnViewDisabled;

        protected override void OnEnabled()
        {
            OnViewEnabled?.Invoke();
        }

        protected override void OnDisabled()
        {
            OnViewDisabled?.Invoke();
        }
    }
}
