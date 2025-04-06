using System.Collections.Generic;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.WordSlots;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Cluster
{
    public class ClusterItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform _clusterItemLayout;
        
        private readonly List<ClusterItem> _clusterItems = new();
        
        private IClusterUIFactory _clusterUIFactory;
        private WordSlotHolder _wordSlotHolder;
        private IClusterService _clusterService;

        [Inject] 
        private void Construct(IClusterUIFactory clusterUIFactory, IClusterService clusterService)
        {
            _clusterService = clusterService;
            _clusterUIFactory = clusterUIFactory;
        }

        public void Clear()
        {
            foreach (var clusterItem in _clusterItems)
            {
                Destroy(clusterItem.gameObject);
            }
            
            _clusterItems.Clear();
        }

        public void CreateClusterItems(IEnumerable<string> clusters, Canvas parentCanvas)
        {
            foreach (string cluster in clusters)
            {
                ClusterItem clusterItem = _clusterUIFactory.CreateClusterItem(_clusterItemLayout);
            
                clusterItem.Initialize(cluster, _clusterItemLayout, parentCanvas);
                _clusterService.RegisterCreatedCluster(clusterItem);
                _clusterItems.Add(clusterItem);
            }
        }
    }
}