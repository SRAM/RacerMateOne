#define NO_SERVER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;       // Needed for process invocation
using System.ComponentModel; // CancelEventArgs
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

/************************************************************************************************

************************************************************************************************/

namespace RacerMateOne  {
	public class RM1  {

		/************************************************************************

		************************************************************************/

		static RM1() {
#if DEBUG
			Log.WriteLine("RM1() static constructor");
			Debug.WriteLine("RM1.cs   RM1() static constructor");
			//FILE* stream = fopen("x.x", "wt");
			//fclose(stream);
#endif

			BarRadians = new Double[RM1.BarCount];
			double step = Math.PI / RM1.HalfBarCount;

			for (int i = 0; i < RM1.HalfBarCount; i++) {
				BarRadians[i + RM1.HalfBarCount] = BarRadians[i] = step * i + (step * 0.5);
			}
		}

		/************************************************************************

		************************************************************************/

		RM1()
		{
			IntPtr pfullpath = Marshal.StringToHGlobalAnsi(RacerMatePaths.DebugFullPath);
			_status_logpath = DLL.Setlogfilepath(pfullpath);
			Marshal.FreeBSTR(pfullpath);
			DLL.Enablelogs(true, true, true, true, true, true);

			StartedANT = false;
			antSensors.sensorCount = 0;
			antSensors.sensors = new SENSOR[MAXSENSORS];

			// for testing
			antSensors.sensorCount = 2;
			antSensors.sensors[0].assigned_channel = 123;
			antSensors.sensors[0].sensor_number = 12345;
			antSensors.sensors[0].transtype = 1;
			antSensors.sensors[0].type = 120;
			antSensors.sensors[0].val = 33;

			antSensors.sensors[1].assigned_channel = 124;
			antSensors.sensors[1].sensor_number = 11111;
			antSensors.sensors[1].transtype = 2;
			antSensors.sensors[1].type = 120;
			antSensors.sensors[1].val = 34;
		} // RM1() constructor

		public static void Exit()
		{
            ms_bShutdownScanningThread = true;
			ms_BackgroundScanningThread.Join();
            RM1.Trainer.Exit();
		}


		/************************************************************************

		************************************************************************/
		public static bool Initialize_Server()
		{
			Log.WriteLine("Initializing Racermate Server.");
#if DEBUG
			String dir = Directory.GetCurrentDirectory();            // "D:\\_fs\\rm1\\RacerMateOne_Source\\RacerMateOne\\bin\\Debug"
			System.Console.WriteLine(dir);
#endif

#if DEBUG
			Debug.WriteLine("RM1.cs   calling DLL.racermate_init()");
#endif
			try {
				DLL.racermate_init();
			}
			catch (Exception e)
			{
				Log.WriteLine(e.ToString());
				RacerMateOne.Dialogs.JustInfo info = new RacerMateOne.Dialogs.JustInfo("Failed to Initialize RacerMate.\nYour installation may have incorrect files.\n\n" + e.ToString(), "OK", "Cancel");
				info.ShowDialog();
				return false;
			}

			int status = 0;
			int debug_level = 2;

			try {
#if NO_SERVER
				status = 100;						// don't start net server, return "disabled"
#else

            status = DLL.start_server(
               RM1_Settings.General.WifiListenPort,        // int listen_port
               RM1_Settings.General.WifiBroadcastPort,     // int broadcast_port
               RM1_Settings.General.WifiOverrideIPAddress, // override ip adress
               debug_level);
#endif
			}
			catch (Exception e) {
				Log.WriteLine(e.ToString());
				RacerMateOne.Dialogs.JustInfo info = new RacerMateOne.Dialogs.JustInfo("Failed to start trainer server.\nRacerMate will not work correctly without a network connection.\n\n" + e.ToString(), "OK", "Cancel");
				info.ShowDialog();
				return false;
			}

            bool success = true;
			switch (status)
			{
				case 0:
					// no error
					Log.WriteLine("Server started successfully.");
					break;
				case 1:
					// no network
					Log.WriteLine("Server started, but no network is available.");
					break;
				case 2:
					// can't compute broadcast address
					Log.WriteLine("Server started, but an error occurred while computing the broadcast address, so wireless controllers will not work.");
					break;
				case 100:
					Log.WriteLine("Server is disabled in source.");
					break;
				default:
                    // unknown error
                    success = false;
					Log.WriteLine("Server did not start! An unknown error occurred.");
					break;
			}

            if (success)
            {
                if (ms_BackgroundScanningThread == null)
                {
                    ms_BackgroundScanningThread = new Thread(new ThreadStart(ScanningThreadFunc));
                    ms_BackgroundScanningThread.Name = "ScanningThread";
                    ms_BackgroundScanningThread.Start();
                }
            }

			int numAttempts = 0;
			const int maxAttempts = 10;
			while (!StartedANT && numAttempts < maxAttempts)
			{
				++numAttempts;
				if (StartANT(debug_level) == false)
				{
					DLL.stop_ant();
					//RacerMateOne.Dialogs.JustInfo info = new RacerMateOne.Dialogs.JustInfo("Failed to start ANT+ listener.\nRacerMate will not correctly detect ANT+ sensors.", "OK", "Cancel");
					//info.ShowDialog();
				}
			}


			////--------------------------------------------------------------------------
			//// for testing, I block up to 10 seconds until there are some ANT sensors
			//// this takes 7 to 8 seconds once the ANT stick sees a heartrate sensor,
			//// sort of like it takes a few seconds for the communication to establish
			//// for a UDP computrainer.
			////--------------------------------------------------------------------------
			//Thread.Sleep(8 * 1000);

			//SENSORS sensors;
			//sensors.sensorCount = 0;
			//sensors.sensors = null;
			//string[] sensor_strings;
			//while (sensors.sensorCount == 0)
			//{
			//	sensor_strings = DLL.GetANTSensorString();
			//	if (sensor_strings != null)
			//	{
			//		break;
			//	}
			//	//sensors = DLL.get_ant_sensors();
			//	//if (sensors)
			//	//{
			//	//	if (sensors->n > 0)
			//	//	{
			//	//		break;
			//	//	}
			//	//}
			//	Thread.Sleep(50);
			//}

			// just some testing/exercises

			////			if (sensors && sensors->n > 0)
			//			{
			////				string[] sensor_strings = DLL.GetANTSensorString();
			//				//DLL.associate("UDP-5678", sensors->sensors[0].sn);
			//				//DLL.associate("UDP-5678", sensors->sensors[0].sn);

			//				//status = DLL.unassociate("UDP-5678", sensors->sensors[0].sn);
			//				//status = DLL.unassociate("UDP-5678", sensors->sensors[0].sn);
			//				//status = DLL.unassociate("xxx", sensors->sensors[0].sn);
			//			}


#if DEBUG
			System.IO.File.Delete("client.log");
			String s;
			//IntPtr pfullpath = Marshal.StringToHGlobalAnsi(RacerMatePaths.DebugFullPath);
			s = ".";
			IntPtr pfullpath = Marshal.StringToHGlobalAnsi(s);
			int _status_logpath = DLL.Setlogfilepath(pfullpath);
			//Marshal.FreeBSTR(pfullpath);
			//DLL.Enablelogs(false, false, false, false, false, false);			// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			DLL.Enablelogs(true, true, true, true, true, true);

			s = APIVersion;											// "1.1.0"			<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			s = DLLVersion;											// "1.0.13"			<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			s = ms_DLLVersion;										// "1.0.13"			<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			s = dll_build_date;                             // "Apr 14 2015 11:39:59"			<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			//Log.WriteLine("RM1() static constructor");
			Debug.WriteLine(s, "build date");

			bp = 0;
#endif
			return true;
		}											// Initialize_Server()


#if DEBUG
		static private int bp = 0;
#endif
		static public int MAX_FRONT_GEARS = 3;
		static public int MAX_REAR_GEARS = 11;
		static public int MAX_FRONT_GEARS_SPACE = 5;
		static public int MAX_REAR_GEARS_SPACE = 20;
		public const int BarCount = 24;
		public const int HalfBarCount = 12;
		public const double SecondsPerHour = (60 * 60);
		public const double iSecondsPerHour = (1.0 / (60 * 60));
		public const float DraftWind = (float)(-10.0 * ConvertConst.MilesToKilometers);
		public const double TicksPerSecond = 10000000.0;
		public const UInt32 TicksPerSecond_Int = 10000000;
		public const double SecondsPerTick = 0.0000001;
		public const double UpdateFPS = 30.0;
		static public readonly double[] BarRadians;
		static public float INCHES_TO_MM = 25.4f;
		static public float MM_TO_INCHES = 0.039370078740f;
		static public float LBS_TO_KGS = 0.45359237f;
		static public float METER_TO_MILE = 0.000621371192f;
		public const float c_WeirdCadence = 254.99f;							// Fix for a weird cadence of 255.
		public const float c_WeirdCadenceSpeed = (float)(5 * ConvertConst.MilesToKilometers);
		public struct GP {										// gearpair
			public int Front;
			public int Rear;
		}

		public struct TD {									// TrainerData
			public float Speed;
			public float Cadence;
			public float HR;
			public float Power;
		};

		public static bool StartedANT
		{
			get; private set;
		}

		/// <summary>
		/// Attempts to call start_ant from the racermate DLL; handles exceptions.
		/// Sets StartedANT property if ant is started successfully; returns same value.
		/// </summary>
		/// <param name="debug_level"></param>
		/// <returns></returns>
		public static bool StartANT(int debug_level)
		{
			try
			{
				int status = DLL.start_ant(debug_level);
				StartedANT = (status == 0);
			}
			catch (Exception e)
			{
				Log.WriteLine(e.ToString());
				StartedANT = false;
			}
			return StartedANT;
		}

		//---------------------------------------------------------------------
		// These ANT+ Sensor related structs are defined by the racermate DLL.
		// DO NOT MODIFY them here unless they are also modified in the DLL.
		public const int MAXSENSORS = 32;

		public struct SENSOR
		{
			public ushort sensor_number;// = 0xffff;
			public byte assigned_channel;// = 0xff;
			public byte val;// = 0;
			public byte type;// = 0x00;
			public byte transtype;// = 0x00;
		};

		public struct SENSORS
		{
			public byte sensorCount;
			public SENSOR[] sensors;
		};

		public static List<SENSOR> GetAntSensorList()
		{
			List<SENSOR> sensorList = new List<SENSOR>();
			string[] sensorStrings = DLL.GetANTSensorString();

#if DEBUG
			//// For testing HR sensors
			//if (sensorStrings == null)
			//{
			//	sensorStrings = new string[6];
			//	sensorStrings[0] = "11111 120";
			//	sensorStrings[1] = "22222 120";
			//	sensorStrings[2] = "22333 120";
			//	sensorStrings[3] = "33333 120";
			//	sensorStrings[4] = "44444 120";
			//	sensorStrings[5] = "55555 120";
			//	//sensorStrings[6] = "65535 120"; // this is max value for sensor ID
			//	//sensorStrings[7] = "777 120";
			//}
#endif // DEBUG

			if (sensorStrings != null)
			{
				foreach (string sensorString in sensorStrings)
				{
					string[] sensorParts = sensorString.Split(' ');
					if (sensorParts.Length == 2)
					{
						SENSOR sensor = new SENSOR();
						if (ushort.TryParse(sensorParts[0], out sensor.sensor_number))
						{
							sensor.type = byte.Parse(sensorParts[1]);
							sensorList.Add(sensor);
						}
					}
				}
			}

			return sensorList;
		}

		public SENSORS antSensors;
		// end ANT+ Sensor structs
		//---------------------------------------------------------------------

		public enum TRAINER_COMMUNICATION_TYPE {
			BAD_INPUT_TYPE = -1,
			WIN_RS232,
			UNIX_RS232,
			TRAINER_IS_SERVER,									// trainer is a server
			TRAINER_IS_CLIENT									// trainer is a client
		};

		public struct SSD {								// SpinScanData
			public float ss;
			public float lss;
			public float rss;
			public float lsplit;
			public float rsplit;
		};
		public delegate void IStatsEvent(IStats istats, object arguments);	// Depends on the event.
		public interface IStats {
			bool Metric { get; }
			Int64 Ticks { get; }
			float SplitTime { get; }
			float Speed { get; }
			float Cadence { get; }
			float HeartRate { get; }
			float Watts { get; }

			float SS { get; }
			float SSLeft { get; }
			float SSRight { get; }
			float SSLeftSplit { get; }
			float SSRightSplit { get; }

			float Calories { get; }
			float PulsePower { get; }
			float NP { get; }
			float IF { get; }
			float TSS { get; }

			float[] Bars { get; }
			float[] AverageBars { get; }

			int FrontGear { get; }	// Velotron only -1 if not valid
			int RearGear { get; }	// Velotron only -1 if not valie
			int GearInches { get; } // Velotron only - We should be able to caluclate this for non-computrainers.

			float Grade { get; set; }
			float Watts_Load { get; set; }
			float Wind { get; set; }
			bool Drafting { get; set; }

			event IStatsEvent OnUpdate;
		}

		public interface IStatsEx : IStats {
			double Time { get; }

			float Speed_Avg { get; }
			float Speed_Max { get; }
			double SpeedDisplay { get; }
			float Watts_Avg { get; }
			float Watts_Max { get; }
			float HeartRate_Avg { get; }
			float HeartRate_Max { get; }
			float Cadence_Avg { get; }
			float Cadence_Max { get; }
			float SSLeftATA { get; }
			float SSRightATA { get; }
			float SSLeft_Avg { get; }
			float SSRight_Avg { get; }
			float SS_Avg { get; }
			String GearingString { get; }
		}

		public enum DeviceType  {
			NOT_SCANNED = 0,				// unknown, not scanned
			DOES_NOT_EXIST,				// serial port does not exist
			EXISTS,							// exists, openable, but no RM device on it
			COMPUTRAINER,
			VELOTRON,
			SIMULATOR,
			PERF_FILE,
			ACCESS_DENIED,					// port present but can't open it because something else has it open
			OPEN_ERROR,						// port present, unable to open port
			NO_URL_AND_OR_TCP_PORT,
			SERVER_NOT_RUNNING,
			OTHER_ERROR						// prt present, error, none of the above
		}

		// Modified for v 155 Feb 2012 by Smeulders to align with dll EnumDeviceType, add Simulator and Perf_File

		public static string[] DeviceNames =  {
			"Not scanned",
			"Does not exists",
			"Exists",
			"CompuTrainer",
			"Velotron",
			"simulator",
			"perf_file",
			"Access denied",
			"Open error",
			"NO_URL_AND_OR_TCP_PORT",
			"SERVER_NOT_RUNNING",
			"Other Error"
		};

