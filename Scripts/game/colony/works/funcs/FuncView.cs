using misc;

using UnityEngine;
namespace game.colony.works.funcs
{
    public class FuncView : ExtMonoBeh
    {
        [SerializeField] public FuncState State;

        public void DoFinish()
        {
            DestroyImmediate(this);
        }
    }
}
