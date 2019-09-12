using System;

namespace Resolink
{
    [Serializable]
    public class SubTarget : IComparable<SubTarget>, IEquatable<SubTarget>
    {
        public int Type;
        public int OptionIndex;

        public bool IsValid()
        {
            return Type != 0 || OptionIndex != 0;
        }

        public int CompareTo(SubTarget other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return ReferenceEquals(null, other) ? 1 : OptionIndex.CompareTo(other.OptionIndex);
        }

        public bool Equals(SubTarget other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Type == other.Type && OptionIndex == other.OptionIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubTarget) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type * 397) ^ OptionIndex;
            }
        }

        public static bool operator ==(SubTarget left, SubTarget right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SubTarget left, SubTarget right)
        {
            return !Equals(left, right);
        }
    }
}