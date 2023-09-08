using UnityEngine;

using Zenject;
namespace misc
{
    public static class ZenjectExtensions
    {
        public static T Resolve<T>()
        {
            return Application.isPlaying
                ? ProjectContext.Instance.Container.Resolve<T>()
                : StaticContext.Container.Resolve<T>();
        }

        public static T ResolveInScene<T>()
        {
            var sceneContext = Object.FindObjectOfType<SceneContext>();
            return sceneContext.Container.Resolve<T>();
        }
    }
}
