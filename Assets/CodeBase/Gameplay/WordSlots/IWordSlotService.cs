using System.Collections.Generic;
using CodeBase.UI.WordSlots;

namespace CodeBase.Gameplay.WordSlots
{
    public interface IWordSlotService
    {
        void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder);
        IReadOnlyDictionary<int,string> GetFormedWordsFromRows();
        List<WordSlot> GetWordSlotsByRow(int row);
        WordSlot GetTargetSlot(int index);
        int IndexOf(WordSlot wordSlot);
        int SlotCount { get; }
        int MaxLettersInWord { get; }
        IReadOnlyList<string> WordsToFind { get; }
        bool HasFormedWords { get; }
        int FormedWordCount { get; }
        int LastFormedWordCount { get; }
        bool NewWordFormed { get; }
        void Init(IEnumerable<string> words);
    }
}