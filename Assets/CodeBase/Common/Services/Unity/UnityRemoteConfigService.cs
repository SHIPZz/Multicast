using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Zenject;

namespace CodeBase.Common.Services.Unity
{
    public class UnityRemoteConfigService : IUnityRemoteConfigService, IInitializable, IDisposable
    {
        private readonly Subject<ConfigResponse> _configsFetched = new();
        private readonly Subject<Unit> _newDataLoaded = new();
        
        private readonly RemoteConfigService _remoteConfigService;
        
        public IObservable<ConfigResponse> OnConfigsFetched => _configsFetched;
        public IObservable<Unit> OnNewDataLoaded => _newDataLoaded;

        public RemoteConfigService RemoteConfigService => _remoteConfigService;

        public UnityRemoteConfigService(RemoteConfigService remoteConfigService)
        {
            _remoteConfigService = remoteConfigService;
        }

        public void Initialize()
        {
            _remoteConfigService.FetchCompleted += MarkFetchCompleted;
        }

        public void Dispose()
        {
            _remoteConfigService.FetchCompleted -= MarkFetchCompleted;
            _configsFetched?.Dispose();
        }

        public void FetchConfigs<T, T2>(T userAttributes, T2 appAttributes) where T : struct where T2 : struct
        {
            _remoteConfigService.FetchConfigs(userAttributes, appAttributes);
        }
        
        public async UniTask FetchConfigsAsync<T,T2>(T userAttributes, T2 appAttributes, CancellationToken token = default) where T : struct where T2 : struct
        {
            await _remoteConfigService.FetchConfigsAsync(userAttributes, appAttributes).AsUniTask().AttachExternalCancellation(token);
        }

        public void FetchConfigs<T, T2>(string configType, T userAttributes, T2 appAttributes) where T : struct where T2 : struct
        {
            _remoteConfigService.FetchConfigs(configType, userAttributes, appAttributes);
        }

        public void FetchConfigs<T, T2, T3>(T userAttributes, T2 appAttributes, T3 filterAttributes) where T : struct where T2 : struct where T3 : struct
        {
            _remoteConfigService.FetchConfigs(userAttributes, appAttributes, filterAttributes);
        }

        public string[] GetKeys() => _remoteConfigService.appConfig.GetKeys();

        public string GetJsonSetting(string key, string defaultValue = null)
        {
            return _remoteConfigService.appConfig.GetJson(key, defaultValue);
        }

        public async UniTask InitializeAsync(CancellationToken token = default)
        {
            await UnityServices.InitializeAsync().AsUniTask().AttachExternalCancellation(token);
            
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask().AttachExternalCancellation(token);
            }
        }

        private void MarkFetchCompleted(ConfigResponse configResponse)
        {
            _configsFetched?.OnNext(configResponse);

            switch (configResponse.requestOrigin)
            {
                case ConfigOrigin.Default:
                    Debug.LogWarning("Config loaded from default values.");
                    break;
                
                case ConfigOrigin.Cached:
                    Debug.Log("Config loaded from cache.");
                    break;
                
                case ConfigOrigin.Remote:
                    Debug.Log("Config loaded from remote.");
                    _newDataLoaded?.OnNext(Unit.Default);
                    break;
                
                default:
                    Debug.LogError("Unknown config origin.");
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}