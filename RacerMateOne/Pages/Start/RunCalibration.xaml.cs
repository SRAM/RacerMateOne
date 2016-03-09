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
	/// Interaction logic for RunCalibration.xaml
	/// </summary>
	public partial class RunCalibration : Page
	{
		public bool FirstTime;
		public RunCalibration()
		{
			InitializeComponent();
		}
		void SaveDoNotAsk()
		{
			if (RM1_Settings.General.CalibrationCheck != (bool)!ckDoNotAskAgain.IsChecked)
			{
				RM1_Settings.General.CalibrationCheck = (bool)!ckDoNotAskAgain.IsChecked;
				RM1_Settings.SaveToFile();
			}
		}
		private void Yes_Click(object sender, RoutedEventArgs e)
		{
			SaveDoNotAsk();

			RM1.Trainer t = RM1.ValidTrainers.First();
			int num = -1;
			Unit.LoadFromSettings();
			foreach(Unit unit in Unit.Units)
			{
				if (unit.Trainer == t)
				{
					num = unit.Number;
					break;
				}
			}
			if (num < 0)
			{
				foreach (Unit unit in Unit.Units)
				{
					if (unit.Trainer != null)
					{
						num = unit.Number;
						break;
					}
				}
			}
			if (num < 0)
			{
				Unit.Units[0].Trainer = t;
				num = 0;
			}

			//Pages.RideOptions.ms_SelectedTab = "Hardware setup"
			//Pages.RideOptions p = new Pages.RideOptions();
			//AppWin.Instance.MainFrame.Navigate(p);
			if (Pages.Modes.Calibrate2.OkToUse())
			{
				Pages.Modes.Calibrate2 cal2 = new RacerMateOne.Pages.Modes.Calibrate2();
				cal2.ExitToSelection = true;
				Course course = new Course();
				if (course.Load(RacerMatePaths.EXEPath + @"\Courses\Distance and Grade\Warmup_3dc.rmc"))
				{
					Controls.Render3D.Course = Unit.Course = course;
					AppWin.Instance.MainFrame.Navigate(cal2);
				}
				else
					throw new InvalidOperationException("Cannot find a valid trainer");
			}
			else
			{
				Pages.Modes.Calibrate cpage = new Pages.Modes.Calibrate();
				cpage.ExitToSelection = true;
				cpage.UnitNumber = num;
				Course course = new Course();
				if (course.Load(RacerMatePaths.EXEPath + @"\Courses\Distance and Grade\Warmup_3dc.rmc"))
				{
					Controls.Render3D.Course = Unit.Course = course;
					AppWin.Instance.MainFrame.Navigate(cpage);
				}
				else
					throw new InvalidOperationException("Cannot find a valid trainer");
			}
		}

		private void No_Click(object sender, RoutedEventArgs e)
		{
			SaveDoNotAsk();
			NavigationService.Navigate(new Pages.Selection());
		}

		private void ckDoNotAskAgain_Checked(object sender, RoutedEventArgs e)
		{

		}
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			System.Environment.Exit(0);
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help("Rolling_Calibration_CT.htm");
		}
	}
}
