using misc;

using UnityEngine;
namespace loader
{
    internal class PerformanceMeter : ExtMonoBeh
    {
        private int _counter;
        private bool _isPaused;
        private bool _isStarted;

        private float _min;

        private float _skipTimer;
        private int _sumCounter;

        private float _timer;

        private void Start()
        {
            SceneSwitcher.BeforeSwitch += DoStop;
            SceneSwitcher.AfterSwitch += DoStart;
            DoStart();
        }

        private void Update()
        {
            if (!_isStarted || _isPaused)
            {
                return;
            }

            if (_skipTimer > 0)
            {
                _skipTimer -= Time.deltaTime;
                return;
            }

            _timer += Time.deltaTime;
            _counter += 1;

            if (_timer > 1)
            {
                float val = _counter / _timer;
                _counter = 0;
                _timer = 0;
                _sumCounter += 1;
                if (_min < 0 || _min > val)
                {
                    _min = val;
                }
            }
        }

        private void OnDestroy()
        {
            SceneSwitcher.BeforeSwitch -= DoStop;
            SceneSwitcher.AfterSwitch -= DoStart;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            Pause(pauseStatus);
        }

        private void Pause(bool isPaused)
        {
            _isPaused = isPaused;
            _skipTimer = 1;
        }

        private void DoStart()
        {
            _isStarted = true;
            _isPaused = false;
            _timer = 0;
            _counter = 0;
            _skipTimer = 1;
            _sumCounter = 0;
            _min = -1;
        }

        private void DoStop()
        {
            _isStarted = false;
            if (_sumCounter > 0)
            {
                _sumCounter = 0;
            }
        }
    }
}
