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

        private readonly List<WordSlot> _wordSlots = new(GameplayConstants.MaxWordSlotCount);
        private IWordSlotUIFactory _wordSlotUIFactory;
        private IWordSlotRepository _wordSlotRepository;

        [Inject]
        private void Construct(IWordSlotUIFactory wordSlotUIFactory, IWordSlotRepository wordSlotRepository)
        {
            _wordSlotRepository = wordSlotRepository;
            _wordSlotUIFactory = wordSlotUIFactory;
        }

        public void CreateWordSlots()
        {
            ClearSlots();

            WordSlotGrid wordSlotGrid = _wordSlotRepository.Grid;

            for (int row = 0; row < wordSlotGrid.Rows; row++)
            {
                for (int column = 0; column < wordSlotGrid.Columns; column++)
                {
                    WordSlot slot = _wordSlotUIFactory.CreateWordSlotPrefab(_slotsContainer);
                    slot.Initialize(row, column);
                    _wordSlotRepository.RegisterCreatedSlot(slot);
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

            _wordSlotRepository.ClearRegisterSlots();
        }
    }
}