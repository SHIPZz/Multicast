using System;

namespace CodeBase.UI.WordSlots.Services.Cell
{
    public struct WordSlotCell : IEquatable<WordSlotCell>
    {
        private readonly int _row;
        private readonly int _column;
        private char _letter;
        private bool _isOccupied;

        public char Letter => _letter;
        public bool IsOccupied => _isOccupied;

        public WordSlotCell(int row, int column)
        {
            _row = row;
            _column = column;
            _letter = '\0';
            _isOccupied = false;
        }

        public void SetLetter(char letter)
        {
            if (letter == '\0')
                return;
            
            _letter = letter;
            _isOccupied = true;
        }

        public void Clear()
        {
            _letter = '\0';
            _isOccupied = false;
        }

        public bool Equals(WordSlotCell other)
        {
            return _row == other._row && _column == other._column && _letter == other._letter && _isOccupied == other._isOccupied;
        }

        public override bool Equals(object obj)
        {
            return obj is WordSlotCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_row, _column, _letter, _isOccupied);
        }
    }
} 