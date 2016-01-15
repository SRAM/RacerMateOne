using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// Dan added these:
using System.Diagnostics;       // Needed for process invocation
using Microsoft.Win32;
using System.ComponentModel; // CancelEventArgs
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Windows.Threading;
using System.Threading;


namespace RacerMateOne.Pages.Start
{
	/// <summary>
	/// Interaction logic for ImportCSV.xaml
	/// </summary>
	public partial class ImportCSV : Page
	{
		public ImportCSV()
		{
			InitializeComponent();
		}
		private void ToOptions()
		{
			// Done with first run.
			if (RM1_Settings.gFirstRun)
			{
				RM1_Settings.gFirstRun = false;
				RM1_Settings.SaveToFile();
			}

			NavigationService.Navigate(new Pages.Selection());
			//NavigationService.Navigate(new Pages.RideOptions());
			/*
			//NavigationService.Navigate( new 
			Log.WriteLine("Rider Options Screen", true);
			Window_Configuration windowConfiguration = new Window_Configuration();
			windowConfiguration.Owner = AppWin.Instance;
			windowConfiguration.Left = AppWin.Instance.Left + 28;
			windowConfiguration.Top = AppWin.Instance.Top + 28;
			windowConfiguration.ShowRidersTab();

			windowConfiguration.ShowDialog(); //show as modal
			 */
		}
		private void Yes_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog f = new OpenFileDialog();
			f.InitialDirectory = RacerMatePaths.SettingsFullPath;
			f.Multiselect = false;
			f.Title = "Select a Riders.csv to import";
			f.ValidateNames = true;
			f.AddExtension = true;
			f.CheckFileExists = true;
			f.DefaultExt = ".csv";
			f.Filter = "CSV Files (.csv)|Riders*.csv";
			if (f.ShowDialog() == true)
			{
				Log.WriteLine("Importing \"" + f.FileName + "\"");
			}
			ToOptions();
		}

		private void No_Click(object sender, RoutedEventArgs e)
		{
			ToOptions();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Exit();
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help();
		}
	}
}
