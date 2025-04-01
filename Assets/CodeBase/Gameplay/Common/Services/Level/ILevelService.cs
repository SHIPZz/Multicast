using System;
using System.Threading;
using CodeBase.Data;
using Cysharp.Threading.Tasks;
using UniRx;

namespace CodeBase.Gameplay.Common.Services.Level
{
    public interface ILevelService
    {
        IObservable<LevelData> OnLevelLoaded { get; }
        IObservable<Unit> OnLevelCompleted { get; }
        UniTask LoadLevelAsync(int level,CancellationToken token = default);
        void ValidateLevel();
        LevelData GetCurrentLevel();
        void UpdateLevel();
    }
}