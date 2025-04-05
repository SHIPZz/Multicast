using System.Collections.Generic;
using CodeBase.UI.Cluster;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterRepository : IClusterRepository
    {
        private readonly Dictionary<string, ClusterItem> _createdClusters = new(32);
        private readonly HashSet<string> _availableClusters = new();
        private readonly HashSet<string> _placedClusters = new();

        public void RegisterCluster(ClusterItem clusterItem) => 
            _createdClusters[clusterItem.Text] = clusterItem;

        public void AddAvailableClusters(IEnumerable<string> clusters) => 
            _availableClusters.UnionWith(clusters);

        public void AddPlacedClusters(IEnumerable<string> clusters) =>
            _placedClusters.UnionWith(clusters);

        public IEnumerable<string> GetAvailableClusters() => _availableClusters;

        public IEnumerable<string> GetPlacedClusters() => _placedClusters;

        public bool IsClusterAvailable(string clusterText) => 
            _availableClusters.Contains(clusterText);

        public bool IsClusterPlaced(string clusterText) => 
            _placedClusters.Contains(clusterText);

        public bool TryGetCluster(string clusterText, out ClusterItem clusterItem) => 
            _createdClusters.TryGetValue(clusterText, out clusterItem);

        public void MarkClusterAsPlaced(string clusterText)
        {
            _placedClusters.Add(clusterText);
            _availableClusters.Remove(clusterText);
        }

        public void MarkClusterAsAvailable(string clusterText)
        {
            _placedClusters.Remove(clusterText);
            _availableClusters.Add(clusterText);
        }

        public void Clear()
        {
            _createdClusters.Clear();
            _availableClusters.Clear();
            _placedClusters.Clear();
        }
    }
}