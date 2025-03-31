using CodeBase.StaticData;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Game
{
    public class WordSlotUIFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly IUIStaticDataService _uiStaticDataService;

        public WordSlotUIFactory(
            IInstantiator instantiator,
            IUIStaticDataService uiStaticDataService)
        {
            _instantiator = instantiator;
            _uiStaticDataService = uiStaticDataService;
        }

        public WordSlotHolder CreateWordSlotHolder(Transform parent)
        {
            var holder = _instantiator.InstantiatePrefabForComponent<WordSlotHolder>(
                _uiStaticDataService.GetWordSlotPrefab().gameObject,
                parent);

            return holder;
        }

        public WordSlot CreateWordSlot(Transform parent)
        {
            var slot = _instantiator.InstantiatePrefabForComponent<WordSlot>(
                _uiStaticDataService.GetWordSlotPrefab().gameObject,
                parent);

            return slot;
        }
    }
} 