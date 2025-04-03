using CodeBase.Data;

namespace CodeBase.Common.Services.Persistent
{
    public interface IProgressWatcher
    {
        void Save(ProgressData progressData);
        void Load(ProgressData progressData);
    }
}