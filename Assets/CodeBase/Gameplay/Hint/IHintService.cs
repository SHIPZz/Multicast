using System;

namespace CodeBase.Gameplay.Hint
{
    public interface IHintService
    {
        IObservable<string> OnHintShown { get; }
        IObservable<int> OnHintsCountChanged { get; }
        
        int RemainingHints { get; }
        void Initialize();
        void ShowHint();
        bool CanShowHint();
    }
} 