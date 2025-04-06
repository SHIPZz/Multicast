using System.Collections.Generic;
using CodeBase.Gameplay.WordSlots;
using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterPlacement : IClusterPlacement
    {
        private readonly Dictionary<ClusterItem, List<WordSlot>> _clustersOccupiedSlots = new(32);
        private readonly Dictionary<int, Dictionary<int, ClusterItem>> _clusters = new(32);
        private readonly IWordSlotService _wordSlotService;

        public IReadOnlyDictionary<int, Dictionary<int, ClusterItem>> Clusters => _clusters;

        public ClusterPlacement(IWordSlotService wordSlotService)
        {
            _wordSlotService = wordSlotService;
        }

        public void Initialize(int rowCount)
        {
            for (int i = 0; i < rowCount; i++)
                _clusters[i] = new Dictionary<int, ClusterItem>(6);
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

            _wordSlotService.RefreshFormedWords();
            _clustersOccupiedSlots.Remove(cluster);
        }

        public IEnumerable<ClusterItem> GetClustersInRow(int row)
        {
            if (_clusters.TryGetValue(row, out var rowClusters))
                return rowClusters.Values;

            return new List<ClusterItem>();
        }

        public bool IsPlacementAvailable(ClusterItem cluster, int startIndex)
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


        public void PlaceCluster(ClusterItem cluster, int startIndex)
        {
            List<WordSlot> occupiedSlots = new List<WordSlot>(cluster.Text.Length);

            for (int i = 0; i < cluster.Text.Length; i++)
            {
                int targetIndex = startIndex + i;
                WordSlot slot = _wordSlotService.GetTargetSlot(targetIndex);
                int row = _wordSlotService.GetRowBySlot(slot);
                int column = _wordSlotService.GetColumnBySlot(slot);

                if (!_clusters.TryGetValue(row, out Dictionary<int, ClusterItem> rowClusters))
                {
                    rowClusters = new Dictionary<int, ClusterItem>();
                    _clusters[row] = rowClusters;
                }

                rowClusters[column] = cluster;
                slot.SetText(cluster.Text[i]);
                occupiedSlots.Add(slot);
            }

            _clustersOccupiedSlots[cluster] = occupiedSlots;
        }

        public void ClearSlot(int row, int column)
        {
            if (_clusters.TryGetValue(row, out var rowClusters))
            {
                rowClusters.Remove(column);

                if (rowClusters.Count == 0)
                    _clusters.Remove(row);
            }
        }

        public void Clear()
        {
            _clustersOccupiedSlots.Clear();
            _clusters.Clear();
        }
    }
}