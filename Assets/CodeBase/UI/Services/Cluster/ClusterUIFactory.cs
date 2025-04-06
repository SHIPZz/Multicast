using CodeBase.StaticData;
using CodeBase.UI.Cluster;
using CodeBase.UI.Game;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterUIFactory : IClusterUIFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly IStaticDataService _staticDataService;

        public ClusterUIFactory(
            IInstantiator instantiator,
            IStaticDataService staticDataService)
        {
            _instantiator = instantiator;
            _staticDataService = staticDataService;
        }
        
        public ClusterItemHolder CreateClusterItemHolder(Transform parent)
        {
            ClusterItemHolder prefab = _staticDataService.GetClusterItemHolder();
            
            var clusterItem = _instantiator.InstantiatePrefabForComponent<ClusterItemHolder>(prefab, parent);

            return clusterItem;
        }

        public ClusterItem CreateClusterItem(Transform parent)
        {
            ClusterItem prefab = _staticDataService.GetClusterItem();
            
            var clusterItem = _instantiator.InstantiatePrefabForComponent<ClusterItem>(prefab, parent);

            return clusterItem;
        }
    }
} 