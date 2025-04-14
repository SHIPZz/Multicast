using CodeBase.UI.WordSlots;

namespace CodeBase.UI.Cluster.Services.Placement
{
    public interface IClusterPlacement
    {
        bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot);
        void ResetCluster(ClusterItem cluster);
        void Clear();
    }
}