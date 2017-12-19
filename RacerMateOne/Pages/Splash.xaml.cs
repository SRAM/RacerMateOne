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
using System.Threading;
using System.Diagnostics;
namespace RacerMateOne.Pages {


public partial class Splash : Page {
	bool m_bRetry = false;
	int m_RetryCount = 0;
	bool m_bContinue;
	bool m_bFullScan;


	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	public Splash() {
		InitializeComponent();
#if DEBUG
		Log.WriteLine("Splash::Splash()");
#endif


		System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
		//Version.Text = "Version " + ver.ProductVersion;
		//Version.ToolTip = "RM1.dll " + RM1.DLLVersion + ", api " + RM1.APIVersion;
		//Log.Debug(string.Format("{0} - Splash", DateTime.Now));
		//Log.WriteLine("Initializing 3D Start");
		//AppWin.InitRM1ExtDLL();
		//Log.WriteLine("Initializing 3D Done");
	}


	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	private void FormFadeIn_Completed(object sender, EventArgs e) {
#if DEBUG
		Log.WriteLine("Splash::FormFadeIn_Completed(), FormFadeIn_Completed()");
		Log.WriteLine("Splash::FormFadeIn_Completed(), calling AppWin::Initialize()");
#endif

		AppWin.Instance.Initialize();

		//Log.WriteLine("");
		//Log.WriteLine("Splash.xaml.cs: " + string.Format("{0} - Hardware Scanning start", DateTime.Now));
		Log.WriteLine("Splash::FormFadeIn_Completed(), hardware scanning start");

		while (Courses.ms_WaitForOtherThread) {
			Thread.Sleep(100);
		}

#if DEBUG
		Log.WriteLine("Splash::FormFadeIn_Completed(), calling AppWin::Initialize2()");
#endif

		AppWin.Instance.Initialize2();

		if (RM1_Settings.gFirstRun) {
			#if DEBUG
			Log.WriteLine("Splash::FormFadeIn_Completed(), RM1_Settings.gFirstRun = true");
			Log.WriteLine("Splash::FormFadeIn_Completed(), first run == true: calling RM1.StartFullSca()");
			Log.WriteLine("Splash::FormFadeIn_Completed(), setting m_bFullScan to true");
			#endif

			TopLine.Content = "Setup";
			m_bFullScan = true;
			RM1.StartFullScan(false, Continue_FirstRun);			// Continue_firstRun() is the event handler
		}
		else  {
			#if DEBUG
			Log.WriteLine("Splash::FormFadeIn_Completed(), RM1_Settings.gFirstRun = false");
			Log.WriteLine("Splash::FormFadeIn_Completed(), first run == false: calling RM1.StartFullSca()");
			#endif

			RM1.StartFullScan(true, Continue_Init);         // Continue_Init() is the ScanCompleteEventHandler
		}

		Log.WriteLine("Splash::FormFadeIn_Completed(), exit");
	}                       // FormFadeIn_Completed()



	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	private void pageSplash_Loaded(object sender, RoutedEventArgs e) {

#if DEBUG
		AppWin.Instance.Title = "Splash.xaml.cs";
#endif
		Log.WriteLine("Splash.xaml.cs: Fading in splash screen");
	}

	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	private void pageSplash_Unloaded(object sender, RoutedEventArgs e) {
		Log.WriteLine("Splash.xaml.cs: Splash screen done");
	}


	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	public void AutoAssign() {
		Unit.LoadFromSettings();
		Unit.AllocateHardware(true);
		Unit.SaveToSettings();
		Unit.UpdateTrainerData(true);    // Clear out the training data.
	}

	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	public void Continue_FirstRun() {
#if DEBUG
		Log.WriteLine("Splash::Continue_FirstRun(): " + "RM1.ValidTrainerCount = " + RM1.ValidTrainerCount);
#endif
//		Log.WriteLine(RM1.ValidTrainerCount <= 0 ? "Splash::Continue_FirstRun(), Didn't find any trainers" :
//		              RM1.ValidTrainerCount > 1 ? "Found " + RM1.ValidTrainerCount + " trainers" : "Found 1 trainers");

		// added check for m_bContinue so we can get pass this
		if (RM1.ValidTrainerCount <= 0 && !m_bContinue) {
			DoRetry();
			return;
		}

//			RM1.OnTrainerInitialized -= new RM1.TrainerInitialized(Continue_FirstRun);
		AutoAssign();    // On first run get everything into slots

		Log.WriteLine("More than 3+ present?", true);
		// < 2 and both of one type will get the welcome screen.... otherwise we are in commercial mode.
		if (RM1.ValidTrainerCount <= 2 && (RM1.Computrainers == RM1.ValidTrainerCount || RM1.Velotrons == RM1.ValidTrainerCount)) {
			Log.WriteLine("User Type?", true);
			// Hand control over to the welcome page.
			NavigationService.Navigate(new Pages.Start.Welcome());
		}
		else  {
			RM1_Settings.General.Commercial = true;
			NavigationService.Navigate(new Pages.Start.ImportCSV());
		}
		Log.Debug(string.Format("{0} - Continue_FirstRun", DateTime.Now));
	}

	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	void DoRetry() {
		ErrorB.Visibility = Visibility.Visible;
		CannotFind.Visibility = RM1_Settings.gFirstRun ? Visibility.Visible : Visibility.Hidden;
		NoneFound.Visibility = RM1_Settings.gFirstRun ? Visibility.Hidden : Visibility.Visible;

		m_bRetry = false;
		FadeInRetry.Begin();
	}

	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	public void Continue_Init() {
		if (RM1.ValidTrainerCount <= 0 && !RM1_Settings.General.Commercial && !m_bContinue) {
			DoRetry();
			return;
		}

		if (m_bFullScan) {
			RM1_Settings.SaveToFile();
		}
		//CheckingAnim.Stop();

		Unit.LoadFromSettings();          // Load up all the units.

		if (RM1_Settings.General.Commercial) {
			NavigationService.Navigate(new Pages.Selection());
		}
		else if (AppWin.style_sheet && AppWin.style_sheet_after_splash) {
			NavigationService.Navigate(new Pages.StyleSheet());
		}
		else if (Pages.Modes.Calibrate.PreUse() || Pages.Modes.Calibrate2.OkToUse()) {
			NavigationService.Navigate(new Pages.Start.RunCalibration());
		}
		else {
			NavigationService.Navigate(new Pages.Selection());
		}
		Log.Debug(string.Format("{0} - Continue_Init", DateTime.Now));
	}

	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	private void NoHardware_Click(object sender, RoutedEventArgs e) {
		m_bContinue = true;
		Continue_Init();
	}

	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	private void HardwareRescan_Click(object sender, RoutedEventArgs e) {
		if (m_bRetry) {
			return;
		}
		Debug.WriteLine("HardwareRescan_Click");
		m_bRetry = true;
		FadeInRetry.Stop();
		FadeOutRetry.Begin();
		RM1.ClearAllTrainers();
		m_RetryCount++;
		Log.WriteLine("Splash:: HardwareRescan_Click() calling RM1.StartFullScan()");
		m_bFullScan = true;
		RM1.StartFullScan(false, Continue_FirstRun);
		Log.WriteLine("Splash:: HardwareRescan_Click() exit");
	}

	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	private void Continue_Click(object sender, RoutedEventArgs e) {
		if (RM1_Settings.gFirstRun) {
			m_bContinue = true;
			Continue_FirstRun();
		}
		else  {
			m_bContinue = true;
			Continue_Init();
		}
	}

	/*******************************************************************************************************************************

	*******************************************************************************************************************************/

	private void Retry_Click(object sender, RoutedEventArgs e) {
		if (m_bRetry) {
			return;
		}
		Debug.WriteLine("Retry_Click");
		m_bRetry = true;
		FadeInRetry.Stop();
		FadeOutRetry.Begin();
		RM1.ClearAllTrainers();
		m_RetryCount++;
		Log.WriteLine("Splash:: Retry_Click() 1 ============================================");
		if (RM1_Settings.gFirstRun) {
			m_bFullScan = true;
			RM1.StartFullScan(false, Continue_FirstRun);
		}
		else  {
			RM1.StartFullScan(true, Continue_Init);
		}
		Log.WriteLine("Splash:: Retry_Click() 2 ============================================");
	}

	private void Exit_Click(object sender, RoutedEventArgs e) {
		AppWin.Exit();
	}
	private void Help_Click(object sender, RoutedEventArgs e) {
		AppWin.Help("Hardware_Detect.htm");
	}
}
}
