using System;

using misc.tweens;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace misc.components
{
#pragma warning disable 649

    [Serializable]
    internal class AnimData
    {
        public AnimationClip Clip;
        public float Delay;
        public float Duration = 0.5f;
        public Sprite Sprite;
        public bool Hide;
    }

    internal class Animatroller : ExtMonoBeh, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private AnimData _idle;
        [SerializeField] private AnimData _onShow;
        [SerializeField] private AnimData _onHide;
        [SerializeField] private AnimData _onDown;
        [SerializeField] private AnimData _onUp;
        [Space(5), SerializeField]
         private Animation _anims;
        [SerializeField] private Image _image;

        private Button _btn;
        private bool _isHidden;

        private void Update()
        {
            if (!_isHidden && _anims != null && _anims.gameObject.activeSelf && !Input.GetMouseButton(0) && !_anims.isPlaying
                && _idle.Clip != null && _anims.clip != _idle.Clip)
            {
                PlayClip(_idle);
            }
        }

        private void OnEnable()
        {
            _isHidden = false;
            if (_anims != null && _idle != null && !_anims.isPlaying && _idle.Clip != null && _anims.clip != _idle.Clip)
            {
                PlayClip(_idle);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_btn != null && !_btn.interactable)
            {
                return;
            }
            if (_anims != null && _onDown.Clip != null)
            {
                PlayClip(_onDown);
                if (_image != null && _onDown.Sprite != null)
                {
                    _image.sprite = _onDown.Sprite;
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_btn != null && !_btn.interactable)
            {
                return;
            }
            if (_anims != null && _onUp.Clip != null)
            {
                PlayClip(_onUp);
                if (_image != null && _onUp.Sprite != null)
                {
                    _image.sprite = _onUp.Sprite;
                }
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (_anims != null)
            {
                if (_idle.Clip != null)
                {
                    _anims.AddClip(_idle.Clip, _idle.Clip.name);
                }
                if (_onShow.Clip != null)
                {
                    _anims.AddClip(_onShow.Clip, _onShow.Clip.name);
                }
                if (_onHide.Clip != null)
                {
                    _anims.AddClip(_onHide.Clip, _onHide.Clip.name);
                }
                if (_onDown.Clip != null)
                {
                    _anims.AddClip(_onDown.Clip, _onDown.Clip.name);
                }
                if (_onUp.Clip != null)
                {
                    _anims.AddClip(_onUp.Clip, _onUp.Clip.name);
                }
                _anims.clip = null;
                _anims.playAutomatically = true;
            }

            _btn = GetComponent<Button>();
        }

        public float OnShow()
        {
            if (!Visible)
            {
                return 0;
            }
            _isHidden = false;
            if (_anims != null && _onShow.Clip != null)
            {
                return PlayClip(_onShow);
            }

            return 0;
        }

        public float OnHide()
        {
            _isHidden = true;
            if (!Visible)
            {
                return 0;
            }
            if (_anims != null && _onHide.Clip != null)
            {
                return PlayClip(_onHide);
            }

            return 0;
        }

        private float PlayClip(AnimData data)
        {
            if (!Helper.IsEqual(data.Duration, 0) && data.Duration > 0)
            {
                if (_anims[data.Clip.name] == null)
                {
                    _anims.AddClip(data.Clip, data.Clip.name);
                }
                _anims[data.Clip.name].speed = data.Duration;
            }
            else
            {
                _anims[data.Clip.name].speed = 1;
            }

            if (data.Hide)
            {
                _anims.gameObject.SetActive(false);
            }

            if (!Helper.IsEqual(data.Delay, 0) && data.Delay > 0)
            {
                new TweenWait(data.Delay, arg =>
                {
                    _anims.clip = data.Clip;
                    _anims.playAutomatically = true;

                    //_anims.gameObject.SetActive(false);
                    _anims.gameObject.SetActive(true);
                    _anims.Play();
                    /*if (data == _idle && data.Clip.isLooping)
                {
                    _anims[_idle.Clip.name].normalizedTime = UnityEngine.Random.Range(0, 1);
                }*/
                }).DoAlive();
            }
            else
            {
                _anims.clip = data.Clip;
                _anims.playAutomatically = true;

                //_anims.gameObject.SetActive(false);
                _anims.gameObject.SetActive(true);
                _anims.Play();
                /*if (data == _idle && data.Clip.isLooping)
            {
                _anims[_idle.Clip.name].normalizedTime = UnityEngine.Random.Range(0, 1);
            }*/
            }
            return data.Clip.length / _anims[data.Clip.name].speed + data.Delay;
        }

        public void TurnOnOff(bool isOn)
        {
            _isHidden = !isOn;
            if (_anims != null)
            {
                _anims.Stop();
                if (isOn && _idle.Clip != null)
                {
                    PlayClip(_idle);
                }
            }
        }
    }
}
