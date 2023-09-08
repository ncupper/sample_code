using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#pragma warning disable 649

namespace misc.components
{
    [ExecuteInEditMode]
    internal class ExToggle : ExtMonoBeh
    {
        [SerializeField] private GameObject _active;
        [SerializeField] private GameObject _notActive;
        [SerializeField] private bool _isOn;
        private Button _btn;
        private ExToggleGroup _group;

#if UNITY_EDITOR
        private bool _prevIsOn;
#endif

        public UnityAction<ExToggle> OnChange = delegate {};

        public bool IsOn
        {
            get { return _isOn; }
            set
            {
                bool oldIsOn = _isOn;
                _isOn = value;
#if UNITY_EDITOR
                _prevIsOn = _isOn;
#endif
                Update();
                if (oldIsOn != _isOn)
                {
                    OnChange(this);
                }
            }
        }

        public GameObject Active => _active;

        public GameObject NotActive => _notActive;

        public bool Interactable
        {
            get => _btn != null && _btn.interactable;
            set
            {
                if (_btn != null)
                {
                    _btn.interactable = value;
                }
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (_prevIsOn != _isOn)
            {
                _prevIsOn = _isOn;
                OnChange(this);
            }
#endif
            if (_active != null)
            {
                _active.SetActive(_isOn);
            }
            if (_notActive != null)
            {
                _notActive.SetActive(!_isOn);
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _group = GetComponentInParent<ExToggleGroup>();

            _btn = GetComponent<Button>();
            if (_btn != null)
            {
                _btn.onClick.AddListener(OnClick);
            }
#if UNITY_EDITOR
            _prevIsOn = _isOn;
#endif
        }

        private void OnClick()
        {
            if (_isOn && _group != null && !_group.AllowSwitchOff)
            {
                return;
            }
            _isOn = !_isOn;
            OnChange(this);
        }

        public T GetBeh<T>() where T : ExtMonoBeh
        {
            var c = _active.GetComponent<T>();
            if (c != null)
            {
                return c;
            }
            return _notActive.GetComponent<T>();
        }

        public T GetCurBeh<T>() where T : ExtMonoBeh
        {
            return IsOn ? _active.GetComponent<T>() : _notActive.GetComponent<T>();
        }
    }
}
