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

        public async UniTaskVoid LaunchCheckingEveryFixedIntervalAsync()
        {
            while (true)
            {
                CheckConnection();

                try
                {
                    await UniTask.WaitForSeconds(CheckInternetConnectionInterval,true,PlayerLoopTiming.Update,_cancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    
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

            Debug.LogError("Нет интернета!");

            IsInternetAvailable = false;
            return false;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Dispose();
        }
    }
}