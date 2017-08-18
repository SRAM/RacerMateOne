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
using System.Diagnostics;       // Needed for process invocation
using Microsoft.Win32;
using System.ComponentModel; // CancelEventArgs
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;
using System.Data;
using System.Xml.Linq;
using System.Windows.Threading;
using System.Threading;
using System.Reflection;
using System.Web;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Interop;

namespace RacerMateOne  {
    public enum EXECUTION_STATE : uint  {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
        // Legacy flag, should not be used.
        // ES_USER_PRESENT = 0x00000004
    }

	/// <summary>
	/// Interaction logic for AppWin.xaml
	/// </summary>
	public partial class AppWin : Window  {
		public static Random Random = new Random();
		public static bool style_sheet = false;
		public static bool style_sheet_after_splash = false;
		//========================================================
		private static bool? _isInDesignMode;
		public static bool IsInDesignMode  {
			get
			{
				if (!_isInDesignMode.HasValue)
				{
#if SILVERLIGHT
					_isInDesignMode = DesignerProperties.IsInDesignTool;
#else
					var prop = DesignerProperties.IsInDesignModeProperty;
					_isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
#endif
				}
				return _isInDesignMode.Value;
			}
		}
		//========================================================


		public static AppWin Instance;
		//================================================
		public Pages.Splash Page_Splash;
		public Pages.Blank Page_Blank;

       public Pages.Start.Registration Page_Registration;
       //================================================
		
		// Helpers for disabling the poweroptions and screensaver
        public static bool bIgnorePowerOptions = true; // Set this to false if you don't want to prevent screensavers or monitor's screen shutdown.
        #region poweroptions
        //private const int WM_SYSCOMMAND = 0x112;
        //private const int SC_SCREENSAVE = 0xF140;
        //private const int SC_MONITORPOWER = 0xF170;
        //private static IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    if (msg == WM_SYSCOMMAND &&
        //        ((((long)wParam & 0xFFF0) == SC_SCREENSAVE) ||
        //        ((long)wParam & 0xFFF0) == SC_MONITORPOWER))
        //    {
        //        handled = bIgnorePowerOptions;
        //        if (bIgnorePowerOptions)
        //        {
        //            Controls.Render3D.DLL.StopScreenSaver();
        //            Debug.WriteLine("Mouse move just fired");
        //        }
        //    }
        //    return IntPtr.Zero;
        //}
        #endregion
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern uint SetThreadExecutionState(EXECUTION_STATE esFlags); 
		
        public AppWin()		{												// constructor
            InitializeComponent();

				#if DEBUG
					Log.WriteLine("AppWin.xaml.cs, AppWin constructor");
				#endif

				Instance = this;

            //Log.WriteLine("Initializing 3D Start");
            //InitRM1ExtDLL();
            //Log.WriteLine("Initializing 3D Done");

            // Setup for disabling the poweroptions and screensaver using Window's hook
            #region poweroptions
            //SourceInitialized += delegate
            //{
            //    HwndSource hwndSource = (HwndSource)
            //        PresentationSource.FromVisual(this);
            //    hwndSource.AddHook(Hook);
            //};

            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED );
            
            #endregion

        }

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void SetPageTitle(String n)  {
			System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
#if DEBUG
			//this.Title = "AppWin";
			this.Title = "RacerMate One" + (n == null || n == "" ? "" : " - " + n) + " (ver. " + ver.ProductVersion + ")";
#else
			this.Title = "RacerMate One" + (n == null || n == "" ? "" : " - " + n) + " (ver. " + ver.ProductVersion + ")";
#endif
		}

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		static String ms_Version = null;

		public static String Version  {
			get  {
				if (ms_Version == null)  {
					System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
					ms_Version = ver.ProductVersion;
				}
				return ms_Version;
			}
		}


		public static SolidColorBrush StdBrush_Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#76A7CA"));
		public static SolidColorBrush StdBrush_BackgroundLight = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8FB7D2"));
		public static SolidColorBrush StdBrush_Dark = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF266388"));
		public static SolidColorBrush StdBrush_Drafting = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0080FF"));
		public static SolidColorBrush StdBrush_ButtonBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6d9bbc"));

