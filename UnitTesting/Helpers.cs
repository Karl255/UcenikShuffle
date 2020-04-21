using System;
using System.Collections.Generic;
using System.Text;
using UcenikShuffle.ConsoleApp.Common;

namespace UcenikShuffle.UnitTests
{
	class Helpers
	{
		public static void ResetData()
		{
			Group.Groups.Clear();
			Group.History.Clear();
			Student.Students.Clear();
		}
	}
}