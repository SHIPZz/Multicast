﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Common.Services.Sound;
using CodeBase.Constants;
using CodeBase.Gameplay.Sound;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Cluster;
using CodeBase.UI.Game;
using CodeBase.UI.WordSlots;
using UnityEngine;

namespace CodeBase.StaticData
{
    public class UIStaticDataService : IUIStaticDataService
    {
        private readonly Dictionary<Type, AbstractWindowBase> _windows;
        private readonly IAssetProvider _assetProvider;
        private readonly Dictionary<SoundTypeId, SoundConfig> _soundConfigs;

        public UIStaticDataService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            _windows = Resources.LoadAll<AbstractWindowBase>(AssetPath.Windows)
                .ToDictionary(x => x.GetType(), x => x);
            
            _soundConfigs = Resources.LoadAll<SoundConfig>(AssetPath.SoundConfigs)
                .ToDictionary(x => x.Id, x => x);
        }
        
        public SoundConfig GetSoundConfig(SoundTypeId id) => _soundConfigs.GetValueOrDefault(id);

        public T GetWindow<T>() where T : AbstractWindowBase
        {
            if (!_windows.TryGetValue(typeof(T), out AbstractWindowBase windowPrefab))
                throw new Exception();

            return (T)windowPrefab;
        }

        public WordSlot GetWordSlotPrefab()
        {
            return _assetProvider.LoadAsset<WordSlot>(AssetPath.WordSlotPrefab);
        }
        
        public WordSlotHolder GetWordSlotHolderPrefab()
        {
            return _assetProvider.LoadAsset<WordSlotHolder>(AssetPath.WordSlotHolderPrefab);
        }

        public ClusterItemHolder GetClusterItemHolder()
        {
            return _assetProvider.LoadAsset<ClusterItemHolder>(AssetPath.ClusterItemHolder);
        }

        public ClusterItem GetClusterItem()
        {
            return _assetProvider.LoadAsset<ClusterItem>(AssetPath.ClusterItem);
        }
    }
}