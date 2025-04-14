using System;
using CodeBase.Data;
using CodeBase.UI.Cluster;

namespace CodeBase.Extensions
{
    public static class ClusterExtensions
    {
        public static ClusterModel ToModel(this ClusterItem clusterItem, int row, int column,string id)
        {
            return new ClusterModel(
                clusterItem.Text,
                clusterItem.IsPlaced,
                row,
                column,
                id
            );
        }
    }
} 