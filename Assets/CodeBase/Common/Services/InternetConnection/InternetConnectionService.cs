using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Common.Services.InternetConnection
{
    public class InternetConnectionService : IInternetConnectionService, IDisposable
    {
        private const float CheckInternetConnectionInterval = 5f;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        public bool IsInternetAvailable { get; private set; }

        public async UniTaskVoid LaunchCheckingEveryFixedIntervalAsync(CancellationToken cancellationToken = default)
        {
          CancellationTokenSource linkedToken =  CancellationTokenSource.CreateLinkedTokenSource(cancellationToken,_cancellationTokenSource.Token);
            
            while (true)
            {
                CheckConnection();

                try
                {
                    await UniTask.WaitForSeconds(CheckInternetConnectionInterval, true, PlayerLoopTiming.Update, linkedToken.Token);
                }
                catch (OperationCanceledException)
                {
                    break; 
                }
                catch (Exception e)
                {
                    Debug.LogError($"Internet connection error: {e}");
                }
            }
        }

        public bool CheckConnection()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                IsInternetAvailable = true;
                return true;
            }

            Debug.LogError("No internet!");

            IsInternetAvailable = false;
            return false;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}