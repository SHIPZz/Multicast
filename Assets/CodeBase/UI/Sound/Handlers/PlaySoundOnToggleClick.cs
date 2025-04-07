using CodeBase.UI.Sound.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Sound.Handlers
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