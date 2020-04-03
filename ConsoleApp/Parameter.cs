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
		public enum Commands { Student, Group, DetailedOutput, Save, Load, StartDate, Frequency, LvCount };
		static Dictionary<string, Commands> commandDictionary = new Dictionary<string, Commands>();

		static Parameter()
		{
			commandDictionary.Add("-do", Commands.DetailedOutput);
			commandDictionary.Add("-g", Commands.Group);
			commandDictionary.Add("-load", Commands.Load);
			commandDictionary.Add("-save", Commands.Save);
			commandDictionary.Add("-s", Commands.Student);
			commandDictionary.Add("-sd", Commands.StartDate);
			commandDictionary.Add("-f", Commands.Frequency);
			commandDictionary.Add("-c", Commands.LvCount);
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
				if (args[i][0] == '-')
				{
					commandCount++;
					currentCommand = ParameterToCommand(args[i]);
				}
				//If the current parameter is a command parameter
				else
				{
					currentCommandParameters.Add(args[i]);
				}

				//If the next parameter is a command or if this is the last parameter
				if (i == args.Length - 1 || args[i + 1][0] == '-')
				{
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
			if (command == null || commandDictionary.TryGetValue(command.ToLower(), out Commands result) == false)
			{
				throw new UnknownCommandException(command);
			}

			//If the command was found
			return result;
		}

		public static string CommandToParameter(Commands command)
		{
			//Getting the first command parameter from the dictionary
			var result = (from _command in commandDictionary
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

		public static void Execute(string[] args)
		{
			var parseResults = ParseParameters(args);

			//Loading the commands from the file, executing them and exiting the function if the load command has been used
			if(parseResults.LoadFilePath != null)
			{
				var text = File.ReadAllText(parseResults.LoadFilePath);
				Process cmd = new Process();
				cmd.StartInfo = new ProcessStartInfo() { Arguments = text, CreateNoWindow = false, FileName = Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase).Replace(".dll", ".exe") };
				cmd.Start();
				cmd.WaitForExit();
				return;
			}

			//Creating the groups
			foreach (var size in parseResults.GroupSizes)
			{
				Group.Groups.Add(new Group(size));
			}

			//Creating students
			//Students which have a label will be first in the list
			foreach (var label in parseResults.StudentLabels.OrderBy(l => l))
			{
				Student.Students.Add(new Student() { Label = label });
			}
			//Students which don't have a label will be last in the list
			for(int i = parseResults.StudentLabels.Count(); i < parseResults.GroupSizes.Sum(); i++)
			{
				Student.Students.Add(new Student());
			}

			//Calculating the result and printing it
			Group.CreateGroupsForLvs((int)parseResults.LvCount);
			Print.PrintResult(parseResults.DetailedOutput, parseResults.StartDate, parseResults.Frequency);

			//Saving the executed commands if the save command has been chosen
			if(parseResults.SaveFilePath != null)
			{
				StringBuilder builder = new StringBuilder(256);
				foreach(var arg in args)
				{
					builder.Append($"\"{arg}\" ");
				}
				File.WriteAllText(parseResults.SaveFilePath, builder.ToString());
			}
		}
	}
}