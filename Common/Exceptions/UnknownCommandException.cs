using System;
using System.Collections.Generic;
using System.Text;

namespace UcenikShuffle.Common.Exceptions
{
	public class UnknownCommandException : Exception
	{
		public UnknownCommandException(string command) : base("Unknown command \""+ command + '"')
		{

		}
	}
}
