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
        private readonly ClusterGrid _clusterGrid  = new(GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn);
        private readonly IWordSlotService _wordSlotService;

        public ClusterPlacement(IWordSlotService wordSlotService)
        {
            _wordSlotService = wordSlotService;
        }

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
            if (!_clustersOccupiedSlots.TryGetValue(cluster, out var slots))
                return;

            foreach (var slot in slots)
            {
                var (row, column) = GetSlotCoordinates(slot);
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

        private bool CanPlaceCluster(ClusterItem cluster, int placeIndex)
        {
            if (IsInvalidStartIndex(cluster, placeIndex))
                return false;

            var startRow = _wordSlotService.GetRowBySlot(_wordSlotService.GetTargetSlot(placeIndex));
            return AreSlotsAvailableForCluster(cluster, placeIndex, startRow);
        }

        private bool AreSlotsAvailableForCluster(ClusterItem cluster, int placeIndex, int startRow)
        {
            var endIndex = placeIndex + cluster.Text.Length;

            for (int i = placeIndex; i < endIndex; i++)
            {
                var slot = _wordSlotService.GetTargetSlot(i);
                var (row, column) = GetSlotCoordinates(slot);

                if (row != startRow || slot.IsOccupied || _clusterGrid.IsCellOccupied(row, column))
                    return false;
            }

            return true;
        }

        private void PlaceCluster(ClusterItem cluster, int placeIndex)
        {
            var occupiedSlots = ListPool<WordSlot>.Get();

            for (int i = 0; i < cluster.Text.Length; i++)
            {
                int slotIndex = placeIndex + i;
                var slot = _wordSlotService.GetTargetSlot(slotIndex);
                var (row, column) = GetSlotCoordinates(slot);
                var character = cluster.Text[i];

                _clusterGrid.PlaceCluster(cluster.ToModel(row, column, cluster.Id), row, column);
                slot.SetText(character);
                _wordSlotService.UpdateCell(row, column, character);
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

        private (int row, int column) GetSlotCoordinates(WordSlot slot) =>
            (_wordSlotService.GetRowBySlot(slot), _wordSlotService.GetColumnBySlot(slot));

        private bool IsInvalidStartIndex(ClusterItem cluster, int startIndex) =>
            startIndex == -1 || startIndex + cluster.Text.Length > _wordSlotService.SlotCount;
    }
}
