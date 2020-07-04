using System;
using System.Collections.Generic;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public static class Parsers
	{
		/// <summary>
		/// This method converts a string into group sizes
		/// </summary>
		/// <param name="value">String to be converted</param>
		/// <returns>Group sizes</returns>
		/// <exception cref="GroupSizeException">Thrown if groups size not a positive integer</exception>
		public static IEnumerable<int> StringToGroupSizes(string value)
		{
			//If input value is empty
			if (string.IsNullOrEmpty(value))
			{
				throw new GroupSizeException();
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
					throw new GroupSizeException();
				}
				if (size <= 0)
				{
					throw new GroupSizeException();
				}

				yield return size;
			}
		}
	}
}