		// modified DeviceNames feb 2012 Smeulders to align with the Enum DeviceType
		//modified enum DLLError for v 155 Smeulders Feb 12 2012 to align with dll 1.0.10

		public enum DLLError : uint  {
			ALL_OK = 0,
			DEVICE_NOT_RUNNING = 0x80000000, // INT_MIN,			// 0x80000000
			WRONG_DEVICE,							// 0x80000001
			DIRECTORY_DOES_NOT_EXIST,
			DEVICE_ALREADY_RUNNING,
			BAD_FIRMWARE_VERSION,
			VELOTRON_PARAMETERS_NOT_SET,
			BAD_GEAR_COUNT,
			BAD_TEETH_COUNT,
			PORT_DOES_NOT_EXIST,
			PORT_OPEN_ERROR,
			PORT_EXISTS_BUT_IS_NOT_A_TRAINER,
			DEVICE_RUNNING,
			BELOW_UPPER_SPEED,
			ABORTED,
			TIMEOUT,
			BAD_RIDER_INDEX,
			DEVICE_NOT_INITIALIZED,
			CAN_NOT_OPEN_FILE,
			GENERIC_ERROR
		};

		//modified enum DLLError for v 155 Smeulders Feb 12 2012 to align with dll 1.0.10 this appears to never be used.

		public Dictionary<DLLError, String> DLLErrorText = new Dictionary<DLLError, String>()  {
			{DLLError.ALL_OK,"OK"},
				{DLLError.DEVICE_NOT_RUNNING,"Not running"},
				{DLLError.WRONG_DEVICE, "Wrong device" },
				{DLLError.DIRECTORY_DOES_NOT_EXIST, "Directory does not exist"},
				{DLLError.DEVICE_ALREADY_RUNNING,"Device already running"},
				{DLLError.BAD_FIRMWARE_VERSION,"Bad firmware version"},
				{DLLError.VELOTRON_PARAMETERS_NOT_SET,"Velotron parameters not set"},
				{DLLError.BAD_GEAR_COUNT,"Bad gear count"},
				{DLLError.BAD_TEETH_COUNT,"Bad teeth count"},
				{DLLError.PORT_DOES_NOT_EXIST,"Ports does not exist"},
				{DLLError.PORT_OPEN_ERROR,"Port open error"},
				{DLLError.PORT_EXISTS_BUT_IS_NOT_A_TRAINER,"Port Exists but is not a trainer"}, 
				{DLLError.DEVICE_RUNNING,"Device Running"}, 
				{DLLError.BELOW_UPPER_SPEED,"Below upper speed"}, 
				{DLLError.ABORTED,"Aborted"}, 
				{DLLError.TIMEOUT,"Timeout"}, 
				{DLLError.BAD_RIDER_INDEX,"Bad rider index"}, 
				{DLLError.DEVICE_NOT_INITIALIZED,"Device not initialized"}, 
				{DLLError.CAN_NOT_OPEN_FILE,"Cannot open file"}, 
				{DLLError.GENERIC_ERROR,"Generic error"}
		};

		/// <summary>
		/// The valid pad keys.
		/// </summary>
		public enum PadKeys : byte {
			F1,
			F2,
			F3,
			F4,
			F5,
			F6,
			UP,
			DOWN,
			FN_UP,
			FN_DOWN,
			NO_COMMUNICATION,
			MAX,

			// Happens when a press is < 1 second.
			F1_Short,
			F2_Short,
			F3_Short,
			F4_Short,
			F5_Short,
			F6_Short,
			UP_Short,
			DOWN_Short,
			FN_UP_Short,
			FN_DOWN_Short,

			// After KeyLongSeconds seconds pressed we get this message
			F1_Long,
			F2_Long,
			F3_Long,
			F4_Long,
			F5_Long,
			F6_Long,
			UP_Long,
			DOWN_Long,
			FN_UP_Long,
			FN_DOWN_Long,

			// Every KeyRepeatSeconds after the long we get one of thes..
			F1_Repeat,
			F2_Repeat,
			F3_Repeat,
			F4_Repeat,
			F5_Repeat,
			F6_Repeat,
			UP_Repeat,
			DOWN_Repeat,
			FN_UP_Repeat,
			FN_DOWN_Repeat,

			NOKEY
		};

		public enum State  {
			Unknown,
			Initializing,
			Stopped,
			Starting,
			Running
		};

		[Flags]
			public enum StatusFlags  {
				Zero = 0,
				CadencePickup = (1 << 11),		// 11         cadence pickup
				HeartrateSensor = (1 << 10),	// 10         heart rate sensor
				ProVersion = (1 << 9),			//  9         pro or + version, 1 = pro
				DragFactorOperating = (1 << 8),	//  8         1 if drag factor is operating
				Valid = (1 << 0)
			}

		[Flags]
			public enum TrainerChangedFlags  {
				Zero = 0,
				Calibrate = (1 << 0),

				Mask = (1 << 1)
			};
		public int _status_logpath;

		private static String ms_DLLVersion;
		public static String DLLVersion  {
			get  {
				if (ms_DLLVersion == null)  {
						IntPtr iptr = DLL.get_dll_version();
					ms_DLLVersion = Marshal.PtrToStringAnsi(iptr);
				}
				return ms_DLLVersion;
			}
		}										// public static String DLLVersion

		/********************************************************************************************************************

		********************************************************************************************************************/

		private static String ms_APIVersion;
		public static String APIVersion {
			get  {
				if (ms_APIVersion == null) {
						IntPtr iptr = DLL.GetAPIVersion();										// <<<<<<<<<<<<<<<<<<<<<<
					ms_APIVersion = Marshal.PtrToStringAnsi(iptr);
				}
				return ms_APIVersion;
			}
		}										// public static String APIVersion

		/********************************************************************************************************************

		********************************************************************************************************************/

		private static String ms_dll_build_date;
		public static String dll_build_date {
			get {
				if (ms_dll_build_date == null) {
					IntPtr iptr = DLL.get_build_date();
					ms_dll_build_date = Marshal.PtrToStringAnsi(iptr);
				}
				return ms_dll_build_date;
			}
		}												// public static String dll_build_date

		// Lazy Thread safe singleton implementation

		public static RM1 Instance {
			get {
				return Nested.instance;
			}
		}

		/*****************************************************************************************************

		*****************************************************************************************************/

		public static string [] GetPortNames()  {
			string[] s;

			try {
				string cwd = Directory.GetCurrentDirectory();						// in .../bin/Debug!!
				IntPtr iptr = DLL.getPortNames();
				string s2 = Marshal.PtrToStringAnsi(iptr);
				s = s2.Split(' ');
			}
			catch (Exception e) {
				s = null;
#if DEBUG
				string s2 = e.ToString();
				s2 += "\n\ncwd = ";
				s2 += Directory.GetCurrentDirectory();
				s2 += "\n";
				s2 += e.ToString();
				Log.WriteLine(s2);
				System.Console.WriteLine("'{0}'\n", s2);
#else
				Log.WriteLine(e.ToString());
#endif
			}
			return s;
		}                       // GetPortNames()

		/*****************************************************************************************************

		*****************************************************************************************************/

		public static string[] get_udp_trainers()  {
			string[] s = null;

			try
			{
				string cwd = Directory.GetCurrentDirectory();                  // in .../bin/Debug!!
				IntPtr iptr = DLL.get_udp_trainers();
				string s2 = Marshal.PtrToStringAnsi(iptr);
				if (s2 != null)
				{
					s = s2.Trim().Split(' ');
				}
			}
			catch (Exception e)   {
				s = null;
#if DEBUG
				//string s2 = e.ToString();
				//s2 += "\n\ncwd = ";
				//s2 += Directory.GetCurrentDirectory();
				//s2 += "\n";
				//s2 += e.ToString();
				//Log.WriteLine(s2);
				//System.Console.WriteLine("'{0}'\n", s2);
#else
				Log.WriteLine(e.ToString());
#endif
			}
			return s;
        }                       // get_udp_trainers()

        /*****************************************************************************************************

		*****************************************************************************************************/

        public static DeviceType GetRacerMateDeviceID(string portName)  {
			//if (portnum == 6)
			//	return DeviceType.OPEN_ERROR;
			DeviceType id;
			try  {
				id = (DeviceType)DLL.GetRacerMateDeviceID(portName, 1);
			}
			catch (Exception e)  {
				Log.WriteLine(e.ToString());
				id = DeviceType.OPEN_ERROR;
			}
			//Log.WriteLine("opened " + PortName + " got " + id.ToString());
			return id;
		}

		public delegate void TrainerEvent(Trainer trainer, object arguments);	// Depends on the event.
		public static event TrainerEvent OnClosed; // Any trainer closed. arguments is null
		public delegate void IntervalEvent(Int64 LastTicks, float SplitTime);
		public static event IntervalEvent OnInterval;
		public delegate void TrainerInitialized(Trainer trainer, int left);
		public static event TrainerInitialized OnTrainerInitialized;
		public delegate void TrainerInitializationComplete();
		public static event TrainerInitializationComplete OnTrainerInitializationComplete;
		public static event TrainerEvent OnCalibrationChanged;
		public delegate void TrainerPadKey(Trainer trainer, RM1.PadKeys key, double pressed);
		public static event TrainerPadKey OnPadKey;
		public delegate void UpdateEvent(double splittime);
		public static event UpdateEvent OnUpdate;
		/// <summary> 
		/// All the trainers' COM / UDP ports.  Not thread safe.   Only  add/remove from main thread.
		/// </summary>
		protected static Dictionary<string, Trainer> ms_Trainers = new Dictionary<string, Trainer>();

		/// <summary>
		/// Trainers that need to be initialized; Uses the ms_mux to add and remove from.
		/// </summary>
		protected static LinkedList<Trainer> ms_InitList = new LinkedList<Trainer>();
		/// <summary>
		/// When a trainer needs to be started add it here.
		/// </summary>
		protected static LinkedList<Trainer> ms_StartList = new LinkedList<Trainer>();
		/// <summary>
		/// All the trainers that have been successfully started.
		/// </summary>
		protected static LinkedList<Trainer> ms_StartedList = new LinkedList<Trainer>();
		/// <summary>
		/// Trainers that are active.  NOT THREAD SAFE.   Only add/remove from main thread.
		/// </summary>
		protected static List<Trainer> ms_ActiveList = new List<Trainer>();
		protected static int ms_ActiveListVersion = 1;
		protected static int ms_HardwareListVersion = 0;
		protected static List<Trainer> ms_HardwareList = new List<Trainer>();
		public static List<Trainer> HardwareList
		{
			get
			{
				if (ms_ActiveListVersion != ms_HardwareListVersion)
				{
					ms_HardwareList.Clear();
					foreach (KeyValuePair<string, RM1.Trainer> kp in ms_Trainers)
					{
						RM1.Trainer t = kp.Value;
						if (t.IsConnected || t.ShouldBe != DeviceType.DOES_NOT_EXIST)
							ms_HardwareList.Add(t);
					}
					ms_HardwareListVersion = ms_ActiveListVersion;
				}
				return ms_HardwareList;
			}
		}										// public static List<Trainer> HardwareList
		/// <summary>
		/// Number of computrainers detected
		/// </summary>
		public static int Computrainers
		{
			get
			{
				int cnt = 0;
				foreach (Trainer t in ms_ActiveList)
				{
					if (t.Type == DeviceType.COMPUTRAINER)
						cnt++;
				}
				return cnt;
			}
		}
		/// <summary>
		/// Number of velotrons detected
		/// </summary>
		public static int Velotrons
		{
			get
			{
				int cnt = 0;
				foreach (Trainer t in ms_ActiveList)
				{
					if (t.Type == DeviceType.VELOTRON)
						cnt++;
				}
				return cnt;
			}
		}


		//=========================================================================================
		protected static Mutex ms_Mux = new Mutex();
		//=========================================================================================
		protected static List<Exception> m_MuxException = new List<Exception>();
		public static void MutexException(Exception ex)  {
			Debug.WriteLine(String.Format("Exception in MUTEX!!!\nThread:{0}\n------------\n{1}\n-------------", Thread.CurrentThread.Name, ex.ToString()) );
			m_MuxException.Add(ex);
		}
		public static int TrainersInitializing  {
			get  {
				int num;
				ms_Mux.WaitOne();
				try  {
					num = ms_InitList.Count;
				}
				catch (Exception ex)  {
					MutexException(ex);
					num = 0;
				}
				ms_Mux.ReleaseMutex();
				return num;
			}
		}
		public static int ValidTrainerCount  {
			get {
				return ms_ActiveList.Count;
			}
		}
		public static List<Trainer> ValidTrainers {
			get {
				return ms_ActiveList;
			}
		}


        /**********************************************************************
        Intended to be a background thread that continually listens for new
        trainers to be available.

        May need to be a customized Thread object so that I can add callbacks
        to notify when new trainers are available.
        **********************************************************************/
        private static Thread ms_BackgroundScanningThread;
        private static bool ms_bShutdownScanningThread = false;
        private static readonly AutoResetEvent ms_ScanningThreadWaitEvent = new AutoResetEvent(false);

        // Event to tell the app if the available trainers have changed, so that the user can rescan if desired.
        public class TrainersAvailableChangedEventArgs : EventArgs
        {
            List<string> m_foundTrainers;
            List<string> m_lostTrainers;
            public TrainersAvailableChangedEventArgs(List<string> foundTrainers, List<string> lostTrainers)
            { this.m_foundTrainers = foundTrainers; this.m_lostTrainers = lostTrainers; }
            public List<string> FoundTrainers { get { return m_foundTrainers; } }
            public List<string> LostTrainers { get { return m_lostTrainers; } }
        }
        public delegate void TrainersAvailableChangedEventHandler(object sender, TrainersAvailableChangedEventArgs e);
        public static event TrainersAvailableChangedEventHandler TrainersAvailableChanged;

        public delegate void OnTrainerFoundEvent(string trainerName);
        public static event OnTrainerFoundEvent OnTrainerFound;

        public delegate void OnTrainerLostEvent(string trainerName);
        public static event OnTrainerLostEvent OnTrainerLost;

        //// Called when all found trainers have had notifications sent
        //public delegate void OnTrainerFoundNotificationsCompleteEvent();
        //public static event OnTrainerFoundNotificationsCompleteEvent OnTrainerLostCompleted;

        //// Called when all lost trainers have had notifications sent
        //public delegate void OnTrainerLostNotificationsCompleteEvent();
        //public static event OnTrainerLostNotificationsCompleteEvent OnTrainerLostCompleted;

        // MAYBE not needed?
        //public delegate void OnTrainerLostAndRefoundEvent(string );
        //public static event OnTrainerLostAndRefoundEvent OnTrainerLostAndRefound;

