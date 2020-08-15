using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcenikShuffle.Common
{
	/// <summary>
	/// Calculates estimated amount of time for an operation to finish
	/// </summary>
	public class TimeLeftEstimator
	{
		/// <summary>
		/// List of times that are taken into consideration when calculating estimated time left
		/// </summary>
		public IReadOnlyList<TimeSpan> Times => _times;

		private readonly List<TimeSpan> _times = new List<TimeSpan>();
		private readonly int _maxTimes;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxTimes">Max amount of times that can be held in <see cref="Times"/> list. If there are too many times, those at the beginning of the list get deleted</param>
		public TimeLeftEstimator(int maxTimes = 100)
		{
			_maxTimes = maxTimes;
		}

		/// <summary>
		/// Adds <see cref="TimeSpan"/> item to <see cref="Times"/> list
		/// <paramref name="time"/>Item to be added to <see cref="Times"/> list<paramref name="time"/>
		/// </summary>
		public void AddTime(TimeSpan time)
		{
			while (_times.Count >= _maxTimes)
			{
				_times.RemoveAt(0);
			}

			_times.Add(time);
		}

		/// <summary>
		/// Returns estimated time left
		/// <paramref name="loopsLeft">Amount of times an operation has to loop to finish<paramref name="loopsLeft"/>
		/// </summary>
		public TimeSpan GetTimeLeft(long loopsLeft) => new TimeSpan((long)Times.Average(t => t.Ticks) * loopsLeft);
	}
}
