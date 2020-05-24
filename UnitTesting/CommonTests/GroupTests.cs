using System.Collections.Generic;
using System.Linq;
using UcenikShuffle.Common;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public class GroupTests
	{
		[Fact]
		public void SearchGroupHistory_ShouldWork()
		{
			var shuffler = InsertTestData();
			var students = shuffler.Students;

			//IMPORTANT: this function might stop working if test data is changed
			//Searching for groups of students in history
			var groups = new List<HashSet<Student>>() {
				new HashSet<Student>() { students[0], students[1] },
				new HashSet<Student>() { students[0], students[2] },
				new HashSet<Student>() { students[1], students[2] }
			};
			var results = new[] { 2, 1, 0 };
			for (var i = 0; i < groups.Count; i++)
			{
				var result = Group.SearchGroupHistory(groups[i]);
				Assert.Equal(results[i], result.Count());
			}
		}

		[Fact]
		public void CompareGroupHistoryRecords_ShouldWork()
		{
			var shuffler = InsertTestData();
			var students = shuffler.Students;
			var r1 = new HashSet<Student> { students[0], students[1] };
			var r2 = new HashSet<Student> { students[0], students[2] };
			Assert.True(Group.CompareGroupHistoryRecords(r1, r1));
			Assert.False(Group.CompareGroupHistoryRecords(r1, r2));
			Assert.False(Group.CompareGroupHistoryRecords(r1, null));
			Assert.False(Group.CompareGroupHistoryRecords(null, null));
		}

		private static Shuffler InsertTestData()
		{
			var shuffler = new Shuffler(1, new[]{3});

			var students = shuffler.Students;
			Group.History = new List<HashSet<Student>>();
			Group.History.Add(new HashSet<Student>() { students[0], students[1] });
			Group.History.Add(new HashSet<Student>() { students[0], students[1] });
			Group.History.Add(new HashSet<Student>() { students[0], students[2] });

			return shuffler;
		}
	}
}