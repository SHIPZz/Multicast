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
        IReadOnlyList<ClusterItem> GetClustersInRow(int row);
        IReadOnlyDictionary<int, Dictionary<int, string>> GetClustersByRowAndColumns();
        void SetClustersByRowAndColumns(Dictionary<int, Dictionary<int, string>> clusters);
        bool IsPlacementAvailable(string clusterText, int startIndex);
        void PlaceCluster(ClusterItem cluster, int startIndex);
        void Clear();
        void ClearSlot(int row, int column);
    }
}