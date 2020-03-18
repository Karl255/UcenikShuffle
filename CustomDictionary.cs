using System;
using System.Collections.Generic;
using System.Text;

namespace UcenikShuffle
{
    public class CustomDictionary : Dictionary<int, int>
    {
        new public int this [int index]
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