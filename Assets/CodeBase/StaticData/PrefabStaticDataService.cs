using System;
using System.Threading;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI.Cluster;
using CodeBase.UI.Sound;
using CodeBase.UI.WordCells;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.StaticData
{
    public class PrefabStaticDataService : IPrefabStaticDataService
    {
        private readonly IAssetProvider _assetProvider;

        private WordCellView _wordCellView;
        private WordCellsHolder _wordCellsHolder;
        private ClusterItemHolder _clusterItemHolder;
        private ClusterItem _clusterItem;
        private SoundPlayerView _soundPlayer;
        private ClusterAttachItem _clusterAttachItem;

        public PrefabStaticDataService(IAssetProvider assetProvider) =>
            _assetProvider = assetProvider;

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _wordCellView = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<WordCellView>(cancellationToken);
                _wordCellsHolder = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<WordCellsHolder>(cancellationToken);
                _clusterItemHolder = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<ClusterItemHolder>(cancellationToken);
                _clusterItem = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<ClusterItem>(cancellationToken);
                _soundPlayer = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<SoundPlayerView>(cancellationToken);
                _clusterAttachItem = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<ClusterAttachItem>(cancellationToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load prefabs: {e.Message}");
                throw;
            }
        }

        public WordCellView GetWordCellView() => _wordCellView;
        
        public ClusterItem GetClusterItem() => _clusterItem;
        
        public SoundPlayerView GetSoundPlayer() => _soundPlayer;
        
        public WordCellsHolder GetWordCellsHolder() => _wordCellsHolder;
        
        public ClusterItemHolder GetClusterItemHolder() => _clusterItemHolder;
        
        public ClusterAttachItem GetClusterAttachItem() => _clusterAttachItem;
    }
}