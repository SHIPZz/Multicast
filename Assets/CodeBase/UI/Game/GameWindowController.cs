using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Common.Services.Cluster;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.UI.Controllers;
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
        
        private GameWindow _window;

        public GameWindowController(
            ILevelService levelService,
            IClusterService clusterService,
            IWindowService windowService)
        {
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

            _window.OnValidateClicked
                .Subscribe(_ => OnValidateClicked())
                .AddTo(_disposables);
        }

        public void BindView(GameWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnLevelLoaded(LevelData levelData)
        {
            _window.ClearWordSlots();
            _window.ClearClusters();
            
            _window.CreateWordSlotHolder();

            _window.CreateClusterItemHolder(_clusterService.GetAvailableClusters());
        }

        private void OnValidateClicked()
        {
            _levelService.ValidateLevel();
        }

        private void OnLevelCompleted()
        {
            _window.HideClusters();
            _windowService.OpenWindow<VictoryWindow>();
        }
    }
} 