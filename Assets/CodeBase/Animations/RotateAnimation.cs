using DG.Tweening;
using UnityEngine;

namespace CodeBase.Animations
{
    public class RotateAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotationAxis = Vector3.forward;
        [SerializeField] private float _rotationDuration = 1f;
        [SerializeField] private bool _rotateInfinitely = true;
        [SerializeField] private Ease _easeType = Ease.Linear;
        [SerializeField] private Transform _target;
        [SerializeField] private float _targetRotationAngle = 360f;

        private Tween _rotateTween;
        private Quaternion _initialRotation;

        private void Awake()
        {
            _initialRotation = _target.rotation;

            _rotateTween = _target
                    .DORotate(_rotationAxis * _targetRotationAngle, _rotationDuration, RotateMode.FastBeyond360)
                    .From(_initialRotation.eulerAngles)
                    .SetEase(_easeType)
                    .SetLoops(_rotateInfinitely ? -1 : 0, LoopType.Incremental)
                    .OnKill(() => _rotateTween = null)
                    .OnComplete(() => _rotateTween = null)
                ;
        }

        public void Do()
        {
            _target.rotation = _initialRotation;
            
            _rotateTween.Restart();
        }

        public void Stop()
        {
            _rotateTween.Pause();

            _target.rotation = _initialRotation;
        }

        private void OnDestroy()
        {
            _rotateTween?.Kill();
        }
    }
}