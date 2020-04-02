using System;
using System.Collections.Generic;
using System.Text;
using UcenikShuffle.Common.Exceptions;
using System.Linq;
using UcenikShuffle.ConsoleApp.Common;

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

		public static (IEnumerable<int> groupSizes, IEnumerable<string> studentLabels, bool detailedOutput, string saveFilePath, string loadFilePath, int lvCount, DateTime? startDate, int? frequency) ParseParameters(string[] args)
		{
			bool detailedOutput = false;
			int? lvCount = null;
			int? frequency = null;
			string saveFilePath = null;
			string loadFilePath = null;
			string paramsNotSupportedMessage = "This command doesn't support parameters!";
			List<int> groupSizes = null;
			List<string> studentLabels = new List<string>();
			List<string> currentCommandParameters = new List<string>();
			DateTime? startDate = null;
			Commands? currentCommand = null;

			//Going trough every parameter
			foreach (var parameter in args)
			{
				//If this parameter is a command
				if (parameter[0] == '-')
				{
					//If this is not the first command
					if (currentCommand != null)
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
								detailedOutput = true;
								break;
							case Commands.Frequency:
								//Frequency has to have 1 parameter after it which specified how often the laboratory exercises are being held (in days)
								if(currentCommandParameters.Count != 0 || int.TryParse(currentCommandParameters[0], out int _frequency))
								{
									throw new InvalidCommandUsageException(currentCommandString, "This command has to have 1 parameter which specifies how often laboratory exercises are held (in days)!");
								}
								frequency = _frequency;
								break;
							case Commands.Group:
								//Leaving groupSizes parameter to be null if no parameters were entered after the command
								if (currentCommandParameters.Count != 0)
								{
									groupSizes = new List<int>();
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

									groupSizes.Add(groupSize);
								}
								break;
							case Commands.Load:
								//Load command must have 1 parameter after it which specifies file from which commands will be loaded
								if (currentCommandParameters.Count != 1)
								{
									throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies path to the file from which commands will be loaded!");
								}
								loadFilePath = currentCommandParameters[0];
								break;
							case Commands.LvCount:
								//LvCount parameter must have 1 parameter after it which specifies how many laboratory exercises are to be done
								if(currentCommandParameters.Count != 1 || int.TryParse(currentCommandString, out int _lvCount))
								{
									throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter after it which specifies how many laboratory exercises are to be done!");
								}
								lvCount = _lvCount;
								break;
							case Commands.Save:
								//Load command must have 1 parameter after it which specifies file in which commands will be saved
								if (currentCommandParameters.Count != 1)
								{
									throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies path to the file where commands will be saved!");
								}
								saveFilePath = currentCommandParameters[0];
								break;
							case Commands.StartDate:
								//Start date command should have only 1 parameter after it which specifies what the starting date for the laboratory exercises is
								if(currentCommandParameters.Count != 1 || DateTime.TryParse(currentCommandParameters[0], out DateTime _startDate) == false)
								{
									throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies what is the starting date for the laboratory exercises!");
								}
								startDate = _startDate;
								break;
							case Commands.Student:
								studentLabels.AddRange(currentCommandParameters);
								break;
						}
					}
					currentCommandParameters.Clear();
					currentCommand = ParameterToCommand(parameter);
				}
				else
				{
					currentCommandParameters.Add(parameter);
				}
			}

			if (lvCount == null)
			{
				throw new RequiredParameterNotUsedException(CommandToParameter(Commands.LvCount));
			}
			if (frequency == null)
			{
				throw new RequiredParameterNotUsedException(CommandToParameter(Commands.Frequency));
			}
			if (groupSizes == null)
			{
				throw new RequiredParameterNotUsedException(CommandToParameter(Commands.Group));
			}

			return (groupSizes, studentLabels, detailedOutput, saveFilePath, loadFilePath, (int)lvCount, startDate, frequency);
		}

		public static Commands ParameterToCommand(string command)
		{
			//If the command wasn't found
			if(command == null || commandDictionary.TryGetValue(command.ToLower(), out Commands result) == false)
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
			if(result.Key == null)
			{
				throw new NotImplementedException();
			}

			//If the command parameter was found
			return result.Key;
		}

		public static void Execute(string[] args)
		{
			var parseResults = ParseParameters(args);
			foreach (var size in parseResults.groupSizes)
			{
				Group.Groups.Add(new Group(size));
			}
			foreach(var label in parseResults.studentLabels)
			{
				Student.Students.Add(new Student() { Label = label });
			}

			Group.CreateGroupsForLvs(parseResults.lvCount);
			Print.PrintResult();
		}
	}
}