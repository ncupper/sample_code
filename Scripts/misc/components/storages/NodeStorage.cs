using UnityEngine;
#pragma warning disable 649

namespace misc.components.storages
{
    internal class NodeStorage : ExtMonoBeh
    {
        public GameObject[] Nodes;

        public void HideAll()
        {
            for (var i = 0; i < Nodes.Length; ++i)
            {
                if (Nodes[i] != null)
                {
                    Nodes[i].SetActive(false);
                }
            }
        }

        public GameObject ShowNode(string nodeName)
        {
            HideAll();
            for (var i = 0; i < Nodes.Length; ++i)
            {
                if (Nodes[i] != null && Nodes[i].name == nodeName)
                {
                    Nodes[i].SetActive(true);
                    return Nodes[i];
                }
            }

            return null;
        }

        public GameObject ShowNode(int nodeIdx)
        {
            HideAll();
            if (nodeIdx < Nodes.Length && Nodes[nodeIdx] != null)
            {
                Nodes[nodeIdx].SetActive(true);
                return Nodes[nodeIdx];
            }

            return null;
        }
    }
}
