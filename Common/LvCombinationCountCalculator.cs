using System;
using System.Collections.Generic;
using System.Linq;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public class LvCombinationCountCalculator
	{
		List<int> _groupSizes;

		public LvCombinationCountCalculator(List<int> groupSizes, int availableStudentCount)
		{
			_groupSizes = groupSizes;
			int groupSizesSum = groupSizes == null ? 0 : groupSizes.Sum();
			if (availableStudentCount > groupSizesSum)
			{
				_groupSizes.Add(availableStudentCount - groupSizesSum);
			}
		}

		/// <summary>
		/// This method the number of student sitting combinations
		/// </summary>
		/// <param name="groupSizes">Group sizes</param>
		/// <returns>Number of student sitting combinations</returns>
		public ulong GetLvCombinationCount()
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

			//Returning 0 if group sizes is empty or null
			if (_groupSizes == null || !_groupSizes.Any())
			{
				return 0;
			}

			//Checking if any of the groups have invalid size
			if (_groupSizes.Any(s => s <= 0))
			{
				throw new GroupSizeException();
			}

			ulong numberOfCombinations = 1;

			//Grouping groups based on group size
			int availableNumberCount = _groupSizes.Sum();
			var sameSizeGroups = _groupSizes.GroupBy(s => s).ToList();

			//Getting combination count for each group and multiplying them
			for (int i = 0; i < sameSizeGroups.Count; i++)
			{
				numberOfCombinations *= GetLvCombinationCountForSameSizeGroups(sameSizeGroups[i].Select(s => s).ToList(), availableNumberCount);
				availableNumberCount -= sameSizeGroups[i].Sum();
			}
			return numberOfCombinations;
		}

		private ulong GetLvCombinationCountForGroup(int groupSize, int availableNumberCount)
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
		private ulong GetLvCombinationCountForSameSizeGroups(IList<int> groupSizes, int availableNumberCount)
		{
			if (groupSizes.Count == 1)
			{
				return GetLvCombinationCountForGroup(groupSizes[0], availableNumberCount);
			}

			int minAvailableNumberCount = groupSizes.Sum();
			ulong numberOfCombinations = 0;
			var groupSizesWithoutFirst = groupSizes.Skip(1).ToList();

			//Getting all combinations until number of available numbers is the least it can be
			//Number of available numbers is sum of all group sizes. It is used so that duplicates are removed (if it went below this value, duplicate combinations would be returned)
			int groupSize = groupSizes[0] - 1;
			while (availableNumberCount >= minAvailableNumberCount)
			{
				ulong tempNumberOfCombinations = groupSize == 0 ? 1 : GetLvCombinationCountForGroup(groupSize, availableNumberCount - 1);
				tempNumberOfCombinations *= GetLvCombinationCountForSameSizeGroups(groupSizesWithoutFirst, availableNumberCount - groupSizes[0]);
				availableNumberCount--;
				numberOfCombinations += tempNumberOfCombinations;
			}
			return numberOfCombinations;
		}
	}
}