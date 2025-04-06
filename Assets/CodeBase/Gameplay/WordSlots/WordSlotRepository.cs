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

        private WordSlotHolder _wordSlotHolder;

        public int SlotCount => _wordSlotHolder?.WordSlots.Count ?? 0;

        public void SetWordSlotHolder(WordSlotHolder wordSlotHolder)
        {
            _wordSlotHolder = wordSlotHolder;
        }

        public bool FormedWordCountSameTargetWordCount()
        {
            return _formedWords.Count == _targetWordsToFind.Count;
        }

        public bool AllWordsFound()
        {
            foreach (var word in _formedWords.Values)
            {
                Debug.Log($"formed word - {word}");
                
                if(!_targetWordsToFind.Contains(word,StringComparer.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        public void RefreshFormedWords()
        {
            _formedWords.Clear();
            
            GetFormedWords();
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

        public IReadOnlyDictionary<int, string> GetFormedWords()
        {
            if (_wordSlotHolder == null)
                return null;

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
                    _formedWords[row.Key] = word;
                }
            }

            return _formedWords;
        }

        public void Clear()
        {
            _targetWordsToFind.Clear();
            _wordSlotsByRowAndColumn.Clear();
            _formedWords.Clear();
        }

        public void Save(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;
            playerData.WordsToFind.Clear();
            playerData.WordSlotsByRowAndColumns.Clear();

            playerData.WordsToFind.AddRange(_targetWordsToFind);

            foreach (KeyValuePair<int, Dictionary<int, WordSlot>> row in _wordSlotsByRowAndColumn)
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

            foreach (KeyValuePair<int, Dictionary<int, string>> rowData in progressData.PlayerData.WordSlotsByRowAndColumns)
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
                        slot.SetText(letter[0]);
                        UpdateWordSlot(row, column, slot);
                    }
                }
            }
        }

        private void UpdateWordSlot(int row, int column, WordSlot slot)
        {
            if (!_wordSlotsByRowAndColumn.ContainsKey(row))
            {
                _wordSlotsByRowAndColumn[row] = new Dictionary<int, WordSlot>(8);
            }

            _wordSlotsByRowAndColumn[row][column] = slot;
        }
    }
}