        // Protects against multiple threads interacting with the ScanningThread's trainer lists.
        private static Mutex ms_scanningThread_ListMutex = new Mutex(false);

		// List of newly found trainers that have NOT yet had a notification sent
		private static List<string> ms_scanningThread_UnnotifiedTrainers = new List<string>();

        // List of trainers that we have previously sent a notification for and should still be active
        private static List<string> ms_scanningThread_NotifiedTrainers = new List<string>();

        // List of trainers that we previously sent a notification for, but are no longer detected
        private static List<string> ms_scanningThread_LostTrainers = new List<string>();
		
        //// HMMM: Maybe this list isn't needed _IF_ the trainer is in both the LOST list and the FOUND list?
        //// List of trainers that we previously sent a notification for, but they were both lost AND re-found since we sent the last notification.
        //// The higher level app may need to special handle this case, so it gets its own notifications
        //private static List<string> ms_scanningThread_LostAndRefoundTrainers;

		// Resets lists used by the scanning thread so that all trainers will be considered new.
		public static void ScanningThread_Reset()
		{
			ms_scanningThread_ListMutex.WaitOne();
			ms_scanningThread_UnnotifiedTrainers.Clear();
			ms_scanningThread_NotifiedTrainers.Clear();
			ms_scanningThread_LostTrainers.Clear();
			ms_scanningThread_ListMutex.ReleaseMutex();
		}

        private static void ScanningThreadFunc()
        {
            int previousLostCount = 0;
            int previousFoundCount = 0;

            // Enter the loop every 1 second(s)
            while(!ms_ScanningThreadWaitEvent.WaitOne(1 * 1000) && !ms_bShutdownScanningThread)
            {
#if DEBUG
                Log.WriteLine("ScanningThread: GetPortNames()");
#endif
                string[] foundTrainers = GetPortNames();
                if (foundTrainers == null)
                {
                    // nothing to add, so just continue to the next loop iteration
                    continue;
                }

#if DEBUG
                Log.WriteLine("ScanningThread: Found: " + foundTrainers.ToString());
#endif

                // protect the lists
                ms_scanningThread_ListMutex.WaitOne();

                // Order of operations (adding / removing from lists) is VERY important here!

                // First, remove things that are MISSING from the list.
                // The found trainer names may be MISSING some trainers that we've previously found and possibly notified 
                // in the past (so remove them from the notified list and add to the lost list)
                // TODO? if a trainer was in the lost list (ie, it was previously notified and existed to the higher level app, and has since been lost and we haven't informed the app of the loss)
                // but has been found again, we need to tell the UI of this because it may need a 'bigger' refresh of higher level data structures.
                List<string> prevNotifiedList = new List<string>(ms_scanningThread_NotifiedTrainers);
                foreach (string notifiedTrainer in prevNotifiedList)
                {
                    if (foundTrainers.Contains(notifiedTrainer) == false)
                    {
                        ms_scanningThread_NotifiedTrainers.Remove(notifiedTrainer);
						ms_scanningThread_LostTrainers.Add(notifiedTrainer);
                    }
                }
				
				// Sort through the found trainer names:
				// If they have NOT already been notified, then they should be in the unnotified list.
				// We are basically rebuilding the unnotified list.
                ms_scanningThread_UnnotifiedTrainers.Clear();
                foreach (string trainerName in foundTrainers)
                {
                    if (ms_scanningThread_NotifiedTrainers.Contains(trainerName) == false)
                    {
                        ms_scanningThread_UnnotifiedTrainers.Add(trainerName);
                    }
                }

                // Alert if there are either newly found or lost trainers
                if (TrainersAvailableChanged != null &&
                    (ms_scanningThread_UnnotifiedTrainers.Count != previousFoundCount ||
                     ms_scanningThread_LostTrainers.Count != previousLostCount))
                {
                    previousFoundCount = ms_scanningThread_UnnotifiedTrainers.Count;
                    previousLostCount = ms_scanningThread_LostTrainers.Count;
                    TrainersAvailableChanged(null, new TrainersAvailableChangedEventArgs(ms_scanningThread_UnnotifiedTrainers, ms_scanningThread_LostTrainers));
                }

                // If we have event handlers, then send notifications as necessary and update the lists!
                // First notify the lost trainers
                if (OnTrainerLost != null)
                {
                    foreach(string lostTrainer in ms_scanningThread_LostTrainers)
                    {
                        OnTrainerLost(lostTrainer);
                    }

                    ms_scanningThread_LostTrainers.Clear();
                }

                // Now notify the new trainers
                if (OnTrainerFound != null)
                {
                    foreach (string unnotifiedTrainer in ms_scanningThread_UnnotifiedTrainers)
                    {
                        OnTrainerFound(unnotifiedTrainer);
                        ms_scanningThread_NotifiedTrainers.Add(unnotifiedTrainer);
                    }

                    ms_scanningThread_UnnotifiedTrainers.Clear();
                }

				// other threads are safe to affect the lists now
				ms_scanningThread_ListMutex.ReleaseMutex();
			}
#if DEBUG_LOG_ENABLED
			Log.WriteLine("ScanningThread: exited");
#endif
        }

        //===========================================================================
        // This will be used by the OnTrainerFoundEvent and OnTrainerLostEvent delegate 
        // to add the new-found trainer to the list that needs to be init'd.
        // Even Lost trainers need to be re-init'd so that we update the fact that it
        // is no longer connected.
		//===========================================================================
		private static void trainerFoundEventHandler_addTrainerToInitList(string trainerName)
		{
			ms_Mux.WaitOne();

			try
			{
				Log.WriteLine(string.Format("Adding '{0}' to InitList", trainerName));
				Trainer trainer = Trainer.Get(trainerName);
				if (!ms_InitList.Contains(trainer))
				{
					ms_InitList.AddLast(trainer);
				}
			}
			catch (Exception ex)
			{
				MutexException(ex);
			}

			ms_Mux.ReleaseMutex();
		}
        
        /**********************************************************************************************************
        // Enables trainer detection notification for 1 second and then refreshes the hardware list once all the
        // changed trainers have been reinitialized.
        **********************************************************************************************************/
        public static void StartQuickScan()
        {
            if (m_bScanning)
                return;

            m_bScanning = true;

            // protect OnTrainerFound (using same mutex as where we actually call that event
			ms_scanningThread_ListMutex.WaitOne();
            RM1.OnTrainerLost += RM1.trainerFoundEventHandler_addTrainerToInitList;
            RM1.OnTrainerFound += RM1.trainerFoundEventHandler_addTrainerToInitList;
			ms_scanningThread_ListMutex.ReleaseMutex();


            // always include the settings
            {
                foreach (TrainerUserConfigurable tc in RM1_Settings.ActiveTrainerList)
                {
                    // NOTE: getting the trainer will also put it in the InitList if it's a new trainer
                    Trainer t = Trainer.Get(tc.SavedPortName);
                    t.ShouldBe = tc.DeviceType;
                    ms_HardwareListVersion = 0; // Redo the hardware list next time it is requested.
                }

                if (RM1_Settings.General.DemoDevice)
                {
                    AddFake();
                }
            }

            // Enable the timer to signal the FullScanComplete call after 1 seconds.
            ms_fullScanTimer = new System.Windows.Forms.Timer();
            ms_fullScanTimer.Interval = 1 * 1000;
            ms_fullScanTimer.Tick += FullScanComplete;
            ms_fullScanTimer.Start();
        }

        /**********************************************************************************************************
			called from 'Rescan All Hardware' at beginning of program load
				and 'Scan For New Devices'
		**********************************************************************************************************/
        private static bool m_bScanning = false;
        private static System.Windows.Forms.Timer ms_fullScanTimer;
		public delegate void ScanCompleteEventHandler();
		public static event ScanCompleteEventHandler OnScanCompleteEvent;
		public static void StartFullScan(bool bIncludeSettings, ScanCompleteEventHandler scanCompleteEventHandler)  {
            if (m_bScanning)
                return;

            m_bScanning = true;

            Log.WriteLine("Starting full scan...");

			if (scanCompleteEventHandler != null)
			{
				OnScanCompleteEvent += scanCompleteEventHandler;
			}

			ms_scanningThread_ListMutex.WaitOne();
			OnTrainerLost += trainerFoundEventHandler_addTrainerToInitList;
			OnTrainerFound += trainerFoundEventHandler_addTrainerToInitList;
			ms_scanningThread_ListMutex.ReleaseMutex();

            if (bIncludeSettings)
            {
                foreach (TrainerUserConfigurable tc in RM1_Settings.ActiveTrainerList)
                {
                    // NOTE: getting the trainer will also put it in the InitList if it's a new trainer
                    Trainer t = Trainer.Get(tc.SavedPortName);
                    t.ShouldBe = tc.DeviceType;
                    ms_HardwareListVersion = 0; // Redo the hardware list next time it is requested.
                }

                if (RM1_Settings.General.DemoDevice)
                {
                    AddFake();
                }
            }

            // Enable the timer to signal the FullScanComplete call after 10 seconds.
            ms_fullScanTimer = new System.Windows.Forms.Timer();
			ms_fullScanTimer.Interval = 10 * 1000;
			ms_fullScanTimer.Tick += FullScanComplete;
			ms_fullScanTimer.Start();
		}

		private static void FullScanComplete(object sender, EventArgs args)
		{
			Log.WriteLine("...Checking full scan");
			ms_Mux.WaitOne();
			int numTrainersInitializing = RM1.ms_InitList.Count;
			ms_Mux.ReleaseMutex();
			if (numTrainersInitializing == 0)
			{
				Log.WriteLine("...Completed full scan!");
			    ms_scanningThread_ListMutex.WaitOne();
                OnTrainerLost -= trainerFoundEventHandler_addTrainerToInitList;
                OnTrainerFound -= trainerFoundEventHandler_addTrainerToInitList;
			    ms_scanningThread_ListMutex.ReleaseMutex();

                ms_fullScanTimer.Stop();
                ms_fullScanTimer.Dispose();

                if (RM1.OnScanCompleteEvent != null)
				{
					RM1.OnScanCompleteEvent();
					RM1.OnScanCompleteEvent = null;
				}

                m_bScanning = false;
            }
        }


		/**********************************************************************************************************

		**********************************************************************************************************/

		public static void ClearAllTrainers()  {
			foreach (KeyValuePair<string, Trainer> entry in ms_Trainers)  {
				entry.Value.Close();
			}
			ms_Trainers.Clear();
			while (TrainersInitializing > 0)  {
				Thread.Sleep(100);
			}
		}

		/**********************************************************************************************************

		**********************************************************************************************************/

		public static void AddFake()
		{
			RM1.Trainer.Get("Fake");
		}

		/////////////////////////////// C L A S S E S /////////////////////////////

		/******************************************************************************************************************
			Private class for the singleton
		******************************************************************************************************************/

		class Nested {
			static Nested() { }
			internal static readonly RM1 instance = new RM1();
		}

		/******************************************************************************************************************

		******************************************************************************************************************/

		/// <summary>
		/// Direct DLL routines 
		/// </summary>
		protected static class DLL {

			// [DllImport("racermate.dll")] public static extern IntPtr get_errst_r(int err);
			[DllImport("racermate.dll")]
			public static extern IntPtr get_errstr(int err);
			
			//[DllImport("racermate.dll")] public static extern int Setlogfilepat_h(IntPtr pathtosafefolder);
			[DllImport("racermate.dll")]
			public static extern int Setlogfilepath(IntPtr psf);

			//[DllImport("racermate.dll")] public static extern int Enablelog_s(bool bikelog, bool courselog, bool decoder, bool ds, bool gears, bool physics);
			[DllImport("racermate.dll")]
			public static extern int Enablelogs(bool bg, bool cg, bool dr, bool ds, bool gs, bool ps);

			// [DllImport("racermate.dll")] public static extern IntPtr GetAPIVersio_n();
			[DllImport("racermate.dll")]
			public static extern IntPtr GetAPIVersion();

			// [DllImport("racermate.dll")] public static extern IntPtr get_build_date();
			[DllImport("racermate.dll")]
			public static extern IntPtr get_build_date();

			//[DllImport("racermate.dll")] public static extern int GetRacerMateDeviceI_D(Int32 portnum);  //returned is enum DeviceType
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int GetRacerMateDeviceID([MarshalAs(UnmanagedType.LPStr)] string portName, Int32 dummy);  //returned is enum DeviceType
			//[DllImport("racermate.dll")] public static extern int GetFirmWareVersio_n(Int32 portnum);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int GetFirmWareVersion([MarshalAs(UnmanagedType.LPStr)] string portName);
			//[DllImport("racermate.dll")] public static extern byte GetIsCalibrate_d(int ix, int FirmwareVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern byte GetIsCalibrated([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			// [DllImport("racermate.dll")] public static extern int GetCalibratio_n(int ix);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int GetCalibration([MarshalAs(UnmanagedType.LPStr)] string portName);
			//[DllImport("racermate.dll")] public static extern uint startTraine_r(int ix, IntPtr course);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern uint startTrainer([MarshalAs(UnmanagedType.LPStr)] string portName, IntPtr cse, int b);
			// Formally has optionals: int startTrainer(int ix, Course *_course=NULL, LoggingType _logging_type=NO_LOGGING); //
			//[DllImport("racermate.dll")] public static extern int start_traine_r(int ix, bool _b);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int start_trainer([MarshalAs(UnmanagedType.LPStr)] string portName, bool _b); // NEW
			//[DllImport("racermate.dll")] public static extern uint stopTraine_r(int ix);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern uint stopTrainer([MarshalAs(UnmanagedType.LPStr)] string portName);
			//[DllImport("racermate.dll")] public static extern TrainerData GetTrainerDat_a(int ix, int FirmwareVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern TD GetTrainerData([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			//[DllImport("racermate.dll")] public static extern int SetErgModeLoa_d(int ix, int FirmwareVersion, int RRC, float Load);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int SetErgModeLoad([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123, int R, float L);
			// [DllImport("racermate.dll")] public static extern int resetTraine_r(int ix, int FirmwareVersion, int RRC); // V
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int resetTrainer([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123, int R); // V
			//[DllImport("racermate.dll")] public static extern int SetSlop_e(int ix, int FirmwareVersion, int RRC, float bike_kgs, float person_kgs, int DragFactor, float slope);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int SetSlope([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123, int R, float bs, float ps, int DFr, float se);
			//[DllImport("racermate.dll")] public static extern int SetHRBeepBound_s(int ix, int FirmwareVersion, int LowBound, int HighBound, bool BeepEnabled);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int SetHRBeepBounds([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123, int LB, int HB, bool BE);
			//[DllImport("racermate.dll")] public static extern int GetHandleBarButton_s(int ix, int FirmwareVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int GetHandleBarButtons([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			//[DllImport("racermate.dll")] public static extern int SetRecalibrationMod_e(int ix, int FirmwareVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int SetRecalibrationMode([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			//[DllImport("racermate.dll")] public static extern int EndRecalibrationMod_e(int ix, int FirmwareVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int EndRecalibrationMode([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			// [DllImport("racermate.dll")] public static extern int setPaus_e(int ix, bool _paused);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int setPause([MarshalAs(UnmanagedType.LPStr)] string portName, bool _pa);

			// [DllImport("racermate.dll")] public static extern int ResettoIdl_e(int ix);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int ResettoIdle([MarshalAs(UnmanagedType.LPStr)] string portName);
			//[DllImport("racermate.dll")] public static extern int ResetAlltoIdl_e();
			[DllImport("racermate.dll")]
			public static extern int ResetAlltoIdle();

			//[DllImport("racermate.dll")] public static extern int get_accum_td_c(int ix, int FirmwareVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int get_accum_tdc([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			//[DllImport("racermate.dll")] public static extern int get_td_c(int idx, int FirmwareVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int get_tdc([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			/* [DllImport("racermate.dll")] public static extern uint SetVelotronParameter_s(int ix,
				int FWVersion,
				int nfront,
				int nrear,
				IntPtr Chainrings,
				IntPtr cogset,
				float wheeldiameter_mm,            // mm
				int ActualChainring,
				int Actualcog,
				float bike_kgs,
			//float person_kgs,
			int front_index,
			int rear_index
			); */
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern uint SetVelotronParameters(
					[MarshalAs(UnmanagedType.LPStr)] string portName,
					int abc_123,
					int nf,
					int nr,
					IntPtr Chgs,
					IntPtr cgst,
					float wd_mm,            // mm
					int ACh,
					int Acg,
					float bgs,
				//float person_kgs,
					int f_i,
					int r_i
					);
			//[DllImport("racermate.dll")] public static extern GearPair GetCurrentVTGea_r(int ix, int FWVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern GP GetCurrentVTGear([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			//[DllImport("racermate.dll")] public static extern int setGea_r(int ix, int FWVersion, int front_index, int rear_index);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int setGear([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123, int f_i, int r_i);

