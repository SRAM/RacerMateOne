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
	/// Interaction logic for SpinScan.xaml
	/// </summary>
	public partial class ComingSoon : Page
	{
		public RideOptionsAll Options = new RideOptionsAll();
		protected class TabInfo
		{
			public String[] Tags;
		}
		protected Controls.RTab SelectedTab = null;

		String m_HelpPage;

		public ComingSoon(String title,String helppage,String progressurl)
		{
			InitializeComponent();
			t_Title.Content = title;
			m_HelpPage = helppage;
			m_ProgressURL = progressurl;
		}

		private void ComingSoon_Loaded(object sender, RoutedEventArgs e)
		{
		}


		private void Start_Click(object sender, RoutedEventArgs e)
		{
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
			if (m_HelpPage != null)
				AppWin.Help(m_HelpPage);
			else
				AppWin.Help();
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

		String m_ProgressURL = null;
		private void ClickBorder_MouseDown(object sender, MouseButtonEventArgs e)
		{
			AppWin.OpenURL(m_ProgressURL == null ? "http://www.racermate.net/forum/viewforum.php?f=2":m_ProgressURL);
		}


	}
}

