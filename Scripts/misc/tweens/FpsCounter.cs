using UnityEngine;
namespace misc.tweens
{
    public class FpsCounter : ExtMonoBeh
    {
        private float _dt;
        private int _frames;
        private float _skipTimer;

        public int Fps { get; private set; }

        public int MinFps { get; private set; }

        public int MaxFps { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            _skipTimer = 0.5f;
        }

        protected void UpdateFps()
        {
            if (_skipTimer > 0)
            {
                _skipTimer -= Time.deltaTime;
                return;
            }

            _dt += Time.deltaTime;
            ++_frames;
            if (_dt > 1.0f)
            {
                Fps = Mathf.RoundToInt(_frames / _dt);
                _dt = 0;
                _frames = 0;
                if (MinFps == 0 || MinFps > Fps)
                {
                    MinFps = Fps;
                }
                if (MaxFps < Fps)
                {
                    MaxFps = Fps;
                }
            }
        }
    }
}
