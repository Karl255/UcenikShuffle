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
            while (true)
            {
                //If all combinations were returned
                if (startNumber + groupSize > numberCount)
                {
                    break;
                }
				
                //Changing the last number until it hits max number
                for (int i = startNumber + groupSize - 1; i < numberCount; i++)
                {
                    combination.Add(i);
                    //Note: new list must be created otherwise collection modified exception might thrown
                    yield return new List<int>(combination);
                    combination.Remove(i);
                }
				
                //Popping the first number and adding another number to the base combination instead of the popped number
                if (combination.Any())
                {
                    combination.RemoveAt(0);
                    combination.Add(startNumber + groupSize - 1);
                }
                startNumber++;
            }
        }
    }
}