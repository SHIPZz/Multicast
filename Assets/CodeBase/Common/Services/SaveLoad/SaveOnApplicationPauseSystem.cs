using System;
using CodeBase.Common.Services.Persistent;
using UnityEngine;

namespace CodeBase.Common.Services.SaveLoad
{
    public class SaveOnApplicationPauseSystem : ISaveOnApplicationPauseSystem, IDisposable
    {
        private readonly IPersistentService _persistentService;

        public SaveOnApplicationPauseSystem(IPersistentService persistentService) => _persistentService = persistentService;

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