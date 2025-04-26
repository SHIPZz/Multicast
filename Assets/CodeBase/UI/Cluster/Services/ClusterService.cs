using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Common.Services.Persistent;
using CodeBase.Data;
using CodeBase.Extensions;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.Cluster.Services.Placement;
using CodeBase.UI.Sound;
using CodeBase.UI.Sound.Services;
using CodeBase.UI.WordSlots;
using CodeBase.UI.WordSlots.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.UI.Cluster.Services
{
    public class ClusterService : IClusterService, IDisposable, IInitializable, IProgressWatcher
    {
        private readonly IClusterPlacement _clusterPlacement;
        private readonly ISoundService _soundService;
        private readonly IPersistentService _persistentService;
        private readonly IWordSlotRepository _wordSlotRepository;

        private readonly Dictionary<string, ClusterItem> _createdClusters = new();
        private readonly Dictionary<string, ClusterModel> _clusterModels = new();
        private readonly HashSet<string> _placedClusterIds = new();
        private IWordSlotChecker _wordSlotChecker;

        public IEnumerable<ClusterModel> AllClusters => _clusterModels.Values;

        public ClusterService(
            IWordSlotRepository wordSlotRepository,
            IWordSlotChecker wordSlotChecker,
            IPersistentService persistentService,
            ISoundService soundService)
        {
            _wordSlotChecker = wordSlotChecker;
            _wordSlotRepository = wordSlotRepository;
            _persistentService = persistentService;
            _soundService = soundService;
            _clusterPlacement = new ClusterPlacement(wordSlotRepository, wordSlotChecker);
        }

        public void Initialize() => _persistentService.RegisterProgressWatcher(this);
        public void Dispose() => _persistentService.UnregisterProgressWatcher(this);


        public void RegisterCreatedCluster(ClusterItem clusterItem) => 
            _createdClusters[clusterItem.Id] = clusterItem;

        public void SetClusters(IEnumerable<string> clusters)
        {
            foreach (var text in clusters)
            {
                var id = Guid.NewGuid().ToString();
                var model = new ClusterModel(text, false, -1, -1, id);
                _clusterModels[id] = model;
            }
        }

        public IEnumerable<ClusterModel> GetAvailableClusters() => 
            _clusterModels.Values.Where(m => !_placedClusterIds.Contains(m.Id));

        public bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot)
        {
            if (!_clusterPlacement.TryPlaceCluster(cluster, wordSlot))
                return false;

            (int row, int column) = (wordSlot.Row, wordSlot.Column);
            
            ClusterModel model = cluster.ToModel(row, column, cluster.Id);
            _clusterModels[cluster.Id] = model;
            _placedClusterIds.Add(cluster.Id);

            _soundService.Play(SoundTypeId.ClusterPlaced);
            return true;
        }

        public void OnClusterSelected(ClusterItem clusterItem) => 
            _soundService.Play(SoundTypeId.TakeCluster);

        public void ResetCluster(ClusterItem cluster)
        {
            _clusterPlacement.ResetCluster(cluster);
            _placedClusterIds.Remove(cluster.Id);
            _clusterModels[cluster.Id] = cluster.ToModel(-1, -1, cluster.Id);
        }

        public void CheckAndHideFilledClusters()
        {
            var formedWords = _wordSlotChecker.GetFormedWords();
            
            if (formedWords == null) 
                return;

            foreach (var (rowId, formedWord) in formedWords)
            {
                if (!_wordSlotChecker.ContainsInTargetWords(formedWord))
                    continue;

                foreach (var cluster in GetClustersInRow(rowId))
                    cluster.MarkDisabled();
            }
        }

        public void Cleanup()
        {
            _createdClusters.Clear();
            _clusterModels.Clear();
            _placedClusterIds.Clear();
            _clusterPlacement.Clear();
        }

        public void RestorePlacedClusters()
        {
            var placedClusters = _clusterModels.Values
                .Where(m => _placedClusterIds.Contains(m.Id))
                .ToList();

            foreach (var placedCluster in placedClusters)
            {
                if (!_createdClusters.TryGetValue(placedCluster.Id, out var clusterItem))
                {
                    Debug.LogWarning($"Could not find created cluster item for text: {placedCluster}");
                    continue;
                }

                Debug.Log($"Restoring cluster: {placedCluster.Text} at {placedCluster.Row}, {placedCluster.Column}");
                RestoreClusterToSavedSlot(placedCluster, clusterItem);
            }

            _wordSlotChecker.RefreshFormedWords();
            CheckAndHideFilledClusters();
        }

        private void RestoreClusterToSavedSlot(ClusterModel placedCluster, ClusterItem clusterItem)
        {
            WordSlot wordSlot = _wordSlotRepository.GetWordSlotByRowAndColumn(placedCluster.Row, placedCluster.Column);
            
            if (wordSlot == null) 
                return;

            clusterItem.PlaceToSlot(wordSlot);
            _clusterModels[placedCluster.Id] = placedCluster;
            _placedClusterIds.Add(placedCluster.Id);
        }

        private IEnumerable<ClusterItem> GetClustersInRow(int row) => 
            _createdClusters.Values.Where(c => c.Row == row);

        public void Save(ProgressData progressData)
        {
            var playerData = progressData.PlayerData;
            playerData.AvailableClusters.Clear();
            playerData.AvailableClusters.AddRange(GetAvailableClusters());

            SavePlacedClusters(playerData);
        }

        public void Load(ProgressData progressData)
        {
            var playerData = progressData.PlayerData;
            _clusterModels.Clear();
            _placedClusterIds.Clear();

            foreach (var model in playerData.AvailableClusters)
            {
                var newModel = new ClusterModel(model.Text, false, -1, -1, model.Id);
                _clusterModels[model.Id] = newModel;
            }

            LoadPlacedClusters(playerData);
        }

        private void LoadPlacedClusters(PlayerData playerData)
        {
            if (playerData.PlacedClustersGrid == null) return;

            for (int row = 0; row < playerData.PlacedClustersGrid.GetLength(0); row++)
            {
                for (int col = 0; col < playerData.PlacedClustersGrid.GetLength(1); col++)
                {
                    var clusterModel = playerData.PlacedClustersGrid[row, col];
                    if (clusterModel.Text == null) continue;

                    _clusterModels[clusterModel.Id] = clusterModel;
                    _placedClusterIds.Add(clusterModel.Id);
                    Debug.Log($"Loaded placed cluster: {clusterModel.Text} - {clusterModel.Id}");
                }
            }
        }

        private void SavePlacedClusters(PlayerData playerData)
        {
            playerData.PlacedClustersGrid = new ClusterModel[GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn];

            foreach (var placed in _clusterModels.Values.Where(m => _placedClusterIds.Contains(m.Id)))
            {
                Debug.Log($"Saving placed cluster: {placed.Id}: {placed.Text}");
                playerData.PlacedClustersGrid[placed.Row, placed.Column] = placed;
            }
        }
    }
}