using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Common.Services.Persistent;
using CodeBase.Common.Services.Sound;
using CodeBase.Data;
using CodeBase.Gameplay.Sound;
using CodeBase.Gameplay.WordSlots;
using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterService : IClusterService, IProgressWatcher, IDisposable, IInitializable
    {
        private readonly Dictionary<string, List<WordSlot>> _clustersOccupiedSlots = new();
        private readonly IWordSlotService _wordSlotService;
        private readonly Dictionary<int, List<ClusterItem>> _clustersByRow = new();
        private Dictionary<int, Dictionary<int, string>> _clustersByRowAndColumns = new();
        private readonly ISoundService _soundService;
        private readonly List<string> _availableClusters = new();
        private readonly IPersistentService _persistentService;
        private readonly List<string> _placedClusters = new();

        private readonly Dictionary<string, ClusterItem> _createdClusters = new();

        public ClusterService(IWordSlotService wordSlotService,
            IPersistentService persistentService,
            ISoundService soundService)
        {
            _persistentService = persistentService;
            _soundService = soundService;
            _wordSlotService = wordSlotService;
        }

        public void Initialize() => _persistentService.RegisterProgressWatcher(this);

        public void Dispose() => _persistentService.UnregisterProgressWatcher(this);

        public void Init()
        {
            for (int i = 0; i < _wordSlotService.WordsToFind.Count; i++)
                _clustersByRow.Add(i, new List<ClusterItem>());
        }

        public void RegisterCreatedCluster(ClusterItem clusterItem) => _createdClusters[clusterItem.Text] = clusterItem;

        public void SetClusters(IEnumerable<string> clusters) => _availableClusters.AddRange(clusters);

        public IEnumerable<string> GetAvailableClusters() => _availableClusters;

        public IEnumerable<string> GetPlacedClustersFromData() => _placedClusters;

        public bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot)
        {
            int startIndex = _wordSlotService.IndexOf(wordSlot);
            Debug.Log($"Trying to place cluster '{cluster.Text}' at start index {startIndex}");

            if (!IsPlacementAvailable(cluster.Text, startIndex))
            {
                Debug.LogWarning($"Placement not available for cluster '{cluster.Text}' at index {startIndex}");
                return false;
            }

            PlaceCluster(cluster, startIndex);

            _soundService.Play(SoundTypeId.ClusterPlaced);

            return true;
        }

        public void OnClusterSelected(ClusterItem clusterItem) => _soundService.Play(SoundTypeId.TakeCluster);

        public void ResetCluster(ClusterItem cluster)
        {
            if (_clustersOccupiedSlots.TryGetValue(cluster.Text, out List<WordSlot> slots))
            {
                foreach (WordSlot slot in slots)
                {
                    int rowIndex = _wordSlotService.GetRowBySlot(slot);
                    int columnIndex = _wordSlotService.GetColumnBySlot(slot);
                    
                    FindAndClearByRowAndColumn(rowIndex, columnIndex);
                    
                    slot.Clear();
                }

                _placedClusters.Remove(cluster.Text);
                _clustersOccupiedSlots.Remove(cluster.Text);
                _availableClusters.Add(cluster.Text);
            }
        }

        public void CheckAndHideFilledClusters()
        {
            foreach (KeyValuePair<int, string> keyValuePair in _wordSlotService.GetFormedWords())
            {
                string formedWord = keyValuePair.Value;

                int rowIndex = keyValuePair.Key;

                if (SkipIfNullOrEmpty(formedWord, rowIndex))
                    continue;

                if (SkipIfFormedWordInAppropriate(formedWord, rowIndex, keyValuePair)) 
                    continue;

                Debug.Log($"Found word: {formedWord} in row {rowIndex}");
                HideAllClustersOutlineIconInRow(rowIndex);
            }
        }

        public void Cleanup()
        {
            _availableClusters.Clear();
            _clustersByRow.Clear();
            _placedClusters.Clear();
            _clustersByRowAndColumns.Clear();
            _clustersOccupiedSlots.Clear();
        }

        public void RestorePlacedClusters()
        {
            if (_placedClusters.Count == 0)
                return;

            var placedClustersCopy = new List<string>(_placedClusters);

            foreach (string placedCluster in placedClustersCopy)
            {
                if (!_createdClusters.TryGetValue(placedCluster, out ClusterItem clusterItem))
                {
                    Debug.LogWarning($"Could not find created cluster item for text: {placedCluster}");
                    continue;
                }

                RestoreClusterToSavedSlot(placedCluster, clusterItem);
            }

            CheckAndHideFilledClusters();
        }

        public void Save(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;

            CleanupCollections(playerData,playerData.ClustersByRows);

            playerData.AvailableClusters.AddRange(_availableClusters);
            
            playerData.PlacedClusters.AddRange(_placedClusters);

            foreach (KeyValuePair<int, List<ClusterItem>> keyValue in _clustersByRow)
            foreach (ClusterItem clusterItem in keyValue.Value)
            {
                playerData.ClustersByRows[keyValue.Key] = clusterItem.Text;
            }

            playerData.PlacedClustersByRowAndColumns = _clustersByRowAndColumns;
        }

        public void Load(ProgressData progressData)
        {
            _availableClusters.AddRange(progressData.PlayerData.AvailableClusters);

            _placedClusters.AddRange(progressData.PlayerData.PlacedClusters);

            _clustersByRowAndColumns = new Dictionary<int, Dictionary<int, string>>(progressData.PlayerData.PlacedClustersByRowAndColumns);
        }

        private void RestoreClusterToSavedSlot(string placedCluster, ClusterItem clusterItem)
        {
            var clustersByRowAndColumnsCopy = new Dictionary<int, Dictionary<int, string>>(_clustersByRowAndColumns);

            foreach (KeyValuePair<int, Dictionary<int, string>> rowCluster in clustersByRowAndColumnsCopy)
            {
                var columnClustersCopy = new Dictionary<int, string>(rowCluster.Value);
                    
                foreach (KeyValuePair<int, string> columnCluster in columnClustersCopy)
                {
                    if (columnCluster.Value == placedCluster)
                    {
                        WordSlot wordSlot = _wordSlotService.GetWordSlotByRowAndColumn(rowCluster.Key, columnCluster.Key);
                            
                        if (wordSlot != null && !wordSlot.IsOccupied)
                        {
                            clusterItem.PlaceToSlot(wordSlot);
                            clusterItem.MarkPlacedTo(wordSlot);
                        }
                    }
                }
            }
        }

        private bool SkipIfFormedWordInAppropriate(string formedWord, int rowIndex, KeyValuePair<int, string> keyValuePair)
        {
            if (!_wordSlotService.WordsToFind.Contains(formedWord, StringComparer.OrdinalIgnoreCase))
            {
                Debug.Log($"Row {rowIndex} - {keyValuePair.Value} is not found. {_wordSlotService.WordsToFind.Count} - words to find");
                return true;
            }

            return false;
        }

        private void FindAndClearByRowAndColumn(int targetWordSlotRow, int column)
        {
            if (_clustersByRowAndColumns.TryGetValue(targetWordSlotRow, out Dictionary<int, string> rowClusters))
            {
                rowClusters.Remove(column);
                        
                if (rowClusters.Count == 0)
                    _clustersByRowAndColumns.Remove(targetWordSlotRow);
            }
        }

        private bool IsPlacementAvailable(string clusterText, int startIndex)
        {
            if (startIndex == -1 || startIndex + clusterText.Length > _wordSlotService.SlotCount)
                return false;

            int availableSlots = 0;

            for (int i = 0; i < clusterText.Length; i++)
            {
                if (!_wordSlotService.GetTargetSlot(startIndex + i).IsOccupied)
                    availableSlots++;
            }

            return availableSlots >= clusterText.Length;
        }

        private void PlaceCluster(ClusterItem cluster, int startIndex)
        {
            List<WordSlot> occupiedSlots = new List<WordSlot>();

            for (int i = 0; i < cluster.Text.Length; i++)
            {
                int targetIndex = startIndex + i;

                WordSlot slot = _wordSlotService.GetTargetSlot(targetIndex);

                int rowBySlot = _wordSlotService.GetRowBySlot(slot);
                int columnBySlot = _wordSlotService.GetColumnBySlot(slot);

                Debug.Log($"Placing cluster '{cluster.Text}' at row {rowBySlot}, column {columnBySlot}");

                _clustersByRow[rowBySlot].Add(cluster);

                if (_clustersByRowAndColumns.TryGetValue(rowBySlot, out Dictionary<int, string> clusterItemsByColumn))
                    clusterItemsByColumn[columnBySlot] = cluster.Text;
                else
                {
                    Debug.LogWarning($"Row {rowBySlot} not found in _clustersByRowAndColumns, initializing new entry.");
                    _clustersByRowAndColumns[rowBySlot] = new Dictionary<int, string> { { columnBySlot, cluster.Text } };
                }

                slot.SetLetter(cluster.Text[i]);

                occupiedSlots.Add(slot);
            }

            _placedClusters.Add(cluster.Text);
            _clustersOccupiedSlots[cluster.Text] = occupiedSlots;
            _availableClusters.Remove(cluster.Text);
        }

        private void HideAllClustersOutlineIconInRow(int row)
        {
            if (!_clustersByRow.TryGetValue(row, out List<ClusterItem> clustersInRow))
                return;

            foreach (ClusterItem cluster in clustersInRow)
            {
                if (!cluster.IsPlaced)
                    continue;

                cluster.HideOutlineIcon();
                cluster.SetBlocksRaycasts(false);
            }
        }

        private static bool SkipIfNullOrEmpty(string formedWord, int rowIndex)
        {
            if (string.IsNullOrEmpty(formedWord))
            {
                Debug.Log($"Row {rowIndex} is empty");
                return true;
            }

            return false;
        }

        private static void CleanupCollections(PlayerData playerData,Dictionary<int, string> clustersByRows)
        {
            clustersByRows.Clear();
            playerData.PlacedClusters.Clear();
            playerData.AvailableClusters.Clear();
        }
    }
}