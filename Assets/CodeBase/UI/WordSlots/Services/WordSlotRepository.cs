using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordSlots.Services.Grid;

namespace CodeBase.UI.WordSlots.Services
{
    public class WordSlotRepository : IWordSlotRepository
    {
        private readonly WordSlotGrid _grid = new(GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn);
        private readonly List<WordSlot> _createdSlots = new(GameplayConstants.MaxWordSlotCount);

        public IReadOnlyList<WordSlot> CreatedSlots => _createdSlots;

        public WordSlotGrid Grid => _grid;
        public int SlotCount => _grid.SlotCount;

        public void UpdateCell(int row, int column, char letter) => 
            _grid.UpdateCell(row, column, letter);

        public void ClearCell(int row, int column) => 
            _grid.ClearCell(row, column);
        
        public void ClearAllCells() => 
            _grid.ClearAllCells();

        public void ClearRegisterSlots() => 
            _createdSlots.Clear();

        public void RegisterCreatedSlot(WordSlot slot) => 
            _createdSlots.Add(slot);

        public int IndexOf(WordSlot wordSlot) => _createdSlots.IndexOf(wordSlot);
        
        public WordSlot GetTargetSlot(int index) => _createdSlots[index];
        
        public WordSlot GetWordSlotByRowAndColumn(int row, int column) => 
            _createdSlots.FirstOrDefault(slot => slot.Row == row && slot.Column == column);

        public void RestoreState(string[,] savedGrid) => _grid.RestoreState(savedGrid);
    }
}