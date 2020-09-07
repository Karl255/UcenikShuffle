using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UcenikShuffle.Common;
using UcenikShuffle.Common.Exceptions;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public static class ShufflerTests
	{
		public static IEnumerable<object[]> ShuffleCtorShouldWorkData = new List<object[]>()
		{
			new object[]
			{
				1,
				new List<int>(){1,2,3},
				new CancellationTokenSource()
			},
			new object[]
			{
				1,
				new List<int>(){1,2,3},
				null
			},
		};
		[Theory]
		[MemberData(nameof(ShuffleCtorShouldWorkData))]
		public static void Shuffle_Ctor_ShouldWork(int lvCount, List<int> groupSizes, CancellationTokenSource cancellationTokenSource)
		{
			var shuffler = new Shuffler(lvCount, groupSizes, cancellationTokenSource);

			//Checking if shuffle result field is correct
			Assert.True(shuffler.ShuffleResult != null && shuffler.ShuffleResult.Count == 0);

			//Checking if groups field is correct
			for (int i = 0; i < groupSizes.Count; i++)
			{
				int actualGroupCount = shuffler.Groups.Count(g => g.Size == groupSizes[i]);
				int expectedGroupCount = groupSizes.Count(s => s == groupSizes[i]);
				Assert.Equal(expectedGroupCount, actualGroupCount);
			}

			//Checking if students field is correct
			for (int i = 1; i < groupSizes.Sum(); i++)
			{
				int studentCount = shuffler.Students.Count(s => s.Id == i && s.Label == s.Id.ToString());
				Assert.Equal(1, studentCount);
			}
		}

		[Theory]
		//Null list
		[InlineData(null)]
		//Empty list
		[InlineData(new int[] { })]
		//Group whose size is 0
		[InlineData(new int[] { 0 })]
		[InlineData(new int[] { 0, 1, 2 })]
		//Negative group size
		[InlineData(new int[] { -1 })]
		[InlineData(new int[] { 1, 2, -1, 3 })]
		public static void Shuffle_Ctor_ShouldThrowGroupSizeException(int[] groupSizes)
		{
			Assert.Throws<GroupSizeException>(() => new Shuffler(1, groupSizes?.ToList(), null));
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public static void Shuffle_Ctor_ShouldThrowLvCountException(int lvCount)
		{
			Assert.Throws<LvCountException>(() => new Shuffler(lvCount, new List<int>() { 1 }, null));
		}

		public static IEnumerable<object[]> ShuffleShouldWorkData = new List<object[]>()
		{
			////MORE LV'S THAN COMBINATION CASES
			//1 group of size 1
			new object[]
			{
				1,
				new List<int>(){1},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){1}
					}
				}
			},
			//Only 1 lv should be returned since other lv's are just repeating
			new object[]
			{
				2,
				new List<int>(){1},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){1}
					}
				}
			},
			//Only 3 lv's should be returned since other lv's are repeating
			new object[]
			{
				5,
				new List<int>(){1,2},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,3}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){1,2}
					}
				}
			},
			//Only 5 lv's should be returned since others are just repeating 
			new object[]
			{
				20,
				new List<int>(){2,2,1},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,3},
						new List<int>(){4,5},
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,4},
						new List<int>(){3,5},
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){1,5},
						new List<int>(){2,4},
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){1,3},
						new List<int>(){2,5},
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){1,2},
						new List<int>(){3,4},
					}
				}
			},
			////LESS LV'S THAN COMBINATIONS CASES
			new object[]
			{
				2,
				new List<int>{ 1,2 },
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,3}
					}
				}
			},
			new object[]
			{
				8,
				new List<int>(){1,2,3},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,3},
						new List<int>(){4,5,6}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){2,5},
						new List<int>(){1,3,6}
					},
					new List<List<int>>()
					{
						new List<int>(){6},
						new List<int>(){3,5},
						new List<int>(){1,2,4}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,5},
						new List<int>(){3,4,6}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){1,4},
						new List<int>(){2,5,6}
					},
					new List<List<int>>()
					{
						new List<int>(){6},
						new List<int>(){4,5},
						new List<int>(){1,2,3}
					},
					new List<List<int>>()
					{
						new List<int>(){5},
						new List<int>(){1,6},
						new List<int>(){2,3,4}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){2,6},
						new List<int>(){1,3,5}
					}
				}
			},
			new object[]
			{
				4,
				new List<int>(){1,3,3,3,3},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,3,4},
						new List<int>(){5,6,7},
						new List<int>(){8,9,10},
						new List<int>(){11,12,13}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,3,5},
						new List<int>(){4,8,11},
						new List<int>(){6,9,13},
						new List<int>(){7,10,12}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){1,2,6},
						new List<int>(){4,10,13},
						new List<int>(){5,8,12},
						new List<int>(){7,9,11}
					},
					new List<List<int>>()
					{
						new List<int>(){4},
						new List<int>(){1,7,8},
						new List<int>(){2,5,13},
						new List<int>(){3,9,12},
						new List<int>(){6,10,11}
					}
				}
			},
			new object[]
			{
				3,
				new List<int>(){1,3,3},
				new List<List<List<int>>>()
				{
					new List<List<int>>()
					{
						new List<int>(){1},
						new List<int>(){2,3,4},
						new List<int>(){5,6,7}
					},
					new List<List<int>>()
					{
						new List<int>(){2},
						new List<int>(){1,3,5},
						new List<int>(){4,6,7}
					},
					new List<List<int>>()
					{
						new List<int>(){3},
						new List<int>(){1,2,6},
						new List<int>(){4,5,7}
					}
				}
			}
		};
		[Theory]
		[MemberData(nameof(ShuffleShouldWorkData))]
		public static void Shuffle_ShouldWork(int lvCount, List<int> groupSizes, List<List<List<int>>> expectedIndexes)
		{
			//Creating the shuffler
			var shuffler = new Shuffler(lvCount, groupSizes.ToList(), null);

			//Shuffling twice to check if variables that store shuffle state/result are cleared when shuffle is started
			for (int i = 0; i < 2; i++)
			{
				shuffler.Shuffle();
			}

			//Checking if shuffle result is correct after the shuffle
			var expected = new List<List<List<Student>>>();
			foreach (var lv in expectedIndexes)
			{
				var convertedLv = new List<List<Student>>();
				foreach (var group in lv)
				{
					var convertedGroup = group.Select(i => shuffler.Students.First(s => s.Id == i)).ToList();
					convertedLv.Add(convertedGroup);
				}
				expected.Add(convertedLv);
			}
			var actual = shuffler.ShuffleResult;
			if (expected.Count != actual.Count)
			{
				throw new Exception("Expected and actual lv count aren't the same!");
			}
			for (int i = 0; i < expected.Count; i++)
			{
				if (new LvCombination(expected[i]).CompareTo(actual[i]) == false)
				{
					throw new Exception($"Expected and actual student sitting combinations combinations for lv {i + 1} aren't the same!");
				}
			}
		}

		[Fact]
		public static void Shuffle_ShouldThrowOperationCancelledException()
		{
			var cts = new CancellationTokenSource();
			var shuffler = new Shuffler(1, new List<int>() { 1, 2, 3 }, cts);
			cts.Cancel();
			Assert.Throws<OperationCanceledException>(() => shuffler.Shuffle());
			Assert.Empty(shuffler.ShuffleResult);
		}
	}
}