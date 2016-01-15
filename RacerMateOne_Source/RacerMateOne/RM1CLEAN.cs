using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using System.Diagnostics;       // Needed for process invocation
using Microsoft.Win32;
using System.ComponentModel; // CancelEventArgs
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Security.Permissions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;


/*
 * 
 * LATEST VERSION
 * 

	int Setlogfilepath(const char *pathtosafefolder);
	int Enablelogs(bool bikelog, bool courselog, bool decoder, bool ds, bool gears, bool physics);
	EnumDeviceType GetRacerMateDeviceID(int ix);
	int GetFirmWareVersion(int ix);
	bool GetIsCalibrated(int ix, int FirmwareVersion);
	int GetCalibration(int ix);
	int startTrainer(int ix, Course *_course=NULL);
	int start_trainer(int ix, bool _b);                                                                                // sets the 'started' flag so averages will update
	int stopTrainer(int ix);
	inline struct TrainerData GetTrainerData(int ix, int FirmwareVersion);
	int SetErgModeLoad(int ix, int FirmwareVersion, int RRC, float Load);
	int resetTrainer(int ix, int FirmwareVersion, int RRC);
	int SetSlope(int ix, int FirmwareVersion, int RRC, float bike_kgs, float person_kgs, int DragFactor, float slope);
	int SetHRBeepBounds(int ix, int FirmwareVersion, int LowBound, int HighBound, bool BeepEnabled);
	inline int GetHandleBarButtons(int ix, int FirmwareVersion);
	int SetRecalibrationMode(int ix, int FirmwareVersion);
	int EndRecalibrationMode(int ix, int FirmwareVersion);
	int setPause(int ix, bool _paused);
	const char *GetAPIVersion(void);
	int ResettoIdle(int ix);
	int ResetALLtoIdle(void);
	const char *get_errstr(int err);
	inline int get_accum_tdc(int ix, int FirmwareVersion);
	inline int get_tdc(int ix, int FirmwareVersion);

	int SetVelotronParameters(
	 int ix,
	 int FWVersion,
	 int nfront,
	 int nrear,
	 int* Chainrings,
	 int* cogset,
	 float wheeldiameter_mm,                        // mm
	 int ActualChainring,
	 int Actualcog,
	 float bike_kgs,
	 int front_index,
	 int rear_index
	 );

	struct Bike::GEARPAIR GetCurrentVTGear(int ix, int FWVersion);
	int setGear(int ix, int FWVersion, int front_index, int rear_index);


	inline float *get_bars(int ix, int FWVersion);
	inline float *get_average_bars(int ix, int FWVersion);
	inline struct SSDATA get_ss_data(int ix, int fw);

	inline const char *get_dll_version(void);

	inline float get_calories(int ix, int fw);
	inline float get_np(int ix, int fw);
	inline float get_if(int ix, int fw);
	inline float get_tss(int ix, int fw);
	inline float get_pp(int ix, int fw);                                                        // pulsepower
	inline int set_ftp(int ix, int fw, float ftp);

	int ResetAverages(int ix, int fw);
	int set_wind(int ix, int fw, float _wind_kph);
	int set_draftwind(int ix, int fw, float _draft_wind_kph);
	int update_velotron_current(int ix, unsigned short pic_current);
	int set_velotron_calibration(int ix, int fw, int _cal);
	EnumDeviceType check_for_trainers(int _ix);
	int velotron_calibration_spindown(int _ix, int _fw);



	NEW STRUCTURES:
	================

	struct SSDATA    {
		float ss;
		float lss;
		float rss;
		float lsplit;
		float rsplit;
	};

	struct TrainerData    {
		float speed;        // ALWAYS in MPH, application will metric convert.<0 on error
		float cadence;        // in RPM, any number<0 if sensor not connected or errored.
		float HR;            // in BPM, any number<0 if sensor not connected or errored.
		float Power;        // in Watts<0 on error
	}; 
 
	const char *get_dll_version(void);


	ERROR CODES:
	===============

	enum  {
		ALL_OK = 0,
		DEVICE_NOT_RUNNING = INT_MIN,            // 0x80000000
		WRONG_DEVICE,                            // 0x80000001
		DIRECTORY_DOES_NOT_EXIST,
		DEVICE_ALREADY_RUNNING,
		BAD_FIRMWARE_VERSION,
		VELOTRON_PARAMETERS_NOT_SET,
		BAD_GEAR_COUNT,
		BAD_TEETH_COUNT,
		GENERIC_ERROR
	;}

 
	float get_calories(int ix, int fw);
	float get_np(int ix, int fw);						// returns -1.0f if device not initialized
	float get_if(int ix, int fw);						// returns -1.0f if device not initialized
	float get_tss(int ix, int fw);					// returns -1.0f if device not initialized
	float get_pp(int ix, int fw);
	int set_ftp(int ix, int fw, float ftp);

	int ResetAverages(int ix, int fw);
	int set_wind(int ix, int fw, float _wind_kph);
	int set_draftwind(int ix, int fw, float _draft_wind_kph);


	for ftp meaning, see training peaks manual or talk to Roger. I no longer have
	notes on what it means, and haven't checked this stuff in years.  'ftp'
	defaults to 1.0f and I've never set it to anything else. Apparently this needs
	to be set before the training peaks variables to be meaningfull

*/

/************************************************************************************************

************************************************************************************************/

namespace RacerMateOne
{
    public class RM1
    {
        static public int MAX_FRONT_GEARS = 3;
        static public int MAX_REAR_GEARS = 10;
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

        // Fix for a weird cadence of 255.
        public const float c_WeirdCadence = 254.99f;
        public const float c_WeirdCadenceSpeed = (float)(5 * ConvertConst.MilesToKilometers);


        //public struct GearPair
        public struct GP
        {
            public int Front;
            public int Rear;
        }
        //public struct TrainerData
        public struct TD
        {
            public float Speed;
            public float Cadence;
            public float HR;
            public float Power;
        };

        //public struct SpinScanData
        public struct SSD
        {
            public float ss;
            public float lss;
            public float rss;
            public float lsplit;
            public float rsplit;
        };

        public delegate void IStatsEvent(IStats istats, object arguments);	// Depends on the event.
        public interface IStats
        {
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

        public interface IStatsEx : IStats
        {
            double Time { get; }

            float Speed_Avg { get; }
            float Speed_Max { get; }
            float Watts_Avg { get; }
            float Watts_Max { get; }
            float HeartRate_Avg { get; }
            float HeartRate_Max { get; }
            float Cadence_Avg { get; }
            float Cadence_Max { get; }
            /*
            float Cadence_Wkg { get; }

            float Distance { get; }
            float Lead { get; }
            float Grade { get; }
            float Wind { get; }
            */

            float SSLeftATA { get; }
            float SSRightATA { get; }

            float SSLeft_Avg { get; }
            float SSRight_Avg { get; }
            //float SSLeftSplit { get; }
            //float SSRightSplit { get; }
            float SS_Avg { get; }



            String GearingString { get; }
        }



        /// <summary>
        /// Device types.
        /// </summary>
        //public enum DeviceType
        //{
        //    NOT_SCANNED = 0,                // unknown, not scanned
        //    DOES_NOT_EXIST,                 // serial port does not exist
        //    EXISTS,                         // exists, openable, but no RM device on it
        //    COMPUTRAINER,
        //    VELOTRON,
        //    ACCESS_DENIED,                  // port present but can't open it because something else has it open
        //    OPEN_ERROR,                     // port present, unable to open port
        //    OTHER_ERROR                     // port present, error, none of the above
        //}
        /// <summary>
        /// Device types.
        /// </summary>
        public enum DeviceType
        {
            NOT_SCANNED =0, // unknown, not scanned
            DOES_NOT_EXIST, // serial port does not exist
            EXISTS, // exists, openable, but no RM device on it
            COMPUTRAINER,
            VELOTRON,
            SIMULATOR,
            PERF_FILE,
            ACCESS_DENIED, // port present but can't open it because something else has it open
            OPEN_ERROR, // port present, unable to open port
            OTHER_ERROR // prt present, error, none of the above
        }
        // Modified for v 155 Feb 2012 by Smeulders to align with dll EnumDeviceType, add Simulator and Perf_File


        ///// <summary>
        ///// String version of device types.
        ///// </summary>
        //public static string[] DeviceNames =
        //{
        //    "Not scanned",
        //    "Does not exists",
        //    "Exists",
        //    "CompuTrainer",
        //    "Velotron",
        //    "Access denied",
        //    "Open error",
        //    "Port does not exists",
        //    "Port open error",
        //    "Port exists but is not a trainer",
        //    "Other Error",
        //};
        /// <summary>
        /// String version of device types.
        /// </summary>
        public static string[] DeviceNames =
        {
            "Not scanned",
            "Does not exists",
            "Exists",
            "CompuTrainer",
            "Velotron",
            "simulator",
            "perf_file",
            "Access denied",
            "Open error",
            "Other Error",
        };
        // modified DeviceNames feb 2012 Smeulders to align with the Enum DeviceType
        /*
        public enum DLLError :uint
        {
            ALL_OK = 0,
            DEVICE_NOT_RUNNING = 0x80000000, // INT_MIN,            // 0x80000000
            WRONG_DEVICE,                            // 0x80000001
            DIRECTORY_DOES_NOT_EXIST,
            DEVICE_ALREADY_RUNNING,
            BAD_FIRMWARE_VERSION,
            VELOTRON_PARAMETERS_NOT_SET,
            BAD_GEAR_COUNT,
            BAD_TEETH_COUNT,
            GENERIC_ERROR
        };
         */
        //modified enum DLLError for v 155 Smeulders Feb 12 2012 to align with dll 1.0.10
        public enum DLLError : uint {
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

