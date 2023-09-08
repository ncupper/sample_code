using UnityEngine;
using UnityEngine.UI;
namespace misc.components
{
    internal class ScoresText : ExtMonoBeh
    {
        [SerializeField] private float _speed = 5;
        private Animation _anims;
        private float _curVal;

        private string _langKey;
        private float _realVal;
        private Text _view;

        public int Value
        {
            get => Mathf.RoundToInt(_realVal);
            set => _realVal = value;
        }

        private void Update()
        {
            if (!Helper.IsEqual(_curVal, _realVal))
            {
                _curVal = Mathf.Lerp(_curVal, _realVal, _speed * Time.deltaTime);
                if (Mathf.Abs(_curVal - _realVal) < 1)
                {
                    _curVal = _realVal;
                }
                UpdateView();
                if (_anims != null)
                {
                    _anims.Play();
                }
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            _view = GetComponent<Text>();
            var loc = GetComponent<Localized>();
            if (loc != null)
            {
                _langKey = loc.Key;
                loc.enabled = false;
            }
            if (_speed < 0.1f)
            {
                _speed = 0.1f;
            }

            _anims = GetComponent<Animation>();
        }

        public void Setup(int val = 0)
        {
            _curVal = val;
            _realVal = val;
            UpdateView();
        }

        private void UpdateView()
        {
            if (_view == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(_langKey))
            {
                _view.text = Helper.GetSeparatedStr(Mathf.RoundToInt(_curVal));
            }
            else
            {
                _view.text = Lang.Get(_langKey, Mathf.RoundToInt(_curVal));
            }
        }

        public void SetTextVis(bool vis)
        {
            _view.enabled = vis;
        }
    }
}
