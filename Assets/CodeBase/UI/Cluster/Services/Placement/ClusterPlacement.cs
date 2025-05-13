using System.Collections.Generic;
using CodeBase.Extensions;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordCells;
using CodeBase.UI.WordCells.Services;
using UnityEngine.Pool;

namespace CodeBase.UI.Cluster.Services.Placement
{
    public class ClusterPlacement : IClusterPlacement
    {
        private readonly Dictionary<ClusterItem, List<WordCellView>> _occupiedSlotsByClusters = new(GameplayConstants.MaxClusterCount);
        private readonly ClusterGrid _clusterGrid  = new(GameplayConstants.MaxWordCount, GameplayConstants.MaxClustersInColumn);
        private readonly IWordCellRepository _wordCellRepository;
        private readonly IWordCellChecker _wordCellChecker;

        public ClusterPlacement(IWordCellRepository wordCellRepository, IWordCellChecker wordCellChecker)
        {
            _wordCellChecker = wordCellChecker;
            _wordCellRepository = wordCellRepository;
        }

        public bool TryPlaceCluster(ClusterItem cluster, WordCellView wordCellView)
        {
            var startIndex = _wordCellRepository.IndexOf(wordCellView);

            if (!CanPlaceCluster(cluster, startIndex))
                return false;

            PlaceCluster(cluster, startIndex);
            return true;
        }

        public void ResetCluster(ClusterItem cluster)
        {
            if (!_occupiedSlotsByClusters.TryGetValue(cluster, out List<WordCellView> slots))
                return;

            foreach (WordCellView slot in slots)
            {
                var (row, column) = (slot.Row, slot.Column);
                slot.Clear();
                ClearGridCells(row, column);
            }

            ListPool<WordCellView>.Release(slots);
            _occupiedSlotsByClusters.Remove(cluster);
            _wordCellChecker.RefreshFormedWords();
        }

        public void Clear()
        {
            foreach (var slots in _occupiedSlotsByClusters.Values)
                ListPool<WordCellView>.Release(slots);

            _occupiedSlotsByClusters.Clear();
            _clusterGrid.Clear();
        }

        private bool CanPlaceCluster(ClusterItem cluster, int placeIndex)
        {
            if (IsInvalidStartIndex(cluster, placeIndex))
                return false;

            var startRow = _wordCellRepository.GetTargetSlot(placeIndex).Row;
            return AreSlotsAvailableForCluster(cluster, placeIndex, startRow);
        }

        private bool AreSlotsAvailableForCluster(ClusterItem cluster, int placeIndex, int startRow)
        {
            var endIndex = placeIndex + cluster.Text.Length;

            for (int i = placeIndex; i < endIndex; i++)
            {
                WordCellView cellView = _wordCellRepository.GetTargetSlot(i);
                int row = _wordCellRepository.GetTargetSlot(i).Row;

                if (row != startRow || cellView.IsOccupied)
                    return false;
            }

            return true;
        }

        private void PlaceCluster(ClusterItem cluster, int placeIndex)
        {
            var occupiedSlots = ListPool<WordCellView>.Get();

            for (int i = 0; i < cluster.Text.Length; i++)
            {
                int slotIndex = placeIndex + i;
                var slot = _wordCellRepository.GetTargetSlot(slotIndex);
                var (row, column) = (slot.Row, slot.Column);
                var character = cluster.Text[i];

                _clusterGrid.PlaceCluster(cluster.ToModel(row, column, cluster.Id), row, column);
                slot.SetText(character);
                _wordCellRepository.UpdateCell(row, column, character);
                occupiedSlots.Add(slot);
            }

            _occupiedSlotsByClusters[cluster] = occupiedSlots;
            _wordCellChecker.RefreshFormedWords();
        }

        private void ClearGridCells(int row, int column)
        {
            _clusterGrid.ClearCell(row, column);
            _wordCellRepository.ClearCell(row, column);
        }

        private bool IsInvalidStartIndex(ClusterItem cluster, int startIndex) =>
            startIndex == -1 || startIndex + cluster.Text.Length > _wordCellRepository.SlotCount;
    }
}
