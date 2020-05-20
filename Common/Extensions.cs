using System;
using System.Collections.Generic;

namespace UcenikShuffle.Common
{
	public static class Extensions
	{
		public static bool Contains<T>(this IEnumerable<T> collection, T value, Func<T, T, bool> comparer)
		{
			foreach (var item in collection)
			{
				if (comparer(item, value))
				{
					return true;
				}
			}

			return false;
		}
	}
}