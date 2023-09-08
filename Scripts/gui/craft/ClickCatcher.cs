using misc;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace gui.craft
{
    internal class ClickCatcher : ExtMonoBeh, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Button.ButtonClickedEvent _leftOnClick = new Button.ButtonClickedEvent();
        [SerializeField]
        private Button.ButtonClickedEvent _rightOnClick = new Button.ButtonClickedEvent();
        [SerializeField]
        private Button.ButtonClickedEvent _middleOnClick = new Button.ButtonClickedEvent();
        private readonly bool[] _isDown = new bool[3];

        private bool _isPointerInside;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isDown[(int)eventData.button] = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerInside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerInside = false;
            for (var i = 0; i < _isDown.Length; ++i)
            {
                _isDown[i] = false;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            bool isClick = _isPointerInside && _isDown[(int)eventData.button];
            _isDown[(int)eventData.button] = false;
            if (isClick)
            {
                if ((int)eventData.button == (int)PointerEventData.InputButton.Left)
                {
                    _leftOnClick.Invoke();
                }
                else if ((int)eventData.button == (int)PointerEventData.InputButton.Right)
                {
                    _rightOnClick.Invoke();
                }
                else if ((int)eventData.button == (int)PointerEventData.InputButton.Middle)
                {
                    _middleOnClick.Invoke();
                }
            }
        }
    }
}
