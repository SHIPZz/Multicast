using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;

namespace CodeBase.UI.Services.Cluster
{
    public interface IClusterPlacementService
    {
        bool TryPlaceCluster(string clusterText, WordSlotHolder wordSlotHolder, int startIndex);
        void ResetCluster(string clusterText);
        void RegisterClusterItem(string cluster, ClusterItem clusterItem);
        void HideClusterInSlot(WordSlot slot);
    }
}