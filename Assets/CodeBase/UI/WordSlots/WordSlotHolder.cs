using System.Collections.Generic;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordSlots.Services;
using CodeBase.UI.WordSlots.Services.Factory;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.WordSlots
{
    public class WordSlotHolder : MonoBehaviour
    {
        [SerializeField] private Transform _slotsContainer;

        private readonly List<WordSlot> _wordSlots = new(GameplayConstants.WordSlotCount);
        private WordSlot[,] _slotsGrid;
        private int _gridRows;
        private int _gridColumns;

        private IWordSlotUIFactory _wordSlotUIFactory;
        private IWordSlotService _wordSlotService;

        public IReadOnlyList<WordSlot> WordSlots => _wordSlots;
        
        public int GridRows => _gridRows;
        public int GridColumns => _gridColumns;

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

            _gridRows = wordCount;
            _gridColumns = lettersInWord;
            _slotsGrid = new WordSlot[_gridRows, _gridColumns];

            for (int row = 0; row < wordCount; row++)
            {
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
            _slotsGrid = null;
        }

        public WordSlot GetSlotByRowAndColumn(int row, int column)
        {
            if (row < 0 || row >= _gridRows || column < 0 || column >= _gridColumns)
                return null;

            return _slotsGrid[row, column];
        }

        public int GetColumnIndex(WordSlot slot)
        {
            for (int row = 0; row < _gridRows; row++)
            {
                for (int col = 0; col < _gridColumns; col++)
                {
                    if (_slotsGrid[row, col] == slot)
                        return col;
                }
            }
            
            return -1;
        }

        private void CreateSlotsForRow(int lettersInWord, int row)
        {
            for (int column = 0; column < lettersInWord; column++)
            {
                WordSlot slot = _wordSlotUIFactory.CreateWordSlotPrefab(_slotsContainer);
                _wordSlots.Add(slot);
                _slotsGrid[row, column] = slot;
            }
        }
    }
}