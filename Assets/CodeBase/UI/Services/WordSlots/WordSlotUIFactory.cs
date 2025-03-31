using CodeBase.StaticData;
using CodeBase.UI.Game;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Services.WordSlots
{
    public class WordSlotUIFactory : IWordSlotUIFactory
    {
        private readonly IUIStaticDataService _uiStaticDataService;
        private readonly IInstantiator _instantiator;

        public WordSlotUIFactory(IUIStaticDataService uiStaticDataService, IInstantiator instantiator)
        {
            _instantiator = instantiator;
            _uiStaticDataService = uiStaticDataService;
        }

        public WordSlotHolder CreateWordSlotHolder(Transform parent)
        {
            WordSlotHolder prefab = _uiStaticDataService.GetWordSlotHolderPrefab();
            
            return _instantiator.InstantiatePrefabForComponent<WordSlotHolder>(prefab, parent);
        }

        public WordSlot CreateWordSlotPrefab(Transform parent)
        {
            WordSlot prefab = _uiStaticDataService.GetWordSlotPrefab();
            
            return _instantiator.InstantiatePrefabForComponent<WordSlot>(prefab, parent);
        }
    }
} 