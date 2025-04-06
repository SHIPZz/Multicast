using CodeBase.Data;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.UI.Controllers;
using CodeBase.UI.Services.Window;
using UniRx;

namespace CodeBase.UI.Levels
{
    public class LevelWindowController : IController<LevelWindow>
    {
        private readonly IWindowService _windowService;
        private readonly ILevelService _levelService;
        private readonly CompositeDisposable _disposables = new();
        
        private LevelWindow _window;

        public LevelWindowController(IWindowService windowService, ILevelService levelService)
        {
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
        }

        public void BindView(LevelWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        private void OnLevelLoaded(LevelData levelData)
        {
            _window.SetLevelNumber(levelData.LevelId);
            _windowService.OpenWindow<LevelWindow>();
        }

        private void OnLevelCompleted()
        {
            _windowService.Close<LevelWindow>();
        }
    }
} 