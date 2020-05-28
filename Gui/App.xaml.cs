using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using UcenikShuffle.Common.Exceptions;

namespace UcenikShuffle.Gui
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			//Exception type string is necessary because switch can't take in argument of type Type 
			var exceptionTypeString = 
				e.Exception.InnerException == null ? 
				e.Exception.GetType().ToString() : 
				e.Exception.InnerException.GetType().ToString();
			exceptionTypeString = exceptionTypeString.Remove(0, exceptionTypeString.LastIndexOf('.') + 1);
			
			//Getting message box message and image based on what exception occured
			string message;
			MessageBoxImage image;
			switch (exceptionTypeString)
			{
				case nameof(GroupSizeParameterException):
					message = "Veličine grupa moraju biti pozitivni cijeli brojevi!";
					image = MessageBoxImage.Warning;
					break;
				case nameof(ArgumentException):
					message = "Došlo je do pogreške u kodu!";
					image = MessageBoxImage.Error;
					break;
				default:
					message = "Došlo je do neočekivane greške!";
					image = MessageBoxImage.Error;
					break;
			}
			
			//Showing the message box and exiting if a fatal error occured
			MessageBox.Show(message, "Greška", MessageBoxButton.OK, image);
			if (image == MessageBoxImage.Error)
			{
				Environment.Exit(0);
			}
			
			//Stopping current shuffle task
			(Current.MainWindow as MainWindow)?.StopShuffle();
			
			//Setting e.Handled to true because otherwise the program exits
			e.Handled = true;
		}
	}
}