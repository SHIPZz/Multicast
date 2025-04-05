using CodeBase.UI.Controllers;

namespace CodeBase.UI.LoadingWindow
{
    public class LoadingWindowController : IController<LoadingWindow>
    {
        private LoadingWindow _window;

        public void Initialize()
        {
        }

        public void BindView(LoadingWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
        }
    }
} 