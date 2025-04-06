using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Services.RemoteConfig;

namespace CodeBase.Common.Services.Unity
{
    public interface IRemoteConfigService
    {
        void FetchConfigs<T, T2>(T userAttributes, T2 appAttributes) where T : struct where T2 : struct;
        void FetchConfigs<T, T2>(string configType, T userAttributes, T2 appAttributes) where T : struct where T2 : struct;
        void FetchConfigs<T, T2, T3>(T userAttributes, T2 appAttributes, T3 filterAttributes) where T : struct where T2 : struct where T3 : struct;
        UniTask InitializeAsync(CancellationToken token = default);
        IObservable<ConfigResponse> OnConfigsFetched { get; }
        IObservable<Unit> OnNewDataLoaded { get; }
        RemoteConfigService RemoteConfigService { get; }
        string[] GetKeys();
        string GetJsonSetting(string key, string defaultValue = null);
        UniTask FetchConfigsAsync<T, T2>(T userAttributes, T2 appAttributes, CancellationToken token = default) where T : struct where T2 : struct;
    }
}