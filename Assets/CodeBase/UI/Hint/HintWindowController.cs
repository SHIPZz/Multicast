using CodeBase.UI.Controllers;
using CodeBase.UI.Services.Window;
using UniRx;

namespace CodeBase.UI.Hint
{
    public class HintWindowController : IController<HintWindow>
    {
        private readonly IHintService _hintService;
        private readonly IWindowService _windowService;
        private readonly CompositeDisposable _disposables = new();

        private HintWindow _window;

        public HintWindowController(IHintService hintService, IWindowService windowService)
        {
            _hintService = hintService;
            _windowService = windowService;
        }

        public void Initialize()
        {
            _window.OnCloseClicked
                .Subscribe(_ => _windowService.Close<HintWindow>())
                .AddTo(_disposables);

            _window.SetRemainingHints(_hintService.RemainingHints);

            _window.OnShowWordLengthClicked
                .Subscribe(_ => OnShowClicked())
                .AddTo(_disposables);

            _hintService.OnHintShown
                .Subscribe(hint => _window.SetHintText(hint))
                .AddTo(_disposables);

            _hintService.OnHintsCountChanged
                .Subscribe(OnHintCountChanged)
                .AddTo(_disposables);
        }

        private void OnHintCountChanged(int count)
        {
            _window.SetRemainingHints(count);

            if (count <= 0)
                _window.SetButtonsInteractable(false);
        }

        private void OnShowClicked()
        {
            _window.SetButtonsInteractable(false);
            _hintService.ShowHint();
        }

        public void BindView(HintWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}