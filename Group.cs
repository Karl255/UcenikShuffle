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
			var first = studentPool.Pop(0);
			var newEntry = new HashSet<Student> { first };
			bool found = false;

			for (int i = 0; i < studentPool.Count; i++)
			{
				if (found) break;
				
				var studentPoolCopy = studentPool.OrderBy(x => x.SatWith[first.Id] * 100 + 100 - x.Id).ToList();
				var second = studentPoolCopy.Pop(i);

				for (int j = 0; j < studentPoolCopy.Count; j++)
				{
					var studentPoolCopyCopy = studentPoolCopy.OrderBy(x => x.SatWith[first.Id] + x.SatWith[second.Id]).ToList();
					var third = studentPoolCopyCopy.Pop(j);
					var testEntry = new HashSet<int>
					{
						first.Id,
						second.Id,
						third.Id
					};

					if (!History.Contains(testEntry, (h1, h2) => !h1.Except(h2).Any()))
					{
						studentPool.Remove(second);
						studentPool.Remove(third);
						newEntry.Add(second);
						newEntry.Add(third);
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
