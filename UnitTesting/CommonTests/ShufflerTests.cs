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
		//TODO: test if cancellation works (both when cancellation token is null and then it isn't)
		//TODO: check if shuffle algorithm works
		
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
		private static void Shuffle_Ctor_ShouldWork(int lvCount, List<int> groupSizes, CancellationTokenSource cancellationTokenSource)
		{
			var shuffler = new Shuffler(lvCount, groupSizes, cancellationTokenSource);
			
			//Checking if lv count parameter is correct 
			Assert.Equal(lvCount, shuffler.LvCount);
			
			//Checking if group list parameter is correct
			for (int i = 0; i < groupSizes.Count; i++)
			{
				int actualGroupCount = shuffler.Groups.Count(g => g.Size == groupSizes[i]);
				int expectedGroupCount = groupSizes.Count(s => s == groupSizes[i]);
				Assert.Equal(expectedGroupCount, actualGroupCount);
			}
			
			//Checking if student list parameter is correct
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
		[InlineData(new int[]{})]
		//Group whose size is 0
		[InlineData(new int[]{0})]
		[InlineData(new int[]{0,1,2})]
		//Negative group size
		[InlineData(new int[]{-1})]
		[InlineData(new int[]{1,2,-1,3})]
		private static void Shuffle_Ctor_ShouldThrowGroupSizeException(int[] groupSizes)
		{
			Assert.Throws<GroupSizeException>(() => new Shuffler(1, groupSizes?.ToList(), null));
		}
		
		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		private static void Shuffle_Ctor_ShouldThrowLvCountException(int lvCount)
		{
			Assert.Throws<LvCountException>(() => new Shuffler(lvCount, new List<int>() {1}, null));
		}
	}
}