        //modified enum DLLError for v 155 Smeulders Feb 12 2012 to align with dll 1.0.10
        // this appears to never be used.
        public Dictionary<DLLError, String> DLLErrorText = new Dictionary<DLLError, String>() 
		{
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
        /// Velotron data.  Fill out and send to the trainer.
        /// </summary>
        [Serializable]
        public class VelotronData : ICloneable
        {
            private int[] m_Chainrings = new int[] { 28, 39, 56, 0, 0 };
            public int ChainringsCount { get; protected set; }
            public int[] Chainrings
            {
                get { return m_Chainrings; }
                set
                {
                    int i, v;
                    for (i = 0; i < MAX_FRONT_GEARS_SPACE && i < value.Length; i++)
                    {
                        v = value[i];
                        if (v == 0)
                            break;
                        m_Chainrings[i] = v;
                    }
                    ChainringsCount = i;
                    for (; i < MAX_FRONT_GEARS_SPACE; i++)
                        m_Chainrings[i] = 0;
                }
            }
            private int[] m_Cogset = new int[] { 23, 21, 19, 17, 16, 15, 14, 13, 12, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            public int CogsetCount { get; protected set; }
            public int[] Cogset
            {
                get { return m_Cogset; }
                set
                {
                    int i, v;
                    for (i = 0; i < MAX_REAR_GEARS_SPACE && i < value.Length; i++)
                    {
                        v = value[i];
                        if (v == 0)
                            break;
                        m_Cogset[i] = v;
                    }
                    CogsetCount = i;
                    for (; i < MAX_FRONT_GEARS_SPACE; i++)
                        m_Cogset[i] = 0;
                }
            }
            private float m_WheelDiameter_mm = (float)(INCHES_TO_MM * 27.0);
            private float m_WheelDiameter_inches = 27.0f;

            public float WheelDiameter_mm
            {
                get { return m_WheelDiameter_mm; }
                set
                {
                    m_WheelDiameter_mm = value;
                    m_WheelDiameter_inches = value * MM_TO_INCHES;
                }
            }
            public float WheelDiameter_inches
            {
                get { return m_WheelDiameter_inches; }
                set
                {
                    m_WheelDiameter_inches = value;
                    m_WheelDiameter_mm = value * INCHES_TO_MM;
                }
            }
            public int ActualChainring = 62;
            public int ActualCog = 14;
            public float Bike_Kg = (float)(LBS_TO_KGS * 20.0);
            public int FrontGear = 2;
            public int RearGear = 11;
            public object Clone()
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Position = 0;
                object obj = bf.Deserialize(ms);
                ms.Close();
                return obj;
            }
            public VelotronData()
            {
                ChainringsCount = 3;
                CogsetCount = 10;

            }
            //================
            public static bool IsEqual(VelotronData a, VelotronData b)
            {
                // If both are null, or both are same instance, return true.
                if (object.ReferenceEquals(a, b))
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }
                if (a.ChainringsCount != b.ChainringsCount ||
                    a.CogsetCount != b.CogsetCount ||
                    a.WheelDiameter_inches != b.WheelDiameter_inches ||
                    a.ActualChainring != b.ActualChainring ||
                    a.ActualCog != b.ActualCog ||
                    a.Bike_Kg != b.Bike_Kg ||
                    a.FrontGear != b.FrontGear ||
                    a.RearGear != b.RearGear)
                {
                    return false;
                }
                int i;
                for (i = 0; i < a.CogsetCount; i++)
                {
                    if (a.m_Cogset[i] != b.m_Cogset[i])
                        return false;
                }
                for (i = 0; i < b.ChainringsCount; i++)
                {
                    if (a.m_Chainrings[i] != b.m_Chainrings[i])
                        return false;
                }
                return true;	// Everything matches.
            }
        };

        /// <summary>
        /// The valid pad keys.
        /// </summary>
        public enum PadKeys : byte
        {
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
        /*
        [Flags]
        public enum PadKeys: ushort
        {
            Zero = 0;
            FN = (1<<0),
            F1 = (1<<1),
            F2 = (1<<2),
            F3 = (1<<3),
            UP = (1<<4),
            DOWN = (1<<5),
            NO_COMMUNICATION = (1<<6),
            F4 = (FN | F1),
            F5 = (FN | F2),
            F6 = (FN | F3),
            FN_UP = (FN | UP),
            FN_DOWN = (FN | DOWN),

            Long = (1<<7),
            Repeat = (1<<8),

            F1_Long = (F1 | Long),
            F1_Repeat = (F1 | Long | Repeat),
            F2_Long = (F2 | Long),
            F2_Repeat = (F2 | Long | Repeat),
            F3_Long = (F3 | Long),
            F3_Repeat = (F3 | Long | Repeat),
            F4_Long = (F4 | Long),
            F4_Repeat = (F4 | Long | Repeat),
            F5_Long = (F5 | Long),
            F5_Repeat = (F5 | Long | Repeat),
            F6_Long = (F6 | Long),
            F6_Repeat = (F6 | Long | Repeat),
            UP_Long = (UP | Long),
            UP_Repeat = (UP | Long | Repeat),
            DOWN_Long = (DOWN | Long),
            DOWN_Repeat = (DOWN | Long | Repeat),
            FN_UP_Long = (FN_UP | Long),
            FN_UP_Repeat = (FN_UP | Long | Repeat),
            FN_DOWN_Long = (FN_DOWN | Long),
            FN_DOWN_Repeat = (FN_DOWN | Long | Repeat),
        }; 
         */

        public enum State
        {
            Unknown,
            Initializing,
            Stopped,
            Starting,
            Running
        };



        [Flags]
        public enum StatusFlags
        {
            Zero = 0,
            CadencePickup = (1 << 11),		// 11         cadence pickup
            HeartrateSensor = (1 << 10),	// 10         heart rate sensor
            ProVersion = (1 << 9),			//  9         pro or + version, 1 = pro
            DragFactorOperating = (1 << 8),	//  8         1 if drag factor is operating
            Valid = (1 << 0)
        }
        //  7         0
        //  6         0
        //  5         0
        //  4         0
        //  3         0
        //  2         0
        //  1         0
        //  0         1


