using misc.managers;
using misc.tweens;

using UnityEngine;
#pragma warning disable 649

namespace misc.components
{
    internal class AutoHider : ExtMonoBeh
    {
        [SerializeField] private float _delay;
        [SerializeField] private bool _autoActivate;
        private IHaveUpdate _tween;

        public Vector3 LocalPos
        {
            get => Self.localPosition;
            set => Self.localPosition = value;
        }

        public bool Auto => _autoActivate;

        private void OnEnable()
        {
            if (_autoActivate)
            {
                Activate();
            }
        }

        private void OnDisable()
        {
            TimersManager.Remove(_tween);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (_autoActivate)
            {
                Activate();
            }
            else
            {
                Visible = false;
            }
        }

        public void Activate()
        {
            Visible = true;
            TimersManager.Remove(_tween);
            _tween = new TweenWait(_delay, x => { Visible = false; });
        }
    }
}
