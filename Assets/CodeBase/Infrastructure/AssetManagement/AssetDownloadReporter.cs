using System;
using UniRx;
using UnityEngine;

namespace CodeBase.Infrastructure.AssetManagement
{
  public class AssetDownloadReporter : IAssetDownloadReporter
  {
    private const float UpdateThreshold = 0.01f;
    
    public float Progress { get; private set; }
    
    private readonly Subject<Unit> _progressUpdated = new Subject<Unit>();

    public IObservable<Unit> ProgressUpdated => _progressUpdated;

    public void Report(float value)
    {
      if (Mathf.Abs(Progress - value) < UpdateThreshold)
        return;
      
      Progress = value;
      _progressUpdated?.OnNext(Unit.Default);
    }

    public void Reset()
    {
      Progress = 0;
    }
  }
}