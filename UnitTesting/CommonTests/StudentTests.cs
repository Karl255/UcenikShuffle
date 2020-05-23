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
			var shuffler = new Shuffler("0", "2");

			shuffler.Students.Add(new Student());
			shuffler.Students.Add(new Student());
			var students = (from s in shuffler.Students select s).ToList();
			var expected = 0;
			int actual = Student.GetIndexOfId(students, 1);
			Assert.Equal(expected, actual);
			expected = 1;
			actual = Student.GetIndexOfId(students, 2);
			Assert.Equal(expected, actual);
		}
	}
}