using misc.components;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace misc.tweens
{
    internal class TweenMoveTo : Tween
    {
        private readonly Transform _target;
        private readonly Transform _to;
        private Vector3 _endPos;
        private Vector3 _startPos;

        public TweenMoveTo(float tm, Transform target, Transform to) : base(tm)
        {
            _target = target;
            _to = to;
        }

        public TweenMoveTo(float tm, Transform target, Vector3 to) : base(tm)
        {
            _target = target;
            _endPos = to;
        }

        public override void DoAlive()
        {
            base.DoAlive();
            _startPos = _target.position;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            if (_to != null)
            {
                _endPos = _to.position;
            }

            if (_target != null)
            {
                _target.position = IsDone ? _endPos : Vector3.Lerp(_startPos, _endPos, (Duration - Timer) / Duration);
            }
        }
    }

    internal class TweenMoveToLocal : Tween
    {
        private readonly Transform _target;
        private readonly Transform _to;
        private Vector2 _endPos;
        private Vector2 _startPos;

        public TweenMoveToLocal(float tm, Transform target, Transform to) : base(tm)
        {
            _target = target;
            _to = to;
        }

        public TweenMoveToLocal(float tm, Transform target, Vector2 to) : base(tm)
        {
            _target = target;
            _endPos = to;
        }

        public override void DoAlive()
        {
            base.DoAlive();
            _startPos = _target.localPosition;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            if (_to != null)
            {
                _endPos = _to.localPosition;
            }

            if (_target != null)
            {
                _target.localPosition = IsDone ? _endPos : Vector2.Lerp(_startPos, _endPos, (Duration - Timer) / Duration);
            }
        }
    }

    internal class TweenWait : Tween
    {
        private readonly UnityAction _cb;
        private readonly UnityAction<ExtMonoBeh> _cbExt;
        private readonly UnityAction<GameObject> _cbObj;
        private readonly ExtMonoBeh _ext;
        private readonly GameObject _obj;

        public TweenWait(float tm, UnityAction<GameObject> cbObj = null, GameObject obj = null) : base(tm)
        {
            _cbObj = cbObj;
            _obj = obj;
        }

        public TweenWait(float tm, UnityAction cb = null) : base(tm)
        {
            _cb = cb;
        }

        public TweenWait(float tm, UnityAction<ExtMonoBeh> cbExt, ExtMonoBeh ext) : base(tm)
        {
            _cbExt = cbExt;
            _ext = ext;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            if (IsDone)
            {
                _cbObj?.Invoke(_obj);
                _cbExt?.Invoke(_ext);
                _cb?.Invoke();
            }
        }
    }

    internal class TweenHardWait : Tween
    {
        private readonly UnityAction _cb;
        private readonly UnityAction<ExtMonoBeh> _cbExt;
        private readonly UnityAction<GameObject> _cbObj;
        private readonly ExtMonoBeh _ext;
        private readonly GameObject _obj;

        public TweenHardWait(float tm, UnityAction<GameObject> cbObj = null, GameObject obj = null) : base(tm)
        {
            _cbObj = cbObj;
            _obj = obj;
        }

        public TweenHardWait(float tm, UnityAction cb = null) : base(tm)
        {
            _cb = cb;
        }

        public TweenHardWait(float tm, UnityAction<ExtMonoBeh> cbExt, ExtMonoBeh ext) : base(tm)
        {
            _cbExt = cbExt;
            _ext = ext;
        }

        public override void DoUpdate(float dt)
        {
            if (!IsAlive)
            {
                DoAlive();
            }

            if (dt > 1)
            {
                dt = 1;
            }

            Timer -= dt;
            if (Timer < 0.001f)
            {
                Timer = -1;
            }
            if (IsDone)
            {
                _cbObj?.Invoke(_obj);
                _cbExt?.Invoke(_ext);
                _cb?.Invoke();
            }
        }
    }

    internal class TweenFade : Tween
    {
        private readonly float _alpha;
        private readonly Graphic _target;
        private readonly CanvasGroup _targetGroup;
        private float _startAlpha;

        public TweenFade(float tm, Graphic target, float alpha) : base(tm)
        {
            _target = target;
            _alpha = alpha;
        }

        public TweenFade(float tm, CanvasGroup target, float alpha) : base(tm)
        {
            _targetGroup = target;
            _alpha = alpha;
        }

        public override void DoAlive()
        {
            base.DoAlive();
            if (_target != null)
            {
                _target.CrossFadeAlpha(_alpha, Duration, false);
            }
            if (_targetGroup != null)
            {
                _startAlpha = _targetGroup.alpha;
            }
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            if (_targetGroup != null)
            {
                _targetGroup.alpha = IsDone ? _alpha : Mathf.Lerp(_startAlpha, _alpha, (Duration - Timer) / Duration);
            }
        }
    }

    internal class TweenDayTime : Tween
    {
        private readonly float _endDayTime;
        private float _startDayTime;

        public TweenDayTime(float tm, float to) : base(tm)
        {
            _endDayTime = to;
        }

        public override void DoAlive()
        {
            base.DoAlive();
            _startDayTime = DayTimeSettings.CurDayTime;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            if (IsDone)
            {
                DayTimeSettings.ApplyClock(_endDayTime);
            }
            else
            {
                float e = _endDayTime < _startDayTime ? _endDayTime + 24.0f : _endDayTime;
                e = _startDayTime + (Duration - Timer) * (e - _startDayTime) / Duration;
                if (Helper.IsEqual(e, 24))
                {
                    e = 0;
                }
                if (e > 24.0f)
                {
                    e -= 24.0f;
                }
                DayTimeSettings.ApplyClock(e);
            }
        }
    }

    internal class TweenRotate : Tween
    {
        private readonly Transform _target;
        private float _endPos;
        private float _startPos;

        public TweenRotate(float tm, Transform target, float angle) : base(tm)
        {
            _target = target;
            _endPos = angle;
        }

        public override void DoAlive()
        {
            base.DoAlive();
            _startPos = _target.localEulerAngles.z;
            _endPos += _startPos;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            Vector3 r = _target.localEulerAngles;
            r.z = IsDone ? _endPos : Mathf.Lerp(_startPos, _endPos, (Duration - Timer) / Duration);
            _target.localEulerAngles = r;
        }
    }

    internal class TweenRotateY : Tween
    {
        private readonly Transform _target;
        private float _endPos;
        private float _startPos;

        public TweenRotateY(float tm, Transform target, float angle) : base(tm)
        {
            _target = target;
            _endPos = angle;
        }

        public override void DoAlive()
        {
            base.DoAlive();
            _startPos = _target.localEulerAngles.y;
            _endPos += _startPos;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            Vector3 r = _target.localEulerAngles;
            r.y = IsDone ? _endPos : Mathf.Lerp(_startPos, _endPos, (Duration - Timer) / Duration);
            _target.localEulerAngles = r;
        }
    }

    internal class TweenColor : Tween
    {
        private readonly Color _endColor;
        private readonly Graphic _targetImg;
        private readonly SpriteRenderer _targetSprite;
        private Color _startColor;

        public TweenColor(float tm, Graphic target, Color c) : base(tm)
        {
            _endColor = c;
            _targetImg = target;
        }
        public TweenColor(float tm, SpriteRenderer target, Color c) : base(tm)
        {
            _endColor = c;
            _targetSprite = target;
        }

        public override void DoAlive()
        {
            base.DoAlive();
            if (_targetImg != null)
            {
                _startColor = _targetImg.color;
            }
            else if (_targetSprite != null)
            {
                _startColor = _targetSprite.color;
            }
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);

            if (_targetImg != null)
            {
                _targetImg.color = IsDone ? _endColor : Color.Lerp(_startColor, _endColor, (Duration - Timer) / Duration);
            }
            else if (_targetSprite != null)
            {
                _targetSprite.color = IsDone ? _endColor : Color.Lerp(_startColor, _endColor, (Duration - Timer) / Duration);
            }
        }
    }

    internal class TweenScale : Tween
    {
        private readonly Vector3 _endPos;
        private readonly Transform _target;
        private Vector3 _startPos;

        public TweenScale(float tm, Transform target, float scale) : base(tm)
        {
            _target = target;
            _endPos = new Vector3(scale, scale, scale);
        }

        public TweenScale(float tm, Transform target, Vector3 scale) : base(tm)
        {
            _target = target;
            _endPos = scale;
        }

        public override void DoAlive()
        {
            base.DoAlive();
            _startPos = _target.localScale;
        }

        public override void DoUpdate(float dt)
        {
            base.DoUpdate(dt);
            _target.localScale = IsDone ? _endPos : Vector3.Lerp(_startPos, _endPos, (Duration - Timer) / Duration);
        }
    }
}
