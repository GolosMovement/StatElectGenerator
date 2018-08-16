using System;
using Xunit;
using ElectionStatistics.Core.Import;

namespace ElectionStatistics.Tests.Core.Import
{
    public class ColumnLineTest
    {
        [Fact]
        public void ToLetters()
        {
            Assert.Equal("A", ColumnLine.ToLetters(-10));
            Assert.Equal("A", ColumnLine.ToLetters(0));
            Assert.Equal("B", ColumnLine.ToLetters(1));
            Assert.Equal("C", ColumnLine.ToLetters(2));
            Assert.Equal("U", ColumnLine.ToLetters(20));
            Assert.Equal("Z", ColumnLine.ToLetters(25));
            Assert.Equal("AA", ColumnLine.ToLetters(26));
            Assert.Equal("AB", ColumnLine.ToLetters(27));
            Assert.Equal("KF", ColumnLine.ToLetters(291));
            Assert.Equal("AMJ", ColumnLine.ToLetters(1023));
        }

        [Fact]
        public void FromLetters()
        {
            Assert.Equal(1, ColumnLine.FromLetters(""));
            Assert.Equal(1, ColumnLine.FromLetters(null));
            Assert.Equal(1, ColumnLine.FromLetters("A"));
            Assert.Equal(2, ColumnLine.FromLetters("B"));
            Assert.Equal(26, ColumnLine.FromLetters("Z"));
            Assert.Equal(27, ColumnLine.FromLetters("AA"));
            Assert.Equal(28, ColumnLine.FromLetters("AB"));
            Assert.Equal(1024, ColumnLine.FromLetters("AMJ"));
        }
    }
}
