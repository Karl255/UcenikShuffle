using System.Collections.Generic;

namespace UcenikShuffle
{
	public class Student
	{
		public int Id;
		public Dictionary<int, int> SatWith = new Dictionary<int, int>();

		public Student(int id, int[] otherIds)
		{
			Id = id;

			foreach (var otherId in otherIds)
			{
				if (otherId == id)
					continue;
				SatWith[otherId] = 0;
			}
		}

		public int GetLeastSatWith()
		{
			int max = SatWith[0];
			int indexMax = 0;

			for (int i = 1; i < SatWith.Count; i++)
			{
				if (SatWith[i] > max)
				{
					max = SatWith[i];
					indexMax = i;
				}
			}

			return indexMax;
		}
	}
}
