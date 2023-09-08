using System.Collections.Generic;

using misc.managers;
namespace misc.tweens
{
    internal class Tween : IHaveUpdate
    {
        protected readonly float Duration;
        protected float Timer;

        protected Tween(float tm)
        {
            Duration = tm;
            Timer = tm;
            TimersManager.Add(this);
        }

        public float CurTimer => Timer;
        public bool IsAlive
        {
            get;
            private set;
        }

        public float Delay => Timer;

        public virtual void DoUpdate(float dt)
        {
            if (!IsAlive)
            {
                DoAlive();
            }

            Timer -= dt;
            if (Timer < 0.001f)
            {
                Timer = -1;
            }
        }

        public bool IsDone => Timer < 0;

        public virtual void DoAlive()
        {
            IsAlive = true;
            Timer = Duration;
        }
    }

    internal class TweenSequence : IHaveUpdate
    {
        private readonly Queue<Tween> _tweens = new Queue<Tween>();

        public TweenSequence()
        {
            TimersManager.Add(this);
        }

        public float Delay => _tweens.Peek().Delay;

        public void DoUpdate(float dt)
        {
            if (IsDone)
            {
                return;
            }
            Tween tween = _tweens.Peek();
            tween.DoUpdate(dt);
            if (tween.IsDone)
            {
                _tweens.Dequeue();
            }
        }

        public bool IsDone => _tweens.Count == 0;

        public TweenSequence Add(Tween tween)
        {
            _tweens.Enqueue(tween);
            if (_tweens.Count == 1)
            {
                tween.DoAlive();
            }
            TimersManager.Remove(tween);
            return this;
        }
    }
}
