using System;
using System.Collections.Generic;
using System.Linq;

namespace UcenikShuffle
{
    class Program
    {
        static void Main()
        {
            int lvCount = 14;
            Group.CreateGroupsForLvs(lvCount);
            PrintResult();
        }

        static void PrintResult()
        {
            int lvCount = Group.History.Count / Group.Groups.Count;

            for (int i = 0; i < lvCount; i++)
            {
                Console.WriteLine($"LV {i + 1}");
                int beginningJ = i * Group.Groups.Count;
                for (int j = beginningJ; j < beginningJ + Group.Groups.Count; j++)
                {
                    foreach (var student in Group.History[j])
                    {
                        Console.Write($"{student.Id} ");
                    }
                    Console.Write("| ");
                }
                Console.WriteLine("\n");
            }

            for (int i = 0; i < Student.Students.Count; i++)
            {
                Console.WriteLine($"Ucenik { i + 1 }");

                var satWith = Student.Students[i].StudentSittingHistory.OrderBy(x => x.Key.Id);

                foreach (var otherStudent in satWith)
                {
                    Console.WriteLine($"{ otherStudent.Key.Id + 1 } { (int)otherStudent.Value }");
                }
                Console.WriteLine();
            }
        }
    }
}