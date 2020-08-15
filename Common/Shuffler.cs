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
			 An algorithm which calculates best combination takes 4 parameters into consideration:
			
			1st (most important) parameter is maxSittingCount:
			max amount of times a student sat with with another student. 
			Lower amount is better.
			
			2nd parameter is studentSittingDiff:
			(highest amount of times a student sat with another student) - (lowest amount a student sat with another student (doesn't need to be the same student)).
			Lower number is better since it means that student sitting amounts are more equal (ideally they would all be equal).
			
			3rd parameter is minSittingCountCount:
			sum of student sitting values where those values are equal to the least amount of times a student sat with another student.
			Lower is better (however, what's even better is if lowest amount of times a student sat with another student is higher than it was in the previous best combination)
			
			4th (least important) parameter is minMaxSum:
			sum of (max amount of times a student sat with another student) - (min amount of times that same student student sat with another student) for all students.
			Lower is better.

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
			if(combinationCount > (ulong)MaxCombinationCount)
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
		private LvCombination GetBestLvCombination(List<LvCombination> combinations,int currentLvIndex, IProgress<double> progressPercentage, IProgress<TimeSpan> progressTimeLeft)
		{
			LvCombination bestCombination = null;
			bool firstCombination = true;
			int minMaxSittingCount = 0;
			int minMinSittingCountCount = 0;
			int minStudentSittingDiff = 0;
			int minMinMaxSum = 0;
			int maxMinSittingCount = 0;
			int loopCount = 0;
			var timeEstimator = new TimeLeftEstimator();
			var combinationsLoopStopwatch = new Stopwatch();
			combinationsLoopStopwatch.Start();
			foreach (var combination in combinations)
			{
				_cancellationSource?.Token.ThrowIfCancellationRequested();

				UpdateStudentHistory(combination, true);
				var studentSittingHistoryValues = GetStudentSittingHistoryValues().ToList();
				bool isBestCombination = firstCombination || IsBestCombination(combination, studentSittingHistoryValues, minMaxSittingCount, minStudentSittingDiff, maxMinSittingCount, minMinSittingCountCount, minMinMaxSum);

				//Updating variables if this is the best combination so far
				if (isBestCombination)
				{
					//Combination will be skipped if it was already used
					bool skipCombination = false;
					foreach (var record in ShuffleResult)
					{
						if (combination.CompareTo(record))
						{
							skipCombination = true;
						}
					}

					//Variables are being populated using new data since some of the variables in this foreach loop might not have been updated (since that depends on what criteria the best combination was selected)
					if (skipCombination == false)
					{
						bestCombination = combination;
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
		private bool IsBestCombination(LvCombination combination, List<int> studentSittingHistoryValues, int minMaxSittingCount, int minStudentSittingDiff, int maxMinSittingCount, int minMinSittingCountCount, int minMinMaxSum)
		{
			bool isBestCombination = false;

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
			return isBestCombination;
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