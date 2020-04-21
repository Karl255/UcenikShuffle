using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UcenikShuffle.ConsoleApp.Common;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public class StudentTests
	{
		[Fact]
		public void GetIndexOfId_ShouldWork()
		{
			Helpers.ResetData();
			Student.Students.Add(new Student());
			Student.Students.Add(new Student());
			var students = (from s in Student.Students select s).ToList();
			var expected = 0;
			var actual = Student.GetIndexOfId(students, 1);
			Assert.Equal(expected, actual);
			expected = 1;
			actual = Student.GetIndexOfId(students, 2);
			Assert.Equal(expected, actual);
		}
	}
}
