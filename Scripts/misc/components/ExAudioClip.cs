using misc.managers;

using UnityEngine;
#pragma warning disable 649

namespace misc.components
{
    internal class ExAudioClip : ExtMonoBeh
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] private float _delay;
        [SerializeField] private bool _loop;
        [SerializeField] private int _limit = 1;
        [SerializeField] private bool _skip = true;
        [SerializeField] private bool _auto = true;

        private AudioSource _audio;

        protected override void OnEnabled()
        {
            if (_skip)
            {
                _skip = false;
                return;
            }

            if (_auto)
            {
                _audio = AudioManager.Instance.PlaySound(_clip, _loop, _delay, _limit);
            }
        }

        protected override void OnDisabled()
        {
            if (_auto && _audio != null && (_loop || _audio.clip == _clip))
            {
                _audio.Stop();
            }
        }

        public void Play()
        {
            _audio = AudioManager.Instance.PlaySound(_clip, _loop, _delay, _limit);
            if (_clip != null)
            {
                Helper.Log("Play clip: " + _clip.name);
            }
        }

        public void Stop()
        {
            if (_audio != null && (_loop || _audio.clip == _clip))
            {
                _audio.Stop();
            }
        }
    }
}
