using System;
using System.Collections.Generic;
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
using Zenject;

namespace CodeBase.UI.Cluster.Services
{
    public class ClusterService : IClusterService, IDisposable, IInitializable
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

        public void Initialize() => _persistentService.RegisterProgressWatcher(_clusterRepository);

        public void Dispose() => _persistentService.UnregisterProgressWatcher(_clusterRepository);

        public void Init() => _clusterPlacement.Initialize(_wordSlotService.WordsToFind.Count);

        public void RegisterCreatedCluster(ClusterItem clusterItem) => _clusterRepository.RegisterCluster(clusterItem);

        public void SetClusters(IEnumerable<string> clusters)
        {
            using var pooledList = UnityEngine.Pool.ListPool<ClusterModel>.Get(out var clusterModels);

            foreach (var text in clusters)
                clusterModels.Add(new ClusterModel(text, false, -1, -1));

            _clusterRepository.AddAvailableClusters(clusterModels);
        }

        public IEnumerable<ClusterModel> GetAvailableClusters() =>
            _clusterRepository.GetAvailableClusters();

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

                MarkClustersDisabledInRow(rowId);
            }
        }

        public IEnumerable<ClusterModel> PlacedClusters => _clusterRepository.GetPlacedClusters();

        public IEnumerable<ClusterModel> AllClusters => _clusterRepository.GetAllClusters();

        public void Cleanup()
        {
            _clusterRepository.Clear();
            _clusterPlacement.Clear();
        }

        public void RestorePlacedClusters()
        {
            using var pool = UnityEngine.Pool.ListPool<ClusterModel>.Get(out List<ClusterModel> list);

            list.AddRange(_clusterRepository.GetPlacedClusters());

            foreach (ClusterModel placedCluster in list)
            {
                if (!_clusterRepository.TryGetCluster(placedCluster, out ClusterItem clusterItem))
                {
                    Debug.LogWarning($"Could not find created cluster item for text: {placedCluster}");
                    continue;
                }

                RestoreClusterToSavedSlot(placedCluster, clusterItem);
            }

            CheckAndHideFilledClusters();
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

        private void MarkClustersDisabledInRow(int row)
        {
            IEnumerable<ClusterItem> clustersInRow = _clusterPlacement.GetClustersInRow(row);

            if (clustersInRow == null)
                return;

            foreach (ClusterItem cluster in clustersInRow) 
                cluster.MarkDisabled();
        }
    }
}