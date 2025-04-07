using UnityEngine;

namespace CodeBase.UI.Cluster.Services.Factory
{
    public interface IClusterUIFactory
    {
        ClusterItem CreateClusterItem(Transform parent);
        ClusterItemHolder CreateClusterItemHolder(Transform parent);
        GameObject Create(Transform parent, GameObject prefab);
    }
}