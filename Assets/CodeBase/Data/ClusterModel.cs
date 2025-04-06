using System;

namespace CodeBase.Data
{
    [Serializable]
    public struct ClusterModel
    {
        public string Text;
        public bool IsPlaced;
        public int Row;
        public int Column;

        public ClusterModel(string text, bool isPlaced, int row, int column)
        {
            Text = text;
            IsPlaced = isPlaced;
            Row = row;
            Column = column;
        }
    }
} 