using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
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
			Groups = Groups.OrderByDescending(g => g.Size).ToList();

			//Going trough each LV
			_progress?.Report(0);
			for (int lv = 0; lv < LvCount; lv++)
			{
				var studentPool = new List<Student>(Students);
				for(int i = 0; i < Groups.Count; i++)
				{
					_cancellationSource.Token.ThrowIfCancellationRequested();
					Groups[i].AddStudents(studentPool,out studentPool, out var addedStudents);
					ShuffleResult.Add(addedStudents);
				}

				_progress?.Report((float)(lv + 1) / LvCount);
			}
			
			//DEBUGGING OUTPUT: used for testing purposes
			foreach (var student in Students)
			{
				Debug.WriteLine($"Student {student.Id}");
				foreach (var h in student.StudentSittingHistory.OrderBy(h => h.Key.Id))
				{
					Debug.WriteLine($"{h.Key.Id}: {h.Value}");
				}
				Debug.WriteLine("");
			}

			return ShuffleResult;
		}
	}
}