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
        public static IEnumerable<List<int>> GetAllNumberCombinations(int groupSize, int numberCount)
        {
            int startNumber = 0;
            var combination = new List<int>();
			
            //Initial combination
            for (int i = 0; i < groupSize - 1; i++)
            {
                combination.Add(i);
            }

			//TODO: convert this into a for loop
            while (startNumber + groupSize <= numberCount)
            {
                //Changing the last number until it hits max number
                for (int i = startNumber + groupSize - 1; i < numberCount; i++)
                {
                    combination.Add(i);
                    //Note: new list must be created otherwise collection modified exception might thrown
					//TODO: use the clone/copy method
                    yield return new List<int>(combination);
                    combination.Remove(i);
                }
				
                //Popping the first number and adding another number to the base combination instead of the popped number
                //TODO: switch to using Length instead of Linq's slower Any() method
				if (combination.Any())
                {
                    combination.RemoveAt(0);
					combination.Add(startNumber + groupSize - 1);
                }

                startNumber++;
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
				//TODO: reverse loop to go in the *normal* direction
				//      it's much easier to understand the loop that way
                int combinations = 0;
                for (int i = studentCount - size + 1; i > 0; i--)
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
