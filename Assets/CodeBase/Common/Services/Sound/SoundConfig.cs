using CodeBase.Gameplay.Sound;
using UnityEngine;

namespace CodeBase.Common.Services.Sound
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Path/SoundConfig", order = 1)]
    public class SoundConfig : ScriptableObject
    {
        public SoundTypeId Id;
        public AudioClip Clip;
    }
}