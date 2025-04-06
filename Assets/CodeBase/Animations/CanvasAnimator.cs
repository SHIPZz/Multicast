using System;
using DG.Tweening;
using UnityEngine;

namespace CodeBase.Animations
{
    public class CanvasAnimator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private Ease _easeType = Ease.Linear;

        private Tweener _fadeTween;
        private float _initialAlpha;

        private void Awake()
        {
            _initialAlpha = _canvasGroup.alpha;
            
            _fadeTween = _canvasGroup
                .DOFade(0, _fadeDuration)
                .SetEase(_easeType)
                .SetAutoKill(false)
                .Pause()
                .OnComplete(() => _canvas.enabled = false)
                .OnKill(() => _fadeTween = null);
        }

        public void Show()
        {
            _canvasGroup.alpha = _initialAlpha;
            _canvas.enabled = true;
            _fadeTween.Rewind();
        }

        public void Hide(Action callback = null)
        {
            _fadeTween.OnComplete(() =>
            {
                _canvas.enabled = false;
                callback?.Invoke();
            }).Restart();
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
        }
    }
} 