			//[DllImport("racermate.dll")] public static extern IntPtr get_bar_s(int ix, int FWVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern IntPtr get_bars([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			// [DllImport("racermate.dll")] public static extern IntPtr get_average_bar_s(int ix, int FWVersion);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern IntPtr get_average_bars([MarshalAs(UnmanagedType.LPStr)] string portName, int abc_123);
			//[DllImport("racermate.dll")] public static extern SpinScanData get_ss_dat_a(int ix, int fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern SSD get_ss_data([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt);

			//[DllImport("racermate.dll")] public static extern IntPtr get_dll_versio_n();
			[DllImport("racermate.dll")]
			public static extern IntPtr get_dll_version();

			// [DllImport("racermate.dll")] public static extern float get_calorie_s(int ix, int fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern float get_calories([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt);
			//[DllImport("racermate.dll")] public static extern float get_n_p(int ix, int fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern float get_np([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt);						// returns -1.0f if device not initialized
			//[DllImport("racermate.dll")] public static extern float get_i_f(int ix, int fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern float get_if([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt);						// returns -1.0f if device not initialized
			//[DllImport("racermate.dll")] public static extern float get_ts_s(int ix, int fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern float get_tss([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt);					// returns -1.0f if device not initialized
			//[DllImport("racermate.dll")] public static extern float get_p_p(int ix, int fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern float get_pp([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt);
			//[DllImport("racermate.dll")] public static extern int set_ft_p(int ix, int fw, float ftp);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int set_ftp([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt, float fp);

			//[DllImport("racermate.dll")] public static extern int ResetAverage_s(int ix, int fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int ResetAverages([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt);
			// [DllImport("racermate.dll")] public static extern int set_win_d(int ix, int fw, float _wind_kph);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int set_wind([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt, float _w_k);
			//[DllImport("racermate.dll")] public static extern int set_draftwin_d(int ix, int fw, float _draft_wind_kph);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int set_draftwind([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt, float _d_w_k);

			//[DllImport("racermate.dll")] public static extern int update_velotron_curren_t(int ix, ushort pic_current);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int update_velotron_current([MarshalAs(UnmanagedType.LPStr)] string portName, ushort p_ct);

			//[DllImport("racermate.dll")] public static extern int set_velotron_calibratio_n(int ix, int fw, int _cal);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int set_velotron_calibration([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt, int _c);

			//[DllImport("racermate.dll")] public static extern int check_for_trainer_s(int _ix); 
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int check_for_trainers([MarshalAs(UnmanagedType.LPStr)] string portName);                       //returned is enum DeviceType

			//[DllImport("racermate.dll")] public static extern int velotron_calibration_spindow_n(int _ix, int _fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int velotron_calibration_spindown([MarshalAs(UnmanagedType.LPStr)] string portName, int _fw);

			//[DllImport("racermate.dll")] public static extern int get_status_bit_s(int ix, int fw);
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int get_status_bits([MarshalAs(UnmanagedType.LPStr)] string portName, int qwt);

			//////////////////////////////////////////////////////////////////////////////////////////

			// 20150109
			[DllImport("racermate.dll")]
			public static extern IntPtr getPortNames();

			[DllImport("racermate.dll")]
			public static extern IntPtr get_udp_trainers();

			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int start_server(int _listen_port, int _broadcast_port, [MarshalAs(UnmanagedType.LPStr)] string _myip, int _debug_level);

			[DllImport("racermate.dll")]
			public static extern int racermate_init();

			[DllImport("racermate.dll")]
			public static extern int racermate_close();

			[DllImport("racermate.dll")]
			public static extern int set_port_info(int _ix, IntPtr _name, int _type, int _portnum);

			// should return 0 if OK.
			// returns 1 if libusb could not be initialized.
			[DllImport("racermate.dll")]
			public static extern int start_ant(int _debug_level);

			// always returns 0
			[DllImport("racermate.dll")]
			public static extern int stop_ant();

			// returns a SENSORS structure or NULL if there are no sensors
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern IntPtr get_ant_sensors();

			// returns a C string of sensors in the form "38760 121, 33666 120".
			// Commas separate sensors and spaces separate the sensor serial number and
			// type. Right now I'm only allowing type 120 (heart rate). I created the other function
			// get_ant_sensors() because it was easier/quicker to parse.
			// This returns NULL if no sensors can be found.
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern IntPtr get_ant_sensors_string();

			public static string[] GetANTSensorString()
			{
				string[] sensorString;
				try
				{
					IntPtr iptr = DLL.get_ant_sensors_string();
					string s2 = Marshal.PtrToStringAnsi(iptr);
					if (s2 == null)
					{
						sensorString = null;
					}
					else
					{
						sensorString = s2.Split(',');
					}
				}
				catch (Exception e)
				{
					sensorString = null;
#if DEBUG
					string s2 = e.ToString();
					Log.WriteLine(s2);
					System.Console.WriteLine("'{0}'\n", s2);
#else
					Log.WriteLine(e.ToString());
#endif
				}
				return sensorString;
			} // GetANTSensorString()

			// always returns 0
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int associate([MarshalAs(UnmanagedType.LPStr)] string portname, ushort sensor_number);

			// returns the number of associations undone (0 or 1)
			[DllImport("racermate.dll", CharSet = CharSet.Ansi)]
			public static extern int unassociate([MarshalAs(UnmanagedType.LPStr)] string portname, ushort sensor_number);

		} // class DLL





		/**********************************************************************************************************

		**********************************************************************************************************/

		/// <summary>
		/// Represents one comm port - (PortName) 
		/// </summary>
		public class Trainer : DispatcherObject, INotifyPropertyChanged, IStats {
			[Flags]
			public enum UpdateFlags {
				Zero = 0,
				Grade = (1 << 0),
				Watts = (1 << 1),
				Drag = (1 << 2),
				Wind = (1 << 3),
				Drafting = (1 << 4),
				ResetAverages = (1 << 5),
				Pause = (1 << 6),
				FTP = (1 << 7),
				All = 0xffff
			}
			Object m_UpdateLock = new Object();
			UpdateFlags m_UpdateFlags = UpdateFlags.Zero;
			public void SetUpdateFlags(UpdateFlags flags) {
				lock (m_UpdateLock) {
					m_UpdateFlags |= flags;
				}
			}

			UpdateFlags ClearUpdateFlags(out bool paused) {
				UpdateFlags f;
				lock (m_UpdateLock) {
					paused = m_Paused;
					f = m_UpdateFlags;
					m_UpdateFlags = UpdateFlags.Zero;
				}
				return f;
			}

			public TrainerChangedFlags m_Changed = TrainerChangedFlags.Zero;
			public static string FakePort = "Demo258";
			public static Trainer Fake;
			public RM1.State State { get; protected set; }
			public bool Closed { get; protected set; }
			public readonly string PortName;
			public DeviceType Type { get; protected set; }

			protected bool bFake = false;
			public int FakeKeys;

			/// <summary>
			/// What previous times through have got this.
			/// </summary>
			public DeviceType ShouldBe = DeviceType.DOES_NOT_EXIST;

			public bool IsConnected { get; protected set; }
			public bool IsStarted { get; protected set; }
			public DLLError LastError { get; protected set; }

			protected System.Timers.Timer m_Interval;
			protected VelotronData m_VelotronData;									// in Trainer class
			public VelotronData VelotronData {										// in Trainer class
				get {
					return m_VelotronData;
				}
				set {
					if (!VelotronData.IsEqual(m_VelotronData, value)) {
						m_VelotronData = value;
					}
					SetVelotron_trnr_Parameters();
				}
			}

			public String TypeString {
				get {
					return (Type == DeviceType.COMPUTRAINER ? "CompuTrainer" : Type == DeviceType.VELOTRON ? "Velotron" : "Unknown");
				}
			}

			public int Ver { get; protected set; }
			public int VersionNum { get; protected set; }

			public bool IsCalibrated { get; protected set; }
			public int CalibrationValue { get; protected set; }
			public String CalibrationString {
				get {
					if (!IsConnected)
						return "";
					if (Type == DeviceType.COMPUTRAINER) {
						return String.Format("{0:F2}", CalibrationValue / 100.0);
					}
					//Paul Added feb 5 2012 bugrocket Ticket 11
					else if (Type == DeviceType.VELOTRON) {
						return CalibrationValue.ToString();
					}
					//End Paul Added feb 5 2102
					return "";
				}
			}



			/** <summary>String version or "Invaild" if not valid</summary> */
			public string Version {
				get {
					int v = VersionNum;
					if (v == 0)
						return "Invalid";
					int t = v % 100;
					return "" + (v / 100) + "." + (t < 10 ? "0" : "") + t;
				}
			}


			Rider m_SetRider;
			protected Rider m_Rider;

