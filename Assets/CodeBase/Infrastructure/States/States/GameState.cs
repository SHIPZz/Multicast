using System;
using CodeBase.Common.Services.SaveLoad;
using CodeBase.Data;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.UI.Game;
using CodeBase.UI.Services.Window;

namespace CodeBase.Infrastructure.States.States
{
    public class GameState : IState
    {
        private readonly IWindowService _windowService;
        private readonly ILevelService _levelService;
        private readonly ISaveLoadSystem _saveLoadSystem;

        public GameState(IWindowService windowService, ISaveLoadSystem saveLoadSystem, ILevelService levelService)
        {
            _saveLoadSystem = saveLoadSystem;
            _levelService = levelService;
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public void Enter()
        {
            ProgressData progressData = _saveLoadSystem.Load();
            
            _windowService.OpenWindow<GameWindow>();
            
            _levelService.LoadLevel(progressData.PlayerData.Level);
        }

        public void Exit()
        {
            
        }
    }
}