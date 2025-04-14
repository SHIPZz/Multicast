using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordSlots.Services;
using CodeBase.UI.WordSlots.Services.Cell;
using CodeBase.UI.WordSlots.Services.Factory;
using CodeBase.UI.WordSlots.Services.Grid;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.WordSlots
{
    public class WordSlotHolder : MonoBehaviour
    {
        [SerializeField] private Transform _slotsContainer;

        private readonly List<WordSlot> _wordSlots = new(GameplayConstants.WordSlotCount);
        private IWordSlotUIFactory _wordSlotUIFactory;
        private IWordSlotService _wordSlotService;

        public IReadOnlyList<WordSlot> WordSlots => _wordSlots;

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

            WordSlotGrid wordSlotGrid = _wordSlotService.Grid;
            
            for (int row = 0; row < wordSlotGrid.Rows; row++)
            {
                for (int column = 0; column < wordSlotGrid.Columns; column++)
                {
                    WordSlot slot = _wordSlotUIFactory.CreateWordSlotPrefab(_slotsContainer);
                    slot.Initialize(row, column);
                    _wordSlots.Add(slot);
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
        }

        public WordSlot GetSlotByRowAndColumn(int row, int column)
        {
            return _wordSlots.FirstOrDefault(slot => slot.Row == row && slot.Column == column);
        }
    }
}