using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Cluster;
using CodeBase.UI.Game;
using CodeBase.UI.Hint;
using CodeBase.UI.LoadingCurtains;
using CodeBase.UI.Menu;
using CodeBase.UI.NoInternet;
using CodeBase.UI.Settings;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Configs;
using CodeBase.UI.Victory;
using CodeBase.UI.WordSlots;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.StaticData
{
    public class StaticDataService : IStaticDataService, IDisposable
    {
        private readonly Dictionary<Type, AbstractWindowBase> _windowPrefabs = new();
        private readonly IAssetProvider _assetProvider;
        private readonly Dictionary<SoundTypeId, SoundConfig> _soundConfigs = new();

        private WordSlot _wordSlotPrefab;
        private WordSlotHolder _wordSlotHolderPrefab;
        private ClusterItemHolder _clusterItemHolderPrefab;
        private ClusterItem _clusterItemPrefab;
        private SoundPlayerView _soundPlayerPrefab;
        private ClusterAttachItem _clusterAttachItemPrefab;

        public StaticDataService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask LoadAllAsync(CancellationToken cancellationToken = default)
        {
            await InitializeWindowsAsync(cancellationToken);
            await InitializeSoundConfigsAsync(cancellationToken);
            
            try
            {
                _wordSlotPrefab = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<WordSlot>(cancellationToken);
                _wordSlotHolderPrefab = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<WordSlotHolder>(cancellationToken);
                _clusterItemHolderPrefab = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<ClusterItemHolder>(cancellationToken);
                _clusterItemPrefab = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<ClusterItem>(cancellationToken);
                _soundPlayerPrefab = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<SoundPlayerView>(cancellationToken);
                _clusterAttachItemPrefab = await _assetProvider.LoadGameObjectAssetAsyncByTypePath<ClusterAttachItem>(cancellationToken);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load prefabs: {e.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _assetProvider.ReleaseAll();
        }

        private async UniTask InitializeSoundConfigsAsync(CancellationToken cancellationToken = default)
        {
            var soundTypes = Enum.GetValues(typeof(SoundTypeId))
                .Cast<SoundTypeId>()
                .Where(x => x != SoundTypeId.None);

            foreach (SoundTypeId soundType in soundTypes)
            {
                try
                {
                    var soundConfig = await _assetProvider.LoadAssetAsync<SoundConfig>(soundType.ToString(), cancellationToken);
                    
                    _soundConfigs[soundType] = soundConfig;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to load sound config for {soundType}: {e.Message}");
                }
            }
        }

        public SoundPlayerView GetSoundPlayerViewPrefab()
        {
            return _soundPlayerPrefab;
        }

        public SoundConfig GetSoundConfig(SoundTypeId id) => _soundConfigs.GetValueOrDefault(id);

        public T GetWindow<T>() where T : AbstractWindowBase
        {
            if (!_windowPrefabs.TryGetValue(typeof(T), out AbstractWindowBase window))
                throw new Exception($"Window of type {typeof(T).Name} not found");

            return (T)window;
        }

        public WordSlot GetWordSlotPrefab()
        {
            return _wordSlotPrefab;
        }

        public WordSlotHolder GetWordSlotHolderPrefab()
        {
            return _wordSlotHolderPrefab;
        }

        public ClusterItemHolder GetClusterItemHolder()
        {
            return _clusterItemHolderPrefab;
        }

        public ClusterItem GetClusterItem()
        {
            return _clusterItemPrefab;
        }

        public async UniTask LoadWindowAsync<T>(CancellationToken cancellationToken = default) where T : AbstractWindowBase
        {
            try
            {
                AbstractWindowBase windowPrefab = await _assetProvider.LoadGameObjectAssetAsync<AbstractWindowBase>(typeof(T).Name, cancellationToken);
                _windowPrefabs[typeof(T)] = windowPrefab;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load window {typeof(T).Name}: {e.Message}");
            }
        }

        public ClusterAttachItem GetClusterAttachItem()
        {
            return _clusterAttachItemPrefab;
        }

        private async UniTask InitializeWindowsAsync(CancellationToken cancellationToken = default)
        {
            Type[] windowTypes =
            {
                typeof(GameWindow),
                typeof(MenuWindow),
                typeof(SettingsWindow),
                typeof(VictoryWindow),
                typeof(LoadingCurtainWindow),
                typeof(HintWindow),
                typeof(NoInternetWindow)
            };

            foreach (Type windowType in windowTypes)
            {
                try
                {
                    if(_windowPrefabs.ContainsKey(windowType))
                        continue;
                    
                    AbstractWindowBase windowPrefab = await _assetProvider.LoadGameObjectAssetAsync<AbstractWindowBase>(windowType.Name, cancellationToken);
                    _windowPrefabs[windowType] = windowPrefab;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load window {windowType.Name}: {e.Message}");
                }
            }
        }
    }
}