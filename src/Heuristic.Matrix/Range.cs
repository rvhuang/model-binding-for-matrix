using System;
using System.Collections;
using System.Collections.Generic; 

namespace Heuristic.Matrix
{ 
    public struct Range : IEnumerable<int>, IEquatable<Range>
    {
        #region Fields

        public readonly static Range Empty = new Range();

        // all inclusive
        private readonly int? min;
        private readonly int? max;

        #endregion

        #region Properties

        public int? Min => min;

        public int? Max => max;

        public bool IsEmpty => min == null && max == null;

        #endregion

        #region Constructors

        public Range(int value)
        {
            min = value;
            max = value;
        }

        public Range(int min, int max)
        {
            this.min = Math.Min(min, max);
            this.max = Math.Max(min, max);
        }

        #endregion

        #region Methods

        public bool Contains(Range other)
        {
            if (IsEmpty || other.IsEmpty)
                return false;
              
            return min <= other.min && max >= other.max;
        }

        public bool IntersectWith(Range other)
        {
            if (IsEmpty || other.IsEmpty)
                return false;

            if (max.GetValueOrDefault() < other.min.GetValueOrDefault())
                return false;

            if (other.max.GetValueOrDefault() < min.GetValueOrDefault())
                return false;

            return true;
        }

        public Range Intersect(Range other)
        {
            if (IntersectWith(other))
            {
                if (max.GetValueOrDefault() <= other.max.GetValueOrDefault())
                    return new Range(other.min.GetValueOrDefault(), max.GetValueOrDefault());

                if (other.max.GetValueOrDefault() <= max.GetValueOrDefault())
                    return new Range(min.GetValueOrDefault(), other.max.GetValueOrDefault());
            }
            return Empty;
        }

        public IEnumerable<Range> Splits(Range other)
        {
            if (Contains(other))
            {
                yield return new Range(min.GetValueOrDefault(), other.min.GetValueOrDefault());
                yield return other;
                yield return new Range(other.max.GetValueOrDefault(), max.GetValueOrDefault());
                yield break;
            }
            if (other.Contains(this))
            {
                yield return new Range(other.min.GetValueOrDefault(), min.GetValueOrDefault());
                yield return this;
                yield return new Range(max.GetValueOrDefault(), other.max.GetValueOrDefault());
                yield break;
            }
            if (!IntersectWith(other)) yield break;
            if (max.GetValueOrDefault() <= other.max.GetValueOrDefault())
            {
                yield return new Range(min.GetValueOrDefault(), other.min.GetValueOrDefault());
                yield return new Range(other.min.GetValueOrDefault(), max.GetValueOrDefault());
                yield return new Range(max.GetValueOrDefault(), other.max.GetValueOrDefault());
                yield break;
            }
            if (max.GetValueOrDefault() >= other.max.GetValueOrDefault())
            {
                yield return new Range(other.min.GetValueOrDefault(), min.GetValueOrDefault());
                yield return new Range(min.GetValueOrDefault(), other.max.GetValueOrDefault());
                yield return new Range(other.max.GetValueOrDefault(), max.GetValueOrDefault());
                yield break;
            }
        }

        #endregion

        #region Overriding Object Members

        public override int GetHashCode()
        {
            return (max.GetValueOrDefault() % 19) ^ min.GetValueOrDefault();
        }

        public override string ToString()
        {
            if (min == null && max == null) return string.Empty;
            if (min == max) return Convert.ToString(min);
            /*
            if (min != null && max == null) return Convert.ToString(min);
            if (min == null && max != null) return Convert.ToString(max);
            */
            return max.GetValueOrDefault() - min.GetValueOrDefault() > 1 ? $"{min}-{max}" : $"{min},{max}";
        }

        public override bool Equals(object obj)
        {
            return obj is Range && Equals((Range)obj);
        }

        #endregion

        #region Interface Members

        public bool Equals(Range other)
        {
            return min == other.min && max == other.max;
        }

        public IEnumerator<int> GetEnumerator()
        {
            if (min != null && max != null)
                for (var v = min.GetValueOrDefault(); v <= max.GetValueOrDefault(); v++)
                    yield return v;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