		public static SolidColorBrush StdBrush_Selected = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB5CFE1"));

		private Brush m_BackgroundSave;
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void Window_Loaded(object sender, RoutedEventArgs e)
		{
			m_BackgroundSave = Background;
			Log.WriteLine("Show splash page");

			Controls.RM1PageTitle.UpdateTitle();

            if (style_sheet && !style_sheet_after_splash)
			{
				Pages.StyleSheet ps = new Pages.StyleSheet();
				MainFrame.Navigate(ps);
			}
            else if (!IsRegistered())  {
				FixDirectKeyboard();
				Page_Splash = new Pages.Splash();
                //Pages.Start.Registration reg = new Pages.Start.Registration();
                Page_Registration = new Pages.Start.Registration();
                MainFrame.Navigate(Page_Registration);
			}
			else  {
                // Sample of how to get the registration info
                string regCode = "", CDKey = "", Email = "";
                bool bRegistered = GetRegistrationInfo(ref regCode, ref CDKey, ref Email);

				Page_Splash = new Pages.Splash();
				MainFrame.Navigate(Page_Splash);	// Handles the initialization process.
			}


            Log.Debug(string.Format("{0} - Window_Loaded", DateTime.Now));
        }
		static int timenum = 0;
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		static public void reset_time() 
		{
			Debug.WriteLine("===========================");
			lasttime = DateTime.Now;
		} 
		static DateTime lasttime;
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		static public void time(String id)
		{
			DateTime newtime = DateTime.Now;
			TimeSpan ts = newtime - lasttime;
			lasttime = newtime;
			Debug.WriteLine(String.Format("{0},{1}: {2:F5}", id,timenum++, ts.TotalSeconds));
		}


        //[DllImport("RM1_Ext.dll")]
        //public static extern void InitRM1ExtDLL();

