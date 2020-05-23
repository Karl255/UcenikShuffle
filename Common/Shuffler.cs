using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UcenikShuffle.Common
{
	public class Shuffler
	{
		private int LvCount;
		//All groups on laboratory exercises (should be changed if calculations are needed for another situation)
		public List<Group> Groups = new List<Group>();
		public ObservableCollection<Student> Students = new ObservableCollection<Student>();

		public Shuffler(int lvCount)
		{
			LvCount = lvCount;

			Students.CollectionChanged += (o, e) =>
			{
				Students = new ObservableCollection<Student>(Students.OrderBy(s => s.Label));
			};
		}

		/// <summary>
		/// This method creates the groups for the LV based on the <see cref="Group.Groups"/> and <see cref="Students"/> variables
		/// </summary>
		private void CreateGroupsForLvs()
		{
			Group.History = new List<HashSet<Student>>();
			//Going trough each laboratory exercise (lv)
			for (int lv = 0; lv < LvCount; lv++)
			{
				var studentPool = new List<Student>(Students);
				for (int i = 0; i < Groups.Count; i++)
				{
					studentPool = Groups[i].AddStudents(studentPool);
				}
			}
		}

		public void Shuffle()
		{
			CreateGroupsForLvs();
		}
	}
}