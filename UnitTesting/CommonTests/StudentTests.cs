using System.Linq;
using UcenikShuffle.Common;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
    public class StudentTests
    {
        [Fact]
        public void GetIndexOfId_ShouldWork()
        {
            var shuffler = new Shuffler(0, new[]{2});
            
            //Checking if Id's for created students are correct 
            var expected = 0;
            var actual = Student.GetIndexOfId(shuffler.Students, 1);
            Assert.Equal(expected, actual);
            expected = 1;
            actual = Student.GetIndexOfId(shuffler.Students, 2);
            Assert.Equal(expected, actual);
        }
    }
}