using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UcenikShuffle.Common;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Gui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private Task _currentShuffleTask = null;
		private CancellationTokenSource _cancellationToken;
		private double _shuffleProgress;
		private string _shuffleCurrentStepText;
		private string _shuffleTimeLeftText;

		public event PropertyChangedEventHandler PropertyChanged = (o,e) => {};

		/// <summary>
		/// Shuffle progress (in percentages)
		/// </summary>
		public double ShuffleProgress
		{
			get => _shuffleProgress;
			set
			{
				_shuffleProgress = value;
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(ShuffleProgress)));
				PropertyChanged(this, new PropertyChangedEventArgs(nameof(ShuffleProgressText)));
			}
		}

		/// <summary>
		/// Shuffle progress (in percentages) in textual form
		/// </summary>
		public string ShuffleProgressText => $"{ Math.Round(ShuffleProgress * 100, 2) }%".ToString();

		/// <summary>
		/// Text holding information about current shuffle step
		/// </summary>
		public string ShuffleCurrentStepText 
		{
			get => _shuffleCurrentStepText;
			set 
			{
				_shuffleCurrentStepText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShuffleCurrentStepText)));
			}
		}

		/// <summary>
		/// Aproximate time left until current shuffle step is completed
		/// </summary>
		public string ShuffleTimeLeftText
		{
			get => _shuffleTimeLeftText;
			set
			{
				_shuffleTimeLeftText = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShuffleTimeLeftText)));
			}
		}

		public MainWindow()
		{
			DataContext = this;
			InitializeComponent();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => _cancellationToken?.Cancel();

		private void ResetOutputGrid(Shuffler shuffler)
		{
			OutputGrid.Children.Clear();
			OutputGrid.ColumnDefinitions.Clear();
			OutputGrid.RowDefinitions.Clear();

			// one column for LVs (size: auto)
			OutputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

			// columns for students and groups (size: *)
			for (int i = 0; i < shuffler.Students.Count; i++)
			{
				OutputGrid.ColumnDefinitions.Add(new ColumnDefinition() /*{ Width = new GridLength(1, GridUnitType.Star) }*/);
			}

			// rows for the header and LVs
			for (int i = 0; i < shuffler.ShuffleResult.Count + 1; i++)
			{
				OutputGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			}

			// ---populating the grid---
			OutputGrid.AddTextAt("LV", 0, 0);

			// LVs
			for (int i = 1; i <= shuffler.ShuffleResult.Count; i++)
			{
				OutputGrid.AddTextAt(i.ToString(), 0, i);
			}

			// group headers
			{
				int columnIndex = 1;

				for (int i = 0; i < shuffler.Groups.Count; i++)
				{
					OutputGrid.AddTextAt($"Grupa {i + 1}", columnIndex, 0, shuffler.Groups[i].Size);
					columnIndex += shuffler.Groups[i].Size;
				}
			}

		}

		private async void Button_Shuffle(object sender, RoutedEventArgs e)
		{
			if (_currentShuffleTask is null)
			{
				//if it's not running
				await Shuffle();
			}
			else
			{
				_currentShuffleTask = null;
				_cancellationToken?.Cancel();
				LoadingScreen.Visibility = Visibility.Collapsed;
			}

		}

		private async Task Shuffle()
		{
			//Cancelling currently running task
			if (_currentShuffleTask != null)
			{
				_cancellationToken.Cancel();
			}
			_cancellationToken = new CancellationTokenSource();

			//Parsing lv count input
			bool isInt = int.TryParse(LvCountInput.Text, out int lvCount);
			if (isInt == false)
			{
				throw new LvCountException();
			}

			//Parsing group sizes input
			var groupSizes = Parsers.StringToGroupSizes(GroupSizesInput.Text).ToList();

			//if the numbers are too large, ask user for confirmation
			ulong complexity = new ShuffleComplexityCalculator(groupSizes, lvCount, Shuffler.MaxCombinationCount).Complexity;
			if (complexity > 10000000)
			{
				var choice = MessageBox.Show(
					"S velikim brojevima proces može potrajati dosta dugo. Želite li nastaviti?",
					"Upozorenje",
					MessageBoxButton.YesNo,
					MessageBoxImage.Warning
				);

				//if anything but the Yes button was clicked
				if (choice != MessageBoxResult.Yes)
					return;
			}

			// ---start of shuffling procedure---

			//showing loading screen and cancel button while shuffling is in progress
			LoadingScreen.Visibility = Visibility.Visible;
			UniButton.Content = "Prekini";

			DateTime? lastProgressPercentageUpdate = null;
			var progressPercentage = new Progress<double>(p =>
			{
				//Making sure progress value doesn't change too often
				if(lastProgressPercentageUpdate != null && DateTime.Now.Subtract((DateTime)lastProgressPercentageUpdate).TotalMilliseconds < 250 && p != 0 && p != 1)
				{
					return;
				}
				lastProgressPercentageUpdate = (p == 0 || p == 1) ? null : (DateTime?)DateTime.Now;
				
				//Updating the value
				ShuffleProgress = p;
			});
			var progressText = new Progress<string>(t =>
			{
				ShuffleCurrentStepText = t;
			});
			DateTime? lastTimeLeftUpdate = null;
			var progressTimeLeft = new Progress<TimeSpan>(t =>
			{
				//Making sure time left value doesn't change too often
				if(lastTimeLeftUpdate != null && DateTime.Now.Subtract((DateTime)lastTimeLeftUpdate).TotalSeconds < 1 && t.Ticks != 0)
				{
					return;
				}
				lastTimeLeftUpdate = (t.Ticks == 0) ? null : (DateTime?)DateTime.Now;

				//Updating the value
				ShuffleTimeLeftText = $"Preostalo vrijeme: {((int)t.TotalHours):00}:{t.Minutes:00}:{t.Seconds:00}";
			});

			//Shuffling
			var shuffler = new Shuffler(lvCount, groupSizes, _cancellationToken);
			_currentShuffleTask = Task.Factory.StartNew(() => shuffler.Shuffle(progressPercentage, progressText, progressTimeLeft));

			try
			{
				await _currentShuffleTask;
			}
			catch (OperationCanceledException)
			{
				//if the shuffle has been canceled
				UniButton.Content = "Kreiraj raspored";
				return;
			}

			UniButton.Content = "Kreiraj raspored";
			CleanupShuffle(shuffler);
		}

		/// <summary>
		/// This method stops the currently running shuffle operation and cleans up after that
		/// </summary>
		public void StopShuffle()
		{
			_cancellationToken.Cancel();
			CleanupShuffle(null);
		}

		/// <summary>
		/// This method performs needed operations after the shuffle operation
		/// </summary>
		/// <param name="shuffler"></param>
		private void CleanupShuffle(Shuffler shuffler)
		{
			_currentShuffleTask = null;
			UniButton.Content = "Kreiraj raspored";
			LoadingScreen.Visibility = Visibility.Collapsed;
			if (shuffler == null)
				return;
			ResetOutputGrid(shuffler);

			//filling up the OutputGrid
			{
				//Outputting each student combination to the output grid
				for (int i = 0; i < shuffler.ShuffleResult.Count; i++)
				{
					var lvCombination = shuffler.ShuffleResult[i % shuffler.ShuffleResult.Count];
					int column = 1;
					for (int j = 0; j < lvCombination.Combination.Count; j++)
					{
						for (int k = 0; k < lvCombination.Combination[j].Count; k++)
						{
							OutputGrid.AddTextAt(lvCombination.Combination[j][k].Label, column, i + 1);
							column++;
						}
					}
				}
			}
		}
	}
}