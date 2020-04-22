using System;
using System.Collections.Generic;
using System.Text;
using UcenikShuffle.Common.Exceptions;
using System.Linq;
using UcenikShuffle.ConsoleApp.Common;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace UcenikShuffle.ConsoleApp
{
	public class Parameter
	{
		public enum Commands { Student, Group, DetailedOutput, Save, Load, StartDate, Frequency, LvCount, Help };
		public static Dictionary<string, Commands> CommandDictionary = new Dictionary<string, Commands>();
		public static Dictionary<Commands, string> DescriptionDictionary = new Dictionary<Commands, string>();

		static Parameter()
		{
			CommandDictionary.Add("/do", Commands.DetailedOutput);
			CommandDictionary.Add("/detailedoutput", Commands.DetailedOutput);
			DescriptionDictionary.Add(Commands.DetailedOutput, "Prints out a detailed output of the result.\nExample: /g 1 2 /c 5 /do");
			CommandDictionary.Add("/g", Commands.Group);
			CommandDictionary.Add("/group", Commands.Group);
			DescriptionDictionary.Add(Commands.Group, "Used for specifying sizes of groups.\nExample: /g 1 2 3 /c 5");
			CommandDictionary.Add("/load", Commands.Load);
			DescriptionDictionary.Add(Commands.Load, "Loads commands from a file and executed them.\nExample: /load C:\\Users\\Korisnik\file.txt");
			CommandDictionary.Add("/save", Commands.Save);
			DescriptionDictionary.Add(Commands.Save, "Saves commands that are being executed to a specified file.\nExample: /g 1 2 /c 5 /save C:\\Users\\Korisnik\\file.txt");
			CommandDictionary.Add("/s", Commands.Student);
			CommandDictionary.Add("/student", Commands.Student);
			DescriptionDictionary.Add(Commands.Student, "Used for specifying student names/lables.\nExample: /g 3 /s \"A\" \"B\" \"C\" /c 5");
			CommandDictionary.Add("/sd", Commands.StartDate);
			CommandDictionary.Add("/startdate", Commands.StartDate);
			DescriptionDictionary.Add(Commands.StartDate, "Used for specifying a start date.\nExample: /g 2 2 /c 5 /sd 11.4.2020. /f 1");
			CommandDictionary.Add("/f", Commands.Frequency);
			CommandDictionary.Add("/frequency", Commands.Frequency);
			DescriptionDictionary.Add(Commands.Frequency, "Used for specifying frequency of the laboratory exercises (in days).\nExample: /g 1 2 /c 5 /sd 20.1.1990. /f 5");
			CommandDictionary.Add("/c", Commands.LvCount);
			CommandDictionary.Add("/count", Commands.LvCount);
			DescriptionDictionary.Add(Commands.LvCount, "Used for specifying the ammount of laboratory exercises.\nExample: /g 1 2 /c 10");
			CommandDictionary.Add("/?", Commands.Help);
			CommandDictionary.Add("/h", Commands.Help);
			CommandDictionary.Add("/help", Commands.Help);
			DescriptionDictionary.Add(Commands.Help, "Prints out help.\nExample: /?");
		}

		public static ParseResult ParseParameters(string[] args)
		{
			ParseResult result = new ParseResult();
			string paramsNotSupportedMessage = "This command doesn't support parameters!";
			List<string> currentCommandParameters = new List<string>();
			Commands? currentCommand = null;
			int commandCount = 0;

			//Going trough every parameter
			for (int i = 0; i < args.Length; i++)
			{
				//If the current parameter is a command
				if (args[i][0] == '/')
				{
					commandCount++;
					currentCommand = ParameterToCommand(args[i]);

					//If user chose to print help
					if(currentCommand == Commands.Help)
					{
						if(i != 0 || args.Count() > 1)
						{
							throw new InvalidCommandUsageException(CommandToParameter((Commands)currentCommand), "This command should be the only parameter!");
						}
						Print.PrintHelp();
						return null;
					}
				}
				//If the current parameter is a command parameter
				else
				{
					currentCommandParameters.Add(args[i]);
				}

				//If the next parameter is a command or if this is the last parameter
				if (i == args.Length - 1 || args[i + 1][0] == '/')
				{
					if(currentCommand == null)
					{
						throw new UnknownCommandException(args[0]);
					}
					string currentCommandString = CommandToParameter((Commands)currentCommand);

					//Getting the command before this one and executing different operations based on what command it was
					switch (currentCommand)
					{
						case Commands.DetailedOutput:
							//Detailed output must not have any parameters after it
							if (currentCommandParameters.Count != 0)
							{
								throw new InvalidCommandUsageException(currentCommandString, paramsNotSupportedMessage);
							}
							result.DetailedOutput = true;
							break;
						case Commands.Frequency:
							//Frequency has to have 1 parameter after it which specified how often the laboratory exercises are being held (in days)
							if (currentCommandParameters.Count != 1 || int.TryParse(currentCommandParameters[0], out int _frequency) == false)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command has to have 1 parameter which specifies how often laboratory exercises are held (in days)!");
							}
							result.Frequency = _frequency;
							break;
						case Commands.Group:
							//Leaving groupSizes parameter to be null if no parameters were entered after the command
							if (currentCommandParameters.Count != 0)
							{
								result.GroupSizes = new List<int>();
							}
							foreach (var _parameter in currentCommandParameters)
							{
								//Converting the current parameter to group size (int)
								bool isInt = int.TryParse(_parameter, out int groupSize);

								//If current parameter isn't an integer or is lower than 1
								if (isInt == false || groupSize <= 0)
								{
									throw new InvalidCommandUsageException(currentCommandString, "All parameters for this command have to be positive integers!");
								}

								result.GroupSizes.Add(groupSize);
							}
							break;
						case Commands.Load:
							//Load command must have 1 parameter after it which specifies file from which commands will be loaded
							if (currentCommandParameters.Count != 1)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies path to the file from which commands will be loaded!");
							}
							result.LoadFilePath = currentCommandParameters[0];
							break;
						case Commands.LvCount:
							//LvCount parameter must have 1 parameter after it which specifies how many laboratory exercises are to be done
							if (currentCommandParameters.Count != 1 || int.TryParse(currentCommandParameters[0], out int _lvCount) == false)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter after it which specifies how many laboratory exercises are to be done!");
							}
							result.LvCount = _lvCount;
							break;
						case Commands.Save:
							//Load command must have 1 parameter after it which specifies file in which commands will be saved
							if (currentCommandParameters.Count != 1)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies path to the file where commands will be saved!");
							}
							result.SaveFilePath = currentCommandParameters[0];
							break;
						case Commands.StartDate:
							//Start date command should have only 1 parameter after it which specifies what the starting date for the laboratory exercises is
							if (currentCommandParameters.Count != 1 || DateTime.TryParse(currentCommandParameters[0], out DateTime _startDate) == false)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies what is the starting date for the laboratory exercises!");
							}
							result.StartDate = _startDate;
							break;
						case Commands.Student:
							result.StudentLabels.AddRange(currentCommandParameters);
							break;
					}
					currentCommandParameters.Clear();
				}
			}

			if (result.LoadFilePath == null)
			{
				if (result.LvCount == null)
				{
					throw new RequiredParameterNotUsedException(CommandToParameter(Commands.LvCount));
				}
				if (result.GroupSizes == null)
				{
					throw new RequiredParameterNotUsedException(CommandToParameter(Commands.Group));
				}
			}
			else if(commandCount != 1)
			{
				throw new InvalidCommandUsageException(CommandToParameter(Commands.Load), "This command should be the only command that is being executed (no other commands are allowed if this command is being used)!");
			}

			return result;
		}

		public static Commands ParameterToCommand(string command)
		{
			//If the command wasn't found
			if (command == null || CommandDictionary.TryGetValue(command.ToLower(), out Commands result) == false)
			{
				throw new UnknownCommandException(command);
			}

			//If the command was found
			return result;
		}

		public static string CommandToParameter(Commands command)
		{
			//Getting the first command parameter from the dictionary
			var result = (from _command in CommandDictionary
						  where _command.Value == command
						  select _command).FirstOrDefault();

			//If the command parameter wasn't found
			if (result.Key == null)
			{
				throw new NotImplementedException();
			}

			//If the command parameter was found
			return result.Key;
		}

		public static void Execute(ParseResult parseResult, string[] args)
		{
			//Loading the commands from the file, executing them and exiting the function if the load command has been used
			if(parseResult.LoadFilePath != null)
			{
				var text = File.ReadAllText(parseResult.LoadFilePath);
				Process cmd = new Process();
				cmd.StartInfo = new ProcessStartInfo() { Arguments = text, CreateNoWindow = false, FileName = Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase).Replace(".dll", ".exe") };
				cmd.Start();
				cmd.WaitForExit();
				return;
			}

			//Creating the groups
			foreach (var size in parseResult.GroupSizes)
			{
				Group.Groups.Add(new Group(size));
			}

			//Creating students
			//Students which have a label will be first in the list
			foreach (var label in parseResult.StudentLabels.OrderBy(l => l))
			{
				Student.Students.Add(new Student() { Label = label });
			}
			//Students which don't have a label will be last in the list
			for(int i = parseResult.StudentLabels.Count(); i < parseResult.GroupSizes.Sum(); i++)
			{
				Student.Students.Add(new Student());
			}

			//Calculating the result and printing it
			Group.CreateGroupsForLvs((int)parseResult.LvCount);
			Print.PrintResult(parseResult.DetailedOutput, parseResult.StartDate, parseResult.Frequency);

			//Saving the executed commands if the save command has been chosen
			if(parseResult.SaveFilePath != null)
			{
				File.WriteAllText(parseResult.SaveFilePath, ParametersToString(args));
			}
		}

		public static string ParametersToString(string[] args)
		{
			StringBuilder result = new StringBuilder(256);
			foreach (var a in args)
			{
				result.Append($"\"{a}\" ");
			}
			return result.ToString();
		}
	}
}