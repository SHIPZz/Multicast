using CodeBase.StaticData;
using CodeBase.UI.WordSlots;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Services.WordSlots
{
    public class WordSlotUIFactory : IWordSlotUIFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IInstantiator _instantiator;

        public WordSlotUIFactory(IStaticDataService staticDataService, IInstantiator instantiator)
        {
            _instantiator = instantiator;
            _staticDataService = staticDataService;
        }

        public WordSlotHolder CreateWordSlotHolder(Transform parent)
        {
            WordSlotHolder prefab = _staticDataService.GetWordSlotHolderPrefab();
            
            return _instantiator.InstantiatePrefabForComponent<WordSlotHolder>(prefab, parent);
        }

        public WordSlot CreateWordSlotPrefab(Transform parent)
        {
            WordSlot prefab = _staticDataService.GetWordSlotPrefab();
            
            return _instantiator.InstantiatePrefabForComponent<WordSlot>(prefab, parent);
        }
    }
} 