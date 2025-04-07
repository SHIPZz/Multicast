using UnityEngine;

namespace CodeBase.UI.Sound
{
    public class SoundPlayerView : MonoBehaviour
    {
        public SoundTypeId Id;

        [SerializeField] private AudioSource _audioSource;

        public void SetClip(AudioClip audioClip) => _audioSource.clip = audioClip;

        public void Play()
        {
            _audioSource.Play();
        }
    }
}