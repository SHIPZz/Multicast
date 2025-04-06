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
        private readonly IClusterRepository _clusterRepository;
        private readonly IClusterPlacement _clusterPlacement;
        private readonly ISoundService _soundService;
        private readonly IPersistentService _persistentService;
        private readonly IWordSlotService _wordSlotService;

        public ClusterService(
            IWordSlotService wordSlotService,
            IPersistentService persistentService,
            ISoundService soundService)
        {
            _wordSlotService = wordSlotService;
            _persistentService = persistentService;
            _soundService = soundService;

            _clusterRepository = new ClusterRepository();
            _clusterPlacement = new ClusterPlacement(wordSlotService);
        }

        public void Initialize() => _persistentService.RegisterProgressWatcher(this);

        public void Dispose() => _persistentService.UnregisterProgressWatcher(this);

        public void Init()
        {
            _clusterPlacement.Initialize(_wordSlotService.WordsToFind.Count);
        }

        public void RegisterCreatedCluster(ClusterItem clusterItem) =>
            _clusterRepository.RegisterCluster(clusterItem);

        public void SetClusters(IEnumerable<string> clusters) =>
            _clusterRepository.AddAvailableClusters(clusters);

        public IEnumerable<string> GetAvailableClusters() =>
            _clusterRepository.GetAvailableClusters();

        public IEnumerable<string> GetPlacedClustersFromData() =>
            _clusterRepository.GetPlacedClusters();

        public bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot)
        {
            if (!_clusterPlacement.TryPlaceCluster(cluster, wordSlot))
                return false;

            _clusterRepository.MarkClusterAsPlaced(cluster.Text);
            _soundService.Play(SoundTypeId.ClusterPlaced);
            return true;
        }

        public void OnClusterSelected(ClusterItem clusterItem) =>
            _soundService.Play(SoundTypeId.TakeCluster);

        public void ResetCluster(ClusterItem cluster)
        {
            _clusterPlacement.ResetCluster(cluster);
            _clusterRepository.MarkClusterAsAvailable(cluster.Text);
        }

        public void CheckAndHideFilledClusters()
        {
            foreach (var  row  in _wordSlotService.GetRowsWithMatchingWords())
            {
                bool isRowFilledCorrectly = row.Value;

                SetAllClustersOutlineIconActiveInRow(row.Key, !isRowFilledCorrectly);
            }
        }

        public void Cleanup()
        {
            _clusterRepository.Clear();
            _clusterPlacement.Clear();
        }

        public void RestorePlacedClusters()
        {
            List<string> copiedClusters = _clusterRepository.GetPlacedClusters().ToList();

            foreach (string placedCluster in copiedClusters)
            {
                Debug.Log($"placed {placedCluster}");

                if (!_clusterRepository.TryGetCluster(placedCluster, out ClusterItem clusterItem))
                {
                    Debug.LogWarning($"Could not find created cluster item for text: {placedCluster}");
                    continue;
                }

                RestoreClusterToSavedSlot(placedCluster, clusterItem);
            }
        }

        public void Save(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;
            playerData.ClustersByRows.Clear();
            playerData.PlacedClusters.Clear();
            playerData.AvailableClusters.Clear();

            playerData.AvailableClusters.AddRange(_clusterRepository.GetAvailableClusters());
            playerData.PlacedClusters.AddRange(_clusterRepository.GetPlacedClusters());
            playerData.PlacedClustersByRowAndColumns =
                new Dictionary<int, Dictionary<int, string>>(_clusterPlacement.GetClustersByRowAndColumns());
        }

        public void Load(ProgressData progressData)
        {
            _clusterRepository.AddAvailableClusters(progressData.PlayerData.AvailableClusters);
            _clusterRepository.AddPlacedClusters(progressData.PlayerData.PlacedClusters);
            _clusterPlacement.SetClustersByRowAndColumns(progressData.PlayerData.PlacedClustersByRowAndColumns);
        }

        private void RestoreClusterToSavedSlot(string placedCluster, ClusterItem clusterItem)
        {
            var clustersByRowAndColumns = _clusterPlacement.GetClustersByRowAndColumns();

            foreach (var row in clustersByRowAndColumns)
            {
                var columns = new Dictionary<int, string>(row.Value);

                foreach (var column in columns)
                {
                    if (column.Value == placedCluster)
                    {
                        WordSlot wordSlot = _wordSlotService.GetWordSlotByRowAndColumn(row.Key, column.Key);

                        if (wordSlot != null && !wordSlot.IsOccupied)
                        {
                            clusterItem.PlaceToSlot(wordSlot);
                            clusterItem.MarkPlacedTo(wordSlot);
                        }
                    }
                }
            }
        }

        private void SetAllClustersOutlineIconActiveInRow(int row, bool isActive)
        {
            IReadOnlyList<ClusterItem> clustersInRow = _clusterPlacement.GetClustersInRow(row);

            if (clustersInRow == null)
                return;

            foreach (ClusterItem cluster in clustersInRow)
            {
                if (!cluster.IsPlaced)
                    continue;

                Debug.Log($"isactive {isActive}");
                cluster.SetOutlineIconActive(isActive);
                cluster.SetBlocksRaycasts(isActive);
            }
        }
    }
}