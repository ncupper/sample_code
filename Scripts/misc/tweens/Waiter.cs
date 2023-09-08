namespace misc.tweens
{
    internal class Waiter
    {
        private float _timer = -1f;

        public bool CheckTimer(float dt)
        {
            _timer -= dt;
            if (_timer < -0.1f)
            {
                _timer = -1;
            }
            return _timer < 0;
        }

        public void Setup(float duration)
        {
            _timer = duration;
        }
    }
}
