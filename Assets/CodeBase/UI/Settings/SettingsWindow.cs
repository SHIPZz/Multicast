using System;
using CodeBase.UI.AbstractWindow;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Settings
{
    public class SettingsWindow : AbstractWindowBase
    {
        [SerializeField] private Toggle _soundToggle;
        [SerializeField] private Button _backButton;

        public IObservable<bool> OnSoundToggled => _soundToggle.OnValueChangedAsObservable();
        public IObservable<Unit> OnBackClicked => _backButton.OnClickAsObservable();

        public void SetSoundEnabled(bool enabled)
        {
            _soundToggle.isOn = enabled;
        }
    }
} 