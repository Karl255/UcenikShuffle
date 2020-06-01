using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using UcenikShuffle.Common.Exceptions;

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
		/// This method tries to find the best possible combination of students to put in the same group for a LV
		/// </summary>
		/// <param name="studentPool">List of all available students (the ones that are in other groups for the current LV should be excluded)</param>
		/// <param name="cancellationSource">Cancellation source for shuffle canceling</param>
		/// <returns>List of all available students (after removing those who were chosen for the current group)</returns>
		public void AddStudents(List<Student> studentPool, out List<Student> availableStudents, out List<Student> addedStudents, out bool clearHistorySuggested)
		{
			clearHistorySuggested = false;
			
			//Ordering by difference in maximum and minimum amount of times student sat with other students
			//This is done so that students that were previously neglected get the best possible schedule
			studentPool = studentPool
				.OrderByDescending(x =>
				{
					if (x.StudentSittingHistory.Count > 0)
					{
						var studentSittingHistoryValues = x.StudentSittingHistory.Select(h => h.Value).ToList();
						var difference = studentSittingHistoryValues.Max() - studentSittingHistoryValues.Min();
						return difference;
					}
					return 0;
				})
				//Ordering by amount of times students sat in this groups
				.ThenBy(x => x.GroupSittingHistory[this])
				//Ordering by student ID
				.ThenBy(x => x.Id)
				.ToList();

			//-------- ALGORITHM BEGINNING --------//
			
			//Getting all combinations for a group
			var numberCombinations = HelperMethods
				.GetAllNumberCombinations(Size, studentPool.Count);
			
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
					var sittingValues =
						from history in student.StudentSittingHistory
						where combination.Contains(history.Key)
						select history.Value;
					sum += sittingValues.Sum();
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
				newEntry = History.Where(h => h.Count == Size && !h.Except(studentPool).Any()).OrderBy(h => SearchGroupHistory(h).Count()).First();
				clearHistorySuggested = true;
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
			History.Add(new HashSet<Student>(addedStudents));

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