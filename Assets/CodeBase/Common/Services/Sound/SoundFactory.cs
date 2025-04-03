using System;
using CodeBase.Constants;
using CodeBase.Extensions;
using CodeBase.Gameplay.Sound;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.StaticData;
using UnityEngine;
using Zenject;

namespace CodeBase.Common.Services.Sound
{
    public class SoundFactory : ISoundFactory
    {
        private readonly IUIStaticDataService _uiStaticDataService;
        private readonly IInstantiator _instantiator;
        private readonly IAssetProvider _assetProvider;

        public SoundFactory(IUIStaticDataService uiStaticDataService, IAssetProvider assetProvider,
            IInstantiator instantiator)
        {
            _assetProvider = assetProvider;
            _uiStaticDataService = uiStaticDataService;
            _instantiator = instantiator;
        }

        public SoundPlayerView Create(Transform parent, SoundTypeId soundTypeId)
        {
            if(soundTypeId == SoundTypeId.None)
                throw new ArgumentException("SoundTypeId cannot be None");
            
            SoundConfig soundConfig = _uiStaticDataService.GetSoundConfig(soundTypeId);

            SoundPlayerView prefab = _assetProvider.LoadAsset<SoundPlayerView>(AssetPath.SoundPlayerView);

            return _instantiator.InstantiatePrefabForComponent<SoundPlayerView>(prefab, parent)
                    .With(x => x.Id = soundConfig.Id)
                    .With(x => x.SetClip(soundConfig.Clip))
                ;
        }
    }
}