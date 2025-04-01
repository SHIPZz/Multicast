using CodeBase.Common.Services.SaveLoad;
using CodeBase.Data;
using UnityEngine;
using Zenject;

namespace CodeBase.Common.Services.Sound
{
    public class SoundService : ISoundService, IInitializable
    {
        private readonly ISaveLoadSystem _saveLoadSystem;
        private bool _isSoundEnabled;

        public bool IsSoundEnabled => _isSoundEnabled;

        public SoundService(ISaveLoadSystem saveLoadSystem)
        {
            _saveLoadSystem = saveLoadSystem;
            LoadSettings();
        }

        public void Initialize()
        {
            LoadSettings();
        }

        public void ToggleSound()
        {
            SetSoundEnabled(!_isSoundEnabled);
        }

        public void SetSoundEnabled(bool enabled)
        {
            _isSoundEnabled = enabled;
            SaveSettings();
            ApplySoundState();
        }

        private void LoadSettings()
        {
            ProgressData progressData = _saveLoadSystem.Load();
            _isSoundEnabled = progressData.SettingsData.IsSoundEnabled;
            ApplySoundState();
        }

        private void SaveSettings()
        {
            ProgressData progressData = _saveLoadSystem.Load();
            progressData.SettingsData.IsSoundEnabled = _isSoundEnabled;
            _saveLoadSystem.Save(progressData);
        }

        private void ApplySoundState()
        {
            AudioListener.volume = _isSoundEnabled ? 1f : 0f;
        }
    }
}