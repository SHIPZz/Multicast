using System;
using CodeBase.Common.Services.Persistent;
using UnityEngine;

namespace CodeBase.Common.Services.SaveLoad
{
    public class SaveOnApplicationFocusChangedSystem : ISaveOnApplicationPauseSystem, IDisposable
    {
        private readonly IPersistentService _persistentService;

        public SaveOnApplicationFocusChangedSystem(IPersistentService persistentService) => _persistentService = persistentService;

        public void Initialize() => Application.focusChanged += OnApplicationFocusChanged;

        public void Dispose() => Application.focusChanged -= OnApplicationFocusChanged;

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            if (!hasFocus)
            {
                Save();
            }
        }

        private void Save() => _persistentService.Save();
    }
}