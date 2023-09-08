using UnityEngine;
#pragma warning disable 414
#pragma warning disable 649

namespace misc.components
{
    internal class SortingLayer : ExtMonoBeh
    {
        [SerializeField] private string _layer = "Default";

        [SerializeField] private int _order;

        private void OnEnable()
        {
            var selfRenderer = GetComponent<Renderer>();
            if (selfRenderer != null)
            {
                selfRenderer.sortingOrder = _order;
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (var i = 0; i < renderers.Length; ++i)
            {
                renderers[i].sortingOrder = _order;
            }
        }
    }
}
