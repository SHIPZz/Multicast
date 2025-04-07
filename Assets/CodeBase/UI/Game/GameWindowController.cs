using CodeBase.Data;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.Infrastructure.States.States;
using CodeBase.UI.Cluster.Services;
using CodeBase.UI.Controllers;
using CodeBase.UI.Hint;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Services;
using CodeBase.UI.Victory;
using CodeBase.UI.WordSlots.Services;
using UniRx;

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
            
            _window.CreateClusterItems(levelData.Clusters);
            
            _clusterService.RestorePlacedClusters();
        }

        private void OnValidateClicked()
        {
            if (_wordSlotService.UpdateFormedWordsAndCheckNew())
                _soundService.Play(SoundTypeId.WordFormedFound);
                
            _clusterService.CheckAndHideFilledClusters();
            
            if(!_wordSlotService.ValidateFormedWords())
                return;
            
            _wordSlotService.Cleanup();
            _clusterService.Cleanup();
            
            _levelService.MarkLevelCompleted();
        }

        private void OnHintClicked() => _windowService.OpenWindow<HintWindow>(true);

        private void OnLevelCompleted()
        {
            _window.SetInteractableItemsActive(false);

            _windowService.OpenWindow<VictoryWindow>(onTop: true);
        }
    }
}