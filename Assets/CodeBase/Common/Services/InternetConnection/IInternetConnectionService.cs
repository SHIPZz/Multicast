using Cysharp.Threading.Tasks;

namespace CodeBase.Common.Services.InternetConnection
{
    public interface IInternetConnectionService
    {
        UniTaskVoid LaunchCheckingEveryFixedIntervalAsync();
        bool IsInternetAvailable { get; }
    }
}