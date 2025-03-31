using CodeBase.Data;

namespace CodeBase.Common.Services.SaveLoad
{
    public interface ISaveLoadSystem
    {
        void Save(ProgressData data);

        ProgressData Load();
    }
}