﻿using System;
using CodeBase.Data;
using UniRx;

namespace CodeBase.Gameplay.Common.Services.Level
{
    public interface ILevelService
    {
        IObservable<LevelData> OnLevelLoaded { get; }
        IObservable<Unit> OnLevelCompleted { get; }
        void ValidateLevel();
        LevelData GetCurrentLevel();
        void MarkLevelLoaded(int level);
        void Initialize();
        LevelData GetTargetLevelData(int level);
    }
}