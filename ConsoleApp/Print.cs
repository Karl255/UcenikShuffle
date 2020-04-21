using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcenikShuffle.ConsoleApp.Common;

namespace UcenikShuffle.ConsoleApp
{
	class Print
	{
		public static void PrintResult(bool detailedOutput, DateTime? startDate, int? frequency)
		{
			int lvCount = Group.History.Count / Group.Groups.Count;
			var maxLabelLength = (from s in Student.Students
								  select s.Label.Length).Max();
			var maxGroupSize = (from g in Group.Groups
								select g.Size).Max();

			//table header
			Console.Write("  LV  ");
			for (int i = 0; i < Group.Groups.Count; i++)
			{
				var groupLabel = "| Group " + (i + 1).ToString();
				PrintMessage(groupLabel, (maxLabelLength + 1) * Group.Groups[i].Size + 3);
			}
			Console.WriteLine();

			//line between header and content
			Console.Write("──────");
			for (int i = 0; i < Group.Groups.Count; i++)
			{
				PrintMessage("┼", (maxLabelLength + 1) * Group.Groups[i].Size + 3, '─');
			}
			Console.WriteLine();

			//table content
			for (int i = 0; i < lvCount; i++)
			{
				string dateString = null;
				if (startDate != null)
				{
					dateString = ((DateTime)startDate).AddDays(frequency == null ? 0 : (int)frequency * i).ToString("dd.MM.yyyy.");
				}
				Console.Write($"{i + 1,4} {dateString} │ ");
				int beginningJ = i * Group.Groups.Count;

				for (int j = beginningJ; j < beginningJ + Group.Groups.Count; j++)
				{
					if (j != beginningJ)
					{
						Console.Write(" │ ");
					}
					foreach (var student in Group.History[j])
					{
						PrintMessage(student.Label, maxLabelLength + 1);
					}
				}
				Console.WriteLine();
			}

			///No additional info will be printed if <param name="detailedOutput"/> is set to false
			if(detailedOutput == false)
			{
				return;
			}

			//Printing out how many times students sat with each other
			Console.WriteLine();
			for (int i = 0; i < Student.Students.Count; i++)
			{
				Console.WriteLine($"Student { i + 1 }");
				PrintMessage("Label", maxLabelLength);
				Console.WriteLine(" Sat with ammount");

				var satWith = Student.Students[i].StudentSittingHistory.OrderBy(x => x.Key.Id);

				foreach (var otherStudent in satWith)
				{
					PrintMessage(otherStudent.Key.Label, maxLabelLength);
					Console.WriteLine($" { (int)otherStudent.Value }");
				}
				Console.WriteLine();
			}

			//Printing out groups which repeat
			Console.WriteLine("Repeating groups:");
			var groupHistory = Group.History.ToList();
			var repeatingGroups = (from _group in groupHistory
								   where Group.SearchGroupHistory(_group).Count() != 1
								   select _group).ToList();
			var repeatingGroupsDictionary = new Dictionary<HashSet<Student>, int>();
			while (repeatingGroups.Count > 0)
			{
				int repeatCount = repeatingGroups.Count;
				var currentGroup = repeatingGroups[0];
				repeatingGroups.RemoveAll(g => Group.CompareGroupHistoryRecords(g, currentGroup));
				repeatCount -= repeatingGroups.Count;
				repeatingGroupsDictionary.Add(currentGroup, repeatCount);
			}
			repeatingGroupsDictionary = new Dictionary<HashSet<Student>, int>(repeatingGroupsDictionary.OrderByDescending(g => g.Value));
			foreach(var repeatingGroup in repeatingGroupsDictionary)
			{
				StringBuilder message = new StringBuilder(128);
				var lastStudent = repeatingGroup.Key.Last();
				repeatingGroup.Key.Remove(lastStudent);
				repeatingGroup.Key.ToList().ForEach(s => message.Append($"{s.Label}-"));
				repeatingGroup.Key.Add(lastStudent);
				message.Append(lastStudent.Label);
				PrintMessage(message.ToString(), (maxLabelLength + 1) * maxGroupSize, ' ');
				Console.Write("| Repeat count: ");
				Console.WriteLine(repeatingGroup.Value);
			}
		}

		/// <summary>
		/// This function prints out the wanted message to the console screen 
		/// </summary>
		/// <param name="message">Message to be printed</param>
		/// <param name="minSize">Minimum message size</param>
		/// <param name="fillCharacter">Character that will be printed until the message is long enough</param>
		static void PrintMessage(string message, int minSize, char fillCharacter = ' ')
		{
			Console.Write(message);
			for(int i = message.Length; i < minSize; i++)
			{
				Console.Write(fillCharacter);
			}
		}

		/// <summary>
		/// Prints out help for all program arguments
		/// </summary>
		public static void PrintHelp()
		{
			var commandGroups = (from command in Parameter.CommandDictionary
			 group command by command.Value into g
							select g);
			StringBuilder commandsString = new StringBuilder(128);
			foreach (var commands in commandGroups)
			{
				commandsString.Clear();
				foreach(var command in commands)
				{
					commandsString.Append(command.Key);
					commandsString.Append(", ");
				}
				commandsString.Remove(commandsString.Length - 2, 2);
				Console.WriteLine(commandsString.ToString());
				Console.WriteLine((from description in Parameter.DescriptionDictionary
								   where description.Key == commands.First().Value
								   select description.Value).First());
				Console.WriteLine();
			}
		}
	}
}