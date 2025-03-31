using CodeBase.StaticData;
using CodeBase.UI.Game;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterUIFactory : IClusterUIFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly IUIStaticDataService _uiStaticDataService;

        public ClusterUIFactory(
            IInstantiator instantiator,
            IUIStaticDataService uiStaticDataService)
        {
            _instantiator = instantiator;
            _uiStaticDataService = uiStaticDataService;
        }
        
        public ClusterItemHolder CreateClusterItemHolder(Transform parent)
        {
            ClusterItemHolder prefab = _uiStaticDataService.GetClusterItemHolder();
            
            var clusterItem = _instantiator.InstantiatePrefabForComponent<ClusterItemHolder>(prefab, parent);

            return clusterItem;
        }

        public ClusterItem CreateClusterItem(Transform parent)
        {
            ClusterItem prefab = _uiStaticDataService.GetClusterItem();
            
            var clusterItem = _instantiator.InstantiatePrefabForComponent<ClusterItem>(prefab, parent);

            return clusterItem;
        }
    }
} 