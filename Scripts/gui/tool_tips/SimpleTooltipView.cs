using misc;

using TMPro;

using UnityEngine;
namespace gui.tool_tips
{
    internal class SimpleTooltipView : TooltipView
    {
        [SerializeField] private TextMeshProUGUI _text;

        public override void Setup(TooltipPlace place)
        {
            base.Setup(place);
            _text.text = Lang.Get("tt_" + place.name);
        }
    }
}
