using System;
using System.Collections.Generic;

namespace UcenikShuffle.ConsoleApp
{
	public class ParseResult
	{
		public bool DetailedOutput = false;
		public string SaveFilePath = null;
		public string LoadFilePath = null;
		public int? LvCount = null;
		public int? Frequency = null;
		public DateTime? StartDate = null;
		public List<int> GroupSizes = null;
		public List<string> StudentLabels = new List<string>();
	}
}