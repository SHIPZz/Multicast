using System.Threading;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Cluster;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Configs;
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
        UniTask LoadWindowAsync<T>(CancellationToken cancellationToken = default) where T : AbstractWindowBase;
    }
}