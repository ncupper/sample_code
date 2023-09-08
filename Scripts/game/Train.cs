using System.Collections.Generic;

using misc;

using UnityEngine;
namespace game
{
    internal class Train : ExtMonoBeh
    {
        [SerializeField] private float _speed = 15;
        [SerializeField] private WagonView _locomotive;
        [SerializeField] private WagonView[] _wagons;
        [SerializeField] private ExtMonoBeh _roadBlock;

        private readonly List<ExtMonoBeh> _poolRoad = new List<ExtMonoBeh>();

        public static Train Instance
        {
            get;
            private set;
        }

        public WagonView Wagon => _wagons[0];
        public WagonView Locomotive => _locomotive;

        private void Update()
        {
            var shift = new Vector3(-_speed * Time.deltaTime, 0, 0);
            for (var i = 0; i < _poolRoad.Count; ++i)
            {
                _poolRoad[i].Self.position += shift;
            }

            if (_poolRoad[0].Self.position.x < -180)
            {
                ExtMonoBeh last = _poolRoad[_poolRoad.Count - 1];
                _poolRoad.Add(_poolRoad[0]);
                _poolRoad.RemoveAt(0);
                _poolRoad[_poolRoad.Count - 1].Self.position = last.Self.position + new Vector3(120, 0, 0);
                RndPrefabPlacer[] pool = _poolRoad[_poolRoad.Count - 1].GetComponentsInChildren<RndPrefabPlacer>();
                for (var i = 0; i < pool.Length; ++i)
                {
                    if (pool[i].Self != null)
                    {
                        pool[i].Setup();
                    }
                }
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
            for (var i = 0; i < _wagons.Length; ++i)
            {
                _wagons[i].OnWalkClick += OnWalkClick;
            }

            Helper.ResizePool(_poolRoad, _roadBlock, 4);
            for (var i = 0; i < _poolRoad.Count; ++i)
            {
                _poolRoad[i].Self.position = new Vector3(120 * i - 60, 0, 0);
            }
        }

        private void OnWalkClick(WagonView wagon, Vector3 point)
        {
        }

        public void SetFloorCellsVis(bool vis)
        {
            for (var i = 0; i < _wagons.Length; ++i)
            {
                _wagons[i].SetFloorCellsVis(vis);
            }
        }
    }
}
