using System;
using System.Collections.Generic;
using CodeBase.Common.Services.Persistent;
using CodeBase.UI.WordSlots;
using Zenject;

namespace CodeBase.Gameplay.WordSlots
{
    public class WordSlotService : IWordSlotService, IInitializable, IDisposable
    {
        private readonly IWordSlotRepository _repository;
        private readonly IPersistentService _persistentService;
        
        private WordSlotHolder _wordSlotHolder;

        public WordSlotService(IPersistentService persistentService)
        {
            _persistentService = persistentService;
            _repository = new WordSlotRepository();
        }

        public int SlotCount => _repository.SlotCount;

        public bool NewWordFormed => _repository.NewWordFormed;

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

        public IReadOnlyList<string> WordsToFind => _repository.GetTargetWords();

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
            if (_repository.FormedWordCountLessTargetWordCount()) 
                return false;

            return _repository.AllWordsFound();
        }

        public void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder)
        {
            _wordSlotHolder = wordSlotHolder;
            _repository.SetWordSlotHolder(wordSlotHolder);
        }

        public int GetRowBySlot(WordSlot slot)
        {
            foreach (KeyValuePair<int, Dictionary<int, WordSlot>> row in _wordSlotHolder.GetSlotsByRowAndColumn())
            {
                if (row.Value.ContainsValue(slot))
                    return row.Key;
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

        public bool WordsMatchIgnoringCase(string formedWordValue, IEnumerable<string> wordsToFind) => _repository.WordsMatchIgnoringCase(formedWordValue, wordsToFind);

        public void Cleanup() => _repository.Clear();

        public IReadOnlyDictionary<int, string> GetFormedWords() => _repository.GetFormedWords();
    }
}