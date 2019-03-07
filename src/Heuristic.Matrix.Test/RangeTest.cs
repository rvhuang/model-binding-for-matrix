using Xunit;

namespace Heuristic.Matrix.Test
{
    public class RangeTest
    {
        [Fact]
        public void IntersectWithTrueTest()
        {
            var range1 = new Range(1, 4);
            var range2 = new Range(3, 6);

            Assert.True(range1.IntersectWith(range2));
            Assert.True(range2.IntersectWith(range1));
        }

        [Fact]
        public void IntersectWithFalseTest()
        {
            var range1 = new Range(1, 3);
            var range2 = new Range(4, 6);

            Assert.False(range1.IntersectWith(range2));
            Assert.False(range2.IntersectWith(range1));
        }

        [Fact]
        public void IntersectSuccessTest()
        {
            var range1 = new Range(1, 4);
            var range2 = new Range(3, 6);
            var expected = new Range(3, 4);
            var actual = Range.Intersect(range1, range2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IntersectEmptyTest()
        {
            var range1 = new Range(1, 3);
            var range2 = new Range(4, 6);
            var expected = Range.Empty;
            var actual = Range.Intersect(range1, range2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IntersectReverseSuccessTest()
        {
            var range1 = new Range(1, 4);
            var range2 = new Range(3, 6);
            var expected = new Range(3, 4);
            var actual = Range.Intersect(range2, range1);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IntersectReverseEmptyTest()
        {
            var range1 = new Range(1, 3);
            var range2 = new Range(4, 6);
            var expected = Range.Empty;
            var actual = Range.Intersect(range2, range1);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ContainsTrueTest()
        {
            var range1 = new Range(1, 6);
            var range2 = new Range(4, 3);

            Assert.True(range1.Contains(range2));
            Assert.False(range2.Contains(range1));
        }

        [Fact]
        public void ContainsFalseTest()
        {
            var range1 = new Range(1, 4);
            var range2 = new Range(3, 6);

            Assert.False(range1.Contains(range2));
            Assert.False(range2.Contains(range1));
        }

        [Fact]
        public void SplitsIntoThreeTest()
        {
            var range1 = new Range(1, 4);
            var range2 = new Range(3, 6);
            var expected = new[]
            {
                new Range(1, 3),
                new Range(3, 4),
                new Range(4, 6),
            };
            var actual = range1.Split(range2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitsContainsIntoThreeTest()
        {
            var range1 = new Range(1, 6);
            var range2 = new Range(4, 3);
            var expected = new[]
            {
                new Range(1, 3),
                new Range(3, 4),
                new Range(4, 6),
            };
            var actual = range1.Split(range2);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitsContainsIntoThreeReverseTest()
        {
            var range1 = new Range(1, 6);
            var range2 = new Range(4, 3);
            var expected = new[]
            {
                new Range(1, 3),
                new Range(3, 4),
                new Range(4, 6),
            };
            var actual = range2.Split(range1);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitsIntoThreeReverseTest()
        {
            var range1 = new Range(1, 4);
            var range2 = new Range(3, 6);
            var expected = new[]
            {
                new Range(1, 3),
                new Range(3, 4),
                new Range(4, 6),
            };
            var actual = range2.Split(range1);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SplitsIntoNoneTest()
        {
            var range1 = new Range(1, 3);
            var range2 = new Range(4, 6);

            Assert.Empty(range1.Split(range2));
            Assert.Empty(range2.Split(range1));
        }
    }
}