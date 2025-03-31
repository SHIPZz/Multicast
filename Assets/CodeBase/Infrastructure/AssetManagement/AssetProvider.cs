using UnityEngine;

namespace CodeBase.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public GameObject LoadAsset(string path)
        {
            return Resources.Load<GameObject>(path);
        }
        
        public T[] LoadAllAssets<T>(string path) where T : Component
        {
            return Resources.LoadAll<T>(path);
        }

        public T LoadAsset<T>(string path) where T : Component
        {
            return Resources.Load<T>(path);
        }
    }
}