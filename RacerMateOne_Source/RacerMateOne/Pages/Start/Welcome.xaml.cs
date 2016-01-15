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

namespace RacerMateOne.Pages.Start
{
	/// <summary>
	/// Interaction logic for Welcome.xaml
	/// </summary>
	public partial class Welcome : Page
	{
		public Welcome()
		{
			InitializeComponent();
            Log.Debug(string.Format("{0} - Welcome", DateTime.Now));
        }
		private void HomeUse_Click(object sender, RoutedEventArgs e)
		{
			RM1_Settings.General.Commercial = false;
			NavigationService.Navigate(new Pages.Start.FirstUsers());
		}

		private void Commercial_Click(object sender, RoutedEventArgs e)
		{
			RM1_Settings.General.Commercial = true;
			NavigationService.Navigate(new Pages.Start.ImportCSV());
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			System.Environment.Exit(0);		
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help("First_Use.htm");
		}

	}
}
