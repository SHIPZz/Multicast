using UnityEngine;

namespace CodeBase.Common.Services.InternetConnection
{
    public class InternetConnectionService : IInternetConnectionService
    {
        public bool CheckConnection()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                return true;
                
            Debug.LogError("Нет интернета!");

            return false;
        }
    }
}