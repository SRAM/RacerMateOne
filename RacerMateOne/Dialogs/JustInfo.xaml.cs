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

namespace RacerMateOne.Dialogs
{
	/// <summary>
	/// Interaction logic for TextLine.xaml
	/// </summary>
	public partial class JustInfo : Window
	{
		bool m_IsOK = false;
		public bool IsOK { get { return m_IsOK; } set { m_IsOK = value; } }

		public JustInfo()
		{
			InitializeComponent();
			Owner = AppWin.Instance;
		}
		public JustInfo(String toptext, String oktext, String canceltext)
		{
			InitializeComponent();
			Owner = AppWin.Instance;
			TopText.Text = toptext;
			if (oktext == null)
				OK.Visibility = Visibility.Collapsed;
			else
				OK.Text = oktext;
			if (canceltext == null)
				Cancel.Visibility = Visibility.Collapsed;
			else
				Cancel.Text = canceltext;
		}

		public void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			m_IsOK = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
