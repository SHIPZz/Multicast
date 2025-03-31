using System;
using CodeBase.Infrastructure.Loading;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.UI.Services.Window;

namespace CodeBase.Infrastructure.States.States
{
    public class LoadGameState : IState
    {
        private readonly IWindowService _windowService;
        private readonly ISceneLoader _sceneLoader;
        private readonly IStateMachine _stateMachine;

        public LoadGameState(IWindowService windowService, ISceneLoader sceneLoader, IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public void Enter()
        {
            _windowService.CloseAll();
            
            _sceneLoader.LoadScene(Scenes.Game, () =>_stateMachine.Enter<GameState>());
        }

        public void Exit()
        {
            
        }
    }
}