namespace CodeBase.UI.Sound.Services
{
    public interface ISoundService
    {
        bool IsSoundEnabled { get; }
        void SetSoundEnabled(bool enabled);
        void Play(SoundTypeId soundTypeId);
    }
} 