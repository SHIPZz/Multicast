using CodeBase.UI.Controllers;

namespace CodeBase.UI.NoInternet
{
    public class NoInternetWindowController : IController<NoInternetWindow>
    {
        private NoInternetWindow _window;

        public void BindView(NoInternetWindow window)
        {
            _window = window;
        }

        public void Initialize()
        {
            _window.SetText("CHECK YOUR INTERNET CONNECTION");
        }

        public void Dispose()
        {
            
        }
    }
}