using System;
using System.Linq;
using CodeBase.Gameplay.Cluster;
using UniRx;
using Zenject;

namespace CodeBase.Gameplay.Hint
{
    public class HintService : IHintService, IInitializable
    {
        private const int MaxHintsPerLevel = 3;
        
        private readonly IClusterService _clusterService;
        private readonly Subject<string> _onHintShown = new();
        private readonly Subject<int> _onHintsCountChanged = new();
        
        private int _remainingHints;

        public IObservable<string> OnHintShown => _onHintShown;
        public IObservable<int> OnHintsCountChanged => _onHintsCountChanged;
        
        public int RemainingHints => _remainingHints;

        public HintService(IClusterService clusterService)
        {
            _clusterService = clusterService;
        }

        public void Initialize()
        {
            _remainingHints = MaxHintsPerLevel;
            _onHintsCountChanged.OnNext(_remainingHints);
        }

        public void ShowHint()
        {
            if (!CanShowHint()) return;

            var words = _clusterService.GetCurrentWords();
            
            if (words.Any())
            {
                var firstLetters = words.Select(w => w.Length >= 2 ? w.Substring(0, 2).ToUpper() : w.ToUpper());
                string hint = $"Подсказка: {string.Join(", ", firstLetters)}";
                _onHintShown.OnNext(hint);
                UseHint();
            }
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
    }
} 