using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.SO.Hints;
using CodeBase.Gameplay.WordSlots;
using UniRx;
using Zenject;

namespace CodeBase.Gameplay.Hint
{
    public class HintService : IHintService, IInitializable, IDisposable
    {
        private const int MaxHintsPerLevel = 3;

        private readonly Subject<string> _onHintShown = new();
        private readonly Subject<int> _onHintsCountChanged = new();
        private readonly ILevelService _levelService;
        private readonly IWordSlotService _wordSlotService;
        private readonly CompositeDisposable _disposables = new();

        private int _remainingHints;
        private string _hint;
        private HintConfig _hintConfig;

        public IObservable<string> OnHintShown => _onHintShown;
        public IObservable<int> OnHintsCountChanged => _onHintsCountChanged;

        public int RemainingHints => _remainingHints;

        public HintService(IWordSlotService wordSlotService, ILevelService levelService, HintConfig hintConfig)
        {
            _hintConfig = hintConfig;
            _wordSlotService = wordSlotService;
            _levelService = levelService;
        }

        public void Init()
        {
            SetRemainingHints(MaxHintsPerLevel);

            IEnumerable<string> firstTwoLettersOfWords = _wordSlotService
                .WordsToFind
                .Select(w => w.Length >= _hintConfig.LettersToShow ? w.Substring(0, _hintConfig.LettersToShow).ToUpper() : w.ToUpper());
            
            _hint = $"{_hintConfig.Text} {string.Join(", ", firstTwoLettersOfWords)}";
        }

        public void Initialize()
        {
            _levelService
                .OnLevelCompleted
                .Subscribe(_ => SetRemainingHints(MaxHintsPerLevel))
                .AddTo(_disposables);
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