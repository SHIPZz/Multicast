using System.Collections.Generic;
using CodeBase.Gameplay.Cluster;
using CodeBase.UI.Services.WordSlots;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.WordSlots
{
    public class WordSlotHolder : MonoBehaviour
    {
        [SerializeField] private Transform _slotsContainer;

        private readonly List<WordSlot> _wordSlots = new();

        private IWordSlotUIFactory _wordSlotUIFactory;
        private IClusterService _clusterService;

        public IReadOnlyList<WordSlot> WordSlots => _wordSlots;
        
        public int IndexOf(WordSlot wordSlot) => _wordSlots.IndexOf(wordSlot);

        [Inject]
        private void Construct(IWordSlotUIFactory wordSlotUIFactory, IClusterService clusterService)
        {
            _clusterService = clusterService;
            _wordSlotUIFactory = wordSlotUIFactory;
        }

        public void CreateWordSlots()
        {
            ClearSlots();

            int targetCount =  _clusterService.GetCurrentWords().Count * _clusterService.MaxLettersInWord;
            
            for (int i = 0; i < targetCount; i++)
            {
                WordSlot slot = _wordSlotUIFactory.CreateWordSlotPrefab(_slotsContainer);

                _wordSlots.Add(slot);
            }
        }

        public void ClearSlots()
        {
            foreach (var slot in _wordSlots)
            {
                Destroy(slot.gameObject);
            }

            _wordSlots.Clear();
        }
    }
}