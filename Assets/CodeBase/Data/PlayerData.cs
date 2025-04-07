using System;
using System.Collections.Generic;

namespace CodeBase.Data
{
    [Serializable]
    public class PlayerData
    {
        public int Level = 1;
        public List<string> WordsToFind = new();
        public List<ClusterModel> AvailableClusters = new();
        
        public ClusterModel[,] PlacedClustersGrid;
        public string[,] WordSlotsGrid;
        
        public int GridRows;
        public int GridColumns;
    }
}