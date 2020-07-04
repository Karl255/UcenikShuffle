using System;
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
	public partial class MainWindow : Window
	{
		private Task _currentShuffleTask = null;
		private CancellationTokenSource _cancellationToken;

		public MainWindow() => InitializeComponent();

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
			for (int i = 0; i < shuffler.LvCount + 1; i++)
			{
				OutputGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			}

			// ---populating the grid---
			OutputGrid.AddTextAt("LV", 0, 0);

			// LVs
			for (int i = 1; i <= shuffler.LvCount; i++)
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
			int lvCount;
			
			bool isInt = int.TryParse(LvCountInput.Text, out lvCount);
			if (isInt == false)
			{
				throw new LvCountException();
			}


			//Parsing group sizes input
			var groupSizes = Parsers.StringToGroupSizes(GroupSizesInput.Text).ToList();

			//if the numbers are too large, ask user for confirmation
			int complexity = HelperMethods.GetShuffleComplexity(groupSizes, lvCount);
			if (complexity > 10000 || lvCount > 200)
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

			var progress = new Progress<double>(x =>
			{
				ProgressBar.Value = x;
				ProgressText.Text = $"{ Math.Round(x * 100, 2) }%";
			});

			//Shuffling
			var shuffler = new Shuffler(lvCount, groupSizes, _cancellationToken);
			_currentShuffleTask = Task.Factory.StartNew(() => shuffler.Shuffle(progress));

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
				for(int i = 0; i < shuffler.LvCount; i++)
				{
					var lvCombination = shuffler.ShuffleResult[i % shuffler.ShuffleResult.Count];
					int column = 1;
					for (int j = 0; j < lvCombination.Count; j++)
					{
						for (int k = 0; k < lvCombination[j].Count; k++)
						{
							OutputGrid.AddTextAt(lvCombination[j][k].Label, column, i + 1);
							column++;
						}
					}
				}
			}
		}
	}
}