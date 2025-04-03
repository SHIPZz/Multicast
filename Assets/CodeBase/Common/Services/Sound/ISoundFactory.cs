using CodeBase.Gameplay.Sound;
using UnityEngine;

namespace CodeBase.Common.Services.Sound
{
    public interface ISoundFactory
    {
        SoundPlayerView Create(Transform parent, SoundTypeId soundTypeId);
    }
}