		/// <summary>
		/// Called from the splash screen.
		/// </summary>
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void Initialize()
		{
#if DEBUG
			Log.WriteLine("\nAppWin::Initialize");
			Debug.WriteLine("\nAppWin.xaml.cs   AppWin::Initialize()");
#endif

			Log.WriteLine("Loading settings");

			// Locate the user settings file, load it.
			// The file is in a fixed location "<Environment.SpecialFolder.MyDocuments>\RacerMate\Settings\RM1_Settings.xml"
			// and is changeable only in class RacerMatePaths
			// On error, reset the settings to the defaults

#if DEBUG
			Log.WriteLine("AppWin::Initialize, calling RM1_Settings.LoadSettings()");
#endif

			if (RM1_Settings.LoadSettings() == false)  {
				AppWin.Exit();
			}

			if (RM1.Initialize_Server() == false)
			{
				AppWin.Exit();
			}

			Log.WriteLine("Loading Rider Database");


#if DEBUG
			// visibility
			//int ii;
			//ii = RM1.BarCount;						// 24, static things
			//RM1.VelotronData vd = new RM1.VelotronData();					// internal public class
			//RM1_Settings.ActiveTrainerList
			//RacerMateOne.RM1.DeviceType devtype = RM1_Settings.ActiveTrainerList[0].DeviceType;				// VELOTRON
#endif


			// Load the RM1_RiderDB file
			// The file is in a fixed location "<Environment.SpecialFolder.MyDocuments>\RacerMate\Settings\RM1_RiderDB.xml"
			// On error, reset the settings to the defaults, which is the single riderdb file


#if DEBUG
			Log.WriteLine("AppWin.cs, AppWin::Initialize, calling Riders.LoadFromFile()");
#endif
			if (Riders.LoadFromFile() == false) {
				AppWin.Exit();
			}

            Courses.InitOtherThread();

            m_BBOff.Add(p8);
			m_BBOff.Add(p7);
			m_BBOff.Add(p6);
			m_BBOff.Add(p5);
			m_BBOff.Add(p4);
			m_BBOff.Add(p3);
			m_BBOff.Add(p2);
			m_BBOff.Add(p1);

			RM1.OnTrainerInitialized += new RM1.TrainerInitialized(OnTrainerInitialized);
			RM1.OnClosed += new RM1.TrainerEvent(OnTrainerClosed);

            // this appears to be for some preview function with fake trainer.
            //PreviewKeyDown += Window_PreviewKeyDown;
            //PreviewKeyUp += Window_PreviewKeyUp;

			//Course c = new Course();
			//if (c.Load(@"C:\Real Course Video\IMCanada\IMCanada.avi"))
			//	Courses.Add(c);

			FixDirectKeyboard();
        }

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		/// <summary>
		/// Called from the splash screen.
		/// </summary>
        public void Initialize2()
        {
            /*
            Log.WriteLine("Initializing 3D Start");
            InitRM1ExtDLL();
            Log.WriteLine("Initializing 3D Done");
            */
            //Debug.WriteLine(string.Format("{0} - Worker Scanning start", DateTime.Now));
            Log.WriteLine("Starting Course Scan");
            // remove this to prevent the double-up of the last performance in all performance lists.
            //foreach (KeyValuePair<String, CourseInfo> kv in RM1_Settings.General.SelectedCourse)
            //{
            //    Courses.AddScanFile(kv.Value.FileName);
            //}

            Courses.Scan(RacerMatePaths.RCVFullPath);

            //Courses.Scan(RacerMatePaths.CommonCoursesFullPath);
            //foreach (string dir in Directory.GetDirectories(RacerMatePaths.CommonCoursesFullPath))
            //{
            //    Debug.WriteLine(" ******* scanning directory: ********* " + System.IO.Path.Combine(RacerMatePaths.CommonCoursesFullPath, dir));
            //    Courses.Scan(System.IO.Path.Combine(RacerMatePaths.CommonCoursesFullPath, dir));
            //}
            Courses.Scan(RacerMatePaths.CoursesFullPath);
            //Courses.Scan(RacerMatePaths.CoursesFullPath + "\\3DC");
            foreach (string dir in Directory.GetDirectories(RacerMatePaths.CoursesFullPath))
            {
             //  Debug.WriteLine(" ******* scanning directory: ********* " + System.IO.Path.Combine(RacerMatePaths.CoursesFullPath, dir));
                Courses.Scan(System.IO.Path.Combine(RacerMatePaths.CoursesFullPath, dir));
            }
            Courses.ScanVideos();
            Log.WriteLine("Initializing Button Boxes");
           // Debug.WriteLine("Done init2");
            //Debug.WriteLine(string.Format("{0} - Worker Scanning running in thread", DateTime.Now));
        }


		  /*******************************************************************************************************************

		  *******************************************************************************************************************/

		  private void Window_Closed(object sender, EventArgs e)
		{
			Exit();
		}

