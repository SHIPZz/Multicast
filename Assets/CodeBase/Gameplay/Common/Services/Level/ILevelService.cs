using System;
using CodeBase.Data;
using UniRx;

namespace CodeBase.Gameplay.Common.Services.Level
{
    public interface ILevelService
    {
        IObservable<LevelData> OnLevelLoaded { get; }
        IObservable<Unit> OnLevelCompleted { get; }
        void LoadLevel(int level);
        void ValidateLevel();
        LevelData GetCurrentLevel();
        void UpdateLevel();
    }
}