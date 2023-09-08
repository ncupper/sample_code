using UnityEngine;
using UnityEngine.Events;
namespace misc.components
{
    [ExecuteInEditMode]
    internal class ExToggleGroup : ExtMonoBeh
    {
        [SerializeField] private bool _allowSwitchOff = true;
        private ExToggle _curToggle;
        private ExToggle[] _togles;

        public UnityAction<ExtMonoBeh> OnChange = delegate {};

        public int SelIdx
        {
            get
            {
                for (var i = 0; i < _togles.Length; ++i)
                {
                    if (_togles[i].IsOn)
                    {
                        return i;
                    }
                }
                return -1;
            }
            set
            {
                for (var i = 0; i < _togles.Length; ++i)
                {
                    _togles[i].IsOn = value == i;
                }
            }
        }

        public bool AllowSwitchOff => _allowSwitchOff;

#if UNITY_EDITOR
        private void Update()
        {
            for (var i = 0; i < _togles.Length; ++i)
            {
                _togles[i].OnChange -= OnTogleChange;
            }
            _togles = GetComponentsInChildren<ExToggle>();
            for (var i = 0; i < _togles.Length; ++i)
            {
                _togles[i].OnChange += OnTogleChange;
            }
        }
#endif

        protected override void OnAwake()
        {
            base.OnAwake();
            ReInit();
        }

        public void ReInit()
        {
            if (_togles != null)
            {
                for (var i = 0; i < _togles.Length; ++i)
                {
                    _togles[i].OnChange -= OnTogleChange;
                }
            }
            _togles = GetComponentsInChildren<ExToggle>();
            for (var i = 0; i < _togles.Length; ++i)
            {
                _togles[i].OnChange += OnTogleChange;
            }

            if (!_allowSwitchOff && _togles.Length > 0)
            {
                _curToggle = _togles[0];
            }
        }

        private void OnTogleChange(ExToggle toogle)
        {
            if (toogle.IsOn)
            {
                _curToggle = toogle;
                for (var i = 0; i < _togles.Length; ++i)
                {
                    if (_togles[i].IsOn && _togles[i] != toogle)
                    {
                        _togles[i].IsOn = false;
                    }
                }

                OnChange(toogle);
            }
            else if (toogle == _curToggle)
            {
                if (!_allowSwitchOff)
                {
                    toogle.IsOn = true;
                }
                OnChange(toogle);
            }
        }

        public void TurnAllOff()
        {
            for (var i = 0; i < _togles.Length; ++i)
            {
                _togles[i].IsOn = false;
            }
        }
    }
}
