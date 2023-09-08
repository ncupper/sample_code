using misc;
using misc.components;
using misc.managers;

using UnityEngine;
namespace game
{
    [RequireComponent(typeof(Camera))]
    internal class TrainCameraController : ExtMonoBeh
    {
        [SerializeField] private float _keyScrollSpeed;
        [SerializeField] private float _moveElastic = 5;
        [SerializeField] private bool _allowForwardBackward;
        [SerializeField] private Vector3 _zoom;
        [Space(10), SerializeField]
         private Vector3[] _poses;
        [SerializeField] private Vector3[] _angles;
        private int _curPos;

        private Vector3 _speed;

        private void Update()
        {
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.D))
            {
                dir += Vector3.right * _keyScrollSpeed;

            }
            if (Input.GetKey(KeyCode.A))
            {
                dir += Vector3.left * _keyScrollSpeed;
            }

            if (_allowForwardBackward)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    dir += Vector3.forward * _keyScrollSpeed;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    dir += Vector3.back * _keyScrollSpeed;
                }
            }

            if (!Helper.IsEqual(Input.mouseScrollDelta.y, 0) && TouchCatcher.Current.IsOver)
            {
                if (Input.mouseScrollDelta.y > 0.5f)
                {
                    _curPos = Mathf.Clamp(_curPos - 1, 0, 2);
                }
                else
                {
                    _curPos = Mathf.Clamp(_curPos + 1, 0, 2);
                }

                EventBus.Invoke(new Changed(_curPos));
            }

            float dt = Time.unscaledDeltaTime;
            _speed = Vector3.Lerp(_speed, dir, _moveElastic * dt);

            Self.position += _speed * dt;

            LerpToPos(dt);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _curPos = 1;
        }

        private void LerpToPos(float dt)
        {
            Vector3 pos = Self.position;
            pos.y = Mathf.Lerp(pos.y, _poses[_curPos].y, 3 * dt);
            pos.z = Mathf.Lerp(pos.z, _poses[_curPos].z, 3 * dt);
            Self.position = pos;

            Vector3 angles = Self.eulerAngles;
            angles.x = Mathf.Lerp(angles.x, _angles[_curPos].x, 3 * dt);
            Self.eulerAngles = angles;
        }

        public class Changed : EventData
        {
            public Changed(int zoom)
            {
                Zoom = zoom;
            }

            public int Zoom { get; }
        }
    }
}
