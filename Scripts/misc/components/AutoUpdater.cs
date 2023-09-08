using UnityEngine;
using UnityEngine.Events;
namespace misc.components
{
    public class AutoUpdater : ExtMonoBeh
    {
        [SerializeField] private float _updatePeriod;

        private float _waitTime;

        public UnityAction OnUpdate = delegate {};

        private void Update()
        {
            _waitTime -= Time.deltaTime;
            if (_waitTime <= 0)
            {
                _waitTime = _updatePeriod;
                OnUpdate();
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _waitTime = _updatePeriod;
        }
    }
}
