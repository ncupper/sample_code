using System.Collections;

using misc.managers;
using misc.tweens;

using UnityEngine;
using UnityEngine.Events;
namespace misc.components
{
#pragma warning disable 649

    internal class FlyBezierItem : ExtMonoBeh
    {
        [SerializeField] private Transform _object;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private float _delay;
        [SerializeField] private float _flyTime;
        [SerializeField] private bool _autoStart;
        [SerializeField] private bool _autoHide;

        [SerializeField] private float _xCoef1 = 0.2f;
        [SerializeField] private float _yCoef1 = 0.1f;
        [SerializeField] private float _xCoef2 = 1.0f;
        [SerializeField] private float _yCoef2 = 0.3f;

        [Space(10), SerializeField]
         private bool _usePortraitSettings;
        [SerializeField] private float _xPortCoef1 = 0.2f;
        [SerializeField] private float _yPortCoef1 = 0.1f;
        [SerializeField] private float _xPortCoef2 = 1.0f;
        [SerializeField] private float _yPortCoef2 = 0.3f;
        private readonly BezierCurve _bezierCurve = new BezierCurve();
        private Transform _from;

        private bool _isLandscape;
        private float _speed = 1;
        private bool _stopped;

        private float _time = -1;
        private Transform _to;
        private TrailRenderer _trail;
        private TweenWait _wait;

        public UnityAction<FlyBezierItem> OnAction = delegate {};

        public GameObject Object => _object.gameObject;

        public float Delay
        {
            get => _delay;
            set => _delay = value;
        }

        public bool IsFly => _time > -0.5f || _wait != null;

        private void Update()
        {
            if (_time > -0.5f)
            {
                _time -= Time.deltaTime * _speed;
                if (_time < 0)
                {
                    _time = -1;
                    _wait = null;
                    _object.position = _to.position;
                    OnAction(this);
                    if (_autoHide)
                    {
                        Object.SetActive(false);
                    }
                }
                else
                {
                    _object.position = _bezierCurve.GetPointAtTime(1.0f - _time);

                    if (_isLandscape != Screen.width > Screen.height)
                    {
                        if (_usePortraitSettings && Screen.height > Screen.width)
                        {
                            _bezierCurve.Init(_from.position, _to.position, _xPortCoef1, _yPortCoef1, _xPortCoef2, _yPortCoef2);
                        }
                        else
                        {
                            _bezierCurve.Init(_from.position, _to.position, _xCoef1, _yCoef1, _xCoef2, _yCoef2);
                        }

                        _isLandscape = Screen.width > Screen.height;

                        _object.position = (_object.position + _bezierCurve.GetPointAtTime(1.0f - _time)) * 0.5f;
                    }
                }
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            if (_object == null)
            {
                _object = Self;
            }

            _trail = _object.gameObject.GetComponent<TrailRenderer>();
            _stopped = false;
        }

        protected override void OnEnabled()
        {
            if (_autoStart && _object != null && _startPoint != null)
            {
                Run();
            }
        }

        public IEnumerator ResetTrail()
        {
            if (_trail != null)
            {
                _object.gameObject.SetActive(true);
                float trailTime = _trail.time;
                _trail.time = -1;
                yield return new WaitForEndOfFrame();
                _trail.time = trailTime;
            }
        }

        public void Run()
        {
            _stopped = false;
            _object.position = _startPoint.position;
            if (!_autoStart)
            {
                _object.gameObject.SetActive(false);
            }
            if (_delay > 0.001f)
            {
                _wait = new TweenWait(_delay, () =>
                {
                    if (!_stopped)
                    {
                        _object.gameObject.SetActive(true);
                        Setup(_startPoint, Self, _flyTime);
                    }
                });
            }
            else
            {
                _object.gameObject.SetActive(true);
                Setup(_startPoint, Self, _flyTime);
            }
        }

        public void Run(float delay, Transform to)
        {
            Self.position = to.position;
            _delay = delay;
            Run();
        }

        public void Stop()
        {
            _time = -1;
            _object.gameObject.SetActive(false);
            _stopped = true;
            if (_wait != null)
            {
                TimersManager.Remove(_wait);
                _wait = null;
            }
        }

        public void ForceFinish()
        {
            Stop();
            _object.position = Self.position;
            _object.gameObject.SetActive(true);
        }

        public void Setup(Transform from, Transform to, float speed = 1)
        {
            _time = 1.0f;
            _speed = speed;

            if (from == null)
            {
                from = _startPoint;
            }

            _to = to;
            _from = from;
            if (_usePortraitSettings && Screen.height > Screen.width)
            {
                _bezierCurve.Init(_from.position, _to.position, _xPortCoef1, _yPortCoef1, _xPortCoef2, _yPortCoef2);
            }
            else
            {
                _bezierCurve.Init(_from.position, _to.position, _xCoef1, _yCoef1, _xCoef2, _yCoef2);
            }

            _isLandscape = Screen.width > Screen.height;
        }

        public void Setup(Transform to, float speed = 1)
        {
            _time = 1.0f;
            _speed = speed;

            _to = to;
            if (_usePortraitSettings && Screen.height > Screen.width)
            {
                _bezierCurve.Init(_startPoint.position, _to.position, _xPortCoef1, _yPortCoef1, _xPortCoef2, _yPortCoef2);
            }
            else
            {
                _bezierCurve.Init(_startPoint.position, _to.position, _xCoef1, _yCoef1, _xCoef2, _yCoef2);
            }
        }

        public void SetStartPoint(Transform startPoint)
        {
            _startPoint = startPoint;
        }
    }

    internal class BezierCurve
    {
        private readonly Vector3[] _p = new Vector3[4];

        public void Init(Vector3 from, Vector3 to, float x1, float y1, float x2, float y2)
        {
            if (Screen.width >= Screen.height)
            {
                x1 *= 0.4f;
                x2 *= 0.4f;
            }
            _p[0] = from;
            _p[1] = from + (from.x < to.x ? Vector3.right * Random.Range(1.0f, 3.0f) * x1
                    : Vector3.left * Random.Range(1.0f, 3.0f) * x1) +
                (from.y < to.y ? Vector3.up * Random.Range(1.0f, 3.0f) * y1 : Vector3.down * Random.Range(1.0f, 3.0f) * y1);
            _p[2] = to + (from.x > to.x ? Vector3.right * Random.Range(1.0f, 3.0f) * x2
                    : Vector3.left * Random.Range(1.0f, 3.0f) * x2) +
                (from.y > to.y ? Vector3.up * Random.Range(1.0f, 3.0f) * y2 : Vector3.down * Random.Range(1.0f, 3.0f) * y2);
            _p[3] = to;
        }

        public Vector3 GetPointAtTime(float t)
        {
            float u = 1.0f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            return uuu * _p[0] + 3 * uu * t * _p[1] + 3 * u * tt * _p[2] + ttt * _p[3];
        }
    }
}
