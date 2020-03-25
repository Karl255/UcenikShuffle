using System;
using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle
{
	class Program
	{
		static void Main()
		{
			int lvCount = 14;
			Group.CreateGroupsForLvs(lvCount);
			PrintResult();
		}

		static void PrintResult()
		{
			int lvCount = Group.History.Count / Group.Groups.Count;

			for (int i = 0; i < lvCount; i++)
			{
				Console.Write($"LV {i + 1}\t| ");
				int beginningJ = i * Group.Groups.Count;
				for (int j = beginningJ; j < beginningJ + Group.Groups.Count; j++)
				{
					foreach (var student in Group.History[j])
					{
						Console.Write($"{student.Id} ");
					}
					Console.Write("| ");
				}
				Console.WriteLine("\n");
			}

			for (int i = 0; i < Student.Students.Count; i++)
			{
				Console.WriteLine($"Ucenik { i + 1 }");
				Console.WriteLine("ID\tBrojSjedenja");

				var satWith = Student.Students[i].StudentSittingHistory.OrderBy(x => x.Key.Id);

				foreach (var otherStudent in satWith)
				{
					Console.WriteLine($"{ otherStudent.Key.Id + 1 }\t{ (int)otherStudent.Value }");
				}
				Console.WriteLine();
			}

			//Printing out groups which repeat
			Console.WriteLine("Repeating groups:");
			var groupHistory = Group.History.ToList();
			var repeatingGroups = (from _group in groupHistory
								   where Group.SearchHistory(_group).Count() != 1
								   select _group).ToList();
			while (repeatingGroups.Count > 0)
			{
				Console.Write("Group: ");
				PrintGroupHistoryRecord(repeatingGroups[0]);
				Console.Write("| Repeat count: ");
				int repeatCount = repeatingGroups.RemoveAll(g => g.Except(repeatingGroups[0]).Count() == 0);
				Console.WriteLine(repeatCount);
			}
		}

		/// <summary>
		/// This function prints out a history record
		/// </summary>
		/// <param name="record"></param>
		static void PrintGroupHistoryRecord(IEnumerable<Student> record)
		{
			record.ToList().ForEach(s => Console.Write($"{s.Id} "));
		}
	}
}