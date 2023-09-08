using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#pragma warning disable 649

namespace misc.components
{
    internal class ExMultiToggle : ExtMonoBeh
    {
        [SerializeField] private GameObject[] _states;
        [SerializeField] private int _select;

#if UNITY_EDITOR
        private bool _prevIsOn;
#endif
        public UnityAction<ExMultiToggle> OnChange = delegate {};

        public int Select
        {
            get => _select;
            set
            {
                int oldVal = _select;
                _select = value;
                UpdateView();
                if (oldVal != _select)
                {
                    OnChange(this);
                }
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            var btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnClick);
            }

            UpdateView();
        }

        private void OnClick()
        {
            _select = (_select + 1) % _states.Length;
            OnChange(this);
            UpdateView();
        }

        private void UpdateView()
        {
            for (var i = 0; i < _states.Length; ++i)
            {
                if (_states[i] != null)
                {
                    _states[i].SetActive(i == _select);
                }
            }
        }
    }
}
