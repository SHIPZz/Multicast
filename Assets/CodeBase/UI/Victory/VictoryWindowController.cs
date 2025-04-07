using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.Infrastructure.States.States;
using CodeBase.UI.Controllers;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Services;
using UniRx;

namespace CodeBase.UI.Victory
{
    public class VictoryWindowController : IController<VictoryWindow>
    {
        private readonly IWindowService _windowService;
        private readonly IStateMachine _stateMachine;
        private readonly CompositeDisposable _disposables = new();
        private readonly ISoundService _soundService;

        private VictoryWindow _window;

        public VictoryWindowController(IWindowService windowService,
            ISoundService soundService,
            IStateMachine stateMachine)
        {
            _soundService = soundService;
            _stateMachine = stateMachine;
            _windowService = windowService;
        }

        public void Initialize()
        {
            _soundService.Play(SoundTypeId.Victory);
            
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
            
            _stateMachine.Enter<CleanupBeforeLoadingGameState>();
        }

        private void OnMainMenuClicked()
        {
            _windowService.Close<VictoryWindow>();
            
            _stateMachine.Enter<LoadingMenuState>();
        }
    }
} 