        /// <summary>
        /// Direct DLL routines 
        /// </summary>
        protected static class DLL
        {
            //[DllImport("RM1.dll")] public static extern int Setlogfilepat_h(IntPtr pathtosafefolder);
            [DllImport("RM1.dll")] public static extern int Setlogfilepath(IntPtr psf);
            //[DllImport("RM1.dll")] public static extern int Enablelog_s(bool bikelog, bool courselog, bool decoder, bool ds, bool gears, bool physics);
            [DllImport("RM1.dll")] public static extern int Enablelogs(bool bg, bool cg, bool dr, bool ds, bool gs, bool ps);
            //[DllImport("RM1.dll")] public static extern int GetRacerMateDeviceI_D(Int32 portnum);  //returned is enum DeviceType
            [DllImport("RM1.dll")] public static extern int GetRacerMateDeviceID(Int32 pn);  //returned is enum DeviceType
            //[DllImport("RM1.dll")] public static extern int GetFirmWareVersio_n(Int32 portnum);
            [DllImport("RM1.dll")] public static extern int GetFirmWareVersion(Int32 pn);
            //[DllImport("RM1.dll")] public static extern byte GetIsCalibrate_d(int ix, int FirmwareVersion);
            [DllImport("RM1.dll")] public static extern byte GetIsCalibrated(int ix, int abc_123);
            // [DllImport("RM1.dll")] public static extern int GetCalibratio_n(int ix);
            [DllImport("RM1.dll")] public static extern int GetCalibration(int ix);
            //[DllImport("RM1.dll")] public static extern uint startTraine_r(int ix, IntPtr course);
            [DllImport("RM1.dll")] public static extern uint startTrainer(int ix, IntPtr cse, int b);
           // Formally has optionals: int startTrainer(int ix, Course *_course=NULL, LoggingType _logging_type=NO_LOGGING); //
            //[DllImport("RM1.dll")] public static extern int start_traine_r(int ix, bool _b);
            [DllImport("RM1.dll")] public static extern int start_trainer(int ix, bool _b); // NEW
            //[DllImport("RM1.dll")] public static extern uint stopTraine_r(int ix);
            [DllImport("RM1.dll")] public static extern uint stopTrainer(int ix);
            //[DllImport("RM1.dll")] public static extern TrainerData GetTrainerDat_a(int ix, int FirmwareVersion);
            [DllImport("RM1.dll")] public static extern TD GetTrainerData(int ix, int abc_123);
            //[DllImport("RM1.dll")] public static extern int SetErgModeLoa_d(int ix, int FirmwareVersion, int RRC, float Load);
            [DllImport("RM1.dll")] public static extern int SetErgModeLoad(int ix, int abc_123, int R, float L);
            // [DllImport("RM1.dll")] public static extern int resetTraine_r(int ix, int FirmwareVersion, int RRC); // V
            [DllImport("RM1.dll")] public static extern int resetTrainer(int ix, int abc_123, int R); // V
            //[DllImport("RM1.dll")] public static extern int SetSlop_e(int ix, int FirmwareVersion, int RRC, float bike_kgs, float person_kgs, int DragFactor, float slope);
            [DllImport("RM1.dll")] public static extern int SetSlope(int ix, int abc_123, int R, float bs, float ps, int DFr, float se);
            //[DllImport("RM1.dll")] public static extern int SetHRBeepBound_s(int ix, int FirmwareVersion, int LowBound, int HighBound, bool BeepEnabled);
            [DllImport("RM1.dll")] public static extern int SetHRBeepBounds(int ix, int abc_123, int LB, int HB, bool BE);
            //[DllImport("RM1.dll")] public static extern int GetHandleBarButton_s(int ix, int FirmwareVersion);
            [DllImport("RM1.dll")] public static extern int GetHandleBarButtons(int ix, int abc_123);
            //[DllImport("RM1.dll")] public static extern int SetRecalibrationMod_e(int ix, int FirmwareVersion);
            [DllImport("RM1.dll")] public static extern int SetRecalibrationMode(int ix, int abc_123);
            //[DllImport("RM1.dll")] public static extern int EndRecalibrationMod_e(int ix, int FirmwareVersion);
            [DllImport("RM1.dll")] public static extern int EndRecalibrationMode(int ix, int abc_123);
            // [DllImport("RM1.dll")] public static extern int setPaus_e(int ix, bool _paused);
            [DllImport("RM1.dll")] public static extern int setPause(int ix, bool _pa);
            // [DllImport("RM1.dll")] public static extern IntPtr GetAPIVersio_n();
            [DllImport("RM1.dll")] public static extern IntPtr GetAPIVersion();
            // [DllImport("RM1.dll")] public static extern int ResettoIdl_e(int ix);
            [DllImport("RM1.dll")] public static extern int ResettoIdle(int ix);
            //[DllImport("RM1.dll")] public static extern int ResetAlltoIdl_e();
            [DllImport("RM1.dll")] public static extern int ResetAlltoIdle();
            // [DllImport("RM1.dll")] public static extern IntPtr get_errst_r(int err);
            [DllImport("RM1.dll")] public static extern IntPtr get_errstr(int err);
            //[DllImport("RM1.dll")] public static extern int get_accum_td_c(int ix, int FirmwareVersion);
            [DllImport("RM1.dll")] public static extern int get_accum_tdc(int ix, int abc_123);
            //[DllImport("RM1.dll")] public static extern int get_td_c(int idx, int FirmwareVersion);
            [DllImport("RM1.dll")] public static extern int get_tdc(int idx, int abc_123);
            /* [DllImport("RM1.dll")] public static extern uint SetVelotronParameter_s(int ix,
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
            [DllImport("RM1.dll")] public static extern uint SetVelotronParameters(
                int ix,
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
            //[DllImport("RM1.dll")] public static extern GearPair GetCurrentVTGea_r(int ix, int FWVersion);
            [DllImport("RM1.dll")] public static extern GP GetCurrentVTGear(int ix, int abc_123);
            //[DllImport("RM1.dll")] public static extern int setGea_r(int ix, int FWVersion, int front_index, int rear_index);
            [DllImport("RM1.dll")] public static extern int setGear(int ix, int abc_123, int f_i, int r_i);
            
            //[DllImport("RM1.dll")] public static extern IntPtr get_bar_s(int ix, int FWVersion);
            [DllImport("RM1.dll")] public static extern IntPtr get_bars(int ix, int abc_123);
            // [DllImport("RM1.dll")] public static extern IntPtr get_average_bar_s(int ix, int FWVersion);
            [DllImport("RM1.dll")] public static extern IntPtr get_average_bars(int ix, int abc_123);
            //[DllImport("RM1.dll")] public static extern SpinScanData get_ss_dat_a(int ix, int fw);
            [DllImport("RM1.dll")] public static extern SSD get_ss_data(int ix, int qwt);

            //[DllImport("RM1.dll")] public static extern IntPtr get_dll_versio_n();
            [DllImport("RM1.dll")] public static extern IntPtr get_dll_version();

            // [DllImport("RM1.dll")] public static extern float get_calorie_s(int ix, int fw);
            [DllImport("RM1.dll")] public static extern float get_calories(int ix, int qwt);
            //[DllImport("RM1.dll")] public static extern float get_n_p(int ix, int fw);
            [DllImport("RM1.dll")] public static extern float get_np(int ix, int qwt);						// returns -1.0f if device not initialized
            //[DllImport("RM1.dll")] public static extern float get_i_f(int ix, int fw);
            [DllImport("RM1.dll")] public static extern float get_if(int ix, int qwt);						// returns -1.0f if device not initialized
            //[DllImport("RM1.dll")] public static extern float get_ts_s(int ix, int fw);
            [DllImport("RM1.dll")] public static extern float get_tss(int ix, int qwt);					// returns -1.0f if device not initialized
            //[DllImport("RM1.dll")] public static extern float get_p_p(int ix, int fw);
            [DllImport("RM1.dll")] public static extern float get_pp(int ix, int qwt);
            //[DllImport("RM1.dll")] public static extern int set_ft_p(int ix, int fw, float ftp);
            [DllImport("RM1.dll")] public static extern int set_ftp(int ix, int qwt, float fp);

            //[DllImport("RM1.dll")] public static extern int ResetAverage_s(int ix, int fw);
            [DllImport("RM1.dll")] public static extern int ResetAverages(int ix, int qwt);
            // [DllImport("RM1.dll")] public static extern int set_win_d(int ix, int fw, float _wind_kph);
            [DllImport("RM1.dll")] public static extern int set_wind(int ix, int qwt, float _w_k);
            //[DllImport("RM1.dll")] public static extern int set_draftwin_d(int ix, int fw, float _draft_wind_kph);
            [DllImport("RM1.dll")] public static extern int set_draftwind(int ix, int qwt, float _d_w_k);

            //[DllImport("RM1.dll")] public static extern int update_velotron_curren_t(int ix, ushort pic_current);
            [DllImport("RM1.dll")] public static extern int update_velotron_current(int ix, ushort p_ct);
            //[DllImport("RM1.dll")] public static extern int set_velotron_calibratio_n(int ix, int fw, int _cal);
            [DllImport("RM1.dll")] public static extern int set_velotron_calibration(int ix, int qwt, int _c);

            //[DllImport("RM1.dll")] public static extern int check_for_trainer_s(int _ix); 
            [DllImport("RM1.dll")] public static extern int check_for_trainers(int _ix);                       //returned is enum DeviceType
            //[DllImport("RM1.dll")] public static extern int velotron_calibration_spindow_n(int _ix, int _fw);
            [DllImport("RM1.dll")] public static extern int velotron_calibration_spindown(int _ix, int _fw);
            
            //[DllImport("RM1.dll")] public static extern int get_status_bit_s(int ix, int fw);
            [DllImport("RM1.dll")] public static extern int get_status_bits(int ix, int qwt);
        }


        [Flags]
        public enum TrainerChangedFlags
        {
            Zero = 0,
            Calibrate = (1 << 0),

            Mask = (1 << 1)
        };


        // Special DLL section, Rename the DLL above to DLLs and ucomment this section. 
        #region DLL_LOG
        /*
		protected static class DLL
		{
			public static LinkedList<String> LogList = new LinkedList<string>();
			public static void Log( String str ) 
			{
				LogList.AddLast(str);
			}
			public static void ErrLog(Exception ex)
			{
				String str = String.Format("Exception in DLL call!!!\nThread:{0}\n------------\n{1}\n-------------",
					Thread.CurrentThread.Name,ex.ToString()
					);
				Debug.WriteLine(str);
				LogList.AddLast(str);
			}
			public static void WriteLog()
			{
				Debug.WriteLine("==========================================");
				foreach (String s in LogList)
				{
					Debug.WriteLine(s);
				}
				Debug.WriteLine("==========================================");
			}


			public static String intStr( IntPtr iptr ) { return String.Format("{0:x8}",iptr); }

			public static IntPtr get_dll_version()
			{
				try
				{
					IntPtr iptr = DLLs.get_dll_version();
					Log(String.Format("IntPtr get_dll_version(); {0}", Marshal.PtrToStringAnsi(iptr)));
					return iptr;
				}
				catch (Exception ex) { ErrLog(ex); }
				return IntPtr.Zero;
			}
			public static IntPtr GetAPIVersion()
			{
				try
				{
					IntPtr iptr = DLLs.GetAPIVersion();
					Log(String.Format("IntPtr GetAPIVersion(); {0}", Marshal.PtrToStringAnsi(iptr)));
					return iptr;
				}
				catch (Exception ex) { ErrLog(ex); }
				return IntPtr.Zero;
			}
			public static int Setlogfilepath(IntPtr pathtosafefolder)
			{
				try
				{
					int ans = DLLs.Setlogfilepath(pathtosafefolder);
					Log(String.Format("int Setlogfilepath(\"{0}\"); {1}", Marshal.PtrToStringAnsi(pathtosafefolder), ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int GetRacerMateDeviceID(Int32 portnum)
			{
				try
				{
					int ans = DLLs.GetRacerMateDeviceID(portnum);
					Log(String.Format("int GetRacerMateDeviceID({0}); {1}", portnum, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int GetFirmWareVersion(Int32 portnum)
			{
				try
				{
					int ans = DLLs.GetFirmWareVersion(portnum);
					Log(String.Format("int GetFirmWareVersion({0}); {1}", portnum, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int Enablelogs(bool bikelog, bool courselog, bool decoder, bool ds, bool gears, bool physics)
			{
				try
				{
					int ans = DLLs.Enablelogs(bikelog, courselog, decoder, ds, gears, physics);
					Log(String.Format("int Enablelogs( {0},{1},{2},{3},{4},{5} ); {6}", bikelog, courselog, decoder, ds, gears, physics, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;

			}
			public static byte GetIsCalibrated(int ix, int FirmwareVersion)
			{
				try
				{
					byte ans = DLLs.GetIsCalibrated(ix, FirmwareVersion);
					Log(String.Format("byte GetIsCalibrated({0},{1}); {2}", ix, FirmwareVersion, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int GetCalibration(int ix)
			{
				try
				{
					int ans = DLLs.GetCalibration(ix);
					Log(String.Format("int GetCalibrated({0}); {1}", ix, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int SetRecalibrationMode(int ix, int FirmwareVersion)
			{
				try
				{
					int ans = DLLs.SetRecalibrationMode(ix, FirmwareVersion);
					Log(String.Format("int SetRecalibrationMode({0},{1}); {2}", ix, FirmwareVersion, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int EndRecalibrationMode(int ix, int FirmwareVersion)
			{
				try
				{
					int ans = DLLs.EndRecalibrationMode(ix, FirmwareVersion);
					Log(String.Format("int EndRecalibrationMode({0},{1}); {2}", ix, FirmwareVersion, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static uint startTrainer(int ix, IntPtr course)
			{
				uint ans;
				try
				{
					ans = DLLs.startTrainer(ix,course);
					Log(String.Format("uint startTrainer({0},{1}); {2}", ix, course, ans));
				}
				catch (Exception ex)
				{
					ErrLog(ex);
					Log(String.Format("DLL: uint startTrainer({0},{1}); {2}", ix, course, ex));
					WriteLog();
					ans = (uint)DLLError.GENERIC_ERROR;
				}

				return ans;
			}
			public static uint stopTrainer(int ix)
			{
				try
				{
					uint ans = DLLs.stopTrainer(ix);
					Log(String.Format("uint stopTrainer({0}); {1}", ix, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return (uint)DLLError.GENERIC_ERROR;;
			}
			public static int GetHandleBarButtons(int ix, int FirmwareVersion)
			{
				try
				{
					int ans = DLLs.GetHandleBarButtons(ix, FirmwareVersion);
					Log(String.Format("int GetHandleBarButtons({0},{1}); {2}", ix, FirmwareVersion, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static uint SetVelotronParameters(
				int ix,
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
				)
			{
				try
				{
					uint ans = DLLs.SetVelotronParameters(ix, FWVersion, nfront, nrear, Chainrings, cogset, wheeldiameter_mm,
						ActualChainring, Actualcog, bike_kgs, front_index, rear_index);
					Log(String.Format("uint DLL.SetVelotronParameters({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
						ix, FWVersion, nfront, nrear, Chainrings, cogset, wheeldiameter_mm,
						ActualChainring, Actualcog, bike_kgs, front_index, rear_index, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return (uint)DLLError.GENERIC_ERROR; ;
			}
			public static TrainerData GetTrainerData(int ix, int FirmwareVersion)
			{
				TrainerData td;
				try
				{
					td = DLLs.GetTrainerData(ix, FirmwareVersion);
					Log(String.Format("TrainerData GetTrainerData({0},{1});\n  {2}", ix, FirmwareVersion, td));
				}
				catch (Exception ex) { ErrLog(ex); td = new TrainerData(); }
				return td;
			}
			public static SpinScanData get_ss_data(int ix, int fw)
			{
				SpinScanData ss;
				try
				{
					ss = DLLs.get_ss_data(ix, fw);
					Log(String.Format("int get_ss_data({0},{1}); {2}", ix, fw, ss));
				}
				catch (Exception ex) { ErrLog(ex); ss = new SpinScanData(); }
				return ss;
			}
			public static IntPtr get_bars(int ix, int FWVersion)
			{
				try
				{
					IntPtr iptr = DLLs.get_bars(ix, FWVersion);
					Log(String.Format("int get_bars({0},{1}); {2}", ix, FWVersion, intStr(iptr)));
					return iptr;
				}
				catch (Exception ex) { ErrLog(ex); }
				return IntPtr.Zero;
			}
			public static IntPtr get_average_bars(int ix, int FWVersion)
			{
				try
				{
					IntPtr iptr = DLLs.get_average_bars(ix, FWVersion);
					Log(String.Format("int get_average_bars({0},{1}); {2}", ix, FWVersion, intStr(iptr)));
					return iptr;
				}
				catch (Exception ex) { ErrLog(ex); }
				return IntPtr.Zero;
			}
			public static int SetSlope(int ix, int FirmwareVersion, int RRC, float bike_kgs, float person_kgs, int DragFactor, float slope)
			{
				try
				{
					int ans = DLLs.SetSlope(ix, FirmwareVersion, RRC, bike_kgs, person_kgs, DragFactor, slope);
					Log(String.Format("int SetSlope({0},{1},{2},{3},{4},{5},{6}); {7}",
						ix, FirmwareVersion, RRC, bike_kgs, person_kgs, DragFactor, slope, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int SetErgModeLoad(int ix, int FirmwareVersion, int RRC, float Load)
			{
				try 
				{
					int ans = DLLs.SetErgModeLoad(ix, FirmwareVersion, RRC, Load);
					Log(String.Format("int SetErgModeLoad({0},{1},{2},{3}); {4}",
						ix, FirmwareVersion, RRC, Load, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int SetHRBeepBounds(int ix, int FirmwareVersion, int LowBound, int HighBound, bool BeepEnabled)
			{
				try
				{
					int ans = DLLs.SetHRBeepBounds(ix, FirmwareVersion, LowBound, HighBound, BeepEnabled);
					Log(String.Format("int SetHRBeepBounds({0},{1},{2},{3},{4}); {5}",
						ix, FirmwareVersion, LowBound, HighBound, BeepEnabled, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static GearPair GetCurrentVTGear(int ix, int FWVersion)
			{
				GearPair gp;
				try
				{
					gp = DLLs.GetCurrentVTGear(ix, FWVersion);
					Log(String.Format("int SetErgModeLoad({0},{1}); {2}",
						ix, FWVersion, gp));
				}
				catch (Exception ex) { ErrLog(ex); gp = new GearPair(); }
				return gp;
			}
			public static int setGear(int ix, int FWVersion, int front_index, int rear_index)
			{
				try
				{
					int ans = DLLs.setGear(ix, FWVersion, front_index, rear_index);
					Log(String.Format("int setGear({0},{1},{2},{3}); {4}", ix, FWVersion, front_index, rear_index, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static float get_calories(int ix, int fw)
			{
				try
				{
					float ans = DLLs.get_calories(ix, fw);
					Log(String.Format("int get_calories({0},{1}); {2}", ix, fw, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static float get_np(int ix, int fw)
			{
				try
				{
					float ans = DLLs.get_np(ix, fw);
					Log(String.Format("int get_np({0},{1}); {2}", ix, fw, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0.0f;
			}
			public static float get_if(int ix, int fw)
			{
				try
				{
					float ans = DLLs.get_if(ix, fw);
					Log(String.Format("int get_if({0},{1}); {2}", ix, fw, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static float get_tss(int ix, int fw)
			{
				try
				{
					float ans = DLLs.get_tss(ix, fw);
					Log(String.Format("int get_tss({0},{1}); {2}", ix, fw, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static float get_pp(int ix, int fw)
			{
				try
				{
					float ans = DLLs.get_pp(ix, fw);
					Log(String.Format("int get_pp({0},{1}); {2}", ix, fw, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int set_ftp(int ix, int fw, float ftp)
			{
				try
				{
					int ans = DLLs.set_ftp(ix, fw,ftp);
					Log(String.Format("int set_ftp({0},{1},{2}); {3}", ix, fw, ftp,ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int ResetAverages(int ix, int fw)
			{
				try
				{
					int ans = DLLs.ResetAverages(ix, fw);
					Log(String.Format("int ResetAverages({0},{1}); {2}", ix, fw, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int set_wind(int ix, int fw, float _wind_kph)
			{
				try
				{
					int ans = DLLs.set_wind(ix, fw,_wind_kph);
					Log(String.Format("int set_wind({0},{1},{2}); {3}", ix, fw, _wind_kph,ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
			public static int set_draftwind(int ix, int fw, float _draft_wind_kph)
			{
				try
				{
					int ans = DLLs.set_draftwind(ix, fw, _draft_wind_kph);
					Log(String.Format("int set_draftwind({0},{1},{2}); {3}", ix, fw, _draft_wind_kph, ans));
					return ans;
				}
				catch (Exception ex) { ErrLog(ex); }
				return 0;
			}
		}
		*/
        #endregion DLL_LOG
        // =====================

