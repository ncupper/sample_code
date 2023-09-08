using TMPro;

using UnityEngine;
namespace misc.components
{
    internal enum TextTimerType
    {
        Dotted,
        Readable
    }

    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimerText : ExtMonoBeh
    {
        [SerializeField] private TextTimerType _type = TextTimerType.Dotted;
        [SerializeField] private bool _needHours = true;
        private int _curVal;

        private string _langKey;
        private TextMeshProUGUI _view;

        public int Value
        {
            get => _curVal;
            set
            {
                if (_curVal != value)
                {
                    Setup(value);
                }
            }
        }

        public long ValueLong
        {
            get => _curVal;
            set
            {
                if (_curVal != value)
                {
                    Setup((int)value);
                }
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            _view = GetComponent<TextMeshProUGUI>();
            var loc = GetComponent<Localized>();
            if (loc != null)
            {
                _langKey = loc.Key;
                loc.enabled = false;
            }
        }

        public void Setup(int val = 0)
        {
            if (val < 0)
            {
                val = 0;
            }
            _curVal = val;
            Visible = _curVal >= 0;
            if (Visible)
            {
                UpdateView();
            }
        }

        public void Setup(long val)
        {
            Setup((int)val);
        }

        private void UpdateView()
        {
            if (_view == null)
            {
                OnAwake();
            }

            if (_view == null)
            {
                return;
            }

            if (_type == TextTimerType.Dotted)
            {
                if (string.IsNullOrEmpty(_langKey))
                {
                    _view.text = Helper.GetDottedDeltaTime(_curVal, _needHours);
                }
                else
                {
                    _view.text = Lang.Get(_langKey, Helper.GetDottedDeltaTime(_curVal, _needHours));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_langKey))
                {
                    _view.text = Helper.GetReadableDeltaTime(_curVal);
                }
                else
                {
                    _view.text = Lang.Get(_langKey, Helper.GetReadableDeltaTime(_curVal));
                }
            }
        }

        public void SetText(string val)
        {
            if (_view == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(_langKey))
            {
                _view.text = val;
            }
            else
            {
                _view.text = Lang.Get(_langKey, val);
            }
        }
    }
}
