using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public class Group
	{
		public readonly int Size;

		public Group(int size)
		{
			Size = size;
		}
	}
}