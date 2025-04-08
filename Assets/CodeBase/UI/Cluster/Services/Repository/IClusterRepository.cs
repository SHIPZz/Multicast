using System.Collections.Generic;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;

namespace CodeBase.UI.Cluster.Services.Repository
{
    public interface IClusterRepository : IProgressWatcher
    {
        void RegisterCluster(ClusterItem clusterItem);
        void AddAvailableClusters(IEnumerable<ClusterModel> clusters);
        IEnumerable<ClusterModel> GetAvailableClusters();
        IEnumerable<ClusterModel> GetPlacedClusters();
        bool TryGetCluster(ClusterModel cluster, out ClusterItem clusterItem);
        void MarkClusterAsPlaced(ClusterModel cluster);
        void MarkClusterAsAvailable(ClusterModel cluster);
        void Clear();
        IEnumerable<ClusterModel> GetAllClusters();
    }
}