		//=======================================================
		public static int SimTestNumTrainers = 0;
		public static bool SimTrainersConnected = false;
		//public static List<Trainer> TrainersAvailableThisSession = new List<Trainer>();
		public static List<String> TrainersAvailableThisSessionAsStrings = new List<String>(); //must keep one trainer Is realTrainer=False at the top of this list at all times
		/*
		public static List<String> InitializeTrainersAsStrings(List<Trainer> inlist)
		{
			List<String> retval = new List<String>();
			retval.Clear();
			return retval;
		}
		 */
		//=======================================================

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public static void Exit()
		{
            RM1.Exit();
			Instance.Close();		
			System.Environment.Exit(0);
		}

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void DeferExit()
		{
			Hide();
			Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				Exit();
			});
		}


		//=======================================================
		Dictionary<RM1.Trainer, Controls.ButtonBox> m_BBMap = new Dictionary<RM1.Trainer, Controls.ButtonBox>();
		List<Controls.ButtonBox> m_BBOff = new List<Controls.ButtonBox>();

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		private void OnTrainerInitialized(RM1.Trainer trainer, int left)
		{
			if (trainer == null || !trainer.IsConnected)
				return;
			if (!m_BBMap.ContainsKey(trainer) && m_BBOff.Count > 0)
			{
				m_BBMap[trainer] = m_BBOff.Last();
				m_BBOff.Remove(m_BBOff.Last());
				m_BBMap[trainer].Trainer = trainer;
			}
		}
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		private void OnTrainerClosed(RM1.Trainer trainer, object obj)
		{
			if (m_BBMap.ContainsKey( trainer ))
			{
				m_BBOff.Add(m_BBMap[trainer]);
				m_BBMap.Remove( trainer );
			}
		}

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (RM1.Trainer.Fake == null)
				return;
			RM1.Trainer t = RM1.Trainer.Fake;
			switch (e.Key)
			{
				case Key.NumPad7:
					t.FakeKeys |= 1;
					break;
				case Key.NumPad4:
					t.FakeKeys |= 2;
					break;
				case Key.NumPad8:
					t.FakeKeys |= 4;
					break;
				case Key.NumPad5:
					t.FakeKeys |= 8;
					break;
				case Key.NumPad9:
					t.FakeKeys |= 16;
					break;
				case Key.NumPad6:
					t.FakeKeys |= 32;
					break;
			}
		}

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (RM1.Trainer.Fake == null)
				return;
			RM1.Trainer t = RM1.Trainer.Fake;
			switch (e.Key)
			{
				case Key.NumPad7:
					t.FakeKeys &= ~1;
					break;
				case Key.NumPad4:
					t.FakeKeys &= ~2;
					break;
				case Key.NumPad8:
					t.FakeKeys &= ~4;
					break;
				case Key.NumPad5:
					t.FakeKeys &= ~8;
					break;
				case Key.NumPad9:
					t.FakeKeys &= ~16;
					break;
				case Key.NumPad6:
					t.FakeKeys &= ~32;
					break;
			}

		}


		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public static void OpenURL(String url)
		{
			try
			{
				// launch default browser
				System.Diagnostics.Process.Start(url);
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message);
			}
		}

		[DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
		static extern int UrlCreateFromPath(
			[In]     string path,
			[Out]    StringBuilder url,
			[In, Out] ref uint urlLength,
			[In]     uint reserved
			);
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		private static string UrlFromPath(string filepath)
		{
			uint maxLen = 2048 + 32 + 3;//see INTERNET_MAX_URL_LENGTH
			StringBuilder url = new StringBuilder((int)maxLen);
			UrlCreateFromPath(filepath, url, ref maxLen, 0);
			return url.ToString();
		}

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public static void Help()
		{
			Help("Help.htm");
		}
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public static void Help(String page)
		{
            string s = Assembly.GetEntryAssembly().Location;
			Regex regexp = new Regex(@"\\[^\\]*$");
			s = regexp.Replace(s, @"\Help\" + page);
			//+"Help\\Help.html";
    		s = UrlFromPath(s);
     		OpenURL(s);
		}

		//===================================================

		public delegate void OnDirectKey( RM1.PadKeys key );
		private event OnDirectKey m_OnDirectKey;

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void AddDirectKeyboardEvent(OnDirectKey ev)
		{
			m_OnDirectKey += ev;
			FixDirectKeyboard();
		}
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void RemoveDirectKeyboardEvent(OnDirectKey ev)
		{
			m_OnDirectKey -= ev;
			FixDirectKeyboard();
		}

		private bool m_bDirectKeyboardOn = true;
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		private void FixDirectKeyboard()
		{
			bool on = (m_OnDirectKey != null);
			if (on == m_bDirectKeyboardOn)
				return;

			m_bDirectKeyboardOn = on;
			if (on)
			{
				InputBindings.Add(Key_A);
				InputBindings.Add(Key_S);
				InputBindings.Add(Key_G);
				InputBindings.Add(Key_R);
				InputBindings.Add(Key_B);
				InputBindings.Add(Key_Up);
				InputBindings.Add(Key_Down);
				InputBindings.Add(Key_CtrlUp);
				InputBindings.Add(Key_CtrlDown);
			}
			else
			{
				InputBindings.Remove(Key_A);
				InputBindings.Remove(Key_S);
				InputBindings.Remove(Key_G);
				InputBindings.Remove(Key_R);
				InputBindings.Remove(Key_B);
				InputBindings.Remove(Key_Up);
				InputBindings.Remove(Key_Down);
				InputBindings.Remove(Key_CtrlUp);
				InputBindings.Remove(Key_CtrlDown);
			}
		}
		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void DirectKey(char s)
		{
			if (m_OnDirectKey == null)
				return;
			switch (s)
			{
				case 'G':
					m_OnDirectKey(RM1.PadKeys.F1);
					break;

				case 'S': m_OnDirectKey(RM1.PadKeys.F2); break;
				case 'A': m_OnDirectKey(RM1.PadKeys.F3); break;
				case 'u': m_OnDirectKey(RM1.PadKeys.UP); break;
				case 'd': m_OnDirectKey(RM1.PadKeys.DOWN); break;
				case 'R': m_OnDirectKey(RM1.PadKeys.F5); break;
				case 'B': m_OnDirectKey(RM1.PadKeys.F6); break;
				case 'U': m_OnDirectKey(RM1.PadKeys.FN_UP); break;
				case 'D': m_OnDirectKey(RM1.PadKeys.FN_DOWN); break;
			}
		}

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void Render3DOn()
		{
			Background = Brushes.Black;
			Controls.Render3D.Active.Visibility = Visibility.Visible;
		}

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void Render3DOff()
		{
			Background = m_BackgroundSave;
			Controls.Render3D.Active.Visibility = Visibility.Hidden;
		}

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public void About()
		{
			Dialogs.About dlg = new Dialogs.About();
			dlg.Owner = AppWin.Instance;
			dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			dlg.ShowDialog(); //shows as modal
		}

		bool m_RenderFront = false;
		public bool RenderFront
		{
			get { return m_RenderFront; }
			set
			{
				if (value == m_RenderFront)
					return;
				if (RenderCenter)
					RenderCenter = false;
				m_RenderFront = value;
				MainGrid.Children.Remove(MainRender3D);
				MainGrid.Children.Insert(value ? MainGrid.Children.IndexOf(MainFrame)+1:0,MainRender3D);
			}
		}

		bool m_RenderCenter = false;
		public bool RenderCenter
		{
			get { return m_RenderCenter; }
			set
			{
				if (value == m_RenderCenter)
					return;
				if (RenderFront)
					RenderFront = false;
				m_RenderCenter = value;
				if (value)
				{
					MainGrid.Children.Remove(MainRender3D);
					FixedCenter.Children.Insert(0, MainRender3D);
					FixedCenterBackground.Visibility = Visibility.Visible;
				}
				else
				{
					FixedCenterBackground.Visibility = Visibility.Collapsed;
					FixedCenter.Children.Remove(MainRender3D);
					MainGrid.Children.Insert(0, MainRender3D);
				}
			}
		}


		//===================================
		/// <summary>
		/// 
		/// </summary>
		/// <param name="seconds">Seconds since program is up</param>
		/// <returns></returns>
		public delegate bool RenderUpdate( double seconds, double split );

		private static LinkedList<RenderUpdateNode> m_RenderUpdateList = new LinkedList<RenderUpdateNode>();
		struct RenderUpdateNode
		{
			public int pri;
			public RenderUpdate ru;
		};

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		public static void AddRenderUpdate(RenderUpdate ru, int priority)
		{
			RemoveRenderUpdate(ru);

			RenderUpdateNode rnode = new RenderUpdateNode();
			rnode.ru = ru;
			rnode.pri = priority;

			if (m_RenderUpdateList.Count <= 0 || priority <= m_RenderUpdateList.First.Value.pri)
				m_RenderUpdateList.AddFirst(rnode);
			else if (priority >= m_RenderUpdateList.Last.Value.pri)
				m_RenderUpdateList.AddLast(rnode);
			else
			{
				LinkedListNode<RenderUpdateNode> rn;
				for(rn = m_RenderUpdateList.First;rn != null;rn = rn.Next)
				{
					if (priority >= rn.Value.pri)
						break;
				}
				m_RenderUpdateList.AddBefore(rn, rnode);
			}
			if (m_RenderUpdateList.Count == 1)
			{
				CompositionTarget.Rendering += new EventHandler(AppRender);
			}
		}
		public static void RemoveRenderUpdate(RenderUpdate ru)
		{
			LinkedListNode<RenderUpdateNode> rnode;
			for (rnode = m_RenderUpdateList.First; rnode != null; rnode = rnode.Next)
			{
				if (rnode.Value.ru == ru)
				{
					m_RenderUpdateList.Remove(rnode);
					break;
				}
			}
			if (m_RenderUpdateList.Count == 0)
			{
				ms_LastRenderTime = ms_TimeSpanZero;
				CompositionTarget.Rendering += new EventHandler(AppRender);
			}
		}
		static TimeSpan ms_TimeSpanZero = new TimeSpan(0);

		static TimeSpan ms_LastRenderTime = new TimeSpan();
		static double ms_LastRenderSeconds;
		public static double LastRenderSeconds { get { return ms_LastRenderSeconds; } }

		/*******************************************************************************************************************

		*******************************************************************************************************************/

		static void AppRender(object sender, EventArgs e)
		{
            RenderingEventArgs rt = e as RenderingEventArgs;
			if (ms_LastRenderTime == rt.RenderingTime) // Occurse the same as last frame... don't redo this
				return;

            //Debug.WriteLine(string.Format("{0} - AppRender start", DateTime.Now));

            double seconds = rt.RenderingTime.TotalSeconds;
			double split = ms_LastRenderTime == ms_TimeSpanZero ? 0:seconds - ms_LastRenderSeconds;
			ms_LastRenderSeconds = seconds;
			ms_LastRenderTime = rt.RenderingTime;	

			LinkedListNode<RenderUpdateNode> rnode,rnext;
			for (rnode = m_RenderUpdateList.First; rnode != null; rnode = rnext)
			{
				rnext = rnode.Next;
				if (rnode.Value.ru(seconds,split))
				{
					m_RenderUpdateList.Remove(rnode);
				}
			}
			if (m_RenderUpdateList.Count == 0)
			{
				CompositionTarget.Rendering += new EventHandler(AppRender);
			}
            //Debug.WriteLine(string.Format("{0} - AppRender end", DateTime.Now));
        }
		//======================================================================
		public class HistoryEntry
		{
			public HistoryEntry(Page page)
			{
				m_Page = page;
				try { PageClass = page.GetType().ToString(); }
				catch { PageClass = "Error: Cannot find class for content"; }
				StartTime = DateTime.Now;
				Log.WriteLine(PageClass, true);
			}
			public void Close()
			{
				if (m_Page != null)
				{
					CloseTime = DateTime.Now;
					m_Page = null;
				}
			}
			Page m_Page;
			public Page Page { get { return m_Page; } }
			public String PageClass { get; protected set; }
			public DateTime StartTime { get; protected set; }
			public DateTime CloseTime { get; protected set; }
			List<String>	m_Notes;
			public void AddNote(String note)
			{
				if (m_Notes == null)
					m_Notes = new List<string>();
				m_Notes.Add(note);
			}

			public String Info
			{
				get
				{
					DateTime et = m_Page == null ? CloseTime:DateTime.Now;
					DateTime ss = new DateTime(et.Subtract(StartTime).Ticks);
					String s = String.Format("{0}, {1} {2:u} - {3:u}\r\n", 
						PageClass, ss.ToString("HH:mm:ss"), StartTime, et );
					if (m_Notes != null)
					{
						foreach(String n in m_Notes)
							s += "   " + n + "\r\n";
					}
					return s;
				}
			}
			public void Write( StreamWriter sw )
			{
				DateTime et = m_Page == null ? CloseTime : DateTime.Now;
				DateTime ss = new DateTime(et.Subtract(StartTime).Ticks);
				sw.WriteLine("{0}, {1} {2:u} - {3:u}",
					PageClass, ss.ToString("HH:mm:ss"), StartTime, et);
				if (m_Notes != null)
				{
					foreach (String n in m_Notes)
						sw.WriteLine( "   " + n );
				}
			}
		}													// class HistoryEntry

		List<HistoryEntry> m_HistoryList = new List<HistoryEntry>();
		public List<HistoryEntry> HistoryList { get { return m_HistoryList; } }
		HistoryEntry m_CurrentHistory;

		static public void Note( String note )
		{
			if (AppWin.Instance.m_CurrentHistory != null)
			{
				AppWin.Instance.m_CurrentHistory.AddNote( note );
			}
		}



		private void MainFrame_Navigated(object sender, NavigationEventArgs e)
		{
			Page page = MainFrame.NavigationService.Content as Page;
            //Debug.WriteLine("navigating to page : " + page.Title);
			if (page == null)
			{
				if (m_CurrentHistory != null)
					m_CurrentHistory.Close();
				return;
			}

			if (m_CurrentHistory != null)
			{
				if (m_CurrentHistory.Page == page)
					return;
				m_CurrentHistory.Close();
			}
			m_CurrentHistory = new HistoryEntry(page);
			m_HistoryList.Add(m_CurrentHistory);
		}

		public delegate void OnCloseEvent( CancelEventArgs e );
		public event OnCloseEvent OnClose;

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (OnClose != null)
			{
				OnClose(e);
			}
		}

        enum RegisterType : int
        {
            PIRATED = -1,
            DEMO = 0,
            TEMPREG = 1,
            DELUXE = 2
        }
        [DllImport("RM1_Ext.dll")]
        private static extern RegisterType GetRegistrationInfo(out IntPtr regCode, out IntPtr CDKey, out IntPtr email);
        [DllImport("RM1_Ext.dll")]
        private static extern RegisterType CheckRegisteredType(IntPtr hardwareid);
        [DllImport("RM1_Ext.dll")]
        private static extern RegisterType TryRegister(IntPtr Keycode, IntPtr HardwareSerialNum, IntPtr CDKey, IntPtr email, bool bForce);

        /// <summary>
        /// Should return true if everything is registered and false if is not.
        /// </summary>
        /// <returns></returns>
        public static bool GetRegistrationInfo(ref string regCode, ref string CDKey, ref string email)
        {
            IntPtr p_regCode = IntPtr.Zero;
            IntPtr p_CDKey = IntPtr.Zero;
            IntPtr p_email = IntPtr.Zero;
            RegisterType registered = GetRegistrationInfo(out p_regCode, out p_CDKey, out p_email);
            regCode = Marshal.PtrToStringAnsi(p_regCode);
            CDKey = Marshal.PtrToStringAnsi(p_CDKey);
            email = Marshal.PtrToStringAnsi(p_email);
            return (registered == RegisterType.DELUXE);
        }

        /// <summary>
		/// Should return true if everything is registered and false if is not.
		/// </summary>
		/// <returns></returns>
		public static bool IsRegistered()  {
            return true;

//#if DEBUG
//            	// The "#if DEBUG" is to protect it, just in case someone forgets to uncomment it, but should be commented out unless absolutely necessary.
//            	return true;
//#else
//            string hardwareid = App.GetHardwareID();
//            IntPtr p_hardwareid = Marshal.StringToHGlobalAnsi(hardwareid);
//            RegisterType registered = CheckRegisteredType(p_hardwareid);
//            Marshal.FreeHGlobal(p_hardwareid);
//            return (registered == RegisterType.DELUXE);
//#endif
        }

        /// <summary>
        /// Should return true if everything is registered and false if is not.
        /// </summary>
        /// <returns></returns>
        public static bool TryRegister(string Keycode, string HardwareSerialNum, string CDKey, string email, bool bForce)
        {
            IntPtr p_Keycode = Marshal.StringToHGlobalAnsi(Keycode);
            IntPtr p_HardwareSerialNum = Marshal.StringToHGlobalAnsi(HardwareSerialNum);
            IntPtr p_CDKey = Marshal.StringToHGlobalAnsi(CDKey);
            IntPtr p_email = Marshal.StringToHGlobalAnsi(email);
            RegisterType registered = TryRegister(p_Keycode, p_HardwareSerialNum, p_CDKey, p_email, bForce);
            Marshal.FreeHGlobal(p_email);
            Marshal.FreeHGlobal(p_CDKey);
            Marshal.FreeHGlobal(p_HardwareSerialNum);
            Marshal.FreeHGlobal(p_Keycode);
            return (registered == RegisterType.DELUXE);
        }

		private static bool ms_PreviewMode = false;
		public static bool PreviewMode { get { return ms_PreviewMode; } }
		public static void SetPreviewMode()
		{
			ms_PreviewMode = true;
		}
        

    }
}
