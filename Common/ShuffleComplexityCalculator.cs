using System;
using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle.Common
{
	public class ShuffleComplexityCalculator
	{
		/// <summary>
		/// Complexity of a shuffle operation 
		/// </summary>
		public int Complexity
		{
			get
			{
				if (_complexity == null)
				{
					_complexity = GetShuffleComplexity();
				}
				return (int)_complexity;
			}
		}

		private readonly List<int> _groupSizes;
		private readonly int _lvCount;
		private int? _complexity = null;

		public ShuffleComplexityCalculator(IReadOnlyList<int> groupSizes, int lvCount)
		{
			_groupSizes = groupSizes.ToList();
			_lvCount = lvCount;
		}

		private int GetShuffleComplexity()
		{
			int studentCount = _groupSizes.Sum();
			int points = 0;

			//Adding up complexity of each group
			foreach (int size in _groupSizes)
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
			return (int)(points * Math.Pow(_lvCount, 0.25));
		}
	}
}