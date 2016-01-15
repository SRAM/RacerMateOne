using System;
using System.Management;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Markup;
using System.Xml;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.InteropServices;


/*****************************************************************************************************************************
	'~' opens debug window

	flow:
 		AppWin.xaml.cs			AppWin::Initialize()
 		App.xaml.cs				SetBasePath(), basepath = Z:\data\_fs\rm1\RacerMateOne_Source\RacerMateOne\bin\Debug



	todo:
		fix velotron gear shifting

		add udp server stuff

*****************************************************************************************************************************/

namespace RacerMateOne
{


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		public static ErrorLog Logger = new ErrorLog();
        public static Mutex mutex;
        public static bool AllowPerfSave = true;
        public App()
        {
			#if DEBUG
			  Log.WriteLine("\n\nApp.xaml.cs, constructor");
			  Debug.WriteLine("\n\nApp.xaml.cs, constructor");
#else
				DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Application_DispatcherUnhandledException);
			#endif

            Thread.CurrentThread.Name = "MainThread";	  

            bool onlyone;
            mutex = new System.Threading.Mutex(true, "RacerMateOne", out onlyone);
            if (!onlyone)
                System.Environment.Exit(0); // there is another process running

			string[] args = Environment.GetCommandLineArgs();
			Log.WriteLine("RacerMate1 Starting "+args, true);

            foreach (string arg in args)
            {
                if (arg.Contains("-install"))
                    System.Environment.Exit(0);
            }

            SetDefaultCulture();

