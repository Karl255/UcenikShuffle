using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UcenikShuffle.Common
{
	public class Shuffler
	{
		public int LvCount { get; private set; }
		public List<Group> Groups { get; private set; }
		public List<Student> Students { get; private set; }

		private readonly CancellationTokenSource _cancellationSource;
		private IProgress<double> _progress;

		public Shuffler(int lvCount, IEnumerable<int> groupSizes, CancellationTokenSource cancellationSource)
		{
			_cancellationSource = cancellationSource;

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

		public void Shuffle(Progress<double> progress = null)
		{
			_progress = progress;

			//Resetting group history after each shuffle
			Group.History = new List<HashSet<Student>>();

			//Going trough each LV
			_progress?.Report(0);
			for (int lv = 0; lv < LvCount; lv++)
			{
				var studentPool = new List<Student>(Students);
				foreach (var group in Groups)
				{
					studentPool = group.AddStudents(studentPool, _cancellationSource);
				}
				_progress?.Report((float)(lv + 1) / LvCount);
			}
		}
	}
}