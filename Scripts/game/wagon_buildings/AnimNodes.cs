using misc;

using UnityEngine;
namespace game.wagon_buildings
{
    [ExecuteAlways]
    internal class AnimNodes : ExtMonoBeh
    {
        [SerializeField] private GameObject[] _nodes;
        [SerializeField] private float _speed = 1;
        private int _curNode;

        private float _curTime;

        private void Update()
        {
            _curTime += Time.deltaTime;
            if (_curTime > _speed && _nodes != null && _nodes.Length > 0)
            {
                _curTime -= _speed;
                _curNode = (_curNode + 1) % _nodes.Length;
                UpdateSprite();
            }
        }

        private void OnEnable()
        {
            UpdateSprite();
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (_nodes == null || _curNode < 0 || _curNode >= _nodes.Length)
            {
                return;
            }
            for (var i = 0; i < _nodes.Length; ++i)
            {
                if (_nodes[i] != null)
                {
                    _nodes[i].SetActive(i == _curNode);
                }
            }
        }
    }
}
