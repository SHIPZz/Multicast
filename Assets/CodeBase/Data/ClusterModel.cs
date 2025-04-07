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
        public Guid Id;

        public ClusterModel(string text, bool isPlaced, int row, int column)
        {
            Text = text;
            IsPlaced = isPlaced;
            Row = row;
            Column = column;
            Id = Guid.NewGuid();
        }

        public bool Equals(ClusterModel other)
        {
            return Text == other.Text && IsPlaced == other.IsPlaced && Row == other.Row && Column == other.Column && Id.Equals(other.Id);
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