using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Constants;
using UnityEngine;

namespace CodeBase.UI.WordSlots.Services.Repository
{
    public class WordSlotRepository : IWordSlotRepository
    {
        private readonly HashSet<string> _targetWordsToFind = new(GameplayConstants.MaxWordCount);
        private readonly Dictionary<int, string> _formedWords = new(GameplayConstants.MaxWordCount);

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

                if (!_targetWordsToFind.Contains(word, StringComparer.OrdinalIgnoreCase))
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

        public IReadOnlyCollection<string> GetTargetWords() => _targetWordsToFind;

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
            _formedWords.Clear();
        }

        public void Save(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;
            playerData.WordsToFind.Clear();

            playerData.WordsToFind.AddRange(_targetWordsToFind);
            SaveWordSlotsByRowAndColumn(playerData);
        }

        public void Load(ProgressData progressData)
        {
            Clear();

            _targetWordsToFind.AddRange(progressData.PlayerData.WordsToFind);
        }

        private void SaveWordSlotsByRowAndColumn(PlayerData playerData)
        {
            if (_wordSlotHolder == null)
                return;

            playerData.GridRows = _wordSlotHolder.GridRows;
            playerData.GridColumns = _wordSlotHolder.GridColumns;
            playerData.WordSlotsGrid = new string[playerData.GridRows, playerData.GridColumns];

            for (int row = 0; row < playerData.GridRows; row++)
            {
                for (int col = 0; col < playerData.GridColumns; col++)
                {
                    WordSlot slot = _wordSlotHolder.GetSlotByRowAndColumn(row, col);
                    
                    playerData.WordSlotsGrid[row, col] = slot?.IsOccupied == true ? slot.CurrentLetter.ToString() : string.Empty;
                }
            }
        }

        private void AddWordFromToListSlots()
        {
            if (_wordSlotHolder == null)
                return;

            for (int row = 0; row < _wordSlotHolder.GridRows; row++)
            {
                string word = "";

                for (int col = 0; col < _wordSlotHolder.GridColumns; col++)
                {
                    WordSlot slot = _wordSlotHolder.GetSlotByRowAndColumn(row, col);

                    if (slot?.IsOccupied == true)
                        word += slot.CurrentLetter;
                }

                if (!string.IsNullOrEmpty(word))
                    _formedWords[row] = word;
            }
        }
    }
}