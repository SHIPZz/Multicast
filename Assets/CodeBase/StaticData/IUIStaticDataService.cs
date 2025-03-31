using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Game;

namespace CodeBase.StaticData
{
    public interface IUIStaticDataService
    {
        T GetWindow<T>() where T : AbstractWindowBase;
        WordSlot GetWordSlotPrefab();
        ClusterItem GetClusterItem();
        WordSlotHolder GetWordSlotHolderPrefab();
        ClusterItemHolder GetClusterItemHolder();
    }
}