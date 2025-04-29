using System;

namespace CodeBase.UI.Hint
{
    public interface IHintService
    {
        IObservable<string> OnHintShown { get; }
        IObservable<int> OnHintsCountChanged { get; }
        
        int RemainingHints { get; }
        void Init();
        void ShowHint();
    }
} 