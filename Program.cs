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
			int[] studentIds = Enumerable.Range(0, 13).ToArray();
			var students = new List<Student>();
			var allLVs = new List<List<int>>();

			//Adding students to students list
			foreach (var studentId in studentIds)
			{
				students.Add(new Student(studentId, studentIds, Group.Groups));
			}

			//Going trough each laboratory exercise (lv)
			for (int lv = 0; lv < lvCount; lv++)
			{
				var studentPool = new List<Student>(students);
				for (int i = 0; i < Group.Groups.Count; i++)
				{
					Group.Groups[i].AddStudents(ref studentPool);
				}
			}

			Console.Write(" LV ");

			for (int i = 1; i <= 5; i++)
			{
				Console.Write("|{0,12} ", $"Grupa { i }");
			}
			Console.WriteLine();

			Console.Write("---");
			for (int i = 0; i < 5; i++)
			{
				Console.Write("-+------------");
			}
			Console.WriteLine();

			for (int i = 0; i < lvCount; i++)
			{
				for(int j = 0; j < Group.Groups.Count; j++)
				{
					foreach(var student in Group.History[i * Group.Groups.Count + j])
					{
						Console.Write($"{student} ");
					}
					Console.Write(" | ");
				}
				Console.WriteLine();
			}

			for (int i = 0; i < students.Count; i++)
			{
				Console.WriteLine($"Ucenik { i + 1 }");

				var satWith = students[i].StudentSittingHistory.OrderBy(x => x.Key);

				foreach (var otherStudent in satWith)
				{
					Console.WriteLine($"{ otherStudent.Key + 1 } { otherStudent.Value }");
				}
				Console.WriteLine();
			}
		}
	}
}
