using System;
using System.Collections.Generic;
using CodeBase.UI.WordSlots;

namespace CodeBase.Gameplay.WordSlots
{
    public interface IWordSlotService
    {
        void SetCurrentWordSlotHolder(WordSlotHolder wordSlotHolder);
        IEnumerable<WordSlot> GetWordSlotsByRow(int row);
        WordSlot GetTargetSlot(int index);
        int IndexOf(WordSlot wordSlot);
        int SlotCount { get; }
        int MaxLettersInWord { get; }
        IReadOnlyList<string> WordsToFind { get; }
        bool NewWordFormed { get; }
        IObservable<bool> OnValidationResult { get; }
        void SetTargetWordsToFind(IEnumerable<string> words);
        int GetRowBySlot(WordSlot slot);
        int GetColumnBySlot(WordSlot slot);
        WordSlot GetWordSlotByRowAndColumn(int row, int column);
        bool ValidateFormedWords();
        void Cleanup();
        IReadOnlyDictionary<int, string> GetFormedWords();
    }
}