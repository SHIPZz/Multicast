﻿using System.Collections.Generic;
using CodeBase.Data;
using CodeBase.UI.WordCells;

namespace CodeBase.UI.Cluster.Services
{
    public interface IClusterService
    {
        bool TryPlaceCluster(ClusterItem cluster, WordCellView wordCellView);
        void ResetCluster(ClusterItem cluster);
        void CheckAndHideFilledClusters();
        void OnClusterSelected(ClusterItem clusterItem);
        void SetClusters(IEnumerable<string> clusters);
        IEnumerable<ClusterModel> GetAvailableClusters();
        void Cleanup();
        void RegisterCreatedCluster(ClusterItem clusterItem);
        void RestorePlacedClusters();
        IEnumerable<ClusterModel> AllClusters { get; }
    }
}