using System.Collections;
using System.Collections.Generic;

using game;

using loader.database;

using misc;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
namespace gui
{
    [RequireComponent(typeof(Camera))]
    public class IconsCamera : ExtMonoBeh
    {
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _pivotPersHead;

        private readonly Queue<KeyValuePair<ExtMonoBeh, RenderIconContainer>> _queue =
            new Queue<KeyValuePair<ExtMonoBeh, RenderIconContainer>>();

        private Camera _camera;

        private Coroutine _queueWork;

        protected override void OnAwake()
        {
            base.OnAwake();

            _camera = GetComponent<Camera>();
        }

        public void Setup(ExtMonoBeh prefab, RawImage image)
        {
            StartCoroutine(SetupCoroutine(prefab, image));
        }
        public void Setup(ExtMonoBeh prefab, RenderIconContainer container)
        {
            _queue.Enqueue(new KeyValuePair<ExtMonoBeh, RenderIconContainer>(prefab, container));
            if (_queueWork == null)
            {
                _queueWork = StartCoroutine(SetupQueue());
            }
        }

        private IEnumerator SetupCoroutine(ExtMonoBeh prefab, RawImage image)
        {
            RenderTexture oldTexture = _camera.targetTexture;

            Transform parent = prefab.GetComponent<WorkerView>() != null ? _pivotPersHead : _pivot;
            var s = prefab.Clone<ExtMonoBeh>(parent);

            s.Self.localPosition = Vector3.zero;
            RenderTexture t = Instantiate(_camera.targetTexture);
            _camera.targetTexture = t;

            yield return new WaitForEndOfFrame();

            DestroyImmediate(s.gameObject);

            _camera.targetTexture = oldTexture;

            image.texture = t;
        }

        private IEnumerator SetupQueue()
        {
            while (_queue.Count > 0)
            {
                KeyValuePair<ExtMonoBeh, RenderIconContainer> pair = _queue.Dequeue();
                RenderTexture oldTexture = _camera.targetTexture;

                Transform parent = pair.Key.GetComponent<WorkerView>() != null ? _pivotPersHead : _pivot;

                var s = pair.Key.Clone<ExtMonoBeh>(parent);

                if (parent == _pivotPersHead)
                {
                    s.GetComponent<NavMeshAgent>().enabled = false;
                    s.transform.localPosition = Vector3.zero;
                }

                RenderTexture t = Instantiate(_camera.targetTexture);
                _camera.targetTexture = t;

                yield return new WaitForEndOfFrame();

                DestroyImmediate(s.gameObject);

                _camera.targetTexture = oldTexture;

                pair.Value.Texture = t;
            }

            _queueWork = null;
        }
    }
}
