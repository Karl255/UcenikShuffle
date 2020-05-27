using System.Diagnostics;
using UcenikShuffle.Common;
using Xunit;

namespace UcenikShuffle.UnitTests.CommonTests
{
    public class HelperMethodsTests
    {
        [Fact]
        private static void HelperMethods_GetAllNumberCombinations()
        {
            //TODO: right now this entire function is used just to see how the method works (testing isn't built in yet)
            var combinations = HelperMethods.GetAllNumberCombinations(10, 3);
            foreach (var combination in combinations)
            {
                foreach (var number in combination)
                {
                    Debug.Write(number + " ");
                }
                Debug.WriteLine("");
            }
        }
    }
}