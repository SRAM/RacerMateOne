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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for LogoAbout.xaml
	/// </summary>
	public partial class LogoAbout : UserControl
	{
		public LogoAbout()
		{
			InitializeComponent();
		}
		private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
		{
			AppWin.Instance.About();
		}
	}
}
