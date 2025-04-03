using CodeBase.Data;

namespace CodeBase.Common.Services.SaveLoad
{
    public interface ISaveOnApplicationPauseSystem
    {
        void UpdateData(ProgressData newData);
    }
} 