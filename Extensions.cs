using System;
using System.Collections.Generic;

namespace UcenikShuffle
{
	public static class Extensions
	{
		public static T Pop<T>(this List<T> collection, int index)
		{
			T toPop = collection[index];
			collection.RemoveAt(index);
			return toPop;
		}

		public static void Add<T>(this IList<T> thisCollection, IEnumerable<T> collectionToAdd)
		{
			foreach (var item in collectionToAdd)
			{
				thisCollection.Add(item);
			}
		}

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
