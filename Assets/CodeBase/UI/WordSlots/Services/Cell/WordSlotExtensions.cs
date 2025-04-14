using CodeBase.UI.WordSlots;

namespace CodeBase.UI.WordSlots.Services.Cell
{
    public static class WordSlotExtensions
    {
        public static WordSlotCell ToCell(this WordSlot slot)
        {
            if (slot == null)
                return default;

            var cell = new WordSlotCell(slot.Row, slot.Column);
            if (slot.IsOccupied)
                cell.SetLetter(slot.CurrentLetter);
            return cell;
        }

        public static void UpdateFromCell(this WordSlot slot, WordSlotCell cell)
        {
            if (slot == null)
                return;

            if (cell.IsOccupied)
                slot.SetText(cell.Letter);
            else
                slot.Clear();
        }

        public static bool IsSamePosition(this WordSlotCell cell, WordSlot slot)
        {
            return slot != null && cell.Row == slot.Row && cell.Column == slot.Column;
        }

        public static bool IsSamePosition(this WordSlotCell cell, WordSlotCell other)
        {
            return cell.Row == other.Row && cell.Column == other.Column;
        }
    }
} 