using System;
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
using CodeBase.UI.WordCells.Services;
using UnityEngine;

namespace CodeBase.Infrastructure.States.States
{
    public class GameState : IState
    {
        private readonly IWindowService _windowService;
        private readonly ILevelService _levelService;
        private readonly IHintService _hintService;
        private readonly IClusterService _clusterService;
        private readonly IWordCellChecker _wordCellChecker;

        public GameState(IWindowService windowService,
            IHintService hintService,
            ILevelService levelService,
            IWordCellChecker wordCellChecker,
            IClusterService clusterService)
        {
            _hintService = hintService ?? throw new ArgumentNullException(nameof(hintService));
            _levelService = levelService ?? throw new ArgumentNullException(nameof(levelService));
            _clusterService = clusterService ?? throw new ArgumentNullException(nameof(clusterService));
            _wordCellChecker = wordCellChecker ?? throw new ArgumentNullException(nameof(wordCellChecker));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
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
            if (_wordCellChecker.TargetWordsToFind.IsNullOrEmpty()) 
            {
                IEnumerable<string> capitalizedWords = currentLevelData.Words.Shuffle().Take(GameplayConstants.MaxWordCount);

                _wordCellChecker.Init(capitalizedWords);
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
            foreach (var word in _wordCellChecker.TargetWordsToFind)
            {
                Debug.Log($"Word to find: {word}");
            }
        }

        public void Exit() { }
    }
}