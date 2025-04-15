using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Common.Services.Level;
using CodeBase.Gameplay.Constants;
using CodeBase.Infrastructure.States.StateInfrastructure;
using CodeBase.UI.Cluster.Services;
using CodeBase.UI.Game;
using CodeBase.UI.Hint;
using CodeBase.UI.Services.Window;
using CodeBase.UI.WordSlots.Services;
using UnityEngine;

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
            _hintService = hintService;
            _levelService = levelService;
            _wordSlotService = wordSlotService;
            _clusterService = clusterService;
            _windowService = windowService;
        }

        public void Enter()
        {
            LevelData currentLevelData = _levelService.GetCurrentLevel();

            SetTargetWordsToFind(currentLevelData);

            SetClusters(currentLevelData);

            _hintService.Init();
            
            _windowService.OpenWindow<GameWindow>();
            
            _levelService.MarkLevelLoaded(currentLevelData.LevelId);

            LogWordsToFind();
        }

        private void SetTargetWordsToFind(LevelData currentLevelData)
        {
            if (_wordSlotService.WordsToFind.IsNullOrEmpty()) 
            {
                IEnumerable<string> capitalizedWords = currentLevelData.Words.Shuffle().Take(GameplayConstants.MaxWordCount);

                _wordSlotService.SetTargetWordsToFind(capitalizedWords);
            }
        }

        private void SetClusters(LevelData currentLevelData)
        {
            if (_clusterService.GetAvailableClusters().IsNullOrEmpty())
            {
                IEnumerable<string> capitalizedClusters = currentLevelData.Clusters.Shuffle();

                _clusterService.SetClusters(capitalizedClusters);
            }
        }

        private void LogWordsToFind()
        {
            foreach (var word in _wordSlotService.WordsToFind)
            {
                Debug.Log($"Word to find: {word}");
            }
        }

        public void Exit() { }
    }
}