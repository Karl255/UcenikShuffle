using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle.Common
{
	public class Student
	{
		public int Id;
		private string _label;
		public string Label
		{
			get => _label ?? Id.ToString();
			set => _label = value;
		}

		public CustomDictionary<Student> StudentSittingHistory = new CustomDictionary<Student>();
		public CustomDictionary<Group> GroupSittingHistory = new CustomDictionary<Group>();

		/// <summary>
		/// This method searches for a student with the specified ID and returns his index
		/// </summary>
		/// <param name="students">List of students to be searched</param>
		/// <param name="id">ID of the student whose index will be returned</param>
		/// <returns></returns>
		public static int GetIndexOfId(List<Student> students, int id)
		{
			int? index = students.IndexOf(students.Where(s => s.Id == id).FirstOrDefault());
			return (index == null) ? -1 : (int)index;
		}
	}
}