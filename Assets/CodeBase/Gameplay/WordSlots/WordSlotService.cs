using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using CodeBase.UI.WordSlots;
using UniRx;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.WordSlots
{
    //todo refactor: optimize, improve
    public class WordSlotService : IWordSlotService, IInitializable, IDisposable, IProgressWatcher
    {
        private const int MaxWordCount = 4;

        private readonly List<string> _targetWordsToFind = new();
        private readonly Subject<bool> _onValidationResult = new();
        private readonly IPersistentService _persistentService;
        private Dictionary<int, Dictionary<int, WordSlot>> _wordsByColumnAndRow = new();

        private WordSlotHolder _wordSlotHolder;

        public WordSlotService(IPersistentService persistentService)
        {
            _persistentService = persistentService;
        }

        public int LastFormedWordCount { get; private set; }

        public int SlotCount => _wordSlotHolder.WordSlots.Count;

        public IObservable<bool> OnValidationResult => _onValidationResult;

        private bool HasFormedWords => GetFormedWords().Count > 0;

        private int FormedWordCount => GetFormedWords().Count;

        public bool NewWordFormed => LastFormedWordCount != FormedWordCount && HasFormedWords;

        public int MaxLettersInWord
        {
            get
            {
                int maxLength = 0;

                foreach (var word in _targetWordsToFind)
                {
                    if (word.Length > maxLength)
                        maxLength = word.Length;
                }

                return maxLength;
            }
        }

        public IReadOnlyList<string> WordsToFind => _targetWordsToFind;

        public void Initialize()
        {
            _persistentService.RegisterProgressWatcher(this);
        }

        public void Dispose()
        {
            _persistentService.UnregisterProgressWatcher(this);
        }

        public void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder) => _wordSlotHolder = wordSlotHolder;

        public IEnumerable<WordSlot> GetWordSlotsByRow(int row)
        {
           return _wordSlotHolder.GetSlotsByRow(row);
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

        public void SetTargetWordsToFind(IEnumerable<string> words)
        {
            foreach (string word in words)
            {
                if (_targetWordsToFind.Count >= MaxWordCount)
                    break;

                Debug.Log($"Target word: {word}");

                _targetWordsToFind.Add(word);
            }
        }

        public void Cleanup()
        {
            _targetWordsToFind.Clear();
            _wordsByColumnAndRow.Clear();
            LastFormedWordCount = 0;
        }

        public bool ValidateFormedWords()
        {
            var formedWords = GetFormedWords();
            IReadOnlyList<string> wordsToFind = WordsToFind;

            if (formedWords.Count != WordsToFind.Count)
            {
                _onValidationResult.OnNext(false);
                return false;
            }

            foreach (var word in formedWords.Values)
            {
                if (!wordsToFind.Contains(word, StringComparer.OrdinalIgnoreCase))
                {
                    _onValidationResult.OnNext(false);
                    return false;
                }
            }

            _onValidationResult.OnNext(true);
            return true;
        }

        public IReadOnlyDictionary<int, string> GetFormedWords()
        {
            var formedWords = new Dictionary<int, string>();

            Debug.Log("who");

            foreach (KeyValuePair<int, Dictionary<int, WordSlot>> row in _wordSlotHolder.GetSlotsByRowAndColumn())
            {
                string word = "";

                foreach (var slot in row.Value.Values)
                {
                    if (slot.IsOccupied)
                        word += slot.CurrentLetter;
                }

                if (!string.IsNullOrEmpty(word))
                    formedWords[row.Key] = word;
            }

            LastFormedWordCount = formedWords.Count;

            return formedWords;
        }

        public void Save(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;
            playerData.WordsToFind.Clear();
            playerData.FormedWordsByRows.Clear();
            
            FillFormedWordsFromHolder();
            
            IReadOnlyDictionary<int, string> formedWords = GetFormedWords();

            playerData.WordsToFind.AddRange(_targetWordsToFind);

            foreach (KeyValuePair<int, string> keyValue in formedWords)
                playerData.FormedWordsByRows[keyValue.Key] = keyValue.Value;

            foreach (KeyValuePair<int, string> word in formedWords)
            {
                Debug.Log($"{word.Value} - saved word");
            }
        }

        public void Load(ProgressData progressData)
        {
            SetTargetWordsFromData(progressData);
            SetFormedWordsFromData(progressData);
        }

        private void SetTargetWordsFromData(ProgressData progressData)
        {
            _targetWordsToFind.AddRange(progressData.PlayerData.WordsToFind);
        }

        private void SetFormedWordsFromData(ProgressData progressData)
        {
            foreach (var word in progressData.PlayerData.FormedWordsByRows)
            {
                if (!_wordsByColumnAndRow.ContainsKey(word.Key))
                    _wordsByColumnAndRow[word.Key] = new Dictionary<int, WordSlot>();
            }
        }

        private void FillFormedWordsFromHolder()
        {
            if (_wordSlotHolder == null)
                return;

            var slotsByRowAndColumn = _wordSlotHolder.GetSlotsByRowAndColumn();
            _wordsByColumnAndRow = new Dictionary<int, Dictionary<int, WordSlot>>(slotsByRowAndColumn);
        }
    }
}