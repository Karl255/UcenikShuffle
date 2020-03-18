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
        public int Id;
        public int Size;
        public static List<HashSet<int>> History = new List<HashSet<int>>();

        public Group(int id, int size)
        {
            Id = id;
            Size = size;
        }

        public void AddStudents(ref List<Student> studentPool)
        {
            //Getting the student that sat the least ammount of times in the current group
            studentPool = studentPool.OrderBy(x => x.GroupSittingHistory[Id]).ToList();
            var studentPoolCopy = studentPool.ToList();

            //Getting all combinations for a group and ordering them from the best combination to worst
            var combinations = HelperFunctions.GetAllNumberCombinations(Size, studentPool.Count).ToList();
            combinations = (from combination in combinations
                            //Ordering by ammount of times the current student sat with other students
                            orderby (from index in combination
                                         //Getting the ammount of times students in a group sat with each other
                                     select (from history in studentPoolCopy[index].StudentSittingHistory
                                             where combination.Contains(Student.GetIndexOfId(studentPoolCopy, history.Key))
                                             select history.Value).Sum()).Sum(),
                                             //Ordering by group sitting history
                                             (from index in combination
                                              select index).Sum()
                            select combination).ToList();

            //Going trough all group combinations
            HashSet<int> newEntry = null;
            foreach (var combination in combinations)
            {
                newEntry = new HashSet<int>(combination.Select(x => studentPoolCopy[x].Id));

                //Checking if current group combination is unique (exiting the loop if that's the case)
                if (History.Contains(newEntry, (h1, h2) => h1.Count == h2.Count && !h1.Except(h2).Any()) == false)
                {
                    foreach (var studentId in newEntry)
                    {
                        studentPool.RemoveAt(Student.GetIndexOfId(studentPool, studentId));
                    }
                    break;
                }
            }

            //If all groups have been tried out
            if (newEntry == null)
            {
                newEntry = new HashSet<int>(combinations.First().Select(x => studentPoolCopy[x].Id));
            }

            //Updating histories of individual students
            foreach (var stud1 in newEntry)
            {
                foreach (var stud2 in newEntry)
                {
                    if (stud1 == stud2)
                        continue;

                    studentPoolCopy[Student.GetIndexOfId(studentPoolCopy, stud1)].StudentSittingHistory[stud2]++;
                }
                studentPoolCopy[Student.GetIndexOfId(studentPoolCopy, stud1)].GroupSittingHistory[Id]++;
            }

            //Updating history for the current group
            History.Add(new HashSet<int>(newEntry));
        }
    }
}