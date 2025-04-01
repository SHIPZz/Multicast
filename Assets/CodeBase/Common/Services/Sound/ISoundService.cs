namespace CodeBase.Common.Services.Sound
{
    public interface ISoundService
    {
        bool IsSoundEnabled { get; }
        void SetSoundEnabled(bool enabled);
    }
} 