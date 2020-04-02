using System;
using System.Collections.Generic;
using System.Text;

namespace UcenikShuffle.Common.Exceptions
{
	public class InvalidCommandUsageException : Exception
	{
		public InvalidCommandUsageException(string command, string additionalInfo) : base($"Invalid command usage for command {command}! {additionalInfo}")
		{

		}
	}
}