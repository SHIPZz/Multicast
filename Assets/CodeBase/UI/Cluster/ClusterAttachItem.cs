using UnityEngine;

namespace CodeBase.UI.Cluster
{
    public class ClusterAttachItem : MonoBehaviour
    {
        public void Cleanup()
        {
            Destroy(gameObject);
        }
    }
}