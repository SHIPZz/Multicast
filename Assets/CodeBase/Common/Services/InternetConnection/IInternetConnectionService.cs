using System.Threading;
using Cysharp.Threading.Tasks;

namespace CodeBase.Common.Services.InternetConnection
{
    public interface IInternetConnectionService
    {
        UniTaskVoid LaunchCheckingEveryFixedIntervalAsync(CancellationToken cancellationToken = default);
        bool IsInternetAvailable { get; }
    }
}