using System;
using System.Collections.Generic;

using OneLine;

using UnityEngine;

using Random = UnityEngine.Random;
namespace misc
{
    [Serializable]
    internal struct RndPropDataInt
    {
        public int Min;
        public int Max;
        public int Step;
    }

    [Serializable]
    internal struct RndPrefabItem
    {
        public GameObject Prefab;
        public int Weight;
        [OneLineWithHeader] public RndPropDataInt RotationX;
        [OneLine] public RndPropDataInt RotationY;
        [OneLine] public RndPropDataInt RotationZ;
        [OneLine] public RndPropDataInt Scale;
    }

    internal class RndPrefabPlacer : ExtMonoBeh
    {
        [SerializeField] private int _emptyChance;
        [SerializeField] private RndPrefabItem[] _items;

        private GameObject _view;

        private int Rnd => Random.Range(0, 100);

#if UNITY_EDITOR
        private void Update()
        {
            if (_waiterF5 != this)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var needDel = new List<RndPrefabPlacer>();
                for (var i = 0; i < Pool.Count; ++i)
                {
                    if (Pool[i].Self == null)
                    {
                        needDel.Add(Pool[i]);
                    }
                    else
                    {
                        Pool[i].Setup();
                    }
                }

                for (var i = 0; i < needDel.Count; ++i)
                {
                    Pool.Remove(needDel[i]);
                }
            }
        }
#endif

        private void OnDestroy()
        {
#if UNITY_EDITOR
            if (_waiterF5 == this)
            {
                _waiterF5 = null;
            }
            Pool.Remove(this);
#endif
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            Setup();
#if UNITY_EDITOR
            if (_waiterF5 == null)
            {
                _waiterF5 = this;
            }
            Pool.Add(this);
#endif
        }

        public void Setup()
        {
            if (_view != null)
            {
                DestroyImmediate(_view);
            }

            if (_emptyChance > Rnd)
            {
                return;
            }

            var weight = 0;
            for (var i = 0; i < _items.Length; ++i)
            {
                if (_items[i].Prefab == null || _items[i].Weight == 0)
                {
                    continue;
                }
                weight += _items[i].Weight;
            }

            int rnd = Random.Range(0, weight);
            weight = 0;
            RndPrefabItem item;
            for (var i = 0; i < _items.Length; ++i)
            {
                if (_items[i].Prefab == null || _items[i].Weight == 0)
                {
                    continue;
                }
                weight += _items[i].Weight;
                if (weight > rnd)
                {
                    item = _items[i];
                    _view = Helper.Clone(item.Prefab, Self);
                    float angleX = 0;
                    if (item.RotationX.Max != item.RotationX.Min)
                    {
                        if (item.RotationX.Step != 0)
                        {
                            int limit = (item.RotationX.Max - item.RotationX.Min) / item.RotationX.Step;
                            rnd = Random.Range(0, limit);
                            angleX = item.RotationX.Min + rnd * item.RotationX.Step;
                        }
                        else
                        {
                            angleX = Random.Range(item.RotationX.Min, item.RotationX.Max);
                        }
                    }

                    float angleY = 0;
                    if (item.RotationY.Max != item.RotationY.Min)
                    {
                        if (item.RotationY.Step != 0)
                        {
                            int limit = (item.RotationY.Max - item.RotationY.Min) / item.RotationY.Step;
                            rnd = Random.Range(0, limit);
                            angleY = item.RotationY.Min + rnd * item.RotationY.Step;
                        }
                        else
                        {
                            angleY = Random.Range(item.RotationY.Min, item.RotationY.Max);
                        }
                    }

                    float angleZ = 0;
                    if (item.RotationZ.Max != item.RotationZ.Min)
                    {
                        if (item.RotationZ.Step != 0)
                        {
                            int limit = (item.RotationZ.Max - item.RotationZ.Min) / item.RotationZ.Step;
                            rnd = Random.Range(0, limit);
                            angleZ = item.RotationZ.Min + rnd * item.RotationZ.Step;
                        }
                        else
                        {
                            angleZ = Random.Range(item.RotationZ.Min, item.RotationZ.Max);
                        }
                    }

                    _view.transform.localEulerAngles = new Vector3(angleX, angleY, angleZ);

                    var scale = 1.0f;
                    if (item.Scale.Max != item.Scale.Min)
                    {
                        if (item.Scale.Step != 0)
                        {
                            int limit = (item.Scale.Max - item.Scale.Min) / item.Scale.Step;
                            rnd = Random.Range(0, limit);
                            scale = 1.0f + (item.Scale.Min + rnd * item.Scale.Step) / 100.0f;
                        }
                        else
                        {
                            scale = 1.0f + Random.Range(item.Scale.Min, item.Scale.Max) / 100.0f;
                        }
                    }

                    _view.transform.localScale = Vector3.one * scale;
                    break;
                }
            }
        }

#if UNITY_EDITOR
        private static RndPrefabPlacer _waiterF5;
        private static readonly List<RndPrefabPlacer> Pool = new List<RndPrefabPlacer>();
#endif
    }
}
