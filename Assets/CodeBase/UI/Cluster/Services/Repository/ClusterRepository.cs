using System;
using System.Collections.Generic;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using CodeBase.Gameplay.Constants;

namespace CodeBase.UI.Cluster.Services.Repository
{
    public class ClusterRepository : IClusterRepository
    {
        private readonly List<ClusterItem> _createdClusters = new(GameplayConstants.MaxClusterCount);
        private readonly List<ClusterModel> _availableClusters = new(GameplayConstants.MaxClusterCount);
        private readonly List<ClusterModel> _placedClusters = new(GameplayConstants.MaxClusterCount);
        private readonly List<ClusterModel> _allClusters = new(GameplayConstants.MaxClusterCount);

        public void RegisterCluster(ClusterItem clusterItem) => 
            _createdClusters.Add(clusterItem);

        public void AddAvailableClusters(IEnumerable<ClusterModel> clusters)
        {
            _availableClusters.AddRange(clusters);
            _allClusters.AddRange(_availableClusters);
        }

        public IEnumerable<ClusterModel> GetAvailableClusters() => _availableClusters;

        public IEnumerable<ClusterModel> GetPlacedClusters() => _placedClusters;
        
        public IEnumerable<ClusterModel> GetAllClusters() => _allClusters;

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
            
            _availableClusters.RemoveAll(c => c.Id == cluster.Id);
        }

        public void MarkClusterAsAvailable(ClusterModel cluster)
        {
            _placedClusters.RemoveAll(c => c.Id == cluster.Id);
            _availableClusters.Add(cluster);
        }

        public void Clear()
        {
            _createdClusters.Clear();
            _availableClusters.Clear();
            _placedClusters.Clear();
            _allClusters.Clear();
        }

        public void Save(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;
            playerData.AvailableClusters.Clear();

            playerData.AvailableClusters.AddRange(GetAvailableClusters());

            SavePlacedClusters(playerData);
        }

        public void Load(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;

            List<ClusterModel> models = new();

            FormAvailableClusters(playerData, models);

            AddAvailableClusters(models);

            LoadPlacedClusters(playerData);
        }

        private void LoadPlacedClusters(PlayerData playerData)
        {
            if (playerData.PlacedClustersGrid == null)
                return;

            for (int row = 0; row < playerData.GridRows; row++)
            {
                for (int col = 0; col < playerData.GridColumns; col++)
                {
                    if (playerData.PlacedClustersGrid[row, col].Text != null)
                    {
                        ClusterModel cluster = playerData.PlacedClustersGrid[row, col];
                        var newCluster = new ClusterModel(cluster.Text, cluster.IsPlaced, cluster.Row, cluster.Column);
                         MarkClusterAsPlaced(newCluster);
                    }
                }
            }
        }

        private void SavePlacedClusters(PlayerData playerData)
        {
            int maxRow = 0;
            int maxCol = 0;

            foreach (ClusterModel placed in GetPlacedClusters())
            {
                maxRow = Math.Max(maxRow, placed.Row);
                maxCol = Math.Max(maxCol, placed.Column);
            }

            playerData.GridRows = maxRow + 1;
            playerData.GridColumns = maxCol + 1;
            playerData.PlacedClustersGrid = new ClusterModel[playerData.GridRows, playerData.GridColumns];
            
            foreach (ClusterModel placed in GetPlacedClusters())
            {
                playerData.PlacedClustersGrid[placed.Row, placed.Column] = placed;
            }
        }

        private static void FormAvailableClusters(PlayerData playerData, List<ClusterModel> models)
        {
            foreach (ClusterModel model in playerData.AvailableClusters)
                models.Add(new ClusterModel(model.Text, false, -1, -1));
        }
    }
}