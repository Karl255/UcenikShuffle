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
		public ulong Complexity
		{
			get
			{
				if (_complexity == null)
				{
					_complexity = GetShuffleComplexity();
				}
				return (ulong)_complexity;
			}
		}

		private readonly List<int> _groupSizes;
		private readonly int _lvCount;
		private readonly int _maxCombinationCount;
		private ulong? _complexity = null;

		public ShuffleComplexityCalculator(IReadOnlyList<int> groupSizes, int lvCount, int maxCombinationCount)
		{
			_groupSizes = groupSizes.ToList();
			_lvCount = lvCount;
			_maxCombinationCount = maxCombinationCount;
		}

		private ulong GetShuffleComplexity()
		{
			ulong combinationCount = new LvCombinationCountCalculator(_groupSizes, _groupSizes.Sum()).GetLvCombinationCount();
			if(_maxCombinationCount <= 0)
			{
				return combinationCount * (ulong)_lvCount;
			}
			return (ulong)(Math.Sqrt(combinationCount) * 100) + (ulong)_maxCombinationCount * (ulong)_lvCount;
		}
	}
}