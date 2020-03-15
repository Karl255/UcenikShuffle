using System.Collections.Generic;

namespace UcenikShuffle
{
	public class Student
	{
		public int Id;
		public Dictionary<int, int> SatWithStudent = new Dictionary<int, int>();
		public Dictionary<int, int> SatInGroup = new Dictionary<int, int>();

		public Student(int id, int[] otherIds, IEnumerable<Group> groups)
		{
			Id = id;

			foreach (var otherId in otherIds)
			{
				if (otherId == id)
					continue;
				SatWithStudent[otherId] = 0;
			}

			foreach (var group in groups)
			{
				SatInGroup[group.Id] = 0;
			}
		}

		public int GetLeastSatWith()
		{
			int max = SatWithStudent[0];
			int indexMax = 0;

			for (int i = 1; i < SatWithStudent.Count; i++)
			{
				if (SatWithStudent[i] > max)
				{
					max = SatWithStudent[i];
					indexMax = i;
				}
			}

			return indexMax;
		}
	}
}
