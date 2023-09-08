using misc.managers;

using UnityEngine;
namespace misc.components
{
    internal class ExSound : ExtMonoBeh
    {
        private AudioSource _audio;

        private void Update()
        {
            if (_audio.mute == AudioManager.Instance.IsSoundOn())
            {
                _audio.mute = !AudioManager.Instance.IsSoundOn();
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _audio = GetComponent<AudioSource>();
            Update();
        }
    }
}
