using System;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.UI.LoadingCurtains;
using CodeBase.UI.Menu;
using CodeBase.UI.Services.Window;

namespace CodeBase.Infrastructure.States.States
{
    public class MenuState : IState
    {
        private readonly IWindowService _windowService;
        private readonly ILoadingCurtain _loadingCurtain;

        public MenuState(IWindowService windowService, ILoadingCurtain loadingCurtain)
        {
            _loadingCurtain = loadingCurtain;
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public void Enter()
        {
            _windowService.OpenWindow<MenuWindow>();
            _loadingCurtain.Hide();
        }

        public void Exit()
        {
            
        }
    }
}