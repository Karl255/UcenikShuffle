using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle
{
	public class Student
	{
		public int Id;
		public Dictionary<int, int> StudentSittingHistory = new Dictionary<int, int>();
		public Dictionary<int, int> GroupSittingHistory = new Dictionary<int, int>();

		public Student(int id, int[] otherIds, IEnumerable<Group> groups)
		{
			Id = id;

			//Setting up student sitting history
			foreach (var otherId in otherIds)
			{
				if (otherId == id)
					continue;
				StudentSittingHistory[otherId] = 0;
			}

			//Setting up group sitting history
			foreach (var group in groups)
			{
				GroupSittingHistory[group.Id] = 0;
			}
		}

		/// <summary>
		/// This function returs the ID of a student that the current student sat the least ammounts of time with
		/// </summary>
		/// <returns></returns>
		public int GetLeastSatWith() => StudentSittingHistory.OrderBy(student => student.Value).First().Key;

		public static int GetIndexOfId(List<Student> students, int id)
		{
			int? index = students.IndexOf(students.Where(s => s.Id == id).FirstOrDefault());
			return (index == null) ? -1 : (int)index;
		}
	}
}