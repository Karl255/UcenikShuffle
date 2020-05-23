using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

		private void ResetOutputGrid(int[] groupSizes, int lvCount)
		{
			OutputGrid.Children.Clear();
			OutputGrid.ColumnDefinitions.Clear();
			OutputGrid.RowDefinitions.Clear();

			int studentCount = groupSizes.Sum();

			// one column for LVs (size: auto)
			OutputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

			// columns for students and groups (size: *)
			for (int i = 0; i < studentCount; i++)
			{
				OutputGrid.ColumnDefinitions.Add(new ColumnDefinition() /*{ Width = new GridLength(1, GridUnitType.Star) }*/);
			}

			// rows for the header and LVs
			for (int i = 0; i < lvCount + 1; i++)
			{
				OutputGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			}

			// ---populating the grid---
			OutputGrid.AddTextAt("LV", 0, 0);

			// LVs
			for (int i = 1; i <= lvCount; i++)
			{
				OutputGrid.AddTextAt(i.ToString(), 0, i);
			}

			// groups
			{
				int columnIndex = 1;

				for (int i = 0; i < groupSizes.Length; i++)
				{
					OutputGrid.AddTextAt($"Grupa {i + 1}", columnIndex, 0, groupSizes[i]);
					columnIndex += groupSizes[i];
				}
			}
		}

		private void Button_Shuffle(object sender, RoutedEventArgs e) => Shuffle();
		
		private void Shuffle()	
		{
			//TODO: add input validation
			int lvCount = int.Parse(LvCountInput.Text);

			var shuffler = new Shuffler(lvCount);

			var groupSizes = GroupSizesInput.Text.Replace(" ", null).Split(',').Select(int.Parse).ToArray();

			foreach (var size in groupSizes)
			{
				shuffler.Groups.Add(new Group(size));
			}

			var studentCount = groupSizes.Sum();

			for (int i = 0; i < studentCount; i++)
			{
				shuffler.Students.Add(new Student { Id = i + 1 });
			}

			shuffler.Shuffle();

			ResetOutputGrid(groupSizes, lvCount);
			{
				int x = 1;
				for (int i = 0; i < Group.History.Count; i++)
				{

					var set = Group.History[i].OrderBy(x => x.Id).ToArray();
					for (int j = 0; j < set.Length; j++)
					{
						OutputGrid.AddTextAt(set[j].Label, x % studentCount + j, x / studentCount + 1);
					}
					x += set.Length;
				}
			}
		}
	}
}
