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

namespace RacerMateOne.Pages
{
	/// <summary>
	/// Interaction logic for Basic.xaml
	/// </summary>
	public partial class Credits : Page
	{
		public Credits()
		{
			InitializeComponent();
		}

		//=============================================================
		private void Options_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.RideOptions());
		}
		private void Back_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help();
		}
		private void Start_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}

	}
}
