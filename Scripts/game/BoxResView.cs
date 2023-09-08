using loader.database;

using misc;

using UnityEngine;
namespace game
{
    public class BoxResView : ExtMonoBeh
    {
        [SerializeField] private ResId _resId;

        public ResId ResId => _resId;
    }
}
