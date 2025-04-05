using System.Collections.Generic;
using CodeBase.UI.Cluster;

namespace CodeBase.UI.Services.Cluster
{
    public interface IClusterRepository
    {
        void RegisterCluster(ClusterItem clusterItem);
        void AddAvailableClusters(IEnumerable<string> clusters);
        void AddPlacedClusters(IEnumerable<string> clusters);
        IEnumerable<string> GetAvailableClusters();
        IEnumerable<string> GetPlacedClusters();
        bool IsClusterAvailable(string clusterText);
        bool IsClusterPlaced(string clusterText);
        bool TryGetCluster(string clusterText, out ClusterItem clusterItem);
        void MarkClusterAsPlaced(string clusterText);
        void MarkClusterAsAvailable(string clusterText);
        void Clear();
    }
}