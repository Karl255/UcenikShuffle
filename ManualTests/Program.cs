using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UcenikShuffle.Common;

namespace ManualTests
{
	class Program
	{
		static void Main(string[] args)
		{
			
			//IMPORTANT: this program is used as help for programmers. It prints out student sitting combinations
			//and statistics which are useful in diagnosing how well the shuffling algorithm works.
			//To add a combination for which lv combinations will be calculated, add groups sizes in groupSizesList variable
			//and add number of lv's to combinationCountList (IMPORTANT: groupSizesList and combinationCountList should have the same amount of elements).
			//NOTE: this doesn't mean that this program is used for testing. UnitTests project is where the tests are, this is used just for quick testing
			//and statistics printing since unit tests won't print out statistics

			var groupSizesList = new List<List<int>>()
			{
				new List<int>(){ 1,3,3,3,3 },
				//new List<int>(){ 1,3,3,3,3 }
			};
			var combinationCountList = new List<int>()
			{
				13,
				//50
			};

			var watch = new Stopwatch();
			for (int i = 0; i < groupSizesList.Count; i++)
			{
				var groupSizes = groupSizesList[i];
				var students = new List<Student>();
				for (int j = 1; j <= groupSizes.Sum(); j++)
				{
					students.Add(new Student(j));
				}
				Console.WriteLine($"Combination count: {new LvCombinationCountCalculator(groupSizes, students.Count).GetLvCombinationCount()}");
				Console.WriteLine("Shuffling...");
				watch.Restart();
				var shuffler = new Shuffler(combinationCountList[i], groupSizes, null);
				shuffler.Shuffle();
				watch.Stop();
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
				if (args.Select(a => a.ToLowerInvariant()).Contains("y"))
				{
					foreach (var student in students)
					{
						Console.WriteLine(student.Label);
						foreach (var student2 in students.Except(new List<Student>() { student }))
						{
							Console.WriteLine($"{student2.Label} \t {shuffler.ShuffleResult.Count(r => r.Combination.Any(g => g.Any(s => s.Id == student.Id) && g.Any(s => s.Id == student2.Id)))}");
						}
						Console.WriteLine();
					}
				}
				Console.WriteLine($"ELAPSED SECONDS: {watch.ElapsedMilliseconds / (double)1000}\n\n\n");

				if(shuffler.ShuffleResult.Any(c1 => shuffler.ShuffleResult.Count(c2 => c1.CompareTo(c2)) != 1))
				{
					throw new Exception();
				}
			}
		}
	}
}
