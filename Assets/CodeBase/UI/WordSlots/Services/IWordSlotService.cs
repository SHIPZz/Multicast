using System.Collections.Generic;
using CodeBase.UI.WordSlots.Services.Grid;

namespace CodeBase.UI.WordSlots.Services
{
    public interface IWordSlotService
    {
        void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder);
        WordSlot GetTargetSlot(int index);
        int IndexOf(WordSlot wordSlot);
        int SlotCount { get; }
        IReadOnlyCollection<string> WordsToFind { get; }
        WordSlotGrid Grid { get; }
        void SetTargetWordsToFind(IEnumerable<string> words);
        int GetRowBySlot(WordSlot slot);
        int GetColumnBySlot(WordSlot slot);
        WordSlot GetWordSlotByRowAndColumn(int row, int column);
        bool AreWordsFormedCorrectly();
        void Cleanup();
        IReadOnlyDictionary<int, string> GetFormedWords();
        bool ContainsInTargetWords(string word);
        bool UpdateFormedWordsAndCheckNew();
        void RefreshFormedWords();
        void UpdateCell(int row, int column, char letter);
        void ClearCell(int row, int column);
    }
}