            public Rider Rider {
                get {
                    return m_SetRider;
                }
                set {
                    bool isNewRider = (m_Rider != value);

                    m_SetRider = value;
                    m_Rider = value != null ? value : Riders.DefaultRider;
                    FTP = m_Rider != null ? (float)m_Rider.PowerFTP : 1.0f;

                    if (Type == DeviceType.VELOTRON && m_Rider != null) {

                        bool needsNewGears = false;

                        if (VelotronData.FrontGear != m_FrontGearNumber ||
                            VelotronData.RearGear != m_RearGearNumber ||
                            VelotronData.Bike_Kg != m_Rider.WeightBikeKGS ||
                            m_Rider.GearingCogset.Count() != VelotronData.CogsetCount ||
                            m_Rider.GearingCrankset.Count() != VelotronData.ChainringsCount)
                        {
                            needsNewGears = true;
                        }
                        else
                        {
                            // check actual Chainring teeth
                            int numGears = m_Rider.GearingCrankset.Count();
                            for (int i = 0; i < numGears; ++i)
                            {
                                if (m_Rider.GearingCrankset[i] != VelotronData.Chainrings[i])
                                {
                                    needsNewGears = true;
                                    break;
                                }
                            }

                            // check actual cogset teeth (if still not sure if we need new gears)
                            if (!needsNewGears)
                            {
                                numGears = m_Rider.GearingCogset.Count();
                                for (int i = 0; i < numGears; ++i)
                                {
                                    if (m_Rider.GearingCogset[i] != VelotronData.Cogset[i])
                                    {
                                        needsNewGears = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (isNewRider || needsNewGears)
                        {
                            VelotronData.Cogset = m_Rider.GearingCogset;
                            VelotronData.Chainrings = m_Rider.GearingCrankset;
                            VelotronData.FrontGear = m_FrontGearNumber;
                            VelotronData.RearGear = m_RearGearNumber;
                            VelotronData.Bike_Kg = m_Rider.WeightBikeKGS;

                            // update the ms_ip_chainrings and ms_ip_cogset
                            SetVelotron_trnr_Parameters();

                            // Tell the Velotron about these new values
                            DLLError derr = (DLLError)DLL.SetVelotronParameters(PortName, Ver,
                                            m_VelotronData.ChainringsCount,
                                            m_VelotronData.CogsetCount,
                                            ms_ip_chainrings,
                                            ms_ip_cogset,
                                            m_VelotronData.WheelDiameter_mm,
                                            m_VelotronData.ActualChainring,
                                            m_VelotronData.ActualCog,
                                            m_VelotronData.Bike_Kg,
                                            m_VelotronData.FrontGear,
                                            m_VelotronData.RearGear
                                            );

                            SetVelotronGears();
                        }

                    }
                    SetUpdateFlags(UpdateFlags.Drag);
				}
			}													// public Rider Rider

			protected float m_FTP = 1.0f;
			public float FTP {
				get { return m_FTP; }
				set {
					if (m_FTP != value) {
						m_FTP = value;
						SetUpdateFlags(UpdateFlags.FTP);
					}
				}
			}

			protected bool m_Paused = false;
			public bool Paused {
				get { return m_Paused; }
				set {
					if (m_Paused == value)
						return;

					m_Paused = value;
					SetUpdateFlags(UpdateFlags.Pause);
				}
			}
			public void SetPaused(bool paused, bool force) {
				if (force || paused != m_Paused) {
					m_Paused = paused;
					SetUpdateFlags(UpdateFlags.Pause);
				}
			}



			protected bool m_bERG;
			protected float m_Grade;
			public float Grade {
				get { return m_Grade; }
				set {
					if (m_bERG || m_Grade != value) {
						m_bERG = false;
						m_Grade = value;
						SetUpdateFlags(UpdateFlags.Grade);
					}
				}
			}
			public void SetGrade(float grade, bool force) {
				if (force || m_Grade != grade || m_bERG) {
					m_bERG = false;
					m_Grade = grade;
					SetUpdateFlags(UpdateFlags.Grade);
				}
			}
			public void UpdateDragFactor() {
				SetUpdateFlags(UpdateFlags.Grade);
			}


			protected float m_Watts_Load;
			public float Watts_Load {
				get { return m_Watts_Load; }
				set {
					if (!m_bERG || m_Watts_Load != value) {
						m_Watts_Load = value;
						m_bERG = true;
						SetUpdateFlags(UpdateFlags.Watts);
					}
				}
			}
			public void SetWattsLoad(float load, bool force) {
				if (force || m_Watts_Load != load || !m_bERG) {
					m_Watts_Load = load;
					m_bERG = true;
					SetUpdateFlags(UpdateFlags.Watts);
				}
			}

			protected StatusFlags m_StatusFlags = StatusFlags.Zero;
			public StatusFlags StatusFlags { get { return m_StatusFlags; } }


			protected int m_Drag;
			public int Drag {
				get { return m_Drag; }
				set {
					int v = value < 0 ? 0 : value > 120 ? 120 : value;
					if (m_Drag != value) {
						m_Drag = v;
						SetUpdateFlags(UpdateFlags.Drag);
					}
				}
			}


			protected float m_Wind; // Stored in KPH for trainer.
			public float Wind {
				get { return (float)(m_Wind * ConvertConst.KPHToMetersPerSecond); }
				set {
					float v = (float)(value * ConvertConst.MetersPerSecondToKPH);
					if (m_Wind != v) {
						m_Wind = v;
						SetUpdateFlags(UpdateFlags.Wind);
					}
				}
			}
			public void SetWind(float wind, bool bforce) {
				wind = (float)(wind * ConvertConst.MetersPerSecondToKPH);
				if (bforce || m_Wind != wind) {
					m_Wind = wind;
					SetUpdateFlags(UpdateFlags.Wind);
				}
			}


			protected bool m_Drafting;
			public bool Drafting {
				get { return m_Drafting; }
				set {
					if (m_Drafting != value) {
						m_Drafting = value;
						SetUpdateFlags(UpdateFlags.Wind | UpdateFlags.Drafting);
					}
				}
			}

			public void Reset_trnr_Averages() {
				SetUpdateFlags(UpdateFlags.ResetAverages);
			}




			/// <summary>
			///  The index of the current front gear (chainring).
			/// </summary>
			private int m_FrontGearNumber;

			/// <summary>
			/// Gets / Sets the index of the current front gear (chainring).
			/// </summary>
			public int FrontGearNumber {
				get {
					return m_FrontGearNumber;
				}
				set {
					int tmp = value < 0 ? 0 : value >= m_VelotronData.ChainringsCount ? m_VelotronData.ChainringsCount - 1 : value;
					if (tmp != m_FrontGearNumber)
					{
						m_FrontGearNumber = tmp;
						SetVelotronGears();
					}
				}
			}

			/// <summary>
			/// The index of the current rear gear (cog).
			/// </summary>
			private int m_RearGearNumber;

			/// <summary>
			/// Gets / Sets the index of the current rear gear (cog).
			/// </summary>
			public int RearGearNumber {
				get {
					return m_RearGearNumber;
				}
				set {
					int tmp = value < 0 ? 0 : value >= m_VelotronData.CogsetCount ? m_VelotronData.CogsetCount - 1 : value;
					if (tmp != m_RearGearNumber)
					{
						m_RearGearNumber = tmp;
						SetVelotronGears();
					}
				}
			}


			/// <summary>
			/// Initializes all information related to a trainer.
			/// </summary>
			Trainer(string portName) {
				InitKeys();
				Closed = false;
				LastError = DLLError.ALL_OK;
				Type = DeviceType.NOT_SCANNED;
				Ver = 0;
				VersionNum = 0;
				IsCalibrated = false;
				CalibrationValue = 0;
				IsConnected = false;
				m_Buttons = 0;

				ms_Trainers[portName] = this;
				PortName = portName;
				bFake = false;
				//if (portName == FakePort)
				//{
				//    bFake = true;
				//    PortNumber = 99;
				//    VersionNum = 4543;
				//    Ver = 4095;
				//    //I want to kill the com 100 fake case in release
				//   // IsConnected = true;
				//   // Type = DeviceType.COMPUTRAINER;
				//    Type = DeviceType.NOT_SCANNED;

				//    IsConnected = false;
				//}
				ms_Mux.WaitOne();
				try {
					ms_InitList.AddLast(this);
				}
				catch (Exception ex) { MutexException(ex); }
				ms_Mux.ReleaseMutex();
			}
			~Trainer() {
				Stop();
			}

			public void Close() {
				if (Closed)
					return;
				Stop();
				Closed = true;
				if (this.OnClosed != null)
					this.OnClosed(this, null);
				if (RM1.OnClosed != null)
					RM1.OnClosed(this, null);
			}

			public static Trainer Get(string portName) {
				Trainer t;
				if (ms_Trainers.ContainsKey(portName))
					t = ms_Trainers[portName];
				else {
					t = new Trainer(portName);
				}
				return t;
			}

			public static Trainer Find(string portName) {
				return (ms_Trainers.ContainsKey(portName) ? ms_Trainers[portName] : null);
			}



			bool m_CalibrateMode = false;
			bool m_CurCalibrationMode = false;

			public bool CalibrateMode {
				get { return m_CalibrateMode; }
				set {
					//Debug.WriteLine("RM1.cs:::calibrate mode is being changed to : " + value.ToString());
					// Debug.WriteLine("Stack trace = '{0}'", Environment.StackTrace);
					bool v = IsStarted ? value : false;
					if (v == m_CalibrateMode)
						return;
					ms_Mux.WaitOne();
					try {
						m_Changed |= TrainerChangedFlags.Calibrate;
						m_CalibrateMode = v;
					}
					catch (Exception ex) {
						MutexException(ex);
					}
					ms_Mux.ReleaseMutex();
				}
			}




			protected bool Start() {
#if DEBUG_LOG_ENABLED
				Log.WriteLine("RM1.cs, Start()");
#endif

				if (Type != DeviceType.VELOTRON && Type != DeviceType.COMPUTRAINER) {
					return false;
				}

				if (!IsStarted) {
					ms_Mux.WaitOne();
					try {
						if (Type == DeviceType.VELOTRON) {
							DLLError derr = (DLLError)DLL.SetVelotronParameters(PortName, Ver,
									m_VelotronData.ChainringsCount,
									m_VelotronData.CogsetCount,
									ms_ip_chainrings,
									ms_ip_cogset,
									m_VelotronData.WheelDiameter_mm,
									m_VelotronData.ActualChainring,
									m_VelotronData.ActualCog,
									m_VelotronData.Bike_Kg,
									m_VelotronData.FrontGear,
									m_VelotronData.RearGear
									);
							m_CurVelotron = m_VelotronData;
						}
						DLLError ans;
						if (bFake)
							ans = DLLError.ALL_OK;
						else {
							try {

								ans = (DLLError)RM1.DLL.startTrainer(PortName, IntPtr.Zero, 0);
								DLL.start_trainer(PortName, true);
							}
							catch (Exception exc) {
								Debug.WriteLine("ERROR IN STARTING Trainer: " + exc.ToString());
								ans = DLLError.GENERIC_ERROR;
							}
						}
						if (ans == DLLError.ALL_OK)  {
							Log.WriteLine(TypeString + " " + Version + " - Started");
							IsStarted = true;
							if (!ms_StartedList.Contains(this))
								ms_StartedList.AddLast(this);
							m_Buttons = DLL.GetHandleBarButtons(PortName, Ver) & 0x3f;
							if (m_Buttons != 0)
								m_bButtonsStart = true;
						}
						else
							LastError = ans;
						Reset_trnr_Averages();
					}
					catch (Exception ex) { MutexException(ex); }
					ms_Mux.ReleaseMutex();
				}
				SetVelotronGears();
				SetUpdateFlags(UpdateFlags.All);

				return IsStarted;
			}														// Start()

			/// <summary>
			/// Stops the trainer.
			/// </summary>

			protected void Stop() {
#if DEBUG_LOG_ENABLED
				Debug.WriteLine("RM1.cs   Stop()");
#endif
				if (IsStarted) {
					ms_Mux.WaitOne();
					try {
						CalibrateMode = false;
						Log.WriteLine(TypeString + " " + Version + " - Stopped");
						IsStarted = false;
						if (!bFake)
						{
							RM1.DLL.stopTrainer(PortName);
						}
						ms_StartedList.Remove(this);
					}
					catch (Exception ex) {
						MutexException(ex);
					}
					ms_Mux.ReleaseMutex();
				}
			}										// Stop()

#pragma warning disable 649
			static int temp_int = 0;
			static IntPtr ms_ip_chainrings = Marshal.AllocHGlobal(Marshal.SizeOf(temp_int) * 32);
			static IntPtr ms_ip_cogset = Marshal.AllocHGlobal(Marshal.SizeOf(temp_int) * 32);

			protected VelotronData m_CurVelotron;

			/******************************************************************************************************************

			 ******************************************************************************************************************/

			/// <summary>
			/// Copies Chainring and Cog arrays and adds the trainers to the start list (list of trainers that need to be started).
			/// </summary>
			/// <returns></returns>
			public DLLError SetVelotron_trnr_Parameters() {
#if DEBUG_LOG_ENABLED
				Log.WriteLine("SetVelotron_trnr_Parameters()");
#endif
				if (Type != DeviceType.VELOTRON) {
					return DLLError.WRONG_DEVICE;
				}

				ms_Mux.WaitOne();

				try {
					Marshal.Copy(
							m_VelotronData.Chainrings,			// source
							0,									// start index
							ms_ip_chainrings,					// destination
							MAX_FRONT_GEARS						// length
							);


					Marshal.Copy(
							m_VelotronData.Cogset,
							0,
							ms_ip_cogset,
							MAX_REAR_GEARS
							);

					if (!ms_StartList.Contains(this)) {
						ms_StartList.AddLast(this);
					}
				}
				catch (Exception ex) {
					MutexException(ex);
				}

				ms_Mux.ReleaseMutex();

				return DLLError.ALL_OK;
			}												// SetVelotron_trnr_Parameters()

			//=============================================================
			protected String m_CBLine = null;
			public String CBLine {
				get {
					if (!IsConnected) {
						if (ShouldBe == DeviceType.COMPUTRAINER || ShouldBe == DeviceType.VELOTRON) {
							return PortName + ": / " + DeviceNames[(int)Type] + " - Not detected";
						}
						return PortName + ": / Unknown";
					}

					m_CBLine = PortName + ": / v" + Version + " / " + DeviceNames[(int)Type] + " / " + (Type == DeviceType.COMPUTRAINER ? "RRC = " : "Accuwatt = ") + CalibrationString;
					return m_CBLine;
				}
			}									// String CBLine

			//=============================================================

			protected int m_IntervalCount = 0;

			protected TD m_TrainerData;
			protected SSD m_SpinScanData;
			protected float[] m_Bars = new float[24];
			protected float[] m_AverageBars = new float[24];
			protected GP m_GearPair;

			protected float m_Calories;
			protected float m_PulsePower;
			protected float m_NP;
			protected float m_IF;
			protected float m_TSS;

			//==========================================================
			// IStats interface
			public bool Metric { get { return true; } }

			public Int64 Ticks { get { return ms_LastTicks; } }
			public float SplitTime { get { return ms_SplitTime; } }

			public float Speed {
				get {
					return (float)(m_TrainerData.Speed * ConvertConst.KPHToMetersPerSecond);
				}
			}

			public float Cadence {
				get {
					return m_TrainerData.Cadence;
				}
			}

			public float HeartRate {
				get {
					return (float)Math.Round(m_TrainerData.HR);
				}
			}

			public float Watts { get { return m_TrainerData.Power; } }

			public float SS { get { return m_SpinScanData.ss; } }
			public float SSLeft { get { return m_SpinScanData.lss; } }
			public float SSRight { get { return m_SpinScanData.rss; } }
			public float SSLeftSplit { get { return m_SpinScanData.lsplit; } }
			public float SSRightSplit { get { return m_SpinScanData.rsplit; } }

			public float Calories { get { return m_Calories; } }
			public float PulsePower { get { return m_PulsePower; } }
			public float NP { get { return m_NP; } }
			public float IF { get { return m_IF; } }
			public float TSS { get { return m_TSS; } }

			public float[] Bars { get { return m_Bars; } }
			public float[] AverageBars { get { return m_AverageBars; } }

			public int FrontGear {
				get {
					return m_GearPair.Front;
				}
			}	// Velotron only -1 if not valid
			public int RearGear {
				get {
					return m_GearPair.Rear;
				}
			}	// Velotron only -1 if not valid

			public int GearInches {
				get {
					return Type == DeviceType.VELOTRON && m_GearPair.Rear > 0 ? (int)m_VelotronData.WheelDiameter_inches * m_GearPair.Front / m_GearPair.Rear : 0;
				}
			}


			public event IStatsEvent OnUpdate;
			//=============================================================================
			class Key {
				const Int64 c_LongTicks = (Int64)(1.0 * ConvertConst.SecondToHundredNanosecond);
				const Int64 c_RepeatTicks = (Int64)(0.5 * ConvertConst.SecondToHundredNanosecond);
				const Int64 c_HRepeatTicks = (Int64)(0.1 * ConvertConst.SecondToHundredNanosecond);
				const int c_HCount = 3;
				delegate void xkey(PadKeys key, double pressed);

				//===============
				LinkedListNode<Key> m_Node;
				PadKeys m_Key;
				PadKeys m_ShortKey;
				PadKeys m_LongKey;
				PadKeys m_RepeatKey;

				bool m_Pressed = false;
				int m_Count = 0;
				Int64 m_DownTime;
				Int64 m_Next;
				Trainer m_Trainer;

				public Key(Trainer trainer, PadKeys key, PadKeys shortkey, PadKeys longkey, PadKeys repeat) {
					m_Trainer = trainer;
					m_Key = key;
					m_ShortKey = shortkey;
					m_LongKey = longkey;
					m_RepeatKey = repeat;
					if (longkey != RM1.PadKeys.NOKEY || repeat != RM1.PadKeys.NOKEY)
						m_Node = new LinkedListNode<Key>(this);
				}
				public void Down() {
					if (!m_Pressed) {
						m_DownTime = ms_LastTicks;
						m_Next = m_DownTime + c_LongTicks;
						if (m_Node != null)
							m_Trainer.m_KeysDown.AddLast(m_Node);
						AppWin.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new xkey(OnKey), m_Key, 0);
						m_Pressed = true;
						m_Count = 0;
					}
				}
				public void Up() {
					if (m_Pressed) {
						m_Pressed = false;
						if (m_Node != null)
							m_Trainer.m_KeysDown.Remove(m_Node);
						if (m_Count > 0)
							m_Count = 0;
						else if (m_ShortKey != PadKeys.NOKEY)
							AppWin.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new xkey(OnKey), m_ShortKey, 0);
						AppWin.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new xkey(OnKey), m_Key, ConvertConst.HundredNanosecondToSecond * (ms_LastTicks - m_DownTime));
					}
				}
				public void Update() {
					if (ms_LastTicks >= m_Next) {
						m_Next = m_Next + (m_Count > c_HCount ? c_HRepeatTicks : c_RepeatTicks);
						if (ms_LastTicks >= m_Next)
							m_Next = ms_LastTicks + 1; // Make sure this doesn't keep going back and back
						PadKeys key = m_Count > 0 ? m_RepeatKey : m_LongKey;
						if (key != RM1.PadKeys.NOKEY)
							AppWin.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new xkey(OnKey), key, 0);
						m_Count++;
					}
				}
				public void OnKey(PadKeys key, double pressed) {
					Log.WriteLine(String.Format("{0} {1}", key.ToString(), pressed == 0 ? "Down" : "Up " + String.Format("{0:F2} seconds", pressed)));

					if (m_Trainer.OnPadKey != null)
						m_Trainer.OnPadKey(m_Trainer, key, pressed);
					if (RM1.OnPadKey != null)
						RM1.OnPadKey(m_Trainer, key, pressed);
				}
				public bool IsDown { get { return m_Pressed; } }
			}
			LinkedList<Key> m_KeysDown = new LinkedList<Key>();
			void UpdateKeysDown() {
				foreach (Key k in m_KeysDown)
					k.Update();
			}

