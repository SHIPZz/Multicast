using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.Infrastructure.States.States;
using CodeBase.UI.Controllers;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Settings;
using UniRx;

namespace CodeBase.UI.Menu
{
    public class MenuWindowController : IController<MenuWindow>
    {
        private readonly IWindowService _windowService;
        private readonly IStateMachine _stateMachine;
        private readonly CompositeDisposable _disposables = new();
        
        private MenuWindow _window;

        public MenuWindowController(IWindowService windowService, IStateMachine stateMachine)
        {
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