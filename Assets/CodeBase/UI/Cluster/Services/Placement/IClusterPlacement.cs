using System.Collections.Generic;
using CodeBase.UI.WordSlots;

namespace CodeBase.UI.Cluster.Services.Placement
{
    public interface IClusterPlacement
    {
        void Initialize(int rowCount);
        bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot);
        void ResetCluster(ClusterItem cluster);
        IEnumerable<ClusterItem> GetClustersInRow(int row);
        void Clear();
    }
}