			Key[] m_Keys = new Key[(int)RM1.PadKeys.MAX];
			void InitKeys() {
				m_Keys[(int)RM1.PadKeys.F1] = new Key(this, RM1.PadKeys.F1, RM1.PadKeys.F1_Short, RM1.PadKeys.F1_Long, RM1.PadKeys.F1_Repeat);
				m_Keys[(int)RM1.PadKeys.F2] = new Key(this, RM1.PadKeys.F2, RM1.PadKeys.F2_Short, RM1.PadKeys.F2_Long, RM1.PadKeys.F2_Repeat);
				m_Keys[(int)RM1.PadKeys.F3] = new Key(this, RM1.PadKeys.F3, RM1.PadKeys.F3_Short, RM1.PadKeys.F3_Long, RM1.PadKeys.F3_Repeat);
				m_Keys[(int)RM1.PadKeys.F4] = new Key(this, RM1.PadKeys.F4, RM1.PadKeys.F4_Short, RM1.PadKeys.F4_Long, RM1.PadKeys.F4_Repeat);
				m_Keys[(int)RM1.PadKeys.F5] = new Key(this, RM1.PadKeys.F5, RM1.PadKeys.F5_Short, RM1.PadKeys.F5_Long, RM1.PadKeys.F5_Repeat);
				m_Keys[(int)RM1.PadKeys.F6] = new Key(this, RM1.PadKeys.F6, RM1.PadKeys.F6_Short, RM1.PadKeys.F6_Long, RM1.PadKeys.F6_Repeat);
				m_Keys[(int)RM1.PadKeys.UP] = new Key(this, RM1.PadKeys.UP, RM1.PadKeys.UP_Short, RM1.PadKeys.UP_Long, RM1.PadKeys.UP_Repeat);
				m_Keys[(int)RM1.PadKeys.DOWN] = new Key(this, RM1.PadKeys.DOWN, RM1.PadKeys.DOWN_Short, RM1.PadKeys.DOWN_Long, RM1.PadKeys.DOWN_Repeat);
				m_Keys[(int)RM1.PadKeys.FN_UP] = new Key(this, RM1.PadKeys.FN_UP, RM1.PadKeys.FN_UP_Short, RM1.PadKeys.FN_UP_Long, RM1.PadKeys.FN_UP_Repeat);
				m_Keys[(int)RM1.PadKeys.FN_DOWN] = new Key(this, RM1.PadKeys.FN_DOWN, RM1.PadKeys.FN_DOWN_Short, RM1.PadKeys.FN_DOWN_Long, RM1.PadKeys.FN_DOWN_Repeat);
				m_Keys[(int)RM1.PadKeys.NO_COMMUNICATION] = new Key(this, RM1.PadKeys.NO_COMMUNICATION, RM1.PadKeys.NOKEY, RM1.PadKeys.NOKEY, RM1.PadKeys.NOKEY);
			}

			public bool NoCommunication {
				get { return m_Keys[(int)RM1.PadKeys.NO_COMMUNICATION].IsDown; }
			}
			//=============================================================================



			public TD TrainerData {
				get {
					return m_TrainerData;
				}
			}

			public SSD SpinScanData { get { return m_SpinScanData; } }

			protected bool m_bButtonsStart;
			protected int m_Buttons;
			public int RawButtons { get { return m_Buttons; } }
			public int RawChanged { get; protected set; }
			public int RawDown { get; protected set; }
			public int RawUp { get; protected set; }


			public event TrainerPadKey OnPadKey;
			public event TrainerEvent OnPadChanged;
			public event TrainerEvent OnClosed;	// Single trainer closed.


			private delegate void _event(TrainerEvent trainerevent, RM1.Trainer trainer, object obj);
			private static void s_event(TrainerEvent trainerevent, RM1.Trainer trainer, object obj) {
				trainerevent(trainer, obj);
			}

			private Int64 m_FN_DOWN_Ticks;
			private Int64 m_FN_UP_Ticks;
			private bool m_UpGear;
			private bool m_DownGear;
			private int m_KeyZeroCount;

			protected bool m_CurPaused = false;

			bool m_bSetErg = false;
			int m_CurDragFactor = 0;
			//float m_CurWind = 0;

			/*******************************************************************************************************

			 *******************************************************************************************************/

			protected void Update() {
				if (!IsStarted) {
					return;
				}

				m_IntervalCount++;
				bool paused;
				UpdateFlags uflags = ClearUpdateFlags(out paused);

				// Do all the nessary update flags.
				//==================================

				if (uflags != UpdateFlags.Zero) {
					if ((uflags & UpdateFlags.Pause) != UpdateFlags.Zero && !paused) {
						DLL.setPause(PortName, paused);
						m_CurPaused = paused;
					}

					if ((uflags & UpdateFlags.ResetAverages) != UpdateFlags.Zero) {
						DLL.ResetAverages(PortName, Ver);
						DLL.associate(PortName, (ushort)m_Rider.HrSensorId);
						DLL.start_trainer(PortName, true);
						uflags |= UpdateFlags.Grade | UpdateFlags.Watts | UpdateFlags.Drag | UpdateFlags.Pause | UpdateFlags.FTP | UpdateFlags.Pause;
					}

					if ((uflags & UpdateFlags.FTP) != UpdateFlags.Zero) {
						DLL.set_ftp(PortName, Ver, m_FTP);
					}
					if ((uflags & UpdateFlags.Pause) != UpdateFlags.Zero && paused) {
						int status = DLL.setPause(PortName, paused);
						m_CurPaused = paused;
					}

					if ((uflags & UpdateFlags.Drafting) != UpdateFlags.Zero) {
						DLL.set_draftwind(PortName, Ver, m_Drafting ? RM1.DraftWind : 0.0f);
					}

					if ((uflags & UpdateFlags.Wind) != UpdateFlags.Zero) {
						/*
							if (m_CurWind != m_Wind)
							{
							m_CurWind = m_Wind;
							Log.WriteLine(String.Format("Wind changed to {0}", m_Wind));
							}
						 */
						DLL.set_wind(PortName, Ver, m_Wind);// + (m_Drafting ? RM1.DraftWind : 0.0f));
					}


					// update grade, watts, drag if they have changed

					if ((uflags & (UpdateFlags.Grade | UpdateFlags.Watts | UpdateFlags.Drag)) != UpdateFlags.Zero) {
						if (m_bERG) {
							m_bSetErg = true;
							DLL.SetErgModeLoad(PortName, Ver, CalibrationValue, AppWin.PreviewMode ? 0 : m_Watts_Load);
						}
						else {											// windload mode update
							if (m_Rider == null) {
								if (m_bSetErg) {
									if (m_Grade == 0) {
										DLL.SetSlope(PortName, Ver, CalibrationValue, 0.0f, 0.0f, 100, 1);
									}
									m_bSetErg = false;
								}
								DLL.SetSlope(PortName, Ver, CalibrationValue, 0.0f, 0.0f, 100, AppWin.PreviewMode ? 0 : m_Grade);
							}
							else {
								float bw = m_Rider.WeightBikeKGS;
								float rw = m_Rider.WeightRiderKGS;
								if (m_Rider.DragFactor != m_CurDragFactor) {
									m_CurDragFactor = m_Rider.DragFactor;
									Log.WriteLine(String.Format("Drag factor changed to {0}", m_CurDragFactor));
								}
								if (m_bSetErg) {
									if (m_Grade == 0) {
										DLL.SetSlope(PortName, Ver, CalibrationValue, 0.0f, 0.0f, 100, 1);
									}
									m_bSetErg = false;
								}
								DLL.SetSlope(PortName, Ver, CalibrationValue, bw, rw, m_CurDragFactor, AppWin.PreviewMode ? 0 : m_Grade);
							}
						}
					}								// if ((uflags & (UpdateFlags.Grade | UpdateFlags.Watts | UpdateFlags.Drag)) != UpdateFlags.Zero)  {
				}										// if (uflags != UpdateFlags.Zero)  {


				int raw = 0;


#if DEBUG
				try {
#endif
					//xxx
					raw = (bFake ? FakeKeys : DLL.GetHandleBarButtons(PortName, VersionNum));
#if DEBUG
				}
				catch (Exception ex) {
					MutexException(ex);
				}
#endif

				if (AppWin.PreviewMode)
					raw &= 0x7fffff80;
				if (raw != 0)
					m_KeyZeroCount = 0;
				else {
					m_KeyZeroCount++;
					if (m_KeyZeroCount <= 1) {
						UpdateKeysDown();
						return;		// Just don't do the first zero we get.
					}
				}
				int b = raw & 0x7f;
				//modified here by Paul Smeulders Feb 5 2012 Bugrocket Ticket 1, remove the bit swap on CompuTrainer key bits.
				if (Type == DeviceType.COMPUTRAINER) {
					//    // Swap the first two bits.
					//    b = (b & ~3) | ((b & 1) << 1) | ((b & 2) >> 1);
					//end modify by Paul Smeulders
				}

				if (m_Buttons != b) {
					// Raise an event for the keypad
					int changed = b ^ m_Buttons;
					int down = changed & b;
					int up = changed & m_Buttons;
					RawChanged = changed;
					RawDown = down;
					RawUp = up;

					bool fn = (b & 1) != 0;
					RM1.PadKeys key;

					if (down != 0) {
						if ((down & 1) != 0) {										// 0x01, ct reset
							// apparently not implemented
						}

						if ((down & 2) != 0) {										// 0x02, ct f1
							key = fn ? RM1.PadKeys.F4 : RM1.PadKeys.F1;
							m_Keys[(int)key].Down();
						}

						if ((down & 4) != 0)  {										// 0x04, ct f2
							key = fn ? RM1.PadKeys.F5 : RM1.PadKeys.F2;
							m_Keys[(int)key].Down();
						}

						if ((down & 8) != 0) {										// 0x08, ct f3
							key = fn ? RM1.PadKeys.F6 : RM1.PadKeys.F3;
							m_Keys[(int)key].Down();
						}

						if ((down & 16) != 0) {										// 0x10, ct +
							key = fn ? RM1.PadKeys.FN_UP : RM1.PadKeys.UP;
							m_Keys[(int)key].Down();
						}

						if ((down & 32) != 0) {										// 0x20, ct -
							key = fn ? RM1.PadKeys.FN_DOWN : RM1.PadKeys.DOWN;
							m_Keys[(int)key].Down();
						}

						if ((down & 64) != 0)  {									// 0x40
							Debug.WriteLine("no communication");

							key = RM1.PadKeys.NO_COMMUNICATION;
							m_Keys[(int)key].Down();
							Unit unit = Unit.GetUnit(this);
							if (unit != null) {
								unit.Statistics.Changed |= StatFlags.Disconnected;
								unit.Statistics.PerfChanged |= StatFlags.Disconnected;
								Statistics.AllChanged |= StatFlags.Disconnected;
							}
							////see if I can save thread
							//string[] Portnames = System.IO.Ports.SerialPort.GetPortNames();
							//if (!Portnames.Contains("COM" + (PortNumber + 1)))
							//{
							//    DLL.stopTrainer(PortNumber);
							//    DLL.ResettoIdle(PortNumber);
							//    Debug.WriteLine("trying to restore");
							//}
						}
					}

					if (up != 0) {
						if ((up & 2) != 0) {
							m_Keys[(int)RM1.PadKeys.F1].Up();
							m_Keys[(int)RM1.PadKeys.F4].Up();
						}
						if ((up & 4) != 0) {
							m_Keys[(int)RM1.PadKeys.F2].Up();
							m_Keys[(int)RM1.PadKeys.F5].Up();
						}
						if ((up & 8) != 0) {
							m_Keys[(int)RM1.PadKeys.F3].Up();
							m_Keys[(int)RM1.PadKeys.F6].Up();
						}
						if ((up & 16) != 0) {
							m_Keys[(int)RM1.PadKeys.UP].Up();
							m_Keys[(int)RM1.PadKeys.FN_UP].Up();
						}
						if ((up & 32) != 0) {
							m_Keys[(int)RM1.PadKeys.DOWN].Up();
							m_Keys[(int)RM1.PadKeys.FN_DOWN].Up();
						}
						if ((up & 64) != 0) {
							m_Keys[(int)RM1.PadKeys.NO_COMMUNICATION].Up();
							SetCalibrationValue(false);
							Unit unit = Unit.GetUnit(this);
							if (unit != null) {
								unit.Statistics.Changed |= StatFlags.Disconnected;
								unit.Statistics.PerfChanged |= StatFlags.Disconnected;
								Statistics.AllChanged |= StatFlags.Disconnected;
							}
						}
					}

					if (OnPadChanged != null)
						Dispatcher.BeginInvoke(DispatcherPriority.Normal, new _event(s_event), OnPadChanged, this, b);

					m_Buttons = b;
				}

				if (m_DownGear && ms_LastTicks > m_FN_DOWN_Ticks) {
					m_DownGear = false;
					//FrontGearNumber--;
				}
				if (m_UpGear && ms_LastTicks > m_FN_UP_Ticks) {
					m_UpGear = false;
					//FrontGearNumber++;
				}

				// Deal with the trainer data.

				if (!bFake) {
					try {
						if (m_Changed != TrainerChangedFlags.Zero) {
							if (m_CurCalibrationMode != m_CalibrateMode) {
								m_CurCalibrationMode = m_CalibrateMode;
								if (m_CurCalibrationMode) {

									//Debug.WriteLine("RM1.cs::Entering calibration mode...");
									//Debug.WriteLine("stack trace : '{0}'" , Environment.StackTrace ); 
									Thread.Sleep(100);
									DLL.SetRecalibrationMode(PortName, Ver);
									Thread.Sleep(100);
									//  Debug.WriteLine("RM1.cs::...Calibration mode entered.\n");
								}
								else {
									//  Debug.WriteLine("RM1.cs::Exiting calibration mode...");
									Thread.Sleep(100);
									DLL.EndRecalibrationMode(PortName, Ver);
									Thread.Sleep(100);
									SetCalibrationValue(true);
									// Debug.WriteLine("RM1.cs::...Calibration mode exited.\n");
								}
								if (ms_WaitAfter < 100)
									ms_WaitAfter = 100;
							}
							m_Changed = TrainerChangedFlags.Zero;
						}
						IntPtr ip;


						if (!m_CurCalibrationMode) {
							if (!AppWin.PreviewMode) {
								m_TrainerData = DLL.GetTrainerData(PortName, Ver);
								// speed is in kph = m_TrainerData.Speed

								// Sanity checks on the trainer data.
								if (m_TrainerData.Cadence >= c_WeirdCadence && m_TrainerData.Speed < c_WeirdCadenceSpeed) {
#if DEBUG
									m_TrainerData.Cadence = 77;
#else
									m_TrainerData.Cadence = 0;
#endif
								}


								m_SpinScanData = DLL.get_ss_data(PortName, Ver);
#if DEBUG
									try {
#endif
										//xxx
										ip = DLL.get_bars(PortName, Ver);
										if (ip != IntPtr.Zero) {
											Marshal.Copy(ip, m_Bars, 0, 24);
										}
#if DEBUG
									}
									catch (Exception ex)  {
										MutexException(ex);
										bp = 3;
									}
#endif

#if DEBUG
								try {
#endif
								ip = DLL.get_average_bars(PortName, Ver);
								if (ip != IntPtr.Zero) {
									Marshal.Copy(ip, m_AverageBars, 0, 24);
								}
								else {
								}
#if DEBUG
								}
								catch (Exception ex) {
									MutexException(ex);
								}
#endif

								m_PulsePower = DLL.get_pp(PortName, Ver);
								m_Calories = DLL.get_calories(PortName, Ver);
								m_IF = DLL.get_if(PortName, Ver);
								m_NP = DLL.get_np(PortName, Ver);
								m_TSS = DLL.get_tss(PortName, Ver);
								// TODO: revisit if we need the status bits
								//StatusFlags sf = (StatusFlags)DLL.get_status_bits(PortName, Ver);				// not implemented!

								//if (m_StatusFlags != sf) {
								//	m_StatusFlags = sf;
								//	Log.WriteLine(String.Format("Trainer Port {0}, Status Bit changed (0x{1:X4},{2})", PortName, (Int32)sf, sf.ToString(), sf.ToString()));
								//}

								if (Type == DeviceType.VELOTRON) {
									m_GearPair = DLL.GetCurrentVTGear(PortName, Ver);
								}
							}												//	if (!m_CurCalibrationMode)   {
							else {										// if (AppWin.PreviewMode)
								// calibrating:
								m_TrainerData = DLL.GetTrainerData(PortName, Ver); // Keep this live, even through we are going to write fake data in there.
								m_TrainerData.Cadence = 75.91368f;
								m_TrainerData.HR = 0.0f;
								m_TrainerData.Power = 640.7542f;
								m_TrainerData.Speed = 23.8940315f;
								m_SpinScanData.lsplit = 45.55287f;
								m_SpinScanData.lss = 99.84009f;
								m_SpinScanData.rsplit = 54.44713f;
								m_SpinScanData.rss = 79.56448f;
								m_SpinScanData.ss = 89.7022858f;
								m_Bars[0] = 6.62390947f;
								m_Bars[1] = 6.62577248f;
								m_Bars[2] = 6.624319f;
								m_Bars[3] = 6.629265f;
								m_Bars[4] = 6.62212753f;
								m_Bars[5] = 6.61649561f;
								m_Bars[6] = 6.611492f;
								m_Bars[7] = 6.615699f;
								m_Bars[8] = 6.615697f;
								m_Bars[9] = 6.612983f;
								m_Bars[10] = 6.612182f;
								m_Bars[11] = 6.61402464f;
								m_Bars[12] = 6.613258f;
								m_Bars[13] = 6.61195135f;
								m_Bars[14] = 6.60954762f;
								m_Bars[15] = 6.61452f;
								m_Bars[16] = 6.613853f;
								m_Bars[17] = 6.61203575f;
								m_Bars[18] = 7.660412f;
								m_Bars[19] = 8.76417f;
								m_Bars[20] = 9.32529f;
								m_Bars[21] = 9.681499f;
								m_Bars[22] = 9.882229f;
								m_Bars[23] = 9.942836f;
								m_AverageBars[0] = 1.98130882f;
								m_AverageBars[1] = 2.0038867f;
								m_AverageBars[2] = 2.00340962f;
								m_AverageBars[3] = 1.99159563f;
								m_AverageBars[4] = 2.00532126f;
								m_AverageBars[5] = 2.05315042f;
								m_AverageBars[6] = 2.08238029f;
								m_AverageBars[7] = 2.08782029f;
								m_AverageBars[8] = 2.08151364f;
								m_AverageBars[9] = 2.06743836f;
								m_AverageBars[10] = 2.05630422f;
								m_AverageBars[11] = 2.06120944f;
								m_AverageBars[12] = 2.08145761f;
								m_AverageBars[13] = 2.09165478f;
								m_AverageBars[14] = 2.05820584f;
								m_AverageBars[15] = 2.03270936f;
								m_AverageBars[16] = 1.98586369f;
								m_AverageBars[17] = 1.94186223f;
								m_AverageBars[18] = 1.987597f;
								m_AverageBars[19] = 2.05060148f;
								m_AverageBars[20] = 2.08014679f;
								m_AverageBars[21] = 2.10211587f;
								m_AverageBars[22] = 2.13841f;
								m_AverageBars[23] = 2.10149455f;
								m_PulsePower = 0.0f;
								m_Calories = 7.825437f;
								m_StatusFlags = StatusFlags.Zero;
								m_GearPair.Front = 56;
								m_GearPair.Rear = 14;
							}
						}											// if (!m_CurCalibrationMode)   {
						else if (ms_WaitAfter < 100) {
							ms_WaitAfter = 100;
						}

						if (OnUpdate != null) {
							OnUpdate(this, ms_SplitTime);
						}
					}
					catch (Exception ex) {
						Debug.WriteLine(ex.ToString());
					}
				}															// if (!bFake)  {
				UpdateKeysDown();
			}																// Update()


