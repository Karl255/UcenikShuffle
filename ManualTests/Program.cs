using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UcenikShuffle.Common;

namespace ManualTests
{
	class Program
	{
		static void Main()
		{
			var groupSizesList = new List<List<int>>()
			{
				new List<int>() { 1, 2, 2 },
				new List<int>(){ 1,3,3,3,3 },
				new List<int>(){ 1,3,3,3,3 }
			};
			var combinationCountList = new List<int>()
			{
				10,
				14,
				100
			};

			var watch = new Stopwatch();
			for (int i = 0; i < groupSizesList.Count; i++)
			{
				watch.Start();
				var groupSizes = groupSizesList[i];
				var students = new List<Student>();
				for (int j = 1; j <= groupSizes.Sum(); j++)
				{
					students.Add(new Student(j));
				}
				var shuffler = new Shuffler(combinationCountList[i], groupSizes, null);
				shuffler.Shuffle();
				foreach (var combination in shuffler.ShuffleResult)
				{
					foreach (var group in combination.Combination)
					{
						foreach (var student in group)
						{
							Console.Write($"{student.Id},");
						}
						Console.Write("\t\t");
					}
					Console.WriteLine();
				}
				Console.WriteLine();
				foreach (var student in students)
				{
					Console.WriteLine(student.Label);
					foreach (var student2 in students.Except(new List<Student>() { student }))
					{
						Console.WriteLine($"{student2.Label} \t {shuffler.ShuffleResult.Where(r => r.Combination.Any(g => g.Any(s => s.Id == student.Id) && g.Any(s => s.Id == student2.Id))).Count()}");
					}
					Console.WriteLine();
				}
				watch.Stop();
				Console.WriteLine($"ELAPSED SECONDS: {watch.ElapsedMilliseconds / (double)1000}\n\n\n");
			}
		}
	}
}
