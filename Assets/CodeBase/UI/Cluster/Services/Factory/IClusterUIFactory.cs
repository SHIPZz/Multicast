using UnityEngine;

namespace CodeBase.UI.Cluster.Services.Factory
{
    public interface IClusterUIFactory
    {
        ClusterItem CreateClusterItem(Transform parent);
        ClusterItemHolder CreateClusterItemHolder(Transform parent);
    }
}