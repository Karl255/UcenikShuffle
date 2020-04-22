using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcenikShuffle.ConsoleApp.Common;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public class GroupTests
	{
		[Fact]
		public void SearchGroupHistory_ShouldWork()
		{
			InsertTestData();
			var students = Student.Students;
			var history = Group.History;

			//Searching for groups of students in history
			List<HashSet<Student>> groups = new List<HashSet<Student>>() { 
				new HashSet<Student>() { students[0], students[1] }, 
				new HashSet<Student>() { students[0], students[2] }, 
				new HashSet<Student>() { students[1], students[2] } 
			};
			var results = new int[] { 2,1,0 };
			for(int i = 0; i < groups.Count; i++)
			{
				var result = Group.SearchGroupHistory(groups[i]);
				Assert.Equal(results[i], result.Count());
			}
		}

		[Fact]
		public void CompareGroupHistoryRecords_ShouldWork()
		{
			InsertTestData();
			var students = Student.Students;
			students.Add(new Student());
			students.Add(new Student());
			students.Add(new Student());
			var r1 = new HashSet<Student> { students[0], students[1] };
			var r2 = new HashSet<Student> { students[0], students[2] };
			Assert.True(Group.CompareGroupHistoryRecords(r1, r1));
			Assert.False(Group.CompareGroupHistoryRecords(r1, r2));
			Assert.False(Group.CompareGroupHistoryRecords(r1, null));
			Assert.False(Group.CompareGroupHistoryRecords(null, null));
		}

		static void InsertTestData()
		{
			Helpers.ResetData();
			for (int i = 0; i < 3; i++)
			{
				Student.Students.Add(new Student());
			}
			var students = Student.Students;
			Group.History.Add(new HashSet<Student>() { students[0], students[1] });
			Group.History.Add(new HashSet<Student>() { students[0], students[1] });
			Group.History.Add(new HashSet<Student>() { students[0], students[2] });
		}
	}
}