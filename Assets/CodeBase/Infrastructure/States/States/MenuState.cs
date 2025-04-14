using System;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.UI.Menu;
using CodeBase.UI.Services.Window;

namespace CodeBase.Infrastructure.States.States
{
    public class MenuState : IState
    {
        private readonly IWindowService _windowService;

        public MenuState(IWindowService windowService)
        {
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public void Enter()
        {
            _windowService.CloseAll();
            
            _windowService.OpenWindow<MenuWindow>();
        }

        public void Exit()
        {
            
        }
    }
}