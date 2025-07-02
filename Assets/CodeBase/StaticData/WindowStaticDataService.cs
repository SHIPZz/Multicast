using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Game;
using CodeBase.UI.Hint;
using CodeBase.UI.LoadingCurtains;
using CodeBase.UI.Menu;
using CodeBase.UI.NoInternet;
using CodeBase.UI.Settings;
using CodeBase.UI.Victory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.StaticData
{
    public class WindowStaticDataService : IWindowStaticDataService
    {
        private readonly Dictionary<Type, AbstractWindowBase> _windows = new();
        private readonly IAssetProvider _assetProvider;

        private static readonly Type[] WindowTypes = 
        {
            typeof(GameWindow),
            typeof(MenuWindow),
            typeof(SettingsWindow),
            typeof(VictoryWindow),
            typeof(LoadingCurtainWindow),
            typeof(HintWindow),
            typeof(NoInternetWindow)
        };

        public WindowStaticDataService(IAssetProvider assetProvider) =>
            _assetProvider = assetProvider;

        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            foreach (Type windowType in WindowTypes)
            {
                if (_windows.ContainsKey(windowType))
                    continue;

                try
                {
                    var window = await _assetProvider.LoadGameObjectAssetAsync<AbstractWindowBase>(windowType.Name, cancellationToken);
                    _windows[windowType] = window;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to load window {windowType.Name}: {e.Message}");
                }
            }
        }

        public T GetWindow<T>() where T : AbstractWindowBase
        {
            if (_windows.TryGetValue(typeof(T), out var window))
                return (T)window;

            throw new Exception($"Window of type {typeof(T).Name} not found");
        }

        public async UniTask<T> LoadWindowAsync<T>(CancellationToken cancellationToken) where T : AbstractWindowBase
        {
            try
            {
                var window = await _assetProvider.LoadGameObjectAssetAsync<AbstractWindowBase>(typeof(T).Name, cancellationToken);
                _windows[typeof(T)] = window;

                return (T)window;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load window {typeof(T).Name}: {e.Message}");
            }
            
            throw new Exception($"Window of type {typeof(T).Name} not found");
        }
    }
}