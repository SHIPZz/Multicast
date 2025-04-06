using System;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.Hint;
using CodeBase.Gameplay.WordSlots;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.UI.Game;
using CodeBase.UI.Services.Cluster;
using CodeBase.UI.Services.Window;

namespace CodeBase.Infrastructure.States.States
{
    public class GameState : IState
    {
        private readonly IWindowService _windowService;
        private readonly ILevelService _levelService;
        private readonly IHintService _hintService;
        private readonly IClusterService _clusterService;
        private readonly IWordSlotService _wordSlotService;

        public GameState(IWindowService windowService,
            IHintService hintService,
            ILevelService levelService,
            IWordSlotService wordSlotService,
            IClusterService clusterService)
        {
            _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
            _levelService = levelService ?? throw new ArgumentNullException(nameof(levelService));
            _wordSlotService = wordSlotService ?? throw new ArgumentNullException(nameof(wordSlotService));
            _clusterService = clusterService ?? throw new ArgumentNullException(nameof(clusterService));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
        }

        public void Enter()
        {
            LevelData currentLevelData = _levelService.GetCurrentLevel();

            if (_wordSlotService.WordsToFind.IsNullOrEmpty())
                _wordSlotService.SetTargetWordsToFind(currentLevelData.Words.Shuffle());

            if (_clusterService.GetAvailableClusters().IsNullOrEmpty())
                _clusterService.SetClusters(currentLevelData.Clusters);

            _hintService.Init();
            
            _clusterService.Init();

            _windowService.OpenWindow<GameWindow>();

            _levelService.MarkLevelLoaded(currentLevelData.LevelId);
        }

        public void Exit() { }
    }
}