        public int _status_logpath;
        static RM1()
        {
            BarRadians = new Double[RM1.BarCount];
            double step = Math.PI / RM1.HalfBarCount;
            for (int i = 0; i < RM1.HalfBarCount; i++)
            {
                BarRadians[i + RM1.HalfBarCount] = BarRadians[i] = step * i + (step * 0.5);
            }

        }


        RM1()
        {
            IntPtr pfullpath = Marshal.StringToHGlobalAnsi(RacerMatePaths.DebugFullPath);
            _status_logpath = DLL.Setlogfilepath(pfullpath);
            Marshal.FreeBSTR(pfullpath);

            DLL.Enablelogs(true, true, true, true, true, true);
        }

        private static String ms_DLLVersion;
        public static String DLLVersion
        {
            get
            {
                if (ms_DLLVersion == null)
                {
                    IntPtr iptr = DLL.get_dll_version();
                    ms_DLLVersion = Marshal.PtrToStringAnsi(iptr);
                }
                return ms_DLLVersion;
            }
        }
        private static String ms_APIVersion;
        public static String APIVersion
        {
            get
            {
                if (ms_APIVersion == null)
                {
                    IntPtr iptr = DLL.GetAPIVersion();
                    ms_APIVersion = Marshal.PtrToStringAnsi(iptr);
                }
                return ms_APIVersion;
            }
        }

        /**
         * Lazy Thread safe singleton implementation
         */
        public static RM1 Instance { get { return Nested.instance; } }
        /** Private class for the singleton */
        class Nested
        {
            static Nested() { }
            internal static readonly RM1 instance = new RM1();
        }



        public static DeviceType GetRacerMateDeviceID(int portnum)
        {
            //if (portnum == 6)
            //	return DeviceType.OPEN_ERROR;
            DeviceType id;
            try
            {
                // my spoof
                //string[] Portnames = System.IO.Ports.SerialPort.GetPortNames();
                //if (!Portnames.Contains("COM" + (portnum + 1)))
                //{ id = DeviceType.DOES_NOT_EXIST; }
                //else 
                    // nice limited list of active coms, not 256
                    //look for the COMnumber in the list
                    id = (DeviceType)DLL.GetRacerMateDeviceID(portnum);
            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString());
                id = DeviceType.OPEN_ERROR;
            }
            //Log.WriteLine("opened " + portnum + "  got " + id.ToString());
            return id;
        }


        public delegate void TrainerEvent(Trainer trainer, object arguments);	// Depends on the event.
        public static event TrainerEvent OnClosed; // Any trainer closed. arguments is null

        public delegate void IntervalEvent(Int64 LastTicks, float SplitTime);
        public static event IntervalEvent OnInterval;

        public delegate void TrainerInitialized(Trainer trainer, int left);
        public static event TrainerInitialized OnTrainerInitialized;

        public static event TrainerEvent OnCalibrationChanged;

        public delegate void TrainerPadKey(Trainer trainer, RM1.PadKeys key, double pressed);
        public static event TrainerPadKey OnPadKey;

        public delegate void UpdateEvent(double splittime);
        public static event UpdateEvent OnUpdate;


        /// <summary>
        /// All the trainers comm ports.  Not thread safe.   Only  add/remove from main thread.
        /// </summary>
        protected static Dictionary<int, Trainer> ms_Trainers = new Dictionary<int, Trainer>();
        /// <summary>
        /// Trainers that need to be initizlized  Uses the ms_mux to add and remove from.
        /// </summary>
        protected static LinkedList<Trainer> ms_InitList = new LinkedList<Trainer>();

        /// <summary>
        /// When a trainer needs to be started add it here.
        /// </summary>
        protected static LinkedList<Trainer> ms_StartList = new LinkedList<Trainer>();

        /// <summary>
        /// All the trainers that have been successuflly started.
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
                    foreach (KeyValuePair<int, RM1.Trainer> kp in ms_Trainers)
                    {
                        RM1.Trainer t = kp.Value;
                        if (t.IsConnected || t.ShouldBe != DeviceType.DOES_NOT_EXIST)
                            ms_HardwareList.Add(t);
                    }
                    ms_HardwareListVersion = ms_ActiveListVersion;
                }
                return ms_HardwareList;
            }
        }



        /// <summary>
        /// Number of computrainers detedted
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
        public static void MutexException(Exception ex)
        {
            Debug.WriteLine(String.Format("Exception in MUTEX!!!\nThread:{0}\n------------\n{1}\n-------------",
                Thread.CurrentThread.Name, ex.ToString())
                );
            m_MuxException.Add(ex);

        }


        public static int TrainersInitializing
        {
            get
            {
                int num;
                ms_Mux.WaitOne();
                try
                {
                    num = ms_InitList.Count;
                }
                catch (Exception ex) { MutexException(ex); num = 0; }
                ms_Mux.ReleaseMutex();
                return num;
            }
        }
        public static int ValidTrainerCount
        {
            get { return ms_ActiveList.Count; }
        }

        public static List<Trainer> ValidTrainers { get { return ms_ActiveList; } }

        /// <summary>
        /// Represents one comm port - (PortNumber) 
        /// </summary>
        public class Trainer : DispatcherObject, INotifyPropertyChanged, IStats
        {
            [Flags]
            public enum UpdateFlags
            {
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
            public void SetUpdateFlags(UpdateFlags flags)
            {
                lock (m_UpdateLock)
                {
                    m_UpdateFlags |= flags;
                }
            }
            UpdateFlags ClearUpdateFlags(out bool paused)
            {
                UpdateFlags f;
                lock (m_UpdateLock)
                {
                    paused = m_Paused;
                    f = m_UpdateFlags;
                    m_UpdateFlags = UpdateFlags.Zero;
                }
                return f;
            }


            public TrainerChangedFlags m_Changed = TrainerChangedFlags.Zero;

            public static int FakePort = 99;
            public static Trainer Fake;

            public RM1.State State { get; protected set; }

            public bool Closed { get; protected set; }
            public readonly int PortNumber;
            public DeviceType Type { get; protected set; }

            protected bool bFake;
            public int FakeKeys;

            /// <summary>
            /// What previous times through have got this.
            /// </summary>
            public DeviceType ShouldBe = DeviceType.DOES_NOT_EXIST;
        
