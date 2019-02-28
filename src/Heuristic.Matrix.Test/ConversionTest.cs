using System;
using System.Linq;
using Xunit;

namespace Heuristic.Matrix.Test
{
    public class ConversionTest
    {
        [Theory] // All should produce same result.
        [InlineData("2,[3-4,6];[2,4-6],1;1,5")]
        [InlineData("2,[1,3-4,6];[4-6],1;1,5")]
        [InlineData("2,[3,4,6];[2,4,5,6],1;1,5")]
        public void ConvertFromStringTest(string input)
        {
            var array = new[]
            {
                new [] { 0, 0, 0, 0, 0, 0, 0 }, // Y = 0
                new [] { 0, 0, 1, 0, 1, 1, 1 }, // Y = 1
                new [] { 0, 0, 0, 0, 0, 0, 0 }, // Y = 2
                new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 3
                new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 4
                new [] { 0, 1, 0, 0, 0, 0, 0 }, // Y = 5
                new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 6
            };
            var selectFromArray = from j in Enumerable.Range(0, array.Length)
                                  from i in Enumerable.Range(0, array[j].Length)
                                  where array[j][i] == 1
                                  select (I: i, J: j);
            var expected = MatrixIndicator.Create(selectFromArray.ToArray(), t => t.I, t => t.J).AsEnumerable(ValueTuple.Create).ToHashSet();
            var actual = MatrixIndicator.Parse(input).AsEnumerable(ValueTuple.Create).ToHashSet();
             
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertToStringTest()
        {
            var array = new[]
            {
                new [] { 0, 0, 0, 0, 0, 0, 0 }, // Y = 0
                new [] { 0, 0, 1, 0, 1, 1, 1 }, // Y = 1
                new [] { 0, 0, 0, 0, 0, 0, 0 }, // Y = 2
                new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 3
                new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 4
                new [] { 0, 1, 0, 0, 0, 0, 0 }, // Y = 5
                new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 6
            };
            var selectFromArray = from j in Enumerable.Range(0, array.Length)
                                  from i in Enumerable.Range(0, array[j].Length)
                                  where array[j][i] == 1
                                  select (I: i, J: j);
            var expected = MatrixIndicator.Create(selectFromArray.ToArray(), t => t.I, t => t.J);
            var actual = MatrixIndicator.Parse(expected.ToString());

            Console.WriteLine(expected);
            Console.WriteLine(actual);

            Assert.Equal(expected.AsEnumerable(ValueTuple.Create).ToHashSet(), actual.AsEnumerable(ValueTuple.Create).ToHashSet());
        }
    }
}