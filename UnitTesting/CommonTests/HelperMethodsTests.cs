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
		public static IEnumerable<object[]> GetAllStudentCombinationsShouldWorkData = new List<object[]>()
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
			},
			//2 groups - checking if #30 and #36 issues are fixed
			new object[]
			{
				new List<int>(){1,1,2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){1},
						new List<int>(){2,3},
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){2},
						new List<int>(){1,3},
					},
					new List<List<int>>()
					{
						new List<int>(){0},
						new List<int>(){3},
						new List<int>(){1,2},
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2},
						new List<int>(){0,3},
					},
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){3},
						new List<int>(){0,2},
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){3},
						new List<int>(){0,1},
					}
				}
			}
		};
		[Theory]
		[MemberData(nameof(GetAllStudentCombinationsShouldWorkData))]
		private static void GetAllStudentCombinations_ShouldWork(List<int> groupSizes, List<List<List<int>>> expected)
		{
			//Setup
			var students = new List<Student>();
			foreach (int id in Enumerable.Range(1, groupSizes.Sum()).ToList())
			{
				students.Add(new Student() {Id = id});
			}
			var actualCombinations = HelperMethods.GetAllStudentCombinations(groupSizes, students).Select(c => c.Select(g => g.Select(s => s).ToList()).ToList()).ToList();
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
			}
		}

		public static IEnumerable<object[]> GetAllStudentCombinationsShouldThrowGroupSizeParameterExceptionData = new List<object[]>
		{
			//0
			new object[]{new List<int>{ 0 }},
			//Negative number
			new object[]{new List<int>{ -1 }},
			//Multiple groups
			new object[]{new List<int>{ 1,2,3,0 }},
			new object[]{new List<int>{ 1,-1,2,3 }}
		};
		[Theory]
		[MemberData(nameof(GetAllStudentCombinationsShouldThrowGroupSizeParameterExceptionData))]
		private static void GetAllStudentCombinations_ShouldThrowGroupSizeParameterException(List<int> groupSizes)
		{
			//Setup
			var students = new List<Student>();
			int studentCount = groupSizes.Sum();
			studentCount = (studentCount < 0) ? 0 : studentCount;
			foreach (int id in Enumerable.Range(1, studentCount))
			{
				students.Add(new Student(){ Id = id });
			}
			
			//Testing
			Assert.Throws<GroupSizeException>(() => HelperMethods.GetAllStudentCombinations(groupSizes, students).ToList());
		}

		public static IEnumerable<object[]> CompareShuffleRecordsShouldWorkData = new List<object[]>()
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
					new List<int>(){1},
				},
				new List<List<int>>()
				{
					new List<int>(){1}
				}, 
				true
			},
			//Same records
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>(){1,2,3},
				},
				new List<List<int>>()
				{
					new List<int>(){1,2,3}
				},
				true
			},
			//Same records - different order
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>(){1,2,3}
				},
				new List<List<int>>()
				{
					new List<int>(){2,3,1}
				},
				true
			},
			//Different records
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>(){1,2,3}
				},
				new List<List<int>>()
				{
					new List<int>(){1,2,4}
				},
				false
			},
			//Different record lengths
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>(){1,2}
				},
				new List<List<int>>()
				{
					new List<int>(){1,2,3}
				},
				false
			},
			////MULTIPLE GROUPS
			//Different amount of groups
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>(){1},
					new List<int>(){1,2}
				},
				new List<List<int>>()
				{
					new List<int>(){1}
				},
				false
			},
			//Same records
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>(){1},
					new List<int>(){2,3}
				},
				new List<List<int>>()
				{
					new List<int>(){1},
					new List<int>(){3,2}
				},
				true
			},
			//Multiple same size groups, different order
			new object[]
			{
				new List<List<int>>()
				{
					new List<int>(){1},
					new List<int>(){2}
				},
				new List<List<int>>()
				{
					new List<int>(){2},
					new List<int>(){1}
				},
				true
			}
		};
		[Theory]
		[MemberData(nameof(CompareShuffleRecordsShouldWorkData))]
		private static void CompareShuffleRecords_ShouldWork(IList<List<int>> r1, List<List<int>> r2, bool expected)
		{
			//Populating student list with students
			var students = new List<Student>();
			foreach (var group in r1.Concat(r2))
			{
				foreach (int index in group)
				{
					if (students.All(s => s.Id != index))
					{
						students.Add(new Student(){Id = index});
					}
				}
			}
			
			//Converting records from int lists to student lists
			bool actual;
			if (r1.Count != r2.Count)
			{
				actual = false;
				Assert.Equal(expected, actual);
				return;
			}
			var record1 = new List<List<Student>>();
			var record2 = new List<List<Student>>();
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
						record1.Add(studentGroup);
					}
					else
					{
						record2.Add(studentGroup);
					}
				}
			}

			//Checking if records are the same
			actual = HelperMethods.CompareShuffleRecords(record1, record2);
			Assert.Equal(expected, actual);
		}
	}
}