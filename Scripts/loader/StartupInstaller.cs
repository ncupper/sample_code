using gui;

using misc;
using misc.components.storages;

using UnityEngine;
using UnityEngine.AddressableAssets;

using Zenject;
namespace loader
{
    public class StartupInstaller : ScriptableObjectInstaller<StartupInstaller>
    {
        [SerializeField] private DataLoader _dataLoader;
        [SerializeField] private IconsCamera _iconsCamera;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.BindFactory<ExtMonoBeh, Transform, ExtMonoBeh, UniStorage.Factory>()
                     .FromFactory<PlaceholderFactory<ExtMonoBeh, Transform, ExtMonoBeh>>();
            Container.BindFactory<AssetReference, Transform, ExtMonoBeh, UniAsyncStorage.Factory>()
                     .FromFactory<PlaceholderFactory<AssetReference, Transform, ExtMonoBeh>>();

            Container.Bind<PerformanceMeter>()
                     .FromNewComponentOnNewGameObject()
                     .WithGameObjectName("PerformanceMeter")
                     .AsSingle();
            Container.Bind<DataLoader>()
                     .FromComponentInNewPrefab(_dataLoader)
                     .WithGameObjectName(_dataLoader.name)
                     .AsSingle();
            Container.Bind<IconsCamera>()
                     .FromComponentInNewPrefab(_iconsCamera)
                     .WithGameObjectName(_iconsCamera.name)
                     .AsSingle();
        }
    }
}
