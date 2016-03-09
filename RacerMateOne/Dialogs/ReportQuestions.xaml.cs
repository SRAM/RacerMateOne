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
	public partial class ReportQuestions : Window
	{
		public bool IsOK { get { return ExportSave || PWXSave || ReportSave; } }
		public bool ExportSave { get; protected set; }
		public bool PWXSave { get; protected set; }
		public bool ReportSave { get; protected set; }

		public ReportQuestions()
		{
			InitializeComponent();
			t_ExportSave.IsChecked = RM1_Settings.General.ExportSave;
			t_PWXSave.IsChecked = RM1_Settings.General.PWXSave;
			t_ReportSave.IsChecked = RM1_Settings.General.ReportSave;
		}

		public void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			ExportSave = t_ExportSave.IsChecked == true;
			PWXSave = t_PWXSave.IsChecked == true;
			ReportSave = t_ReportSave.IsChecked == true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
