using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CodeBase.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        private readonly Dictionary<string, AsyncOperationHandle> _handles = new();

        public async UniTask<T> LoadGameObjectAssetAsync<T>(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(path);
                _handles[path] = handle;
                
                GameObject prefab = await handle.ToUniTask(cancellationToken: cancellationToken);
                return prefab.GetComponent<T>();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load prefab {typeof(T).Name}: {e.Message}");
                throw;
            }
        }
        
        public async UniTask<T> LoadAssetAsync<T>(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<T>(path);
                _handles[path] = handle;
                
                return await handle.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load prefab:  {path}: {e.Message}");
                throw;
            }
        }

        public async UniTask<T> LoadGameObjectAssetAsyncByTypePath<T>(CancellationToken cancellationToken = default)
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

        public void Release(string path)
        {
            if (_handles.TryGetValue(path, out var handle))
            {
                Addressables.Release(handle);
                _handles.Remove(path);
            }
        }

        public void ReleaseAll()
        {
            foreach (var handle in _handles.Values)
            {
                Addressables.Release(handle);
            }
            
            _handles.Clear();
        }
    }
}