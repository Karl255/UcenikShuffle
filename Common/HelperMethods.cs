using System;
using System.Collections.Generic;
using System.Linq;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public static class HelperMethods
	{
		/// <summary>
		/// This method returns all student sitting combinations for a lv
		/// </summary>
		/// <param name="groups">Groups in lv</param>
		/// <param name="students">Students</param>
		/// <returns></returns>
		public static IEnumerable<List<List<Student>>> GetAllStudentCombinations(IList<Group> groups, IList<Student> students)
		{
			var combinations = new List<List<Student>>();

			//Checking if passed parameters are valid
			foreach (var group in groups)
			{
				if (group.Size <= 0)
				{
					throw new GroupSizeException();
				}
			}
			if (students == null || students.Count <= 0)
			{
				throw new ArgumentException("Broj učenika mora biti pozitivni cijeli broj!");
			}

			//Getting combinations for the current group
			var pivot = students[0];
			List<List<Student>> tempCombinations;

			//Pivot is a student which is present in each combination for this group and is used only if a group after this one has the same size
			if (groups.Count > 1 && groups[0].Size == groups[1].Size)
			{
				var studentsCopy = new List<Student>(students);
				studentsCopy.Remove(pivot);
				tempCombinations = GetAllStudentCombinationsForGroup(groups[0].Size - 1, studentsCopy).Select(c => c.ToList()).ToList();
				for (int i = 0; i < tempCombinations.Count; i++)
				{
					tempCombinations[i].Insert(0, pivot);
				}
			}
			//Pivot isn't used if this is the last groups or if there aren't any groups of this size in the group list
			else
			{
				tempCombinations = GetAllStudentCombinationsForGroup(groups[0].Size, students).Select(c => c.ToList()).ToList();
			}
			combinations.AddRange(tempCombinations);

			//Getting combinations for other groups based on available indexes
			foreach (var combination in combinations)
			{
				var tempGroups = new List<Group>(groups);
				tempGroups.Remove(groups[0]);
				var availableStudents = new List<Student>(students);
				availableStudents.RemoveAll(i => combination.Contains(i));
				var lvCombination = new List<List<Student>>();
				lvCombination.Add(combination);

				//Not getting inner combinations if this is the last group
				if (combination.Count == students.Count)
				{
					yield return lvCombination;
				}
				//Getting inner combinations if this isn't the last group
				else
				{
					var innerCombinations = GetAllStudentCombinations(tempGroups, availableStudents).ToList();
					foreach (var innerCombination in innerCombinations)
					{
						var tempLvCombination = new List<List<Student>>(lvCombination);
						tempLvCombination.AddRange(innerCombination.Select(c => c.ToList()).ToList());
						yield return tempLvCombination;
					}
				}
			}
		}

		/// <summary>
		/// This method returns all possible student combinations for a certain group size
		/// </summary>
		/// <param name="groupSize">Size of the group (for example, if there are 5 numbers and group size is 3, only 3 of those numbers would be able to fit in a certain group combination)</param>
		/// <param name="students">Students</param>
		/// <returns></returns>
		private static IEnumerable<IEnumerable<Student>> GetAllStudentCombinationsForGroup(int groupSize, IList<Student> students)
		{
			var combination = new List<Student>();
			
			//If group size is bigger than the number of available numbers or if group size is 1
			if (groupSize >= students.Count)
			{
				yield return students;
				yield break;
			}

			//If group size is 1
			if (groupSize == 1)
			{
				foreach (var student in students)
				{
					yield return new List<Student>() { student };
				}
				yield break;
			}
			
			//Initial combination
			for(int i = 0; i < groupSize; i++)
			{
				combination.Add(students[i]);
			}
			yield return new List<Student>(combination);

			//Getting all combinations
			while (true)
			{
				bool noMoreCombinations = true;
				for (int i = groupSize - 1; i >= 0; i--)
				{
					//lastStudent is the last possible student for this spot in the combination
					var lastStudent = students[i + students.Count - groupSize];
					if (combination[i] != lastStudent)
					{
						noMoreCombinations = false;
						var newIndex = students.IndexOf(combination[i]) + 1;
						combination[i] = students[newIndex];
						for (int j = 1; j < groupSize - i; j++)
						{
							combination[i + j] = students[newIndex + j];
						}
						yield return new List<Student>(combination);
						break;
					}
				}

				//If all combinations have been tried out
				if (noMoreCombinations)
				{
					yield break;
				}
			}
		}

		/// <summary>
		/// This method calculates the complexity of a shuffle operation
		/// </summary>
		/// <param name="groupSizes">Sizes of student groups</param>
		/// <param name="lvCount">Amount of LVs</param>
		/// <returns></returns>
		public static int GetShuffleComplexity(IEnumerable<int> groupSizes, int lvCount)
		{
			int studentCount = groupSizes.Sum();
			int points = 0;
            
			//Adding up complexity of each group
			foreach (int size in groupSizes)
			{
				//Calculating number of group combinations
				int combinations = 0;
				for (int i = 1; i <= studentCount - size + 1; i++)
				{
					combinations += i;
				}
                
				//Points are an arbitrary value used to measure shuffle complexity
				//Points heavily depend on the size of the group since it is the value which affects the complexity the most 
				points += combinations * size;
				
				//Removing number of students from the list of available students
				studentCount -= size;
			}
            
			//Adding number of laboratory exercises into consideration when calculating complexity
			//(this value isn't as important as number of combinations or group size - this was found out during testing)
			return (int)(points * Math.Pow(lvCount, 0.25));
		}

		/// <summary>
		/// This method compares 2 shuffle records
		/// </summary>
		/// <param name="r1">First record which is compared with the second one</param>
		/// <param name="r2">Second record which is compared with the first one</param>
		/// <returns>True if records are the same, false otherwise</returns>
		public static bool CompareShuffleRecords(IReadOnlyList<List<Student>> r1, IReadOnlyList<List<Student>> r2)
		{
			//If number of groups isn't the same for both records 
			if (r1.Count != r2.Count)
			{
				return false;
			}

			//Going trough each group in record 1, and checking if that group also exists in record 2
			var tempR2 = new List<List<Student>>(r2);
			for (int i = 0; i < r1.Count; i++)
			{
				var group = tempR2.FirstOrDefault(r => r1[i].Count == r.Count && r1[i].Except(r).Any() == false);
				if (group == null)
				{
					return false;
				}
				tempR2.Remove(group);
			}
			return true;
		}
	}
}