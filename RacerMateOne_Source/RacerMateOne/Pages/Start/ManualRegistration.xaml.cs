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
	/// Interaction logic for Basic.xaml
	/// </summary>
	public partial class ManualRegistration : Page
	{
		public ManualRegistration()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			// Fill in the Manual key here.
		}

		//=============================================================
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help();
		}

		private void t_Register_Click(object sender, RoutedEventArgs e)
		{

		}

		private void t_RegisterOffline_Click(object sender, RoutedEventArgs e)
		{

		}


	}
}