			/**********************************************************************************************************

			 **********************************************************************************************************/

			void SetVelotronGears() {
				if (Type == DeviceType.VELOTRON && IsStarted)
					DLL.setGear(PortName, Ver, m_FrontGearNumber, m_RearGearNumber);
			}

			/**********************************************************************************************************

			 **********************************************************************************************************/

			void SetCalibrationValue(bool notify) {
				ms_Mux.WaitOne();
				try {
					int orgv = DLL.GetFirmWareVersion(PortName);
					int v = (orgv == 4095 ? 4543 : orgv);

					bool c = (DLL.GetIsCalibrated(PortName, orgv) & 0xff) == 0 ? false : true;
					int cnum = DLL.GetCalibration(PortName);                // wifi = 3, serial = 200

					//Paul enabled feb 5
					//Log.WriteLine (String.Format("Found {0} on port {1}, Version {2}", Type, PortName, v) +
					//	(c ? "" : " Not calibrated")+ " calnum is " + cnum);
					//end Paul enabled feb 5
					Ver = orgv;
					VersionNum = v;
					if (!notify)
						notify = (c != IsCalibrated || CalibrationValue != cnum);
					notify = true;
					IsCalibrated = c;
					CalibrationValue = cnum;
				}
				catch (Exception ex) { MutexException(ex); }
				ms_Mux.ReleaseMutex();
				if (notify) {
					Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate() {
								if (RM1.OnCalibrationChanged != null)
									RM1.OnCalibrationChanged(this, null);
								Unit unit = Unit.TrainerUnit(this);
								if (unit != null) {
									unit.Statistics.SetCalibrationValue(IsCalibrated, CalibrationValue, true);
								}
							});
				}
			}
			/// <summary>
			/// RUN FROM WORKER THREAD
			/// </summary>
			/**********************************************************************************************************

			 **********************************************************************************************************/

			private void initDevice() {
				//int status;

				if (IsStarted) {
					Log.WriteLine(String.Format("Stopping Trainer {0}", PortName));
					Stop(); // Stop the device if started.
				}
				Log.WriteLine(String.Format("Getting Trainer {0}", PortName));

				DeviceType t;
				if (bFake) {
					t = Type;
				}
				else {
					t = GetRacerMateDeviceID(PortName);
#if DEBUG_LOG_ENABLED
					Log.WriteLine("RM1.cs, back from GetRacerMateDeviceID");
					Debug.WriteLine("RM1.cs, back from GetRacerMateDeviceID");			// outputdebugstring
#endif

					if (t == DeviceType.COMPUTRAINER || t == DeviceType.VELOTRON) {
#if DEBUG_LOG_ENABLED
						Log.WriteLine("RM1.cs, found computrainer");
						Debug.WriteLine("RM1.cs, found computrainer");
#endif
						SetCalibrationValue(true);
						IsConnected = true;
					}
					//Feb 5 2012 Paul Smeulders add
					else {
#if DEBUG_LOG_ENABLED
						Log.WriteLine("RM1.cs, no trainer found");
						Debug.WriteLine("RM1.cs, no trainer found");
#endif
						IsConnected = false;
						Log.WriteLine(String.Format("{0} on port {1}", RM1.DeviceNames[(int)t], PortName));
					}
					//Feb 5 2012 Paul Smeulders end of add
				}
				// This will mark things as ready.
				if (t == DeviceType.VELOTRON && m_VelotronData == null)
					m_VelotronData = new VelotronData();
				Type = t;

				// OK Now before we start lets see if we can fill in the initial rider.
				foreach (TrainerUserConfigurable tc in RM1_Settings.ActiveTrainerList) {
					if (PortName == tc.SavedPortName) {
						// We got it.
						Rider = Riders.FindRiderByKey(tc.PreviousRiderKey);
					}
				}
			}

			private static int orderByNum(Trainer a, Trainer b) {
				return a.PortName.CompareTo(b.PortName);
			}

			/// <summary>
			/// Should be on the main thread... add it to the proper location and raise the proper events.
			/// </summary>
			private void initDone() {
				ms_Mux.WaitOne(); // Make sure the sOnTimedEvent is not running
				try {
					ms_InitList.Remove(this);
				}
				catch (Exception ex) { MutexException(ex); }
				ms_Mux.ReleaseMutex();
			}

			private void notifyInit(int cnt)  {
				Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate() {
					if (RM1.OnTrainerInitialized != null) {
						Log.WriteLine(String.Format("Dispatch - {0} left", cnt));
						RM1.OnTrainerInitialized(this, cnt);
					}
				});
			}

			//===============================================================
			// Used to control how many updates (during normal operation
			// there will be.   Note if we are taking too much time then 
			// statistics will be kept
			//===============================================================
			private static double ms_TargetFPS = -1;
			private static Int64 ms_TargetTicks;
			public static double TargetFPS {
				get { return ms_TargetFPS; }
				set {
					double v = value < 0.1 ? 0.1 : value;
					if (ms_TargetFPS == v)
						return;
					ms_Mux.WaitOne();
					try {
						ms_TargetFPS = v;
						ms_TargetTicks = (Int64)(ConvertConst.SecondToHundredNanosecond * (1.0 / v));
					}
					catch (Exception ex) { MutexException(ex); }
					ms_Mux.ReleaseMutex();
				}
			}


			//==================================================================================
			// Main thread that continues while the entire system is going.
			//==================================================================================
			private static Thread ms_Thread;
			private static bool ms_bShutdown = false;
			private static readonly AutoResetEvent ms_WaitEvent = new AutoResetEvent(false);

			static int ms_WaitAfter;

			/**********************************************************************************************************
			  one ThreadLoop for each Trainer();
              Actually, there's only one threadloop and it handles all the trainers (it runs in ms_Thread declared above).
              It would probably be better if there WAS one threadloop per trainer, because things would be 
              more parallelized and with many trainers they could be updated faster.
              Also, this one thread is both updating existing trainers and initializing new ones.
			 **********************************************************************************************************/

