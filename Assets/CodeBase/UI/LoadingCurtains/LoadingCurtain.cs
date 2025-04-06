using System;
using CodeBase.UI.Loading;
using DG.Tweening;
using UnityEngine;

namespace CodeBase.UI.LoadingCurtains
{
    public class LoadingCurtain : MonoBehaviour, ILoadingCurtain
    {
        private const float CloseDuration = 1f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private LoadingView _loadingView;

        private Canvas _canvas;

        private void Awake()
        {
            _canvas = _canvasGroup.GetComponent<Canvas>();
            DontDestroyOnLoad(this);
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvas.enabled = true;
        }

        public void Hide(Action callback = null)
        {
            _canvasGroup
                .DOFade(0, CloseDuration)
                .OnComplete(() =>
                {
                    _canvas.enabled = false;
                    _loadingView.Hide();
                    callback?.Invoke();
                }).SetUpdate(true);
        }
    }
}