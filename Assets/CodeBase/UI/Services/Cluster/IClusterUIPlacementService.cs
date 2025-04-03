using CodeBase.UI.Cluster;

namespace CodeBase.UI.Services.Cluster
{
    public interface IClusterUIPlacementService
    {
        bool TryPlaceCluster(string clusterText, int startIndex);
        void ResetCluster(string clusterText);
        void CheckAndHideFilledClusters();
        void SetClusterItemHolder(ClusterItemHolder clusterItemHolder);
        void OnClusterSelected(ClusterItem clusterItem);
    }
}