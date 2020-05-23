using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UcenikShuffle.Common
{
	public class Shuffler
	{
		public readonly int LvCount;
		//All groups on laboratory exercises (should be changed if calculations are needed for another situation)
		public readonly List<Group> Groups = new List<Group>();
		public ObservableCollection<Student> Students { get; private set; }

		public Shuffler(string lvCountInput, string groupSizesInput)
		{
			//Configuring the list of students
			Students = new ObservableCollection<Student>();

			//Parsing lv count and group sizes input
			//TODO: add input validation
			LvCount = int.Parse(lvCountInput);
			var groupSizes = groupSizesInput.Replace(" ", null).Split(',').Select(int.Parse).ToArray();

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

		public void Shuffle()
		{
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
			for (int lv = 0; lv < LvCount; lv++)
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