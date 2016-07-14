using System;

namespace April32
{
    [Serializable]
    public struct WordPart
    {
        public bool Equals(WordPart other)
        {
            return IsBeginning == other.IsBeginning && 
                IsEnding == other.IsEnding && 
                string.Equals(Vow1, other.Vow1) && 
                string.Equals(Vow2, other.Vow2) && 
                string.Equals(Mid, other.Mid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is WordPart && Equals((WordPart)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IsBeginning.GetHashCode();
                hashCode = (hashCode * 397) ^ IsEnding.GetHashCode();
                hashCode = (hashCode * 397) ^ (Vow1?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Vow2?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Mid?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public bool IsBeginning { get; set; }

        public bool IsEnding { get; set; }

        public string Vow1 { get; set; }

        public string Vow2 { get; set; }

        public string Mid { get; set; }

        public int Length => Vow1?.Length + Mid?.Length + Vow2?.Length ?? 0;

        public override string ToString()
        {
            return $"{Vow1}{Mid}{Vow2}{(IsBeginning?" BEG":"")}{(IsEnding?" END":"")}";
        }

    }
}