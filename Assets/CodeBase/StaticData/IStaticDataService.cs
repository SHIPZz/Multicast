using System.Threading;
using CodeBase.Common.Services.Sound;
using CodeBase.Gameplay.Sound;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;
using Cysharp.Threading.Tasks;

namespace CodeBase.StaticData
{
    public interface IStaticDataService
    {
        UniTask LoadAllAsync(CancellationToken cancellationToken = default);
        SoundConfig GetSoundConfig(SoundTypeId id);
        T GetWindow<T>() where T : AbstractWindowBase;
        WordSlot GetWordSlotPrefab();
        WordSlotHolder GetWordSlotHolderPrefab();
        ClusterItemHolder GetClusterItemHolder();
        ClusterItem GetClusterItem();
        SoundPlayerView GetSoundPlayerViewPrefab();
    }
}