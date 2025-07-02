using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Configs;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.StaticData
{
    public class SoundStaticDataService : ISoundStaticDataService
    {
        private readonly IAssetProvider _assetProvider;
        private readonly Dictionary<SoundTypeId, SoundConfig> _soundConfigs = new();

        public SoundStaticDataService(IAssetProvider assetProvider) =>
            _assetProvider = assetProvider;

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            var soundTypes = Enum.GetValues(typeof(SoundTypeId))
                .Cast<SoundTypeId>()
                .Where(x => x != SoundTypeId.None);

            foreach (var type in soundTypes)
            {
                try
                {
                    var config = await _assetProvider.LoadAssetAsync<SoundConfig>(type.ToString(), cancellationToken);
                    _soundConfigs[type] = config;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to load sound config for {type}: {e.Message}");
                }
            }
        }

        public SoundConfig GetSoundConfig(SoundTypeId id) =>
            _soundConfigs.GetValueOrDefault(id);
    }
}