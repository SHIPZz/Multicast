using System.Collections.Generic;
using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;

namespace CodeBase.UI.Services.Cluster
{
    public interface IClusterPlacement
    {
        void Initialize(int rowCount);
        bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot);
        void ResetCluster(ClusterItem cluster);
        IEnumerable<ClusterItem> GetClustersInRow(int row);
        void Clear();
        IReadOnlyDictionary<int, Dictionary<int, ClusterItem>> Clusters { get; }
    }
}