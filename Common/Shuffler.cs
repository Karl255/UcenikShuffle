using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public class Shuffler
	{
		private readonly int _lvCount;
		private readonly CancellationTokenSource _cancellationSource;
		private readonly List<Group> _groups;
		private readonly List<Student> _students;
		private readonly List<LvCombination> _shuffleResult;

		/// <summary>
		/// Defaulf maximum for the amount of student combinations that will be returned by the shuffle operation
		/// </summary>
		public static readonly int MaxCombinationCount = 100000;

		/// <summary>
		/// List of groups
		/// </summary>
		public IReadOnlyList<Group> Groups => _groups;

		/// <summary>
		/// List of students
		/// </summary>
		public IReadOnlyList<Student> Students => _students;

		/// <summary>
		/// Result of the shuffle operation
		/// </summary>
		public IReadOnlyList<LvCombination> ShuffleResult => _shuffleResult;

		public Shuffler(int lvCount, IReadOnlyList<int> groupSizes, CancellationTokenSource cancellationSource)
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

			//Initializing variables 
			_cancellationSource = cancellationSource;
			_shuffleResult = new List<LvCombination>();
			_lvCount = lvCount;
			_groups = groupSizes.ToList().OrderBy(s => s).Select(s => new Group(s)).ToList();

			int studentCount = groupSizes.Sum();
			_students = new List<Student>();
			for (int i = 0; i < studentCount; i++)
			{
				_students.Add(new Student(i + 1));
			}
		}

		/// <summary>
		/// This method is used to create best student sitting combinations based on the fields in this class
		/// </summary>
		/// <param name="progressPercentage">Completed percentage for the shuffle step which is being executed at that moment</param>
		/// <param name="progressText">Text containing data about shuffle step which is being executed at that moment</param>
		/// <param name="progressTimeLeft">Estimated time left for shuffle step which is being executed at that moment to finish</param>
		/// <returns></returns>
		public void Shuffle(IProgress<double> progressPercentage = null, IProgress<string> progressText = null, IProgress<TimeSpan> progressTimeLeft = null)
		{
			/*BEST COMBINATION ALGORITHM EXPLANATION:
			 An algorithm which calculates best combination takes 6 parameters into consideration:
			
			1st (most important) parameter is maxSittingCount:
			max amount of times a student sat with with another student. 
			Lower amount is better.
			
			2nd parameter is minSittingCount:
			min amount of times a student sat with another student.
			Higher is better.

			3rd parameter is studentSittingDiff:
			(highest sum of student sitting values for a student) - (lowest sum of student sitting value for a student (doesn't need to be the same student)).
			Lower number is better since that forces the students to sit in groups of various sizes, instead of alwaays sitting in the biggest/smalled group.
			
			4th parameter is minMaxSum:
			sum of (max amount of times a student sat with another student) - (min amount of times that same student student sat with another student) for all students.
			Lower is better.

			5th parameter is minAndMaxSittingCountCount:
			count of student sitting count values (student sitting count values - amount of times students sat with each other) where student sitting count is min or max.
			Lower is better since it means that the deviation from average student sitting count is lower.

			6th (least important) parameter is groupRepetitionCount:
			Sum of amount of times each group in the combination was used in previous combinations

			Algorithm is pretty much doing 
			lvCombinations.OrderBy(1st parameter).OrderBy(2nd parameter).OrderBy(3nd parameter).OrderBy(4th parameter).First(), 
			but doing it manually since progress could otherwise only be updated once the best combination is found (which can take a lot of time)*/

			//Setup
			foreach (var student in Students)
			{
				student.StudentSittingHistory.Clear();
			}
			_shuffleResult.Clear();
			progressPercentage?.Report(0);
			progressText?.Report("Računanje svih kombinacija sjedenja");
			progressTimeLeft?.Report(new TimeSpan(0));

			ulong combinationCount = new LvCombinationCountCalculator(_groups.Select(g => g.Size).ToList(), _students.Count).GetLvCombinationCount();
			if (combinationCount > (ulong)MaxCombinationCount)
			{
				combinationCount = (ulong)MaxCombinationCount;
			}

			//Going trough each LV
			var combinations = GetLvCombinations(progressPercentage, progressTimeLeft, combinationCount);
			progressPercentage?.Report(0);
			progressText?.Report("Rasporeda se stvara");
			progressTimeLeft?.Report(new TimeSpan(0));
			for (int lv = 0; lv < _lvCount; lv++)
			{
				//Getting best student sitting combination for current lv
				LvCombination bestCombination = GetBestLvCombination(combinations, lv, progressPercentage, progressTimeLeft);

				//Updating shuffle result and progress
				UpdateStudentHistory(bestCombination, true);
				_shuffleResult.Add(bestCombination);

				//If every student sat with other students the same amount of times, there is no need to do further calculations since other lv's would just be repeats of current student sitting combinations
				if (GetStudentSittingHistoryValues().Distinct().Count() <= 1)
				{
					break;
				}
			}
			progressTimeLeft?.Report(new TimeSpan(0));

			//DEBUGGING OUTPUT: used for testing purposes
			Debug_PrintResult();
		}

		private static void UpdateStudentHistory(LvCombination combination, bool increment)
		{
			foreach (var groupCombination in combination.Combination)
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
		private static int GetStudentSittingDiff(LvCombination combination)
		{
			int minCount = 0;
			int maxCount = 0;
			bool firstStudent = true;
			foreach (var groupCombination in combination.Combination)
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
		private IEnumerable<int> GetMinMaxValues(LvCombination combination)
		{
			foreach (var group in combination.Combination)
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
		private int GetMinAndMaxSittingCountCount(IReadOnlyList<int> studentSittingHistoryValues) 
		{
			int minSittingCount = studentSittingHistoryValues.Min();
			int maxSittingCount = studentSittingHistoryValues.Max();
			return studentSittingHistoryValues.Where(v => v == minSittingCount || v == maxSittingCount).Count();
		}
		private int GetGroupRepetitionCount(LvCombination combination)
		{
			int count = 0;
			foreach (var group in combination.Combination)
			{
				foreach (var _combination in ShuffleResult)
				{
					foreach (var _group in _combination.Combination)
					{
						if (group.Count == _group.Count && group.Except(_group).Any() == false)
						{
							count++;
						}
					}
				}
			}
			return count;
		}
		private IEnumerable<int> GetStudentSittingHistoryValues()
		{
			foreach (var student in Students)
			{
				//Returning student sitting history values for the current student
				foreach (var record in student.StudentSittingHistory)
				{
					yield return record.Value;
				}

				//Returning 0's for all students that aren't present in the student sitting history list
				for (int i = student.StudentSittingHistory.Count; i < Students.Count - 1; i++)
				{
					yield return 0;
				}
			}
		}
		private List<LvCombination> GetLvCombinations(IProgress<double> progressPercentage, IProgress<TimeSpan> progressTimeLeft, ulong combinationCount)
		{
			var timeEstimator = new TimeLeftEstimator();
			var getCombinationsStopwatch = new Stopwatch();
			getCombinationsStopwatch.Start();
			ulong loopCount = 0;

			//Getting all lv combinations and updating user on progress of the operation
			var combinations =
				new LvCombinationProcessor(Groups.Select(g => g.Size).ToList(), Students.ToList(), MaxCombinationCount).LvCombinations
				.Select(c =>
				{
					_cancellationSource?.Token.ThrowIfCancellationRequested();
					loopCount++;

					//Progress is updated every 100 combinations
					if (loopCount % 100 == 0)
					{
						getCombinationsStopwatch.Stop();
						progressPercentage?.Report((double)loopCount / combinationCount);
						timeEstimator.AddTime(new TimeSpan(getCombinationsStopwatch.ElapsedTicks));
						progressTimeLeft?.Report(timeEstimator.GetTimeLeft((long)combinationCount - (long)loopCount) / 100);
						getCombinationsStopwatch.Restart();
					}
					return c;
				})
				.ToList();
			return combinations;
		}
		private LvCombination GetBestLvCombination(List<LvCombination> combinations, int currentLvIndex, IProgress<double> progressPercentage, IProgress<TimeSpan> progressTimeLeft)
		{
			LvCombination bestCombination = null;
			bool firstCombination = true;
			int bestCombinationMaxSittingCount = 0;
			int bestCombinationStudentSittingDiff = 0;
			int bestCombinationMinMaxSum = 0;
			int bestCombinationMinSittingCount = 0;
			int bestCombinationMinAndMaxSittingCountCount = 0;
			int bestCombinationGroupRepetitionCount = 0;
			int loopCount = 0;
			var timeEstimator = new TimeLeftEstimator();
			var combinationsLoopStopwatch = new Stopwatch();
			combinationsLoopStopwatch.Start();
			foreach (var combination in combinations)
			{
				_cancellationSource?.Token.ThrowIfCancellationRequested();

				UpdateStudentHistory(combination, true);
				var studentSittingHistoryValues = GetStudentSittingHistoryValues().ToList();
				bool isBestCombination = firstCombination || IsBestCombination(combination, studentSittingHistoryValues, bestCombinationMaxSittingCount, bestCombinationMinSittingCount, bestCombinationStudentSittingDiff, bestCombinationMinMaxSum, bestCombinationMinAndMaxSittingCountCount, bestCombinationGroupRepetitionCount);

				//Updating variables if this is the best combination so far
				if (isBestCombination)
				{
					//Variables are being populated using new data since some of the variables in this foreach loop might not have been updated (since that depends on what criteria the best combination was selected)
					bestCombination = combination;
					if (studentSittingHistoryValues.Count != 0)
					{
						bestCombinationMaxSittingCount = studentSittingHistoryValues.Max();
						bestCombinationMinSittingCount = studentSittingHistoryValues.Min();
						bestCombinationStudentSittingDiff = GetStudentSittingDiff(combination);
						bestCombinationMinMaxSum = GetMinMaxValues(combination).Sum();
						bestCombinationGroupRepetitionCount = GetGroupRepetitionCount(combination);
						bestCombinationMinAndMaxSittingCountCount = GetMinAndMaxSittingCountCount(studentSittingHistoryValues);
						firstCombination = false;
					}
				}

				UpdateStudentHistory(combination, false);

				loopCount++;
				if (loopCount % 1000 == 0 || loopCount == combinations.Count)
				{
					combinationsLoopStopwatch.Stop();
					timeEstimator?.AddTime(new TimeSpan(combinationsLoopStopwatch.ElapsedTicks));
					var timeLeft = timeEstimator.GetTimeLeft((combinations.Count - loopCount) / 1000 + (_lvCount - currentLvIndex - 1) * combinations.Count / 1000);
					progressTimeLeft?.Report(timeLeft);
					progressPercentage?.Report((double)((currentLvIndex + (double)loopCount / combinations.Count) / _lvCount));
					combinationsLoopStopwatch.Restart();
				}
			}
			return bestCombination;
		}
		private bool IsBestCombination(LvCombination combination, List<int> studentSittingHistoryValues, int bestCombinationMaxSittingCount, int bestCombinationMinSittingCount, int bestCombinationStudentSittingDiff, int bestCombinationMinMaxSum, int bestCombinationMinAndMaxSittingCountCount, int bestCombinationGroupRepetitionCount)
		{
			//Checking if max sitting count is better
			int maxSittingCount = studentSittingHistoryValues.Max();
			if (maxSittingCount < bestCombinationMaxSittingCount) return true;
			if(maxSittingCount > bestCombinationMaxSittingCount) return false;

			//Checking if min sitting count is better
			int minSittingCount = studentSittingHistoryValues.Min();
			if (minSittingCount > bestCombinationMinSittingCount) return true;
			else if (minSittingCount < bestCombinationMinSittingCount) return false;

			//Checking if min student sitting diff is better
			int studentSittingDiff = GetStudentSittingDiff(combination);
			if (studentSittingDiff < bestCombinationStudentSittingDiff) return true;
			else if (studentSittingDiff > bestCombinationStudentSittingDiff) return false;

			//Checking if minMaxDiff is better
			int minMaxSum = GetMinMaxValues(combination).Sum();
			if (minMaxSum < bestCombinationMinMaxSum) return true;
			else if(minMaxSum > bestCombinationMinMaxSum) return false;

			//Checking if minMinAndMax sitting count is better or if min sitting count is higher or if max sitting count is lower
			int minAndMaxSittingCountCount = GetMinAndMaxSittingCountCount(studentSittingHistoryValues);
			if (bestCombinationMinAndMaxSittingCountCount < minAndMaxSittingCountCount) return true;
			else if (bestCombinationMinAndMaxSittingCountCount > minAndMaxSittingCountCount) return false;

			//Checking is group repetition count is better
			int groupRepetitionCount = GetGroupRepetitionCount(combination);
			if (groupRepetitionCount < bestCombinationGroupRepetitionCount) return true;
			return false;
		}
		private void Debug_PrintResult()
		{
			foreach (var student in Students)
			{
				Debug.WriteLine($"Student {student.Id}");
				foreach (var h in student.StudentSittingHistory.OrderBy(h => h.Key.Id))
				{
					Debug.WriteLine($"{h.Key.Id}: {h.Value}");
				}
				Debug.WriteLine("");
			}
		}
	}
}