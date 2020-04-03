using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
		const string testFilePath = @"C:\Users\Korisnik\Desktop\le clerc o beat.txt";

		[Theory]
		[InlineData("-s", Commands.Student)]
		[InlineData("-g", Commands.Group)]
		[InlineData("-G", Commands.Group)]
		[InlineData("-dO", Commands.DetailedOutput)]
		[InlineData("-do", Commands.DetailedOutput)]
		[InlineData("-save", Commands.Save)]
		[InlineData("-lOAd", Commands.Load)]
		[InlineData("-f", Commands.Frequency)]
		[InlineData("-sd", Commands.StartDate)]
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

		///IMPORTANT: SHOULD BE CHANGED IF <see cref="Execute_ShouldWork(string[], int)"/> FUNCTION IS BEING CHANGED!!!
		List<ParseResult> execute_results = new List<ParseResult>()
		{
			new ParseResult(){ DetailedOutput = false, Frequency = 7, GroupSizes = new List<int>(){ 1,1,2 }, LoadFilePath = null, LvCount = 14, SaveFilePath = null, StartDate = new DateTime(2002, 11, 27), StudentLabels = new List<string>(){ "Pero Peric", "Markic", "Barkic", "Bono Pls" } },
			new ParseResult() { DetailedOutput = true, Frequency = 10, GroupSizes  = new List<int>(){ 3, 5 }, LoadFilePath = null, LvCount = 3, SaveFilePath = @"C:\Users\Korisnik\Desktop\le clerc o beat.txt", StartDate = new DateTime(1900, 1, 1), StudentLabels = new List<string>(){ "DD" } },
			new ParseResult() { LvCount = 3, GroupSizes = new List<int>(){ 1 } },
			new ParseResult(){ LoadFilePath = testFilePath }
		};
		[Theory]
		[InlineData(new string[] { "-g", "1", "2", "1", "-s", "Pero Peric", "Markic", "Barkic", "Bono Pls", "-c", "14", "-sd", "27/11/2002", "-f", "7" }, 0)]
		[InlineData(new string[] { "-do", "-f", "10", "-g", "3", "5", "-c", "3", "-save", testFilePath, "-sd", "1900-1-1", "-s", "DD" }, 1)]
		[InlineData(new string[] { "-c", "3", "-g", "1" }, 2)]
		[InlineData(new string[] { "-load", testFilePath }, 3)]
		public void Execute_ShouldWork(string[] args, int resultID)
		{
			var parseResult = execute_results[resultID];

			//Writing test commands to file which from which the commands will be loaded
			if (parseResult.LoadFilePath != null)
			{
				File.WriteAllText(parseResult.LoadFilePath, "-g 1 -c 5");
			}

			Parameter.Execute(ParseParameters(args), args);

			//Exiting the function if commands have been loaded from a file (this is done because results can't be checked - program is executed as a separate program so all the data gets saved in that program and not this one (all data including users, groups etc. will be empty for this program))
			if(parseResult.LoadFilePath != null)
			{
				File.Delete(parseResult.LoadFilePath);
				return;
			}

			Group.Groups = Group.Groups.OrderBy(g => g.Size).ToList();
			parseResult.GroupSizes = parseResult.GroupSizes?.OrderBy(s => s).ToList();
			parseResult.StudentLabels = parseResult.StudentLabels?.OrderBy(l => l).ToList();

			//Checking if groups are correct
			for (int i = 0; i < parseResult.GroupSizes.Count; i++)
			{
				if (parseResult.GroupSizes[i] != Group.Groups[i].Size)
				{
					throw new Exception("Wrong size groups");
				}
			}

			//Checking if student labels and ID's are correct
			for (int i = 0; i < parseResult.StudentLabels.Count; i++)
			{
				if (Student.Students[i].Label.CompareTo(parseResult.StudentLabels[i]) != 0)
				{
					throw new Exception("Wrong student label");
				}
				if (Student.Students[i].Id != i + 1)
				{
					throw new Exception("Wrong student ID");
				}
			}

			//Checking is the file to which the commands should be saved to exists
			if (parseResult.SaveFilePath != null)
			{
				if (File.Exists(parseResult.SaveFilePath) == false)
				{
					throw new Exception("Commands weren't saved!");
				}
				File.Delete(parseResult.SaveFilePath);
			}

			//Checking if the file from which the commands were supposed to be loaded from exists
			if (parseResult.LoadFilePath != null)
			{
				if (File.Exists(parseResult.LoadFilePath) == false)
				{
					throw new Exception("File from which commands should be loaded wasn't found!");
				}
				File.Delete(parseResult.LoadFilePath);
			}

			//Checking if the lvCount parameter is correct
			if (Group.History.Count % Group.Groups.Count != 0)
			{
				throw new Exception("For some reason the group history record count isn't a multiple of the number of groups...");
			}
			Assert.Equal(parseResult.LvCount, Group.History.Count / Group.Groups.Count);

			//Checking if the output is correct
			Process process = new Process();
			var executableLocation = AppDomain.CurrentDomain.BaseDirectory;
			for (int i = 0; i < 5; i++)
			{
				executableLocation = Directory.GetParent(executableLocation).FullName;
			}
			executableLocation = Path.Combine(executableLocation, "ConsoleApp", "bin", "Debug", "netcoreapp3.1", "ConsoleApp.exe");
			process.StartInfo = new ProcessStartInfo() { FileName = executableLocation, Arguments = ParametersToString(args), CreateNoWindow = false, RedirectStandardOutput = true };
			process.Start();
			process.WaitForExit();
			var outputText = process.StandardOutput.ReadToEnd();
			if (parseResult.StartDate != null && parseResult.Frequency != null)
			{
				for (int i = 0; i < parseResult.LvCount; i++)
				{
					var currentDate = ((DateTime)parseResult.StartDate).AddDays((int)parseResult.Frequency * i).ToString("dd.MM.yyyy.");
					if (outputText.Contains(currentDate) == false)
					{
						throw new Exception("There is something wrong about date/frequency! It could also be that the executable file for the console app wasn't found (check if the executable exsists at the path which is being searched).");
					}
				}
			}
		}

		///IMPORTANT: SHOULD BE CHANGED IF <see cref="ParseParameters_ShouldWork(string)/> FUNCTION IS BEING CHANGED!!!
		List<ParseResult> parseParameters_results = new List<ParseResult>()
		{
			new ParseResult(){ GroupSizes = new List<int>(){ 1,2,1 }, StudentLabels = new List<string>(){ "A", "B", "C", "D" }, LvCount = 100 },
			new ParseResult(){ }
		};
		[Theory]
		[InlineData("-g 1 2 1 -s A B C D -c 100")]
		public void ParseParameters_ShouldWork(string commands)
		{

		}
	}
}