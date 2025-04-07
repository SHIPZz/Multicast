using System;
using System.Collections.Generic;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Sound.Services
{
    public class SoundService : ISoundService, IProgressWatcher, IInitializable, IDisposable
    {
        private readonly IPersistentService _persistentService;
        private readonly ISoundFactory _soundFactory;

        private readonly Dictionary<SoundTypeId, SoundPlayerView> _soundPlayers = new();

        private bool _isSoundEnabled;

        public bool IsSoundEnabled => _isSoundEnabled;

        public SoundService(IPersistentService persistentService, ISoundFactory soundFactory)
        {
            _soundFactory = soundFactory;
            _persistentService = persistentService;
        }

        public void Initialize() => _persistentService.RegisterProgressWatcher(this);

        public void Load(ProgressData progressData)
        {
            _isSoundEnabled = progressData.SettingsData.IsSoundEnabled;

            ApplySoundState();
        }

        public void Play(SoundTypeId soundTypeId)
        {
            SoundPlayerView soundPlayerView = null;

            if (!_soundPlayers.TryGetValue(soundTypeId, out SoundPlayerView soundPlayer))
            {
                soundPlayerView = _soundFactory.Create(null, soundTypeId);
                _soundPlayers[soundTypeId] = soundPlayerView;
            }
            else
            {
                soundPlayerView = soundPlayer;
            }

            soundPlayerView.Play();
        }

        public void Save(ProgressData progressData) => progressData.SettingsData.IsSoundEnabled = _isSoundEnabled;

        public void SetSoundEnabled(bool enabled)
        {
            _isSoundEnabled = enabled;
            ApplySoundState();
        }

        public void Dispose() => _persistentService.UnregisterProgressWatcher(this);

        private void ApplySoundState() => AudioListener.volume = _isSoundEnabled ? 1f : 0f;
    }
}