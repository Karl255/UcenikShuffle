using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UcenikShuffle.Common;

namespace UcenikShuffle.Gui
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
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
		}

		private void Button_Shuffle(object sender, RoutedEventArgs e)
		{
			//Parsing lv count input
			if (int.TryParse(LvCountInput.Text, out var lvCount) == false)
			{
				var message = "Broj laboratorijskih vježbi mora biti pozitivni cijeli broj!";
				MessageBox.Show(message, "Neispravno polje", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			
			//Parsing group sizes input
			char[] delimiters = { ' ', ',', ';' };
			var groupSizesText = GroupSizesInput.Text.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
			var groupSizes = new List<int>();
			foreach (var groupSize in groupSizesText)
			{
				//If group size input was in correct format
				if (int.TryParse(groupSize, out var size))
				{
					groupSizes.Add(size);
					continue;
				}
				
				//If group size input was in invalid format
				var message = "Veličine grupa moraju biti cijeli brojevi odvojeni zarezom ili razmakom\n\nPrimjeri:\n1,2,3\n1 2 3\n1, 2, 3";
				MessageBox.Show(message, "Neispravno polje", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			
			var shuffler = new Shuffler(lvCount, groupSizes);
			shuffler.Shuffle();
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