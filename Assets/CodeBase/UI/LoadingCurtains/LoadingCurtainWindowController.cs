using CodeBase.UI.Controllers;

namespace CodeBase.UI.LoadingCurtains
{
    public class LoadingCurtainWindowController : IController<LoadingCurtainWindow>
    {
        private LoadingCurtainWindow _window;

        public void BindView(LoadingCurtainWindow window)
        {
            _window = window;
        }

        public void Initialize()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}