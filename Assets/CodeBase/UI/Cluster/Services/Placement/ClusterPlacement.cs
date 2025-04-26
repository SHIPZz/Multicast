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
        private readonly Dictionary<ClusterItem, List<WordSlot>> _occupiedSlotsByClusters = new(GameplayConstants.MaxClusterCount);
        private readonly ClusterGrid _clusterGrid  = new(GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn);
        private readonly IWordSlotRepository _wordSlotRepository;
        private readonly IWordSlotChecker _wordSlotChecker;

        public ClusterPlacement(IWordSlotRepository wordSlotRepository, IWordSlotChecker wordSlotChecker)
        {
            _wordSlotChecker = wordSlotChecker;
            _wordSlotRepository = wordSlotRepository;
        }

        public bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot)
        {
            var startIndex = _wordSlotRepository.IndexOf(wordSlot);

            if (!CanPlaceCluster(cluster, startIndex))
                return false;

            PlaceCluster(cluster, startIndex);
            return true;
        }

        public void ResetCluster(ClusterItem cluster)
        {
            if (!_occupiedSlotsByClusters.TryGetValue(cluster, out List<WordSlot> slots))
                return;

            foreach (WordSlot slot in slots)
            {
                var (row, column) = (slot.Row, slot.Column);
                slot.Clear();
                ClearGridCells(row, column);
            }

            ListPool<WordSlot>.Release(slots);
            _occupiedSlotsByClusters.Remove(cluster);
            _wordSlotChecker.RefreshFormedWords();
        }

        public void Clear()
        {
            foreach (var slots in _occupiedSlotsByClusters.Values)
                ListPool<WordSlot>.Release(slots);

            _occupiedSlotsByClusters.Clear();
            _clusterGrid.Clear();
        }

        private bool CanPlaceCluster(ClusterItem cluster, int placeIndex)
        {
            if (IsInvalidStartIndex(cluster, placeIndex))
                return false;

            var startRow = _wordSlotRepository.GetTargetSlot(placeIndex).Row;
            return AreSlotsAvailableForCluster(cluster, placeIndex, startRow);
        }

        private bool AreSlotsAvailableForCluster(ClusterItem cluster, int placeIndex, int startRow)
        {
            var endIndex = placeIndex + cluster.Text.Length;

            for (int i = placeIndex; i < endIndex; i++)
            {
                WordSlot slot = _wordSlotRepository.GetTargetSlot(i);
                int row = _wordSlotRepository.GetTargetSlot(i).Row;

                if (row != startRow || slot.IsOccupied)
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
                var slot = _wordSlotRepository.GetTargetSlot(slotIndex);
                var (row, column) = (slot.Row, slot.Column);
                var character = cluster.Text[i];

                _clusterGrid.PlaceCluster(cluster.ToModel(row, column, cluster.Id), row, column);
                slot.SetText(character);
                _wordSlotRepository.UpdateCell(row, column, character);
                occupiedSlots.Add(slot);
            }

            _occupiedSlotsByClusters[cluster] = occupiedSlots;
            _wordSlotChecker.RefreshFormedWords();
        }

        private void ClearGridCells(int row, int column)
        {
            _clusterGrid.ClearCell(row, column);
            _wordSlotRepository.ClearCell(row, column);
        }

        private bool IsInvalidStartIndex(ClusterItem cluster, int startIndex) =>
            startIndex == -1 || startIndex + cluster.Text.Length > _wordSlotRepository.SlotCount;
    }
}
