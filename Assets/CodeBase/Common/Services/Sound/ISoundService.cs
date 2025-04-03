using CodeBase.Gameplay.Sound;

namespace CodeBase.Common.Services.Sound
{
    public interface ISoundService
    {
        bool IsSoundEnabled { get; }
        void SetSoundEnabled(bool enabled);
        void Play(SoundTypeId soundTypeId);
    }
} 