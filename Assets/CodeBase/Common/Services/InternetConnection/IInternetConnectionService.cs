using Cysharp.Threading.Tasks;

namespace CodeBase.Common.Services.InternetConnection
{
    public interface IInternetConnectionService
    {
        bool CheckConnection();
        UniTaskVoid LaunchCheckingEveryFixedIntervalAsync();
        bool IsInternetAvailable { get; }
    }
}