using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#pragma warning disable 649

namespace misc.components
{
    [ExecuteAlways]
    internal class AnimSprite : ExtMonoBeh
    {
        public Sprite[] Sprites;
        public float Speed;
        [SerializeField] private bool _autoStart = true;
        [SerializeField] private bool _playOnce;
        [SerializeField] private int _finishCadr = 0;

        [Space(10), SerializeField]
         private GameObject _hideObject = null;
        [SerializeField] private int _hideWhileCadr = 0;

        [Space(10), SerializeField]
         private bool _randomCadr = false;
        private int _curSprite;
        private float _curTime;

        private Image _image;

        private UnityAction _onFinish;
        private bool _paused;
        private SpriteRenderer _sprite;

        public AnimSprite Link
        {
            get;
        }

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                if (!value && Link != null)
                {
                    Link.Visible = false;
                }
            }
        }

        private void Update()
        {
            if (_paused)
            {
                return;
            }
            _curTime += Time.deltaTime;
            if (_curTime > Speed && Sprites != null && Sprites.Length > 0)
            {
                _curTime -= Speed;
                int oldIdx = _curSprite;
                _curSprite = (_curSprite + 1) % Sprites.Length;
                UpdateSprite();

                if (Link != null)
                {
                    Link._curSprite = _curSprite;
                    Link.UpdateSprite();
                }
                if (_curSprite < oldIdx && _playOnce)
                {
                    Stop();
                    if (_onFinish != null)
                    {
                        _onFinish();
                        _onFinish = null;
                    }
                }
            }
        }

        private void OnEnable()
        {
            if (_autoStart)
            {
                _curSprite = _randomCadr && !_playOnce ? Random.Range(0, Sprites.Length) : 0;
                _paused = false;
                UpdateSprite();
            }
            else if (_randomCadr)
            {
                ShowCadr(Random.Range(0, Sprites.Length));
            }
        }

        private void OnDisable()
        {
            if (_hideObject != null)
            {
                _hideObject.SetActive(true);
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _image = GetComponent<Image>();
            _sprite = GetComponent<SpriteRenderer>();
            if (Sprites != null && Sprites.Length == 0 || !_autoStart)
            {
                _paused = true;
            }

            UpdateSprite();
        }

        private void UpdateSprite()
        {
            if (Sprites == null || _curSprite < 0 || _curSprite >= Sprites.Length)
            {
                return;
            }

            if (_image != null)
            {
                _image.sprite = Sprites[_curSprite];
            }
            else if (_sprite != null)
            {
                _sprite.sprite = Sprites[_curSprite];
            }
            if (_image != null)
            {
                _image.enabled = _image.sprite != null;
            }
            else if (_sprite != null)
            {
                _sprite.enabled = _sprite.sprite != null;
            }

            if (_hideObject != null)
            {
                _hideObject.SetActive(_curSprite >= _hideWhileCadr);
            }
        }

        public void Play()
        {
            Visible = true;
            _paused = false;
            _playOnce = false;
            _curSprite = 0;
            _curTime = 0;
            UpdateSprite();
        }

        public void PlayOnce(UnityAction onFinish)
        {
            _onFinish = onFinish;
            Visible = true;
            _paused = false;
            _playOnce = true;
            _curSprite = 0;
            _curTime = 0;
            UpdateSprite();
        }

        public void Stop()
        {
            _paused = true;
            _curSprite = _finishCadr;
            _curTime = 0;
            UpdateSprite();
        }

        public void StopLoop()
        {
            _playOnce = true;
        }

        public void ShowCadr(int cadrIdx)
        {
            _paused = true;
            _curSprite = cadrIdx;
            _curTime = 0;
            UpdateSprite();
        }
    }
}
