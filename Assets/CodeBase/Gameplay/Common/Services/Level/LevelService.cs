using System;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Common.Services.Cluster;
using CodeBase.Gameplay.SO.Level;
using CodeBase.Infrastructure.States.States;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeBase.Gameplay.Common.Services.Level
{
    public class LevelService : ILevelService
    {
        private readonly IClusterService _clusterService;
        private readonly Subject<LevelData> _onLevelLoaded = new();
        private readonly Subject<Unit> _onLevelCompleted = new();
        private readonly LevelDataSO _levelDataSo;

        public IObservable<LevelData> OnLevelLoaded => _onLevelLoaded;
        public IObservable<Unit> OnLevelCompleted => _onLevelCompleted;

        private LevelData _currentLevel;
        private int _currentLevelIndex = 1;

        public LevelService(IClusterService clusterService, LevelDataSO levelDataSO)
        {
            _levelDataSo = levelDataSO;
            _clusterService = clusterService;
        }

        public async UniTask LoadLevelAsync(int level, CancellationToken token = default)
        {
            if (level > 1)
                await LoadLevelFromAddressablesAsync(level, token);
            else
                _currentLevel = _levelDataSo.GetLevelData(level);

            _clusterService.Init(_currentLevel.Clusters.Shuffle(), _currentLevel.Words.Shuffle());
            
            _onLevelLoaded.OnNext(_currentLevel);
        }

        private async UniTask LoadLevelFromAddressablesAsync(int level,CancellationToken cancellationToken = default)
        {
            try
            {
                string address = _levelDataSo.GetLevelAddress(level);
                    
                if (string.IsNullOrEmpty(address))
                    throw new Exception($"No address found for level {level}");

                TextAsset textAsset = await Addressables.LoadAssetAsync<TextAsset>(address).Task.AsUniTask().AttachExternalCancellation(cancellationToken);
                
                _currentLevel = JsonConvert.DeserializeObject<LevelData>(textAsset.text);
                    
                if (_currentLevel == null)
                    throw new Exception("Failed to deserialize level data");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load level {level} from Addressables: {e}");
                
                _currentLevel = _levelDataSo.GetLevelData(level);
            }
        }

        public void UpdateLevel()
        {
            _currentLevelIndex++;
            
            if(_currentLevelIndex > _levelDataSo.MaxLevelCount)
                _currentLevelIndex = 0;
            
            Debug.Log($"level updated: {_currentLevelIndex}");
            
            LoadLevelAsync(_currentLevelIndex).Forget();
        }

        public void ValidateLevel()
        {
            if (_currentLevel == null)
                return;

            bool isValid = _clusterService.ValidateClusters();
            
            if (isValid) 
                _onLevelCompleted.OnNext(Unit.Default);
        }

        public LevelData GetCurrentLevel() => _currentLevel;
    }
}