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
using System.Diagnostics;
using System.IO;

namespace RacerMateOne.Pages
{
	/// <summary>
	/// Interaction logic for Page_Selection.xaml
	/// </summary>
	public partial class Selection : Page
	{
		/*
		ExternalProgram p3D = new ExternalProgram("3D", @"Bike.exe", @"C:\CompuTrainer 3D V3", @"C:\program files\computrainer 3D V3");
		//ExternalProgram pRCV = null;
		ExternalProgram pChart = new ExternalProgram("3D", @"cchart.exe", @"C:\compcs",@"C:\program files\compcs" );
		ExternalProgram pTopo = new ExternalProgram("3D", @"TopoGPS.exe", @"C:\Topo GPS CC", @"C:\program files\Topo GPS CC");

		ExternalProgram pMulti = new ExternalProgram("3D", @"rmulti.exe", @"C:\MultiRider 2009", @"C:\program files\MultiRider 2009");
		*/


		public Selection()
		{
			InitializeComponent();
			//ErgVideo.NotAvailable = true;

            Log.Debug(string.Format("{0} - Selection", DateTime.Now));
        }

		public void Set(int num, String text, String img, bool visible,bool reg, String tooltip )
		{
			Control_Selection cs = (Control_Selection)this.FindName("V" + num);
			if (cs != null)
			{
				cs.ToolTip = tooltip;
				cs.Title = text;
				cs.Image = img;
				if (reg == true)
					cs.AddReg();
				cs.Visibility = (visible ? Visibility.Visible:Visibility.Hidden);
			}
		}
		static bool ms_bFirst;

		private void pageSelection_Loaded(object sender, RoutedEventArgs e)
		{
			if (!ms_bFirst && RM1_Settings.General.Commercial)
			{
				Pages.RideOptions options = new Pages.RideOptions();
				Pages.RideOptions.ms_SelectedTab = "Hardware setup";
				NavigationService.Navigate(options);
			}

			ms_bFirst = true;

			if (RM1_Settings.gFirstRun)
			{
				RM1_Settings.gFirstRun = false;
				RM1_Settings.SaveToFile();
			}
            Log.Debug(string.Format("{0} - pageSelection_Loaded", DateTime.Now));
        }

		public void ExeProgram(String path, String file )
		{
			String d = Directory.GetCurrentDirectory();
			try
			{
				Directory.SetCurrentDirectory(path);
				Process executable = new Process();
				executable.StartInfo.FileName = file;
				executable.StartInfo.UseShellExecute = true;
				executable.Start();
				AppWin.Exit();
			}
			catch
			{
				MessageBox.Show("Couldn't execute \"path\"");
			}
			Directory.SetCurrentDirectory(d);
		}




		private void r3D_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.Modes.Staging(Pages.Modes.Staging.Staging_Ride3D));
			//NavigationService.Navigate(new Pages.Modes.Ride3D_Setup());
		}

		private void rRCV_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.Modes.Staging(Pages.Modes.Staging.Staging_RCV));
			//NavigationService.Navigate(new Pages.Modes.RCV_Setup());
		}

		private void rWattTesting_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.Modes.Staging( Pages.Modes.Staging.Staging_PowerTraining ) );
			//NavigationService.Navigate(new Pages.Modes.PowerTraining_Setup());
		}

		private void rSpinScan_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.Modes.Staging(Pages.Modes.Staging.Staging_SpinScan));
			//NavigationService.Navigate(new Pages.Modes.SpinScan_Setup());
		}

		private void rCourseCreation_Click(object sender, RoutedEventArgs e)
		{
			/*
			String ans = pTopo.ExeProgram();
			if (ans != null)
				MessageBox.Show(ans);
			 */
		}

        private void rErgVideo_Click(object sender, RoutedEventArgs e)
        {

            AppWin.OpenURL("http://www.ergvideo.com/ErgVideoRM1Info.aspx");
        }


		//=========================================================================
		private void r_MouseLeave(object sender, MouseEventArgs e)
		{
			TopLine.Content = "Welcome, let's ride!";
			SubLine.Text = "";
		}

		private void r3D_MouseEnter(object sender, MouseEventArgs e)
		{
			Controls.ModeDisplay md = sender as Controls.ModeDisplay;
			if (!md.NotAvailable)
			{
				TopLine.Content = md.Title;
				SubLine.Text = md.Sub;
			}
		}

		private void rRCV_MouseEnter(object sender, MouseEventArgs e)
		{
			Controls.ModeDisplay md = sender as Controls.ModeDisplay;
			if (!md.NotAvailable)
			{
				TopLine.Content = md.Title;
				SubLine.Text = md.Sub;
			}
		}

		private void rWattTesting_MouseEnter(object sender, MouseEventArgs e)
		{
			Controls.ModeDisplay md = sender as Controls.ModeDisplay;
			if (!md.NotAvailable)
			{
				TopLine.Content = md.Title;
				SubLine.Text = md.Sub;
			}
		}

		private void rSpinScan_MouseEnter(object sender, MouseEventArgs e)
		{
			Controls.ModeDisplay md = sender as Controls.ModeDisplay;
			if (!md.NotAvailable)
			{
				TopLine.Content = md.Title;
				SubLine.Text = md.Sub;
			}
		}

		private void rMultiRider_MouseEnter(object sender, MouseEventArgs e)
		{
			Controls.ModeDisplay md = sender as Controls.ModeDisplay;
			if (!md.NotAvailable)
			{
				TopLine.Content = md.Title;
				SubLine.Text = md.Sub;
			}
		}

		private void rErgVideo_MouseEnter(object sender, MouseEventArgs e)
		{
			Controls.ModeDisplayPartners md = sender as Controls.ModeDisplayPartners;
			if (!md.NotAvailable)
			{
				TopLine.Content = md.Title;
				SubLine.Text = md.Sub;
			}
		}

		private void rMultiRider_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Pages.Modes.ClassicMultiRider());
			//NavigationService.Navigate(new Pages.Modes.MultiRider_Setup());
		}

		private void Branding_Drop(object sender, DragEventArgs e)
		{
			Log.WriteLine(sender.ToString());
			Log.WriteLine(e.ToString());
		}

		private void GroupBox_MouseEnter(object sender, MouseEventArgs e)
		{
			TopLine.Content = "Partners";
            SubLine.Text = "Learn more about ErgVideo™";
		}

        //private void btn_Click(object sender, RoutedEventArgs e)
        //{
        //    AppWin.OpenURL( "http://www.ergvideo.com/ErgVideoRM1Info.aspx" );
        //}
		//=============================================================
		private void Options_Click(object sender, RoutedEventArgs e)
		{
#if DEBUG
			Debug.WriteLine("Selection.xaml.cs, Options_Click()");
#endif

			NavigationService.Navigate(new Pages.RideOptions());
		}
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Exit();
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help();
		}

	}
}
