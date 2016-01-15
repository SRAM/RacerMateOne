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
using System.Windows.Shapes;

namespace RacerMateOne
{
	/// <summary>
	/// Interaction logic for Window_Splash.xaml
	/// </summary>
	public partial class Window_Splash : Window
	{
		public Window_Splash()
		{
			InitializeComponent();
		}
		private void FormFadeIn_Completed(object sender, EventArgs e)
		{
			Checking.Visibility = Visibility.Visible;
			CheckingAnim.Begin();
		}
		private void FormFadeOut_Completed(object sender, EventArgs e)
		{
			CheckingAnim.Stop();
			Checking.Visibility = Visibility.Hidden;
			this.Close();
		}

		private void winSplash_Loaded(object sender, RoutedEventArgs e)
		{
		}
	}
}
