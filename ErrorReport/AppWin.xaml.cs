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
using System.IO;

namespace ErrorReport
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class AppWin : Window
	{
		public AppWin()
		{
			InitializeComponent();
			string[] args = Environment.GetCommandLineArgs();
			MessageBox.Show(args[1].ToString());
			using (StreamReader reader = File.OpenText(args[1]))
			{
				Report.Text = reader.ReadToEnd();
			}
		}

		private void Button_Send(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Cancel(object sender, RoutedEventArgs e)
		{
			System.Environment.Exit(0);
		}
	}
}
