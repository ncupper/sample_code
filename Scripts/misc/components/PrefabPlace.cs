using UnityEngine;
#pragma warning disable 649

namespace misc.components
{
    internal class PrefabPlace : ExtMonoBeh
    {
        [SerializeField] private GameObject _prefab;

        public GameObject View
        {
            get;
            private set;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            View = Helper.Clone(_prefab, Self);
            View.SetActive(true);
            var rectTransform = View.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                View.transform.localPosition = Vector3.zero;
                View.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
