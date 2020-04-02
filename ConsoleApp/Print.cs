using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcenikShuffle.ConsoleApp.Common;

namespace UcenikShuffle.ConsoleApp
{
	class Print
	{
		public static void PrintResult()
		{
			int lvCount = Group.History.Count / Group.Groups.Count;

			//table header
			Console.Write("  LV");
			for (int i = 0; i < Group.Groups.Count; i++)
			{
				Console.Write(" │{0,12}", $"Group {i + 1}");
			}
			Console.WriteLine();

			//line between header and content
			Console.Write("────");
			for (int i = 0; i < Group.Groups.Count; i++)
			{
				Console.Write("─┼────────────");
			}
			Console.WriteLine();

			//table content
			for (int i = 0; i < lvCount; i++)
			{
				Console.Write($"{i + 1,4} │        ");
				int beginningJ = i * Group.Groups.Count;

				for (int j = beginningJ; j < beginningJ + Group.Groups.Count; j++)
				{
					if (j != beginningJ)
					{
						Console.Write(" │");
					}
					foreach (var student in Group.History[j])
					{
						Console.Write($"{student.Id,4}");
					}
				}
				Console.WriteLine();
			}

			for (int i = 0; i < Student.Students.Count; i++)
			{
				Console.WriteLine($"Student { i + 1 }");
				Console.WriteLine("ID\tSat with ammount");

				var satWith = Student.Students[i].StudentSittingHistory.OrderBy(x => x.Key.Id);

				foreach (var otherStudent in satWith)
				{
					Console.WriteLine($"{ otherStudent.Key.Id }\t{ (int)otherStudent.Value }");
				}
				Console.WriteLine();
			}

			//Printing out groups which repeat
			Console.WriteLine("Repeating groups:");
			var groupHistory = Group.History.ToList();
			var repeatingGroups = (from _group in groupHistory
								   where Group.SearchGroupHistory(_group).Count() != 1
								   select _group).ToList();
			while (repeatingGroups.Count > 0)
			{
				Console.Write("Group: ");
				PrintGroupHistoryRecord(repeatingGroups[0]);
				Console.Write("| Repeat count: ");
				int repeatCount = repeatingGroups.Count;
				var currentGroup = repeatingGroups[0];
				repeatingGroups.RemoveAll(g => Group.CompareGroupHistoryRecords(g, currentGroup));
				repeatCount -= repeatingGroups.Count;
				Console.WriteLine(repeatCount);
			}
		}

		/// <summary>
		/// This function prints out a history record
		/// </summary>
		/// <param name="record"></param>
		public static void PrintGroupHistoryRecord(IEnumerable<Student> record)
		{
			record.ToList().ForEach(s => Console.Write($"{s.Id} "));
		}
	}
}