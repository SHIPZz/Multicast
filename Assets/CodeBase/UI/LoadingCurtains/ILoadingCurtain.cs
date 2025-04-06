using System;

namespace CodeBase.UI.LoadingCurtains
{
    public interface ILoadingCurtain
    {
        void Show();
        void Hide(Action callback = null);
    }
}