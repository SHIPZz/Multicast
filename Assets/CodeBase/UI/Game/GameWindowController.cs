using CodeBase.Common.Services.Sound;
using CodeBase.Data;
using CodeBase.Gameplay.Cluster;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.Hint;
using CodeBase.Gameplay.Sound;
using CodeBase.Gameplay.WordSlots;
using CodeBase.UI.Controllers;
using CodeBase.UI.Hint;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Victory;
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
        private readonly IClusterUIPlacementService _clusterUIPlacementService;
        private readonly IWordSlotService _wordSlotService;
        private readonly ISoundService _soundService;
        
        private GameWindow _window;

        public GameWindowController(
            ILevelService levelService,
            IClusterService clusterService,
            IClusterUIPlacementService clusterUIPlacementService,
            IWordSlotService wordSlotService,
            ISoundService soundService,
            IHintService hintService,
            IWindowService windowService)
        {
            _soundService = soundService;
            _wordSlotService = wordSlotService;
            _clusterUIPlacementService = clusterUIPlacementService;
            _hintService = hintService;
            _clusterService = clusterService;
            _levelService = levelService;
            _windowService = windowService;
        }

        public void Initialize()
        {
            _levelService.OnLevelLoaded
                .Subscribe(OnLevelLoaded)
                .AddTo(_disposables);

            _levelService.OnLevelCompleted
                .Subscribe(_ => OnLevelCompleted())
                .AddTo(_disposables);
            
            _hintService
                .OnHintsCountChanged
                .Subscribe(hintCount => _window.SetHintButtonActive(hintCount > 0))
                .AddTo(_disposables);

            _window.OnValidateClicked
                .Subscribe(_ => OnValidateClicked())
                .AddTo(_disposables);

            _window.OnHintClicked
                .Subscribe(_ => OnHintClicked())
                .AddTo(_disposables);
        }

        public void BindView(GameWindow window) => _window = window;

        public void Dispose() => _disposables?.Dispose();

        private void OnLevelLoaded(LevelData levelData)
        {
            _window.Cleanup();
            
            _window.SetInteractableItemsActive(true);
            
            _window.CreateWordSlotHolder();

            _window.CreateClusterItemHolder(_clusterService.GetAvailableClusters());
        }

        private void OnValidateClicked()
        {
            _clusterUIPlacementService.CheckAndHideFilledClusters();
            
            if(_wordSlotService.NewWordFormed)
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