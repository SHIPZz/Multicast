using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.UI.WordSlots;
using UnityEngine;

namespace CodeBase.Gameplay.WordSlots
{
    public class WordSlotRepository : IWordSlotRepository
    {
        private const int MaxWordCount = 4;

        private readonly List<string> _targetWordsToFind = new();
        private readonly Dictionary<int, Dictionary<int, WordSlot>> _wordSlotsByRowAndColumn = new();
        private readonly Dictionary<int, string> _formedWords = new();
        private readonly Dictionary<int, string> _previousFormedWords = new();

        private WordSlotHolder _wordSlotHolder;

        public int SlotCount => _wordSlotHolder?.WordSlots.Count ?? 0;

        public bool NewWordFormed
        {
            get
            {
                var currentFormedWords = GetFormedWords();

                bool hasNewWord = currentFormedWords.Count > _previousFormedWords.Count;

                _previousFormedWords.Clear();

                foreach (var word in currentFormedWords)
                {
                    _previousFormedWords[word.Key] = word.Value;
                }

                return hasNewWord;
            }
        }

        public void SetWordSlotHolder(WordSlotHolder wordSlotHolder)
        {
            _wordSlotHolder = wordSlotHolder;
        }

        public bool FormedWordCountLessTargetWordCount()
        {
            if (_formedWords.Count != _targetWordsToFind.Count)
            {
                return true;
            }

            return false;
        }

        public bool AllWordsFound()
        {
            var isWordFound = false;

            foreach (var word in _formedWords.Values)
            {
                isWordFound = WordsMatchIgnoringCase(word, _targetWordsToFind);
            }

            return isWordFound;
        }

        public void SetTargetWords(IEnumerable<string> words)
        {
            _targetWordsToFind.Clear();

            foreach (string word in words)
            {
                if (_targetWordsToFind.Count >= MaxWordCount)
                    break;

                Debug.Log($"target word to find: {word}");
                _targetWordsToFind.Add(word);
            }
        }

        public IReadOnlyList<string> GetTargetWords() => _targetWordsToFind;

        public void UpdateWordSlot(int row, int column, WordSlot slot)
        {
            if (!_wordSlotsByRowAndColumn.ContainsKey(row))
            {
                _wordSlotsByRowAndColumn[row] = new Dictionary<int, WordSlot>();
            }

            _wordSlotsByRowAndColumn[row][column] = slot;
        }

        public IReadOnlyDictionary<int, string> GetFormedWords()
        {
            var formedWords = new Dictionary<int, string>();

            if (_wordSlotHolder == null)
                return formedWords;

            foreach (KeyValuePair<int, Dictionary<int, WordSlot>> row in _wordSlotHolder.GetSlotsByRowAndColumn())
            {
                string word = "";

                foreach (var slot in row.Value.Values)
                {
                    if (slot.IsOccupied)
                        word += slot.CurrentLetter;
                }

                if (!string.IsNullOrEmpty(word))
                {
                    formedWords[row.Key] = word;
                    _formedWords[row.Key] = word;
                }
            }

            return formedWords;
        }

        public void Clear()
        {
            _targetWordsToFind.Clear();
            _wordSlotsByRowAndColumn.Clear();
            _formedWords.Clear();
            _previousFormedWords.Clear();
        }

        public bool WordsMatchIgnoringCase(string formedWord, IEnumerable<string> targetWords)
        {
            if (targetWords.Contains(formedWord, StringComparer.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public void Save(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;
            playerData.WordsToFind.Clear();
            playerData.WordSlotsByRowAndColumns.Clear();

            playerData.WordsToFind.AddRange(_targetWordsToFind);

            foreach (var row in _wordSlotsByRowAndColumn)
            {
                Dictionary<int, string> rowData = new Dictionary<int, string>();

                foreach (KeyValuePair<int, WordSlot> column in row.Value)
                {
                    WordSlot slot = column.Value;
                    rowData[column.Key] = slot.IsOccupied ? slot.CurrentLetter.ToString() : string.Empty;
                }

                playerData.WordSlotsByRowAndColumns[row.Key] = rowData;
            }
        }

        public void Load(ProgressData progressData)
        {
            Clear();

            _targetWordsToFind.AddRange(progressData.PlayerData.WordsToFind);

            foreach (KeyValuePair<int, Dictionary<int, string>> rowData in progressData.PlayerData
                         .WordSlotsByRowAndColumns)
            {
                int row = rowData.Key;

                UpdateColumnsByRow(rowData, row);
            }
        }

        private void UpdateColumnsByRow(KeyValuePair<int, Dictionary<int, string>> rowData, int row)
        {
            foreach (KeyValuePair<int, string> columnData in rowData.Value)
            {
                int column = columnData.Key;
                string letter = columnData.Value;

                if (!string.IsNullOrEmpty(letter) && _wordSlotHolder != null)
                {
                    WordSlot slot = _wordSlotHolder.GetSlotByRowAndColumn(row, column);

                    if (slot != null)
                    {
                        slot.SetLetter(letter[0]);
                        UpdateWordSlot(row, column, slot);
                    }
                }
            }
        }
    }
}