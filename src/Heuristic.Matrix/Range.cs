using System;
using System.Collections;
using System.Collections.Generic;

namespace Heuristic.Matrix
{
    /// <summary>
    /// Represents an inclusive range of integers.
    /// </summary>
    public struct Range : IReadOnlyList<int>, IEquatable<Range>
    {
        #region Fields

        /// <summary>
        /// Represents an empty range which <see cref="Min"/> and <see cref="Max"/> are null.
        /// </summary>
        public readonly static Range Empty = new Range();

        // all inclusive
        private readonly int? min;
        private readonly int? max;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the minimum value of the range.
        /// </summary>
        public int? Min => min;

        /// <summary>
        /// Gets the maximum value of the range.
        /// </summary>
        public int? Max => max;

        /// <summary>
        /// Gets a boolean value that represents if the instance is empty.
        /// </summary>
        public bool IsEmpty => min == null && max == null;

        int IReadOnlyCollection<int>.Count => IsEmpty ? 0 : max.GetValueOrDefault() - min.GetValueOrDefault() + 1;

        int IReadOnlyList<int>.this[int index]
        {
            get
            {
                if (IsEmpty) throw new IndexOutOfRangeException();

                var value = min.GetValueOrDefault() + index;

                if (value > max.GetValueOrDefault())
                    throw new IndexOutOfRangeException();

                return value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an instance in which minimum value is equal to maximum value.
        /// </summary>
        /// <param name="value">The value of <see cref="Min"/> and <see cref="Max"/>.</param>
        public Range(int value)
        {
            min = value;
            max = value;
        }

        /// <summary>
        /// Creates an instance with minimum value and maximum value.
        /// </summary>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        /// <remarks>If <paramref name="min"/> is greater than <paramref name="max"/>, the values will be exchanged.</remarks>
        public Range(int min, int max)
        {
            this.min = Math.Min(min, max);
            this.max = Math.Max(min, max);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the range contains a specified value.
        /// </summary>
        /// <param name="value">The value to be determined.</param>
        /// <returns><c>true</c> if the range contains the value; otherwise <c>false</c>.</returns>
        public bool Contains(int value)
        {
            return !IsEmpty && min <= value && max >= value;
        }

        /// <summary>
        /// Determines whether the range contains another range.
        /// </summary>
        /// <param name="other">The range to be determined.</param>
        /// <returns><c>true</c> if the range contains the other range; otherwise <c>false</c>.</returns>
        public bool Contains(Range other)
        {
            return !IsEmpty && !other.IsEmpty && min <= other.min && max >= other.max;
        }

        /// <summary>
        /// Determines if current range intersects with <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The instance to test.</param>
        /// <returns>This method returns <c>true</c> if there is any intersection, otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Splits a range into multiple ranges.
        /// </summary>
        /// <param name="other">The range to split.</param>
        /// <returns>An array that consists of multiple ranges.</returns>
        public Range[] Split(Range other)
        {
            if (Contains(other))
            {
                return new[]
                {
                    new Range(min.GetValueOrDefault(), other.min.GetValueOrDefault()),
                    other,
                    new Range(other.max.GetValueOrDefault(), max.GetValueOrDefault()),
                };
            }
            if (other.Contains(this))
            {
                return new[]
                {
                    new Range(other.min.GetValueOrDefault(), min.GetValueOrDefault()),
                    this,
                    new Range(max.GetValueOrDefault(), other.max.GetValueOrDefault()),
                };
            }
            if (!IntersectWith(other)) return Array.Empty<Range>();
            if (max.GetValueOrDefault() <= other.max.GetValueOrDefault())
            {
                return new[]
                {
                    new Range(min.GetValueOrDefault(), other.min.GetValueOrDefault()),
                    new Range(other.min.GetValueOrDefault(), max.GetValueOrDefault()),
                    new Range(max.GetValueOrDefault(), other.max.GetValueOrDefault()),
                };
            }
            if (max.GetValueOrDefault() >= other.max.GetValueOrDefault())
            {
                return new[]
                {
                    new Range(other.min.GetValueOrDefault(), min.GetValueOrDefault()),
                    new Range(min.GetValueOrDefault(), other.max.GetValueOrDefault()),
                    new Range(other.max.GetValueOrDefault(), max.GetValueOrDefault()),
                };
            }
            return Array.Empty<Range>();
        }

        #endregion

        #region Overriding Object Members

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return (max.GetValueOrDefault() % 19) ^ min.GetValueOrDefault();
        }

        /// <summary>
        /// Gets the string that represents current instance.
        /// </summary>
        /// <returns>The string that represents current instance.</returns>
        public override string ToString()
        {
            if (min == null && max == null) return string.Empty;
            if (min == max) return Convert.ToString(min);

            return max.GetValueOrDefault() - min.GetValueOrDefault() > 1 ? $"{min}-{max}" : $"{min},{max}";
        }

        /// <summary>
        /// Determines whether two instances have same range. 
        /// </summary>
        /// <param name="obj">The other instance to compare.</param>
        /// <returns><c>true</c> if two instances have same range; otherwise <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return obj is Range && Equals((Range)obj);
        }

        #endregion

        #region Interface Methods

        /// <summary>
        /// Determines whether two instances have same range. 
        /// </summary>
        /// <param name="other">The other instance to compare.</param>
        /// <returns><c>true</c> if two instances have same range; otherwise <c>false</c>.</returns>
        public bool Equals(Range other)
        {
            return min == other.min && max == other.max;
        }

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            if (min != null && max != null)
                for (var v = min.GetValueOrDefault(); v <= max.GetValueOrDefault(); v++)
                    yield return v;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (min != null && max != null)
                for (var v = min.GetValueOrDefault(); v <= max.GetValueOrDefault(); v++)
                    yield return v;
        }

        #endregion

        #region Others

        /// <summary>
        /// Returns a third <see cref="Range"/> structure that represents the intersection of two other <see cref="Range"/> structures. 
        /// If there is no intersection, <see cref="Empty"/> is returned.
        /// </summary>
        /// <param name="a">A range to intersect.</param>
        /// <param name="b">A range to intersect.</param>
        /// <returns>An instance that represents the intersection of <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static Range Intersect(Range a, Range b)
        {
            if (a.IntersectWith(b))
            {
                if (a.max.GetValueOrDefault() <= b.max.GetValueOrDefault())
                    return new Range(b.min.GetValueOrDefault(), a.max.GetValueOrDefault());

                if (b.max.GetValueOrDefault() <= a.max.GetValueOrDefault())
                    return new Range(a.min.GetValueOrDefault(), b.max.GetValueOrDefault());
            }
            return Empty;
        }

        #endregion
    }
}