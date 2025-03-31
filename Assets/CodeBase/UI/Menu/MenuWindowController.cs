using CodeBase.Common.Services.SaveLoad;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.Infrastructure.States.States;
using CodeBase.UI.Controllers;
using CodeBase.UI.Game;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Settings;
using UniRx;

namespace CodeBase.UI.Menu
{
    public class MenuWindowController : IController<MenuWindow>
    {
        private readonly IWindowService _windowService;
        private MenuWindow _window;
        private readonly CompositeDisposable _disposables = new();
        private IStateMachine _stateMachine;
        private ISaveLoadSystem _saveLoadSystem;

        public MenuWindowController(IWindowService windowService, IStateMachine stateMachine, ISaveLoadSystem saveLoadSystem)
        {
            _saveLoadSystem = saveLoadSystem;
            _stateMachine = stateMachine;
            _windowService = windowService;
        }

        public void Initialize()
        {
            _window.OnPlayClicked
                .Subscribe(_ => OnPlayClicked())
                .AddTo(_disposables);

            _window.OnSettingsClicked
                .Subscribe(_ => OnSettingsClicked())
                .AddTo(_disposables);
        }

        public void BindView(MenuWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnPlayClicked()
        {
            _windowService.Close<MenuWindow>();
            
            _stateMachine.Enter<LoadGameState>();
        }

        private void OnSettingsClicked()
        {
            _windowService.Close<MenuWindow>();
            _windowService.OpenWindow<SettingsWindow>();
        }
    }
} 