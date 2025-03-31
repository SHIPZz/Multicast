using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.AssetManagement
{
  public interface IAssetDownloadService
  {
    UniTask InitializeDownloadDataAsync();
    float GetDownloadSizeMb();
    UniTask UpdateContentAsync();
  }
}