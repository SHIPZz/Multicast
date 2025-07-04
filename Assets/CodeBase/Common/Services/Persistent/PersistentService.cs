﻿using System.Collections.Generic;
using CodeBase.Common.Services.SaveLoad;
using CodeBase.Data;
using Cysharp.Threading.Tasks;

namespace CodeBase.Common.Services.Persistent
{
    public class PersistentService : IPersistentService
    {
        private readonly List<IProgressWatcher> _progressWatchers = new();

        private readonly ISaveLoadSystem _saveLoadSystem;
        
        private ProgressData _currentProgress;

        public PersistentService(ISaveLoadSystem saveLoadSystem)
        {
            _saveLoadSystem = saveLoadSystem;
        }

        public void RegisterProgressWatcher(IProgressWatcher progressWatcher)
        {
            if (!_progressWatchers.Contains(progressWatcher))
            {
                _progressWatchers.Add(progressWatcher);
            }
        }

        public void LoadAll()
        {
            LoadAsync().Forget();

            foreach (IProgressWatcher progressWatcher in _progressWatchers)
            {
                progressWatcher.Load(_currentProgress);
            }
        }

        public void Save()
        {
            foreach (IProgressWatcher progressWatcher in _progressWatchers)
            {
                progressWatcher.Save(_currentProgress);
            }

            _saveLoadSystem.Save(_currentProgress);
        }

        public void UnregisterProgressWatcher(IProgressWatcher progressWatcher)
        {
            if (_progressWatchers.Contains(progressWatcher))
            {
                _progressWatchers.Remove(progressWatcher);
            }
        }

        private async UniTaskVoid LoadAsync()
        {
            _currentProgress = await _saveLoadSystem.LoadAsync();
        }
    }
}