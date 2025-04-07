using System.Collections.Generic;

namespace CodeBase.UI.WordSlots.Services
{
    public interface IWordSlotService
    {
        void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder);
        WordSlot GetTargetSlot(int index);
        int IndexOf(WordSlot wordSlot);
        int SlotCount { get; }
        int MaxLettersInWord { get; }
        IReadOnlyCollection<string> WordsToFind { get; }
        void SetTargetWordsToFind(IEnumerable<string> words);
        int GetRowBySlot(WordSlot slot);
        int GetColumnBySlot(WordSlot slot);
        WordSlot GetWordSlotByRowAndColumn(int row, int column);
        bool ValidateFormedWords();
        void Cleanup();
        IReadOnlyDictionary<int, string> GetFormedWords();
        bool ContainsInTargetWords(string word);
        bool UpdateFormedWordsAndCheckNew();
        void RefreshFormedWords();
    }
}