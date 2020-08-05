using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
		public List<List<List<Student>>> ShuffleResult { get; private set; }

		public Shuffler(int lvCount, List<int> groupSizes, CancellationTokenSource cancellationSource)
		{
			//Checking if passed parameters are valid 
			if (groupSizes == null || groupSizes.Count == 0 || groupSizes.Any(s => s <= 0))
			{
				throw new GroupSizeException();
			}
			if (lvCount <= 0)
			{
				throw new LvCountException();
			}

			////Initializing variables 
			_cancellationSource = cancellationSource;
			ShuffleResult = new List<List<List<Student>>>();
			LvCount = lvCount;
			
			groupSizes.Sort((g1, g2) => g1.CompareTo(g2));
			Groups = groupSizes.Select(x => new Group(x)).ToList();
			
			int studentCount = groupSizes.Sum();
			Students = new List<Student>();
			for (int i = 0; i < studentCount; i++)
			{
				Students.Add(new Student { Id = i + 1 });
			}
		}
		
		/// <summary>
		/// This method is used to create a student sitting combination based on the fields in this class
		/// </summary>
		/// <param name="progress">Object which holds data about shuffle progress</param>
		/// <returns></returns>
		public List<List<List<Student>>> Shuffle(Progress<double> progress = null)
		{
			//Clearing all current shuffle data before shuffling
			foreach (var student in Students)
			{
				student.StudentSittingHistory.Clear();
			}
			ShuffleResult.Clear();
			var combinations = HelperMethods.GetAllStudentCombinations(Groups.Select(g => g.Size).ToList(), Students, 100000).ToList();
			_progress = progress;

			//Going trough each LV
			_progress?.Report(0);
			for (int lv = 0; lv < LvCount; lv++)
			{
				//Getting best student sitting combination for current lv
				//student sitting diff is max - min difference in student sitting count (student sitting count is the amount of times a student sat with other students)
				List<List<Student>> bestCombination = null;
				bool firstCombination = true;
				int minMaxSittingCount = 0;
				int minMinSittingCountCount = 0;
				int minStudentSittingDiff = 0;
				int minMinMaxSum = 0;
				int maxMinSittingCount = 0;
				foreach (var combination in combinations)
				{
					bool isBestCombination = false;
					_cancellationSource?.Token.ThrowIfCancellationRequested();
					UpdateStudentHistory(combination, true);

					var studentSittingHistoryValues = GetStudentSittingHistoryValues().ToList();
					if (firstCombination == false)
					{
						//If max sitting count is lower or equal to the the lowest sitting count
						int maxSittingCount = studentSittingHistoryValues.Max();
						if (maxSittingCount < minMaxSittingCount)
						{
							isBestCombination = true;
						}
						else if (maxSittingCount == minMaxSittingCount)
						{
							//If current student sitting diff is lower or equal to the lowest one
							int studentSittingDiff = GetStudentSittingDiff(combination);
							if (studentSittingDiff < minStudentSittingDiff)
							{
								isBestCombination = true;
							}
							else if (studentSittingDiff == minStudentSittingDiff)
							{
								//If min sitting count count is lower or equal to the lowest min sitting count count or if min sitting count is higher than the highest min sitting count
								int minSittingCount = studentSittingHistoryValues.Min();
								int minSittingCountCount = studentSittingHistoryValues.Count(v => v == maxMinSittingCount);
								if (minSittingCount > maxMinSittingCount || (minSittingCount == maxMinSittingCount && minSittingCountCount < minMinSittingCountCount))
								{
									isBestCombination = true;
								}
								else if (minSittingCountCount == minMinSittingCountCount)
								{
									//Checking if minMaxDiff ((most times a student sat with other students - least times a student sat with other students)) sum for all students is better (lower) than the best (lowest) minMaxDiff sum for all students if current student sitting diff is the same as the best student sitting diff
									int minMaxSum = GetMinMaxValues(combination).Sum();
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
						var combinationList = combination.Select(g => g.ToList()).ToList();

						//Combination will be skipped if it was already used
						bool skipCombination = false;
						foreach(var record in ShuffleResult)
						{
							if(HelperMethods.CompareShuffleRecords(record, combinationList))
							{
								skipCombination = true;
							}
						}

						//Variables are being populated using new data since some of the variables in this foreach loop might not have been updated (since that depends on what criteria the best combination was selected)
						if (skipCombination == false)
						{
							bestCombination = combinationList;
							if (studentSittingHistoryValues.Count != 0)
							{
								int minStudentSittingHistoryValue = studentSittingHistoryValues.Min();
								minMaxSittingCount = studentSittingHistoryValues.Max();
								minMinSittingCountCount = studentSittingHistoryValues.Count(v => v == minStudentSittingHistoryValue);
								minStudentSittingDiff = GetStudentSittingDiff(combination);
								minMinMaxSum = GetMinMaxValues(combination).Sum();
								maxMinSittingCount = minStudentSittingHistoryValue;
								firstCombination = false;
							}
						}
					}

					UpdateStudentHistory(combination, false);
				}

				//Updating shuffle result and progress
				UpdateStudentHistory(bestCombination, true);
				ShuffleResult.Add(bestCombination);
				_progress?.Report((float) (lv + 1) / LvCount);
				
				//If every student sat with other students the same amount of times, there is no need to do further calculations since other lv's are just repeating
				if(GetStudentSittingHistoryValues().Distinct().Count() <= 1)
				{
					break;
				}
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

		private static void UpdateStudentHistory(IEnumerable<List<Student>> students, bool increment)
		{
			foreach (var groupCombination in students)
			{
				foreach (var student in groupCombination)
				{
					foreach (var student2 in groupCombination)
					{
						if (student == student2)
						{
							continue;
						}
						if (student.StudentSittingHistory.ContainsKey(student2) == false)
						{
							student.StudentSittingHistory.Add(student2, 0);
						}
						student.StudentSittingHistory[student2] += (increment) ? 1 : -1;
					}
				}
			}
		}
		private static int GetStudentSittingDiff(IEnumerable<IEnumerable<Student>> combination)
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