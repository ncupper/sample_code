using UnityEngine.UI;
namespace misc.components
{
    internal class TouchCatcher : Button
    {
        public static TouchCatcher Current { get; protected set; }

        public bool IsDown => IsPressed();

        public bool IsOver => IsHighlighted();

        protected override void OnEnable()
        {
            base.OnEnable();
            Current = this;
        }
    }
}
