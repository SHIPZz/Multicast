using System;
using System.Collections.Generic;
using CodeBase.Common.Services.Persistent;
using CodeBase.UI.WordSlots.Services.Repository;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.WordSlots.Services
{
    public class WordSlotService : IWordSlotService, IInitializable, IDisposable
    {
        private readonly HashSet<string> _correctlyFormedWords = new(StringComparer.OrdinalIgnoreCase);
        private readonly IWordSlotRepository _repository = new WordSlotRepository();
        private readonly IPersistentService _persistentService;

        private WordSlotHolder _wordSlotHolder;
        
        public WordSlotService(IPersistentService persistentService)
        {
            _persistentService = persistentService;
        }

        public int SlotCount => _repository.SlotCount;

        public int MaxLettersInWord
        {
            get
            {
                int maxLength = 0;

                foreach (var word in _repository.GetTargetWords())
                {
                    if (word.Length > maxLength)
                        maxLength = word.Length;
                }

                return maxLength;
            }
        }

        public IReadOnlyCollection<string> WordsToFind => _repository.GetTargetWords();

        public void Initialize()
        {
            _persistentService.RegisterProgressWatcher(_repository);
        }

        public void Dispose()
        {
            _persistentService.UnregisterProgressWatcher(_repository);
        }

        public bool ValidateFormedWords()
        {
            return _repository.FormedWordCountSameTargetWordCount() && _repository.AllWordsFound();
        }

        public bool UpdateFormedWordsAndCheckNew()
        {
            IReadOnlyDictionary<int, string> currentFormedWords = _repository.GetFormedWords();
            var newWordFormed = false;

            foreach (var word in currentFormedWords.Values)
            {
                if (!_correctlyFormedWords.Contains(word) && ContainsInTargetWords(word))
                {
                    _correctlyFormedWords.Add(word);
                    newWordFormed = true;
                }
            }

            return newWordFormed;
        }

        public void RefreshFormedWords()
        {
            _repository.RefreshFormedWords();
        }

        public void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder)
        {
            _wordSlotHolder = wordSlotHolder;
            _repository.SetWordSlotHolder(wordSlotHolder);
        }

        public int GetRowBySlot(WordSlot slot)
        {
            for (int row = 0; row < _wordSlotHolder.GridRows; row++)
            {
                for (int col = 0; col < _wordSlotHolder.GridColumns; col++)
                {
                    if (_wordSlotHolder.GetSlotByRowAndColumn(row, col) == slot)
                        return row;
                }
            }

            return -1;
        }

        public int GetColumnBySlot(WordSlot slot)
        {
            return _wordSlotHolder.GetColumnIndex(slot);
        }

        public WordSlot GetWordSlotByRowAndColumn(int row, int column)
        {
            return _wordSlotHolder.GetSlotByRowAndColumn(row, column);
        }

        public WordSlot GetTargetSlot(int index) => _wordSlotHolder.WordSlots[index];

        public int IndexOf(WordSlot wordSlot) => _wordSlotHolder.IndexOf(wordSlot);

        public void SetTargetWordsToFind(IEnumerable<string> words) => _repository.SetTargetWords(words);

        public void Cleanup() => _repository.Clear();

        public IReadOnlyDictionary<int, string> GetFormedWords() => _repository.GetFormedWords();

        public bool ContainsInTargetWords(string word)
        {
            IReadOnlyCollection<string> targetWords = _repository.GetTargetWords();

            foreach (string targetWord in targetWords)
            {
                Debug.Log($"{targetWord} - {word} compare - {targetWord.Equals(word, StringComparison.OrdinalIgnoreCase)}");
                
                if (targetWord.Equals(word, StringComparison.OrdinalIgnoreCase))
                    return true;
            }


            return false;
        }
    }
}