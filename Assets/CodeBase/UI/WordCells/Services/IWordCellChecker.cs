using System.Collections.Generic;

namespace CodeBase.UI.WordCells.Services
{
    public interface IWordCellChecker
    {
        bool AreWordsFormedCorrectly();
        void Cleanup();
        bool UpdateFormedWordsAndCheckNew();
        void RefreshFormedWords();
        bool ContainsInTargetWords(string word);
        IReadOnlyDictionary<int, string> GetFormedWords();
        IReadOnlyCollection<string> TargetWordsToFind { get; }
        void Init(IEnumerable<string> words);
    }
}