using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CodeBase.Infrastructure.AssetManagement
{
  public class LabeledAssetDownloadService : IAssetDownloadService
  {
    public const string RemoteLabel = "remote";
    
    private readonly IAssetDownloadReporter _downloadReporter;
    private long _downloadSize;

    public LabeledAssetDownloadService(IAssetDownloadReporter downloadReporter)
    {
      _downloadReporter = downloadReporter;
    }
    
    public async UniTask InitializeDownloadDataAsync(CancellationToken token = default)
    {
      await Addressables.InitializeAsync().ToUniTask(cancellationToken: token);
      await UpdateCatalogsAsync();
      await UpdateDownloadSizeAsync();
    }
    
    public float GetDownloadSizeMb() => 
      SizeToMb(_downloadSize);

    public async UniTask UpdateContentAsync(CancellationToken cancellationToken = default)
    {
      try
      {
        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(RemoteLabel);
      
        while (!downloadHandle.IsDone && downloadHandle.IsValid())
        {
          await UniTask.Delay(100, cancellationToken: cancellationToken);
          _downloadReporter.Report(downloadHandle.GetDownloadStatus().Percent);
        }
      
        _downloadReporter.Report(1);
        
        if (downloadHandle.Status == AsyncOperationStatus.Failed) 
          Debug.LogError("Error while downloading catalog dependencies");

        if(downloadHandle.IsValid())
          Addressables.Release(downloadHandle);
      
        _downloadReporter.Reset();
      }
      catch (Exception e)
      {
        Debug.LogError(e);
      }
    }

    private async UniTask UpdateCatalogsAsync(CancellationToken cancellationToken = default)
    {
      List<string> catalogsToUpdate = await Addressables.CheckForCatalogUpdates().ToUniTask(cancellationToken: cancellationToken);

      if (catalogsToUpdate.IsNullOrEmpty())
        return;

      await Addressables.UpdateCatalogs(catalogsToUpdate).ToUniTask(cancellationToken: cancellationToken);
    }

    private async UniTask UpdateDownloadSizeAsync()
    {
      _downloadSize = await Addressables
        .GetDownloadSizeAsync(RemoteLabel)
        .ToUniTask();
    }

    private static float SizeToMb(long downloadSize) => downloadSize * 1f / 1048576;
  }
}