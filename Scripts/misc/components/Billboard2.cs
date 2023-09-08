using UnityEngine;
namespace misc.components
{
    public class Billboard2 : MonoBehaviour
    {
        private void Update()
        {
            if (Camera.main != null)
            {
                transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
            }
        }
    }
}
