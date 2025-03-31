using CodeBase.Common.Services.SaveLoad;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.Infrastructure.States.States;
using CodeBase.UI.Controllers;
using CodeBase.UI.Services.Window;
using UniRx;

namespace CodeBase.UI.Victory
{
    public class VictoryWindowController : IController<VictoryWindow>
    {
        private readonly IWindowService _windowService;
        private readonly ILevelService _levelService;
        private readonly IStateMachine _stateMachine;
        private readonly CompositeDisposable _disposables = new();
        private VictoryWindow _window;

        public VictoryWindowController(IWindowService windowService, ILevelService levelService, IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _levelService = levelService;
            _windowService = windowService;
        }

        public void Initialize()
        {
            _window.OnNextLevelClicked
                .Subscribe(_ => OnNextLevelClicked())
                .AddTo(_disposables);

            _window.OnMainMenuClicked
                .Subscribe(_ => OnMainMenuClicked())
                .AddTo(_disposables);
        }

        public void BindView(VictoryWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnNextLevelClicked()
        {
            _windowService.Close<VictoryWindow>();
            
            _levelService.UpdateLevel();
        }

        private void OnMainMenuClicked()
        {
            _windowService.Close<VictoryWindow>();
            
            _stateMachine.Enter<LoadingMenuState>();
        }
    }
} 