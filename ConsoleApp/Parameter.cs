using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.ConsoleApp
{
	public class Parameter
	{
		public enum Commands { Student, Group, DetailedOutput, Save, Load, StartDate, Frequency, LvCount, Help };
		public static Dictionary<string, Commands> CommandDictionary { get; private set; } = new Dictionary<string, Commands>
		{
			{ "/c",              Commands.LvCount },
			{ "/count",          Commands.LvCount },
			{ "/do",             Commands.DetailedOutput },
			{ "/detailedoutput", Commands.DetailedOutput },
			{ "/f",              Commands.Frequency },
			{ "/frequency",      Commands.Frequency },
			{ "/g",              Commands.Group },
			{ "/group",          Commands.Group },
			{ "/h",              Commands.Help },
			{ "/help",           Commands.Help },
			{ "/load",           Commands.Load },
			{ "/s",              Commands.Student },
			{ "/save",           Commands.Save },
			{ "/sd",             Commands.StartDate },
			{ "/startdate",      Commands.StartDate },
			{ "/student",        Commands.Student },
			{ "/?",              Commands.Help }
		};

		public static Dictionary<Commands, string> DescriptionDictionary { get; private set; } = new Dictionary<Commands, string>
		{
			{ Commands.DetailedOutput, "Prints out a detailed output of the result.\nExample: /g 1 2 /c 5 /do" },
			{ Commands.Frequency,      "Used for specifying frequency of the laboratory exercises (in days).\nExample: /g 1 2 /c 5 /sd 20.1.1990. /f 5" },
			{ Commands.Group,          "Used for specifying sizes of groups.\nExample: /g 1 2 3 /c 5" },
			{ Commands.Help,           "Prints out help.\nExample: /?" },
			{ Commands.Load,           "Loads commands from a file and executed them.\nExample: /load C:\\Users\\Korisnik\file.txt" },
			{ Commands.LvCount,        "Used for specifying the ammount of laboratory exercises.\nExample: /g 1 2 /c 10" },
			{ Commands.Save,           "Saves commands that are being executed to a specified file.\nExample: /g 1 2 /c 5 /save C:\\Users\\Korisnik\\file.txt" },
			{ Commands.StartDate,      "Used for specifying a start date.\nExample: /g 2 2 /c 5 /sd 11.4.2020. /f 1" },
			{ Commands.Student,        "Used for specifying student names/lables.\nExample: /g 3 /s \"A\" \"B\" \"C\" /c 5" }
		};

		private string[] Args;
		public ParseResult ParseResults = new ParseResult();

		public Parameter(string[] args)
		{
			Args = args;
		}

		private void ParseParameters()
		{
			string paramsNotSupportedMessage = "This command doesn't support parameters!";
			List<string> currentCommandParameters = new List<string>();
			Commands? currentCommand = null;
			int commandCount = 0;

			//Going trough every parameter
			for (int i = 0; i < Args.Length; i++)
			{
				//If the current parameter is a command
				if (Args[i][0] == '/')
				{
					commandCount++;
					currentCommand = ParameterToCommand(Args[i]);

					//If user chose to print help
					if (currentCommand == Commands.Help)
					{
						/* no, the app shouldn't do this, wtf dominik, just let it print help, and help only
						if (i != 0 || Args.Count() > 1)
						{
							throw new InvalidCommandUsageException(CommandToParameter((Commands)currentCommand), "This command should be the only parameter!");
						}
						*/

						ParseResults = null;
						return;
					}
				}
				//If the current parameter is a command parameter
				else
				{
					currentCommandParameters.Add(Args[i]);
				}

				//If the next parameter is a command or if this is the last parameter
				if (i == Args.Length - 1 || Args[i + 1][0] == '/')
				{
					if (currentCommand == null)
					{
						throw new UnknownCommandException(Args[0]);
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
							ParseResults.DetailedOutput = true;
							break;
						case Commands.Frequency:
							//Frequency has to have 1 parameter after it which specified how often the laboratory exercises are being held (in days)
							if (currentCommandParameters.Count != 1 || int.TryParse(currentCommandParameters[0], out int _frequency) == false)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command has to have 1 parameter which specifies how often laboratory exercises are held (in days)!");
							}
							ParseResults.Frequency = _frequency;
							break;
						case Commands.Group:
							//Leaving groupSizes parameter to be null if no parameters were entered after the command
							if (currentCommandParameters.Count != 0)
							{
								ParseResults.GroupSizes = new List<int>();
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

								ParseResults.GroupSizes.Add(groupSize);
							}
							break;
						case Commands.Load:
							//Load command must have 1 parameter after it which specifies file from which commands will be loaded
							if (currentCommandParameters.Count != 1)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies path to the file from which commands will be loaded!");
							}
							ParseResults.LoadFilePath = currentCommandParameters[0];
							break;
						case Commands.LvCount:
							//LvCount parameter must have 1 parameter after it which specifies how many laboratory exercises are to be done
							if (currentCommandParameters.Count != 1 || int.TryParse(currentCommandParameters[0], out int _lvCount) == false)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter after it which specifies how many laboratory exercises are to be done!");
							}
							ParseResults.LvCount = _lvCount;
							break;
						case Commands.Save:
							//Load command must have 1 parameter after it which specifies file in which commands will be saved
							if (currentCommandParameters.Count != 1)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies path to the file where commands will be saved!");
							}
							ParseResults.SaveFilePath = currentCommandParameters[0];
							break;
						case Commands.StartDate:
							//Start date command should have only 1 parameter after it which specifies what the starting date for the laboratory exercises is
							if (currentCommandParameters.Count != 1 || DateTime.TryParse(currentCommandParameters[0], out DateTime _startDate) == false)
							{
								throw new InvalidCommandUsageException(currentCommandString, "This command should have only 1 parameter which specifies what is the starting date for the laboratory exercises!");
							}
							ParseResults.StartDate = _startDate;
							break;
						case Commands.Student:
							ParseResults.StudentLabels.AddRange(currentCommandParameters);
							break;
					}
					currentCommandParameters.Clear();
				}
			}

			if (ParseResults.LoadFilePath == null)
			{
				if (ParseResults.LvCount == null)
				{
					throw new RequiredParameterNotUsedException(CommandToParameter(Commands.LvCount));
				}
				if (ParseResults.GroupSizes == null)
				{
					throw new RequiredParameterNotUsedException(CommandToParameter(Commands.Group));
				}
			}
			else if (commandCount != 1)
			{
				throw new InvalidCommandUsageException(CommandToParameter(Commands.Load), "This command should be the only command that is being executed (no other commands are allowed if this command is being used)!");
			}
		}

		private Commands ParameterToCommand(string command)
		{
			//If the command wasn't found
			if (command == null || CommandDictionary.TryGetValue(command.ToLower(), out Commands result) == false)
			{
				throw new UnknownCommandException(command);
			}

			//If the command was found
			return result;
		}

		private string CommandToParameter(Commands command)
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

		public ParseResult ParseArgs()
		{
			ParseParameters();

			//Loading the commands from the file, and reparsing the parameters
			if (ParseResults.LoadFilePath != null)
			{
				Args = File.ReadAllText(ParseResults.LoadFilePath).Split(' ');
				ParseParameters();
			}

			return ParseResults;
		}

		public string ParametersToString()
		{
			StringBuilder result = new StringBuilder();
			foreach (var a in Args)
			{
				result.Append($"\"{a}\" ");
			}
			return result.ToString();
		}
	}
}