            // Call to initialize current directory used by 3D first
            InitBasePath();
        }

        [DllImport("RM1_Ext.dll")]
        private static extern int SetBasePath(IntPtr basepath);
        public static void InitBasePath()
        {
            String basepath = Directory.GetCurrentDirectory(); //System.IO.Path.GetDirectoryName(Assembly.GetAssembly(typeof(App)).CodeBase); 
            IntPtr fname = Marshal.StringToHGlobalAnsi(basepath);
            SetBasePath(fname);
            Marshal.FreeHGlobal(fname);
#if DEBUG
				Debug.WriteLine("App.xaml.cs, SetBasePath(), basepath = " + basepath);
#endif

        }
        [DllImport("RM1_Ext.dll")]
        private static extern IntPtr CalcShrink(IntPtr PlainString);
        public static string CalcShrink(string PlainString)
        {
            string ShrinkedString;
            IntPtr p_PlainString = Marshal.StringToHGlobalAnsi(PlainString);
            ShrinkedString = Marshal.PtrToStringAnsi(CalcShrink(p_PlainString));
            Marshal.FreeHGlobal(p_PlainString);
            return ShrinkedString;
        }

        // Set default culture to fix crashes string conversion problems when user is in a different region settings
        // Called at start of each thread
        // If culture in thread is changed, make sure to revert back to previous
        public static void SetDefaultCulture()
        {
            System.Globalization.CultureInfo cultureInfo =
                new System.Globalization.CultureInfo("en-US");
            /*
            // Creating the DateTime Information specific to our application.
            System.Globalization.DateTimeFormatInfo dateTimeInfo =
                new System.Globalization.DateTimeFormatInfo();
            // Defining various date and time formats.
            dateTimeInfo.DateSeparator = "/";
            dateTimeInfo.LongDatePattern = "dd-MMM-yyyy";
            dateTimeInfo.ShortDatePattern = "dd-MMM-yy";
            dateTimeInfo.LongTimePattern = "hh:mm:ss tt";
            dateTimeInfo.ShortTimePattern = "hh:mm tt";
            // Setting application wide date time format.
            cultureInfo.DateTimeFormat = dateTimeInfo;
            // Assigning our custom Culture to the application.
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
             */
            Thread.CurrentThread.CurrentCulture = cultureInfo;
        }

        private static string _HardwareID = "";
        public static string GetHardwareID()
        {
            if (_HardwareID.Length <= 0)
            {
                string outstr = "";
                try
                {
                    /*
                    {
                        ManagementObjectSearcher searcher =
                            new ManagementObjectSearcher("root\\CIMV2",
                            "SELECT * FROM Win32_DiskDrive");

                        foreach (ManagementObject queryObj in searcher.Get())
                        {
                            Debug.WriteLine("-----------------------------------");
                            Debug.WriteLine("Win32_DiskDrive instance");
                            Debug.WriteLine("-----------------------------------");
                            Debug.WriteLine("Caption: {0}", queryObj["Caption"]);
                            Debug.WriteLine("InterfaceType: {0}", queryObj["InterfaceType"]);
                            Debug.WriteLine("SerialNumber: {0}", queryObj["SerialNumber"]);

                            outstr += string.Format(@"""{0}"" ", queryObj["SerialNumber"]);
                        }
                    }
                    {
                        ManagementObjectSearcher searcher =
                            new ManagementObjectSearcher("root\\CIMV2",
                            "SELECT * FROM Win32_Processor");

                        foreach (ManagementObject queryObj in searcher.Get())
                        {
                            Debug.WriteLine("-----------------------------------");
                            Debug.WriteLine("Win32_Processor instance");
                            Debug.WriteLine("-----------------------------------");
                            Debug.WriteLine("Name: {0}", queryObj["Name"]);
                            Debug.WriteLine("Family: {0}", queryObj["Family"]);
                            Debug.WriteLine("ProcessorId: {0}", queryObj["ProcessorId"]);

                            outstr += string.Format(@"""{0}"" ", queryObj["ProcessorId"]);
                        }
                    }
                     * */
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_Processor");
                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        outstr += string.Format("{0}", queryObj["ProcessorId"]);
                    }
                }
                catch //(ManagementException e)
                {
                    //MessageBox.Show("An error occurred while querying for WMI data: " + e.Message);
                    outstr = "UnknownCPUID";
                }
                //outstr += "]";
                string hashstr = HashOutStream.ComputeHash(String.Format("{0}PROLIFIC",outstr));
                _HardwareID = CalcShrink(hashstr.Substring(0,11));
            }
            return _HardwareID;
        }

		public static UIElement CloneElement(UIElement orig)
		{
			if (orig == null)
				return (null);

			string s = XamlWriter.Save(orig);
			StringReader stringReader = new StringReader(s);
			XmlReader xmlReader = XmlTextReader.Create(stringReader, new XmlReaderSettings());
			return (UIElement)XamlReader.Load(xmlReader);
		}


		static bool ms_Exception = false;
		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			if (ms_Exception)
				return;
			ms_Exception = true;
            MessageBoxResult result = MessageBoxResult.OK;
			try
			{
				Exception ex = (Exception)e.Exception;
				String[] arr = Logger.LogError(ex);
				String LogFile = arr[0];
				String LogName = arr[1];


				result = MessageBox.Show(
					"The application encountered a fatal error and must exit.\n\n"+
					"This error has been logged to \n\t\""+LogFile+"\"\n"
 
					/*
					+
						"Error:\n" +
						ex.Message +
						"\n\nStack Trace:\n" +
						ex.StackTrace
					 */
					,
					"Fatal Error",
                    //MessageBoxButton.OKCancel, //ECT - forgot to remove the cancel path for testing.
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);

				/*
				Process proc = new Process();
				proc.EnableRaisingEvents = false;
				proc.StartInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorReport.exe");
				proc.StartInfo.Arguments = LogFile;
				proc.Start();
				 */
			}
			catch { }
			e.Handled = true;
            if(result == MessageBoxResult.OK)
    			AppWin.Exit();

		}
    }

	public class ExternalProgram
	{
		public readonly String Name;
		public readonly String Exe;
		public readonly String Folder;
		public readonly String Path;

		public ExternalProgram(String name, String exe, params object[] folders)
		{
			Name = name;
			Path = null;
			Exe = exe;
			foreach (object args in folders)
			{
				String p = ((String)args) + @"\" + exe;
				if (File.Exists(p))
				{
					Folder = (String)args;
					Path = p;
					break;
				}
			}
		}

		public String ExeProgram()
		{
			if (Path == null)
				return "Couldn't find \"" + Exe + "\"";
			;
			String ans;
			String d = Directory.GetCurrentDirectory();
			try
			{
				Directory.SetCurrentDirectory(Folder);
				Process executable = new Process();
				executable.StartInfo.FileName = Exe;
				executable.StartInfo.UseShellExecute = true;
				executable.Start();
				AppWin.Exit();
				ans = null;
			}
			catch { ans = "Couldn't execute \"" + Exe + "\""; }
			Directory.SetCurrentDirectory(d);
			return ans;
		}
	}


	public sealed class ErrorLog
	{
		#region Properties

		private string _LogPath;
		public string LogPath
		{
			get
			{
				return _LogPath;
			}
		}

		#endregion

		#region Constructors

		public ErrorLog()
		{
			_LogPath = RacerMatePaths.ErrorsFullPath;							// C:\Users\name\Documents\RacerMate\Errors
				/*
				System.IO.Path.Combine(
				System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Windows.Forms.Application.ProductName), 
				"Errors");
				 */
			#if DEBUG
			  Log.WriteLine("App.xaml.cs, ErrorLog constructor, path = " + _LogPath);
			#endif

			if (!Directory.Exists(_LogPath))
				Directory.CreateDirectory(_LogPath);
		}

		public ErrorLog(string logPath)
		{
			_LogPath = logPath;
			if (!Directory.Exists(_LogPath))
				Directory.CreateDirectory(_LogPath);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Logs exception information to the assigned log file.
		/// </summary>
		/// <param name="exception">Exception to log.</param>
		public String[] LogError(Exception exception)
		{
			Assembly caller = Assembly.GetEntryAssembly();
			Process thisProcess = Process.GetCurrentProcess();

			string username = "Unknown";
			try { username = Environment.UserName; }
			catch { }
			string LogFile = username+"_"+DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss") + ".txt";
			string FullPath = System.IO.Path.Combine(_LogPath, LogFile);
			using (StreamWriter sw = new StreamWriter(FullPath))
			{
				sw.WriteLine("==============================================================================");
				sw.WriteLine(caller.FullName);
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Application Information");
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Program      : " + caller.Location);
				try
				{
					System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
					sw.WriteLine("Version      : " + ver.ProductVersion);
				}
				catch { }
				try
				{
					sw.WriteLine("DLL Version  : " + RM1.DLLVersion );
				}
				catch { }
				sw.WriteLine("Time         : " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss zzz"));
				sw.WriteLine("User         : " + Environment.UserName);
				sw.WriteLine("Computer     : " + Environment.MachineName);
				sw.WriteLine("OS           : " + Environment.OSVersion.ToString());
				//sw.WriteLine("Culture      : " + CultureInfo.CurrentCulture.Name);
				sw.WriteLine("Processors   : " + Environment.ProcessorCount);
				sw.WriteLine("Working Set  : " + Environment.WorkingSet);
				sw.WriteLine("Framework    : " + Environment.Version);
				sw.WriteLine("Run Time     : " + (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());
				try { sw.WriteLine("Frame        : " + AppWin.Instance.MainFrame.NavigationService.Content.GetType().ToString()); }
				catch { }

				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Page History");
				sw.WriteLine("------------------------------------------------------------------------------");
				try
				{
					List<AppWin.HistoryEntry> list = AppWin.Instance.HistoryList;
					foreach (AppWin.HistoryEntry hentry in list)
					{
						hentry.Write(sw);
					}
				}
				catch { }
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Exception Information");
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Source       : " + exception.Source.ToString().Trim());
				sw.WriteLine("Method       : " + exception.TargetSite.Name.ToString());
				sw.WriteLine("Type         : " + exception.GetType().ToString());
				sw.WriteLine("Error        : " + GetExceptionStack(exception));
				sw.WriteLine("Stack Trace  : " + exception.StackTrace.ToString().Trim());
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Loaded Modules");
				sw.WriteLine("------------------------------------------------------------------------------");
				foreach (ProcessModule module in thisProcess.Modules)
				{
					try
					{
						sw.WriteLine(module.FileName + " | " + module.FileVersionInfo.FileVersion + " | " + module.ModuleMemorySize);
					}
					catch (FileNotFoundException)
					{
						sw.WriteLine("File Not Found: " + module.ToString());
					}
					catch (Exception)
					{

					}
				}
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine(LogFile);
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Hardware");
				sw.WriteLine("------------------------------------------------------------------------------");
				try
				{
					foreach (RM1.Trainer trainer in RM1.ValidTrainers)
					{
						Unit unit = Unit.TrainerUnit(trainer);
						sw.WriteLine("\tPort: {0}, Type {1}, Version: {2}, Unit: {3}", 
							trainer.PortNumber + 1, trainer.TypeString, trainer.Version, unit == null ? "-":(unit.Number).ToString() );
					}
				}
				catch { }
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Misc");
				sw.WriteLine("------------------------------------------------------------------------------");
				try
				{
					if (Unit.Course == null)
						sw.WriteLine("Course       : none");
					else
						sw.WriteLine("Course       : " + Unit.Course.Name + ", " + Unit.Course.FileName );

					foreach (Unit unit in Unit.Units)
					{
						sw.WriteLine(String.Format("Unit {0}: {1}, {2}, {3}, {4}",
							unit.Number,
							unit.IsActive ? "Active" : "Inactive",
							unit.Rider != null ? unit.Rider.IDName : "No Rider",
							unit.Bot != null ? unit.Bot.Key : "No bot",
							unit.Trainer != null ? unit.Trainer.CBLine:"no trainer"));
					}
				}
				catch { }


				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Settings");
				sw.WriteLine("------------------------------------------------------------------------------");
				try
				{
					XDocument doc = RM1_Settings.CreateXDocFromSettings();
					sw.WriteLine(doc.ToString());
				}
				catch
				{
					sw.WriteLine("Failed to get settings!");
				}
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Rider DB");
				sw.WriteLine("------------------------------------------------------------------------------");
				try
				{
					XDocument doc = Riders.CreateXDocFromRiders();
					sw.WriteLine(doc.ToString());
				}
				catch
				{
					sw.WriteLine("Failed to get settings!");
				}
				if (Perf.LoadedSets.Count > 0)
				{
					sw.WriteLine("------------------------------------------------------------------------------");
					sw.WriteLine("Current Loaded Performances");
					sw.WriteLine("------------------------------------------------------------------------------");
					try
					{
						foreach (String str in Perf.LoadedSets)
							sw.WriteLine(str);
					}
					catch
					{
						sw.WriteLine("Failed ");
					}
				}
				sw.WriteLine("------------------------------------------------------------------------------");
				sw.WriteLine("Log");
				sw.WriteLine("------------------------------------------------------------------------------");
				try
				{
					List<Log.Entry> entry = Log.GetList(true);
					foreach(Log.Entry e in entry )
					{
						String s = String.Format("{0} {1}",e.Num,e.TimeStamp.ToString());
						if (e.ThreadName != null && e.ThreadName != "")
							s += ","+e.ThreadName;
						if (e.Flags != Log.Flags.Zero)
							s += ", ("+e.Flags.ToString()+")";
						s += ": "+e.Message;
						sw.WriteLine(s);
					}
				}
				catch
				{
					sw.WriteLine("Failed to get settings!");
				}
				sw.WriteLine("==============================================================================");
			}
			String[] ss = new String[2];
			ss[0] = FullPath;
			ss[1] = LogFile;
			return ss;
		}

		#endregion

		#region Private Methods

		private string GetExceptionStack(Exception e)
		{
			StringBuilder message = new StringBuilder();
			message.Append(e.Message);
			while (e.InnerException != null)
			{
				e = e.InnerException;
				message.Append(Environment.NewLine);
				message.Append(e.Message);
			}

			return message.ToString();
		}

		#endregion
	}

}
