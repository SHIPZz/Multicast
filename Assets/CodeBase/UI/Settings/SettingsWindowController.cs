using CodeBase.UI.Controllers;
using CodeBase.UI.Menu;
using CodeBase.UI.Services.Window;
using UniRx;
using UnityEngine;

namespace CodeBase.UI.Settings
{
    public class SettingsWindowController : IController<SettingsWindow>
    {
        private readonly IWindowService _windowService;
        private SettingsWindow _window;
        private readonly CompositeDisposable _disposables = new();

        public SettingsWindowController(IWindowService windowService)
        {
            _windowService = windowService;
        }

        public void Initialize()
        {
            _window.OnSoundToggled
                .Subscribe(OnSoundToggled)
                .AddTo(_disposables);

            _window.OnBackClicked
                .Subscribe(_ => OnBackClicked())
                .AddTo(_disposables);

            _window.SetSoundEnabled(true);
        }

        public void BindView(SettingsWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnSoundToggled(bool enabled)
        {
            Debug.Log($"Sound {(enabled ? "enabled" : "disabled")}");
        }

        private void OnBackClicked()
        {
            _windowService.Close<SettingsWindow>();
            _windowService.OpenWindow<MenuWindow>();
        }
    }
} 