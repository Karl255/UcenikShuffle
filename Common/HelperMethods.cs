using System;
using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle.Common
{
	public static class HelperMethods
	{
		/// <summary>
		/// This method returns all possible number combinations for a certain group size
		/// </summary>
		/// <param name="groupSize">Size of the group (for example, if there are 5 numbers and group size is 3, only 3 of those numbers would be able to fit in a certain group combination)</param>
		/// <param name="numberCount">Number of numbers for which all group combinations will be made (so if this parameter is 2 then every group combination can contain a maximum of one number 1 and one number 2 - no duplicates, no other numbers)</param>
		/// <returns></returns>
		public static IEnumerable<IEnumerable<int>> GetAllNumberCombinations(int groupSize, int numberCount)
		{
			var combination = new List<int>();
			
			//Checking if passed parameters are valid
			if (groupSize <= 0 || numberCount <= 0)
			{
				throw new ArgumentException("Group size and number count parameters must be positive integers!");
			}
			
			//If group size is bigger than the number of available numbers
			if (groupSize > numberCount)
			{
				yield return Enumerable.Range(0, numberCount);
			}
			
			//Initial combination
			combination.AddRange(Enumerable.Range(0, groupSize - 1));
            
			for(int startNumber = 0; startNumber + groupSize <= numberCount; startNumber++)
			{
				//Changing the last number until it hits max number
				for (int lastNumber = startNumber + groupSize - 1; lastNumber < numberCount; lastNumber++)
				{
					combination.Add(lastNumber);
					//Note: new list must be created otherwise collection modified exception might thrown
					//TODO: use the clone/copy method
					yield return new List<int>(combination);
					combination.Remove(lastNumber);
				}
				
				//Popping the first number and adding another number to the base combination instead of the popped number
				if (combination.Count > 0)
				{
					combination.RemoveAt(0);
					combination.Add(startNumber + groupSize - 1);
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