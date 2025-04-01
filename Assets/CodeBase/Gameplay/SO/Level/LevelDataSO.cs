using System.Collections.Generic;
using CodeBase.Data;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace CodeBase.Gameplay.SO.Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Gameplay/Data/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levels = new();
        [SerializeField] private List<LevelAddress> _levelAddresses;
        [SerializeField] private string _addressablesGroupName = "Levels";

        public int MaxLevelCount => _levels.Count + _levelAddresses.Count;

        public LevelData GetLevelData(int id) => _levels[Mathf.Clamp(id, 0, _levels.Count - 1)];

        public IReadOnlyList<LevelData> Levels => _levels;

        public string GetLevelAddress(int levelIndex) => _levelAddresses.Find(x => x.Level == levelIndex).Address;

        [ContextMenu("Refresh Level Addresses")]
        private void RefreshLevelAddresses()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            
            _levelAddresses.Clear();

            AddressableAssetGroup targetGroup = settings.groups.Find(g => g.Name == _addressablesGroupName);

            if (targetGroup == null)
            {
                Debug.LogError($"Group {_addressablesGroupName} not found in Addressables");
                return;
            }

            var sortedEntries = new List<AddressableAssetEntry>(targetGroup.entries);
            sortedEntries.Sort((a, b) => string.Compare(a.address, b.address));

            for (int i = 0; i < sortedEntries.Count; i++)
            {
                _levelAddresses.Add(new LevelAddress
                {
                    Level = i + 2,
                    Address = sortedEntries[i].address
                });
            }
            
            Debug.Log($"Refreshed {_levelAddresses.Count} level addresses from group {_addressablesGroupName}");
        }
    }
}