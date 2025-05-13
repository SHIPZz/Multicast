using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.UI.Hint.Configs;
using CodeBase.UI.WordCells.Services;
using UniRx;
using Zenject;

namespace CodeBase.UI.Hint.Serviсes
{
    public class HintService : IHintService, IInitializable, IDisposable
    {
        private readonly Subject<string> _onHintShown = new();
        private readonly Subject<int> _onHintsCountChanged = new();
        private readonly ILevelService _levelService;
        private readonly IWordCellChecker _wordCellChecker;
        private readonly CompositeDisposable _disposables = new();
        private readonly HintConfig _hintConfig;

        private int _remainingHints;
        private string _hint;

        public IObservable<string> OnHintShown => _onHintShown;
        public IObservable<int> OnHintsCountChanged => _onHintsCountChanged;

        public int RemainingHints => _remainingHints;

        public HintService(IWordCellChecker wordCellChecker, ILevelService levelService, HintConfig hintConfig)
        {
            _hintConfig = hintConfig;
            _wordCellChecker = wordCellChecker;
            _levelService = levelService;
        }

        public void Init()
        {
            SetRemainingHints(_hintConfig.MaxHintsPerLevel);

            IEnumerable<string> firstTwoLettersOfWords = _wordCellChecker
                .TargetWordsToFind
                .Select(w => w.Length >= _hintConfig.LettersToShow ? w.Substring(0, _hintConfig.LettersToShow).ToUpper() : w.ToUpper());
            
            _hint = $"{_hintConfig.Text} {string.Join(", ", firstTwoLettersOfWords)}";
        }

        public void Initialize()
        {
            _levelService
                .OnLevelCompleted
                .Subscribe(_ => SetRemainingHints(_hintConfig.MaxHintsPerLevel))
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

        private bool CanShowHint() => _remainingHints > 0;
    }
}