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

namespace RacerMateOne.Pages.Modes
{
	/// <summary>
	/// Interaction logic for Scanning.xaml
	/// </summary>
	public partial class Scanning : Page
	{
		public Scanning()
		{
			InitializeComponent();
		}
		private void Back_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}
	}
}
