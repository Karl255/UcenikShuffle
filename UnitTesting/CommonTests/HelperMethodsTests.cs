using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using UcenikShuffle.Common;
using UcenikShuffle.Common.Exceptions;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public class HelperMethodsTests
	{
		public static IEnumerable<object[]> Data = new List<object[]>()
		{
			//Normal tests
			new object[]{ 2,2, new int[][] { new[]{0,1} } },
			new object[]{ 2,3, new int[][] { new[]{0,1}, new[]{0,2}, new[]{1,2} } },
			new object[]{ 1,1, new int[][] { new[]{0} } },
			//Unexpected data tests
			//(higher group size than number count)
			new object[]{ 5,2, new int[][] { new[]{0,1} } }
		};
		
		[Theory]
		[MemberData(nameof(Data))]
		private static void GetAllNumberCombinations_ShouldWork(int groupSize, int numberCount, IEnumerable<IEnumerable<int>> expected)
		{
			var actual = HelperMethods.GetAllNumberCombinations(groupSize, numberCount).ToList();
			expected = expected.ToList();
			Assert.Equal(expected.Count(), actual.Count());
            
			//Checking if the lists match
			foreach (var combination in expected)
			{
				var throwException = 
					(from c in actual
						where combination.Except(c).Any() == false && c.Count() == combination.Count()
						select c).FirstOrDefault() == null;
				if (throwException)
				{
					throw new Exception();
				}
			}
		}

		[Theory]
		[InlineData(1, 0)]
		[InlineData(1, -1)]
		private static void GetAllNumberCombinations_ShouldThrowArgumentException(int groupSize, int numberCount)
		{
			Assert.Throws<ArgumentException>(() => HelperMethods.GetAllNumberCombinations(groupSize, numberCount).ToList());
		}

		[Theory]
		[InlineData(0, 1)]
		[InlineData(-1, 1)]
		private static void GetAllNumberCombinations_ShouldThrowGroupSizeParameterException(int groupSize, int numberCount)
		{
			Assert.Throws<GroupSizeParameterException>(() => HelperMethods.GetAllNumberCombinations(groupSize, numberCount).ToList());
		}
	}
}