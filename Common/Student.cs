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
				var students = (from s in Students
							   where string.IsNullOrEmpty(s.Label) == false
							   select s).ToList();
				for(int i = 0; i < students.Count; i++)
				{
					students[i].Id = i + 1;
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