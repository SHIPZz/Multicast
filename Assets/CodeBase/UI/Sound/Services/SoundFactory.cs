using CodeBase.Extensions;
using CodeBase.StaticData;
using CodeBase.UI.Sound.Configs;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Sound.Services
{
    public class SoundFactory : ISoundFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IInstantiator _instantiator;

        public SoundFactory(IStaticDataService staticDataService, IInstantiator instantiator)
        {
            _staticDataService = staticDataService;
            _instantiator = instantiator;
        }

        public SoundPlayerView Create(Transform parent, SoundTypeId soundTypeId)
        {
            if (soundTypeId == SoundTypeId.None)
            {
                Debug.LogError("Sound Type is not specified.");
                return null;
            }
            
            SoundConfig soundConfig = _staticDataService.GetSoundConfig(soundTypeId);

            SoundPlayerView prefab =_staticDataService.GetSoundPlayerViewPrefab();

            return _instantiator.InstantiatePrefabForComponent<SoundPlayerView>(prefab, parent)
                    .With(x => x.Id = soundConfig.Id)
                    .With(x => x.SetClip(soundConfig.Clip))
                ;
        }
    }
}