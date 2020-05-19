using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UcenikShuffle.Common
{
	public class Shuffler
	{
		//All groups on laboratory exercises (should be changed if calculations are needed for another situation)
		public List<Group> Groups = new List<Group>();
		public ObservableCollection<Student> Students = new ObservableCollection<Student>();

		public Shuffler()
		{
			Students.CollectionChanged += (o, e) =>
			{
				//Ordering student Ids by their label
				var students = Students.Where(s => string.IsNullOrEmpty(s.Label) == false).ToList();
				int i;
				for (i = 0; i < students.Count; i++)
				{
					students[i].Id = i + 1;
				}
				students = Students.Where(s => string.IsNullOrEmpty(s.Label) == true).ToList();
				for (int j = 0; j < students.Count; j++)
				{
					students[j].Id = i + 1 + j;
				}
			};
		}

		/// <summary>
		/// This method creates the groups for the LV based on the <see cref="Group.Groups"/> and <see cref="Students"/> variables
		/// </summary>
		public void CreateGroupsForLvs(int lvCount)
		{
			//Going trough each laboratory exercise (lv)
			for (int lv = 0; lv < lvCount; lv++)
			{
				var studentPool = new List<Student>(Students);
				for (int i = 0; i < Groups.Count; i++)
				{
					studentPool = Groups[i].AddStudents(studentPool);
				}
			}
		}

	}
}
