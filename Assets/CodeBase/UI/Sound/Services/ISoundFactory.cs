using UnityEngine;

namespace CodeBase.UI.Sound.Services
{
    public interface ISoundFactory
    {
        SoundPlayerView Create(Transform parent, SoundTypeId soundTypeId);
    }
}