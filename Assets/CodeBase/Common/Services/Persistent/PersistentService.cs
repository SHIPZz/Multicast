using System.Collections.Generic;
using CodeBase.Common.Services.SaveLoad;
using CodeBase.Data;

namespace CodeBase.Common.Services.Persistent
{
    public class PersistentService : IPersistentService
    {
        private readonly List<IProgressWatcher> _progressWatchers = new();

        private readonly ISaveLoadSystem _saveLoadSystem;
        
        private ProgressData _progressData;

        public PersistentService(ISaveLoadSystem saveLoadSystem)
        {
            _saveLoadSystem = saveLoadSystem;
        }

        public void RegisterProgressWatcher(IProgressWatcher progressWatcher)
        {
            if(_progressData == null)
                Load();
            
            progressWatcher.Load(_progressData);

            if (!_progressWatchers.Contains(progressWatcher))
            {
                _progressWatchers.Add(progressWatcher);
            }
        }

        public void Load()
        {
            _progressData = _saveLoadSystem.Load();
        }

        public void LoadAll()
        {
            _progressData = _saveLoadSystem.Load();

            foreach (IProgressWatcher progressWatcher in _progressWatchers)
            {
                progressWatcher.Load(_progressData);
            }
        }
        
        public void Save()
        {
            foreach (IProgressWatcher progressWatcher in _progressWatchers)
            {
                progressWatcher.Save(_progressData);
            }

            _saveLoadSystem.Save(_progressData);
        }

        public void UnregisterProgressWatcher(IProgressWatcher progressWatcher)
        {
            if (_progressWatchers.Contains(progressWatcher))
            {
                _progressWatchers.Remove(progressWatcher);
            }
        }
    }
}