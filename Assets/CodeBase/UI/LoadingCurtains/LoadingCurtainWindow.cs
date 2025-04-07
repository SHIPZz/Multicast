using CodeBase.Animations;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Loading;
using UnityEngine;

namespace CodeBase.UI.LoadingCurtains
{
    public class LoadingCurtainWindow : AbstractWindowBase
    {
        [SerializeField] private CanvasAnimator _canvasAnimator;
        [SerializeField] private LoadingView _loadingView;

        public override void OnOpen()
        {
            base.OnOpen();
            
            _canvasAnimator.Show();
            _loadingView.Show();
        }

        public override void OnClose()
        {
            base.OnClose();
            
            _loadingView.Hide();
            _canvasAnimator.Hide();
        }
    }
}