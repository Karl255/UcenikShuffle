using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UcenikShuffle.Common;

namespace UcenikShuffle.Gui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Task _currentShuffleTask = null;
		private CancellationTokenSource _cancellationSource;

		public MainWindow()
		{
			InitializeComponent();
		}

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

			// groups
			{
				int columnIndex = 1;

				for (int i = 0; i < shuffler.Groups.Count; i++)
				{
					OutputGrid.AddTextAt($"Grupa {i + 1}", columnIndex, 0, shuffler.Groups[i].Size);
					columnIndex += shuffler.Groups[i].Size;
				}
			}

			LoadingScreenGrid.Visibility = Visibility.Hidden;
		}

		private async void Button_Shuffle(object sender, RoutedEventArgs e) => await Shuffle();

		private async Task Shuffle()
		{
			//Cancelling currently running task
			if (_currentShuffleTask != null)
			{
				_cancellationSource.Cancel();
			}
			_cancellationSource = new CancellationTokenSource();

			//Parsing lv count input
			if (int.TryParse(LvCountInput.Text, out var lvCount) == false)
			{
				var message = "Broj laboratorijskih vježbi mora biti pozitivni cijeli broj!";
				MessageBox.Show(message, "Neispravno polje", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			//Parsing group sizes input
			int[] groupSizes;
			try
			{
				groupSizes =
					GroupSizesInput.Text
					.Replace(" ", null) // remove spaces
					.Split(',', StringSplitOptions.RemoveEmptyEntries) //split by ','
					.Select(int.Parse) //convert each to int
					.ToArray();
			}
			catch
			{
				MessageBox.Show(
					"Veličine grupa moraju biti cijeli brojevi odvojeni zarezom\nPrimjeri:\n1,2,3,4,5\n1, 2, 3",
					"Neispravno polje",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);
				return;
			}

			//showing loading screen while shuffling is in progress
			LoadingScreenGrid.Visibility = Visibility.Visible;

			var progress = new Progress<float>();
			progress.ProgressChanged += (o, e) =>
			{
				ShuffleProgressBar.Value = e * 100;
			};

			//Shuffling
			var shuffler = new Shuffler(lvCount, groupSizes, _cancellationSource);
			_currentShuffleTask = Task.Factory.StartNew(() => shuffler.Shuffle(progress));
			try
			{
				await _currentShuffleTask;
			}
			catch
			{
				//Returning if the shuffle has been canceled
				return;
			}
			_currentShuffleTask = null;

			//Refreshing UI
			ResetOutputGrid(shuffler);

			{
				int x = 1;
				for (int i = 0; i < Group.History.Count; i++)
				{

					var set = Group.History[i].OrderBy(x => x.Id).ToArray();
					for (int j = 0; j < set.Length; j++)
					{
						OutputGrid.AddTextAt(set[j].Label, x % shuffler.Students.Count + j, x / shuffler.Students.Count + 1);
					}
					x += set.Length;
				}
			}
		}
	}
}