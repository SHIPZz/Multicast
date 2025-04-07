using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
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
        
        private readonly List<GameObject> _attachItems = new(GameplayConstants.MaxClusterCount);
        
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
            foreach (var clusterItem in _attachItems)
            {
                Destroy(clusterItem.gameObject);
            }
            
            _attachItems.Clear();
        }

        public void CreateClusterItems(IEnumerable<ClusterModel> clusters, Canvas parentCanvas)
        {
            for (int i = 0; i < clusters.Count(); i++)
            {
                GameObject attachItem = _clusterUIFactory.Create(_clusterItemLayout, _clusterItemAttachPrefab);
                
                ClusterItem clusterItem = _clusterUIFactory.CreateClusterItem(attachItem.transform);
            
                clusterItem.Initialize(clusters.ElementAt(i).Text, attachItem.transform, parentCanvas);
                _clusterService.RegisterCreatedCluster(clusterItem);
                _attachItems.Add(attachItem);
            }
        }
    }
}