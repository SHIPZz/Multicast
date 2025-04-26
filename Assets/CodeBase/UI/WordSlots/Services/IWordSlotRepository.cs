using System.Collections.Generic;
using CodeBase.UI.WordSlots.Services.Grid;

namespace CodeBase.UI.WordSlots.Services
{
    public interface IWordSlotRepository
    {
        WordSlotGrid Grid { get; }
        int SlotCount { get; }
        public IReadOnlyList<WordSlot> CreatedSlots { get;}
        void UpdateCell(int row, int column, char letter);
        void ClearCell(int row, int column);
        void ClearRegisterSlots();
        void RegisterCreatedSlot(WordSlot slot);
        int IndexOf(WordSlot wordSlot);
        WordSlot GetTargetSlot(int placeIndex);
        WordSlot GetWordSlotByRowAndColumn(int row, int column);
        void ClearAllCells();
        void RestoreState(string[,] savedGrid);
    }
}