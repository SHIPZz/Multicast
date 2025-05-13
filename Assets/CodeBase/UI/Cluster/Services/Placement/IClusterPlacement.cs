using CodeBase.UI.WordCells;

namespace CodeBase.UI.Cluster.Services.Placement
{
    public interface IClusterPlacement
    {
        bool TryPlaceCluster(ClusterItem cluster, WordCellView wordCellView);
        void ResetCluster(ClusterItem cluster);
        void Clear();
    }
}