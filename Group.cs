using System;
using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle
{
    public class Group
    {
        //All groups on laboratory exercises (should be changed if calculations are needed for another situation)
        public static List<Group> Groups = new List<Group>
        {
            new Group(0, 1),
            new Group(1, 3),
            new Group(2, 3),
            new Group(3, 3),
            new Group(4, 3)
        };
        public static List<HashSet<int>> History = new List<HashSet<int>>();
        public int Id;
        public int Size;

        public Group(int id, int size)
        {
            Id = id;
            Size = size;
        }

        /// <summary>
        /// This functions tries to find the best possible combination of students to put in the same group for a laboratory work
        /// </summary>
        /// <param name="studentPool">List of all avaliable students (the ones that are in other groups for the current laborarory work should be excluded)</param>
        /// <returns>List of all avaliable students (after removing those who were chosen for the current group)</returns>
        public List<Student> AddStudents(List<Student> studentPool)
        {
            //Getting the student that sat the least ammount of times in the current group
            studentPool = studentPool.OrderBy(x => x.GroupSittingHistory[Id]).ToList();

            //Getting all combinations for a group and ordering them from the best combination to worst
            var combinations = HelperFunctions.GetAllNumberCombinations(Size, studentPool.Count).ToList();
            combinations = (from combination in combinations
                            //Ordering by ammount of times the current student sat with other students
                            orderby (from index in combination
                                         //Getting the ammount of times students in a group sat with each other
                                     select (from history in studentPool[index].StudentSittingHistory
                                             where combination.Contains(Student.GetIndexOfId(studentPool, (int)history.Key))
                                             select (int)history.Value).Sum()).Sum(),
                                             //Ordering by group sitting history
                                             (from index in combination
                                              select index).Sum()
                            select combination).ToList();

            //Going trough all group combinations
            HashSet<int> newEntry = null;
            foreach (var combination in combinations)
            {
                newEntry = new HashSet<int>(combination.Select(x => studentPool[x].Id));

                //Checking if current group combination is unique (exiting the loop if that's the case)
                if (History.Contains(newEntry, (h1, h2) => h1.Count == h2.Count && !h1.Except(h2).Any()) == false)
                {
                    break;
                }
            }

            //If all groups have been tried out
            if (newEntry == null)
            {
                newEntry = new HashSet<int>(combinations.First().Select(x => studentPool[x].Id));
            }

            //Updating histories of individual students
            foreach (var stud1 in newEntry)
            {
                foreach (var stud2 in newEntry)
                {
                    if (stud1 == stud2)
                        continue;

                    var studentSittingHistory = studentPool[Student.GetIndexOfId(studentPool, stud1)].StudentSittingHistory;
                    studentSittingHistory[stud2] = (int)studentSittingHistory[stud2] + 1;
                }
                var groupSittingHistory = studentPool[Student.GetIndexOfId(studentPool, stud1)].GroupSittingHistory;
                groupSittingHistory[Id] = ((int)groupSittingHistory[Id]) + 1;
            }

            //Updating history for the current group
            History.Add(new HashSet<int>(newEntry));

            //Removing students in the chosen group from the result
            foreach (var studentId in newEntry)
            {
                studentPool.RemoveAt(Student.GetIndexOfId(studentPool, studentId));
            }

            return studentPool;
        }

        /// <summary>
        /// This function creates the groups for the LV based on the <see cref="Group.Groups"/> and <see cref="Student.Students"/> variables
        /// </summary>
        public static void CreateGroupsForLvs(int lvCount)
        {
            //Going trough each laboratory exercise (lv)
            for (int lv = 0; lv < lvCount; lv++)
            {
                var studentPool = new List<Student>(Student.Students);
                for (int i = 0; i < Group.Groups.Count; i++)
                {
                    studentPool = Group.Groups[i].AddStudents(studentPool);
                }
            }
        }
    }
}