using System.Collections.Generic;
using CodeBase.UI.WordSlots;
using UnityEngine;

namespace CodeBase.Gameplay.WordSlots
{
    public class WordSlotService : IWordSlotService
    {
        private const int MaxWordCount = 4;

        private readonly Dictionary<int, string> _formedWordsByRows = new();
        private readonly List<string> _targetWordsToFind = new();
        private WordSlotHolder _wordSlotHolder;

        public int LastFormedWordCount { get; private set; }
        
        public int SlotCount => _wordSlotHolder.WordSlots.Count;
        
        public bool HasFormedWords => FormedWordCount > 0;
        
        public int FormedWordCount => _formedWordsByRows.Count;
        
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

        public void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder) => _wordSlotHolder = wordSlotHolder;

        public List<WordSlot> GetWordSlotsByRow(int row)
        {
            IReadOnlyDictionary<int, List<WordSlot>> rowSlots = _wordSlotHolder.GetRowSlots();

            return rowSlots[row];
        }

        public WordSlot GetTargetSlot(int index) => _wordSlotHolder.WordSlots[index];

        public int IndexOf(WordSlot wordSlot) => _wordSlotHolder.IndexOf(wordSlot);
        
        public void Init(IEnumerable<string> words)
        {
            _targetWordsToFind.Clear();
            _formedWordsByRows.Clear();
            LastFormedWordCount = 0;

            foreach (var word in words)
            {
                if (_targetWordsToFind.Count >= MaxWordCount)
                    break;

                Debug.Log($"Target word: {word}");
                
                _targetWordsToFind.Add(word);
            }
        }

        public IReadOnlyDictionary<int, string> GetFormedWordsFromRows()
        {
            LastFormedWordCount = _formedWordsByRows.Count;
            
            IReadOnlyDictionary<int, List<WordSlot>> rowSlots = _wordSlotHolder.GetRowSlots();

            foreach (KeyValuePair<int, List<WordSlot>> row in rowSlots)
            {
                string formedWord = "";

                foreach (WordSlot slot in row.Value)
                {
                    if (slot.IsOccupied)
                        formedWord += slot.CurrentLetter;
                }

                if (formedWord != "")
                    _formedWordsByRows[row.Key] = formedWord;
            }
            

            return _formedWordsByRows;
        }
    }
}