			private static void ThreadLoop() {
//#if DEBUG
//				Log.WriteLine("RM1.cs, ThreadLoop() beginning");

//				Process proc = Process.GetCurrentProcess();
//				int pid = proc.Id;
//				Debug.WriteLine("RM1.cs   ThreadLoop pid = " + pid.ToString());

//				//long mem1 = GC.GetTotalMemory(true);
//				long startmem = proc.PrivateMemorySize64;
//				long mem;
//				mem = startmem;

//				//Debug.WriteLine("RM1.cs   mem1 = " + mem1.ToString());
//				Debug.WriteLine("RM1.cs   mem = " + mem.ToString());
//#endif

//#if DEBUG
//					 //long cnt = -1;
//					 //long cnt2 = 0;
//					 long lastticks2 = DateTime.Now.Ticks;
//#endif

				/*
						long cnt2 = 0;
						long tot = 0L;
#endif
            */

				App.SetDefaultCulture();

				Trainer trainer;
				Int64 wait = 1, nextframe = 0, ticks, afterticks;

				//------------------------------------------
				// mainloop
				//------------------------------------------

				for (; !ms_WaitEvent.WaitOne(new TimeSpan(wait)) && !ms_bShutdown; ) {
					if (nextframe <= 0) {
						nextframe = DateTime.Now.Ticks + ms_TargetTicks;
					}

					ms_Mux.WaitOne();

					try {
						ticks = DateTime.Now.Ticks; // OK... Mark the time so we are as close as we can be when we enter into this thing
						ms_SplitTime = (float)((double)(ticks - ms_LastTicks) * ConvertConst.HundredNanosecondToSecond);
						ms_LastTicks = ticks;

						ms_IntervalCount++;

						// Any trainers need to be checked?
						if (ms_InitList.Count > 0) {
							if (DateTime.Now >= ms_WaitScan) {
								trainer = ms_InitList.First();
								ms_InitList.RemoveFirst();
								while (ms_StartedList.Count > 0) {
									Trainer t = ms_StartedList.First();
									t.Stop();
									if (!ms_StartList.Contains(t))
										ms_StartList.AddLast(t);
								}
								ms_StartedList.Clear();
								trainer.initDevice();

								if (trainer.Type == DeviceType.COMPUTRAINER || trainer.Type == DeviceType.VELOTRON) {
									if (!ms_ActiveList.Contains(trainer)) {
										ms_ActiveList.Add(trainer);
										ms_ActiveList.Sort(orderByNum);
										ms_ActiveListVersion++;
									}
									if (!ms_StartList.Contains(trainer))
										ms_StartList.AddLast(trainer);
								}
								trainer.notifyInit(ms_InitList.Count);
								if (ms_InitList.Count == 0)
								{
									// We just initialized the last trainer
									if (RM1.OnTrainerInitializationComplete != null)
									{
										RM1.OnTrainerInitializationComplete();
									}
								}
							}
							wait = (Int64)(ConvertConst.HundredNanosecondToMilliSecond * 1);
						}
						else if (ms_StartList.Count > 0) {
							foreach (Trainer t in ms_StartList) {
								t.Stop();	// Stop if already started.
								t.Start();
							}
							ms_StartList.Clear();
							wait = (Int64)(ConvertConst.HundredNanosecondToMilliSecond * 1);
						}
						else {
							// Update the trainers.
							ms_WaitAfter = 0;

							foreach (Trainer t in ms_ActiveList) {
								t.Update();								// ThreadLoop(), RM1::Update(), sends commands TO the trainer (like setting slope, etc)
								// also gets new trainer data, GetTrainerData(), get_bars(), etc
							}

							if (ms_WaitAfter > 0) {
								Thread.Sleep(100);
							}
							else {
								Thread.Sleep(0); // ECT - Just to make sure the UI stays responsive, probably not needed but it didn't seem to hurt.
							}

							Unit.IntervalUpdate(ms_LastTicks, ms_SplitTime);

							if (OnInterval != null) {
								OnInterval(ms_LastTicks, ms_SplitTime);					// in Bot.cs, Bot::OnInterval(), calls Bot::Update(), updates the pacer
							}

							ms_UpdateSplit += ms_SplitTime;

							if (RM1.OnUpdate != null && ms_updateop == null) {
								ms_updateop = AppWin.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, ms_updatedelegate);
							}

							// OK here is the endticks...

							afterticks = DateTime.Now.Ticks;
							trackTime(afterticks);
							wait = nextframe - afterticks;

							if (wait < 1) {
								wait = 1;
								nextframe = 0;
							}
							else {
								nextframe += ms_TargetTicks;

								/*
									#if DEBUG
								cnt = (cnt+1)%30;
								if (cnt == 0) {
										cnt2++;
									long xxx = DateTime.Now.Ticks;
									uint delta = (uint) ((xxx - lastticks2) & 0x0000ffff);
									lastticks2 = xxx;
									Debug.WriteLine("tick " + cnt2.ToString() + ", delta = " + delta.ToString());
								}
								#endif
								*/
							}
						}
					}
					catch (Exception ex)  {
						MutexException(ex);
						string s = ex.ToString();
					}
					ms_Mux.ReleaseMutex();
				}																// for (; !ms_WaitEvent.WaitOne(new TimeSpan(wait)) && !ms_bShutdown; )  {

#if DEBUG_LOG_ENABLED
				Log.WriteLine("RM1.cs, ThreadLoop() ending");
#endif

				ms_Mux.WaitOne();

				try {
					foreach (Trainer t in ms_ActiveList) {
						t.Stop();
					}
				}
				catch (Exception ex) {
					MutexException(ex);
				}
				ms_Mux.ReleaseMutex();
#if DEBUG_LOG_ENABLED
				Log.WriteLine("RM1.cs, ThreadLoop() exit");
#endif

			}														// ThreadLoop()

			/**********************************************************************************************************
			// Make sure everything is closed and shutdown correctly if RM1 is trying to Exit().
            **********************************************************************************************************/
			public static void Exit() {
				Log.WriteLine("rm1.cs, RM1::Exit()");
                ms_bShutdownScanningThread = true;
                ms_BackgroundScanningThread.Join();
                ms_bShutdown = true;
                ms_Thread.Join();
				DLL.stop_ant();
                DLL.racermate_close();
			}

			/******************************************************************************************************************

			******************************************************************************************************************/
			private const int ms_TrackTimeArrMax = 100;
			private static float[] ms_TrackTimeArr = new float[ms_TrackTimeArrMax];
			private static double ms_TrackTimeTotal = 0.0;
			private static int ms_TrackTimePos = 0;
			//private static int ms_Clip = 0;
			private static Int64 ms_TrackTicksLast = 0;

			private static void trackTime(Int64 ticks) {
				float tm = (float)((ticks - ms_TrackTicksLast) * ConvertConst.HundredNanosecondToSecond);
				ms_TrackTicksLast = ticks;
				ms_TrackTimeTotal -= ms_TrackTimeArr[ms_TrackTimePos];
				ms_TrackTimeArr[ms_TrackTimePos++] = tm;
				ms_TrackTimeTotal += tm;
				if (ms_TrackTimePos >= ms_TrackTimeArrMax)
					ms_TrackTimePos = 0;
			}


			/******************************************************************************************************************
			  <summary>
			  Start up the initial stuff
			  </summary>
			 ******************************************************************************************************************/

			public double FPS {
				get {
					double t = ms_TrackTimeTotal / ms_TrackTimeArrMax; return t > 0 ? 1.0 / t : 0.0;
				}
			}

			static Trainer() {
#if DEBUG_LOG_ENABLED
				Log.WriteLine("RM1.cs, Trainer(), starting ThreadLoop");
#endif
				TargetFPS = 30;
				ms_Mux.WaitOne();
				ms_Thread = new Thread(new ThreadStart(ThreadLoop));
				ms_Thread.Name = "Trainer";
				//ms_Thread.Priority = ThreadPriority.AboveNormal;
				ms_Thread.Start();
				ms_Mux.ReleaseMutex();

				/*
					ms_Interval = new System.Timers.Timer();
					ms_Interval.Elapsed += new ElapsedEventHandler(sOnTimedEvent);
					ms_Interval.Interval = 1000.0 / RM1.UpdateFPS;
				 */

				/*
					ms_DispatchTimer = new DispatcherTimer();
					ms_DispatchTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
					ms_DispatchTimer.Tick += new EventHandler(OnDispatchTimerTick);
				 */
			}


			// ===================================================================
			// Global timer thread.  So that we don't have 8 timer threads going.
			// ===================================================================
			//protected static Mutex ms_InitMux = new Mutex();
			protected static int ms_IntervalCount = 0;
			public static int IntervalCount { get { return ms_IntervalCount; } }

			////============
			//protected static int ms_StartCount = 0;
			///******************************************************************************************************************

			// ******************************************************************************************************************/

			public static void IncreaseStartCount()
			{
			}
			/******************************************************************************************************************

			 ******************************************************************************************************************/

			public static void DecreaseStartCount()
			{
			}


			protected static Int64 ms_LastTicks = DateTime.Now.Ticks;
			protected static float ms_SplitTime;
			public static Int64 LastTicks { get { return ms_LastTicks; } }

			protected static System.Timers.Timer ms_Interval;

			//protected static DispatcherTimer ms_DispatchTimer;

			/******************************************************************************************************************

			 ******************************************************************************************************************/

			static TimeSpan ms_LockStatsMaxTime = new TimeSpan(0, 0, 10);
			public static void LockStats() {
				ms_Mux.WaitOne();
				/*
					if (!ms_Mux.WaitOne( ms_LockStatsMaxTime ))
					{
					Debug.WriteLine("ERROR! Lockstats timed out ...something every wrong");
					DLL.WriteLog();
					}
				 */
			}

			public static void UnlockStats() {
				ms_Mux.ReleaseMutex();
			}

			/******************************************************************************************************************

			 ******************************************************************************************************************/

			private static double ms_UpdateSplit;
			private static DispatcherOperation ms_updateop;
			private delegate void _update();
			private static Delegate ms_updatedelegate = new _update(d_update);

			private static void d_update() {
				ms_Mux.WaitOne();
				try {
					ms_updateop = null;
					if (RM1.OnUpdate != null) {
						RM1.OnUpdate(ms_UpdateSplit);								// Unit.cs, Update()
					}
					ms_UpdateSplit = 0.0;
				}
				catch (Exception ex) { MutexException(ex); }
				ms_Mux.ReleaseMutex();
			}

			/*
				private static void OnDispatchTimerTick(object sender, EventArgs e)
				{
				IntPtr ip;
				foreach (Trainer trainer in ms_ActiveList)
				{
				}
				}
			 */


			// This doesn't appear to be needed...
			//protected static void background_Init(object s, DoWorkEventArgs args) {
			//	RM1.Trainer trainer = args.Argument as RM1.Trainer;
			//	try {
			//		trainer.initDevice();
			//	}
			//	catch {
			//		Log.WriteLine(String.Format("Error in initializing trainer {0}", trainer.PortName));
			//	}
			//	args.Result = trainer;
			//}

			// This doesn't appear to be needed...
			//protected static void background_Done(object s, RunWorkerCompletedEventArgs args) {
			//	RM1.Trainer trainer = args.Result as Trainer;
			//	trainer.initDone();
			//}

			//=================================================================================
			/// <summary>
			/// Part of the IPropertyChanged interface
			/// </summary>
			/// 
			public event PropertyChangedEventHandler PropertyChanged;

			public void OnPropertyChanged(string name) {
				if (PropertyChanged != null) {
					PropertyChanged(this, new PropertyChangedEventArgs(name));
				}
			}

			static DateTime ms_WaitScan = new DateTime(0);
			public static void WaitToScan(int seconds) {
				ms_Mux.WaitOne();
				try {
					ms_WaitScan = DateTime.Now + (new TimeSpan(0, 0, seconds));
				}
				catch (Exception ex) { MutexException(ex); }
				ms_Mux.ReleaseMutex();

			}
		}												// public class Trainer : DispatcherObject, INotifyPropertyChanged, IStats

		/******************************************************************************************************************

		******************************************************************************************************************/

		/// <summary>
		/// Velotron data.  Fill out and send to the trainer.
		/// </summary>
		[Serializable]
		public class VelotronData : ICloneable {
			private int[] m_Chainrings = new int[] { 28, 39, 56, 0, 0 };
			public int ChainringsCount { get; protected set; }

			public int[] Chainrings {
				get {
					return m_Chainrings;
				}

				set {
					int i, v;
					for (i = 0; i < MAX_FRONT_GEARS_SPACE && i < value.Length; i++) {
						v = value[i];
						if (v == 0) {
							break;
						}
						m_Chainrings[i] = v;
					}
					ChainringsCount = i;
					for (; i < MAX_FRONT_GEARS_SPACE; i++) {
						m_Chainrings[i] = 0;
					}
				}
			}											// public int[] Chainrings

			/// <summary>
			/// The number of teeth on each cog in the rear cassette (ie, cog set); 0 indicates cog does not exist.
			/// </summary>
			private int[] m_Cogset = new int[] { 23, 21, 19, 17, 16, 15, 14, 13, 12, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			public int CogsetCount { get; protected set; }

			public int[] Cogset {
				get { return m_Cogset; }
				set {
					int i, v;
					for (i = 0; i < MAX_REAR_GEARS_SPACE && i < value.Length; i++) {
						v = value[i];
						if (v == 0) {
							break;
						}
						m_Cogset[i] = v;
					}
					CogsetCount = i;

					for (; i < MAX_REAR_GEARS_SPACE; i++) {
						m_Cogset[i] = 0;
					}
				}
			}									// public int[] Cogset()

			private float m_WheelDiameter_mm = (float)(INCHES_TO_MM * 27.0);
			private float m_WheelDiameter_inches = 27.0f;

			public float WheelDiameter_mm {
				get { return m_WheelDiameter_mm; }
				set {
					m_WheelDiameter_mm = value;
					m_WheelDiameter_inches = value * MM_TO_INCHES;
				}
			}

			public float WheelDiameter_inches {
				get {
					return m_WheelDiameter_inches;
				}
				set {
					m_WheelDiameter_inches = value;
					m_WheelDiameter_mm = value * INCHES_TO_MM;
				}
			}

			public int ActualChainring = 62;
			public int ActualCog = 14;
			public float Bike_Kg = (float)(LBS_TO_KGS * 20.0);
			public int FrontGear = 2;
			public int RearGear = 11;

			public object Clone() {
				MemoryStream ms = new MemoryStream();
				BinaryFormatter bf = new BinaryFormatter();
				bf.Serialize(ms, this);
				ms.Position = 0;
				object obj = bf.Deserialize(ms);
				ms.Close();
				return obj;
			}										// public object Clone()

			public VelotronData() {
#if DEBUG_LOG_ENABLED
				Log.WriteLine("RM1.cs, VelotronData() constructor");
#endif
				ChainringsCount = 3;
				CogsetCount = 10;
			}										// constructor


			public static bool IsEqual(VelotronData a, VelotronData b) {
				// If both are null, or both are same instance, return true.
				if (object.ReferenceEquals(a, b)) {
					return true;
				}

				// If one is null, but not both, return false.
				if (((object)a == null) || ((object)b == null)) {
					return false;
				}

				if (a.ChainringsCount != b.ChainringsCount ||
						a.CogsetCount != b.CogsetCount ||
						a.WheelDiameter_inches != b.WheelDiameter_inches ||
						a.ActualChainring != b.ActualChainring ||
						a.ActualCog != b.ActualCog ||
						a.Bike_Kg != b.Bike_Kg ||
						a.FrontGear != b.FrontGear ||
						a.RearGear != b.RearGear) {
					return false;
				}

				int i;
				for (i = 0; i < a.CogsetCount; i++) {
					if (a.m_Cogset[i] != b.m_Cogset[i])
						return false;
				}

				for (i = 0; i < b.ChainringsCount; i++) {
					if (a.m_Chainrings[i] != b.m_Chainrings[i])
						return false;
				}
				return true;	// Everything matches.
			}								// public static bool IsEqual()
		};												// class VelotronData


	}									// public class RM1  {
}										// namespace RacerMateOne  {
