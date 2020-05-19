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
			var parseResult = Parameter.ParseParameters(args);
			if (parseResult == null)
			{
				return;
			}
			Parameter.Execute(parseResult, args);
		}
	}
}