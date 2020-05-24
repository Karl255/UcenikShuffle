using System.Collections.Generic;

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
		public static IEnumerable<List<int>> GetAllNumberCombinations(int groupSize, int numberCount)
		{
			var group = new List<int>();

			//Setting up the initial group
			for (int i = 0; i < groupSize; i++)
			{
				group.Add(i);
			}
			yield return new List<int>(group);

			//Going trough all combinations
			while (true)
			{
				//Checking how many numbers need to be shifted
				int counter = 1;
				while (true)
				{
					//Shifting the current number
					group[groupSize - counter]++;

					//If current number is at it's maximum position, the previous number in the group will also be shifted
					if (group[groupSize - counter] >= numberCount + 1 - counter)
					{
						counter++;

						//If all combinations were tried out
						if (counter > groupSize)
						{
							yield break;
						}
					}

					//If no more numbers need to be shifted before the current one
					else
					{
						break;
					}
				}

				//Shifting numbers that need to be shifted (the first number that needed to be shifted was already shifter in the while loop, that's why there is +1 in for loop)
				int beginningNumberIndex = groupSize - counter;
				for (int i = beginningNumberIndex + 1; i < groupSize; i++)
				{
					group[i] = group[beginningNumberIndex] + i - beginningNumberIndex;
				}
				yield return new List<int>(group);
			}
		}
	}
}