using System.Collections.Generic;
using CodeBase.UI.WordCells.Services.Grid;

namespace CodeBase.UI.WordCells.Services
{
    public interface IWordCellRepository
    {
        WordCellGrid Grid { get; }
        int SlotCount { get; }
        public IReadOnlyList<WordCellView> CreatedSlots { get;}
        void UpdateCell(int row, int column, char letter);
        void ClearCell(int row, int column);
        void ClearRegisterSlots();
        void RegisterCreatedSlot(WordCellView cellView);
        int IndexOf(WordCellView wordCellView);
        WordCellView GetTargetSlot(int placeIndex);
        WordCellView GetWordSlotByRowAndColumn(int row, int column);
        void ClearAllCells();
        void RestoreState(string[,] savedGrid);
    }
}