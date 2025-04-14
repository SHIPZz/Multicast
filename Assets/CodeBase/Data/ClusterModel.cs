using System;

namespace CodeBase.Data
{
    [Serializable]
    public struct ClusterModel : IEquatable<ClusterModel>
    {
        public string Text;
        public bool IsPlaced;
        public int Row;
        public int Column;
        public string Id;

        public ClusterModel(string text, bool isPlaced, int row, int column, string id)
        {
            Text = text;
            IsPlaced = isPlaced;
            Row = row;
            Column = column;
            Id = id;
        }

        public void Clear()
        {
            Text = null;
            IsPlaced = false;
            Row = -1;
            Column = -1;
            Id = null;
        }

        public bool Equals(ClusterModel other)
        {
            return Text == other.Text && IsPlaced == other.IsPlaced && Row == other.Row && Column == other.Column && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is ClusterModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, IsPlaced, Row, Column, Id);
        }
    }
}