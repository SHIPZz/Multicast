using System;
using System.Linq;
using CodeBase.Common.Services.Persistent;
using CodeBase.Common.Services.Unity;
using CodeBase.Data;
using CodeBase.Gameplay.SO.Level;
using CodeBase.Gameplay.WordSlots;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using Zenject;

namespace CodeBase.Gameplay.Common.Services.Level
{
    public class LevelService : ILevelService, 
        IProgressWatcher,
        IInitializable
        , IDisposable
    {
        private readonly Subject<LevelData> _onLevelLoaded = new();
        private readonly Subject<Unit> _onLevelCompleted = new();
        private readonly CompositeDisposable _disposables = new();
        
        private readonly LevelDataSO _levelDataSo;
        private readonly IRemoteConfigService _unityRemoteConfigService;
        private readonly IWordSlotService _wordSlotService;
        private readonly IPersistentService _persistentService;

        public IObservable<LevelData> OnLevelLoaded => _onLevelLoaded;
        public IObservable<Unit> OnLevelCompleted => _onLevelCompleted;

        private LevelData _currentLevel;
        private int _currentLevelIndex = 1;
        private int _totalLevelCount;

        public LevelService(
            LevelDataSO levelDataSO,
            IWordSlotService wordSlotService,
            IPersistentService persistentService,
            IRemoteConfigService unityRemoteConfigService
            )
        {
            _persistentService = persistentService;
            _wordSlotService = wordSlotService;
            _levelDataSo = levelDataSO;
            _unityRemoteConfigService = unityRemoteConfigService;
        }

        public void Initialize()
        {
            _unityRemoteConfigService
                .OnNewDataLoaded
                .Subscribe(_ =>_totalLevelCount = _unityRemoteConfigService.GetKeys().Count(x => x.Contains("level_")) + _levelDataSo.Levels.Count)
                .AddTo(_disposables);
            
            _persistentService.RegisterProgressWatcher(this);
        }

        public void Dispose()
        {
            _onLevelLoaded?.Dispose();
            _onLevelCompleted?.Dispose();
            _persistentService.UnregisterProgressWatcher(this);
            _disposables?.Dispose();
        }

        public void Load(ProgressData progressData) => _currentLevel = GetTargetLevelData(progressData.PlayerData.Level);

        public void Save(ProgressData progressData)
        {
            progressData.PlayerData.Level = _currentLevelIndex;
            _currentLevel = GetTargetLevelData(_currentLevelIndex);
        }

        public void MarkLevelLoaded(int level)
        {
            Debug.Log($"load level - {level}");
            
            _currentLevel = GetTargetLevelData(level);

            _onLevelLoaded.OnNext(_currentLevel);
        }

        public LevelData GetTargetLevelData(int level)
        {
            return level > _levelDataSo.Levels.Count ? GetLevelFromUnityConfig(level) : _levelDataSo.GetLevelData(level);
        }

        private LevelData GetLevelFromUnityConfig(int level)
        {
            string jsonFromConfig = _unityRemoteConfigService.GetJsonSetting($"level_{level}");

            if (string.IsNullOrEmpty(jsonFromConfig))
                throw new Exception($"No json found in unity config for level {level}");

            LevelData targetLevel = JsonConvert.DeserializeObject<LevelData>(jsonFromConfig);

            if (targetLevel == null)
                throw new Exception("Failed to deserialize level data");

            return targetLevel;
        }

        public void ValidateLevel()
        {
            if (_currentLevel == null)
                return;

            bool isValid = _wordSlotService.ValidateFormedWords();

            if (isValid)
            {
                UpdateIndex();
                
                _persistentService.Save();
                
                _onLevelCompleted.OnNext(Unit.Default);
            }
        }

        public LevelData GetCurrentLevel() => _currentLevel;

        private void UpdateIndex()
        {
            _currentLevelIndex++;

            SetFirstLevelOnMaxLevelReached();
        }

        private void SetFirstLevelOnMaxLevelReached()
        {
            if (_currentLevelIndex > _totalLevelCount)
                _currentLevelIndex = 1;
        }
    }
}