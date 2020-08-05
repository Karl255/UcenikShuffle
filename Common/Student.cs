using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UcenikShuffle.Common
{
	[DebuggerDisplay("{Id}")]
	public class Student
	{
		public int Id;
		private string _label;
		public string Label
		{
			get => _label ?? Id.ToString();
			set => _label = value;
		}

		public readonly CustomDictionary<Student> StudentSittingHistory = new CustomDictionary<Student>();

		/// <summary>
		/// This method searches for a student with the specified ID and returns his index
		/// </summary>
		/// <param name="students">List of students to be searched</param>
		/// <param name="id">ID of the student whose index will be returned</param>
		/// <returns></returns>
		public static int GetIndexOfId(List<Student> students, int id)
		{
			var student = students.FirstOrDefault(s => s.Id == id);
			if (student == null)
			{
				return -1;
			}
			return students.IndexOf(student);
		}
	}
}