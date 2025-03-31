using System;
using UnityEngine;
using UnityEngine.UI;
using CodeBase.UI.AbstractWindow;
using UniRx;

namespace CodeBase.UI.Menu
{
    public class MenuWindow : AbstractWindowBase
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;

        public IObservable<Unit> OnPlayClicked => _playButton.OnClickAsObservable();
        public IObservable<Unit> OnSettingsClicked => _settingsButton.OnClickAsObservable();
    }
}