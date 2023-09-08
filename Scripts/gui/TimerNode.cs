using misc;
using misc.components;

using UnityEngine;
using UnityEngine.UI;
namespace gui
{
    internal class TimerNode : ExtMonoBeh
    {
        [SerializeField] private TimerText _time;
        [SerializeField] private Image _progress;

        private float _tm;

        private void Update()
        {
            _tm += Time.deltaTime;
            int tm = Mathf.RoundToInt(_tm);
            if (tm > 1440)
            {
                tm %= 1440;
                _tm -= 1440.0f;
            }
            _progress.fillAmount = tm / 1440.0f;
            _time.Value = tm;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _time.Setup();
        }

        public void ClickPause()
        {
            Time.timeScale = 0;
        }

        public void ClickX1()
        {
            Time.timeScale = 1;
        }

        public void ClickX2()
        {
            Time.timeScale = 2;
        }

        public void ClickX4()
        {
            Time.timeScale = 4;
        }
    }
}
