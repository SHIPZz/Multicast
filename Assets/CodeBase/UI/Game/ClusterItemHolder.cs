using System.Collections.Generic;
using CodeBase.UI.Services.Cluster;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Game
{
    public class ClusterItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform _clusterItemLayout;
        
        private readonly List<ClusterItem> _clusterItems = new();
        
        private IClusterUIFactory _clusterUIFactory;
        private WordSlotHolder _wordSlotHolder;

        public IReadOnlyList<ClusterItem> ClusterItems => _clusterItems;

        [Inject] 
        private void Construct(IClusterUIFactory clusterUIFactory)
        {
            _clusterUIFactory = clusterUIFactory;
        }
        
        public void CreateClusterItem(string clusterText, WordSlotHolder wordSlotHolder)
        {
            ClusterItem clusterItem = _clusterUIFactory.CreateClusterItem(_clusterItemLayout);
            
            clusterItem.Initialize(clusterText, wordSlotHolder);
            
            _clusterItems.Add(clusterItem);
        }

        public void Clear()
        {
            foreach (var clusterItem in _clusterItems)
            {
                Destroy(clusterItem.gameObject);
            }
            
            _clusterItems.Clear();
        }

        public void CreateClusterItems(IEnumerable<string> clusters, WordSlotHolder wordSlotHolder)
        {
            foreach (string cluster in clusters)
            {
                CreateClusterItem(cluster, wordSlotHolder);
            }
        }
    }
} 