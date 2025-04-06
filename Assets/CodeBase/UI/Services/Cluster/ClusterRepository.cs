using System;
using System.Collections.Generic;
using CodeBase.Data;
using CodeBase.UI.Cluster;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterRepository : IClusterRepository
    {
        private readonly List<ClusterItem> _createdClusters = new(32);
        private readonly List<ClusterModel> _availableClusters = new();
        private readonly List<ClusterModel> _placedClusters = new();

        public void RegisterCluster(ClusterItem clusterItem) => 
            _createdClusters.Add(clusterItem);

        public void AddAvailableClusters(IEnumerable<ClusterModel> clusters) => 
            _availableClusters.AddRange(clusters);

        public IEnumerable<ClusterModel> GetAvailableClusters() => _availableClusters;

        public IEnumerable<ClusterModel> GetPlacedClusters() => _placedClusters;

        public bool TryGetCluster(ClusterModel cluster, out ClusterItem clusterItem)
        {
            clusterItem = null;
            
            foreach (ClusterItem item in _createdClusters)
            {
                if (cluster.Text.Equals(item.Text, StringComparison.OrdinalIgnoreCase) && !item.IsPlaced)
                {
                    clusterItem = item;
                    return true;
                }
            }

            return clusterItem != null;
        }

        public void MarkClusterAsPlaced(ClusterModel cluster)
        {
            _placedClusters.Add(cluster);
            _availableClusters.RemoveAll(c => c.Text == cluster.Text);
        }

        public void MarkClusterAsAvailable(ClusterModel cluster)
        {
            _placedClusters.RemoveAll(c => c.Text == cluster.Text);
            _availableClusters.Add(cluster);
        }

        public void Clear()
        {
            _createdClusters.Clear();
            _availableClusters.Clear();
            _placedClusters.Clear();
        }
    }
}