using System.Collections.Generic;
using CodeBase.Data;
using UnityEngine;

namespace CodeBase.Gameplay.Common.Services.Level.Configs
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Gameplay/Data/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levels = new();

        public LevelData GetLevelData(int id) => _levels[Mathf.Clamp(id, 0, _levels.Count - 1)];

        public IReadOnlyList<LevelData> Levels => _levels;
    }
}