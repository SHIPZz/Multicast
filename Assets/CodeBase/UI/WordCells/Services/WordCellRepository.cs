using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordCells.Services.Grid;

namespace CodeBase.UI.WordCells.Services
{
    public class WordCellRepository : IWordCellRepository
    {
        private readonly WordCellGrid _grid = new(GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn);
        private readonly List<WordCellView> _createdSlots = new(GameplayConstants.MaxWordSlotCount);

        public IReadOnlyList<WordCellView> CreatedSlots => _createdSlots;

        public WordCellGrid Grid => _grid;
        public int SlotCount => _grid.SlotCount;

        public void UpdateCell(int row, int column, char letter) => 
            _grid.UpdateCell(row, column, letter);

        public void ClearCell(int row, int column) => 
            _grid.ClearCell(row, column);
        
        public void ClearAllCells() => 
            _grid.ClearAllCells();

        public void ClearRegisterSlots() => 
            _createdSlots.Clear();

        public void RegisterCreatedSlot(WordCellView cellView) => 
            _createdSlots.Add(cellView);

        public int IndexOf(WordCellView wordCellView) => _createdSlots.IndexOf(wordCellView);
        
        public WordCellView GetTargetSlot(int index) => _createdSlots[index];
        
        public WordCellView GetWordSlotByRowAndColumn(int row, int column) => 
            _createdSlots.FirstOrDefault(slot => slot.Row == row && slot.Column == column);

        public void RestoreState(string[,] savedGrid) => _grid.Load(savedGrid);
    }
}