using CodeBase.Data;
using Cysharp.Threading.Tasks;

namespace CodeBase.Common.Services.SaveLoad
{
    public interface ISaveLoadSystem
    {
        void Save(ProgressData data);

        UniTask<ProgressData> Load();
    }
}