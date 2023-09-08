using System;

using loader.database;

using misc;
using misc.components.storages;

using UnityEngine;
using UnityEngine.AddressableAssets;
namespace game.wagon_buildings
{
    [Serializable]
    internal struct WagonBuildingLink
    {
        public WagonBuildingId Id;
        public AssetReference AssetRefrence;
    }
    internal class WagonBuildingStorage : ExtMonoBeh
    {

        private static UniAsyncStorage _storage;
        [SerializeField] private WagonBuildingLink[] _references;

        public UniAsyncStorage Storage
        {
            get => _storage;

            set
            {
                _storage = value;
                foreach (WagonBuildingLink buildingLink in _references)
                {
                    _storage.AddPrefab((int)buildingLink.Id, buildingLink.AssetRefrence);
                }
            }
        }
    }
}
