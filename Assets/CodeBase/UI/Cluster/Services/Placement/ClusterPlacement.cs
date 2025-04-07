using System.Collections.Generic;
using UnityEngine.Pool;
using CodeBase.Gameplay.Constants;
using CodeBase.UI.WordSlots;
using CodeBase.UI.WordSlots.Services;

namespace CodeBase.UI.Cluster.Services.Placement
{
    public class ClusterPlacement : IClusterPlacement
    {
        private readonly Dictionary<ClusterItem, List<WordSlot>> _clustersOccupiedSlots =
            new(GameplayConstants.MaxClusterCount);

        private ClusterItem[,] _clustersGrid;
        private readonly IWordSlotService _wordSlotService;

        public ClusterPlacement(IWordSlotService wordSlotService)
        {
            _wordSlotService = wordSlotService;
        }

        public void Initialize(int rowCount)
        {
            _clustersGrid = new ClusterItem[rowCount, GameplayConstants.MaxClustersInColumn];
        }

        public bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot)
        {
            int startIndex = _wordSlotService.IndexOf(wordSlot);

            if (!IsPlacementAvailable(cluster, startIndex))
                return false;

            PlaceCluster(cluster, startIndex);
            return true;
        }

        public void ResetCluster(ClusterItem cluster)
        {
            if (!_clustersOccupiedSlots.TryGetValue(cluster, out List<WordSlot> slots))
                return;

            foreach (WordSlot slot in slots)
            {
                int rowIndex = _wordSlotService.GetRowBySlot(slot);
                int columnIndex = _wordSlotService.GetColumnBySlot(slot);

                ClearSlot(rowIndex, columnIndex);
                slot.Clear();
            }

            ListPool<WordSlot>.Release(slots);
            _clustersOccupiedSlots.Remove(cluster);
            _wordSlotService.RefreshFormedWords();
        }

        public IEnumerable<ClusterItem> GetClustersInRow(int row)
        {
            var rowClusters = ListPool<ClusterItem>.Get();
            
            for (int col = 0; col < _clustersGrid.GetLength(1); col++)
            {
                if (_clustersGrid[row, col] != null)
                {
                    rowClusters.Add(_clustersGrid[row, col]);
                }
            }
            
            return rowClusters;
        }

        private bool IsPlacementAvailable(ClusterItem cluster, int startIndex)
        {
            if (startIndex == -1 || startIndex + cluster.Text.Length > _wordSlotService.SlotCount)
                return false;

            int startRow = _wordSlotService.GetRowBySlot(_wordSlotService.GetTargetSlot(startIndex));

            for (int i = 0; i < cluster.Text.Length; i++)
            {
                int targetIndex = startIndex + i;
                WordSlot slot = _wordSlotService.GetTargetSlot(targetIndex);
                int slotRow = _wordSlotService.GetRowBySlot(slot);
                int slotCol = _wordSlotService.GetColumnBySlot(slot);

                if (slotRow != startRow || slot.IsOccupied || _clustersGrid[slotRow, slotCol] != null)
                    return false;
            }

            return true;
        }

        private void PlaceCluster(ClusterItem cluster, int startIndex)
        {
            var occupiedSlots = ListPool<WordSlot>.Get();

            for (int i = 0; i < cluster.Text.Length; i++)
            {
                int targetIndex = startIndex + i;
                WordSlot slot = _wordSlotService.GetTargetSlot(targetIndex);
                int row = _wordSlotService.GetRowBySlot(slot);
                int column = _wordSlotService.GetColumnBySlot(slot);

                _clustersGrid[row, column] = cluster;
                slot.SetText(cluster.Text[i]);
                occupiedSlots.Add(slot);
            }

            _clustersOccupiedSlots[cluster] = occupiedSlots;
        }

        private void ClearSlot(int row, int column)
        {
            _clustersGrid[row, column] = null;
        }

        public void Clear()
        {
            foreach (List<WordSlot> list in _clustersOccupiedSlots.Values)
            {
                ListPool<WordSlot>.Release(list);
            }
            
            _clustersOccupiedSlots.Clear();
            
            if (_clustersGrid != null)
            {
                for (int row = 0; row < _clustersGrid.GetLength(0); row++)
                {
                    for (int col = 0; col < _clustersGrid.GetLength(1); col++)
                    {
                        _clustersGrid[row, col] = null;
                    }
                }
            }
        }
    }
}