            public bool IsConnected { get; protected set; }
            public bool IsStarted { get; protected set; }
            public DLLError LastError { get; protected set; }

            protected System.Timers.Timer m_Interval;


            protected VelotronData m_VelotronData;
            public VelotronData VelotronData
            {
                get { return m_VelotronData; }
                set
                {
                    if (VelotronData.IsEqual(m_VelotronData, value))
                    {
                        m_VelotronData = value;
                    }
                    SetVelotron_trnr_Parameters();
                }
            }

            public String TypeString
            {
                get
                {
                    return (Type == DeviceType.COMPUTRAINER ? "CompuTrainer" : Type == DeviceType.VELOTRON ? "Velotron" : "Unknown");
                }
            }

            public int Ver { get; protected set; }
            public int VersionNum { get; protected set; }

            public bool IsCalibrated { get; protected set; }
            public int CalibrationValue { get; protected set; }
            public String CalibrationString
            {
                get
                {
                    if (!IsConnected)
                        return "";
                    if (Type == DeviceType.COMPUTRAINER)
                    {
                        return String.Format("{0:F2}", CalibrationValue / 100.0);
                    }
         //Paul Added feb 5 2012 bugrocket Ticket 11
                    else if (Type == DeviceType.VELOTRON)
                    {
                        return CalibrationValue.ToString();
                    }
                    //End Paul Added feb 5 2102
                    return "";
                }
            }



            /** <summary>String version or "Invaild" if not valid</summary> */
            public string Version
            {
                get
                {
                    int v = VersionNum;
                    if (v == 0)
                        return "Invalid";
                    int t = v % 100;
                    return "" + (v / 100) + "." + (t < 10 ? "0" : "") + t;
                }
            }


            Rider m_SetRider;
            protected Rider m_Rider;
            public Rider Rider
            {
                get { return m_SetRider; }
                set
                {
                    m_SetRider = value;
                    m_Rider = value != null ? value : Riders.DefaultRider;
                    FTP = m_Rider != null ? (float)m_Rider.PowerFTP : 1.0f;
                    if (Type == DeviceType.VELOTRON && m_Rider != null)
                    {
                        VelotronData vd = (VelotronData)m_VelotronData.Clone();
                        vd.Cogset = m_Rider.GearingCogset;
                        vd.Chainrings = m_Rider.GearingCrankset;
                        vd.FrontGear = m_FrontGearNumber;
                        vd.RearGear = m_RearGearNumber;
                        vd.Bike_Kg = m_Rider.WeightBikeKGS;
                        VelotronData = vd;
                        SetVelotronGears();
                    }
                    SetUpdateFlags(UpdateFlags.Drag);
                }
            }
            protected float m_FTP = 1.0f;
            public float FTP
            {
                get { return m_FTP; }
                set
                {
                    if (m_FTP != value)
                    {
                        m_FTP = value;
                        SetUpdateFlags(UpdateFlags.FTP);
                    }
                }
            }

            protected bool m_Paused = false;
            public bool Paused
            {
                get { return m_Paused; }
                set
                {
                    if (m_Paused == value)
                        return;

                    m_Paused = value;
                    SetUpdateFlags(UpdateFlags.Pause);
                }
            }
            public void SetPaused(bool paused, bool force)
            {
                if (force || paused != m_Paused)
                {
                    m_Paused = paused;
                    SetUpdateFlags(UpdateFlags.Pause);
                }
            }



            protected bool m_bERG;
            protected float m_Grade;
            public float Grade
            {
                get { return m_Grade; }
                set
                {
                    if (m_bERG || m_Grade != value)
                    {
                        m_bERG = false;
                        m_Grade = value;
                        SetUpdateFlags(UpdateFlags.Grade);
                    }
                }
            }
            public void SetGrade(float grade, bool force)
            {
                if (force || m_Grade != grade || m_bERG)
                {
                    m_bERG = false;
                    m_Grade = grade;
                    SetUpdateFlags(UpdateFlags.Grade);
                }
            }
            public void UpdateDragFactor()
            {
                SetUpdateFlags(UpdateFlags.Grade);
            }


            protected float m_Watts_Load;
            public float Watts_Load
            {
                get { return m_Watts_Load; }
                set
                {
                    if (!m_bERG || m_Watts_Load != value)
                    {
                        m_Watts_Load = value;
                        m_bERG = true;
                        SetUpdateFlags(UpdateFlags.Watts);
                    }
                }
            }
            public void SetWattsLoad(float load, bool force)
            {
                if (force || m_Watts_Load != load || !m_bERG)
                {
                    m_Watts_Load = load;
                    m_bERG = true;
                    SetUpdateFlags(UpdateFlags.Watts);
                }
            }

            protected StatusFlags m_StatusFlags = StatusFlags.Zero;
            public StatusFlags StatusFlags { get { return m_StatusFlags; } }


            protected int m_Drag;
            public int Drag
            {
                get { return m_Drag; }
                set
                {
                    int v = value < 0 ? 0 : value > 120 ? 120 : value;
                    if (m_Drag != value)
                    {
                        m_Drag = v;
                        SetUpdateFlags(UpdateFlags.Drag);
                    }
                }
            }


            protected float m_Wind; // Stored in KPH for trainer.
            public float Wind
            {
                get { return (float)(m_Wind * ConvertConst.KPHToMetersPerSecond); }
                set
                {
                    float v = (float)(value * ConvertConst.MetersPerSecondToKPH);
                    if (m_Wind != v)
                    {
                        m_Wind = v;
                        SetUpdateFlags(UpdateFlags.Wind);
                    }
                }
            }
            public void SetWind(float wind, bool bforce)
            {
                wind = (float)(wind * ConvertConst.MetersPerSecondToKPH);
                if (bforce || m_Wind != wind)
                {
                    m_Wind = wind;
                    SetUpdateFlags(UpdateFlags.Wind);
                }
            }


            protected bool m_Drafting;
            public bool Drafting
            {
                get { return m_Drafting; }
                set
                {
                    if (m_Drafting != value)
                    {
                        m_Drafting = value;
                        SetUpdateFlags(UpdateFlags.Wind | UpdateFlags.Drafting);
                    }
                }
            }

            public void Reset_trnr_Averages()
            {
                SetUpdateFlags(UpdateFlags.ResetAverages);
            }




            public int m_FrontGearNumber;
            public int FrontGearNumber
            {
                get { return m_FrontGearNumber; }
                set
                {
                    m_FrontGearNumber = value < 0 ? 0 : value >= m_VelotronData.ChainringsCount ? m_VelotronData.ChainringsCount - 1 : value;
                    SetVelotronGears();
                }
            }
            public int m_RearGearNumber;
            public int RearGearNumber
            {
                get { return m_RearGearNumber; }
                set
                {
                    m_RearGearNumber = value < 0 ? 0 : value >= m_VelotronData.CogsetCount ? m_VelotronData.CogsetCount - 1 : value;
                    SetVelotronGears();
                }
            }




            /// <summary>
            /// 
            /// </summary>
            /// <param name="portnum">0 Based portnumber. COMM1 = 0</param>
            Trainer(int portnum)
            {
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

                ms_Trainers[portnum] = this;
                PortNumber = portnum;

                if (portnum == FakePort)
                {
                    bFake = true;
                    Type = DeviceType.COMPUTRAINER;
                    PortNumber = 99;
                    VersionNum = 4543;
                    Ver = 4095;
                    IsConnected = true;
                }
                ms_Mux.WaitOne();
                try
                {
                    ms_InitList.AddLast(this);
                }
                catch (Exception ex) { MutexException(ex); }
                ms_Mux.ReleaseMutex();
            }
            ~Trainer()
            {
                Stop();
            }

            public void Close()
            {
                if (Closed)
                    return;
                Stop();
                Closed = true;
                if (this.OnClosed != null)
                    this.OnClosed(this, null);
                if (RM1.OnClosed != null)
                    RM1.OnClosed(this, null);
            }

            public static Trainer Get(int portnum)
            {
                Trainer t;
                if (portnum < 0)
                    portnum = FakePort;

                if (ms_Trainers.ContainsKey(portnum))
                    t = ms_Trainers[portnum];
                else
                {
                    t = new Trainer(portnum);
                    if (portnum == FakePort)
                        Fake = t;
                }
                return t;
            }
            public static Trainer Find(int portnum)
            {
                return (ms_Trainers.ContainsKey(portnum) ? ms_Trainers[portnum] : null);
            }



            bool m_CalibrateMode = false;
            bool m_CurCalibrationMode = false;
            public bool CalibrateMode
            {
                get { return m_CalibrateMode; }
                set
                {
                    //Debug.WriteLine("RM1.cs:::calibrate mode is being changed to : " + value.ToString());
                   // Debug.WriteLine("Stack trace = '{0}'", Environment.StackTrace);
                    bool v = IsStarted ? value : false;
                    if (v == m_CalibrateMode)
                        return;
                    ms_Mux.WaitOne();
                    try
                    {
                        m_Changed |= TrainerChangedFlags.Calibrate;
                        m_CalibrateMode = v;
                    }
                    catch (Exception ex) { MutexException(ex); }
                    ms_Mux.ReleaseMutex();
                }
            }




