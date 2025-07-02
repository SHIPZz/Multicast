using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.AssetManagement
{
  public interface IAssetDownloadService
  {
    UniTask InitializeDownloadDataAsync(CancellationToken cancellationToken = default);
    float GetDownloadSizeMb();
    UniTask UpdateContentAsync(CancellationToken cancellationToken = default);
  }
}