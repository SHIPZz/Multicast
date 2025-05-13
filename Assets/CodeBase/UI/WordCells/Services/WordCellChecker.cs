using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Extensions;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordCells.Services.Grid;

namespace CodeBase.UI.WordCells.Services
{
    public class WordCellChecker : IWordCellChecker
    {
        private readonly HashSet<string> _correctlyFormedWords = new(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> _targetWordsToFind = new(GameplayConstants.MaxWordCount, StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, string> _totalFormedWords = new(GameplayConstants.MaxWordCount);
        
        private readonly IWordCellRepository _repository;

        public IReadOnlyCollection<string> TargetWordsToFind => _targetWordsToFind;

        public WordCellChecker(IWordCellRepository repository)
        {
            _repository = repository;
        }

        public bool AreWordsFormedCorrectly() =>
            _totalFormedWords.Count == _targetWordsToFind.Count &&
            _totalFormedWords.Values.All(word => _targetWordsToFind.Contains(word));

        public void Cleanup()
        {
            _correctlyFormedWords.Clear();
            _targetWordsToFind.Clear();
            _totalFormedWords.Clear();
        }

        public bool ContainsInTargetWords(string word) =>
            _targetWordsToFind.Contains(word);

        public void RefreshFormedWords()
        {
            _totalFormedWords.Clear();
            GetFormedWords();
        }

        public bool UpdateFormedWordsAndCheckNew()
        {
            IReadOnlyDictionary<int, string> currentFormedWords = GetFormedWords();

            if (currentFormedWords == null)
                return false;

            var newWordFormed = false;

            foreach (string word in currentFormedWords.Values)
            {
                if (!_correctlyFormedWords.Contains(word) && _targetWordsToFind.Contains(word))
                {
                    _correctlyFormedWords.Add(word);
                    newWordFormed = true;
                }
            }

            return newWordFormed;
        }

        public IReadOnlyDictionary<int, string> GetFormedWords()
        {
            WordCellGrid grid = _repository.Grid;

            _totalFormedWords.Clear();

            foreach (var (row, word) in grid.GetWordsWithRow())
            {
                _totalFormedWords.Add(row, word);
            }

            return _totalFormedWords;
        }

        public void Init(IEnumerable<string> words)
        {
            _targetWordsToFind.Clear();
            _targetWordsToFind.AddRange(words);
        }
    }
}