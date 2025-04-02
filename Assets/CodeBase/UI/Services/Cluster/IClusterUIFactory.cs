using CodeBase.UI.Cluster;
using CodeBase.UI.Game;
using UnityEngine;

namespace CodeBase.UI.Services.Cluster
{
    public interface IClusterUIFactory
    {
        ClusterItem CreateClusterItem(Transform parent);
        ClusterItemHolder CreateClusterItemHolder(Transform parent);
    }
}