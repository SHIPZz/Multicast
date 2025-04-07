using CodeBase.UI.Controllers;
using CodeBase.UI.Menu;
using CodeBase.UI.Services.Window;
using CodeBase.UI.Sound.Services;
using UniRx;

namespace CodeBase.UI.Settings
{
    public class SettingsWindowController : IController<SettingsWindow>
    {
        private readonly IWindowService _windowService;
        private readonly ISoundService _soundService;
        private readonly CompositeDisposable _disposables = new();
        
        private SettingsWindow _window;

        public SettingsWindowController(IWindowService windowService, ISoundService soundService)
        {
            _soundService = soundService;
            _windowService = windowService;
        }

        public void Initialize()
        {
            _window.SetSoundEnabled(_soundService.IsSoundEnabled);
            
            _window.OnSoundToggled
                .Subscribe(OnSoundToggled)
                .AddTo(_disposables);

            _window.OnBackClicked
                .Subscribe(_ => OnBackClicked())
                .AddTo(_disposables);
        }

        public void BindView(SettingsWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        private void OnSoundToggled(bool enabled)
        {
            _soundService.SetSoundEnabled(enabled);
        }

        private void OnBackClicked()
        {
            _windowService.Close<SettingsWindow>();
            _windowService.OpenWindow<MenuWindow>();
        }
    }
} 