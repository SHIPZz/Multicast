using System.Collections.Generic;
using CodeBase.Common.Services.Persistent;
using CodeBase.UI.WordSlots;

namespace CodeBase.Gameplay.WordSlots
{
    public interface IWordSlotRepository : IProgressWatcher
    {
        int SlotCount { get; }
        IReadOnlyList<string> GetTargetWords();
        IReadOnlyDictionary<int, string> GetFormedWords();
        
        void SetWordSlotHolder(WordSlotHolder wordSlotHolder);
        void AddTargetWord(string word);
        void SetTargetWords(IEnumerable<string> words);
        void UpdateWordSlot(int row, int column, WordSlot slot);
        WordSlot GetWordSlot(int row, int column);
        void Clear();
    }
} 