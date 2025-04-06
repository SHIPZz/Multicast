using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public async UniTask<T> LoadGameObjectAssetAsync<T>(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(path)
                    .ToUniTask(cancellationToken: cancellationToken);

                return prefab.GetComponent<T>();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load prefab {typeof(T).Name}: {e.Message}");
                throw;
            }
        }

        public async UniTask<T> LoadAssetAsyncByTypePath<T>(CancellationToken cancellationToken = default)
        {
            try
            {
                return await LoadGameObjectAssetAsync<T>(typeof(T).Name, cancellationToken);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load prefab {typeof(T).Name}: {e.Message}");
                throw;
            }
        }
    }
}