using misc.tweens;

using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace misc.components
{
    internal class FpsData : MonoBehaviour
    {
        public static string DebugPrefix;
        [SerializeField] private bool _symLowFps;

        public Text Fps;
        private float _dt;
        private FpsCounter _fpsCounter;

        public static int MinFps => 10;

        private void Start()
        {
            _fpsCounter = Helper.GetRootObject<FpsCounter>();
        }

        private void Update()
        {
            if (_symLowFps)
            {
                long tm = Helper.LocalTimeStampMs;
                long wait = 0;
                while (wait < 100)
                {
                    wait += Helper.LocalTimeStampMs - tm;
                    tm = Helper.LocalTimeStampMs;
                }
            }
            _dt += Time.deltaTime;
            if (_dt > 1.0f)
            {
                if (_fpsCounter != null)
                {
                    Fps.text = DebugPrefix + " fps:" + _fpsCounter.Fps + "(" + _fpsCounter.MinFps + "-" + _fpsCounter.MaxFps + ")";
                }
                _dt = 0.0f;
                transform.SetAsLastSibling();
            }
        }
    }
}
