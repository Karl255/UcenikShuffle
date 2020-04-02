using System;
using System.Collections.Generic;
using System.Text;

namespace UcenikShuffle.Common.Exceptions
{
	public class RequiredParameterNotUsedException : Exception
	{
		public RequiredParameterNotUsedException(string parameter) : base($"Required parameter (\"{parameter}\") wasn't used!")
		{

		}
	}
}
