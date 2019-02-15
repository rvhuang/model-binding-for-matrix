using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Heuristic.Matrix
{
    public struct MatrixIndicator
    {
        #region Fields

        public readonly static MatrixIndicator Empty = new MatrixIndicator();

        private readonly string value;

        #endregion

        #region Constructors

        internal MatrixIndicator(string value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion

        #region Methods

        public IEnumerable<T> AsEnumerable<T>(Func<int, int, T> converter)
        {
            if (converter == null) throw new ArgumentNullException(nameof(converter));
            if (value == null) yield break;

            var parseFrom = -1;
            var parsed = 0;
            var multiple = false;
            var range = false;
            var cordinate = Cordinate.I;
            var i = 0;
            var j = 0;

            var iRanges = new List<Range>(8);
            var jRange = default(Range);

            // Example: o=2,[2-4,6];[2,4,5],1;1,5
            for (var index = 0; index < value.Length; index++)
            {
                Debug.WriteLine("{0}: {1}", index, value[index]);

                if (char.IsDigit(value[index]))
                {
                    if (parseFrom == -1)
                        parseFrom = index;

                    continue;
                }
                switch (value[index])
                {
                    case ',':
                        if (parseFrom == -1) continue; // example: ],
                        switch (cordinate)
                        {
                            case Cordinate.I:
                                parsed = int.Parse(value.AsSpan(parseFrom, index - parseFrom));
                                iRanges.Add(range ? new Range(i, parsed) : new Range(parsed));
                                j = 0;
                                cordinate = multiple ? Cordinate.I : Cordinate.J;
                                break;

                            case Cordinate.J:
                                parsed = int.Parse(value.AsSpan(parseFrom, index - parseFrom));
                                jRange = range ? new Range(j, parsed) : new Range(parsed);

                                foreach (var t in EnumerateFromRanges(iRanges, jRange, converter)) yield return t;

                                i = 0;
                                cordinate = multiple ? Cordinate.J : Cordinate.I;
                                break;
                        }
                        parseFrom = -1;
                        range = false;
                        break;
                    case '[':
                        multiple = true; // Now we are in between two brackets.
                        parseFrom = -1;
                        range = false;
                        break;
                    case ']':
                        switch (cordinate)
                        {
                            case Cordinate.I:
                                parsed = int.Parse(value.AsSpan(parseFrom, index - parseFrom));
                                iRanges.Add(range ? new Range(i, parsed) : new Range(parsed));
                                j = 0;
                                cordinate = Cordinate.J;
                                break;

                            case Cordinate.J:
                                parsed = int.Parse(value.AsSpan(parseFrom, index - parseFrom));
                                jRange = range ? new Range(j, parsed) : new Range(parsed);

                                foreach (var t in EnumerateFromRanges(iRanges, jRange, converter)) yield return t;

                                i = 0;
                                cordinate = Cordinate.I;
                                break;
                        }
                        multiple = false; // Now we are out of two brackets.
                        parseFrom = -1;
                        range = false;
                        break;
                    case '-':
                        switch (cordinate)
                        {
                            case Cordinate.I:
                                i = int.Parse(value.AsSpan(parseFrom, index - parseFrom));
                                break;

                            case Cordinate.J:
                                j = int.Parse(value.AsSpan(parseFrom, index - parseFrom));
                                break;
                        }
                        parseFrom = -1;
                        range = true;
                        break;
                    case ';':
                        if (parseFrom > -1) // not in two brackets. 
                        {
                            parsed = int.Parse(value.AsSpan(parseFrom, index - parseFrom));
                            jRange = range ? new Range(j, parsed) : new Range(parsed);

                            foreach (var t in EnumerateFromRanges(iRanges, jRange, converter)) yield return t;

                            i = 0;
                            parseFrom = -1;
                        }
                        iRanges.Clear();
                        cordinate = Cordinate.I;
                        range = false;
                        break;
                }
            }
            foreach (var t in EnumerateFromRanges(iRanges, jRange, converter)) yield return t;
        }

        public override string ToString()
        {
            return value;
        }

        #endregion

        #region Public Helpers

        public static MatrixIndicator Parse(string value)
        {
            return new MatrixIndicator(value);
        }

        public static MatrixIndicator Create<T>(T[][] source, Func<int, int, T, bool> filter)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (filter == null) throw new ArgumentNullException(nameof(filter)); 

            var value = new StringBuilder();
            var iList = new HashSet<int>(128);
            var jList = new HashSet<int>(128);

            for (var j = 0; j < source.Length; j++)
            {
                if (source[j] == null || source[j].Length == 0) continue;

                for (var i = 0; i < source[j].Length; i++)
                {
                    if (filter(i, j, source[i][j]))
                    {

                    }
                }
            }


            return Parse(value.ToString());
        }

        public static MatrixIndicator Create<T>(IReadOnlyList<T> source, Func<T, int> iSelector, Func<T, int> jSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (iSelector == null) throw new ArgumentNullException(nameof(iSelector));
            if (jSelector == null) throw new ArgumentNullException(nameof(jSelector));
            if (source.Count == 0) return Empty;

            var result = new StringBuilder();
            var keyedByI = new Dictionary<int, List<int>>(source.Count); 
            var keyedByJ = new Dictionary<int, List<int>>(source.Count);
            
            for (var index = 0; index < source.Count; index++)
            {
                var t = source[index];
                var i = iSelector(t);
                var j = jSelector(t);

                if (keyedByI.TryGetValue(i, out var row))
                {
                    row.Add(j);
                }
                else
                {
                    keyedByI.Add(i, new List<int>(source.Count) { j });
                }
                if (keyedByJ.TryGetValue(j, out var col))
                {
                    row.Add(i);
                }
                else
                {
                    keyedByJ.Add(j, new List<int>(source.Count) { i });
                }
            }
            foreach (var kvp in keyedByI)
            {
                var i = kvp.Key;
                var jList = kvp.Value;

                if (jList.Count >= keyedByJ[i].Count)
                { 
                }
                else
                {

                }
                result.Append(';');
            }

            return Parse(result.ToString());
        }

        #endregion

        #region Private Helpers

        private static IEnumerable<T> EnumerateFromRanges<T>(IEnumerable<Range> iRanges, Range jRange, Func<int, int, T> converter)
        {
            foreach (var iRange in iRanges)
                foreach (var i in iRange)
                    foreach (var j in jRange)
                        yield return converter(i, j);
        }

        private static void MergeInto(IReadOnlyList<int> indices, StringBuilder result)
        {
            if (indices.Count == 0) return;

            var temp = indices[0];
            var diff = 1;

            for (var index = 1; index < indices.Count; index++)
            {
                switch (indices[index] - temp)
                {
                    case 1:
                        if (diff != 1)
                        {
                            result.Append($"{temp}-{indices[index - 1]},");
                            temp = indices[index];
                        }
                        continue;

                    case 0:
                        break;

                    default:
                        result.Append($"{temp},");
                        temp = indices[index];
                        break;
                }
            }
            result.Append($"{temp}");
        }

        #endregion

    }

    internal enum Cordinate
    {
        I,
        J
    }

    internal struct Range : IEnumerable<int>
    {
        // all inclusive
        private readonly int min;
        private readonly int max;

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

        public IEnumerator<int> GetEnumerator()
        {
            for (var v = min; v <= max; v++)
                yield return v;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}