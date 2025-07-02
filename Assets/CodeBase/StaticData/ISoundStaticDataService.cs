using System.Threading;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Configs;
using Cysharp.Threading.Tasks;

namespace CodeBase.StaticData
{
    public interface ISoundStaticDataService
    {
        UniTask LoadAsync(CancellationToken cancellationToken = default);
        SoundConfig GetSoundConfig(SoundTypeId id);
    }
}