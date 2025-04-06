using CodeBase.Data;
using Cysharp.Threading.Tasks;

namespace CodeBase.Common.Services.Persistent
{
    public interface IPersistentService
    {
        void RegisterProgressWatcher(IProgressWatcher progressWatcher);
        void Save();
        void UnregisterProgressWatcher(IProgressWatcher progressWatcher);
        UniTaskVoid Load();
        void LoadAll();
        ProgressData CurrentProgress { get; }
    }
}