using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.Cluster.Services;
using CodeBase.UI.Cluster.Services.Factory;
using CodeBase.UI.WordSlots;
using UniRx;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Cluster
{
    public class ClusterItemHolder : MonoBehaviour
    {
        [SerializeField] private Transform _clusterItemLayout;
        
        private readonly Dictionary<ClusterAttachItem, ClusterItem> _clusterItems = new(GameplayConstants.MaxClusterCount);
        
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
            foreach (ClusterAttachItem attachItem in _clusterItems.Keys)
            {
                attachItem.Cleanup();
            }
            
            _clusterItems.Clear();
        }

        public void CreateClusterItems(IEnumerable<ClusterModel> clusters, Canvas parentCanvas)
        {
            for (int i = 0; i < clusters.Count(); i++)
            {
                ClusterAttachItem attachItem = _clusterUIFactory.CreateClusterAttachItem(_clusterItemLayout);
                
                ClusterItem clusterItem = _clusterUIFactory.CreateClusterItem(attachItem.transform);
            
                clusterItem.Initialize(clusters.ElementAt(i).Text, attachItem.transform, parentCanvas);
                _clusterService.RegisterCreatedCluster(clusterItem);
                _clusterItems[attachItem] = clusterItem;
                clusterItem.DisabledEvent.Subscribe(_ => attachItem.Cleanup()).AddTo(attachItem);
            }
        }
    }
}