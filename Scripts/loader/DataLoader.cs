using System.Threading.Tasks;

using game;
using game.wagon_buildings;

using gui;

using loader.database;

using misc;
using misc.components.storages;

using UnityEngine;

using Zenject;
#pragma warning disable 649

namespace loader
{
    public class DataLoader : ExtMonoBeh
    {
        [SerializeField] private WagonBuildingStorage _wagonBuildingStorage;
        [SerializeField] private ResourceBoxStorage _resourceBoxStorage;
        private IconsCamera _iconsCamera;

        public UniAsyncStorage WagonBuildingStorage => _wagonBuildingStorage.Storage;
        public UniStorage BoxStorage => _resourceBoxStorage.Storage;
        public UniStorage PropsStorage
        {
            get;
            private set;
        }

        [Inject]
        public void Construct(UniStorage.Factory factory, UniAsyncStorage.Factory asyncFactory, IconsCamera iconsCamera)
        {
            PropsStorage = new UniStorage(factory);
            _wagonBuildingStorage.Storage = new UniAsyncStorage(asyncFactory);
            _resourceBoxStorage.Storage = new UniStorage(factory);

            _iconsCamera = iconsCamera;
        }

        public async Task Load()
        {
            base.OnStart();

            await DataStorage.Load();
            LoadIcons();
        }

        private void LoadIcons()
        {
            for (var i = 0; i < DataStorage.Resources.Items.Length; ++i)
            {
                DbResource res = DataStorage.Resources.Items[i];
                if (!string.IsNullOrEmpty(res.Icon))
                {
                    _iconsCamera.Setup(ResStorage.GetPrefab<ExtMonoBeh>(res.Icon), res);
                }
            }
            for (var i = 0; i < DataStorage.WagonBuildings.Items.Length; ++i)
            {
                DbWagonBuilding res = DataStorage.WagonBuildings.Items[i];
                if (!string.IsNullOrEmpty(res.Icon))
                {
                    _iconsCamera.Setup(ResStorage.GetPrefab<ExtMonoBeh>(res.Icon), res);
                }
            }
        }
    }
}
