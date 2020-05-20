using System;
using System.IO;
using System.Linq;
using UcenikShuffle.Common;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += (o, e) =>
			{
				Console.WriteLine((e.ExceptionObject as UnknownCommandException).Message);
				Environment.Exit(0);
			};

			var argParser = new Parameter(args);
			var parsedArgs = argParser.ParseArgs();

			//if user chose to print help
			if (parsedArgs is null)
			{
				Printer.PrintHelp();
				return;
			}

			var shuffler = new Shuffler((int)parsedArgs.LvCount);

			//Creating the groups
			foreach (var size in parsedArgs.GroupSizes)
			{
				shuffler.Groups.Add(new Group(size));
			}

			//Creating students
			//Students with have a label will be first in the list
			foreach (var label in parsedArgs.StudentLabels.OrderBy(l => l))
			{
				shuffler.Students.Add(new Student() { Label = label });
			}

			//Students which don't have a label will be last in the list
			for (int i = parsedArgs.StudentLabels.Count(); i < parsedArgs.GroupSizes.Sum(); i++)
			{
				shuffler.Students.Add(new Student());
			}

			//Saving the executed commands if the save command has been chosen
			if (parsedArgs.SaveFilePath != null)
			{
				File.WriteAllText(parsedArgs.SaveFilePath, argParser.ParametersToString());
			}

			shuffler.Shuffle();

			var printer = new Printer(shuffler);
			printer.PrintResult(argParser.ParseResults.DetailedOutput, argParser.ParseResults.StartDate, argParser.ParseResults.Frequency);
			
		}
	}
}