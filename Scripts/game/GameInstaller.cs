using game.colony;
using game.wagon_buildings;

using Zenject;
namespace game
{
    public class GameInstaller : MonoInstaller<GameInstaller>
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<TrainCameraController.Changed>().OptionalSubscriber();

            Container.Bind<Colony>()
                     .FromNewComponentOnNewGameObject()
                     .WithGameObjectName("Colony")
                     .AsSingle();
            Container.Bind<MouseSelectionHandler>()
                     .FromNewComponentOnNewGameObject()
                     .WithGameObjectName("SelectionHandler")
                     .AsSingle();
            Container.Bind<WagonBuildingDragger>()
                     .FromNewComponentOnNewGameObject()
                     .WithGameObjectName("WagonBuildingDragger")
                     .AsSingle();
        }
    }
}
