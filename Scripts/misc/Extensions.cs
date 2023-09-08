using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
namespace misc
{
    public static class Extensions
    {
        public static void SetOnClick(this Button go, Action<GameObject> handler)
        {
            go.onClick.AddListener(() =>
            {
                if (handler != null)
                {
                    handler(go.gameObject);
                }
            });
        }

        public static void SetOnClick(this GameObject go, Action<GameObject> handler)
        {
            var eventHandler = go.GetComponent<Button>();

            if (eventHandler == null)
            {
                eventHandler = go.AddComponent<Button>();
            }

            eventHandler.onClick.AddListener(() =>
            {
                if (handler != null)
                {
                    handler(go);
                }
            });
        }

        public static T With<T>(this T self, Action<T> set)
        {
            set.Invoke(self);
            return self;
        }

        public static T With<T>(this T self, Action<T> apply, Func<bool> when)
        {
            if (when())
            {
                apply?.Invoke(self);
            }

            return self;
        }

        public static T With<T>(this T self, Action<T> apply, bool when)
        {
            if (when)
            {
                apply?.Invoke(self);
            }

            return self;
        }

        public static Task AsTask(this AsyncOperation asyncOperation)
        {
            var task = new TaskCompletionSource<bool>();
            asyncOperation.completed += x => { task.SetResult(true); };
            return task.Task;
        }
    }
}
