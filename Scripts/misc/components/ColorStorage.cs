using misc.tweens;

using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace misc.components
{
    internal class ColorStorage : ExtMonoBeh
    {
        [SerializeField] private Color[] _colors;
        [SerializeField] private bool _randomStartup;

        private Graphic _img;
        private SpriteRenderer _sprite;

        protected override void OnAwake()
        {
            base.OnAwake();

            _img = GetComponent<Graphic>();
            _sprite = _img == null ? GetComponent<SpriteRenderer>() : null;
            if (_img != null && _colors.Length > 0)
            {
                _img.color = _colors[_randomStartup ? Random.Range(0, _colors.Length) : 0];
            }
            if (_sprite != null && _colors.Length > 0)
            {
                _sprite.color = _colors[_randomStartup ? Random.Range(0, _colors.Length) : 0];
            }
        }

        public Color GetColor(int idx)
        {
            return idx < _colors.Length ? _colors[idx] : _colors[0];
        }

        public void SetColorForce(int idx)
        {
            if (_colors.Length == 0)
            {
                return;
            }
            if (idx >= _colors.Length)
            {
                idx = 0;
            }
            if (_img != null)
            {
                _img.color = _colors[idx];
            }
            else if (_sprite != null)
            {
                _sprite.color = _colors[idx];
            }
        }

        public void SetColorBlend(int idx, float speed = 1.5f)
        {
            if (_colors.Length == 0)
            {
                return;
            }
            if (idx >= _colors.Length)
            {
                idx = 0;
            }
            if (_img != null)
            {
                new TweenColor(speed, _img, _colors[idx]).DoAlive();
            }
        }
    }
}
