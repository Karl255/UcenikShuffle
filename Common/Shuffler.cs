using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace UcenikShuffle.Common
{
	public class Shuffler
	{
		public readonly int LvCount;
		//All groups on laboratory exercises (should be changed if calculations are needed for another situation)
		public readonly List<Group> Groups = new List<Group>();
		public List<Student> Students { get; private set; }
		private readonly CancellationTokenSource _cancellationSource;
		private IProgress<float> _progress;

		public Shuffler(int lvCount, IEnumerable<int> groupSizes, CancellationTokenSource cancellationSource)
		{
			_cancellationSource = cancellationSource;
			
			//Configuring the list of students
			Students = new List<Student>();

			//Parsing lv count and group sizes input
			LvCount = lvCount;

			foreach (var size in groupSizes)
			{
				Groups.Add(new Group(size));
			}
			
			//Configuring student list based on the group sizes
			var studentCount = groupSizes.Sum();
			for (int i = 0; i < studentCount; i++)
			{
				Students.Add(new Student { Id = i + 1 });
			}
		}

		public void Shuffle(Progress<float> progress = null)
		{
			_progress = progress;
			CreateGroupsForLvs();
		}
		
		/// <summary>
		/// This method creates the groups for the LV based on the <see cref="Groups"/> and <see cref="Students"/> variables
		/// </summary>
		private void CreateGroupsForLvs()
		{
			//Resetting group history after each shuffle
			Group.History = new List<HashSet<Student>>();
			
			//Going trough each laboratory exercise (lv)
			_progress?.Report(0);
			for (int lv = 0; lv < LvCount; lv++)
			{
				var studentPool = new List<Student>(Students);
				foreach(var group in Groups)
				{
					studentPool = group.AddStudents(studentPool, _cancellationSource);
				}
				_progress?.Report((float) (lv + 1) / LvCount);
			}
		}
	}
}