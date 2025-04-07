using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.Gameplay.Constants;
using UnityEngine;
using UnityEngine.Pool;

namespace CodeBase.UI.WordSlots.Services.Repository
{
    public class WordSlotRepository : IWordSlotRepository
    {
        private readonly List<string> _targetWordsToFind = new(GameplayConstants.MaxWordCount);
        private readonly Dictionary<int, Dictionary<int, WordSlot>> _wordSlotsByRowAndColumn = new(GameplayConstants.MaxWordCount);
        private readonly Dictionary<int, string> _formedWords = new(GameplayConstants.MaxWordCount);

        private WordSlotHolder _wordSlotHolder;

        public void Initialize()
        {
            foreach (KeyValuePair<int, Dictionary<int, WordSlot>> row in _wordSlotsByRowAndColumn)
            {
                _wordSlotsByRowAndColumn[row.Key] = new Dictionary<int, WordSlot>(GameplayConstants.MaxClustersInColumn);
            }
        }

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
                if (_targetWordsToFind.Count >= GameplayConstants.MaxWordCount)
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

            AddWordFromToListSlots();

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

            SaveWordSlotsByRowAndColumn(playerData);
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
            foreach (var (column, letter) in rowData.Value)
            {
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
            _wordSlotsByRowAndColumn[row][column] = slot;
        }

        private void SaveWordSlotsByRowAndColumn(PlayerData playerData)
        {
            foreach ((var rowId, Dictionary<int, WordSlot> columns) in _wordSlotsByRowAndColumn)
            {
                var columnDictionary = new Dictionary<int, string>();

                foreach ((var columnId, WordSlot slot) in columns)
                {
                    columnDictionary[columnId] = slot.IsOccupied ? slot.CurrentLetter.ToString() : string.Empty;
                }

                playerData.WordSlotsByRowAndColumns[rowId] = columnDictionary;
            }
        }

        private void AddWordFromToListSlots()
        {
            foreach ((var rowId, Dictionary<int, WordSlot> columns) in _wordSlotHolder.GetSlotsByRowAndColumn())
            {
                string word = "";

                foreach (var slot in columns.Values)
                {
                    if (slot.IsOccupied)
                        word += slot.CurrentLetter;
                }

                if (!string.IsNullOrEmpty(word))
                {
                    _formedWords[rowId] = word;
                }
            }
        }
    }
}