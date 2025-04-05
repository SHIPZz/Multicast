using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Loading;
using UnityEngine;

namespace CodeBase.UI.LoadingWindow
{
    public class LoadingWindow : AbstractWindowBase
    {
        [SerializeField] private LoadingView _loadingView;

        public override void OnOpen()
        {
            base.OnOpen();
            
            _loadingView.Show();
        }

        public override void OnClose()
        {
            base.OnClose();
            
            _loadingView.Hide();
        }
    }
}