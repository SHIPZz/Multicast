using CodeBase.StaticData;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Game
{
    public class ClusterUIFactory
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

        public ClusterItem CreateClusterItem(Transform parent)
        {
            var clusterItem = _instantiator.InstantiatePrefabForComponent<ClusterItem>(
                _uiStaticDataService.GetClusterItem().gameObject,
                parent);

            return clusterItem;
        }
    }
} 