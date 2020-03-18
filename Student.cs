using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle
{
	public class Student
	{
		public int Id;
		public CustomDictionary<Student> StudentSittingHistory = new CustomDictionary<Student>();
		public CustomDictionary<Group> GroupSittingHistory = new CustomDictionary<Group>();
		public static List<Student> Students = new List<Student>(from id in Enumerable.Range(1, 13)
																 select new Student(id));

		public Student(int id)
		{
			Id = id;
		}

		/// <summary>
		/// This function searches for a student with the specified ID and returns his index
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