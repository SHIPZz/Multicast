﻿using System;
using System.Collections.Generic;

namespace CodeBase.Data
{
    [Serializable]
    public class PlayerData
    {
        public int Level = 1;
        public List<string> WordsToFind = new();
        public List<string> AvailableClusters = new();
        public Dictionary<int, string> ClustersByRows = new();
        public List<string> PlacedClusters = new();
        
        public Dictionary<int, Dictionary<int, string>> PlacedClustersByRowAndColumns = new();
        public Dictionary<int, Dictionary<int, string>> WordSlotsByRowAndColumns = new();
    }
}