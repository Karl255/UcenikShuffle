using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public class Group
	{

		private static List<HashSet<Student>> _history;
		public readonly int Size;

		public Group(int size)
		{
			Size = size;
		}

		/// <summary>
		/// This method tries to find the best possible combination of students to put in the same group for a LV
		/// </summary>
		/// <param name="studentPool">List of all available students (the ones that are in other groups for the current LV should be excluded)</param>
		/// <param name="cancellationSource">Cancellation source for shuffle canceling</param>
		/// <returns>List of all available students (after removing those who were chosen for the current group)</returns>
		public void AddStudents(List<Student> studentPool, CancellationTokenSource cancellationSource, out List<Student> availableStudents, out List<Student> addedStudents)
		{
			cancellationSource.Token.ThrowIfCancellationRequested();

			//Getting the student that sat the least amount of times in the current group
			studentPool = studentPool.OrderBy(x => x.GroupSittingHistory[this]).ToList();

			//-------- ALGORITHM BEGINNING --------//
			
			//Getting all combinations for a group
			var numberCombinations = HelperMethods
				.GetAllNumberCombinations(Size, studentPool.Count)
				.AsParallel()
				.WithCancellation(cancellationSource.Token);
			
			//Converting number combinations to student combinations
			var studentCombinations = new List<List<Student>>();
			foreach (var combination in numberCombinations)
			{
				var studentCombination = from c in combination select studentPool[c];
				studentCombinations.Add(studentCombination.ToList());
			}
			
			//Ordering student combinations by amount of times each student sat with other students in the group
			studentCombinations = studentCombinations.OrderBy(combination =>
			{
				int sum = 0;
				foreach (var student in combination)
				{
					sum += (from history in student.StudentSittingHistory
						where combination.Contains(history.Key)
						select history.Value).Sum();
				}
				return sum;
			}).ToList();

			//-------- ALGORITHM ENDING --------//
			
			//Going trough all group combinations
			HashSet<Student> newEntry = null;
			foreach (var combination in studentCombinations)
			{
				//Checking if current group combination is unique (exiting the loop if that's the case)
				if (!SearchGroupHistory(combination).Any())
				{
					newEntry = new HashSet<Student>(combination);
					break;
				}
			}

			//If all groups have been tried out
			if (newEntry == null)
			{
				newEntry = _history.Where(h => h.Count == Size && !h.Except(studentPool).Any()).OrderBy(h => SearchGroupHistory(h).Count()).First();
				
				//Removing unnecessary combinations
				foreach (var combination in studentCombinations)
				{
					_history.RemoveAll(h => h.Count == combination.Count() && !h.Except(combination).Any());
				}
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
			addedStudents = new List<Student>(newEntry);
			_history.Add(new HashSet<Student>(addedStudents));

			//Removing students in the chosen group from the result
			foreach (var student in newEntry)
			{
				studentPool.Remove(student);
			}
			availableStudents = studentPool;
		}

		/// <summary>
		/// This method returns all groups from group history which contain all students in <paramref name="students"/> parameter and don't contain any other students
		/// </summary>
		/// <param name="students"></param>
		public static IEnumerable<IEnumerable<Student>> SearchGroupHistory(IEnumerable<Student> students)
		{
			//Going trough all groups that match the size
			foreach (var history in _history.Where(h => h.Count == students.Count()))
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

		public static void ResetHistory()
		{
			_history = new List<HashSet<Student>>();
		}
	}
}