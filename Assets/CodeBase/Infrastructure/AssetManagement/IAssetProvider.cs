using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        UniTask<T> LoadAssetAsyncByTypePath<T>(CancellationToken cancellationToken = default);
        UniTask<T> LoadGameObjectAssetAsync<T>(string path, CancellationToken cancellationToken = default);
    }
}