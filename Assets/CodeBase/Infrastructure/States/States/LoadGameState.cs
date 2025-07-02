using System;
using CodeBase.Common.Services.InternetConnection;
using CodeBase.Infrastructure.Loading;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.UI.NoInternet;
using CodeBase.UI.Services.Window;

namespace CodeBase.Infrastructure.States.States
{
    public class LoadGameState : IState
    {
        private readonly IWindowService _windowService;
        private readonly ISceneLoader _sceneLoader;
        private readonly IStateMachine _stateMachine;
        private readonly IInternetConnectionService _internetConnectionService;

        public LoadGameState(IWindowService windowService,
            IInternetConnectionService internetConnectionService,
            ISceneLoader sceneLoader,
            IStateMachine stateMachine)
        {
            _internetConnectionService = internetConnectionService ?? throw new ArgumentNullException(nameof(internetConnectionService));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public void Enter()
        {
            if (!_internetConnectionService.IsInternetAvailable)
            {
                _windowService.OpenWindow<NoInternetWindow>();
                return;
            }
            
            _windowService.CloseAll();
            
            _sceneLoader.LoadScene(Scenes.Game, () =>_stateMachine.Enter<GameState>());
        }

        public void Exit()
        {
            
        }
    }
}