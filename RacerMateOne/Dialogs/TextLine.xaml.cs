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
	public partial class TextLine : Window
	{
		public String OutText = null;

		bool m_IsOK = false;
		public bool IsOK { get { return m_IsOK; } }

		public TextLine()
		{
			InitializeComponent();
		}

		public void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Input.Focus();
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			OutText = Input.Text;
			m_IsOK = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			OutText = null;
			Close();
		}
	}
}
