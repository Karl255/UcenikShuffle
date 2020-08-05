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
		/// <param name="maxCombinationCount">Maximum number of combinations that will be returned (if there are more combinations than <paramref name="maxCombinationCount"/>, every nth combinations will be skipped - n depends on number of combinations and <paramref name="maxCombinationCount"/>)</param>
		/// <param name="takeOffset">SHOULDN'T BE SET OUTSIDE OF THIS METHOD! This parameter should only be used on recursive calls in this method. It specifies the offset of the number which will be taken. For example, if every second combination is taken and offset is 3, following combinations will be returned: 5th, 7th, 9th, 11th...</param>
		/// <param name="takeEvery">SHOULDN'T BE SET OUTSIDE OF THIS METHOD! This parameter should only be used on recursive calls in this method. It specifies which combinations will be returned. If there are 1000 combinations and <paramref name="maxCombinationCount"/> is 100, this parameter will be 10. That means that 10th, 20th, 30th etc. combinations will be returned</param>
		/// <returns></returns>
		public static IEnumerable<List<List<Student>>> GetAllStudentCombinations(IList<int> groupSizes, IList<Student> students, int maxCombinationCount = 100000, double takeEvery = 0, double takeOffset = 0)
		{
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
			IEnumerable<List<Student>> firstGroupCombinations;
			var sameGroupSizes = groupSizes.Where(s => s == groupSizes[0]).ToList();

			//Setting takeEvery value if this is the first time this method was called
			if (takeEvery == 0 && takeOffset == 0)
			{
				takeEvery = (double)GetCombinationCount(groupSizes) / maxCombinationCount;
			}

			//Getting all number combinations for the first group
			if (groupSizes.Count > 1 && groupSizes[0] == groupSizes[1])
			{
				firstGroupCombinations = GetFirstGroupCombinations(groupSizes, students);
			}
			else
			{
				//If the second group doesn't have the same size as the first one, no fancy duplicate removal techniques are necessary   
				firstGroupCombinations = GetAllStudentCombinationsForGroup(groupSizes[0], students).Select(c => c.ToList());
			}

			//Returning all number combinations if there is only 1 number group
			//TODO: it might be possible to simplify the algorithm by putting this in the foreach loop below this if (maybe special treatment isn't even necessary)
			if (groupSizes.Count == 1)
			{
				foreach (var combination in firstGroupCombinations)
				{
					//Skipping the combination if necessary
					if (takeOffset > 1)
					{
						takeOffset--;
						continue;
					}

					takeOffset += takeEvery;
					yield return new List<List<Student>>() { combination };
				}
				yield break;
			}

			//Going trough each combination for the first group and getting all possible combinations for other groups
			Student lastPivot = null;
			var availableStudents = new List<Student>(students);
			foreach (var firstGroupCombination in firstGroupCombinations)
			{
				//Removing some available students if the first student in the combination changed
				if (sameGroupSizes.Count > 1)
				{
					var pivot = firstGroupCombination[0];
					if (pivot != lastPivot)
					{
						lastPivot = pivot;
						availableStudents.Remove(pivot);
					}
				}

				//Skipping the combination if all combinations in this group would be skipped
				ulong combinationCount;
				ulong count1 = GetCombinationCountForSameSizeGroups(sameGroupSizes.Skip(1).ToList(), availableStudents.Count - groupSizes[0] + 1);
				ulong count2 = GetCombinationCount(groupSizes.Where(s => s != groupSizes[0]));
				combinationCount = (count1 == 0 ? 1 : count1) * (count2 == 0 ? 1 : count2);
				if (combinationCount < takeEvery + takeOffset)
				{
					takeOffset -= combinationCount;
					continue;
				}
				else
				{
					takeOffset -= combinationCount;
				}

				var tempGroupSizes = new List<int>(sameGroupSizes);

				//Getting all possible combinations for groups that have the same size as the first group
				if (sameGroupSizes.Count > 1)
				{
					var tempAvailableStudents = availableStudents.Except(firstGroupCombination).ToList();
					tempGroupSizes.RemoveAt(0);

					var sameSizeCombinations = GetAllStudentCombinations(tempGroupSizes, tempAvailableStudents, takeEvery: takeEvery, takeOffset: takeOffset + combinationCount);
					foreach (var sameSizeCombination in sameSizeCombinations)
					{
						var lvCombination = new List<List<Student>> { firstGroupCombination };
						lvCombination.AddRange(sameSizeCombination.ToList());

						//Returning the combination if all groups have the same size
						if (allGroupsAreSameSize)
						{
							takeOffset += takeEvery;
							yield return lvCombination;
							continue;
						}

						//Getting all possible combinations for the groups that don't have the same size as the first group
						tempAvailableStudents = new List<Student>(students);
						tempAvailableStudents.RemoveAll(i => firstGroupCombination.Contains(i));
						foreach (var group in sameSizeCombination)
						{
							tempAvailableStudents.RemoveAll(i => group.Contains(i));
						}
						tempGroupSizes = groupSizes.Where(s => s != groupSizes[0]).ToList();
						var otherGroupsCombinations = GetAllStudentCombinations(tempGroupSizes, tempAvailableStudents, takeEvery: takeEvery, takeOffset: takeOffset + combinationCount);
						foreach (var otherGroupsCombination in otherGroupsCombinations)
						{
							//Combining all of the group combinations into 1 combination
							otherGroupsCombination.InsertRange(0, lvCombination);
							takeOffset += takeEvery;
							yield return otherGroupsCombination;
						}
					}
				}
				else
				{
					//Getting number combinations for other groups
					var tempAvailableIndexes = availableStudents.Except(firstGroupCombination).ToList();
					tempGroupSizes = groupSizes.Where(s => s != groupSizes[0]).ToList();
					var combinations = GetAllStudentCombinations(tempGroupSizes, tempAvailableIndexes, takeEvery: takeEvery, takeOffset: takeOffset + combinationCount);
					foreach (var c in combinations)
					{
						//Combining all of the group combinations into 1 combination
						c.Insert(0, firstGroupCombination);
						takeOffset += takeEvery;
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
			for (int i = 0; i < groupSize; i++)
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

		/// <summary>
		/// This method the number of student sitting combinations
		/// </summary>
		/// <param name="groupSizes">Group sizes</param>
		/// <returns>Number of student sitting combinations</returns>
		public static ulong GetCombinationCount(IEnumerable<int> groupSizes)
		{
			#region Algorithm explanation

			/*In order to calculate all combinations without returning any duplicate combinations, groups have to be divided into different groups based on size
			 Groups sizes 1,1,2 and 3 would be divided into 3 groups: 1,1;2;3
			Number of combinations for the first group will be calculated when the available number count is max. Those combinations are:
			1;2		1;3		1;4		1;5		1;6		1;7		=6
			After that number of available numbers will be decremented and the process will be repeated until available number count is equal to sum of group sizes in this group (1+1=2).
			Other combinations:
			2;3		2;4		2;5		2;6		2;7				=5
			3;4		3;5		3;6		3;7						=4
			4;5		4;6		4;7								=3
			5;6		5;7										=2
			6;7												=1
			The number of combinations is 21 (6+5+4+3+2+1).
			Available number count is reset and it is then decremented by the sum of first group group sizes.
			This process is repeated for all groups and then the numbers are multiplied.
			Combinations for the second group:
			1,2		1,3		1,4		1,5		=4
			2,3		2,4		2,5				=3
			3,4		3,5						=2
			4,5								=1
											=4+3+2+1=10
			Combinations for the third group:
			x,x,x (x's are the the leftover numbers - numbers not used in other combinations) =1
			In the second and third group, there is no need for the available number decrement since the number of groups in this group is 1 and there is no need to be concerned with removing duplicates.
			Total number of combinations = 21 * 10 * 1 = 210
			*/

			#endregion

			ulong numberOfCombinations = 1;

			//Grouping groups based on group size
			int availableNumberCount = groupSizes.Sum();
			var sameSizeGroups = groupSizes.GroupBy(s => s).ToList();

			//Getting combination count for each group and multiplying them
			for (int i = 0; i < sameSizeGroups.Count; i++)
			{
				numberOfCombinations *= GetCombinationCountForSameSizeGroups(sameSizeGroups[i].Select(s => s).ToList(), availableNumberCount);
				availableNumberCount -= sameSizeGroups[i].Sum();
			}
			return numberOfCombinations;
		}

		private static ulong GetCombinationCountForGroup(int groupSize, int availableNumberCount)
		{
			#region Algorithm explanation

			/* Detailed algorithm explanation:
			 Number of combinations for a certain group depends on the group size and on the number of available numbers. 

			 Explanation of the algorithm on a simple example:
			 groupSize = 3
			 availableNumberCount = 5
			 1,2,3	1,2,4	1,2,5	1,3,4	1,3,5	1,4,5 = NS(3) = 3+2+1
			 2,3,4	2,3,5	2,4,5 = NS(2) = 2+1
			 3,4,5 = NS(1) = 1
			 In this example, when first number is 1, last number goes 3->2->1->2->1->1
			 This can be shown as NS(3).
			 NS(n) - sum of all numbers before number n (including number n)
			 NS(3) = 3+2+1 = 6
			 When first number is 2, there are NS(2) combinations, and when it's 3, there are NS(1) combinations so the total number of combinations would be:
			 NS(3)+NS(2)+NS(1) = 6+3+1 = 10

			 For complex examples it won't be as easy to calculate the result (not for all cases is the result sum of first NS(n) for first n numbers) so algorithm rules for all cases will be explained here:
			 3: 1+0+0 = (1+0+0)*3 = 3
			 2: 0+1+0 = (0+1+0)*2 = 2
			 1: 0+0+1 = (0+0+1)*1 = 1
			 Above is a number frequency table which shows the number of combinations if only the last 2 numbers change (the way to calculate initial table will be explained later).
			 Since the first example had 3 numbers, and first number is also changing it is necessary to update the table.
			 Every number in the table is updated by taking that number and adding to it all of the numbers after it.
			 3: (1+0+0) + (0+0) + 0
			 2: (0+1+0) + (1+0) + 0
			 1: (0+0+1) + (0+1) + 1
			 The update table looks like this:
			 3: 1+0+0
			 2: 1+1+0
			 1: 1+1+1
			 The number of combinations is (1+0+0)*3 + (1+1+0)*2 + (1+1+1)*1 = 3+4+3 = 10.
			 Initial table needs to be updated groupSize-2 times.
			 That means that if the group in this example had one more number, the table would need to be updated once more and it would look like this:
			 3: 1+0+0
			 2: 2+1+0
			 1: 3+2+1 

			 How to get to the initial table?
			 n = availableNumberCount-groupSize+1
			 The amount of numbers in the table is n.
			 Each row in the table has n numbers which are added together. All numbers are 0, except one number which is 1. That number depends on the number in the row.
			 For number 1, the last number would be 1, for number 2 the second last number would be 1, for number 3, third last number would be 1 etc.

			 Some initial table examples:
			 -example 1 (groupSize=2, availableNumberCount=6):
			 5: 1+0+0+0+0
			 4: 0+1+0+0+0
			 3: 0+0+1+0+0
			 2: 0+0+0+1+0
			 1: 0+0+0+0+1
			 example 2 (groupSize=2, availableNumberCount=2):
			 1: 1

			 A more complex example:
			 groupSize=5
			 availableNumberCount=9
			 Hand calculation:
			 1,2,3,4,5	1,2,3,4,6	1,2,3,4,7	1,2,3,4,8	1,2,3,4,9	1,2,3,5,6	1,2,3,5,7	1,2,3,5,8	1,2,3,5,9	1,2,3,6,7	1,2,3,6,8	1,2,3,6,9	1,2,3,7,8	1,2,3,7,9	1,2,3,8,9 = NS(5)
			 1,2,4,5,6	1,2,4,5,7	1,2,4,5,8	1,2,4,5,9	1,2,4,6,7	1,2,4,6,8	1,2,4,6,9	1,2,4,7,8	1,2,4,7,9	1,2,4,8,9 = NS(4)
			 1,2,5,6,7	1,2,5,6,8	1,2,5,6,9	1,2,5,7,8	1,2,5,7,9	1,2,5,8,9 = NS(3)
			 1,2,6,7,8	1,2,6,7,9	1,2,6,8,9 = NS(2)
			 1,2,7,8,9 = NS(1)
			 1,3,4,5,6	1,3,4,5,7	1,3,4,5,8	1,3,4,5,9	1,3,4,6,7	1,3,4,6,8	1,3,4,6,9	1,3,4,7,8	1,3,4,7,9	1,3,4,8,9 = NS(4)
			 1,3,5,6,7	1,3,5,6,8	1,3,5,6,9	1,3,5,7,8	1,3,5,7,9	1,3,5,8,9 = NS(3)
			 1,3,6,7,8	1,3,6,7,9	1,3,6,8,9 = NS(2)
			 1,3,7,8,9 = NS(1)
			 ...
			 =126
			 Calculation using the algorithm:
			 n = availableNumberCount-groupSize+1 = 5
			 Initial table needs to be updated 3 times (groupSize-2)
			 Initial table:
			 1: 0+0+0+0+1
			 2: 0+0+0+1+0
			 3: 0+0+1+0+0
			 4: 0+1+0+0+0
			 5: 1+0+0+0+0
			 After first update:
			 1: 1+1+1+1+1
			 2: 1+1+1+1+0
			 3: 1+1+1+0+0
			 4: 1+1+0+0+0
			 5: 1+0+0+0+0
			 After second update:
			 1: 5+4+3+2+1
			 2: 4+3+2+1+0
			 3: 3+2+1+0+0
			 4: 2+1+0+0+0
			 5: 1+0+0+0+0
			 After third update:
			 1: 15+10+6+3+1 = 35*1 = 35
			 2: 10+6+3+1+0 = 20*2 = 40
			 3: 6+3+1+0+0 = 10*3 = 30
			 4: 3+1+0+0+0 = 4*4 = 16
			 5: 1+0+0+0+0 = 1*5 = 5
			 Result = 35+40+30+16+5 = 126

			 Algorithm exceptions:
			 -since the table needs to be updated groupSize-2 times, that would mean that the table should be updated negative number of times if the group size is 1. In that case algorithm is ignored and availableNumberCount is returned.
			 -If available number count is equal to group size, 1 is returned
			 */

			#endregion

			if (availableNumberCount == groupSize)
			{
				return 1;
			}
			if (groupSize == 1)
			{
				return (ulong)availableNumberCount;
			}

			//SETUP
			var numberFrequencies = new List<List<int>>();
			for (int j = 0; j < availableNumberCount - groupSize + 1; j++)
			{
				numberFrequencies.Add(new List<int>());
			}
			for (int j = 0; j < numberFrequencies.Count; j++)
			{
				for (int k = 0; k < numberFrequencies.Count; k++)
				{
					numberFrequencies[j].Add(k == Math.Abs(j - numberFrequencies.Count + 1) ? 1 : 0);
				}
			}

			//CALCULATING THE NUMBER OF COMBINATIONS
			//Updating the numberFrequencies array groupSize-2 times
			for (int j = 0; j < groupSize - 2; j++)
			{
				//Updating each row in the array
				for (int k = 0; k < numberFrequencies.Count; k++)
				{
					//Updating each number in the row
					for (int l = 0; l < numberFrequencies[k].Count; l++)
					{
						for (int m = l + 1; m < numberFrequencies[k].Count; m++)
						{
							numberFrequencies[k][l] += numberFrequencies[k][m];
						}
					}
				}
			}

			//Calculating number of combinations based on the numberFrequencies array
			ulong numberOfCombinations = 0;
			for (int j = 0; j < numberFrequencies.Count; j++)
			{
				for (int k = 0; k < numberFrequencies[j].Count; k++)
				{
					numberOfCombinations += (ulong)((j + 1) * numberFrequencies[j][k]);
				}
			}

			return numberOfCombinations;
		}
		private static ulong GetCombinationCountForSameSizeGroups(IList<int> groupSizes, int availableNumberCount)
		{
			if (groupSizes.Count == 0)
			{
				return 0;
			}
			if (groupSizes.Count == 1)
			{
				return GetCombinationCountForGroup(groupSizes[0], availableNumberCount);
			}

			int minAvailableNumberCount = groupSizes.Sum();
			ulong numberOfCombinations = 0;
			var groupSizesWithoutFirst = groupSizes.Skip(1).ToList();

			//Getting all combinations until number of available numbers is the least it can be
			//Number of available numbers is sum of all group sizes. It is used so that duplicates are removed (if it went below this value, duplicate combinations would be returned)
			int groupSize = groupSizes[0] - 1;
			while (availableNumberCount >= minAvailableNumberCount)
			{
				ulong tempNumberOfCombinations = groupSize == 0 ? 1 : GetCombinationCountForGroup(groupSize, availableNumberCount - 1);
				tempNumberOfCombinations *= GetCombinationCountForSameSizeGroups(groupSizesWithoutFirst, availableNumberCount - groupSizes[0]);
				availableNumberCount--;
				numberOfCombinations += tempNumberOfCombinations;
			}
			return numberOfCombinations;
		}
		private static IEnumerable<List<Student>> GetFirstGroupCombinations(IList<int> groupSizes, IList<Student> students)
		{
			var sameGroupSizes = groupSizes.Where(s => s == groupSizes[0]).ToList();

			//If the second group has the same size as the first one than all combinations for the first group are calculated, and all combinations for other groups are calculated later on
			var tempStudents = new List<Student>(students);

			//Max first number is the max number that can be used for the first place in the first group (max number limit is set so that all duplicates are removed) 
			int maxFirstStudent = students.Count - sameGroupSizes.Sum() + 1;
			for (int i = 0; i < maxFirstStudent; i++)
			{
				var firstStudent = tempStudents[0];
				tempStudents.RemoveAt(0);
				var combinations = GetAllStudentCombinationsForGroup(groupSizes[0] - 1, tempStudents).Select(c => c.ToList());
				foreach (var combination in combinations)
				{
					combination.Insert(0, firstStudent);
					yield return combination;
				}
			}
		}
	}
}