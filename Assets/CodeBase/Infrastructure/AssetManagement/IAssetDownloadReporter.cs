using System;
using UniRx;

namespace CodeBase.Infrastructure.AssetManagement
{
  public interface IAssetDownloadReporter : IProgress<float>
  {
    float Progress { get; }
    IObservable<Unit> ProgressUpdated { get; }
    void Report(float value);
    void Reset();
  }
}