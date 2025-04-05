using System.Collections.Generic;
using System.Linq;
using CodeBase.Common.Services.Sound;
using CodeBase.Data;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.Hint;
using CodeBase.Gameplay.Sound;
using CodeBase.Gameplay.WordSlots;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.Infrastructure.States.States;
using CodeBase.UI.Controllers;
using CodeBase.UI.Hint;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Victory;
using UniRx;
using UnityEngine;

namespace CodeBase.UI.Game
{
    public class GameWindowController : IController<GameWindow>
    {
        private readonly ILevelService _levelService;
        private readonly IWindowService _windowService;
        private readonly IClusterService _clusterService;
        private readonly CompositeDisposable _disposables = new();
        private readonly IHintService _hintService;
        private readonly IWordSlotService _wordSlotService;
        private readonly ISoundService _soundService;
        private readonly IStateMachine _stateMachine;

        private GameWindow _window;

        public GameWindowController(
            ILevelService levelService,
            IClusterService clusterService,
            IWordSlotService wordSlotService,
            ISoundService soundService,
            IHintService hintService,
            IWindowService windowService,
            IStateMachine stateMachine)
        {
            _soundService = soundService;
            _wordSlotService = wordSlotService;
            _hintService = hintService;
            _clusterService = clusterService;
            _levelService = levelService;
            _windowService = windowService;
            _stateMachine = stateMachine;
        }

        public void Initialize()
        {
            ProcessLevelServiceEvents();

            ProcessHintServiceEvent();

            ProcessWindowButtonsClicks();
        }

        public void BindView(GameWindow window) => _window = window;

        public void Dispose() => _disposables?.Dispose();

        private void ProcessLevelServiceEvents()
        {
            _levelService.OnLevelLoaded
                .Subscribe(OnLevelLoaded)
                .AddTo(_disposables);

            _levelService.OnLevelCompleted
                .Subscribe(_ => OnLevelCompleted())
                .AddTo(_disposables);
        }

        private void ProcessHintServiceEvent()
        {
            _hintService
                .OnHintsCountChanged
                .Subscribe(hintCount => _window.SetHintButtonActive(hintCount > 0))
                .AddTo(_disposables);
        }

        private void ProcessWindowButtonsClicks()
        {
            _window.OnValidateClicked
                .Subscribe(_ => OnValidateClicked())
                .AddTo(_disposables);

            _window.OnHintClicked
                .Subscribe(_ => OnHintClicked())
                .AddTo(_disposables);

            _window
                .OnRestartClicked
                .Subscribe(_ => _stateMachine.Enter<CleanupBeforeLoadingGameState>())
                .AddTo(_disposables);
        }

        private void OnLevelLoaded(LevelData levelData)
        {
            _window.Cleanup();

            _window.SetInteractableItemsActive(true);

            _window.CreateWordSlotHolder();

            _window.CreateClusterItemHolder();
            
            IEnumerable<string> placedClusters = _clusterService.GetPlacedClustersFromData();
            IEnumerable<string> availableClusters = _clusterService.GetAvailableClusters();

            IEnumerable<string> targetClusters = placedClusters.Union(availableClusters);

            _window.CreateClusterItems(targetClusters);
            
            _clusterService.RestorePlacedClusters();
        }

        private void OnValidateClicked()
        {
            _clusterService.CheckAndHideFilledClusters();

            if (_wordSlotService.NewWordFormed)
                _soundService.Play(SoundTypeId.WordFormedFound);

            _levelService.ValidateLevel();
        }

        private void OnHintClicked() => _windowService.OpenWindow<HintWindow>(true);

        private void OnLevelCompleted()
        {
            _window.SetInteractableItemsActive(false);

            _windowService.OpenWindow<VictoryWindow>(onTop: true);
        }
    }
}