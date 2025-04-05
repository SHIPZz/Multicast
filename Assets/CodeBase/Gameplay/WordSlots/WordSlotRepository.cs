using System.Collections.Generic;
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

        public void AddTargetWord(string word)
        {
            if (_targetWordsToFind.Count >= MaxWordCount)
            {
                Debug.LogWarning($"Cannot add more than {MaxWordCount} target words");
                return;
            }
            
            _targetWordsToFind.Add(word);
        }

        public void SetTargetWords(IEnumerable<string> words)
        {
            _targetWordsToFind.Clear();
            foreach (var word in words)
            {
                if (_targetWordsToFind.Count >= MaxWordCount)
                    break;
                    
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

        public WordSlot GetWordSlot(int row, int column)
        {
            if (_wordSlotsByRowAndColumn.TryGetValue(row, out var rowSlots) &&
                rowSlots.TryGetValue(column, out var slot))
            {
                return slot;
            }
            
            return null;
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

        private void UpdateFormedWord(int row, string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                _formedWords.Remove(row);
            }
            else
            {
                _formedWords[row] = word;
            }
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
                        slot.SetLetter(letter[0]);
                        UpdateWordSlot(row, column, slot);
                    }
                }
            }
        }
    }
} 