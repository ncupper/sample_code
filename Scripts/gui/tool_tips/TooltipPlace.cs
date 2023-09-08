using loader;

using misc;

using UnityEngine;
using UnityEngine.EventSystems;

using Zenject;
namespace gui.tool_tips
{
    public class TooltipPlace : ExtMonoBeh, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TooltipView _prefab;

        private TooltipView.Factory _factory;
        private bool _isOver;
        private float _timer;
        private TooltipView _view;

        private void Update()
        {
            if (_isOver && (_view == null || !_view.Visible))
            {
                _timer += Time.deltaTime;
                if (_timer > DataStorage.Options.TooltipDelay)
                {
                    if (_view == null)
                    {
                        _view = _factory.Create(_prefab, Self);
                    }
                    _view.Setup(this);
                    _view.Visible = true;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isOver = true;
            _timer = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isOver = false;
            if (_view != null && _view.Visible)
            {
                _view.Visible = false;
            }
        }

        [Inject]
        public void Construct(TooltipView.Factory factory)
        {
            _factory = factory;
        }

        protected override void OnDisabled()
        {
            _isOver = false;
            if (_view != null && _view.Visible)
            {
                _view.Visible = false;
            }
        }
    }
}
