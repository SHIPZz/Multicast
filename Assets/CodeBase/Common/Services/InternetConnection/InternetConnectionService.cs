using CodeBase.UI.NoInternet;
using CodeBase.UI.Services.Window;
using UnityEngine;

namespace CodeBase.Common.Services.InternetConnection
{
    public class InternetConnectionService : IInternetConnectionService
    {
        private readonly IWindowService _windowService;
        
        public InternetConnectionService(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public bool CheckConnection()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                return true;
            
            _windowService.OpenWindow<NoInternetWindow>(true);
                
            Debug.LogError("Нет интернета!");

            return false;
        }
    }
}