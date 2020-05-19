using System;
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
			var shuffler = argParser.ParseArgs();

			//if user chose to print help
			if (shuffler is null)
			{
				Printer.PrintHelp();
				return;
			}

			shuffler.Shuffle();

			var printer = new Printer(shuffler);
			printer.PrintResult(argParser.ParseResults.DetailedOutput, argParser.ParseResults.StartDate, argParser.ParseResults.Frequency);


		}
	}
}