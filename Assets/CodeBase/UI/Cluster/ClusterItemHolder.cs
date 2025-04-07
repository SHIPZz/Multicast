using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.Cluster.Services;
using CodeBase.UI.Cluster.Services.Factory;
using CodeBase.UI.WordSlots;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Cluster
{
    public class ClusterItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform _clusterItemLayout;
        [SerializeField] private GameObject _clusterItemAttachPrefab;
        
        private readonly List<ClusterItem> _clusterItems = new(GameplayConstants.MaxClusterCount);
        
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
            for (int i = 0; i < clusters.Count(); i++)
            {
                GameObject attachItem = _clusterUIFactory.Create(_clusterItemLayout, _clusterItemAttachPrefab);
                
                ClusterItem clusterItem = _clusterUIFactory.CreateClusterItem(attachItem.transform);
            
                clusterItem.Initialize(clusters.ElementAt(i), attachItem.transform, parentCanvas);
                _clusterService.RegisterCreatedCluster(clusterItem);
                _clusterItems.Add(clusterItem);
            }
        }
    }
}