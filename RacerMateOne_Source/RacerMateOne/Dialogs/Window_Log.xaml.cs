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
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace RacerMateOne  {

    /// <summary>
    /// Interaction logic for Window_Log.xaml
    /// </summary>

    public partial class Window_Log : Window  {
		List<StackPanel> preList = new List<StackPanel>();
		string preMain = "";

        XDocument ms_XLog;
        string ms_XLogPath;
			bool bLoaded = false;
        ListBox ms_LogBox;
        protected delegate void _log( string text, bool flowchart );
		protected delegate void _logE(Log.Entry e, bool flowchart);

		//=================================================

		static Window_Log instance;
        public static Window_Log Instance  {
            get  {
				if (instance == null)
					instance = new Window_Log();
                return instance;
            }
        }

        //=================================================

        Window_Log()  {
            ms_XLogPath = RacerMatePaths.SettingsFullPath + "\\RunLog.xml";			// in .../settings/Runlog.xml
            //string[] s = _assembly.GetManifestResourceNames();
            //Debug.WriteLine(s);

            InitializeComponent();

				#if DEBUG
					BatchConvert.Visibility = Visibility.Visible;
				#endif

				Log.WriteLine("Window_Log constructor");
		}

		void SentLog(Log.Entry e)  {
			if ((e.Flags & Log.Flags.Debug) != Log.Flags.Zero)
				return;
			LogA(e, (e.Flags & RacerMateOne.Log.Flags.FlowChart) != RacerMateOne.Log.Flags.Zero);
		}
       

        public void Window_Loaded(object sender, EventArgs e)  {
			bLoaded = true;
         
            // Write the version to the bottom part of the log window.
            System.Diagnostics.FileVersionInfo ver = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Version.Content = "Version: " + ver.ProductVersion + " (RM1.dll: " + RM1.DLLVersion + ", API: " + RM1.APIVersion + ")";
         
            ms_LogBox = LogBox;

            Log.WriteLine("Load/Create RunLog.xml file");
            LoadOrCreate(RacerMatePaths.SettingsFullPath + "\\RunLog.xslt", "Templates.RunLog.xslt");
            ms_XLog = LoadOrCreate(ms_XLogPath, "Templates.RunLog.xml");
            XContainer x = ms_XLog.Element( "Log" ).Element( "RunCount" );
            if (x == null)
                ms_XLog.Element("Log").Add( (x = new XElement("RunCount",0) ) );
            int c = XmlConvert.ToInt32((x as XElement).Value);
            x.ReplaceNodes(c + 1);

            ms_XLog.Element("Log").Add( 
                new XElement( "Started",
                    new XElement( "Name", "RacerMateOne" ),
                    new XElement("Version", ver.ProductVersion),
                    new XElement("Date", XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.RoundtripKind))
                    )
                );
            ms_XLog.Save(ms_XLogPath);

			int n = 0;
			foreach(StackPanel sp in preList)
				n = ms_LogBox.Items.Add(sp);
			if (n > 0)
				ms_LogBox.ScrollIntoView(ms_LogBox.Items.GetItemAt(n));

			Current.Content = preMain;

			// Show the list up to this point.
			List<Log.Entry> list = Log.GetList(false);
			foreach( Log.Entry entry in list )
			{
				SentLog(entry);
			}
			RacerMateOne.Log.LogEvent += new Log.LogHandler(SentLog);
        }

        protected XDocument LoadOrCreate(string filepath, string strname)
        {
            XDocument xd;
            try
            {
                xd = XDocument.Load(filepath);
            }
            catch
            {
                Assembly _assembly = Assembly.GetExecutingAssembly();
                Stream _xStream = _assembly.GetManifestResourceStream("RacerMateOne." + strname);
                StreamReader _textStreamReader = new StreamReader(_xStream);
                xd = XDocument.Load(_textStreamReader);
                xd.Save(filepath);
            }
            return xd;
        }

		/**
		 * Thread safe
		 */
		void LogA(Log.Entry e, bool flowchart)
		{
			if (CheckAccess())
			{
				//DateTime.Now.ToLongTimeString()
				StackPanel sp = new StackPanel();
				sp.Orientation = Orientation.Horizontal;

				TextBlock t = new TextBlock();
				t.Text = e.TimeStamp.ToString("hh:mm:ss.fff");
				t.Width = 88;
				sp.Children.Add(t);
				t = new TextBlock();
				String text = e.Message;
				t.Text = (flowchart ? "* " + text + " *" : text);
				sp.Children.Add(t);


				if (bLoaded)
				{

					int n = ms_LogBox.Items.Add(sp);
					ms_LogBox.ScrollIntoView(ms_LogBox.Items.GetItemAt(n));

					if (flowchart)
					{
						//Label c = FindName("Current") as Label;
                        Current.Content = text;
					}
				}
				else
				{
					preList.Add(sp);
					if (flowchart)
						preMain = text;
				}
			}
			else
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Normal, new _logE(LogA), e, flowchart);
			}

		}

        /**
         * Thread safe
         */
        void LogA(string text, bool flowchart )
        {
            if (CheckAccess())
            {
                //DateTime.Now.ToLongTimeString()
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;

                TextBlock t = new TextBlock();
                t.Text = DateTime.Now.ToLongTimeString();
                t.Width = 80;
                sp.Children.Add(t);
                t = new TextBlock();
                t.Text = (flowchart ? "* " + text + " *" : text);
                sp.Children.Add(t);

				if (bLoaded)
				{

					int n = ms_LogBox.Items.Add(sp);
					ms_LogBox.ScrollIntoView(ms_LogBox.Items.GetItemAt(n));

					if (flowchart)
					{
						//Label c = FindName("Current") as Label;
                        Current.Content = text;
					}
				}
				else
				{
					preList.Add(sp);
					if (flowchart)
						preMain = text;
				}
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, new _log(LogA), text, flowchart);
            }

        }

        public void LogA(string text) { LogA(text, false); }

		private void ClearSettings_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show("Are you sure you wish to clear the settings?  This will also exit the program." + Environment.NewLine + "Upon restart, the default courses will be copied into the local Courses folder.", "", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
			{
				RM1_Settings.DeleteSettingsFile();
				//Riders.DeleteRidersFile();
				AppWin.Exit();
			}
		}
        private void ClearRiders_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to clear the settings & riders?  This will also exit the program.", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                RM1_Settings.DeleteSettingsFile();
                Riders.DeleteRidersFile();
                AppWin.Exit();
            }
        }

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			String[] arr = null;
			try
			{
				throw new Exception("No Error!");
			}
			catch (Exception ex)
			{
				arr = App.Logger.LogError(ex);
			}
			if (arr != null)
			{
				try
				{
					String LogFile = arr[0];
					String LogName = arr[1];


					MessageBox.Show(
						"A report has been written to \n\t\"" + LogFile + "\"\n"
						,
						"Report written",
						MessageBoxButton.OK);

				}
				catch {}
			}
		}

        private void BatchConvert_Click(object sender, RoutedEventArgs e)
        {
			if (PerfFile.BatchConvertFolder(RacerMatePaths.CommonCoursesFullPath))
                return;
        }

        [DllImport("RM1_Ext.dll")]
        private static extern void clearRegCode();

        private void ClearReg_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to unregister the program?  This will also exit the program.", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                clearRegCode();
                AppWin.Exit();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                 e.Cancel = true;
        }

        private void btnRestoreCourses_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you wish to restore the default Courses? It will not affect custom courses created.  This will also exit the program.", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                RM1_Settings.CopyDefaultCoursesToLocal();
                AppWin.Exit();
            }
        }

       

       
    }
}
