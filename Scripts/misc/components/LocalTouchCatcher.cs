namespace misc.components
{
    internal class LocalTouchCatcher : TouchCatcher
    {
        protected override void OnEnable()
        {
            TouchCatcher current = Current;
            base.OnEnable();
            Current = current;
        }
    }
}
