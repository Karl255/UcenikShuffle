using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Common
{
	public class LvCombinationProcessor
	{
		public List<LvCombination> LvCombinations
		{
			get
			{
				if(_lvCombinations == null)
				{
					_lvCombinations = GetLvCombinations(_groupSizes, _students).ToList();
				}
				return _lvCombinations;
			}
		}

		private List<LvCombination> _lvCombinations;
		private List<int> _groupSizes;
		private List<Student> _students;
		private int _maxCombinationCount;

		public LvCombinationProcessor(List<int> groupSizes, List<Student> students, int maxCombinationCount = 0)
		{
			if (groupSizes == null || groupSizes.Count == 0 || groupSizes.Any(s => s <= 0))
			{
				throw new GroupSizeException();
			}
			if (students == null || students.Count <= 0)
			{
				throw new ArgumentException("Broj učenika mora biti pozitivni cijeli broj!");
			}
			_groupSizes = groupSizes;
			_students = students;
			_maxCombinationCount = maxCombinationCount;
		}

		/// <summary>
		/// This method returns all student sitting combinations for a lv
		/// </summary>
		/// <param name="takeOffset">Offset of the number which will be taken. For example, if every second combination is taken and offset is 3, following combinations will be returned: 5th, 7th, 9th, 11th...</param>
		/// <param name="takeEvery">Specifies which combinations will be returned. If there are 1000 combinations and <paramref name="maxCombinationCount"/> is 100, this parameter will be 10. That means that 10th, 20th, 30th etc. combinations will be returned</param>
		/// <returns></returns>
		private IEnumerable<LvCombination> GetLvCombinations(List<int> groupSizes, List<Student> students, double takeEvery = 0, double takeOffset = 0)
		{
			bool allGroupsAreSameSize = groupSizes.Distinct().Count() == 1;
			IEnumerable<LvCombination> firstGroupCombinations;
			var sameGroupSizes = groupSizes.Where(s => s == groupSizes[0]).ToList();

			//Setting takeEvery value if this is the first time this method was called
			if (takeEvery == 0 && takeOffset == 0)
			{
				takeEvery = (_maxCombinationCount <= 0) ? -1 : (double)new LvCombinationCountCalculator(groupSizes, groupSizes.Sum()).GetLvCombinationCount() / _maxCombinationCount;
				takeEvery = (takeEvery <= 1) ? -1 : takeEvery;
			}

			//Getting all number combinations for the first group
			if (groupSizes.Count > 1 && groupSizes[0] == groupSizes[1])
			{
				firstGroupCombinations = GetLvCombinationsForFirstGroup(groupSizes, students);
			}
			else
			{
				//If the second group doesn't have the same size as the first one, no fancy duplicate removal techniques are necessary   
				firstGroupCombinations = GetLvCombinationsForGroup(groupSizes[0], students).ToList();
			}

			//Returning all number combinations if there is only 1 number group
			//TODO: it might be possible to simplify the algorithm by putting this in the foreach loop below this if (maybe special treatment isn't even necessary)
			if (groupSizes.Count == 1)
			{
				foreach (var combination in firstGroupCombinations)
				{
					//Skipping the combination if necessary
					if (takeOffset > 0)
					{
						takeOffset--;
						continue;
					}

					takeOffset += takeEvery - 1;
					yield return new LvCombination(combination.Combination);
				}
				yield break;
			}

			//Going trough each combination for the first group and getting all possible combinations for other groups
			Student lastPivot = null;
			var availableStudents = new List<Student>(students);
			foreach (var firstGroupCombination in firstGroupCombinations)
			{
				//Removing some available students if the first student in the combination changed
				if (sameGroupSizes.Count > 1)
				{
					var pivot = firstGroupCombination.Combination[0][0];
					if (pivot != lastPivot)
					{
						lastPivot = pivot;
						availableStudents.Remove(pivot);
					}
				}

				//Skipping the combination if all combinations in this group would be skipped
				ulong combinationCount;
				ulong count1 = new LvCombinationCountCalculator(sameGroupSizes.Skip(1).ToList(), availableStudents.Count - groupSizes[0] + 1).GetLvCombinationCount();
				var tempGroups = groupSizes.Where(s => s != groupSizes[0]).ToList();
				ulong count2 = new LvCombinationCountCalculator(tempGroups, tempGroups.Sum()).GetLvCombinationCount();
				combinationCount = (count1 == 0 ? 1 : count1) * (count2 == 0 ? 1 : count2);
				if (takeOffset - combinationCount > 0)
				{
					takeOffset -= combinationCount;
					continue;
				}
				else
				{
					takeOffset -= combinationCount;
				}

				var tempGroupSizes = new List<int>(sameGroupSizes);

				//Getting all possible combinations for groups that have the same size as the first group
				if (sameGroupSizes.Count > 1)
				{
					var tempAvailableStudents = availableStudents.Except(firstGroupCombination.Combination[0]).ToList();
					tempGroupSizes.RemoveAt(0);

					var sameSizeCombinations = GetLvCombinations(tempGroupSizes, tempAvailableStudents, takeEvery, takeOffset + combinationCount);
					foreach (var sameSizeCombination in sameSizeCombinations)
					{
						var lvCombination = new LvCombination(new List<List<Student>>(firstGroupCombination.Combination));
						lvCombination.Combination.AddRange(sameSizeCombination.Combination);

						//Returning the combination if all groups have the same size
						if (allGroupsAreSameSize)
						{
							takeOffset += takeEvery;
							yield return lvCombination;
							continue;
						}

						//Getting all possible combinations for the groups that don't have the same size as the first group
						tempAvailableStudents = new List<Student>(students);
						tempAvailableStudents.RemoveAll(i => firstGroupCombination.Combination[0].Contains(i));
						foreach (var group in sameSizeCombination.Combination)
						{
							tempAvailableStudents.RemoveAll(i => group.Contains(i));
						}
						tempGroupSizes = groupSizes.Where(s => s != groupSizes[0]).ToList();
						var otherGroupsCombinations = GetLvCombinations(tempGroupSizes, tempAvailableStudents, takeEvery, takeOffset + combinationCount);
						foreach (var otherGroupsCombination in otherGroupsCombinations)
						{
							//Combining all of the group combinations into 1 combination
							otherGroupsCombination.Combination.InsertRange(0, lvCombination.Combination);
							takeOffset += takeEvery;
							yield return otherGroupsCombination;
						}
					}
				}
				else
				{
					//Getting number combinations for other groups
					var tempAvailableIndexes = availableStudents.Except(firstGroupCombination.Combination[0]).ToList();
					tempGroupSizes = groupSizes.Where(s => s != groupSizes[0]).ToList();
					var combinations = GetLvCombinations(tempGroupSizes, tempAvailableIndexes, takeEvery, takeOffset + combinationCount);
					foreach (var c in combinations)
					{
						//Combining all of the group combinations into 1 combination
						c.Combination.Insert(0, firstGroupCombination.Combination[0]);
						takeOffset += takeEvery;
						yield return c;
					}
				}
			}
		}

		private IEnumerable<LvCombination> GetLvCombinationsForGroup(int groupSize, List<Student> students)
		{
			var groupCombination = new List<Student>();

			//If group size is bigger than the number of available numbers or if group size is 1
			if (groupSize >= students.Count)
			{
				yield return new LvCombination(new List<List<Student>>() { students });
				yield break;
			}

			//If group size is 1
			if (groupSize == 1)
			{
				foreach (var student in students)
				{
					yield return new LvCombination(new List<List<Student>>() { new List<Student>() { student } });
				}
				yield break;
			}

			//Initial combination
			for (int i = 0; i < groupSize; i++)
			{
				groupCombination.Add(students[i]);
			}
			yield return new LvCombination(new List<List<Student>>() { new List<Student>(groupCombination) });

			//Getting all combinations
			while (true)
			{
				bool noMoreCombinations = true;
				for (int i = groupSize - 1; i >= 0; i--)
				{
					//lastStudent is the last possible student for this spot in the combination
					var lastStudent = students[i + students.Count - groupSize];
					if (groupCombination[i] != lastStudent)
					{
						noMoreCombinations = false;
						var newIndex = students.IndexOf(groupCombination[i]) + 1;
						groupCombination[i] = students[newIndex];
						for (int j = 1; j < groupSize - i; j++)
						{
							groupCombination[i + j] = students[newIndex + j];
						}
						yield return new LvCombination(new List<List<Student>>() { new List<Student>(groupCombination) }); ;
						break;
					}
				}

				//If all combinations have been tried out
				if (noMoreCombinations)
				{
					yield break;
				}
			}
		}
		private IEnumerable<LvCombination> GetLvCombinationsForFirstGroup(IList<int> groupSizes, IList<Student> students)
		{
			var sameGroupSizes = groupSizes.Where(s => s == groupSizes[0]).ToList();

			//If the second group has the same size as the first one than all combinations for the first group are calculated, and all combinations for other groups are calculated later on
			var tempStudents = new List<Student>(students);

			//Max first number is the max number that can be used for the first place in the first group (max number limit is set so that all duplicates are removed) 
			int maxFirstStudent = students.Count - sameGroupSizes.Sum() + 1;
			for (int i = 0; i < maxFirstStudent; i++)
			{
				var firstStudent = tempStudents[0];
				tempStudents.RemoveAt(0);
				var combinations = GetLvCombinationsForGroup(groupSizes[0] - 1, tempStudents).ToList();
				foreach (var combination in combinations)
				{
					combination.Combination[0].Insert(0, firstStudent);
					yield return combination;
				}
			}
		}		
	}
}