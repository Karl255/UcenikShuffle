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
		/// <param name="groupSizes">Sizes of groups</param>
		/// <param name="students">Students</param>
		/// <returns></returns>
		public static IEnumerable<List<List<Student>>> GetAllStudentCombinations(IList<int> groupSizes, IList<Student> students)
		{
			List<List<Student>> combinations;

			//Checking if passed parameters are valid
			foreach (int size in groupSizes)
			{
				if (size <= 0)
				{
					throw new GroupSizeException();
				}
			}
			if (students == null || students.Count <= 0)
			{
				throw new ArgumentException("Broj učenika mora biti pozitivni cijeli broj!");
			}

			bool allGroupsAreSameSize = groupSizes.Distinct().Count() == 1;
			List<List<Student>> firstGroupCombinations;
			var sameGroupSizes = groupSizes.Where(s => s == groupSizes[0]).ToList();
			
			//Getting all number combinations for the first group
			if (groupSizes.Count > 1 && groupSizes[0] == groupSizes[1])
			{
				//If the second group has the same size as the first one than all combinations for the first group are calculated, and all combinations for other groups are calculated later on
				firstGroupCombinations = new List<List<Student>>();
				var tempStudents = new List<Student>(students);
				
				//Max first number is the max number that can be used for the first place in the first group (max number limit is set so that all duplicates are removed) 
				int maxFirstStudent = students.Count - sameGroupSizes.Sum() + 1;
				for (int i = 0; i < maxFirstStudent; i++)
				{
					var firstStudent = tempStudents[0];
					tempStudents.RemoveAt(0);
					var tempCombinations =  GetAllStudentCombinationsForGroup(groupSizes[0] - 1, tempStudents).Select(c => c.ToList()).ToList();
					for (int j = 0; j < tempCombinations.Count; j++)
					{
						tempCombinations[j].Insert(0, firstStudent);
					}
					firstGroupCombinations.AddRange(tempCombinations);
				}
			}
			else
			{
				//If the second group doesn't have the same size as the first one, no fancy duplicate removal techniques are necessary   
				firstGroupCombinations = GetAllStudentCombinationsForGroup(groupSizes[0], students).Select(c => c.ToList()).ToList();
			}

			//Returning all number combinations if there is only 1 number group
			if (groupSizes.Count == 1)
			{
				foreach (var combination in firstGroupCombinations)
				{
					yield return new List<List<Student>>() {combination};
				}
				yield break;
			}

			//Going trough each combination for the first group and getting all possible combinations for other groups
			Student lastPivot = null;
			var availableStudents = new List<Student>(students);
			foreach (var firstGroupCombination in firstGroupCombinations)
			{
				var tempGroupSizes = new List<int>(sameGroupSizes);

				//Getting all possible combinations for groups that have the same size as the first group
				if (sameGroupSizes.Count > 1)
				{
					var tempAvailableStudents = availableStudents.Except(firstGroupCombination).ToList();
					tempGroupSizes.RemoveAt(0);
					var pivot = firstGroupCombination[0];
					if (pivot != lastPivot)
					{
						lastPivot = pivot;
						availableStudents.Remove(pivot);
					}
					foreach (var otherGroupsCombination in GetAllStudentCombinations(tempGroupSizes, tempAvailableStudents))
					{
						var lvCombination = new List<List<Student>> {firstGroupCombination};
						lvCombination.AddRange(otherGroupsCombination.ToList());
						
						//Returning the combination if all groups have the same size
						if (allGroupsAreSameSize)
						{
							yield return lvCombination;
							continue;
						}
					
						//Getting all possible combinations for the groups that don't have the same size as the first group
						tempAvailableStudents = new List<Student>(students);
						tempAvailableStudents.RemoveAll(i => firstGroupCombination.Contains(i));
						foreach (var group in otherGroupsCombination)
						{
							tempAvailableStudents.RemoveAll(i => group.Contains(i));
						}
						tempGroupSizes = groupSizes.Where(s => s != groupSizes[0]).ToList();
						foreach (var c in GetAllStudentCombinations(tempGroupSizes, tempAvailableStudents))
						{
							//Combining all of the group combinations into 1 combination
							c.InsertRange(0, lvCombination);
							yield return c;
						}
					}
				}
				else
				{
					//Getting number combinations for other groups
					var tempAvailableIndexes = availableStudents.Except(firstGroupCombination).ToList();
					tempGroupSizes = groupSizes.Where(s => s != groupSizes[0]).ToList();
					foreach (var c in GetAllStudentCombinations(tempGroupSizes, tempAvailableIndexes))
					{
						//Combining all of the group combinations into 1 combination
						c.Insert(0, firstGroupCombination);
						yield return c;
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