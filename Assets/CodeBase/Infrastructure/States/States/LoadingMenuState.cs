using System;
using CodeBase.Infrastructure.Loading;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.Infrastructure.States.StateMachine;
using CodeBase.UI.Game;
using CodeBase.UI.Menu;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Settings;
using CodeBase.UI.Victory;

namespace CodeBase.Infrastructure.States.States
{
    public class LoadingMenuState : IState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly IStateMachine _stateMachine;
        private readonly IWindowService _windowService;

        public LoadingMenuState(ISceneLoader sceneLoader, 
            IStateMachine stateMachine,
            IWindowService windowService)
        {
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _sceneLoader = sceneLoader ?? throw new ArgumentNullException(nameof(sceneLoader));
        }

        public void Enter()
        {
            _windowService.CloseAll();
            _windowService.CleanupBindings();
            
            BindWindows();
            
            _sceneLoader.LoadScene(Scenes.Menu, () => _stateMachine.Enter<MenuState>());
        }

        private void BindWindows()
        {
            _windowService.Bind<GameWindow,GameWindowController>();
            _windowService.Bind<MenuWindow,MenuWindowController>();
            _windowService.Bind<SettingsWindow,SettingsWindowController>();
            _windowService.Bind<VictoryWindow,VictoryWindowController>();
        }

        public void Exit()
        {
            
        }
    }
}