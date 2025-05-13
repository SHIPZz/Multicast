using System.Collections.Generic;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordCells.Services;
using CodeBase.UI.WordCells.Services.Factory;
using CodeBase.UI.WordCells.Services.Grid;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.WordCells
{
    public class WordCellsHolder : MonoBehaviour
    {
        [SerializeField] private Transform _slotsContainer;

        private readonly List<WordCellView> _wordSlots = new(GameplayConstants.MaxWordSlotCount);
        private IWordCellUIFactory _wordCellUIFactory;
        private IWordCellRepository _wordCellRepository;

        [Inject]
        private void Construct(IWordCellUIFactory wordCellUIFactory, IWordCellRepository wordCellRepository)
        {
            _wordCellRepository = wordCellRepository;
            _wordCellUIFactory = wordCellUIFactory;
        }

        public void CreateWordSlots()
        {
            ClearSlots();

            WordCellGrid wordCellGrid = _wordCellRepository.Grid;

            for (int row = 0; row < wordCellGrid.Rows; row++)
            {
                for (int column = 0; column < wordCellGrid.Columns; column++)
                {
                    WordCellView cellView = _wordCellUIFactory.CreateWordSlotPrefab(_slotsContainer);
                    cellView.Initialize(row, column);
                    _wordCellRepository.RegisterCreatedSlot(cellView);
                    _wordSlots.Add(cellView);
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

            _wordCellRepository.ClearRegisterSlots();
        }
    }
}