            protected bool Start()
            {
                if (Type != DeviceType.VELOTRON && Type != DeviceType.COMPUTRAINER)
                    return false;
                if (!IsStarted)
                {
                    ms_Mux.WaitOne();
                    try
                    {
                        if (Type == DeviceType.VELOTRON)
                        {
                            DLLError derr = (DLLError)DLL.SetVelotronParameters(PortNumber, Ver,
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
                        else
                        {
                            try
                            {
                                ans = (DLLError)RM1.DLL.startTrainer(PortNumber, IntPtr.Zero,0);
                                DLL.start_trainer(PortNumber, true);
                            }
                            catch (Exception exc)
                            {
                                Debug.WriteLine("ERROR IN STARTING Trainer: " + exc.ToString());
                                ans = DLLError.GENERIC_ERROR;
                            }
                        }
                        if (ans == DLLError.ALL_OK)
                        {
                            Log.WriteLine(TypeString + " " + Version + " - Started");
                            IsStarted = true;
                            if (!ms_StartedList.Contains(this))
                                ms_StartedList.AddLast(this);
                            m_Buttons = DLL.GetHandleBarButtons(PortNumber, Ver) & 0x3f;
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
            }

            /// <summary>
            /// Stops the trainer.
            /// </summary>
            protected void Stop()
            {
                if (IsStarted)
                {
                    ms_Mux.WaitOne();
                    try
                    {
                        CalibrateMode = false;
                        Log.WriteLine(TypeString + " " + Version + " - Stopped");
                        IsStarted = false;
                        if (!bFake)
                            RM1.DLL.stopTrainer(PortNumber);
                        ms_StartedList.Remove(this);
                    }
                    catch (Exception ex) { MutexException(ex); }
                    ms_Mux.ReleaseMutex();
                }
            }
#pragma warning disable 649
            static int temp_int = 0;
            static IntPtr ms_ip_chainrings = Marshal.AllocHGlobal(Marshal.SizeOf(temp_int) * 32);
            static IntPtr ms_ip_cogset = Marshal.AllocHGlobal(Marshal.SizeOf(temp_int) * 32);

            protected VelotronData m_CurVelotron;
            public DLLError SetVelotron_trnr_Parameters()
            {
                if (Type != DeviceType.VELOTRON)
                    return DLLError.WRONG_DEVICE;
                if (m_CurVelotron == m_VelotronData)
                    return DLLError.ALL_OK;	// Don't need to do anything.

                ms_Mux.WaitOne();
                try
                {
                    Marshal.Copy(m_VelotronData.Chainrings, 0, ms_ip_chainrings, MAX_FRONT_GEARS);
                    Marshal.Copy(m_VelotronData.Cogset, 0, ms_ip_cogset, MAX_REAR_GEARS);

                    if (!ms_StartList.Contains(this))
                        ms_StartList.AddLast(this);
                }
                catch (Exception ex) { MutexException(ex); }
                ms_Mux.ReleaseMutex();

                return DLLError.ALL_OK;
            }

            //=============================================================
            protected String m_CBLine = null;
            public String CBLine
            {
                get
                {
                    if (!IsConnected)
                    {
                        if (ShouldBe == DeviceType.COMPUTRAINER || ShouldBe == DeviceType.VELOTRON)
                            return "COM" + (PortNumber + 1) + ": /" + DeviceNames[(int)Type] + " - Not detected";
                        return "COM" + (PortNumber + 1) + ": /Unknown";
                    }
                    m_CBLine = "COM" + (PortNumber + 1) + ": / v" + Version + " /" + DeviceNames[(int)Type] + " / " +
                        (Type == DeviceType.COMPUTRAINER ? "RRC = " : "Accuwatt = ") + CalibrationString;
                    return m_CBLine;
                }
            }

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

            public float Speed { get { return (float)(m_TrainerData.Speed * ConvertConst.KPHToMetersPerSecond); } }
            public float Cadence { get { return m_TrainerData.Cadence; } }
            public float HeartRate { get { return (float)Math.Round(m_TrainerData.HR); } }
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

            public int FrontGear
            {
                get
                {
                    return m_GearPair.Front;
                }
            }	// Velotron only -1 if not valid
            public int RearGear
            {
                get
                {
                    return m_GearPair.Rear;
                }
            }	// Velotron only -1 if not valid

            public int GearInches
            {
                get
                {
                    return Type == DeviceType.VELOTRON && m_GearPair.Rear > 0 ? (int)m_VelotronData.WheelDiameter_inches * m_GearPair.Front / m_GearPair.Rear : 0;
                }
            }


            public event IStatsEvent OnUpdate;
            //=============================================================================
            class Key
            {
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

                public Key(Trainer trainer, PadKeys key, PadKeys shortkey, PadKeys longkey, PadKeys repeat)
                {
                    m_Trainer = trainer;
                    m_Key = key;
                    m_ShortKey = shortkey;
                    m_LongKey = longkey;
                    m_RepeatKey = repeat;
                    if (longkey != RM1.PadKeys.NOKEY || repeat != RM1.PadKeys.NOKEY)
                        m_Node = new LinkedListNode<Key>(this);
                }
                public void Down()
                {
                    if (!m_Pressed)
                    {
                        m_DownTime = ms_LastTicks;
                        m_Next = m_DownTime + c_LongTicks;
                        if (m_Node != null)
                            m_Trainer.m_KeysDown.AddLast(m_Node);
                        AppWin.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new xkey(OnKey), m_Key, 0);
                        m_Pressed = true;
                        m_Count = 0;
                    }
                }
                public void Up()
                {
                    if (m_Pressed)
                    {
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
                public void Update()
                {
                    if (ms_LastTicks >= m_Next)
                    {
                        m_Next = m_Next + (m_Count > c_HCount ? c_HRepeatTicks : c_RepeatTicks);
                        if (ms_LastTicks >= m_Next)
                            m_Next = ms_LastTicks + 1; // Make sure this doesn't keep going back and back
                        PadKeys key = m_Count > 0 ? m_RepeatKey : m_LongKey;
                        if (key != RM1.PadKeys.NOKEY)
                            AppWin.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new xkey(OnKey), key, 0);
                        m_Count++;
                    }
                }
                public void OnKey(PadKeys key, double pressed)
                {
                    Log.WriteLine(String.Format("{0} {1}", key.ToString(), pressed == 0 ? "Down" : "Up " + String.Format("{0:F2} seconds", pressed)));

                    if (m_Trainer.OnPadKey != null)
                        m_Trainer.OnPadKey(m_Trainer, key, pressed);
                    if (RM1.OnPadKey != null)
                        RM1.OnPadKey(m_Trainer, key, pressed);
                }
                public bool IsDown { get { return m_Pressed; } }
            }
            LinkedList<Key> m_KeysDown = new LinkedList<Key>();
            void UpdateKeysDown()
            {
                foreach (Key k in m_KeysDown)
                    k.Update();
            }

            Key[] m_Keys = new Key[(int)RM1.PadKeys.MAX];
            void InitKeys()
            {
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

            public bool NoCommunication
            {
                get { return m_Keys[(int)RM1.PadKeys.NO_COMMUNICATION].IsDown; }
            }
            //=============================================================================



            public TD TrainerData { get { return m_TrainerData; } }
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
            private static void s_event(TrainerEvent trainerevent, RM1.Trainer trainer, object obj)
            {
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
            protected void Update()
            {
                if (!IsStarted)
                    return;
                m_IntervalCount++;
                bool paused;
                UpdateFlags uflags = ClearUpdateFlags(out paused);

                // Do all the nessary update flags.
                //==================================
                if (uflags != UpdateFlags.Zero)
                {
                    if ((uflags & UpdateFlags.Pause) != UpdateFlags.Zero && !paused)
                    {
                        DLL.setPause(PortNumber, paused);
                        m_CurPaused = paused;
                    }

                    if ((uflags & UpdateFlags.ResetAverages) != UpdateFlags.Zero)
                    {
                        DLL.ResetAverages(PortNumber, Ver);
                        DLL.start_trainer(PortNumber, true);
                        uflags |= UpdateFlags.Grade | UpdateFlags.Watts | UpdateFlags.Drag | UpdateFlags.Pause | UpdateFlags.FTP | UpdateFlags.Pause;
                    }
                    if ((uflags & UpdateFlags.FTP) != UpdateFlags.Zero)
                        DLL.set_ftp(PortNumber, Ver, m_FTP);
                    if ((uflags & UpdateFlags.Pause) != UpdateFlags.Zero && paused)
                    {
                        DLL.setPause(PortNumber, paused);
                        m_CurPaused = paused;
                    }
                    if ((uflags & UpdateFlags.Drafting) != UpdateFlags.Zero)
                    {
                        DLL.set_draftwind(PortNumber, Ver, m_Drafting ? RM1.DraftWind : 0.0f);
                    }
                    if ((uflags & UpdateFlags.Wind) != UpdateFlags.Zero)
                    {
                        /*
                        if (m_CurWind != m_Wind)
                        {
                            m_CurWind = m_Wind;
                            Log.WriteLine(String.Format("Wind changed to {0}", m_Wind));
                        }
                        */
                        DLL.set_wind(PortNumber, Ver, m_Wind);// + (m_Drafting ? RM1.DraftWind : 0.0f));
                    }
                    if ((uflags & (UpdateFlags.Grade | UpdateFlags.Watts | UpdateFlags.Drag)) != UpdateFlags.Zero)
                    {
                        if (m_bERG)
                        {
                            m_bSetErg = true;
                            DLL.SetErgModeLoad(PortNumber, Ver, CalibrationValue, AppWin.PreviewMode ? 0 : m_Watts_Load);
                        }
                        else
                        {
                            if (m_Rider == null)
                            {
                                if (m_bSetErg)
                                {
                                    if (m_Grade == 0)
                                        DLL.SetSlope(PortNumber, Ver, CalibrationValue, 0.0f, 0.0f, 100, 1);
                                    m_bSetErg = false;
                                }
                                DLL.SetSlope(PortNumber, Ver, CalibrationValue, 0.0f, 0.0f, 100, AppWin.PreviewMode ? 0 : m_Grade);
                            }
                            else
                            {
                                float bw = m_Rider.WeightBikeKGS;
                                float rw = m_Rider.WeightRiderKGS;
                                if (m_Rider.DragFactor != m_CurDragFactor)
                                {
                                    m_CurDragFactor = m_Rider.DragFactor;
                                    Log.WriteLine(String.Format("Drag factor changed to {0}", m_CurDragFactor));
                                }
                                if (m_bSetErg)
                                {
                                    if (m_Grade == 0)
                                        DLL.SetSlope(PortNumber, Ver, CalibrationValue, 0.0f, 0.0f, 100, 1);
                                    m_bSetErg = false;
                                }
                                DLL.SetSlope(PortNumber, Ver, CalibrationValue, bw, rw, m_CurDragFactor, AppWin.PreviewMode ? 0 : m_Grade);
                            }
                        }
                    }
                }
                //==================================




                int raw = (bFake ? FakeKeys : DLL.GetHandleBarButtons(PortNumber, VersionNum));
                if (AppWin.PreviewMode)
                    raw &= 0x7fffff80;
                if (raw != 0)
                    m_KeyZeroCount = 0;
                else
                {
                    m_KeyZeroCount++;
                    if (m_KeyZeroCount <= 1)
                    {
                        UpdateKeysDown();
                        return;		// Just don't do the first zero we get.
                    }
                }
                int b = raw & 0x7f;
                //modified here by Paul Smeulders Feb 5 2012 Bugrocket Ticket 1, remove the bit swap on CompuTrainer key bits.
                if (Type == DeviceType.COMPUTRAINER)
                {
                    //    // Swap the first two bits.
                    //    b = (b & ~3) | ((b & 1) << 1) | ((b & 2) >> 1);
                    //end modify by Paul Smeulders
                }
                if (m_Buttons != b)
                {
                    // Raise an event for the keypad
                    int changed = b ^ m_Buttons;
                    int down = changed & b;
                    int up = changed & m_Buttons;
                    RawChanged = changed;
                    RawDown = down;
                    RawUp = up;

                    bool fn = (b & 1) != 0;
                    RM1.PadKeys key;
                    if (down != 0)
                    {
                        if ((down & 2) != 0)
                        {
                            key = fn ? RM1.PadKeys.F4 : RM1.PadKeys.F1;
                            m_Keys[(int)key].Down();
                        }
                        if ((down & 4) != 0)
                        {
                            key = fn ? RM1.PadKeys.F5 : RM1.PadKeys.F2;
                            m_Keys[(int)key].Down();
                        }
                        if ((down & 8) != 0)
                        {
                            key = fn ? RM1.PadKeys.F6 : RM1.PadKeys.F3;
                            m_Keys[(int)key].Down();
                        }
                        if ((down & 16) != 0)
                        {
                            key = fn ? RM1.PadKeys.FN_UP : RM1.PadKeys.UP;
                            m_Keys[(int)key].Down();
                        }
                        if ((down & 32) != 0)
                        {
                            key = fn ? RM1.PadKeys.FN_DOWN : RM1.PadKeys.DOWN;
                            m_Keys[(int)key].Down();
                        }
                        if ((down & 64) != 0)
                        {
                            Debug.WriteLine("no communication");

                            key = RM1.PadKeys.NO_COMMUNICATION;
                            m_Keys[(int)key].Down();
                            Unit unit = Unit.GetUnit(this);
                            if (unit != null)
                            {
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
                    if (up != 0)
                    {
                        if ((up & 2) != 0)
                        {
                            m_Keys[(int)RM1.PadKeys.F1].Up();
                            m_Keys[(int)RM1.PadKeys.F4].Up();
                        }
                        if ((up & 4) != 0)
                        {
                            m_Keys[(int)RM1.PadKeys.F2].Up();
                            m_Keys[(int)RM1.PadKeys.F5].Up();
                        }
                        if ((up & 8) != 0)
                        {
                            m_Keys[(int)RM1.PadKeys.F3].Up();
                            m_Keys[(int)RM1.PadKeys.F6].Up();
                        }
                        if ((up & 16) != 0)
                        {
                            m_Keys[(int)RM1.PadKeys.UP].Up();
                            m_Keys[(int)RM1.PadKeys.FN_UP].Up();
                        }
                        if ((up & 32) != 0)
                        {
                            m_Keys[(int)RM1.PadKeys.DOWN].Up();
                            m_Keys[(int)RM1.PadKeys.FN_DOWN].Up();
                        }
                        if ((up & 64) != 0)
                        {
                            m_Keys[(int)RM1.PadKeys.NO_COMMUNICATION].Up();
                            SetCalibrationValue(false);
                            Unit unit = Unit.GetUnit(this);
                            if (unit != null)
                            {
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
                if (m_DownGear && ms_LastTicks > m_FN_DOWN_Ticks)
                {
                    m_DownGear = false;
                    //FrontGearNumber--;
                }
                if (m_UpGear && ms_LastTicks > m_FN_UP_Ticks)
                {
                    m_UpGear = false;
                    //FrontGearNumber++;
                }

                // Deal with the trainer data.
                if (!bFake)
                {
                    try
                    {
                        if (m_Changed != TrainerChangedFlags.Zero)
                        {
                            if (m_CurCalibrationMode != m_CalibrateMode)
                            {
                                m_CurCalibrationMode = m_CalibrateMode;
                                if (m_CurCalibrationMode)
                                {
                                    
                                    //Debug.WriteLine("RM1.cs::Entering calibration mode...");
                                   //Debug.WriteLine("stack trace : '{0}'" , Environment.StackTrace ); 
                                    Thread.Sleep(100);
                                    DLL.SetRecalibrationMode(PortNumber, Ver);
                                    Thread.Sleep(100);
                                  //  Debug.WriteLine("RM1.cs::...Calibration mode entered.\n");
                                }
                                else
                                {
                                  //  Debug.WriteLine("RM1.cs::Exiting calibration mode...");
                                    Thread.Sleep(100);
                                    DLL.EndRecalibrationMode(PortNumber, Ver);
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

                        if (!m_CurCalibrationMode)
                        {
                            if (!AppWin.PreviewMode)
                            {
                                m_TrainerData = DLL.GetTrainerData(PortNumber, Ver);
                                // Sanity checks on the trainer data.

                                if (m_TrainerData.Cadence >= c_WeirdCadence && m_TrainerData.Speed < c_WeirdCadenceSpeed)
                                    m_TrainerData.Cadence = 0;


                                m_SpinScanData = DLL.get_ss_data(PortNumber, Ver);
                                ip = DLL.get_bars(PortNumber, Ver);
                                Marshal.Copy(ip, m_Bars, 0, 24);

                                ip = DLL.get_average_bars(PortNumber, Ver);
                                Marshal.Copy(ip, m_AverageBars, 0, 24);

                                m_PulsePower = DLL.get_pp(PortNumber, Ver);
                                m_Calories = DLL.get_calories(PortNumber, Ver);
                                m_IF = DLL.get_if(PortNumber, Ver);
                                m_NP = DLL.get_np(PortNumber, Ver);
                                m_TSS = DLL.get_tss(PortNumber, Ver);
                                StatusFlags sf = (StatusFlags)DLL.get_status_bits(PortNumber, Ver);
                                if (m_StatusFlags != sf)
                                {
                                    m_StatusFlags = sf;
                                    Log.WriteLine(String.Format("Trainer Port {0}, Status Bit changed (0x{1:X4},{2})",
                                        PortNumber + 1, (Int32)sf, sf.ToString(), sf.ToString()));
                                }
                                if (Type == DeviceType.VELOTRON)
                                {
                                    m_GearPair = DLL.GetCurrentVTGear(PortNumber, Ver);
                                }
                            }
                            else
                            {
                                m_TrainerData = DLL.GetTrainerData(PortNumber, Ver);	// Keep this live, even through we are going to write fake data in there.

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
                        }
                        else if (ms_WaitAfter < 100)
                            ms_WaitAfter = 100;

                        if (OnUpdate != null)
                            OnUpdate(this, ms_SplitTime);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
                UpdateKeysDown();
            }

            void SetVelotronGears()
            {
                if (Type == DeviceType.VELOTRON && IsStarted)
                    DLL.setGear(PortNumber, Ver, m_FrontGearNumber, m_RearGearNumber);
            }

            void SetCalibrationValue(bool notify)
            {
                ms_Mux.WaitOne();
                try
                {
                    int orgv = DLL.GetFirmWareVersion(PortNumber);
                    int v = (orgv == 4095 ? 4543 : orgv);
                    bool c = (DLL.GetIsCalibrated(PortNumber, orgv) & 0xff) == 0 ? false : true;
                    int cnum = DLL.GetCalibration(PortNumber);
                   //Paul enabled feb 5
                    //Log.WriteLine (String.Format("Found {0} on port {1}, Version {2}", Type, PortNumber + 1, v) +
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
                if (notify)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                    {
                        if (RM1.OnCalibrationChanged != null)
                            RM1.OnCalibrationChanged(this, null);
                        Unit unit = Unit.TrainerUnit(this);
                        if (unit != null)
                        {
                            unit.Statistics.SetCalibrationValue(IsCalibrated, CalibrationValue, true);
                        }
                    });
                }
            }
            /// <summary>
            /// RUN FROM WORKER THREAD
            /// </summary>
            private void initDevice()
            {
                if (IsStarted)
                {
                    Log.WriteLine(String.Format("Stopping Trainer {0}", PortNumber + 1));
                    Stop(); // Stop the device if started.
                }
                Log.WriteLine(String.Format("Getting Trainer {0}", PortNumber + 1));
                DeviceType t;
                if (bFake)
                {
                    t = Type;
                }
                else
                {
                    t = GetRacerMateDeviceID(PortNumber);
                    if (t == DeviceType.COMPUTRAINER || t == DeviceType.VELOTRON)
                    {
                        SetCalibrationValue(true);
                        IsConnected = true;
                    }
                    //Feb 5 2012 Paul Smeulders add
                    else
                    {
                        IsConnected = false;  
                        Log.WriteLine(String.Format("{0} on port {1}", RM1.DeviceNames[(int)t], PortNumber + 1));
                    }
                    //Feb 5 2012 Paul Smeulders end of add
                }
                // This will mark things as ready.
                if (t == DeviceType.VELOTRON && m_VelotronData == null)
                    m_VelotronData = new VelotronData();
                Type = t;

                // OK Now before we start lets see if we can fill in the initial rider.
                foreach (TrainerUserConfigurable tc in RM1_Settings.ActiveTrainerList)
                {
                    if (PortNumber == tc.SavedSerialPortNum)
                    {
                        // We got it.
                        Rider = Riders.FindRiderByKey(tc.PreviousRiderKey);
                    }
                }
            }


            private static int orderByNum(Trainer a, Trainer b)
            {
                return a.PortNumber - b.PortNumber;
            }
            /// <summary>
            /// Should be on the main thread... add it to the proper location and raise the proper events.
            /// </summary>
            private void initDone()
            {
                ms_Mux.WaitOne(); // Make sure the sOnTimedEvent is not running
                try
                {
                    ms_InitList.Remove(this);
                }
                catch (Exception ex) { MutexException(ex); }
                ms_Mux.ReleaseMutex();
            }

            private void notifyInit(int cnt)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
                {
                    if (RM1.OnTrainerInitialized != null)
                    {
                        Log.WriteLine(String.Format("Dispatch - {0} left", cnt));
                        RM1.OnTrainerInitialized(this, cnt);
                    }
                });
            }
            //===============================================================
            // Used to control how many updates (durring normal operation
            // there will be.   Note if we are taking too much time then 
            // statistics will be kept
            //===============================================================
            private static double ms_TargetFPS = -1;
            private static Int64 ms_TargetTicks;
            public static double TargetFPS
            {
                get { return ms_TargetFPS; }
                set
                {
                    double v = value < 0.1 ? 0.1 : value;
                    if (ms_TargetFPS == v)
                        return;
                    ms_Mux.WaitOne();
                    try
                    {
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

            private static void ThreadLoop()
            {
                App.SetDefaultCulture();

                Trainer trainer;
                Int64 wait = 1, nextframe = 0, ticks, afterticks;
                for (; !ms_WaitEvent.WaitOne(new TimeSpan(wait)) && !ms_bShutdown; )
                {
                    if (nextframe <= 0)
                        nextframe = DateTime.Now.Ticks + ms_TargetTicks;

                    ms_Mux.WaitOne();
                    try
                    {
                        ticks = DateTime.Now.Ticks; // OK... Mark the time so we are as close as we can be when we enter into this thing
                        ms_SplitTime = (float)((double)(ticks - ms_LastTicks) * ConvertConst.HundredNanosecondToSecond);
                        ms_LastTicks = ticks;

                        ms_IntervalCount++;

                        // Any trainers need to be checked?
                        if (ms_InitList.Count > 0)
                        {
                            if (DateTime.Now >= ms_WaitScan)
                            {
                                trainer = ms_InitList.First();
                                ms_InitList.RemoveFirst();
                                while (ms_StartedList.Count > 0)
                                {
                                    Trainer t = ms_StartedList.First();
                                    t.Stop();
                                    if (!ms_StartList.Contains(t))
                                        ms_StartList.AddLast(t);
                                }
                                ms_StartedList.Clear();
                                trainer.initDevice();
                                if (trainer.Type == DeviceType.COMPUTRAINER || trainer.Type == DeviceType.VELOTRON)
                                {
                                    if (!ms_ActiveList.Contains(trainer))
                                    {
                                        ms_ActiveList.Add(trainer);
                                        ms_ActiveList.Sort(orderByNum);
                                        ms_ActiveListVersion++;
                                    }
                                    if (!ms_StartList.Contains(trainer))
                                        ms_StartList.AddLast(trainer);
                                }
                                trainer.notifyInit(ms_InitList.Count);
                            }
                            wait = (Int64)(ConvertConst.HundredNanosecondToMilliSecond * 1);
                        }
                        else if (ms_StartList.Count > 0)
                        {
                            foreach (Trainer t in ms_StartList)
                            {
                                t.Stop();	// Stop if already started.
                                t.Start();
                            }
                            ms_StartList.Clear();
                            wait = (Int64)(ConvertConst.HundredNanosecondToMilliSecond * 1);
                        }
                        else
                        {

                            // Update the trainers.
                            ms_WaitAfter = 0;
                            foreach (Trainer t in ms_ActiveList)
                                t.Update();
                            if (ms_WaitAfter > 0)
                                Thread.Sleep(100);
                            else
                                Thread.Sleep(0); // ECT - Just to make sure the UI stays responsive, probably not needed but it didn't seem to hurt.

                            Unit.IntervalUpdate(ms_LastTicks, ms_SplitTime);
                            if (OnInterval != null)
                                OnInterval(ms_LastTicks, ms_SplitTime);
                            ms_UpdateSplit += ms_SplitTime;
                            if (RM1.OnUpdate != null && ms_updateop == null)
                                ms_updateop = AppWin.Instance.Dispatcher.BeginInvoke(DispatcherPriority.Normal, ms_updatedelegate);
                            // OK here is the endticks...
                            afterticks = DateTime.Now.Ticks;
                            trackTime(afterticks);
                            wait = nextframe - afterticks;
                            if (wait < 1)
                            {
                                wait = 1;
                                nextframe = 0;
                            }
                            else
                                nextframe += ms_TargetTicks;
                        }
                    }
                    catch (Exception ex) { MutexException(ex); }
                    ms_Mux.ReleaseMutex();
                }
                ms_Mux.WaitOne();
                try
                {
                    foreach (Trainer t in ms_ActiveList)
                        t.Stop();
                }
                catch (Exception ex) { MutexException(ex); }
                ms_Mux.ReleaseMutex();

            }

            public static void Exit()
            {
                ms_bShutdown = true;
                ms_Thread.Join();
            }


            // ===================================================================================
            private const int ms_TrackTimeArrMax = 100;
            private static float[] ms_TrackTimeArr = new float[ms_TrackTimeArrMax];
            private static double ms_TrackTimeTotal = 0.0;
            private static int ms_TrackTimePos = 0;
            //private static int ms_Clip = 0;
            private static Int64 ms_TrackTicksLast = 0;
            private static void trackTime(Int64 ticks)
            {
                float tm = (float)((ticks - ms_TrackTicksLast) * ConvertConst.HundredNanosecondToSecond);
                ms_TrackTicksLast = ticks;
                ms_TrackTimeTotal -= ms_TrackTimeArr[ms_TrackTimePos];
                ms_TrackTimeArr[ms_TrackTimePos++] = tm;
                ms_TrackTimeTotal += tm;
                if (ms_TrackTimePos >= ms_TrackTimeArrMax)
                    ms_TrackTimePos = 0;
            }
            public double FPS { get { double t = ms_TrackTimeTotal / ms_TrackTimeArrMax; return t > 0 ? 1.0 / t : 0.0; } }


            /// <summary>
            /// Start up the initial stuffl
            /// </summary>
            static Trainer()
            {
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
            protected static Mutex ms_InitMux = new Mutex();
            protected static int ms_IntervalCount = 0;
            public static int IntervalCount { get { return ms_IntervalCount; } }

            //============
            protected static int ms_StartCount = 0;
            public static void IncreaseStartCount()
            {
            }
            public static void DecreaseStartCount()
            {
            }


            protected static Int64 ms_LastTicks = DateTime.Now.Ticks;
            protected static float ms_SplitTime;
            public static Int64 LastTicks { get { return ms_LastTicks; } }

            protected static System.Timers.Timer ms_Interval;

            //protected static DispatcherTimer ms_DispatchTimer;

            static TimeSpan ms_LockStatsMaxTime = new TimeSpan(0, 0, 10);
            public static void LockStats()
            {
                ms_Mux.WaitOne();
                /*
                if (!ms_Mux.WaitOne( ms_LockStatsMaxTime ))
                {
                    Debug.WriteLine("ERROR! Lockstats timed out ...something every wrong");
                    DLL.WriteLog();
                }
                 */
            }
            public static void UnlockStats() { ms_Mux.ReleaseMutex(); }

            private static double ms_UpdateSplit;
            private static DispatcherOperation ms_updateop;
            private delegate void _update();
            private static Delegate ms_updatedelegate = new _update(d_update);
            private static void d_update()
            {
                ms_Mux.WaitOne();
                try
                {
                    ms_updateop = null;
                    if (RM1.OnUpdate != null)
                        RM1.OnUpdate(ms_UpdateSplit);
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


            protected static void background_Init(object s, DoWorkEventArgs args)
            {
                RM1.Trainer trainer = args.Argument as RM1.Trainer;
                try
                {
                    trainer.initDevice();
                }
                catch
                {
                    Log.WriteLine(String.Format("Error in initializing trainer {0}", trainer.PortNumber + 1));
                }
                args.Result = trainer;
            }
            protected static void background_Done(object s, RunWorkerCompletedEventArgs args)
            {
                RM1.Trainer trainer = args.Result as Trainer;
                trainer.initDone();
            }

            //=================================================================================
            /// <summary>
            /// Part of the IPropertyChanged interface
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

            /*
            public void UpdateGradeOrLoad()
            {
                if (!IsStarted)
                    return;
                if (m_bERG)
                    DLL.SetErgModeLoad(PortNumber, Ver, CalibrationValue, m_Watts_Load);
                else
                {
                    if (m_Rider == null)
                        DLL.SetSlope(PortNumber, Ver, CalibrationValue, 0.0f, 0.0f, 100, m_Grade);
                    else
                    {
                        float bw = m_Rider.WeightBikeKGS;
                        float rw = m_Rider.WeightRiderKGS;
                        DLL.SetSlope(PortNumber, Ver, CalibrationValue, bw, rw, m_Rider.DragFactor, m_Grade);
                    }
                }
            }
            */

            static DateTime ms_WaitScan = new DateTime(0);
            public static void WaitToScan(int seconds)
            {
                ms_Mux.WaitOne();
                try
                {
                    ms_WaitScan = DateTime.Now + (new TimeSpan(0, 0, seconds));
                }
                catch (Exception ex) { MutexException(ex); }
                ms_Mux.ReleaseMutex();

            }
        }

        public static int StartFullScan()
        {
            Log.WriteLine("Starting full scan");
            string[] portnames = System.IO.Ports.SerialPort.GetPortNames();
            Regex r = new Regex("^COM([0-9]+)$", RegexOptions.IgnoreCase);
            // Make sure the other in the lit don't exit before we leave this.  Assures a final will 
            int cnt;
            ms_Mux.WaitOne();
            try
            {
                List<int> list = new List<int>();
                foreach (String n in portnames)
                {
                    Log.WriteLine(string.Format("Scanning {0}", n));
                    Match m = r.Match(n);
                    if (m.Success)
                    {
                        int port = Convert.ToInt32(m.Groups[1].Value) - 1;
                        Trainer trainer = Trainer.Get(port);
                        if (!ms_InitList.Contains(trainer))
                            ms_InitList.AddLast(trainer);
                    }
                }
                if (RM1_Settings.General.DemoDevice)
                    AddFake();
                cnt = ms_InitList.Count();
            }
            catch (Exception ex) { MutexException(ex); cnt = 0; }
            ms_Mux.ReleaseMutex();
            return cnt;
        }
        public static int StartSpecificScan(List<int> ports)
        {
            Trainer trainer;
            Log.WriteLine("Scanning ports " + ports.ToString());
            int cnt;
            ms_Mux.WaitOne();
            try
            {
                foreach (int i in ports)
                {
                    Log.WriteLine(string.Format("Scanning port {0}", i));

                    trainer = RM1.Trainer.Get(i);
                    if (!ms_InitList.Contains(trainer))
                        ms_InitList.AddLast(trainer);
                }
                if (RM1_Settings.General.DemoDevice)
                    AddFake();
                cnt = ms_InitList.Count();
            }
            catch (Exception ex) { MutexException(ex); cnt = 0; }
            ms_Mux.ReleaseMutex();
            return cnt;
        }
        public static int StartCompleteScan()
        {
            Log.WriteLine("Starting complete scan");
            int cnt;
            ms_Mux.WaitOne();
            try
            {
                for (int i = 0; i < 255; i++)
                {
                    Trainer trainer = RM1.Trainer.Get(i);
                    if (!ms_InitList.Contains(trainer))
                        ms_InitList.AddLast(trainer);
                }
                if (RM1_Settings.General.DemoDevice)
                    AddFake();
                cnt = ms_InitList.Count();
            }
            catch (Exception ex) { MutexException(ex); cnt = 0; }

            ms_Mux.ReleaseMutex();
            return cnt;
        }
        public static void ClearAllTrainers()
        {
            foreach (KeyValuePair<int, Trainer> entry in ms_Trainers)
            {
                entry.Value.Close();
            }
            ms_Trainers.Clear();
            while (TrainersInitializing > 0)
            {
                Thread.Sleep(100);
            }
        }

        public static int StartSettingsScan()
        {
            List<int> tlist = new List<int>();
            int ans;
            foreach (TrainerUserConfigurable tc in RM1_Settings.ActiveTrainerList)
                tlist.Add(tc.SavedSerialPortNum);
            if (tlist.Count() == 0)
                ans = RM1.StartFullScan();
            else
            {
                ans = RM1.StartSpecificScan(tlist);
                foreach (TrainerUserConfigurable tc in RM1_Settings.ActiveTrainerList)
                {
                    Trainer t = RM1.Trainer.Get(tc.SavedSerialPortNum);
                    t.ShouldBe = tc.DeviceType;
                    ms_HardwareListVersion = 0;	// Redo the hardware list next time it is requested.
                }
            }
            return ans;
        }


        public static void AddFake()
        {
            RM1.Trainer.Get(-1);
        }
    }
}
