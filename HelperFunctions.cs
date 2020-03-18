using System;
using System.Collections.Generic;
using System.Text;

namespace UcenikShuffle
{
    public class HelperFunctions
    {
        public static IEnumerable<List<int>> GetAllNumberCombinations(int groupSize, int numberCount)
        {
            List<int> group = new List<int>();

            //Setting up the inital group
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
                    if (group[groupSize - counter] == numberCount + 1 - counter)
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
