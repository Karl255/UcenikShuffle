using System.Collections.Generic;

namespace UcenikShuffle.Common
{
	public class CustomDictionary<T1> : Dictionary<T1, int>
	{
		new public int this[T1 index]
		{
			get
			{
				bool containsValue = TryGetValue(index, out int val);
				if (containsValue == false)
				{
					this[index] = val = 0;
				}
				return val;
			}
			set
			{
				if (ContainsKey(index) == false)
				{
					Add(index, value);
				}
				else
				{
					Remove(index);
					Add(index, value);
				}
			}
		}
	}
}