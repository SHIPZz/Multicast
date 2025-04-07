using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.UI.Cluster.Services.Placement;
using CodeBase.UI.Cluster.Services.Repository;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Services;
using CodeBase.UI.WordSlots;
using CodeBase.UI.WordSlots.Services;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;

namespace CodeBase.UI.Cluster.Services
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

        public void SetClusters(IEnumerable<string> clusters)
        {
            using var pooledList = UnityEngine.Pool.ListPool<ClusterModel>.Get(out var clusterModels);

            foreach (var text in clusters)
                clusterModels.Add(new ClusterModel(text, false, -1, -1));

            _clusterRepository.AddAvailableClusters(clusterModels);
        }

        public IEnumerable<string> GetAvailableClusters() =>
            _clusterRepository.GetAvailableClusters().Select(c => c.Text);

        public bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot)
        {
            if (!_clusterPlacement.TryPlaceCluster(cluster, wordSlot))
                return false;

            ClusterModel clusterModel = cluster.ToModel(_wordSlotService.GetRowBySlot(wordSlot), _wordSlotService.GetColumnBySlot(wordSlot));
            _clusterRepository.MarkClusterAsPlaced(clusterModel);
            _soundService.Play(SoundTypeId.ClusterPlaced);
            return true;
        }

        public void OnClusterSelected(ClusterItem clusterItem) =>
            _soundService.Play(SoundTypeId.TakeCluster);

        public void ResetCluster(ClusterItem cluster)
        {
            _clusterPlacement.ResetCluster(cluster);

            var clusterModel = new ClusterModel(cluster.Text, false, -1, -1);

            _clusterRepository.MarkClusterAsAvailable(clusterModel);
        }

        public void CheckAndHideFilledClusters()
        {
            foreach (var (rowId, formedWord) in _wordSlotService.GetFormedWords())
            {
                if (!_wordSlotService.ContainsInTargetWords(formedWord))
                    continue;

                SetAllClustersOutlineIconActiveInRow(rowId, false);
            }
        }

        public void Cleanup()
        {
            _clusterRepository.Clear();
            _clusterPlacement.Clear();
        }

        public void RestorePlacedClusters()
        {
            using var copiedClusters = UnityEngine.Pool.ListPool<ClusterModel>.Get(out var list);

            list.AddRange(_clusterRepository.GetPlacedClusters());

            foreach (ClusterModel placedCluster in list)
            {
                Debug.Log($"placed {placedCluster.Text}");

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
            playerData.PlacedClusters.Clear();
            playerData.AvailableClusters.Clear();

            playerData.AvailableClusters.AddRange(_clusterRepository.GetAvailableClusters());

            SavePlacedClusters(playerData);
        }

        public void Load(ProgressData progressData)
        {
            PlayerData playerData = progressData.PlayerData;

            using var availableModels = UnityEngine.Pool.ListPool<ClusterModel>.Get(out var models);

            FormAvailableClusters(playerData, models);

            _clusterRepository.AddAvailableClusters(models);

            LoadPlacedClusters(playerData);
        }

        private void RestoreClusterToSavedSlot(ClusterModel placedCluster, ClusterItem clusterItem)
        {
            WordSlot wordSlot = _wordSlotService.GetWordSlotByRowAndColumn(placedCluster.Row, placedCluster.Column);

            if (wordSlot != null && !wordSlot.IsOccupied)
            {
                clusterItem.PlaceToSlot(wordSlot);
                clusterItem.MarkPlacedTo(wordSlot);
            }
        }

        private void SetAllClustersOutlineIconActiveInRow(int row, bool isActive)
        {
            IEnumerable<ClusterItem> clustersInRow = _clusterPlacement.GetClustersInRow(row);

            if (clustersInRow == null)
                return;

            foreach (ClusterItem cluster in clustersInRow)
            {
                if (!cluster.IsPlaced)
                    continue;

                cluster.SetOutlineIconActive(isActive);
                cluster.SetBlocksRaycasts(isActive);
            }
        }

        private void SavePlacedClusters(PlayerData playerData)
        {
            foreach (ClusterModel placed in _clusterRepository.GetPlacedClusters())
            {
                if (!playerData.PlacedClusters.TryGetValue(placed.Row, out Dictionary<int, ClusterModel> rowDict))
                {
                    using var pooled = UnityEngine.Pool.DictionaryPool<int, ClusterModel>.Get(out rowDict);
                    
                    playerData.PlacedClusters[placed.Row] = rowDict;
                }

                rowDict[placed.Column] = placed;
            }
        }

        private void LoadPlacedClusters(PlayerData playerData)
        {
            foreach ((var row, Dictionary<int, ClusterModel> value) in playerData.PlacedClusters)
            {
                foreach ((var column, ClusterModel model) in value)
                {
                    _clusterRepository.MarkClusterAsPlaced(model);
                }
            }
        }

        private static void FormAvailableClusters(PlayerData playerData, List<ClusterModel> models)
        {
            foreach (ClusterModel model in playerData.AvailableClusters)
                models.Add(new ClusterModel(model.Text, false, -1, -1));
        }
    }
}