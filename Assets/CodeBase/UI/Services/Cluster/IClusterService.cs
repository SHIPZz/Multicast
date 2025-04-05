using System.Collections.Generic;
using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;

namespace CodeBase.UI.Services.Cluster
{
    public interface IClusterService
    {
        bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot);
        void ResetCluster(ClusterItem cluster);
        void CheckAndHideFilledClusters();
        void OnClusterSelected(ClusterItem clusterItem);
        void SetClusters(IEnumerable<string> clusters);
        IEnumerable<string> GetAvailableClusters();
        void Cleanup();
        void RegisterCreatedCluster(ClusterItem clusterItem);
        void Init();
        IEnumerable<string> GetPlacedClustersFromData();
        void RestorePlacedClusters();
    }
}