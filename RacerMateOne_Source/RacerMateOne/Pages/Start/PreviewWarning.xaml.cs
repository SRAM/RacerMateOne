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
	public partial class PreviewWarning : Page
	{
		public PreviewWarning()
		{
			AppWin.SetPreviewMode();
			InitializeComponent();
		}

		//=============================================================
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help("DemoMode.htm");
		}
		private void Start_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(AppWin.Instance.Page_Splash);
		}

	}
}
