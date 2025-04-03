using CodeBase.Common.Services.Sound;
using CodeBase.Gameplay.Sound;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Cluster;
using CodeBase.UI.Game;
using CodeBase.UI.WordSlots;

namespace CodeBase.StaticData
{
    public interface IUIStaticDataService
    {
        T GetWindow<T>() where T : AbstractWindowBase;
        WordSlot GetWordSlotPrefab();
        ClusterItem GetClusterItem();
        WordSlotHolder GetWordSlotHolderPrefab();
        ClusterItemHolder GetClusterItemHolder();
        SoundConfig GetSoundConfig(SoundTypeId id);
    }
}