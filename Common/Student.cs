using System.Collections.Generic;
using System.Diagnostics;

namespace UcenikShuffle.Common
{
	[DebuggerDisplay("{Id}")]
	public class Student
	{
		/// <summary>
		/// Id of the student
		/// </summary>
		public readonly int Id;

		/// <summary>
		/// Label of the student which will be used when displaying information about the student (if it's not set, Id will be used instead)
		/// </summary>
		public string Label
		{
			get => _label ?? Id.ToString();
			set => _label = value;
		}

		/// <summary>
		/// List which contains information about the amount of times this student sat with other students
		/// </summary>
		public readonly Dictionary<Student,int> StudentSittingHistory = new Dictionary<Student, int>();

		private string _label;

		public Student(int id)
		{
			Id = id;
		}
	}
}