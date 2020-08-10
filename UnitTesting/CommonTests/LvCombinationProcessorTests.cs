using System;
using System.Collections.Generic;
using System.Linq;
using UcenikShuffle.Common;
using UcenikShuffle.Common.Exceptions;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public class LvCombinationProcessorTests
	{

		public static IEnumerable<object[]> GetAllStudentCombinationsShouldWorkData = new List<object[]>()
		{
			//1 group test
			new object[]
			{
				new List<int>() {2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>() {0, 1}
					}
				}
			},
			//2 groups test - same size groups
			new object[]
			{
				new List<int>() {2, 2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>() {0, 1},
						new List<int>() {2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 2},
						new List<int>() {1, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 3},
						new List<int>() {1, 2}
					}
				}
			},
			//2 groups test - different size groups
			new object[]
			{
				new List<int>() {1, 2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1, 2}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {0, 2}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {0, 1}
					}
				}
			},
			//3 groups test - different and same sizes
			new object[]
			{
				new List<int>() {1, 2, 2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1, 2},
						new List<int>() {3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1, 3},
						new List<int>() {2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1, 4},
						new List<int>() {2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {0, 2},
						new List<int>() {3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {0, 3},
						new List<int>() {2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {0, 4},
						new List<int>() {2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {0, 1},
						new List<int>() {3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {0, 3},
						new List<int>() {1, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {0, 4},
						new List<int>() {1, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {0, 1},
						new List<int>() {2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {0, 2},
						new List<int>() {1, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {0, 4},
						new List<int>() {1, 2}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {0, 1},
						new List<int>() {2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {0, 2},
						new List<int>() {1, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {0, 3},
						new List<int>() {1, 2}
					}
				}
			},
			//3 groups test - same sizes
			new object[]
			{
				new List<int>() {2, 2, 2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>() {0, 1},
						new List<int>() {2, 3},
						new List<int>() {4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 1},
						new List<int>() {2, 4},
						new List<int>() {3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 1},
						new List<int>() {2, 5},
						new List<int>() {3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 2},
						new List<int>() {1, 3},
						new List<int>() {4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 2},
						new List<int>() {1, 4},
						new List<int>() {3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 2},
						new List<int>() {1, 5},
						new List<int>() {3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 3},
						new List<int>() {1, 2},
						new List<int>() {4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 3},
						new List<int>() {1, 4},
						new List<int>() {2, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 3},
						new List<int>() {1, 5},
						new List<int>() {2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 4},
						new List<int>() {1, 2},
						new List<int>() {3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 4},
						new List<int>() {1, 3},
						new List<int>() {2, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 4},
						new List<int>() {1, 5},
						new List<int>() {2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 5},
						new List<int>() {1, 2},
						new List<int>() {3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 5},
						new List<int>() {1, 3},
						new List<int>() {2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0, 5},
						new List<int>() {1, 4},
						new List<int>() {2, 3}
					}
				}
			},
			//3 groups test - different sizes
			new object[]
			{
				new List<int>() {1, 2, 3},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1, 2},
						new List<int>() {3, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1, 3},
						new List<int>() {2, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1, 4},
						new List<int>() {2, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1, 5},
						new List<int>() {2, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {2, 3},
						new List<int>() {1, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {2, 4},
						new List<int>() {1, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {2, 5},
						new List<int>() {1, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {3, 4},
						new List<int>() {1, 2, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {3, 5},
						new List<int>() {1, 2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {4, 5},
						new List<int>() {1, 2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {0, 2},
						new List<int>() {3, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {0, 3},
						new List<int>() {2, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {0, 4},
						new List<int>() {2, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {0, 5},
						new List<int>() {2, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {2, 3},
						new List<int>() {0, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {2, 4},
						new List<int>() {0, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {2, 5},
						new List<int>() {0, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {3, 4},
						new List<int>() {0, 2, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {3, 5},
						new List<int>() {0, 2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {4, 5},
						new List<int>() {0, 2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {0, 1},
						new List<int>() {3, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {0, 3},
						new List<int>() {1, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {0, 4},
						new List<int>() {1, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {0, 5},
						new List<int>() {1, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {1, 3},
						new List<int>() {0, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {1, 4},
						new List<int>() {0, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {1, 5},
						new List<int>() {0, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {3, 4},
						new List<int>() {0, 1, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {3, 5},
						new List<int>() {0, 1, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {4, 5},
						new List<int>() {0, 1, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {0, 1},
						new List<int>() {2, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {0, 2},
						new List<int>() {1, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {0, 4},
						new List<int>() {1, 2, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {0, 5},
						new List<int>() {1, 2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {1, 2},
						new List<int>() {0, 4, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {1, 4},
						new List<int>() {0, 2, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {1, 5},
						new List<int>() {0, 2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {2, 4},
						new List<int>() {0, 1, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {2, 5},
						new List<int>() {0, 1, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {3},
						new List<int>() {4, 5},
						new List<int>() {0, 1, 2}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {0, 1},
						new List<int>() {2, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {0, 2},
						new List<int>() {1, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {0, 3},
						new List<int>() {1, 2, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {0, 5},
						new List<int>() {1, 2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {1, 2},
						new List<int>() {0, 3, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {1, 3},
						new List<int>() {0, 2, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {1, 5},
						new List<int>() {0, 2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {2, 3},
						new List<int>() {0, 1, 5}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {2, 5},
						new List<int>() {0, 1, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {4},
						new List<int>() {3, 5},
						new List<int>() {0, 1, 2}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {0, 1},
						new List<int>() {2, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {0, 2},
						new List<int>() {1, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {0, 3},
						new List<int>() {1, 2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {0, 4},
						new List<int>() {1, 2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {1, 2},
						new List<int>() {0, 3, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {1, 3},
						new List<int>() {0, 2, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {1, 4},
						new List<int>() {0, 2, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {2, 3},
						new List<int>() {0, 1, 4}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {2, 4},
						new List<int>() {0, 1, 3}
					},
					new List<List<int>>()
					{
						new List<int>() {5},
						new List<int>() {3, 4},
						new List<int>() {0, 1, 2}
					}
				}
			},
			//3 groups - checking if #30 and #36 issues are fixed
			new object[]
			{
				new List<int>() {1, 1, 2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {1},
						new List<int>() {2, 3},
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {2},
						new List<int>() {1, 3},
					},
					new List<List<int>>()
					{
						new List<int>() {0},
						new List<int>() {3},
						new List<int>() {1, 2},
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {2},
						new List<int>() {0, 3},
					},
					new List<List<int>>()
					{
						new List<int>() {1},
						new List<int>() {3},
						new List<int>() {0, 2},
					},
					new List<List<int>>()
					{
						new List<int>() {2},
						new List<int>() {3},
						new List<int>() {0, 1},
					}
				}
			}
		};

		[Theory]
		[MemberData(nameof(GetAllStudentCombinationsShouldWorkData))]
		public static void GetLvStudentCombinations_ShouldWork(List<int> groupSizes, List<List<List<int>>> expected)
		{
			//Setup
			var students = new List<Student>();
			foreach (int id in Enumerable.Range(1, groupSizes.Sum()).ToList())
			{
				students.Add(new Student(id));
			}

			var actualCombinations = new LvCombinationProcessor(groupSizes, students).LvCombinations.ToList();
			var expectedCombinations = expected.Select(c => new LvCombination(c.Select(g => g.Select(n => students[n]).ToList()).ToList())).ToList();

			//Testing if actual and expected combinations are the same 
			Assert.Equal(expectedCombinations.Count, actualCombinations.Count);
			if (actualCombinations.Count != expected.Count)
			{
				throw new Exception("Actual combination count and expected combination count aren't the same!");
			}

			for (int i = 0; i < actualCombinations.Count; i++)
			{
				if (actualCombinations[i].CompareTo(expectedCombinations[i]) == false)
				{
					throw new Exception("Expected and actual student combinations aren't the same");
				}
			}
		}

		[Theory]
		//2 same groups - different max combination count
		[InlineData(new int[] { 1, 2 }, -10, new int[] { 0, 1, 2 })]
		[InlineData(new int[] { 1, 2 }, -1, new int[] { 0, 1, 2 })]
		[InlineData(new int[] { 1, 2 }, 0, new int[] { 0, 1, 2 })]
		[InlineData(new int[] { 1, 2 }, 1, new int[] { 0 })]
		[InlineData(new int[] { 1, 2 }, 2, new int[] { 0, 2 })]
		[InlineData(new int[] { 1, 2 }, 10, new int[] { 0, 1, 2 })]
		//1 group
		[InlineData(new int[] { 3 }, 0, new int[] { 0 })]
		[InlineData(new int[] { 3 }, 1, new int[] { 0 })]
		//3 groups
		[InlineData(new int[] { 1, 2, 2 }, 0, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 })]
		[InlineData(new int[] { 1, 2, 2 }, 2, new int[] { 0, 8 })]
		[InlineData(new int[] { 1, 1, 2 }, 2, new int[] { 0, 3 })]
		public static void GetLvStudentCombinations_UsingMaxCombinationCount_ShouldWork(int[] groupSizes, int maxCombinationCount, int[] expectedReturnIdexes)
		{
			var students = new List<Student>();
			for (int i = 0; i < groupSizes.Sum(); i++)
			{
				students.Add(new Student(i + 1));
			}
			var limitedCombinations = new LvCombinationProcessor(groupSizes.ToList(), students, maxCombinationCount).LvCombinations.ToList();
			var unlimitedCombinations = new LvCombinationProcessor(groupSizes.ToList(), students).LvCombinations.ToList();
			var actualReturnIndexes = new List<int>();
			foreach (var combination in limitedCombinations)
			{
				var c = unlimitedCombinations.FirstOrDefault(c => combination.CompareTo(c));
				if (c == null)
				{
					throw new Exception("Expected indexes and actual indexes aren't the same!");
				}
				int index = unlimitedCombinations.IndexOf(c);
				actualReturnIndexes.Add(index);
			}
			Assert.Equal(expectedReturnIdexes.Length, actualReturnIndexes.Count);
			Assert.True(actualReturnIndexes.Except(expectedReturnIdexes).Count() == 0);
		}

		[Theory]
		//0
		[InlineData(new int[] { 0 })]
		//Negative number
		[InlineData(new int[] { -1 })]
		//Multiple groups
		[InlineData(new int[] { 1, 2, 3, 0 })]
		[InlineData(new int[] { 1, -1, 2, 3 })]
		public static void GetLvStudentCombinations_ShouldThrowGroupSizeParameterException(int[] groupSizes)
		{
			//Setup
			var students = new List<Student>();
			int studentCount = groupSizes.Sum();
			studentCount = (studentCount < 0) ? 0 : studentCount;
			foreach (int id in Enumerable.Range(1, studentCount))
			{
				students.Add(new Student(id));
			}

			//Testing
			Assert.Throws<GroupSizeException>(() => new LvCombinationProcessor(groupSizes.ToList(), students).LvCombinations.ToList());
		}

		[Fact]
		public static void GetLvStudentCombinations_ShouldThrowArgumentException()
		{
			var groupSizes = new List<int>() { 1 };
			Assert.Throws<ArgumentException>(() => new LvCombinationProcessor(groupSizes, null).LvCombinations.ToList());
			Assert.Throws<ArgumentException>(() => new LvCombinationProcessor(groupSizes, new List<Student>()).LvCombinations.ToList());
		}
	}
}