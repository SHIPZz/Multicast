using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace CodeBase.Common.Services.Unity
{
    public interface IRemoteConfigService
    {
        UniTask InitializeAsync(CancellationToken token = default);
        IObservable<Unit> OnNewDataLoaded { get; }
        string[] GetKeys();
        string GetJsonSetting(string key, string defaultValue = null);
        UniTask FetchConfigsAsync<T, T2>(T userAttributes, T2 appAttributes, CancellationToken token = default) where T : struct where T2 : struct;
    }
}