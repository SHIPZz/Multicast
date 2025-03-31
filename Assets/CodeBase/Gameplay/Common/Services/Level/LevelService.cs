using System;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Common.Services.Cluster;
using CodeBase.Gameplay.SO.Level;
using CodeBase.Infrastructure.States.States;
using UniRx;
using UnityEngine;

namespace CodeBase.Gameplay.Common.Services.Level
{
    public class LevelService : ILevelService
    {
        private const int MaxLevelCount = 4;
        
        private readonly IClusterService _clusterService;
        private readonly Subject<LevelData> _onLevelLoaded = new();
        private readonly Subject<Unit> _onLevelCompleted = new();
        private readonly LevelDataSO _levelDataSo;

        public IObservable<LevelData> OnLevelLoaded => _onLevelLoaded;
        public IObservable<Unit> OnLevelCompleted => _onLevelCompleted;

        private LevelData _currentLevel;
        private int _currentLevelIndex;

        public LevelService(IClusterService clusterService, LevelDataSO levelDataSO)
        {
            _levelDataSo = levelDataSO;
            _clusterService = clusterService;
        }

        public void LoadLevel(int level)
        {
            _currentLevel = _levelDataSo.GetLevelData(level);
            
            _clusterService.Init(_currentLevel.Clusters.Shuffle(), _currentLevel.Words.Shuffle());
            
            _onLevelLoaded.OnNext(_currentLevel);
        }

        public void UpdateLevel()
        {
            _currentLevelIndex += 1;
            
            if(_currentLevelIndex >= MaxLevelCount)
                _currentLevelIndex = 0;
            
            LoadLevel(_currentLevelIndex);
        }

        public void ValidateLevel()
        {
            if (_currentLevel == null)
                return;

            bool isValid = _clusterService.ValidateClusters(_currentLevel.Words);
            
            if (isValid)
            {
                _onLevelCompleted.OnNext(Unit.Default);
            }
        }

        public LevelData GetCurrentLevel() => _currentLevel;
    }
}