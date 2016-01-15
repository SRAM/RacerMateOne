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
	/// Interaction logic for Window_NoHardwareFound.xaml
	/// </summary>
	public partial class Window_NoHardwareFound : Window
	{
		int debugTries = 2;
		public Window_NoHardwareFound()
		{
			InitializeComponent();
		}

		private void Retry_Click(object sender, RoutedEventArgs e)
		{
			TempFix.Visibility = Visibility.Hidden;
			CheckingError.Visibility = Visibility.Hidden;
			Checking.Visibility = Visibility.Visible;
			CheckingAnim.Begin();
			Retry.IsEnabled = false;
			RM1.OnTrainerInitialized += new RM1.TrainerInitialized(Rescan);
			RM1.ClearAllTrainers();
			RM1.StartFullScan();
		}
		private void Rescan(RM1.Trainer trainer, int left)
		{
			if (left > 0)
				return;
			RM1.OnTrainerInitialized -= new RM1.TrainerInitialized(Rescan);
			if (RM1.ValidTrainerCount > 0)
			{
				Close();
				return;
			}
			CheckingError.Visibility = Visibility.Visible;
			CheckingAnim.Stop();
			Checking.Visibility = Visibility.Hidden;
			Retry.IsEnabled = true;

			debugTries--;
			if (debugTries <= 0)
				TempFix.Visibility = Visibility.Visible;

		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Exit();
		}

		private void TempFix_Click(object sender, RoutedEventArgs e)
		{
			RM1.AddFake();
			Close();
			return;
		}

	}
}
