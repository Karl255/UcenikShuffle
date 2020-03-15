using System;
using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle
{
	public class Group
	{
		public int Size;
		public List<HashSet<int>> History = new List<HashSet<int>>();

		public Group(int size)
		{
			Size = size;
		}

		public void AddStudents(List<Student> studentPool)
		{
			var newEntry = new HashSet<Student> { studentPool.Pop(0) };
			studentPool = studentPool.OrderBy(x => newEntry.First().SatWith[x.Id]).ToList();
			bool found = false;

			for (int i = 0; i < studentPool.Count; i++)
			{
				if (found) break;

				for (int j = i + 1; j < studentPool.Count; j++)
				{
					if (!History.Contains(newEntry.Select(x => x.Id)))
					{
						newEntry.Add(studentPool.Pop(i));
						newEntry.Add(studentPool.Pop(j));
						found = true;
						break;
					}
				}
			}


			foreach (var stud1 in newEntry)
			{
				foreach (var stud2 in newEntry)
				{
					if (stud1 == stud2)
						continue;

					stud1.SatWith[stud2.Id]++;
				}
			}

			History.Add(new HashSet<int>(newEntry.Select(x => x.Id)));
		}

		public void AddStudent(Student student)
		{
			if (Size > 1)
			{
				throw new Exception("This overload is only for gorups of size 1.");
			}

			History.Add(new HashSet<int> { student.Id });
		}
	}
}
