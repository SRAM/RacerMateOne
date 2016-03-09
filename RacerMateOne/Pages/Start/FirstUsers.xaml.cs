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
	/// Interaction logic for FirstUsers.xaml
	/// </summary>
	public partial class FirstUsers : Page
	{
		int Count = 0;
		bool bDone = false;

		int m_nRider = 1;

		
		public FirstUsers()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if (RM1_Settings.General.Metric)
				WeightSystem.SelectedIndex = 1;
			if (RM1.ValidTrainers.Count > 0 && RM1.ValidTrainers[0].Type == RM1.DeviceType.VELOTRON)
			{
				Calibrate.Text = "Continue"; // "Check Accuwatt™";
			}
			Keyboard.Focus(FirstName);
		}
		private bool SaveRider()
		{
			Rider r = new Rider();
			r.FirstName = FirstName.Text.Trim();
			r.LastName = LastName.Text.Trim();
			r.NickName = UserName.Text.Trim();
			if (r.NickName == "")
			{
				MessageBox.Show("You must enter a name for this rider");
				return false;
			}
			r.Gender = (Male.IsChecked == true ? "M" : "F");
			String bkwt = BikeWeight.Text;

			bool metric = WeightSystem.SelectedIndex != 0;
			r.Metric = metric;
			try
			{
				double w = System.Convert.ToDouble(RiderWeight.Text);
				r.WeightRider = w * (metric ? ConvertConst.KGStoLBS:1);
			}
			catch
			{ MessageBox.Show("You must provide the rider's weight"); return false; }

			try
			{
				double w = System.Convert.ToDouble(bkwt);
				r.WeightBike = w * (metric ? ConvertConst.KGStoLBS : 1);
			}
			catch
			{ MessageBox.Show("You must provide the bike's weight"); return false; }

			Riders.AddNewRider(r);
			RM1_Settings.General.SelectedRiderKey = r.DatabaseKey;
			Riders.SaveToFile();
			if (Count == 0)
				RM1_Settings.SavedTrainersList[0].PreviousRiderKey = r.DatabaseKey;
			Count++;
			return true;
		}

		private void ResetFields()
		{
			switch (m_nRider)
			{
				case 1: nRider.Text = ""; break;
				case 2: nRider.Text = "(2nd rider)"; break;
				case 3: nRider.Text = "(3rd rider)"; break;
				case 4: nRider.Text = "(4th rider)"; break;
				default: nRider.Text = String.Format("({0}th rider)", m_nRider); break;
			}
			FirstName.Text = "";
			LastName.Text = "";
			UserName.Text = "";
			tw_save = rw_save = bw_save = "";
			TotalWeight.Text = "";
			RiderWeight.Text = "";
			BikeWeight.Text = "";
			//WeightLBS.Text = "";
			//WeightKGS.Text = "";
			Keyboard.Focus(FirstName);
		}

		private void AddAnother_Click(object sender, RoutedEventArgs e)
		{
			if (bDone)
				return;
			if (SaveRider())
			{
				m_nRider++;
				ResetFields();
			}
		}

		private void DoCalibrate()
		{
			if (Pages.Modes.Calibrate.PreUse() || Pages.Modes.Calibrate2.OkToUse())
			{
				Pages.Start.RunCalibration page = new Pages.Start.RunCalibration();
				page.FirstTime = true;
				NavigationService.Navigate(page);
			}
			else
				NavigationService.Navigate(new Pages.Selection());
		}

		private void Done_Click(object sender, RoutedEventArgs e)
		{
			if (bDone)
				return;

			if (WeightSystem.SelectedIndex != 0)
			{

				RacerMateOne.Dialogs.TextLine tline = new RacerMateOne.Dialogs.TextLine();
				tline.TopText.Text = "Use Metric settings globally?";
				tline.Input.Visibility = Visibility.Collapsed;
				tline.OK.Text = "Yes";
				tline.Cancel.Text = "No";
				tline.Height = 180;
				tline.Owner = AppWin.Instance;

				tline.ShowDialog();
				if (tline.IsOK)
				{
					RM1_Settings.General.Metric = true;
				}
			}


			if (Count > 0 && FirstName.Text.Trim() == "" && LastName.Text.Trim() == "" && UserName.Text.Trim() == "" &&
				TotalWeight.Text.Trim() == "")
			{
				DoCalibrate();
			}
			else if (SaveRider())
			{
				DoCalibrate();
			}
		}

		private void UserName_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void CalcTotal()
		{

			double r, b;
			int c = 0;
			try { r = System.Convert.ToDouble(RiderWeight.Text);} catch { c++; r = 0; }
			try { b = System.Convert.ToDouble(BikeWeight.Text); }
			catch { c++; b = 0; }
			if (c < 2)
			{
				tw_save = "" + (r + b);
				TotalWeight.Text = tw_save;
			}
		}

		bool m_nochange = false;
		String tw_save = "";
		String bw_save = "";
		String rw_save = "";
		private void RiderWeight_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (rw_save == RiderWeight.Text || m_nochange)
				return;
			rw_save = RiderWeight.Text;
			CalcTotal();
		}

		private void BikeWeight_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (bw_save == BikeWeight.Text || m_nochange)
				return;
			bw_save = RiderWeight.Text;
			CalcTotal();
		}

		private void TotalWeight_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (tw_save == TotalWeight.Text || m_nochange)
				return;
			try
			{
				double w = System.Convert.ToDouble((tw_save = TotalWeight.Text));
				bw_save = ""; rw_save = "";
				RiderWeight.Text = "";
				BikeWeight.Text = "";
			}
			catch { }
		}
		private void WeightSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			bool metric = WeightSystem.SelectedIndex != 0;
			if (metric != RM1_Settings.General.Metric)
			{
				//RM1_Settings.General.Metric = metric;
				m_nochange = true;
				try { RiderWeight.Text = (Math.Round(Convert.ToDouble(RiderWeight.Text) * (metric ? ConvertConst.LBStoKGS : ConvertConst.KGStoLBS))).ToString(); }
				catch { }
				try { BikeWeight.Text = (Math.Round(Convert.ToDouble(BikeWeight.Text) * (metric ? ConvertConst.LBStoKGS : ConvertConst.KGStoLBS))).ToString(); }
				catch { }
				try { TotalWeight.Text = (Math.Round(Convert.ToDouble(TotalWeight.Text) * (metric ? ConvertConst.LBStoKGS : ConvertConst.KGStoLBS))).ToString(); }
				catch { }
				m_nochange = false;
			}
		}
		//==================================================
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Exit();
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help("First_Use.htm");
		}

		//==================================================


	}
}
