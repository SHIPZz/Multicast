using CodeBase.Animations;
using UnityEngine;

namespace CodeBase.UI.Loading
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private RotateAnimation _rotateAnimation;

        public void Show()
        {
            _rotateAnimation.Do();
        }

        public void Hide()
        {
            _rotateAnimation.Stop();
        }
    }
}