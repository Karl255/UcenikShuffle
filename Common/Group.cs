using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UcenikShuffle.Common
{
	public class Group
	{

		public static List<HashSet<Student>> History;
		public readonly int Size;
		
		public Group(int size)
		{
			Size = size;
		}

		/// <summary>
		/// This method tries to find the best possible combination of students to put in the same group for a laboratory work
		/// </summary>
		/// <param name="studentPool">List of all available students (the ones that are in other groups for the current laboratory work should be excluded)</param>
		/// <returns>List of all available students (after removing those who were chosen for the current group)</returns>
		public List<Student> AddStudents(List<Student> studentPool, CancellationTokenSource cancellationSource)
		{
			cancellationSource.Token.ThrowIfCancellationRequested();
			
			//Getting the student that sat the least amount of times in the current group
			studentPool = studentPool.OrderBy(x => x.GroupSittingHistory[this]).ToList();
			
			//Getting all combinations for a group and ordering them from the best combination to worst
			var combinations = (from combination in HelperMethods.GetAllNumberCombinations(Size, studentPool.Count).AsParallel().WithCancellation(cancellationSource.Token)
								//Ordering by amount of times the current student sat with other students
							orderby (from index in combination
										 //Getting the amount of times students in a group sat with each other
									 select (from history in studentPool[index].StudentSittingHistory
											 where combination.Contains(Student.GetIndexOfId(studentPool, history.Key.Id))
											 select history.Value).Sum()).Sum(),
											 //Ordering by group sitting history
											 (from index in combination 
												 select index).Sum()
							select combination);

			//Going trough all group combinations
			HashSet<Student> newEntry = null;
			foreach (var combination in combinations)
			{
				newEntry = new HashSet<Student>(combination.Select(x => studentPool[x]));

				//Checking if current group combination is unique (exiting the loop if that's the case)
				if (!SearchGroupHistory(newEntry).Any())
				{
					break;
				}

				newEntry = null;
			}

			//If all groups have been tried out
			if (newEntry == null)
			{
				newEntry = History.Where(h => h.Count == Size && !h.Except(studentPool).Any()).OrderBy(h => SearchGroupHistory(h).Count()).First();
			}

			//Updating histories of individual students
			foreach (var stud1 in newEntry)
			{
				foreach (var stud2 in newEntry)
				{
					if (stud1 == stud2)
					{
						continue;
					}
					stud1.StudentSittingHistory[stud2] = stud1.StudentSittingHistory[stud2] + 1;
				}
				stud1.GroupSittingHistory[this] = stud1.GroupSittingHistory[this] + 1;
			}

			//Updating history for the current group
			History.Add(new HashSet<Student>(newEntry));

			//Removing students in the chosen group from the result
			foreach (var student in newEntry)
			{
				studentPool.Remove(student);
			}

			return studentPool;
		}

		/// <summary>
		/// This method returns all groups from group history which contain all students in <paramref name="students"/> parameter and don't contain any other students
		/// </summary>
		/// <param name="students"></param>
		public static IEnumerable<IEnumerable<Student>> SearchGroupHistory(IEnumerable<Student> students)
		{
			//Going trough all groups that match the size
			foreach (var history in History.Where(h => h.Count == students.Count()))
			{
				///Returning the history information if it matches parameter <param name="students"/>
				if (history.Except(students).Count() == 0)
				{
					yield return history.AsEnumerable();
				}
			}
			yield break;
		}

		public static bool CompareGroupHistoryRecords(IEnumerable<Student> group1, IEnumerable<Student> group2) =>
			group1 != null
			&& group2 != null
			&& group1.Count() == group2.Count()
			&& group1.Except(group2).Count() == 0;

	}
}