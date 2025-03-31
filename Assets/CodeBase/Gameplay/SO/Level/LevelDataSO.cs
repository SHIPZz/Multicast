using System.Collections.Generic;
using CodeBase.Data;
using Unity.VisualScripting;

namespace CodeBase.Gameplay.SO.Level
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "LevelData", menuName = "Gameplay/Data/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levels = new();
        
        public LevelData GetLevelData(int id) => _levels[Mathf.Clamp(id, 0, _levels.Count - 1)];
        
        public IReadOnlyList<LevelData> Levels => _levels;

        [ContextMenu("Add More Words To Level")]
        public void AddMoreWordsToLevel3()
        {
            var level = _levels[0];
            var newWords = new[] { "МОЛОКО", "ПОЛЕНО", "КОРОВА", "ДЕРЕВО" };
            var newClusters = new[] { "МО", "ЛО", "КО", "ПО", "ЛЕ", "НО", "КО", "РО", "ВА", "ДЕ", "РЕ", "ВО" };

            // Создаем новые массивы с увеличенным размером
            var combinedWords = new string[level.Words.Length + newWords.Length];
            var combinedClusters = new string[level.Clusters.Length + newClusters.Length];

            // Копируем существующие данные
            level.Words.CopyTo(combinedWords, 0);
            level.Clusters.CopyTo(combinedClusters, 0);

            // Добавляем новые данные
            newWords.CopyTo(combinedWords, level.Words.Length);
            newClusters.CopyTo(combinedClusters, level.Clusters.Length);

            // Присваиваем новые массивы
            level.Words = combinedWords;
            level.Clusters = combinedClusters;
        }
    }
}