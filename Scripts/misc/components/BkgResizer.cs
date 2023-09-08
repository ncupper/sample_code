using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace misc.components
{
    [ExecuteAlways]
    internal class BkgResizer : ExtMonoBeh
    {
        [SerializeField] private RectTransform _text;
        [SerializeField] private Vector2 _margin;
        [SerializeField] private Vector2 _minSize;

        private void Update()
        {
            if (_text != null)
            {
                Vector2 sz = _text.sizeDelta + _margin;
                sz.x = Mathf.Max(_minSize.x, sz.x);
                sz.y = Mathf.Max(_minSize.y, sz.y);
                if (!Helper.IsEqual(SelfRect.sizeDelta.x, sz.x) || !Helper.IsEqual(SelfRect.sizeDelta.y, sz.y))
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)SelfRect.parent);
                }
                SelfRect.sizeDelta = sz;
            }
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            Update();
        }
#endif
    }
}
