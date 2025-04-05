using System.Collections.Generic;
using CodeBase.Gameplay.WordSlots;
using CodeBase.UI.Cluster;
using CodeBase.UI.WordSlots;

namespace CodeBase.UI.Services.Cluster
{
    public class ClusterPlacement : IClusterPlacement
    {
        private readonly Dictionary<string, List<WordSlot>> _clustersOccupiedSlots;
        private readonly Dictionary<int, List<ClusterItem>> _clustersByRow;
        private readonly Dictionary<int, Dictionary<int, string>> _clustersByRowAndColumns;
        private readonly IWordSlotService _wordSlotService;

        public ClusterPlacement(IWordSlotService wordSlotService)
        {
            _wordSlotService = wordSlotService;
            
            _clustersOccupiedSlots = new Dictionary<string, List<WordSlot>>(20);
            _clustersByRow = new Dictionary<int, List<ClusterItem>>(4);
            _clustersByRowAndColumns = new Dictionary<int, Dictionary<int, string>>(20);
        }

        public void Initialize(int rowCount)
        {
            for (int i = 0; i < rowCount; i++)
                _clustersByRow[i] = new List<ClusterItem>(8);
        }

        public bool TryPlaceCluster(ClusterItem cluster, WordSlot wordSlot)
        {
            int startIndex = _wordSlotService.IndexOf(wordSlot);
            
            if (!IsPlacementAvailable(cluster.Text, startIndex))
                return false;

            PlaceCluster(cluster, startIndex);
            return true;
        }

        public void ResetCluster(ClusterItem cluster)
        {
            if (!_clustersOccupiedSlots.TryGetValue(cluster.Text, out List<WordSlot> slots))
                return;

            foreach (WordSlot slot in slots)
            {
                int rowIndex = _wordSlotService.GetRowBySlot(slot);
                int columnIndex = _wordSlotService.GetColumnBySlot(slot);
                
                ClearSlot(rowIndex, columnIndex);
                slot.Clear();
            }

            _clustersOccupiedSlots.Remove(cluster.Text);
        }

        public IReadOnlyList<ClusterItem> GetClustersInRow(int row)
        {
            return _clustersByRow.GetValueOrDefault(row);
        }

        public IReadOnlyDictionary<int, Dictionary<int, string>> GetClustersByRowAndColumns() => 
            _clustersByRowAndColumns;

        public void SetClustersByRowAndColumns(Dictionary<int, Dictionary<int, string>> clusters)
        {
            _clustersByRowAndColumns.Clear();
            foreach (var row in clusters)
            {
                _clustersByRowAndColumns[row.Key] = new Dictionary<int, string>(row.Value);
            }
        }

        public bool IsPlacementAvailable(string clusterText, int startIndex)
        {
            if (startIndex == -1 || startIndex + clusterText.Length > _wordSlotService.SlotCount)
                return false;

            for (int i = 0; i < clusterText.Length; i++)
            {
                if (_wordSlotService.GetTargetSlot(startIndex + i).IsOccupied)
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
                int rowBySlot = _wordSlotService.GetRowBySlot(slot);
                int columnBySlot = _wordSlotService.GetColumnBySlot(slot);

                _clustersByRow[rowBySlot].Add(cluster);

                if (!_clustersByRowAndColumns.TryGetValue(rowBySlot, out Dictionary<int, string> clusterItemsByColumn))
                {
                    clusterItemsByColumn = new Dictionary<int, string>(8);
                    _clustersByRowAndColumns[rowBySlot] = clusterItemsByColumn;
                }

                clusterItemsByColumn[columnBySlot] = cluster.Text;
                slot.SetLetter(cluster.Text[i]);
                occupiedSlots.Add(slot);
            }

            _clustersOccupiedSlots[cluster.Text] = occupiedSlots;
        }

        public void Clear()
        {
            _clustersOccupiedSlots.Clear();
            _clustersByRow.Clear();
            _clustersByRowAndColumns.Clear();
        }

        public void ClearSlot(int row, int column)
        {
            if (_clustersByRowAndColumns.TryGetValue(row, out Dictionary<int, string> rowClusters))
            {
                rowClusters.Remove(column);
                
                if (rowClusters.Count == 0)
                    _clustersByRowAndColumns.Remove(row);
            }
        }
    }
}