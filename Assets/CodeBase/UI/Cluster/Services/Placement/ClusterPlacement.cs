using System.Collections.Generic;
using CodeBase.Extensions;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordSlots;
using CodeBase.UI.WordSlots.Services;
using UnityEngine.Pool;

namespace CodeBase.UI.Cluster.Services.Placement
{
    public class ClusterPlacement : IClusterPlacement
    {
        private readonly Dictionary<ClusterItem, List<WordSlot>> _clustersOccupiedSlots = new(GameplayConstants.MaxClusterCount);
        private readonly ClusterGrid _clusterGrid = new(GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn);
        private readonly IWordSlotService _wordSlotService;

        public ClusterPlacement(IWordSlotService wordSlotService) => 
            _wordSlotService = wordSlotService;

        public bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot)
        {
            var startIndex = _wordSlotService.IndexOf(wordSlot);
            
            if (!CanPlaceCluster(cluster, startIndex)) 
                return false;

            PlaceCluster(cluster, startIndex);
            return true;
        }

        public void ResetCluster(ClusterItem cluster)
        {
            if (!_clustersOccupiedSlots.TryGetValue(cluster, out var slots)) return;

            foreach (var slot in slots)
            {
                var row = _wordSlotService.GetRowBySlot(slot);
                var column = _wordSlotService.GetColumnBySlot(slot);
                
                slot.Clear();
                ClearGridCells(row, column);
            }

            ListPool<WordSlot>.Release(slots);
            _clustersOccupiedSlots.Remove(cluster);
            _wordSlotService.RefreshFormedWords();
        }

        public void Clear()
        {
            foreach (var slots in _clustersOccupiedSlots.Values)
                ListPool<WordSlot>.Release(slots);
            
            _clustersOccupiedSlots.Clear();
            _clusterGrid.Clear();
        }

        private bool CanPlaceCluster(ClusterItem cluster, int startIndex)
        {
            if (IsInvalidStartIndex(cluster, startIndex))
                return false;

            var startRow = _wordSlotService.GetRowBySlot(_wordSlotService.GetTargetSlot(startIndex));
            var endIndex = startIndex + cluster.Text.Length;

            for (var i = startIndex; i < endIndex; i++)
            {
                var slot = _wordSlotService.GetTargetSlot(i);
                var row = _wordSlotService.GetRowBySlot(slot);
                var column = _wordSlotService.GetColumnBySlot(slot);

                if (row != startRow || slot.IsOccupied || _clusterGrid.IsCellOccupied(row, column))
                    return false;
            }

            return true;
        }

        private void PlaceCluster(ClusterItem cluster, int startIndex)
        {
            var occupiedSlots = ListPool<WordSlot>.Get();
            var endIndex = startIndex + cluster.Text.Length;

            for (var i = startIndex; i < endIndex; i++)
            {
                var slot = _wordSlotService.GetTargetSlot(i);
                var row = _wordSlotService.GetRowBySlot(slot);
                var column = _wordSlotService.GetColumnBySlot(slot);
                var charIndex = i - startIndex;

                _clusterGrid.PlaceCluster(cluster.ToModel(row, column, cluster.Id), row, column);
                slot.SetText(cluster.Text[charIndex]);
                _wordSlotService.UpdateCell(row, column, cluster.Text[charIndex]);
                occupiedSlots.Add(slot);
            }

            _clustersOccupiedSlots[cluster] = occupiedSlots;
            _wordSlotService.RefreshFormedWords();
        }

        private void ClearGridCells(int row, int column)
        {
            _clusterGrid.ClearCell(row, column);
            _wordSlotService.ClearCell(row, column);
        }

        private bool IsInvalidStartIndex(ClusterItem cluster, int startIndex) => 
            startIndex == -1 || startIndex + cluster.Text.Length > _wordSlotService.SlotCount;
    }
}