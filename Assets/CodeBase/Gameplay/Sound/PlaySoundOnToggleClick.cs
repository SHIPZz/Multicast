using CodeBase.Common.Services.Sound;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.Gameplay.Sound
{
    public class PlaySoundOnToggleClick : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private SoundTypeId _soundTypeId;
        
        private ISoundService _soundService;

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }
        
        private void Start()
        {
            _toggle.onValueChanged
                .AsObservable()
                .Subscribe(_ => _soundService.Play(_soundTypeId))
                .AddTo(this);
        }
    }
}