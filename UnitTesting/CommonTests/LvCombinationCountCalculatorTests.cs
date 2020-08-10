using System.Linq;
using UcenikShuffle.Common;
using UcenikShuffle.Common.Exceptions;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public class LvCombinationCountCalculatorTests
	{
		//TODO: check once again if expected variable for all cases is correct
		//TODO: test what happens to GetCombinationCount if unexpected variables are passed to it (negative group size, empty group sizes list etc.)
		//TODO: add unit tests where number of available students is higher than group sizes sum
		[Theory]
		[InlineData(new[] { 2, 2, 3 }, 105)]
		[InlineData(new[] { 6, 1 }, 7)]
		[InlineData(new[] { 1, 6 }, 7)]
		[InlineData(new[] { 1, 2, 3 }, 60)]
		[InlineData(new[] { 1, 3, 3 }, 70)]
		[InlineData(new[] { 1, 2, 2 }, 15)]
		[InlineData(new[] { 1, 2, 3, 4 }, 12600)]
		[InlineData(new[] { 5 }, 1)]
		[InlineData(new[] { 1, 1, 1 }, 1)]
		[InlineData(new[] { 5, 4 }, 126)]
		[InlineData(new[] { 1, 3, 3, 3, 3 }, 200200)]
		[InlineData(new[] { 1, 2, 1 }, 6)]
		//[InlineData(new[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 5, 5, 5 }, someAmmount)]
		[InlineData(new[] { 1, 1, 2, 3 }, 210)]
		[InlineData(new[] { 2, 2 }, 3)]
		[InlineData(null, 0)]
		[InlineData(new int[] { }, 0)]
		private static void GetLvCombinationCount_ShouldWork(int[] groupSizes, ulong expected)
		{
			ulong actual = new LvCombinationCountCalculator(groupSizes == null ? null : groupSizes.ToList(), groupSizes == null ? 0 : groupSizes.Sum()).GetLvCombinationCount();
			Assert.Equal(expected.ToString(), actual.ToString());
		}

		[Theory]
		[InlineData(new int[] { 0 })]
		[InlineData(new int[] { 1, 2, -1, 4 })]
		private static void GetLvCombinationCount_ShouldThrowGroupSizeException(int[] groupSizes)
		{
			Assert.Throws<GroupSizeException>(() => new LvCombinationCountCalculator(groupSizes.ToList(), groupSizes.Sum()).GetLvCombinationCount());
		}
	}
}
