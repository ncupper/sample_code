using misc;

using UnityEngine;
namespace gui.pers
{
    internal class ExProgressBar : ExtMonoBeh
    {
        [SerializeField]
        private RectTransform _bar;

        public int Value
        {
            get => Mathf.RoundToInt(_bar.sizeDelta.x / SelfRect.sizeDelta.x * 100);
            set => _bar.sizeDelta = new Vector2(SelfRect.sizeDelta.x * value / 100.0f, 0);
        }
    }
}
