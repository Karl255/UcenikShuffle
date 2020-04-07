using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UcenikShuffle.ConsoleApp.Common
{
	public class Student
	{
		public int Id;
		public string Label;
		public CustomDictionary<Student> StudentSittingHistory = new CustomDictionary<Student>();
		public CustomDictionary<Group> GroupSittingHistory = new CustomDictionary<Group>();
		public static ObservableCollection<Student> Students = new ObservableCollection<Student>();

		static Student()
		{
			Students.CollectionChanged += (o, e) => {
				//Ordering student Id's by their label
				var students = Students.Where(s => string.IsNullOrEmpty(s.Label) == false).ToList();
				int i;
				for (i = 0; i < students.Count; i++)
				{
					students[i].Id = i + 1;
				}
				students = Students.Where(s => string.IsNullOrEmpty(s.Label) == true).ToList();
				for (int j = 0; j < students.Count; j++)
				{
					students[j].Id = i + 1 + j;
				}
			};
		}

		public Student()
		{
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