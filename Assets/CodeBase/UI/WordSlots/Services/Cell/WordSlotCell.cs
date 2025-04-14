using System;

namespace CodeBase.UI.WordSlots.Services.Cell
{
    public struct WordSlotCell : IEquatable<WordSlotCell>
    {
        private readonly Guid _id;
        private readonly int _row;
        private readonly int _column;
        private char _letter;
        private bool _isOccupied;

        public int Row => _row;
        public int Column => _column;
        public char Letter => _letter;
        public bool IsOccupied => _isOccupied;
        public Guid Id => _id;

        public WordSlotCell(int row, int column)
        {
            _id = Guid.NewGuid();
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
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            return obj is WordSlotCell other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(WordSlotCell left, WordSlotCell right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WordSlotCell left, WordSlotCell right)
        {
            return !left.Equals(right);
        }
    }
} 