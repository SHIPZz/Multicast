using CodeBase.Gameplay.Cluster;
using CodeBase.Gameplay.Hint;
using CodeBase.UI.Controllers;
using CodeBase.UI.Services.Window;
using UniRx;

namespace CodeBase.UI.Hint
{
    public class HintWindowController : IController<HintWindow>
    {
        private readonly IClusterService _clusterService;
        private readonly IHintService _hintService;
        private readonly IWindowService _windowService;
        private readonly CompositeDisposable _disposables = new();
        
        private HintWindow _window;

        public HintWindowController(
            IClusterService clusterService,
            IHintService hintService,
            IWindowService windowService)
        {
            _clusterService = clusterService;
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
                .Subscribe(_ => _hintService.ShowHint())
                .AddTo(_disposables);

            _hintService.OnHintShown
                .Subscribe(hint => _window.SetHintText(hint))
                .AddTo(_disposables);

            _hintService.OnHintsCountChanged
                .Subscribe(count => 
                {
                    _window.SetRemainingHints(count);
                    _window.SetButtonsInteractable(count > 0);
                })
                .AddTo(_disposables);
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