using System.Windows;
using System.Windows.Controls;

namespace UcenikShuffle.Gui
{
	static class Extensions
	{
		private static void AddElementAt(this Grid @this, UIElement element, int column, int row, int colspan = 1, int rowspan = 1)
		{
			@this.Children.Add(element);
			Grid.SetColumn(element, column);
			Grid.SetRow(element, row);
			Grid.SetColumnSpan(element, colspan);
			Grid.SetRowSpan(element, rowspan);
		}

		public static void AddTextAt(this Grid @this, string text, int column, int row, int colspan = 1, int rowspan = 1)
		{
			var tb = new TextBlock { Text = text };
			@this.AddElementAt(tb, column, row, colspan, rowspan);
		}
	}
}
