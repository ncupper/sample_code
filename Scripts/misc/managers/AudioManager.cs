using System.Collections.Generic;

using UnityEngine;
namespace misc.managers
{
    internal interface ISoundOptions
    {
        bool Sound { get; set; }
        bool Music { get; set; }
    }

    internal class AudioManager : ExtMonoBeh
    {
        private static AudioManager _instance;

        private readonly List<AudioSource> _audios = new List<AudioSource>();

        private readonly Dictionary<string, AudioClip> _soundsCache = new Dictionary<string, AudioClip>();
        private bool _isSound;
        private AudioSource _music;
        private ISoundOptions _options;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("AudioManager").AddComponent<AudioManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        private void Update()
        {
            if (_isSound != IsSoundOn())
            {
                _isSound = IsSoundOn();
                for (var i = 0; i < _audios.Count; ++i)
                {
                    if (_audios[i].loop && _audios[i].mute != !_isSound)
                    {
                        _audios[i].mute = !_isSound;
                    }
                }
            }
        }

        public void Clear()
        {
            if (_music != null)
            {
                _music.Stop();
            }
            _music = null;
            for (var i = 0; i < _audios.Count; ++i)
            {
                _audios[i].Stop();
                _audios[i].clip = null;
            }
        }

        public void SetOptions(ISoundOptions options)
        {
            _options = options;
            _isSound = IsSoundOn();
        }

        public bool IsMusicOn()
        {
            return _options == null || _options.Music;
        }

        public bool IsSoundOn()
        {
            return _options == null || _options.Sound;
        }

        public void SetMusic(AudioSource music)
        {
            if (_music != null && _music != music)
            {
                _music.Stop();
            }
            _music = music;
            PlayMusic();
            if (IsMusicOn())
            {
                _music.Stop(); // .DOFade(1.0f, 0.5f);
            }
        }

        public void FadeOutMusic()
        {
            if (IsMusicOn() && _music != null)
            {
                _music.Play(); //.DOFade(0, 1.0f);
            }
        }

        public void PlayMusic()
        {
            if (_music == null)
            {
                return;
            }
            if (IsMusicOn())
            {
                if (_music.isPlaying)
                {
                    _music.UnPause();
                }
                else
                {
                    _music.Play();
                }
            }
            else
            {
                _music.Pause();
            }
        }

        public void StopSound(string clip)
        {
            for (var i = 0; i < _audios.Count; ++i)
            {
                if (_audios[i].clip != null && _audios[i].clip.name == clip)
                {
                    _audios[i].Stop();
                }
            }
        }

        public void StopSound(AudioClip clip)
        {
            for (var i = 0; i < _audios.Count; ++i)
            {
                if (_audios[i].clip == clip)
                {
                    if (_audios[i].loop && _audios[i].isPlaying)
                    {
                        _audios[i].Stop();
                        break;
                    }
                    _audios[i].Stop();
                }
            }
        }

        public void PlaySound(string clip, bool loop = false, float delay = 0, int limit = 1)
        {
            if (!IsSoundOn())
            {
                return;
            }

            AudioClip c = ResStorage.GetSound("sounds/" + clip);
            PlaySound(c, loop, delay, limit);
        }
        public void PlayExternalSound(string clipName, bool loop = false, float delay = 0, int limit = 1)
        {
            if (!IsSoundOn())
            {
                return;
            }
#if UNITY_EDITOR
            AudioClip clip;
            if (!_soundsCache.ContainsKey(clipName))
            {
                clip = WavUtility.ToAudioClip(Application.dataPath + "/Sounds/for_sound_build/" + clipName + ".wav", clipName);
                if (clip == null)
                {
                    clip = WavUtility.ToAudioClip(Application.dataPath + "/Sounds/from_sound_build/" + clipName + ".wav",
                        clipName);
                }

                if (clip == null)
                {
                    Helper.LogError("Not found sound: [" + clipName + "]");
                }
                _soundsCache.Add(clipName, clip);
            }
            else
            {
                clip = _soundsCache[clipName];
            }

            if (clip != null)
            {
                PlaySound(clip, loop, delay, limit);
            }
            //Helper.Log("Play sound: " + clipName);
#endif
        }

        public AudioSource PlaySound(AudioClip clip, bool loop = false, float delay = 0, int limit = 1)
        {
            if (clip == null)
            {
                return null;
            }

            //Helper.LogError("Play sound: " + clip.name);

            if (loop)
            {
                int l = limit;
                for (var i = 0; i < _audios.Count; ++i)
                {
                    if (_audios[i].loop && _audios[i].clip == clip)
                    {
                        if (!_audios[i].isPlaying)
                        {
                            _audios[i].PlayDelayed(delay);
                            _audios[i].mute = !IsSoundOn();
                            return _audios[i];
                        }

                        if (--l <= 0)
                        {
                            return null;
                        }
                    }
                }
            }

            if (!IsSoundOn() && !loop)
            {
                return null;
            }

            for (var i = 0; i < _audios.Count; ++i)
            {
                if (_audios[i].isPlaying && !_audios[i].loop && _audios[i].clip == clip)
                {
                    if (--limit <= 0)
                    {
                        return null;
                    }
                }
            }

            for (var i = 0; i < _audios.Count; ++i)
            {
                if (!_audios[i].isPlaying)
                {
                    _audios[i].clip = clip;
                    _audios[i].loop = loop;
                    _audios[i].PlayDelayed(delay);
                    if (loop)
                    {
                        _audios[i].mute = !IsSoundOn();
                    }
                    return _audios[i];
                }
            }

            var newAudio = gameObject.AddComponent<AudioSource>();
            _audios.Add(newAudio);
            newAudio.playOnAwake = false;
            newAudio.clip = clip;
            newAudio.loop = loop;
            if (loop)
            {
                newAudio.mute = !IsSoundOn();
            }
            newAudio.PlayDelayed(delay);
            return newAudio;
        }
    }
}
