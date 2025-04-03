using System;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using UnityEngine;
using Zenject;

namespace CodeBase.Common.Services.SaveLoad
{
    public class SaveOnApplicationPauseSystem : ISaveOnApplicationPauseSystem, IInitializable, IDisposable
    {
        private ProgressData _currentData;
        private IPersistentService _persistentService;

        public SaveOnApplicationPauseSystem(IPersistentService persistentService)
        {
            _persistentService = persistentService;
        }

        public void Initialize()
        {
            Application.focusChanged += OnApplicationFocusChanged;
        }

        public void Dispose()
        {
            Application.focusChanged -= OnApplicationFocusChanged;
            
            Save();
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            if (!hasFocus)
            {
                Save();
            }
        }

        private void Save()
        {
            _persistentService.Save();
        }

        public void UpdateData(ProgressData newData)
        {
            _currentData = newData;
        }
    }
}