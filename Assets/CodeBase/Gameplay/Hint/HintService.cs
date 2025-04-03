using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.WordSlots;
using UniRx;

namespace CodeBase.Gameplay.Hint
{
    public class HintService : IHintService, IDisposable
    {
        private const int MaxHintsPerLevel = 3;

        private readonly Subject<string> _onHintShown = new();
        private readonly Subject<int> _onHintsCountChanged = new();
        private readonly ILevelService _levelService;
        private readonly IWordSlotService _wordSlotService;
        private readonly CompositeDisposable _disposables = new();

        private int _remainingHints;
        private string _hint;

        public IObservable<string> OnHintShown => _onHintShown;
        public IObservable<int> OnHintsCountChanged => _onHintsCountChanged;

        public int RemainingHints => _remainingHints;

        public HintService(IWordSlotService wordSlotService, ILevelService levelService)
        {
            _wordSlotService = wordSlotService;
            _levelService = levelService;
        }

        public void Initialize()
        {
            SetRemainingHints(MaxHintsPerLevel);

            _levelService
                .OnLevelCompleted
                .Subscribe(_ => SetRemainingHints(MaxHintsPerLevel))
                .AddTo(_disposables);


            IEnumerable<string> firstTwoLettersOfWords = _wordSlotService.WordsToFind.Select(w => w.Length >= 2 ? w.Substring(0, 2).ToUpper() : w.ToUpper());
            _hint = $"Подсказка: {string.Join(", ", firstTwoLettersOfWords)}";
        }

        private void SetRemainingHints(int maxHintsPerLevel)
        {
            _remainingHints = maxHintsPerLevel;
            _onHintsCountChanged?.OnNext(_remainingHints);
        }

        public void ShowHint()
        {
            if (!CanShowHint())
                return;

            _onHintShown.OnNext(_hint);
            
            UseHint();
        }

        public bool CanShowHint()
        {
            return _remainingHints > 0;
        }

        private void UseHint()
        {
            _remainingHints--;
            _onHintsCountChanged.OnNext(_remainingHints);
        }

        public void Dispose()
        {
            _onHintShown?.Dispose();
            _onHintsCountChanged?.Dispose();
            _disposables?.Dispose();
        }
    }
}