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

        private readonly Dictionary<int, Dictionary<int, ClusterItem>> _clusters =
            new(GameplayConstants.MaxClusterCount);

        private readonly IWordSlotService _wordSlotService;

        public ClusterPlacement(IWordSlotService wordSlotService)
        {
            _wordSlotService = wordSlotService;
        }

        public void Initialize(int rowCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                _clusters[i] = new Dictionary<int, ClusterItem>(GameplayConstants.MaxClustersInColumn);
            }
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
            if (_clusters.TryGetValue(row, out var rowClusters))
                return rowClusters.Values;

            return ListPool<ClusterItem>.Get();
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

                if (slotRow != startRow || slot.IsOccupied)
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

                if (!_clusters.TryGetValue(row, out Dictionary<int, ClusterItem> rowClusters))
                {
                      rowClusters = DictionaryPool<int, ClusterItem>.Get();
                    _clusters[row] = rowClusters;
                }

                rowClusters[column] = cluster;
                slot.SetText(cluster.Text[i]);
                occupiedSlots.Add(slot);
            }

            _clustersOccupiedSlots[cluster] = occupiedSlots;
        }

        private void ClearSlot(int row, int column)
        {
            if (_clusters.TryGetValue(row, out Dictionary<int, ClusterItem> rowClusters))
            {
                rowClusters.Remove(column);

                if (rowClusters.Count == 0)
                {
                    DictionaryPool<int, ClusterItem>.Release(rowClusters);
                    _clusters.Remove(row);
                }
            }
        }

        public void Clear()
        {
            foreach (List<WordSlot> list in _clustersOccupiedSlots.Values)
            {
                ListPool<WordSlot>.Release(list);
            }

            foreach (Dictionary<int, ClusterItem> clusters in _clusters.Values)
            {
                DictionaryPool<int, ClusterItem>.Release(clusters);
            }
            
            _clusters.Clear();
            _clustersOccupiedSlots.Clear();
        }
    }
}