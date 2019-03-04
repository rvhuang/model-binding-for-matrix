using System;
using System.Collections;
using System.Collections.Generic;
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
            var jRange = Range.Empty;

            // Example: o=2,[2-4,6];[2,4,5],1;1,5
            for (var index = 0; index < value.Length; index++)
            {
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
                            iRanges.Clear();
                            jRange = Range.Empty;
                        }
                        cordinate = Cordinate.I;
                        range = false;
                        break;
                }
            }
            if (parseFrom > -1) // not in two brackets. 
            {
                parsed = int.Parse(value.AsSpan(parseFrom, value.Length - parseFrom));
                jRange = range ? new Range(j, parsed) : new Range(parsed);

                foreach (var t in EnumerateFromRanges(iRanges, jRange, converter)) yield return t;
            }
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
                    row.Sort();
                }
                else
                {
                    keyedByI.Add(i, new List<int>(source.Count) { j });
                }
                if (keyedByJ.TryGetValue(j, out var col))
                {
                    col.Add(i);
                    col.Sort();
                }
                else
                {
                    keyedByJ.Add(j, new List<int>(source.Count) { i });
                }
            }
            for (var index = 0; index < source.Count; index++)
            {
                var length = result.Length;
                var t = source[index];
                var i = iSelector(t);
                var j = jSelector(t);

                if (i == 5 || j == 5)
                    Console.WriteLine();

                if (keyedByI.ContainsKey(i) && !keyedByJ.ContainsKey(j))
                {
                    continue;
                }
                else if (!keyedByI.ContainsKey(i) && keyedByJ.ContainsKey(j))
                {
                    continue;
                }
                else if (keyedByI[i].Count >= keyedByJ[j].Count)
                {
                    result.Append(i).Append(',');
                    // result.Append($"[{string.Join(",", keyedByI[i])}]");
                    MergeInto(result, keyedByI[i]);

                    keyedByI.Remove(i);
                    keyedByJ[j].Remove(i);
                }
                else
                {
                    MergeInto(result, keyedByJ[j]);
                    // result.Append($"[{string.Join(",", keyedByJ[j])}]");
                    result.Append(',').Append(j);

                    keyedByJ.Remove(j);
                    keyedByI[i].Remove(j);
                }
                if (length != result.Length) result.Append(';');
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

        private static StringBuilder MergeInto(StringBuilder result, IReadOnlyList<int> indices)
        {
            if (indices.Count == 0) return result;
            if (indices.Count == 1) return result.Append(indices[0]);  

            var temp = indices[0];
            var series = indices[1] - indices[0] == 1;

            result.Append('[');

            for (var index = 1; index < indices.Count; index++)
            {
                switch (indices[index] - indices[index - 1])
                {
                    case 1:
                        series = true;
                        break;

                    case 0:
                        break;

                    default:
                        if (series) 
                        {
                            result.Append($"{temp}-{indices[index - 1]},"); 
                        }
                        else
                        {
                            result.Append($"{indices[index - 1]},"); 
                        }
                        temp = indices[index];
                        series = false;
                        break;
                }
            }
            if (series) 
            {
                result.Append($"{temp}-{indices[indices.Count - 1]}]");
            }
            else
            {
                result.Append($"{indices[indices.Count - 1]}]");
            }
            return result;
        }

        #endregion

    }

    internal enum Cordinate
    {
        I,
        J
    }
}