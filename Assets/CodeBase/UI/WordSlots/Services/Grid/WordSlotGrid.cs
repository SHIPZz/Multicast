using System.Collections.Generic;
using System.Text;
using CodeBase.UI.WordSlots.Services.Cell;

namespace CodeBase.UI.WordSlots.Services.Grid
{
    public readonly struct WordSlotGrid
    {
        private readonly WordSlotCell[,] _cells;
        private readonly int _rows;
        private readonly int _columns;

        public int Rows => _rows;
        public int Columns => _columns;
        public int SlotCount => _rows * _columns;

        public WordSlotGrid(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            _cells = new WordSlotCell[rows, columns];

            InitializeCells();
        }

        public WordSlotCell GetCell(int row, int column)
        {
            if (row < 0 || row >= _rows || column < 0 || column >= _columns)
                return new WordSlotCell();

            return _cells[row, column];
        }

        public void UpdateCell(int row, int column, char letter)
        {
            if (row < 0 || row >= _rows || column < 0 || column >= _columns)
                return;

            _cells[row, column].SetLetter(letter);
        }

        public IReadOnlyDictionary<int, string> GetWordsWithRow()
        {
            var words = new Dictionary<int, string>();

            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                string targetWord = GetWordInRow(i);
                words.Add(i, targetWord);
            }
            
            return words;
        }

        public string GetWordInRow(int row)
        {
            StringBuilder word = new StringBuilder();

            for (int column = 0; column < _columns; column++)
            {
                WordSlotCell cell = _cells[row, column];

                if (cell.IsOccupied)
                    word.Append(cell.Letter);
            }

            return word.ToString();
        }

        public void RestoreState(string[,] savedGrid)
        {
            if (savedGrid == null || savedGrid.GetLength(0) != Rows || savedGrid.GetLength(1) != Columns)
                return;

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    string cellState = savedGrid[row, column];

                    if (!string.IsNullOrEmpty(cellState))
                        _cells[row, column].SetLetter(cellState[0]);
                    else
                        _cells[row, column].Clear();
                }
            }
        }

        public void ClearAllCells()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    _cells[row, column].Clear();
                }
            }
        }

        public void ClearCell(int row, int column) => _cells[row, column].Clear();

        private void InitializeCells()
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int column = 0; column < _columns; column++)
                {
                    _cells[row, column] = new WordSlotCell(row, column);
                }
            }
        }
    }
}