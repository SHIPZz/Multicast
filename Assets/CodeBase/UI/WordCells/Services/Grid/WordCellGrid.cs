using System.Collections.Generic;
using System.Text;
using CodeBase.UI.WordCells.Services.Cell;

namespace CodeBase.UI.WordCells.Services.Grid
{
    public readonly struct WordCellGrid
    {
        private readonly WordCell[,] _cells;
        private readonly int _rows;
        private readonly int _columns;

        public int Rows => _rows;
        public int Columns => _columns;
        public int SlotCount => _rows * _columns;

        public WordCellGrid(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            _cells = new WordCell[rows, columns];

            InitializeCells();
        }

        public WordCell GetCell(int row, int column)
        {
            if (row < 0 || row >= _rows || column < 0 || column >= _columns)
                return new WordCell();

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
            var targetWords = new Dictionary<int, string>();
            
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                string targetWord = GetWordInRow(i);
                targetWords[i] = targetWord;
            }

            return targetWords;
        }

        public void Load(string[,] savedGrid)
        {
            if (savedGrid == null || savedGrid.GetLength(0) != Rows || savedGrid.GetLength(1) != Columns)
                return;

            for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
            {
                string cellState = savedGrid[row, column];

                if (!string.IsNullOrEmpty(cellState))
                    _cells[row, column].SetLetter(cellState[0]);
                else
                    _cells[row, column].Clear();
            }
        }

        public void ClearAllCells()
        {
            for (int row = 0; row < Rows; row++)
            for (int column = 0; column < Columns; column++)
                _cells[row, column].Clear();
        }

        public void ClearCell(int row, int column) => _cells[row, column].Clear();

        private void InitializeCells()
        {
            for (int row = 0; row < _rows; row++)
            for (int column = 0; column < _columns; column++)
                _cells[row, column] = new WordCell(row, column);
        }

        private string GetWordInRow(int row)
        {
            StringBuilder word = new StringBuilder();

            for (int column = 0; column < _columns; column++)
            {
                WordCell cell = _cells[row, column];

                if (cell.IsOccupied)
                    word.Append(cell.Letter);
            }

            return word.ToString();
        }
    }
}