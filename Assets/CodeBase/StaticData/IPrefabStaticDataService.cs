using System.Threading;
using CodeBase.UI.Cluster;
using CodeBase.UI.Sound;
using CodeBase.UI.WordCells;
using Cysharp.Threading.Tasks;

namespace CodeBase.StaticData
{
    public interface IPrefabStaticDataService
    {
        UniTask LoadAsync(CancellationToken cancellationToken = default);
        WordCellView GetWordCellView();
        ClusterItem GetClusterItem();
        SoundPlayerView GetSoundPlayer();
        WordCellsHolder GetWordCellsHolder();
        ClusterItemHolder GetClusterItemHolder();
        ClusterAttachItem GetClusterAttachItem();
    }
}