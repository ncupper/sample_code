using misc;
using misc.components.storages;

using UnityEngine;
namespace game
{
    internal class ResourceBoxStorage : ExtMonoBeh
    {

        private static UniStorage _storage;
        [SerializeField] private BoxResView[] _prefabs;

        public UniStorage Storage
        {
            get => _storage;

            set
            {
                _storage = value;
                for (var i = 0; i < _prefabs.Length; ++i)
                {
                    if (_prefabs[i] != null)
                    {
                        _storage.AddPrefab((int)_prefabs[i].ResId, _prefabs[i]);
                    }
                }
            }
        }
    }
}
