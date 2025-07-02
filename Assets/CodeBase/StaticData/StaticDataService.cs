using System.Threading;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.StaticData;
using CodeBase.UI.AbstractWindow;
using CodeBase.UI.Cluster;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Configs;
using CodeBase.UI.WordCells;
using Cysharp.Threading.Tasks;

public class StaticDataService : IStaticDataService
{
    private readonly IWindowStaticDataService _windows;
    private readonly ISoundStaticDataService _sounds;
    private readonly IPrefabStaticDataService _prefabs;

    public StaticDataService(IAssetProvider assetProvider)
    {
        _windows = new WindowStaticDataService(assetProvider);
        _sounds = new SoundStaticDataService(assetProvider);
        _prefabs = new PrefabStaticDataService(assetProvider);
    }

    public async UniTask LoadAllAsync(CancellationToken cancellationToken = default)
    {
        await UniTask.WhenAll(_windows.LoadAsync(cancellationToken),
                _sounds.LoadAsync(cancellationToken),
                _prefabs.LoadAsync(cancellationToken));
    }

    public async UniTask<T> LoadWindowAsync<T>(CancellationToken cancellationToken = default) where T : AbstractWindowBase
    {
        return await _windows.LoadWindowAsync<T>(cancellationToken);
    }
    
    public T GetWindow<T>() where T : AbstractWindowBase => _windows.GetWindow<T>();
    public SoundConfig GetSoundConfig(SoundTypeId id) => _sounds.GetSoundConfig(id);
    public SoundPlayerView GetSoundPlayerViewPrefab() => _prefabs.GetSoundPlayer();
    
    public ClusterAttachItem GetClusterAttachItem() => _prefabs.GetClusterAttachItem();

    public ClusterItemHolder GetClusterItemHolder() => _prefabs.GetClusterItemHolder();

    public WordCellsHolder GetWordSlotHolderPrefab() => _prefabs.GetWordCellsHolder();

    public ClusterItem GetClusterItem() => _prefabs.GetClusterItem();

    public WordCellView GetWordSlotPrefab() => _prefabs.GetWordCellView();
}