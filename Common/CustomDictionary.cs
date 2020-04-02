using System.Collections.Generic;

namespace UcenikShuffle.ConsoleApp.Common
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
				if (this.ContainsKey(index) == false)
				{
					this.Add(index, value);
				}
				else
				{
					this.Remove(index);
					this.Add(index, value);
				}
			}
		}
	}
}