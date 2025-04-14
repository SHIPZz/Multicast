using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordSlots.Services.Cell;
using CodeBase.UI.WordSlots.Services.Grid;
using Zenject;

namespace CodeBase.UI.WordSlots.Services
{
    public class WordSlotService : IWordSlotService, IInitializable, IDisposable, IProgressWatcher
    {
        private readonly HashSet<string> _correctlyFormedWords = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _targetWordsToFind = new(GameplayConstants.MaxWordCount, StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, string> _formedWords = new(GameplayConstants.MaxWordCount);
        private readonly WordSlotGrid _grid = new(GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn);
        private readonly IPersistentService _persistentService;

        private WordSlotHolder _wordSlotHolder;

        public WordSlotService(IPersistentService persistentService) => 
            _persistentService = persistentService;

        public int SlotCount => _grid.SlotCount;
        public IReadOnlyCollection<string> WordsToFind => _targetWordsToFind;
        public WordSlotGrid Grid => _grid;

        public void Initialize() => _persistentService.RegisterProgressWatcher(this);
        public void Dispose() => _persistentService.UnregisterProgressWatcher(this);

        public bool ValidateFormedWords() => 
            _formedWords.Count == _targetWordsToFind.Count && 
            _formedWords.Values.All(word => _targetWordsToFind.Contains(word));

        public bool UpdateFormedWordsAndCheckNew()
        {
            var currentFormedWords = GetFormedWords();
            if (currentFormedWords == null) return false;

            var newWordFormed = false;
            foreach (var word in currentFormedWords.Values)
            {
                if (!_correctlyFormedWords.Contains(word) && _targetWordsToFind.Contains(word))
                {
                    _correctlyFormedWords.Add(word);
                    newWordFormed = true;
                }
            }

            return newWordFormed;
        }

        public void RefreshFormedWords()
        {
            _formedWords.Clear();
            GetFormedWords();
        }

        public void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder) => 
            _wordSlotHolder = wordSlotHolder;

        public int GetRowBySlot(WordSlot slot) => slot?.Row ?? -1;
        public int GetColumnBySlot(WordSlot slot) => slot?.Column ?? -1;

        public WordSlot GetWordSlotByRowAndColumn(int row, int column) => 
            _wordSlotHolder?.GetSlotByRowAndColumn(row, column);

        public WordSlot GetTargetSlot(int index) => _wordSlotHolder?.WordSlots[index];
        public int IndexOf(WordSlot wordSlot) => _wordSlotHolder?.IndexOf(wordSlot) ?? -1;

        public void SetTargetWordsToFind(IEnumerable<string> words)
        {
            _targetWordsToFind.Clear();
            _targetWordsToFind.UnionWith(words);
        }

        public void Cleanup()
        {
            _targetWordsToFind.Clear();
            _formedWords.Clear();
            _grid.ClearAllCells();
        }

        public IReadOnlyDictionary<int, string> GetFormedWords()
        {
            for (int row = 0; row < _grid.Rows; row++)
            {
                var word = _grid.GetWordInRow(row);
                
                if (!string.IsNullOrEmpty(word))
                    _formedWords[row] = word;
            }

            return _formedWords;
        }

        public bool ContainsInTargetWords(string word) => 
            _targetWordsToFind.Contains(word);

        public WordSlotCell GetCell(int row, int column) => 
            _grid.GetCell(row, column);

        public void UpdateCell(int row, int column, char letter) => 
            _grid.UpdateCell(row, column, letter);

        public void ClearCell(int row, int column) => 
            _grid.ClearCell(row, column);

        public void Save(ProgressData progressData)
        {
            var playerData = progressData.PlayerData;
            playerData.WordsToFind.Clear();
            playerData.WordsToFind.AddRange(_targetWordsToFind);

            playerData.WordSlotsGrid = new string[GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn];

            for (int row = 0; row < playerData.WordSlotsGrid.GetLength(0); row++)
            {
                for (int col = 0; col < playerData.WordSlotsGrid.GetLength(1); col++)
                {
                    var cell = _grid.GetCell(row, col);
                    playerData.WordSlotsGrid[row, col] = cell.IsOccupied ? cell.Letter.ToString() : string.Empty;
                }
            }
        }

        public void Load(ProgressData progressData)
        {
            Cleanup();
            _targetWordsToFind.UnionWith(progressData.PlayerData.WordsToFind);

            if (progressData.PlayerData.WordSlotsGrid != null)
                _grid.RestoreState(progressData.PlayerData.WordSlotsGrid);
        }
    }
}