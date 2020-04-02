using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcenikShuffle.Common.Exceptions;
using UcenikShuffle.ConsoleApp;
using UcenikShuffle.ConsoleApp.Common;
using Xunit;
using static UcenikShuffle.ConsoleApp.Parameter;

namespace UcenikShuffle.UnitTests.ConsoleAppTests
{
	public class ParameterTests
	{
		[Theory]
		[InlineData("-s", Commands.Student)]
		[InlineData("-g", Commands.Group)]
		[InlineData("-G", Commands.Group)]
		[InlineData("-dO", Commands.DetailedOutput)]
		[InlineData("-do", Commands.DetailedOutput)]
		[InlineData("-save", Commands.Save)]
		[InlineData("-lOAd", Commands.Load)]
		public void ParameterToCommand_ShouldWork(string parameter, Commands expected)
		{
			var actual = Parameter.ParameterToCommand(parameter);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("-test")]
		[InlineData("-bla bla")]
		[InlineData("-g s")]
		[InlineData("dO")]
		public void ParameterToCommand_ShouldThrowUnknownCommandException(string parameter)
		{
			Assert.Throws<UnknownCommandException>(() => Parameter.ParameterToCommand(parameter));
		}

		[Theory]
		[InlineData(new string[] { "-g", "1", "2", "1", "-s", "Pero Peric", "Markic", "Barkic", "Bono Pls" }, new int[3] { 1, 1, 2 }, new string[4] { "Pero Peric", "Markic", "Barkic", "Bono Pls" })]
		public void Execute_ShouldWork(string[] args, int[] expectedGroupSizes, string[] expectedStudentLabels)
		{
			Parameter.Execute(args);
			Group.Groups = Group.Groups.OrderBy(g => g.Size).ToList();
			expectedGroupSizes = expectedGroupSizes.OrderBy(s => s).ToArray();
			
			//Checking if groups are correct
			for(int i = 0; i < expectedGroupSizes.Length; i++)
			{
				if(expectedGroupSizes[i] != Group.Groups[i].Size)
				{
					throw new Exception("Wrong size groups");
				}
			}

			//Checking if student labels are correct
			for(int i = 0; i < expectedStudentLabels.Length; i++)
			{
				if((from s in Student.Students
				   where expectedStudentLabels.Contains(s.Label)
				   select s).Count() != 1)
				{
					throw new Exception("Wrong student labels");
				}
			}
		}
	}
}