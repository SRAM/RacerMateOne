﻿using System;
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
using System.Threading;
using System.Diagnostics;
namespace RacerMateOne.Pages
{
	/// <summary>
	/// Show a splashscreen
	/// </summary>
	public partial class Splash : Page
	{
		bool m_bRetry = false;
		int m_RetryCount = 0;
		public Splash()
		{
			InitializeComponent();

			System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Version.Text = "Version " + ver.ProductVersion;
            //Version.ToolTip = "RM1.dll " + RM1.DLLVersion + ", api " + RM1.APIVersion;

            Log.Debug(string.Format("{0} - Splash", DateTime.Now));

            //Log.WriteLine("Initializing 3D Start");
            //AppWin.InitRM1ExtDLL();
            //Log.WriteLine("Initializing 3D Done");
        }

		bool m_bFullScan;
		private void FormFadeIn_Completed(object sender, EventArgs e)
		{
#if DEBUG
			Log.WriteLine("FormFadeIn_Completed()");
#endif

			AppWin.Instance.Initialize();

			Log.Debug(string.Format("{0} - Hardware Scanning start", DateTime.Now));

			while (Courses.ms_WaitForOtherThread)
			    Thread.Sleep(100);
			AppWin.Instance.Initialize2();

			if (RM1_Settings.gFirstRun)
			{
                TopLine.Content = "Setup";
                m_bFullScan = true;
                RM1.StartFullScan(false, Continue_FirstRun);
			}
			else
			{
                RM1.StartFullScan(true, Continue_Init);
			}

			//Checking.Visibility = Visibility.Visible;
			//CheckingAnim.Begin();
			Log.Debug(string.Format("{0} - Hardware Scanning end", DateTime.Now));
            /*
            Log.WriteLine("Initializing 3D Start");
            AppWin.InitRM1ExtDLL();
            Log.WriteLine("Initializing 3D Done");
             */
            //while (Courses.ms_WaitForOtherThread)
            //    Thread.Sleep(100);
            //AppWin.Instance.Initialize2();

        }
		private void pageSplash_Loaded(object sender, RoutedEventArgs e)
		{
			Log.WriteLine("Fading in splash screen");
        }

        private void pageSplash_Unloaded(object sender, RoutedEventArgs e)
        {
            Log.WriteLine("Splash screen done");
        }


		public void AutoAssign()
		{
			Unit.LoadFromSettings();
			Unit.AllocateHardware(true);
			Unit.SaveToSettings();
			Unit.UpdateTrainerData(true); // Clear out the training data.
		}

		public void Continue_FirstRun()
		{
			Log.WriteLine(RM1.ValidTrainerCount <= 0 ? "Didn't find any trainers" :
				RM1.ValidTrainerCount > 1 ? "Found " + RM1.ValidTrainerCount + " trainers" : "Found 1 trainers");

			// added check for m_bContinue so we can get pass this 
			if (RM1.ValidTrainerCount <= 0 && !m_bContinue)
			{
				DoRetry();
				return;
			}

//			RM1.OnTrainerInitialized -= new RM1.TrainerInitialized(Continue_FirstRun);
			AutoAssign(); // On first run get everything into slots

			Log.WriteLine("More than 3+ present?", true);
			// < 2 and both of one type will get the welcome screen.... otherwise we are in commercial mode.
			if (RM1.ValidTrainerCount <= 2 && (RM1.Computrainers == RM1.ValidTrainerCount || RM1.Velotrons == RM1.ValidTrainerCount))
			{
				Log.WriteLine("User Type?", true);
				// Hand control over to the welcome page.
				NavigationService.Navigate(new Pages.Start.Welcome());
			}
			else
			{
				RM1_Settings.General.Commercial = true;
				NavigationService.Navigate(new Pages.Start.ImportCSV());
			}
			Log.Debug(string.Format("{0} - Continue_FirstRun", DateTime.Now));
		}
		
		void DoRetry()
		{
			ErrorB.Visibility = Visibility.Visible;
			CannotFind.Visibility = RM1_Settings.gFirstRun ? Visibility.Visible : Visibility.Hidden;
			NoneFound.Visibility = RM1_Settings.gFirstRun ? Visibility.Hidden : Visibility.Visible;
				
			m_bRetry = false;
			FadeInRetry.Begin();
		}

		bool m_bContinue;
		public void Continue_Init()
		{
			if (RM1.ValidTrainerCount <= 0 && !RM1_Settings.General.Commercial && !m_bContinue)
			{
				DoRetry();
				return;
			}

			if (m_bFullScan)
				RM1_Settings.SaveToFile();
			//CheckingAnim.Stop();

			Unit.LoadFromSettings();    // Load up all the units.

			if (RM1_Settings.General.Commercial)
				NavigationService.Navigate(new Pages.Selection());
			else if (AppWin.style_sheet && AppWin.style_sheet_after_splash)
				NavigationService.Navigate(new Pages.StyleSheet());
			else if (Pages.Modes.Calibrate.PreUse() || Pages.Modes.Calibrate2.OkToUse())
				NavigationService.Navigate(new Pages.Start.RunCalibration());
			else
				NavigationService.Navigate(new Pages.Selection());
			Log.Debug(string.Format("{0} - Continue_Init", DateTime.Now));
		}

		private void NoHardware_Click(object sender, RoutedEventArgs e)
		{
			m_bContinue = true;
			Continue_Init();
		}

		private void HardwareRescan_Click(object sender, RoutedEventArgs e)
		{
			if (m_bRetry)
				return;
			Debug.WriteLine("HardwareRescan_Click");
			m_bRetry = true;
			FadeInRetry.Stop();
			FadeOutRetry.Begin();
			RM1.ClearAllTrainers();
			m_RetryCount++;
			Log.WriteLine("============================================");
			m_bFullScan = true;
			RM1.StartFullScan(false, Continue_FirstRun);
			Log.WriteLine("============================================");
		}

		private void Continue_Click(object sender, RoutedEventArgs e)
		{
			if (RM1_Settings.gFirstRun)
			{
				m_bContinue = true;
				Continue_FirstRun(); 
			}
			else
			{
				m_bContinue = true;
				Continue_Init();
			}
		}

		private void Retry_Click(object sender, RoutedEventArgs e)
		{
			if (m_bRetry)
				return;
			Debug.WriteLine("Retry_Click");
			m_bRetry = true;
			FadeInRetry.Stop();
			FadeOutRetry.Begin();
			RM1.ClearAllTrainers();
			m_RetryCount++;
			Log.WriteLine("============================================");
			if (RM1_Settings.gFirstRun)
			{
				m_bFullScan = true;
				RM1.StartFullScan(false, Continue_FirstRun);
			}
			else
			{
				RM1.StartFullScan(true, Continue_Init);
			}
			Log.WriteLine("============================================");
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Exit();
		}
		private void Help_Click(object sender, RoutedEventArgs e)
		{
			AppWin.Help("Hardware_Detect.htm");
		}
	}
}
