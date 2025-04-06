using System.Collections.Generic;
using CodeBase.Gameplay.WordSlots;
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
        private readonly Dictionary<int, Dictionary<int, WordSlot>> _slotsByRowAndColumn = new();

        private IWordSlotUIFactory _wordSlotUIFactory;
        private IWordSlotService _wordSlotService;

        public IReadOnlyList<WordSlot> WordSlots => _wordSlots;

        public IReadOnlyDictionary<int, Dictionary<int, WordSlot>> GetSlotsByRowAndColumn() => _slotsByRowAndColumn;

        [Inject]
        private void Construct(IWordSlotUIFactory wordSlotUIFactory, IWordSlotService wordSlotService)
        {
            _wordSlotService = wordSlotService;
            _wordSlotUIFactory = wordSlotUIFactory;
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

                CreateSlotsForRow(lettersInWord, row);
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
            _slotsByRowAndColumn.Clear();
        }

        public WordSlot GetSlotByRowAndColumn(int row, int column)
        {
            if (_slotsByRowAndColumn.TryGetValue(row, out Dictionary<int, WordSlot> columnDict))
            {
                if (columnDict.TryGetValue(column, out WordSlot slot))
                {
                    return slot;
                }
            }
            return null;
        }

        public int GetColumnIndex(WordSlot slot)
        {
            foreach (var rowDict in _slotsByRowAndColumn.Values)
            {
                foreach (var columnSlot in rowDict)
                {
                    if (columnSlot.Value == slot)
                        return columnSlot.Key;
                }
            }
            return -1;
        }

        public IReadOnlyList<WordSlot> GetSlotsByRow(int row)
        {
            return _rowSlots.GetValueOrDefault(row);
        }

        private void CreateSlotsForRow(int lettersInWord, int row)
        {
            if (!_slotsByRowAndColumn.ContainsKey(row))
                _slotsByRowAndColumn[row] = new Dictionary<int, WordSlot>();
                
            for (int column = 0; column < lettersInWord; column++)
            {
                WordSlot slot = _wordSlotUIFactory.CreateWordSlotPrefab(_slotsContainer);

                _wordSlots.Add(slot);
                _rowSlots[row].Add(slot);
                _slotsByRowAndColumn[row][column] = slot;
            }
        }
    }
}