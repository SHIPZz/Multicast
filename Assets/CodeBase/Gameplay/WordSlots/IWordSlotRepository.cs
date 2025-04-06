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
        void SetTargetWords(IEnumerable<string> words);

        void Clear();
        bool FormedWordCountSameTargetWordCount();
        bool AllWordsFound();
        void RefreshFormedWords();
    }
} 