using System.Collections.Generic;
using CodeBase.Gameplay.Common.Services.Cluster;
using CodeBase.StaticData;
using CodeBase.UI.Services.WordSlots;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Game
{
    public class WordSlotHolder : MonoBehaviour
    {
        //todo: make it to config. count of words * count of letters
        private const int TargetSlots = 24;

        [SerializeField] private Transform _slotsContainer;

        private readonly List<WordSlot> _wordSlots = new();

        private IWordSlotUIFactory _wordSlotUIFactory;

        public IReadOnlyList<WordSlot> WordSlots => _wordSlots;
        
        public int IndexOf(WordSlot wordSlot) => _wordSlots.IndexOf(wordSlot);

        [Inject]
        private void Construct(IWordSlotUIFactory wordSlotUIFactory)
        {
            _wordSlotUIFactory = wordSlotUIFactory;
        }

        public void CreateWordSlots(IReadOnlyList<string> words)
        {
            ClearSlots();

            for (int i = 0; i < TargetSlots; i++)
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