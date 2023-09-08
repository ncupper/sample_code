using TMPro;

using UnityEngine;
using UnityEngine.UI;
namespace misc.components
{
    [RequireComponent(typeof(Slider))]
    internal class ExSlider : ExtMonoBeh
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private string _prefix;

        private Slider _slider;
        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                _slider.value = _value;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(RefreshText);
            RefreshText(_slider.value);
        }

        private void RefreshText(float val)
        {
            _value = Mathf.RoundToInt(val);
            if (_text != null)
            {
                if (string.IsNullOrEmpty(_prefix))
                {
                    _text.text = _value.ToString();
                }
                else
                {
                    _text.text = _prefix + _value;
                }
            }
        }
    }
}
