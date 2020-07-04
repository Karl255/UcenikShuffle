using System.Collections.Generic;
using System.Threading;
using UcenikShuffle.Common;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
    public class StudentTests
    {
        [Fact]
        public void GetIndexOfId_ShouldWork()
        {
            var shuffler = new Shuffler(1, new List<int>(){2}, new CancellationTokenSource());
            
            //Checking if Id's for created students are correct 
            int expected = 0;
            int actual = Student.GetIndexOfId(shuffler.Students, 1);
            Assert.Equal(expected, actual);
            expected = 1;
            actual = Student.GetIndexOfId(shuffler.Students, 2);
            Assert.Equal(expected, actual);
            expected = -1;
            actual = Student.GetIndexOfId(shuffler.Students, 3);
            Assert.Equal(expected, actual);
        }
    }
}