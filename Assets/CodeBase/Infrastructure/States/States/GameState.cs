using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.Hint;
using CodeBase.Gameplay.WordSlots;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.UI.Game;
using CodeBase.UI.Levels;
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

            SetTargetWordsToFind(currentLevelData);

            SetClusters(currentLevelData);

            _hintService.Init();
            _clusterService.Init();
            _windowService.OpenWindow<GameWindow>();
            _windowService.OpenWindow<LevelWindow>();
            _levelService.MarkLevelLoaded(currentLevelData.LevelId);
        }

        private void SetTargetWordsToFind(LevelData currentLevelData)
        {
            if (_wordSlotService.WordsToFind.IsNullOrEmpty()) 
            {
                var capitalizedWords = currentLevelData.Words
                    // .Select(word => word.ToUpper())
                    .Shuffle();

                _wordSlotService.SetTargetWordsToFind(capitalizedWords);
            }
        }

        private void SetClusters(LevelData currentLevelData)
        {
            if (_clusterService.GetAvailableClusters().IsNullOrEmpty())
            {
                IEnumerable<string> capitalizedClusters = currentLevelData.Clusters
                    // .Select(word => word.ToUpper())
                    .Shuffle();

                _clusterService.SetClusters(capitalizedClusters);
            }
        }

        public void Exit() { }
    }
}