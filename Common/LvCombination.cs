using System;
using System.Collections.Generic;
using System.Linq;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public class LvCombination
	{
		/// <summary>
		/// Lv combination (list of group combinations)
		/// </summary>
		public List<List<Student>> Combination;

		public LvCombination(List<List<Student>> combination = null)
		{
			Combination = combination;
		}

		public bool CompareTo(LvCombination combination)
		{
			//Checking if parameters values are valid
			if (Combination == null || combination == null || combination.Combination == null)
			{
				throw new ArgumentException();
			}

			//If number of groups isn't the same for both records
			if (Combination.Count != combination.Combination.Count)
			{
				return false;
			}

			//Going trough each group in record 1, and checking if that group also exists in record 2
			var tempCombination = new List<List<Student>>(combination.Combination);
			for (int i = 0; i < Combination.Count; i++)
			{
				var group = tempCombination.FirstOrDefault(r => Combination[i].Count == r.Count && Combination[i].Except(r).Any() == false);
				if (group == null)
				{
					return false;
				}
				tempCombination.Remove(group);
			}
			return true;
		}
	}
}