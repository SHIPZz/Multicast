namespace CodeBase.Common.Services.Persistent
{
    public interface IPersistentService
    {
        void RegisterProgressWatcher(IProgressWatcher progressWatcher);
        void Save();
        void UnregisterProgressWatcher(IProgressWatcher progressWatcher);
        void Load();
        void LoadAll();
    }
}