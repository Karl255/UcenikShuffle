using System;
using System.Linq;
using UcenikShuffle.Common;
using UcenikShuffle.Common.Exceptions;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
	public class ParserTests
	{
		[Theory]
		[InlineData("100",100)]
		[InlineData("1",1)]
		public void StringToLvCount_ShouldWork(string value, int expected)
		{
			int actual = Parsers.StringToLvCount(value);
			Assert.Equal(expected, actual);
		}
		
		[Theory]
		//<=0 lvCount
		[InlineData("-10")]
		[InlineData("0")]
		//Non integer
		[InlineData("test")]
		//Empty values
		[InlineData(null)]
		[InlineData("")]
		public void StringToLvCount_ShouldThrowArgumentException(string value)
		{
			Assert.Throws<ArgumentException>(() => Parsers.StringToLvCount(value));
		}

		[Theory]
		//Spaces
		[InlineData("1, 2, 3", new[]{1,2,3})]
		//Multiple delimiters in a row
		[InlineData("5,20,,1", new[]{5,20,1})]
		//Single group size
		[InlineData("1", new[]{1})]
		public void StringToGroupSizes_ShouldWork(string value, int[] expected)
		{
			expected = expected.OrderBy(s => s).ToArray();
			var actual = Parsers.StringToGroupSizes(value).OrderBy(s => s).ToArray();
			Assert.Equal(expected.Length, actual.Length);
			for (int i = 0; i < expected.Count(); i++)
			{
				Assert.Equal(expected, actual);
			}
		}

		[Theory]
		//Empty values
		[InlineData(null)]
		[InlineData("")]
		//<=0 group size
		[InlineData("0")]
		[InlineData("-3")]
		//Invalid delimiter
		[InlineData("1;;4")]
		//Decimal group size
		[InlineData("1.5,4")]
		public void StringToGroupSizes_ShouldThrowGroupSizeException(string value)
		{
			Assert.Throws<GroupSizeException>(() => Parsers.StringToGroupSizes(value).ToList());
		}
	}
}