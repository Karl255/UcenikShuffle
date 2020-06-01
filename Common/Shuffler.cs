using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public class Shuffler
	{
		public int LvCount { get; private set; }
		public List<Group> Groups { get; private set; }
		public List<Student> Students { get; private set; }

		private readonly CancellationTokenSource _cancellationSource;
		private IProgress<double> _progress;
		public List<List<Student>> ShuffleResult { get; private set; }

		public Shuffler(int lvCount, IEnumerable<int> groupSizes, CancellationTokenSource cancellationSource)
		{
			_cancellationSource = cancellationSource;

			ShuffleResult = new List<List<Student>>();
			LvCount = lvCount;
			Groups = new List<Group>(groupSizes.Select(x => new Group(x)));

			//initializing Students
			int studentCount = groupSizes.Sum();
			Students = new List<Student>();

			for (int i = 0; i < studentCount; i++)
			{
				Students.Add(new Student { Id = i + 1 });
			}
		}

		public List<List<Student>> Shuffle(Progress<double> progress = null)
		{
			_progress = progress;

			//Resetting group history after each shuffle
			Group.History = new List<HashSet<Student>>();

			//Going trough each LV
			_progress?.Report(0);
			for (int lv = 0; lv < LvCount; lv++)
			{
				var historyCopy = Group.History.ToList();
				var historyClearCheck = false;
				var studentPool = new List<Student>(Students);
				for(int i = 0; i < Groups.Count; i++)
				{
					Groups[i].AddStudents(studentPool, _cancellationSource,out studentPool, out var addedStudents, out bool historyClearSuggested);
					ShuffleResult.Add(addedStudents);
					if (historyClearSuggested)
					{
						historyClearCheck = true;
					}
				}
				
				//Checking if history needs to cleared
				if (historyClearCheck)
				{
					var sizes = 
						(from g in Groups 
						select g.Size).OrderBy(s => s).Distinct();
					
					//Going trough each group size
					foreach (var size in sizes)
					{
						bool clearHistory = true;
						var filteredHistory = Group.History.Where(h => h.Count == size).ToList();
						
						//Checking if history for this group size needs to be cleared
						var numberCombinations = HelperMethods.GetAllNumberCombinations(size, Students.Count);
						foreach (var numberCombination in numberCombinations)
						{
							var studentCombination = new List<Student>(
								from index in numberCombination
								select Students[index]);
							if (filteredHistory.Where(h => !h.Except(studentCombination).Any()).Any() == false)
							{
								clearHistory = false;
								break;
							}
						}
						
						//If history for this group size needs to be cleared
						if (clearHistory)
						{
							var extraHistory = Group.History.Except(historyCopy).ToList();
							historyCopy.RemoveAll(h => h.Count == size);
							Group.History = new List<HashSet<Student>>(historyCopy);
							Group.History.AddRange(extraHistory);
						}
					}
				}
				
				_progress?.Report((float)(lv + 1) / LvCount);
			}

			var tempResult = 
				(from resultRow in ShuffleResult select 
					(from student in resultRow 
					select student.Id).ToList()).ToList();
			//int err = 0;
			for (int i = 0; i < Students.Count; i++)
			{
				Debug.WriteLine($"Student {i}");
				for (int j = 0; j < Students.Count; j++)
				{
					if (i == j)
					{
						continue;
					}
					var count = 
						(from r in tempResult
						where r.Contains(i + 1) && r.Contains(j + 1)
						select r).Count();
					Debug.WriteLine(count);
					// if (count < 140 || count > 160)
					// {
					// 	Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
					// 	err += (count < 140) ? 140-count : count-160;
					// }
				}
				Debug.WriteLine("");
			}
			//Debug.WriteLine(err);
			
			return ShuffleResult;
		}
	}
}