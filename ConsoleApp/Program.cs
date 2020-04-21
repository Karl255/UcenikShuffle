using System;
using System.Collections.Generic;
using System.Linq;
using UcenikShuffle.Common.Exceptions;
using UcenikShuffle.ConsoleApp.Common;

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
			Parameter.Execute(Parameter.ParseParameters(args), args);
		}
	}
}