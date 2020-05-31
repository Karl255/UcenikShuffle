using System;
using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle.Common
{
	public static class Parsers
	{
		/// <summary>
		/// This method converts a string into group sizes
		/// </summary>
		/// <param name="value">String to be converted</param>
		/// <returns>Group sizes</returns>
		/// <exception cref="ArgumentException">Thrown if input is empty or if it is in incorrect format</exception>
		public static IEnumerable<int> StringToGroupSizes(string value)
		{
			//If input value is empty
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException();
			}
			
			//Splitting group sizes
			var groupSizes = value.Replace(" ", null) // remove spaces
				.Split(',', StringSplitOptions.RemoveEmptyEntries); //split by ','
			
			//Returning group sizes one by one
			foreach (var s in groupSizes)
			{
				//Checking if group size if a positive integer
				if (int.TryParse(s, out int size) == false)
				{
					throw new ArgumentException();
				}
				if (size <= 0)
				{
					throw new ArgumentException();
				}
				
				yield return size;
			}
		}

		/// <summary>
		/// This method converts a string into lv count
		/// </summary>
		/// <param name="value">Value to be converted to lv count</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown if input isn't a positive integer</exception>
		public static int StringToLvCount(string value)
		{
			//Checking if input value is empty
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException();
			}

			//Checking if input value is a positive integer 
			if (int.TryParse(value, out int lvCount) == false)
			{
				throw new ArgumentException();
			}
			if (lvCount <= 0)
			{
				throw new ArgumentException();
			}

			return lvCount;
		} 
	}
}