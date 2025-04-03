using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Cluster;
using CodeBase.Gameplay.WordSlots;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.WordSlots;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.WordSlots
{
    public class WordSlotHolder : MonoBehaviour
    {
        [SerializeField] private Transform _slotsContainer;

        private readonly List<WordSlot> _wordSlots = new();
        private readonly Dictionary<int, List<WordSlot>> _rowSlots = new();

        private IWordSlotUIFactory _wordSlotUIFactory;
        private IWordSlotService _wordSlotService;

        public IReadOnlyList<WordSlot> WordSlots => _wordSlots;
        public IReadOnlyDictionary<int, List<WordSlot>> GetRowSlots() => _rowSlots;
        
        [Inject]
        private void Construct(IWordSlotUIFactory wordSlotUIFactory, IWordSlotService wordSlotService)
        {
            _wordSlotService = wordSlotService;
            _wordSlotUIFactory = wordSlotUIFactory;
        }

        private void Start()
        {
            _wordSlotService.SetCurrentWordSlotHolder(this);
        }

        public int IndexOf(WordSlot wordSlot) => _wordSlots.IndexOf(wordSlot);

        public void CreateWordSlots()
        {
            ClearSlots();

            int lettersInWord = _wordSlotService.MaxLettersInWord;
            int wordCount = _wordSlotService.WordsToFind.Count;

            for (int row = 0; row < wordCount; row++)
            {
                _rowSlots[row] = new List<WordSlot>();
                
                for (int i = 0; i < lettersInWord; i++)
                {
                    WordSlot slot = _wordSlotUIFactory.CreateWordSlotPrefab(_slotsContainer);

                    _wordSlots.Add(slot);
                    _rowSlots[row].Add(slot);
                }
            }
        }

        public void ClearSlots()
        {
            foreach (var slot in _wordSlots)
            {
                Destroy(slot.gameObject);
            }

            _wordSlots.Clear();
            _rowSlots.Clear();
        }
    }
}