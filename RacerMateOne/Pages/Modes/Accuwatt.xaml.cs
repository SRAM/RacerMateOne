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
	/// Interaction logic for Basic.xaml
	/// </summary>
	public partial class Accuwatt : Page
	{
		public Accuwatt()
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
			AppWin.Help("Accuwatt_Tips.htm");
		}
		private void Start_Click(object sender, RoutedEventArgs e)
		{
		}

		private void Label_MouseEnter(object sender, MouseEventArgs e)
		{
			ClickText.Foreground = Brushes.White; 
		}

		private void Label_MouseLeave(object sender, MouseEventArgs e)
		{
			ClickText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e3966"));
		}

		private void Border_MouseEnter(object sender, MouseEventArgs e)
		{
			ClickBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#76a7ca"));
		}

		private void Border_MouseLeave(object sender, MouseEventArgs e)
		{
			ClickBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
		}

		private void ClickBorder_MouseDown(object sender, MouseButtonEventArgs e)
		{
			AppWin.OpenURL("http://www.racermate.net/forum/viewforum.php?f=2");
		}

	}
}
