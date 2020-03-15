using System;
using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle
{
	class Program
	{
		static void Main()
		{
			int[] studentIds = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
			var students = new List<Student>();
			var allLVs = new List<List<int>>();
			var groups = new List<Group>
			{
				new Group(1),
				new Group(3),
				new Group(3),
				new Group(3),
				new Group(3)
			};


			foreach (var studentId in studentIds)
			{
				students.Add(new Student(studentId, studentIds));
			}

			for (int lv = 0; lv < 14; lv++)
			{
				allLVs.Add(new List<int>());
				var studentPool = new List<Student>(students);

				groups[0].AddStudent(studentPool.Pop(lv % 13));
				allLVs[lv].Add(groups[0].History[lv]);

				for (int i = 1; i <= 4; i++)
				{
					groups[i].AddStudents(studentPool);
					allLVs[lv].Add(groups[i].History[lv]);
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

			for (int i = 0; i < 14; i++)
			{
				Console.Write("{0,3} |        ", i + 1);
				for (int j = 0; j < allLVs[i].Count; j++)
				{
					if ((j + 2) % 3 == 0)
					{
						Console.Write(" |");
					}
					Console.Write("{0,4}", allLVs[i][j]);
				}
				Console.WriteLine();
			}
		}
	}
}
