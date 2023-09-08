using System;
using System.Threading.Tasks;

using misc;
using misc.managers;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
namespace loader
{
    internal static class SceneSwitcher
    {
        public const int Game = 1;
        public const int Startup = 0;
        public static UnityAction AfterSwitch = delegate {};

        public static UnityAction BeforeSwitch = delegate {};

        public static async Task SwitchSceneAsync(int sceneIndex)
        {
            BeforeSwitch();
            await LoadAsync(sceneIndex);
            AfterSwitch();
        }

        public static void SwitchScene(int sceneIndex)
        {
            BeforeSwitch();
            Load(sceneIndex);
            AfterSwitch();
        }

        private static async Task LoadAsync(int sceneIndex)
        {
            await Task.Yield();

            TimersManager.DoneAll();
            AudioManager.Instance.Clear();
            ResStorage.Clear();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            await Resources.UnloadUnusedAssets().AsTask();

            await SceneManager.LoadSceneAsync(sceneIndex).AsTask();
        }

        private static void Load(int sceneIndex)
        {
            TimersManager.DoneAll();
            AudioManager.Instance.Clear();
            ResStorage.Clear();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            Resources.UnloadUnusedAssets();

            SceneManager.LoadScene(sceneIndex);
        }
    }
}
