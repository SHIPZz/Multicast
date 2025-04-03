using System;
using CodeBase.Common.Services.Persistent;
using CodeBase.Common.Services.SaveLoad;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Cluster;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.Hint;
using CodeBase.Gameplay.WordSlots;
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
        private readonly IHintService _hintService;
        private readonly IClusterService _clusterService;
        private readonly IWordSlotService _wordSlotService;

        public GameState(IWindowService windowService,
            ISaveLoadSystem saveLoadSystem,
            IHintService hintService,
            ILevelService levelService,
            IWordSlotService wordSlotService, 
            IClusterService clusterService)
        {
            _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
            _saveLoadSystem = saveLoadSystem ?? throw new ArgumentNullException(nameof(saveLoadSystem));
            _levelService = levelService ?? throw new ArgumentNullException(nameof(levelService));
            _wordSlotService = wordSlotService ?? throw new ArgumentNullException(nameof(wordSlotService));
            _clusterService = clusterService ?? throw new ArgumentNullException(nameof(clusterService));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public void Enter()
        {
            _windowService.OpenWindow<GameWindow>();

            LevelData currentLevelData = _levelService.GetCurrentLevel();
            
            _clusterService.Init(currentLevelData.Clusters.Shuffle());
            _wordSlotService.Init(currentLevelData.Words.Shuffle());

            _hintService.Initialize();
            
            _levelService.MarkLevelLoaded(currentLevelData.LevelId);
        }

        public void Exit() { }
    }
}