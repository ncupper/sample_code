using System.Linq;

using UnityEngine;
namespace misc
{
    public class ExtMonoBeh : MonoBehaviour
    {
        private bool _visible;

        public Transform Self { get; private set; }
        public RectTransform SelfRect { get; private set; }

        public virtual bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                if (gameObject != null && gameObject.activeSelf != _visible)
                {
                    gameObject.SetActive(_visible);
                }
            }
        }

        private void Awake()
        {
            Self = transform;
            SelfRect = GetComponent<RectTransform>();
            _visible = gameObject.activeSelf;
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void OnEnable()
        {
            OnEnabled();
        }

        private void OnDisable()
        {
            OnDisabled();
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnEnabled()
        {
        }

        protected virtual void OnDisabled()
        {
        }

        public GameObject Clone(Transform parent = null)
        {
            bool oldVis = Visible;
            if (!Visible)
            {
                Visible = true;
            }
            GameObject go = Helper.Clone(gameObject, parent);
            Visible = oldVis;
            return go;
        }

        public T Clone<T>(Transform parent = null) where T : Component
        {
            if (GetComponent<T>() != null)
            {
                GameObject go = Clone(parent);
                return go.GetComponent<T>();
            }
            return null;
        }

        public T GetChild<T>(string childName) where T : Component
        {
            T[] ar = gameObject.GetComponentsInChildren<T>();
            return ar.FirstOrDefault(x => x.name == childName);
        }
    }
}
