using CodeBase.Data;

namespace CodeBase.UI.Cluster.Services.Placement
{
    public readonly struct ClusterGrid
    {
        private readonly ClusterModel[,] _grid;

        public ClusterGrid(int rowCount, int columnCount)
        {
            _grid = new ClusterModel[rowCount, columnCount];
        }
        
        public bool IsCellOccupied(int row, int column)
        {
            return _grid[row, column].Text != null;
        }

        public void PlaceCluster(ClusterModel cluster, int row, int column)
        {
            _grid[row, column] = cluster;
        }

        public void ClearCell(int row, int column)
        {
            _grid[row, column].Clear();
        }

        public void Clear()
        {
            if (_grid == null)
                return;
            
            for (int row = 0; row < _grid.GetLength(0); row++)
            {
                for (int col = 0; col < _grid.GetLength(1); col++)
                {
                    _grid[row, col].Clear();
                }
            }
        }
    }
} 