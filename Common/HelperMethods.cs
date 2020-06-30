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
		public static IEnumerable<IEnumerable<IEnumerable<Student>>> GetAllStudentCombinations(List<Group> groups, List<Student> students)
		{
			var combinations = new List<List<Student>>();
			var studentsCopy = new List<Student>(students);
			studentsCopy = studentsCopy.OrderBy(s => s.Id).ToList();

			//Getting combinations for the current group
			//If group size is 1
			if (groups[0].Size == 1)
			{
				foreach (var student in studentsCopy)
				{
					combinations.Add(new List<Student>() { student });
				}
			}
			//If group size isn't 1
			else
			{
				var pivot = studentsCopy[0];
				List<List<Student>> tempCombinations;

				//Pivot is a student which is present in each combination for this group and is used only if a group after this one has the same size
				if (groups.Count > 1 && groups[0].Size == groups[1].Size)
				{
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
					tempCombinations = GetAllStudentCombinationsForGroup(groups[0].Size, studentsCopy).Select(c => c.ToList()).ToList();
				}
				combinations.AddRange(tempCombinations);
			}

			//Getting combinations for other groups based on available indexes
			foreach (var combination in combinations)
			{
				var tempGroups = new List<Group>(groups);
				tempGroups.Remove(groups[0]);
				var availableStudents = new List<Student>(studentsCopy);
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
		private static IEnumerable<IEnumerable<Student>> GetAllStudentCombinationsForGroup(int groupSize, List<Student> students)
		{
			var combination = new List<Student>();
			
			//Checking if passed parameters are valid
			if (groupSize <= 0)
			{
				throw new GroupSizeParameterException();
			} 
			if (students == null || students.Count <= 0)
			{
				throw new ArgumentException("Broj učenika mora biti pozitivni cijeli broj!"); 
			}
			
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
			for(int i = 0; i < groupSize - 1; i++)
            {
				combination.Add(students[i]);
            }
            
			for(int startIndex = 0; startIndex + groupSize <= students.Count; startIndex++)
			{
				//Changing the last number until it hits max number
				for (int lastIndex = startIndex + groupSize - 1; lastIndex < students.Count; lastIndex++)
				{
					combination.Add(students[lastIndex]);
					//Note: new list must be created otherwise collection modified exception might thrown
					//TODO: use the clone/copy method
					yield return new List<Student>(combination);
					combination.Remove(students[lastIndex]);
				}
				
				//Popping the first number and adding another number to the base combination instead of the popped number
				if (combination.Count > 0)
				{
					combination.RemoveAt(0);
					combination.Add(students[startIndex + groupSize - 1]);
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
			foreach (var size in groupSizes)
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
	}
}