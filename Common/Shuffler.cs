using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public class Shuffler
	{
		public int LvCount { get; private set; }
		public List<Group> Groups { get; private set; }
		public List<Student> Students { get; private set; }
		private readonly CancellationTokenSource _cancellationSource;
		private IProgress<double> _progress;
		public List<List<Student>> ShuffleResult { get; private set; }

		public Shuffler(int lvCount, IEnumerable<int> groupSizes, CancellationTokenSource cancellationSource)
		{
			_cancellationSource = cancellationSource;

			ShuffleResult = new List<List<Student>>();
			LvCount = lvCount;
			Groups = new List<Group>(groupSizes.Select(x => new Group(x)));

			//initializing Students
			int studentCount = groupSizes.Sum();
			Students = new List<Student>();

			for (int i = 0; i < studentCount; i++)
			{
				Students.Add(new Student { Id = i + 1 });
			}
		}
		public List<List<Student>> Shuffle(Progress<double> progress = null)
		{
			_progress = progress;

			//Going trough each LV
			_progress?.Report(0);
			for (int lv = 0; lv < LvCount; lv++)
			{
				//Getting the best student sitting combinations for this LV
				var combinations = HelperMethods.GetAllStudentCombinations(Groups, Students);

				//Getting best student sitting combination for current lv
				//student sitting diff is max - min difference in student sitting count (student sitting count is the amount of times a student sat with other students)
				IEnumerable<IEnumerable<Student>> bestCombination = null;
				bool firstCombination = true;
				int minMaxSittingCount = 0;
				int minMinSittingCountCount = 0;
				int minStudentSittingDiff = 0;
				int minMinMaxSum = 0;
				int maxMinSittingCount = 0;
				foreach (var combination in combinations)
				{
					int maxSittingCount = 0;
					int minSittingCountCount = 0;
					int studentSittingDiff = 0;
					int minMaxSum = 0;
					bool isBestCombination = false;
					
					_cancellationSource.Token.ThrowIfCancellationRequested();
					UpdateStudentHistory(combination, true);

					if (firstCombination == false)
					{
						//If max sitting count is lower or equal to the the lowest sitting count
						maxSittingCount = GetStudentSittingHistoryValues().Max();
						if (maxSittingCount < minMaxSittingCount)
						{
							isBestCombination = true;
						}
						else if (maxSittingCount == minMaxSittingCount)
						{
							//If min sitting count count is lower or equal to the lowest min sitting count count or if min sitting count is higher than the highest min sitting count
							var values = GetStudentSittingHistoryValues();
							int minSittingCount = values.Min();
							minSittingCountCount = values.Count(v => v == maxMinSittingCount);
							if (minSittingCount > maxMinSittingCount || (minSittingCount == maxMinSittingCount && minSittingCountCount < minMinSittingCountCount))
							{
								isBestCombination = true;
							}
							else if (minSittingCountCount == minMinSittingCountCount)
							{
								//If current student sitting diff is lower or equal to the lowest one
								studentSittingDiff = GetStudentSittingDiff(combination);
								if (studentSittingDiff < minStudentSittingDiff)
								{
									isBestCombination = true;
								}
								else if (studentSittingDiff == minStudentSittingDiff)
								{
									//Checking if minMaxDiff ((most times a student sat with other students - least times a student sat with other students)) sum for all students is better (lower) than the best (lowest) minMaxDiff sum for all students if current student sitting diff is the same as the best student sitting diff
									minMaxSum = GetMinMaxValues(combination).Sum();
									if (minMaxSum < minMinMaxSum)
									{
										isBestCombination = true;
									}
								}
							}
						}
					}

					//Updating variables if this is the best combination so far
					if (isBestCombination || firstCombination)
					{
						//Variables are being populated using new data since some of the variables in this foreach loop might not have been updated (since that depends on what criteria the best combination was selected)
						var studentSittingHistoryValues = GetStudentSittingHistoryValues().ToList();
						var tempMinSittingCount = studentSittingHistoryValues.Min();
						minMaxSittingCount = studentSittingHistoryValues.Max();
						minMinSittingCountCount = studentSittingHistoryValues.Where(v => v == tempMinSittingCount).Count();
						minStudentSittingDiff = GetStudentSittingDiff(combination);
						minMinMaxSum = GetMinMaxValues(combination).Sum();
						maxMinSittingCount = studentSittingHistoryValues.Min();
						bestCombination = combination;
						firstCombination = false;
					}

					UpdateStudentHistory(combination, false);
				}

				//Updating shuffle result
				UpdateStudentHistory(bestCombination, true);
				foreach (var groupCombination in bestCombination)
				{
					ShuffleResult.Add(groupCombination.ToList());
				}

				_progress?.Report((float)(lv + 1) / LvCount);
			}

			//DEBUGGING OUTPUT: used for testing purposes
			foreach (var student in Students)
			{
				Debug.WriteLine($"Student {student.Id}");
				foreach (var h in student.StudentSittingHistory.OrderBy(h => h.Key.Id))
				{
					Debug.WriteLine($"{h.Key.Id}: {h.Value}");
				}
				Debug.WriteLine("");
			}

			return ShuffleResult;
		}

		private void UpdateStudentHistory(IEnumerable<IEnumerable<Student>> students, bool increment)
		{
			foreach (var groupCombination in students)
			{
				foreach (var student in groupCombination)
				{
					foreach (var student2 in groupCombination.Where(s => s != student))
					{
						if (student.StudentSittingHistory.ContainsKey(student2) == false)
						{
							student.StudentSittingHistory.Add(student2, 0);
						}
						student.StudentSittingHistory[student2] += (increment) ? 1 : -1;
					}
				}
			}
		}
		private int GetStudentSittingDiff(IEnumerable<IEnumerable<Student>> combination)
		{
			int minCount = 0;
			int maxCount = 0;
			bool firstStudent = true;
			foreach (var groupCombination in combination)
			{
				foreach (var student in groupCombination)
				{
					int sittingCount = student.StudentSittingHistory.Values.Sum();
					if (sittingCount > maxCount)
					{
						maxCount = sittingCount;
					}
					if (sittingCount < minCount || firstStudent)
					{
						minCount = sittingCount;
						firstStudent = false;
					}
				}
			}
			return maxCount - minCount;
		}
		private IEnumerable<int> GetMinMaxValues(IEnumerable<IEnumerable<Student>> combination)
		{
			foreach (var group in combination)
			{
				foreach (var student in group)
				{
					int minMax = 0;
					if (student.StudentSittingHistory.Count != 0)
					{
						minMax = student.StudentSittingHistory.Values.Max();
						if (student.StudentSittingHistory.Count == Students.Count - 1)
						{
							minMax -= student.StudentSittingHistory.Values.Min();
						}
					}
					yield return minMax;
				}
			}
		}

		private IEnumerable<int> GetStudentSittingHistoryValues()
		{
			foreach(var student in Students)
			{
				//Returning student sitting history values for the current student
				foreach(var record in student.StudentSittingHistory)
				{
					yield return record.Value;
				}
				
				//Returning 0's for all students that aren't present in the student sitting history list
				for(int i = student.StudentSittingHistory.Count; i < Students.Count - 1; i++)
				{
					yield return 0;
				}
			}
		}
	}
}