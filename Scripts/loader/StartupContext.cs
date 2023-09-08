using System.Threading.Tasks;

using misc;
using misc.managers;

using UnityEngine;

using Zenject;
namespace loader
{
    public class StartupContext : MonoBehaviour
    {
        [Inject]
        public async void Construct(DataLoader dataLoader)
        {
            await dataLoader.Load();
            await SceneSwitcher.SwitchSceneAsync(SceneSwitcher.Game);
            await Task.Yield();
            var gui = ZenjectExtensions.ResolveInScene<ScreenSwitcher>();
            gui.SwitchScreen("game");
        }
    }
}
