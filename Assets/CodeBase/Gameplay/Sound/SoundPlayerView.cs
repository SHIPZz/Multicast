using UnityEngine;

namespace CodeBase.Gameplay.Sound
{
    public class SoundPlayerView : MonoBehaviour
    {
        [field: SerializeField] public SoundTypeId Id { get; set; }

        [SerializeField] private AudioSource _audioSource;

        public void SetClip(AudioClip audioClip) => _audioSource.clip = audioClip;

        public void Play()
        {
            _audioSource.Play();
        }
    }
}