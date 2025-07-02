using System.Threading;
using System.Threading.Tasks;
using CodeBase.UI.AbstractWindow;
using Cysharp.Threading.Tasks;

namespace CodeBase.StaticData
{
    public interface IWindowStaticDataService
    {
        UniTask LoadAsync(CancellationToken cancellationToken = default);
        T GetWindow<T>() where T : AbstractWindowBase;
        UniTask<T> LoadWindowAsync<T>(CancellationToken cancellationToken) where T : AbstractWindowBase;
    }
}