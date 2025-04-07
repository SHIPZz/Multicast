using CodeBase.UI.Sound.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CodeBase.UI.Sound.Handlers
{
    public class PlaySoundOnButtonClick : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private SoundTypeId _soundTypeId;
        
        private ISoundService _soundService;

        [Inject]
        private void Construct(ISoundService soundService)
        {
            _soundService = soundService;
        }
        
        private void Start()
        {
            _button.onClick
                .AsObservable()
                .Subscribe(_ => _soundService.Play(_soundTypeId))
                .AddTo(this);
        }
    }
}