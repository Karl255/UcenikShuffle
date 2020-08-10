using System;
using System.Collections.Generic;
using System.Linq;
using UcenikShuffle.Common;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public class LvCombinationTests
	{
		public static IEnumerable<object[]> CompareToShouldWorkData = new List<object[]>()
		{
			//EMPTY RECORDS
			new object[]
			{
				new List<List<int>>(),
				new List<List<int>>(),
				true
			},
			////SINGLE GROUP
			//1 (same) student in both records
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>() {1},
				},
				new List<List<int>>()
				{
					new List<int>() {1}
				},
				true
			},
			//Same records
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>() {1, 2, 3},
				},
				new List<List<int>>()
				{
					new List<int>() {1, 2, 3}
				},
				true
			},
			//Same records - different order
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>() {1, 2, 3}
				},
				new List<List<int>>()
				{
					new List<int>() {2, 3, 1}
				},
				true
			},
			//Different records
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>() {1, 2, 3}
				},
				new List<List<int>>()
				{
					new List<int>() {1, 2, 4}
				},
				false
			},
			//Different record lengths
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>() {1, 2}
				},
				new List<List<int>>()
				{
					new List<int>() {1, 2, 3}
				},
				false
			},
			////MULTIPLE GROUPS
			//Different amount of groups
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>() {1},
					new List<int>() {1, 2}
				},
				new List<List<int>>()
				{
					new List<int>() {1}
				},
				false
			},
			//Same records
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>() {1},
					new List<int>() {2, 3}
				},
				new List<List<int>>()
				{
					new List<int>() {1},
					new List<int>() {3, 2}
				},
				true
			},
			//Multiple same size groups, different order
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>() {1},
					new List<int>() {2}
				},
				new List<List<int>>()
				{
					new List<int>() {2},
					new List<int>() {1}
				},
				true
			}
		};

		[Fact]
		private static void Ctor_ShouldWork()
		{
			var lvCombination = new LvCombination(null);
			Assert.Null(lvCombination.Combination);
			var combination = new List<List<Student>>() { new List<Student>() { new Student(0) } };
			lvCombination = new LvCombination(combination);
			Assert.Equal(combination, lvCombination.Combination);
		}

		[Theory]
		[MemberData(nameof(CompareToShouldWorkData))]
		private static void CompareTo_ShouldWork(List<List<int>> r1, List<List<int>> r2, bool expected)
		{
			//Populating student list with students
			var students = new List<Student>();
			foreach (var group in r1.Concat(r2))
			{
				foreach (int index in group)
				{
					if (students.All(s => s.Id != index))
					{
						students.Add(new Student(index));
					}
				}
			}

			//Converting records from int lists to student lists
			Func<List<List<int>>, List<List<Student>>> numberRecordToStudentRecord = record => record.Select(g => g.Select(n => students[students.IndexOf(students.First(s => s.Id == n))]).ToList()).ToList();
			var record1 = new LvCombination(numberRecordToStudentRecord(r1));
			var record2 = new LvCombination(numberRecordToStudentRecord(r2));
			for (int i = 0; i < 2; i++)
			{
				foreach (var group in (i == 0) ? r1 : r2)
				{
					var studentGroup = new List<Student>();
					foreach (int index in group)
					{
						studentGroup.Add(students.First(s => s.Id == index));
					}

					if (i == 0)
					{
						record1.Combination.Add(studentGroup);
					}
					else
					{
						record2.Combination.Add(studentGroup);
					}
				}
			}

			//Checking if records are the same
			bool actual = record1.CompareTo(record2);
			Assert.Equal(expected, actual);
		}

		[Fact]
		private static void CompareTo_ShouldThrowArgumentException()
		{
			Assert.Throws<ArgumentException>(() =>
				new LvCombination(null).CompareTo(new LvCombination(new List<List<Student>>())));
			Assert.Throws<ArgumentException>(() =>
				new LvCombination(new List<List<Student>>()).CompareTo(new LvCombination(null)));
			Assert.Throws<ArgumentException>(() =>
				new LvCombination(null).CompareTo(new LvCombination(null)));
		}
	}
}