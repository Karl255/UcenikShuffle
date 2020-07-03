using System;
using System.Collections.Generic;
using System.Linq;
using UcenikShuffle.Common;
using UcenikShuffle.Common.Exceptions;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public static class HelperMethodsTests
	{
		public static IEnumerable<object[]> GetAllStudentCombinations_ShouldWorkData = new List<object[]>()
		{
			//1 group test
			new object[]
			{ 
				new List<int>(){ 2 },
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){0,1}
					}
				} 
			},
			//2 groups test - same size groups
			new object[]
			{
				new List<int>(){ 2,2 },
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){0,1},
						new List<int>(){2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){0,2},
						new List<int>(){1,3}
					},
					new List<List<int>>()
					{
						new List<int>(){0,3},
						new List<int>(){1,2}
					}
				}
			},
			//2 groups test - different size groups
			new object[]
			{
				new List<int>(){ 1,2 }, 
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1,2}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){0,2}
					},new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){0,1}
					}
					
				}
			},
			//3 groups test - different and same sizes
			new object[]
			{
				new List<int>(){ 1,2,2 }, 
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1,2},
						new List<int>(){3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1,3},
						new List<int>(){2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1,4},
						new List<int>(){2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){0,2},
						new List<int>(){3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){0,3},
						new List<int>(){2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){0,4},
						new List<int>(){2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){0,1},
						new List<int>(){3,4}
					},new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){0,3},
						new List<int>(){1,4}
					},new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){0,4},
						new List<int>(){1,3}
					},new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){0,1},
						new List<int>(){2,4}
					},new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){0,2},
						new List<int>(){1,4}
					},new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){0,4},
						new List<int>(){1,2}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){0,1},
						new List<int>(){2,3}
					},new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){0,2},
						new List<int>(){1,3}
					},new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){0,3},
						new List<int>(){1,2}
					}
					
					// new object[]{ 2,2, new int[][] { new[]{0,1} } },
					// new object[]{ 2,3, new int[][] { new[]{0,1}, new[]{0,2}, new[]{1,2} } },
					// new object[]{ 1,1, new int[][] { new[]{0} } },
					// //Unexpected data tests
					// //(higher group size than number count)
					// new object[]{ 5,2, new int[][] { new[]{0,1} } },
					// //small group size
					// new object[]{1,5, new int[][]{ new[]{0}, new[]{1}, new[]{2}, new[]{3}, new[]{4},  }}
			
					////Tests with multiple groups 
				}
			},
			//3 groups test - same sizes
			new object[]
			{
				new List<int>(){2,2,2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){0,1},
						new List<int>(){2,3},
						new List<int>(){4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0,1},
						new List<int>(){2,4},
						new List<int>(){3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0,1},
						new List<int>(){2,5},
						new List<int>(){3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0,2},
						new List<int>(){1,3},
						new List<int>(){4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0,2},
						new List<int>(){1,4},
						new List<int>(){3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0,2},
						new List<int>(){1,5},
						new List<int>(){3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0,3},
						new List<int>(){1,2},
						new List<int>(){4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0,3},
						new List<int>(){1,4},
						new List<int>(){2,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0,3},
						new List<int>(){1,5},
						new List<int>(){2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0,4},
						new List<int>(){1,2},
						new List<int>(){3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0,4},
						new List<int>(){1,3},
						new List<int>(){2,5}
					},
					new List<List<int>>()
					{
						
						new List<int>(){0,4},
						new List<int>(){1,5},
						new List<int>(){2,3}
						
					},
					new List<List<int>>()
					{
						new List<int>(){0,5},
						new List<int>(){1,2},
						new List<int>(){3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0,5},
						new List<int>(){1,3},
						new List<int>(){2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0,5},
						new List<int>(){1,4},
						new List<int>(){2,3}
					}
				}
			},
			//3 groups test - different sizes
			new object[] 
			{
				new List<int>(){1,2,3},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1,2},
						new List<int>(){3,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1,3},
						new List<int>(){2,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1,4},
						new List<int>(){2,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1,5},
						new List<int>(){2,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){2,3},
						new List<int>(){1,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){2,4},
						new List<int>(){1,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){2,5},
						new List<int>(){1,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){3,4},
						new List<int>(){1,2,5}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){3,5},
						new List<int>(){1,2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){4,5},
						new List<int>(){1,2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){0,2},
						new List<int>(){3,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){0,3},
						new List<int>(){2,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){0,4},
						new List<int>(){2,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){0,5},
						new List<int>(){2,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,3},
						new List<int>(){0,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,4},
						new List<int>(){0,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,5},
						new List<int>(){0,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){3,4},
						new List<int>(){0,2,5}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){3,5},
						new List<int>(){0,2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){4,5},
						new List<int>(){0,2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){0,1},
						new List<int>(){3,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){0,3},
						new List<int>(){1,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){0,4},
						new List<int>(){1,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){0,5},
						new List<int>(){1,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,3},
						new List<int>(){0,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,4},
						new List<int>(){0,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,5},
						new List<int>(){0,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){3,4},
						new List<int>(){0,1,5}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){3,5},
						new List<int>(){0,1,4}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){4,5},
						new List<int>(){0,1,3}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){0,1},
						new List<int>(){2,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){0,2},
						new List<int>(){1,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){0,4},
						new List<int>(){1,2,5}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){0,5},
						new List<int>(){1,2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){1,2},
						new List<int>(){0,4,5}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){1,4},
						new List<int>(){0,2,5}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){1,5},
						new List<int>(){0,2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){2,4},
						new List<int>(){0,1,5}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){2,5},
						new List<int>(){0,1,4}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){4,5},
						new List<int>(){0,1,2}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){0,1},
						new List<int>(){2,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){0,2},
						new List<int>(){1,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){0,3},
						new List<int>(){1,2,5}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){0,5},
						new List<int>(){1,2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){1,2},
						new List<int>(){0,3,5}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){1,3},
						new List<int>(){0,2,5}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){1,5},
						new List<int>(){0,2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){2,3},
						new List<int>(){0,1,5}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){2,5},
						new List<int>(){0,1,3}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){3,5},
						new List<int>(){0,1,2}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){0,1},
						new List<int>(){2,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){0,2},
						new List<int>(){1,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){0,3},
						new List<int>(){1,2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){0,4},
						new List<int>(){1,2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){1,2},
						new List<int>(){0,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){1,3},
						new List<int>(){0,2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){1,4},
						new List<int>(){0,2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){2,3},
						new List<int>(){0,1,4}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){2,4},
						new List<int>(){0,1,3}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){3,4},
						new List<int>(){0,1,2}
					}
				}
			}
		};
		
		[Theory]
		[MemberData(nameof(GetAllStudentCombinations_ShouldWorkData))]
		private static void GetAllStudentCombinations_ShouldWork(List<int> groupSizes, List<List<List<int>>> expected)
		{
			//Setup
			var groups = new List<Group>();
			var students = new List<Student>();
			foreach (var size in groupSizes)
			{
				groups.Add(new Group(size));
			}
			foreach (var id in Enumerable.Range(1, groupSizes.Sum()).ToList())
			{
				students.Add(new Student() {Id = id});
			}
			var actualCombinations = HelperMethods.GetAllStudentCombinations(groups, students).Select(c => c.Select(g => g.Select(s => s).ToList()).ToList()).ToList();
			var expectedCombinations = expected.Select(c => c.Select(g => g.Select(n => students[n]).ToList()).ToList()).ToList();
			
			//Testing if actual and expected combinations are the same 
			Assert.Equal(expectedCombinations.Count, actualCombinations.Count);
			if (actualCombinations.Count != expected.Count)
			{
				throw new Exception("Actual combination count and expected combination count aren't the same!");
			}
			for(int i = 0; i < actualCombinations.Count; i++)
			{
				if(HelperMethods.CompareShuffleRecords(actualCombinations[i], expectedCombinations[i]) == false)
				{
					throw new Exception("Expected and actual student combinations aren't the same");
				}
			};
		}

		public static IEnumerable<object[]> GetAllStudentCombinations_ShouldThrowGroupSizeParameterExceptionData = new List<object[]>
		{
			//0
			new object[]{new List<int>{ 0 }},
			//negative number
			new object[]{new List<int>{ -1 }},
			//multiple groups
			new object[]{new List<int>{ 1,2,3,0 }},
			new object[]{new List<int>{ 1,-1,2,3 }}
		};
		
		[Theory]
		[MemberData(nameof(GetAllStudentCombinations_ShouldThrowGroupSizeParameterExceptionData))]
		private static void GetAllStudentCombinations_ShouldThrowGroupSizeParameterException(List<int> groupSizes)
		{
			//Setup
			var groups = new List<Group>();
			var students = new List<Student>();
			foreach (var size in groupSizes)
			{
				groups.Add(new Group(size));
			}
			var studentCount = groupSizes.Sum();
			studentCount = (studentCount < 0) ? 0 : studentCount;
			foreach (var id in Enumerable.Range(1, studentCount))
			{
				students.Add(new Student(){ Id = id });
			}
			
			//Testing
			Assert.Throws<GroupSizeException>(() => HelperMethods.GetAllStudentCombinations(groups, students).ToList());
		}
	}
}