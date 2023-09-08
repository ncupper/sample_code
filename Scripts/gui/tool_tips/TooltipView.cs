using misc;
using misc.managers;

using UnityEngine;

using Zenject;
namespace gui.tool_tips
{
    public class TooltipView : ExtMonoBeh
    {

        private readonly Vector2 _offset = new Vector2(5, 5);
        private Camera _guiCamera;
        private RectTransform _rootRect;

        private void Update()
        {
            Vector3 mp = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rootRect, mp, _guiCamera, out Vector2 pos);
            SelfRect.localPosition = pos + _offset;
            Self.SetAsLastSibling();
        }

        [Inject]
        public void Construct(ScreenSwitcher switcher)
        {
            _rootRect = switcher.TooltipCanvas.transform as RectTransform;
            transform.SetParent(_rootRect, false);
            _guiCamera = switcher.TooltipCanvas.worldCamera;
        }

        public virtual void Setup(TooltipPlace place)
        {
        }

        public class Factory : PlaceholderFactory<TooltipView, Transform, TooltipView>
        {
            private readonly DiContainer _container;

            public Factory(DiContainer container)
            {
                _container = container;
            }

            public override TooltipView Create(TooltipView prefab, Transform root)
            {
                return _container.InstantiatePrefab(prefab, root).GetComponent<TooltipView>();
            }
        }
    }

}
