using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
#pragma warning disable 649

namespace misc.components
{
    public class ClickableCollider : ExtMonoBeh
    {
        [SerializeField] private GameObject _clickFx;
        [SerializeField] private string _action;

        private Animatroller _animator;
        private Collider _collider;
        private bool _isDown;
        private Vector3 _mouseDownPos;

        public UnityAction<ClickableCollider> ClickAction = delegate {};

        public Vector3 Point { get; private set; }

        public override bool Visible
        {
            get => base.Visible;
            set
            {
                if (base.Visible != value)
                {
                    base.Visible = value;
                    if (value && _animator != null)
                    {
                        _animator.OnShow();
                    }
                }
            }
        }

        public Bounds Bounds => _collider.bounds;

        protected virtual void Update()
        {
            if (_collider == null)
            {
                return;
            }

            if (_isDown && (_mouseDownPos - Input.mousePosition).sqrMagnitude >
                EventSystem.current.pixelDragThreshold * EventSystem.current.pixelDragThreshold)
            {
                _isDown = false;
                if (_animator != null)
                {
                    _animator.OnPointerUp(null);
                }
            }

            if (_isDown && !TouchCatcher.Current.IsDown && !Input.GetMouseButtonUp(0))
            {
                _isDown = false;
                if (_animator != null)
                {
                    _animator.OnPointerUp(null);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (_collider.Raycast(ray, out RaycastHit _, 2000))
                {
                    bool isDown = _isDown;
                    _isDown = TouchCatcher.Current.IsDown;
                    if (isDown != _isDown && _animator != null)
                    {
                        if (_isDown)
                        {
                            _animator.OnPointerDown(null);
                        }
                        else
                        {
                            _animator.OnPointerUp(null);
                        }
                    }
                    _mouseDownPos = Input.mousePosition;
                }
            }
            if (_isDown && Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (_collider.Raycast(ray, out hit, 2000))
                {
                    Point = hit.point;
                    OnClick();
                }
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            _collider = GetComponent<Collider>();
            if (_collider == null)
            {
                _collider = GetComponentInChildren<Collider>();
            }

            if (_collider != null)
            {
                _animator = _collider.GetComponent<Animatroller>();
            }

            if (_clickFx != null)
            {
                _clickFx.SetActive(false);
            }
        }

        protected virtual void OnClick()
        {
            if (_clickFx != null)
            {
                _clickFx.SetActive(false);
                _clickFx.SetActive(true);
            }
            ClickAction(this);
        }
    }
}
