using UnityEngine;

namespace CodeBase.UI.Sound.Configs
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Path/SoundConfig", order = 1)]
    public class SoundConfig : ScriptableObject
    {
        public SoundTypeId Id;
        public AudioClip Clip;
    }
}