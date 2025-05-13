using CodeBase.StaticData;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.WordCells.Services.Factory
{
    public class WordCellUIFactory : IWordCellUIFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IInstantiator _instantiator;

        public WordCellUIFactory(IStaticDataService staticDataService, IInstantiator instantiator)
        {
            _instantiator = instantiator;
            _staticDataService = staticDataService;
        }

        public WordCellsHolder CreateWordSlotHolder(Transform parent)
        {
            WordCellsHolder prefab = _staticDataService.GetWordSlotHolderPrefab();
            
            return _instantiator.InstantiatePrefabForComponent<WordCellsHolder>(prefab, parent);
        }

        public WordCellView CreateWordSlotPrefab(Transform parent)
        {
            WordCellView prefab = _staticDataService.GetWordSlotPrefab();
            
            return _instantiator.InstantiatePrefabForComponent<WordCellView>(prefab, parent);
        }
    }
} 