#define DO_ALLOC

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using XmlStreamLib;
using System.Xml.Linq;
using System.Data;
using System.Threading;
using System.ComponentModel;
using System.IO.Compression;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Diagnostics;


namespace RacerMateOne
{

    static public class xstag
    {
        public const string TimeAcc = "T"; //"TimeAccumulator";
        public const string LapTime = "Lpt"; //"LapTime";
        public const string Lap = "Lp"; //"Lap";
        public const string Distance = "D"; //"Distance";
        public const string Lead = "Ld"; //"Lead";
        public const string Grade = "Grd"; //"Grade";
        public const string Wind = "Wnd"; //"Wind";
        public const string Speed = "Spd"; //"Speed";
        public const string Speed_Avg = "Spda"; //"Speed_Avg";
        public const string Speed_Max = "Spdm"; //"Speed_Max";
        public const string Watts = "Wat"; //"Watts";
        public const string Watts_Avg = "Wata"; //"Watts_Avg";
        public const string Watts_Max = "Watm"; //"Watts_Max";
        public const string Watts_Wkg = "Watw"; //"Watts_Wkg";
        public const string Watts_Load = "Watl"; //"Watts_Load";
        public const string HeartRate = "Hr"; //"HeartRate";
        public const string HeartRate_Avg = "Hra"; //"HeartRate_Avg";
        public const string HeartRate_Max = "Hrm"; //"HeartRate_Max";
        public const string Cadence = "Cdc"; //"Cadence";
        public const string Cadence_Avg = "Cda"; //"Cadence_Avg";
        public const string Cadence_Max = "Cdm"; //"Cadence_Max";
        public const string Calories = "Cal"; //"Calories";
        public const string PulsePower = "Pp"; //"PulsePower";
        public const string DragFactor = "Df"; //"DragFactor";
        public const string SS = "Ss"; //"SpinScan";
        public const string SSLeft = "Ssl"; //"SpinScanLeft";
        public const string SSRight = "Ssr"; //"SpinScanRight";
        public const string SSLeftSplit = "Sslp"; //"SpinScanLeftPowerSplit";
        public const string SSRightSplit = "Ssrp"; //"SpinScanRightPowerSplit";
        public const string SSLeftATA = "Sslt"; //"SpinScanLeftAveTourqeAngle";
        public const string SSRightATA = "Ssrt"; //"SpinScanRightAveTourqeAngle";
        public const string SSLeft_Avg = "Ssla"; //"SpinScanLeft_Avg";
        public const string SSRight_Avg = "Ssra"; //"SpinScanRight_Avg";
        public const string PercentAT = "Pat"; //"PercentAnareobicThreshhold";
        public const string FrontGear = "Fg"; //"FrontGear";
        public const string RearGear = "Rg"; //"RearGear";
        public const string GearInches = "Gi"; //"GearInches";
        public const string RawSpinScan = "Rss"; //"RawSpinScan";
        public const string CadenceTiming = "Cdct"; //"CadenceTiming";
        public const string TSS = "Tss"; //"TrainingStressScore";
        public const string IF = "If"; //"IntensityFactor";
        public const string NP = "Np"; //"NormalizedPower";
        public const string Bars = "Br"; //"Bars";
        public const string Bars_Shown = "Brs"; //"Bars_Shown";
        public const string AverageBars = "Abr"; //"AverageBars";
        public const string RiderName = "Rnm"; //"RiderName";
        public const string CourseScreenX = "Cs"; //"CourseScreenX";
        public const string Order = "Ord"; //"Order";
        public const string HardwareStatus = "Hs"; //"HardwareStatus";
        public const string StatFlags = "Sf"; //"StatFlags";
        public const string RRC = "Rc"; //"RollingCalibration";
        public const string TimeMS = "Tms"; //"TimeMilliseconds";
        public const string Gender = "Gdr"; //"Gender";
        public const string Age = "Age"; //"Age";
        public const string Height = "Ht"; //"Height";
        public const string Weight = "Wt"; //"Weight";
        public const string Upper_HeartRate = "Uhr"; //"Upper_HeartRate";
        public const string Lower_HeartRate = "Lur"; //"Lower_HeartRate";
        public const string CourseName = "Cnm"; //"CourseName";
        public const string CourseType = "Ctp"; //"CourseType";
        public const string Laps = "Lps"; //"Laps";
        public const string CourseLength = "Cl"; //"CourseLength";
        public const string RFDrag = "Rfd"; //"RFDrag";
        public const string RFMeas = "Rfm"; //"RFMeas";
        public const string Watts_Factor = "Watf"; //"Watts_Factor";
        public const string FTP = "Ftp"; //"FunctionalThreshholdPower";
        public const string PerfCount = "Cnt"; //"PerfCount";
        public const string Array = "_"; // "Array"

        public const string RMX = "RMX";
        public const string Header = "Header";
        public const string Course = "Course";
        public const string WattsType = "WattsType";
        public const string Val = "Val";
        public const string EndWatts = "EndWatts";
        public const string StartWatts = "StartWatts";
        public const string Minutes = "Minutes";
        public const string Count = "Count";
        public const string DistanceType = "DistanceType";
        public const string Length = "Length";
        public const string ThreeDType = "ThreeDType";
        public const string Rotation = "Rotation";
        public const string RCVType = "RCVType";
        public const string GPSData = "GPSData";
        public const string Looped = "Looped";
        public const string Modified = "Modified";
        public const string Reverse = "Reverse";
        public const string Mirror = "Mirror";
        public const string EndAt = "EndAt";
        public const string StartAt = "StartAt";
        public const string Type = "Type";
        public const string FileName = "FileName";
        public const string Description = "Description";
        public const string Name = "Name";
        public const string CourseInfo = "CourseInfo";
        public const string Info = "Info";
        public const string DataFlags = "DataFlags";
        public const string Data = "Data";
        public const string CompressType = "CompressType";
        public const string Copyright = "Copyright";
        public const string Comment = "Comment";
        public const string CreatorExe = "CreatorExe";
        public const string Date = "Date";
        public const string Version = "Version";
        public const string KeyFrame = "KeyFrame";
        public const string AnyType = "AnyType";
        public const string XUnits = "XUnits";
        public const string YUnits = "YUnits";
        public const string OriginalHash = "OriginalHash";
        public const string Hash = "Hash";
        public const string DataHash = "DataHash";

        public const string PerfData = "PerfData";
        public const string PerfRawData = "PerfRawData";
        public const string PerfInfo = "PerfInfo";
        public const string RMPHeader = "RMPHeader";

        public const string RM1X = "RM1X";
        public const string XTag = "XTAG";
    }
    static public class xtag
    {
        public const string TimeAcc = "TimeAccumulator";
        public const string LapTime = "LapTime";
        public const string Lap = "Lap";
        public const string Distance = "Distance";
        public const string Lead = "Lead";
        public const string Grade = "Grade";
        public const string Wind = "Wind";
        public const string Speed = "Speed";
        public const string Speed_Avg = "Speed_Avg";
        public const string Speed_Max = "Speed_Max";
        public const string Watts = "Watts";
        public const string Watts_Avg = "Watts_Avg";
        public const string Watts_Max = "Watts_Max";
        public const string Watts_Wkg = "Watts_Wkg";
        public const string Watts_Load = "Watts_Load";
        public const string HeartRate = "HeartRate";
        public const string HeartRate_Avg = "HeartRate_Avg";
        public const string HeartRate_Max = "HeartRate_Max";
        public const string Cadence = "Cadence";
        public const string Cadence_Avg = "Cadence_Avg";
        public const string Cadence_Max = "Cadence_Max";
        public const string Calories = "Calories";
        public const string PulsePower = "PulsePower";
        public const string DragFactor = "DragFactor";
        public const string SS = "SpinScan";
        public const string SSLeft = "SpinScanLeft";
        public const string SSRight = "SpinScanRight";
        public const string SSLeftSplit = "SpinScanLeftPowerSplit";
        public const string SSRightSplit = "SpinScanRightPowerSplit";
        public const string SSLeftATA = "SpinScanLeftAveTourqeAngle";
        public const string SSRightATA = "SpinScanRightAveTourqeAngle";
        public const string SSLeft_Avg = "SpinScanLeft_Avg";
        public const string SSRight_Avg = "SpinScanRight_Avg";
        public const string PercentAT = "PercentAnareobicThreshhold";
        public const string FrontGear = "FrontGear";
        public const string RearGear = "RearGear";
        public const string GearInches = "GearInches";
        public const string RawSpinScan = "RawSpinScan";
        public const string CadenceTiming = "CadenceTiming";
        public const string TSS = "TrainingStressScore";
        public const string IF = "IntensityFactor";
        public const string NP = "NormalizedPower";
        public const string Bars = "Bars";
        public const string Bars_Shown = "Bars_Shown";
        public const string AverageBars = "AverageBars";
        public const string RiderName = "RiderName";
        public const string CourseScreenX = "CourseScreenX";
        public const string Order = "Order";
        public const string HardwareStatus = "HardwareStatus";
        public const string StatFlags = "StatFlags";
        public const string RRC = "RollingCalibration";
        public const string TimeMS = "TimeMilliseconds";
        public const string Gender = "Gender";
        public const string Age = "Age";
        public const string Height = "Height";
        public const string Weight = "Weight";
        public const string Upper_HeartRate = "Upper_HeartRate";
        public const string Lower_HeartRate = "Lower_HeartRate";
        public const string CourseName = "CourseName";
        public const string CourseType = "CourseType";
        public const string Laps = "Laps";
        public const string CourseLength = "CourseLength";
        public const string RFDrag = "RFDrag";
        public const string RFMeas = "RFMeas";
        public const string Watts_Factor = "Watts_Factor";
        public const string FTP = "FunctionalThreshholdPower";
        public const string PerfCount = "PerfCount";
        public const string Array = "Array";
        public const string Disconnected = "Disconnected";
        public const string Calibration = "Calibration";
        public const string Drafting = "Drafting";
        public const string Flags1 = "Flags1";
        public const string Flags2 = "Flags2";

        public const string RMX = "RMX";
        public const string Header = "Header";
        public const string Course = "Course";
        public const string WattsType = "WattsType";
        public const string Val = "Val";
        public const string EndWatts = "EndWatts";
        public const string StartWatts = "StartWatts";
        public const string Minutes = "Minutes";
        public const string Count = "Count";
        public const string DistanceType = "DistanceType";
        public const string Length = "Length";
        public const string ThreeDType = "ThreeDType";
        public const string Rotation = "Rotation";
		public const string Smooth = "Smooth";
		public const string Divisions = "Divisions";
        public const string RCVType = "RCVType";
		public const string Segments = "Segments";
        public const string GPSData = "GPSData";
        public const string Looped = "Looped";
        public const string Modified = "Modified";
        public const string Reverse = "Reverse";
        public const string Mirror = "Mirror";
        public const string EndAt = "EndAt";
        public const string StartAt = "StartAt";
        public const string Type = "Type";
        public const string FileName = "FileName";
        public const string Description = "Description";
        public const string Name = "Name";
        public const string CourseInfo = "CourseInfo";
        public const string Info = "Info";
        public const string InfoExt1 = "InfoExt1";
        public const string DataFlags = "DataFlags";
        public const string Data = "Data";
        public const string CompressType = "CompressType";
        public const string Copyright = "Copyright";
        public const string Comment = "Comment";
        public const string CreatorExe = "CreatorExe";
        public const string Date = "Date";
        public const string Version = "Version";
        public const string KeyFrame = "KeyFrame";
        public const string AnyType = "AnyType";
        public const string XUnits = "XUnits";
        public const string YUnits = "YUnits";
        public const string StartYUnits = "StartYUnits";
        public const string EndYUnits = "EndYUnits";
        public const string OriginalHash = "OriginalHash";
        public const string HeaderHash = "HeaderHash";
        public const string CourseHash = "CourseHash";
        

        public const string PerfData = "PerfData";
        public const string PerfRawData = "PerfRawData";
        public const string PerfInfo = "PerfInfo";
        public const string RMPHeader = "RMPHeader";

        public const string RM1X = "RM1X"; // RacerMateOne header
        public const string RMPI = "RMPI"; // RacerMate Performance Info
        public const string RMPD = "RMPD"; // RacerMate Performance Data
        public const string RMPC = "RMPC"; // RacerMate Performance Course
        public const string RMPX = "RMPX"; // RacerMate Performance Info Extension
        public const string Tag = "Tag";
    }
    static public class xval
    {
        #region FourCC conversion methods

        public static string FromFourCC(int FourCC)
        {
            char[] chars = new char[4];
            chars[0] = (char)(FourCC & 0xFF);
            chars[1] = (char)((FourCC >> 8) & 0xFF);
            chars[2] = (char)((FourCC >> 16) & 0xFF);
            chars[3] = (char)((FourCC >> 24) & 0xFF);

            return new string(chars);
        }

        public static int ToFourCC(string FourCC)
        {
            if (FourCC.Length != 4)
            {
                throw new Exception("FourCC strings must be 4 characters long " + FourCC);
            }

            int result = ((int)FourCC[3]) << 24
                        | ((int)FourCC[2]) << 16
                        | ((int)FourCC[1]) << 8
                        | ((int)FourCC[0]);

            return result;
        }

        public static int ToFourCC(char[] FourCC)
        {
            if (FourCC.Length != 4)
            {
                throw new Exception("FourCC char arrays must be 4 characters long " + new string(FourCC));
            }

            int result = ((int)FourCC[3]) << 24
                        | ((int)FourCC[2]) << 16
                        | ((int)FourCC[1]) << 8
                        | ((int)FourCC[0]);

            return result;
        }

        public static int ToFourCC(char c0, char c1, char c2, char c3)
        {
            int result = ((int)c3) << 24
                        | ((int)c2) << 16
                        | ((int)c1) << 8
                        | ((int)c0);

            return result;
        }
        #endregion

        public static String SecondsToTimeStringLabel(double s)
        {
            double h = Math.Floor(s / 3600.0);
            s -= h * 3600;
            double m = Math.Floor(s / 60);
            s -= m * 60;
            return String.Format("{0:0}h{1:00}m{2:00.0}s", h, m, s);
        }

        public const string RacerMateOne = "RacerMateOne";
        public const string Copyright = "(c) 2011, RacerMateOne, Inc.";
        //public const string Version = "1.01";
        public const float Version = 1.02f;
        public const string None = "None";
        public static readonly int RMHeaderTag = ToFourCC(xtag.RM1X);
        public static readonly int RMPInfoTag = ToFourCC(xtag.RMPI);
        public static readonly int RMPDataTag = ToFourCC(xtag.RMPD);
        public static readonly int RMPCourseTag = ToFourCC(xtag.RMPC);
        public static readonly int RMPInfoExtTag = ToFourCC(xtag.RMPX);

    }

	public class PerfAverages
	{
		Statistics.Average Speed = new Statistics.Average();
		Statistics.Average Cadence = new Statistics.Average();
		Statistics.Average HeartRate = new Statistics.Average();
		Statistics.Average Watts = new Statistics.Average();
		double SSLeft_Acc;
		double SSRight_Acc;
		double Bars_TotalTime;
		double[] Bars_Acc = new double[24];
		double TimeAcc;

		public PerfAverages() { Reset(); }

		public void Reset()
		{
			Speed.Reset();
			Cadence.Reset();
			HeartRate.Reset();
			Watts.Reset();
			SSLeft_Acc = SSRight_Acc = Bars_TotalTime = 0;
			int i;
			for (i = 0; i < 24; i++)
				Bars_Acc[i] = 0;
		}
		public StatFlags UpdateAverages(ref PerfFrame.PerfData pd, double splittime)
		{
			StatFlags f = StatFlags.Zero;
			float d;
			int i;
			bool changed;
			if (pd.Speed > pd.Speed_Max) { pd.Speed_Max = pd.Speed; f |= StatFlags.Speed_Max; }
			if (pd.Cadence > pd.Cadence_Max) { pd.Cadence_Max = pd.Cadence; f |= StatFlags.Cadence_Max; }
			if (pd.HeartRate > pd.HeartRate_Max) { pd.HeartRate_Max = pd.HeartRate; f |= StatFlags.HeartRate_Max; }
			if (pd.Watts > pd.Watts_Max) { pd.Watts_Max = pd.Watts; f |= StatFlags.Watts_Max; }
			if (Speed.Add(pd.Speed, splittime)) { pd.Speed_Avg = Speed; f |= StatFlags.Speed_Avg; }
			if (Cadence.Add(pd.Cadence, splittime)) { pd.Cadence_Avg = Cadence; f |= StatFlags.Cadence_Avg; }
			if (HeartRate.Add(pd.HeartRate, splittime)) { pd.HeartRate_Avg = HeartRate; f |= StatFlags.HeartRate_Avg; }
            if (Watts.Add(pd.Watts, splittime))
            {
                pd.Watts_Avg = Watts;
                f |= StatFlags.Watts_Avg;
                // this would be useful but there is no Watts_Wkg_Avg, so better not do it
             //   f |= StatFlags.Watts_Wkg;
            }
			
			TimeAcc += splittime;
			SSLeft_Acc += pd.SSLeft * splittime;
			SSRight_Acc += pd.SSRight * splittime;
			d = (float)Math.Round(SSLeft_Acc / TimeAcc, 1);
			if (d != pd.SSLeft_Avg)
			{
				pd.SSLeft_Avg = d;
				f |= StatFlags.SSLeft_Avg;
			}
			d = (float)Math.Round(SSRight_Acc / TimeAcc, 1);
			if (d != pd.SSRight_Avg)
			{
				pd.SSRight_Avg = d;
				f |= StatFlags.SSRight_Avg;
			}
			if (((StatFlags)(pd.StatFlags) & StatFlags.Bars_Shown) != StatFlags.Zero)
			{
				Bars_TotalTime += splittime;
				if (Bars_TotalTime > 0.0)
				{
					for (i = 0, changed = false; i < RM1.BarCount; i++)
					{
						Bars_Acc[i] += pd.Bars[i] * splittime;
						if (!changed)
						{
							d = (float)Math.Round(Bars_Acc[i] / Bars_TotalTime, 2);
							if (d != pd.AverageBars[i])
							{
								changed = true;
								pd.AverageBars[i] = d;
							}
						}
						else
							pd.AverageBars[i] = (float)Math.Round(Bars_Acc[i] / Bars_TotalTime, 2);
					}
					if (changed)
						f |= StatFlags.Bars_Avg;
				}
			}
			return f;
		}
	}


    [Flags]
    public enum RawStatFlags : ulong
    {
        Zero = 0L,
        //Time = (1L << 0),		// Always changing

        Group1 = (1L << 1), // Changes very often
        //Distance = (1L << 3),
        //Watts = (1L << 10),
        //Watts_Load = (1L << 14),

        Group2 = (1L << 2),
        //Speed = (1L << 7),
        //Cadence = (1L << 18),

        Group3 = (1L << 3),
        //Grade = (1L << 5),

        Group4 = (1L << 4),
        //Wind = (1L << 6),

        Group5 = (1L << 5),
        //HeartRate = (1L << 15),

        Group6 = (1L << 6),
        //Calories = (1L << 21),

        Group7 = (1L << 7), // Changes very seldom
        //PulsePower = (1L << 22),
        //DragFactor = (1L << 23),
        //Calibration = (1L << 55), // added v1.02

        Group8 = (1L << 8),
        //FrontGear = (1L << 36),
        //RearGear = (1L << 37),
        //GearInches = (1L << 38),

        Group9 = (1L << 9),
        //TSS = (1L << 41),
        //IF = (1L << 42),
        //NP = (1L << 43),

        Group10 = (1L << 10),
        //SS = (1L << 24),
        //SSLeft = (1L << 25),
        //SSRight = (1L << 26),
        //SSLeftSplit = (1L << 27),
        //SSRightSplit = (1L << 28),
        //SSLeftATA = (1L << 29),
        //SSRightATA = (1L << 30),
        //Bars = (1L << 44),

        // Group 11, 12 used for boolean values
        Group11 = (1L << 11), // Seldom changes
        //Disconnected = (1L << 54), // added v1.02
        Group12 = (1L << 12), // changes often
        //Drafting = (1L << 39), // added v1.02

        Max = (1L << 13),
        Mask = Max - 1

        // Not Used right now
        //CadenceTiming = (1L << 40),
        //PercentAT = (1L << 35),
        //Bars_Shown = (1L << 45),
        //HardwareStatus = (1L << 51), // Something changed in the hardware connected to that unit.
    }

    public static class PFile
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct RMHeader
        {
            public Int32 Tag;                              // 4 bytes "RM1X"
            public UInt16 Version;                          // 2 bytes - version of this format / 1000
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] Date;                             // 24 bytes date created
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] CreatorExe;                       // 32 bytes program that created
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Copyright;                        // 32 bytes RacerMate copyright
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Comment;                          // 32 bytes description of this file
            public UInt16 CompressType;                     // 2 bytes different compression type
        }; // 54 bytes

        static public RMHeader RMHeaderCreate()
        {
            return new RMHeader
            {
                Tag = xval.RMHeaderTag,
                Date = new byte[24],
                CreatorExe = new byte[32],
                Copyright = new byte[32],
                Comment = new byte[32],
                CompressType = 0
            };
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct PerfInfo
        {
            public Int32 Tag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] RiderName;
            public Int16 Age;
            public Int16 Height;
            public Int16 Weight;
            public byte Gender;
            public byte HeartRate;
            public byte Upper_HeartRate;
            public byte Lower_HeartRate;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] CourseName;
            public Int16 CourseType;
            public double Distance;
            public float RFDrag;
            public float RFMeas;
            public float Watts_Factor;
            public float FTP;									// functional threshold power

            public Int32 PerfCount;	    // Number of performance points.
            public UInt64 TimeMS;
            public Int64 CourseOffset;
        };
        static public PerfInfo PerfInfoCreate()
        {
            return new PerfInfo
            {
                Tag = xval.RMPInfoTag,
                RiderName = new byte[80],
                CourseName = new byte[80]
            };
        }
        public struct PerfInfoExt1
        {
            public Int32 Tag;

            public UInt16 Mode;
            public Int16 RawCalibrationValue;
            public Int16 DragFactor;
            public UInt16 DeviceType;
            public UInt16 DeviceVersion;
            public UInt16 PowerAnT;
            public UInt16 PowerFTP;
            public byte HrAnT;
            public byte HrMin;
            public byte HrMax;
            public byte HrZone1;
            public byte HrZone2;
            public byte HrZone3;
            public byte HrZone4;
            public byte HrZone5;

        };
        static public PerfInfoExt1 PerfInfoExt1Create()
        {
            float min = 60;
            float max = 170;
            float avg = (min + max) / 2;
            return new PerfInfoExt1()
            {
                Tag = xval.RMPInfoExtTag,
                DragFactor = 0,
                DeviceType = 0,
                DeviceVersion = 0,
                PowerAnT = 0,
                PowerFTP = 0,
                Mode = 0,
                RawCalibrationValue = -200,
                HrMin = (byte)min,
                HrMax = (byte)max,
                HrZone1 = (byte)(avg * 0.50f),
                HrZone2 = (byte)(avg * 0.60f),
                HrZone3 = (byte)(avg * 0.70f),
                HrZone4 = (byte)(avg * 0.80f),
                HrZone5 = (byte)(avg * 0.90f),
                HrAnT = (byte)(avg * 0.90f)
            };
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct CourseHeader
        {
            public Int32 Tag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] Name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] FileName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] Description;
            public double StartAt;
            public double EndAt;
            public float Length;
            public int Laps;
            public int Count;
            public byte Type;
            public byte Attributes; //Mirror; Reverse; Looped; Modified; OutAndBack;
            public byte XUnits;
            public byte YUnits;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] OriginalHash;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] CourseHash;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] HeaderHash;
        };
        public struct CourseDataRCV
        {
            public double Length;
            public GPSData gd;
        };
        public struct CourseDataDist
        {
            public double Length;
            public float Grade;
            public float Wind;
        };
        public struct CourseData3D
        {
            public double Length;
            public float Grade;
            public float Wind;
            public float Rotation;
        };
		public struct CourseData3D_v2
		{
			public double Length;
			public float Grade;
			public float Wind;
			public float Rotation;
			public bool Smooth;
			public int Divisions;
		};
		public struct CourseDataWatts
        {
            public double Minutes;
            public int StartWatts;
            public int EndWatts;
        };
        public struct CourseDataAny
        {
            public double XUnits;
			public double StartYUnits;
            public double EndYUnits;
        };
        static public CourseHeader CourseHeaderCreate()
        {
            return new CourseHeader
            {
                Tag = xval.RMPCourseTag,
                Name = new byte[80],
                FileName = new byte[80],
                Description = new byte[128],
                CourseHash = new byte[32],
                HeaderHash = new byte[32],
                OriginalHash = new byte[32]
            };
        }
    }

		public class PerfFile  {

		class BackgroundTask
		{
			public static bool ms_Running;
			string m_startingPath;
			string m_destPath;
			public void Run(string startingPath,string destPath)
			{
				ms_Running = true;
				m_startingPath = startingPath;
				m_destPath = destPath;
				BackgroundWorker bw = new BackgroundWorker();
				bw.WorkerSupportsCancellation = true;
				bw.WorkerReportsProgress = true;
				bw.DoWork += new DoWorkEventHandler(bw_DoSaveWork);
				bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
				bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
				bw.RunWorkerAsync();
			}
			void bw_DoSaveWork(object sender, DoWorkEventArgs e)
			{
                App.SetDefaultCulture();

                BackgroundWorker bw = sender as BackgroundWorker;
				PerfFile.BatchConvertFolder(m_startingPath, m_destPath, bw );
				e.Result = true;
				if (bw.CancellationPending)
				{
					e.Cancel = true;
				}
			}
			private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
			{
			}

			// This event handler handles end of worker thread
			private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
			{
				ms_Running = false;
			}
			public void Cancel()
			{
			}

		}

		public static bool BatchConvertFolder(string startingPath)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.Description = "Select Folder to Batch Convert";
			dlg.SelectedPath = startingPath;
			DialogResult result = dlg.ShowDialog();
			if (result == DialogResult.OK)
				startingPath = dlg.SelectedPath;
			else
				return false;
			string destPath = startingPath + "\\..\\Converted";

			dlg.Description = "Select Folder to SaveTo";
			dlg.SelectedPath = destPath;
			result = dlg.ShowDialog();
			if (result == DialogResult.OK)
			{
				destPath = dlg.SelectedPath;
			}
			else
				return false;



			BackgroundTask b = new BackgroundTask();
			b.Run(startingPath,destPath);

			return true;
		}
        public static bool BatchConvertFolder(string startingPath, string destPath, BackgroundWorker bw ) // bw could be null 
        {

            int iterator = 0;

            List<string> dirList = new List<string>();
            List<string> fileList = new List<string>();

            dirList.Add(startingPath);
            string parentFolder = startingPath;

            // Every new folder found is added to the list to be searched. Continue until we have
            // found, and reported on, every folder or the calling thread wants us to stop
            while (iterator < dirList.Count)// && !(workerThreadInfo.StopRequested))
            {
                parentFolder = dirList[iterator];       // Each FileTreeEntry wants to know who its parent is
                try
                {
                    foreach (string dir in Directory.GetDirectories(dirList[iterator]))
                    {
                        Debug.WriteLine(dir);
                        dirList.Add(dir);
                        //dirList.Add(Path.Combine(parentFolder, dir));
                    }
                    foreach (string filename in Directory.GetFiles(dirList[iterator]))
                    {
                        FileInfo file = new FileInfo(filename);
                        string fname = Path.Combine(parentFolder, file.Name);
                        fileList.Add(fname);
					}
				}
                catch (UnauthorizedAccessException)
                {
                }
                catch (PathTooLongException)
                {
                }
	            iterator++;
			}
			double topercent = 100.0 / fileList.Count;
			int cnt = 0;
			foreach( string fname in fileList )
			{
				cnt++;
				string s = string.Format("{0:F1} Converting \"{1}\"", topercent * cnt, fname);
				if (bw != null)
					Log.WriteLine(s);
				else
					Debug.WriteLine(s);
                Course c = new Course();
                try
                {
                    if (c.Load(fname))
                    {
                        Debug.WriteLine("Loaded " + fname);
                        string name = Path.GetFileNameWithoutExtension(fname);
                        //string path = Path.GetDirectoryName(fname);
                        string srcfilename = Path.GetFileName(fname);
                        string ext = Path.GetExtension(fname);

                        /*
                        if (ext == ".rmp" || ext == ".rmc")
                        {
                            RMPHeader hdr = PerfFrame.LoadRMPHeader(fname);
                            string tempname = fname.Replace(startingPath, destPath);
                            string newfname = tempname.Replace(ext, ".rmc");
                            string path = Path.GetDirectoryName(newfname);
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            PerfFrame.SaveRMXCourse(newfname, c, hdr.Comment);
                            Debug.WriteLine("Converted " + newfname);
                        }
                        else
                         */
						{
                            string newext = ext.Replace(".", "_");
                            //string tempname =  path + "\\" + name + "_" + ext + ".rmp";

                            string tempname = fname.Replace(startingPath, destPath);
                            string newfname = tempname.Replace(ext, newext + ".rmc");

                            string path = Path.GetDirectoryName(newfname);
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string comment = "Converted from " + srcfilename;
                            PerfFrame.SaveRMXCourse(newfname, c, comment);
                            Debug.WriteLine("Converted " + newfname);
                        }
                    }
                }
                catch
                {
                    Debug.WriteLine("Failed to convert " + fname);
                }
				// There are two *acceptable* exceptions that we may see, but should not consider fatal
			}
			return true;
        }
        public static void Compress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Prevent compressing hidden and already compressed files.
                if ((File.GetAttributes(fi.FullName) & FileAttributes.Hidden)
                        != FileAttributes.Hidden & fi.Extension != ".gz")
                {
                    // Create the compressed file.
                    using (FileStream outFile = File.Create(fi.FullName + ".gz"))
                    {
                        using (GZipStream Compress = new GZipStream(outFile,
                                CompressionMode.Compress))
                        {
                            // Copy the source file into the compression stream.
                            byte[] buffer = new byte[4096];
                            int numRead;
                            while ((numRead = inFile.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                Compress.Write(buffer, 0, numRead);
                            }
                            Debug.WriteLine(String.Format("Compressed {0} from {1} to {2} bytes.",
                                fi.Name, fi.Length.ToString(), outFile.Length.ToString()));
                        }
                    }
                }
            }
        }

        public static void Decompress(FileInfo fi)
        {
            // Get the stream of the source file.
            using (FileStream inFile = fi.OpenRead())
            {
                // Get original file extension, for example "doc" from report.doc.gz.
                string curFile = fi.FullName;
                string origName = curFile.Remove(curFile.Length - fi.Extension.Length);

                //Create the decompressed file.
                using (FileStream outFile = File.Create(origName))
                {
                    using (GZipStream Decompress = new GZipStream(inFile,
                            CompressionMode.Decompress))
                    {
                        //Copy the decompression stream into the output file.
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = Decompress.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outFile.Write(buffer, 0, numRead);
                        }
                        Debug.WriteLine("Decompressed: {0}", fi.Name);

                    }
                }
            }
        }

        static public int TrimEndZero(ref byte[] t)
        {
            int i;
            for (i = t.Length - 1; i >= 0; i--)
                if (t[i] != 0)
                    break;
            return i + 1;
        }

        public class CRMHeader
        {
            public string CreatorExe = xval.RacerMateOne;       // 32 bytes program that created
            public DateTime Date = DateTime.Now;                // date created
            public float Version = xval.Version;                // version of this format
            public string Copyright = xval.Copyright;           // 32 bytes RacerMate copyright
            public string Comment = "";                         // 32 bytes description of this file
            public Int32 CompressType = 0;                      // different compression type

            public void Write(RawStream strOut)
            {
                byte[] tbuf;
                try
                {
                    PFile.RMHeader hdr = PFile.RMHeaderCreate();
                    hdr.Version = (UInt16)(0.5f + Version * 1000);

                    string datestr = string.Format("{0:s}", Date);
                    tbuf = new UTF8Encoding().GetBytes(datestr);
                    Array.Copy(tbuf, hdr.Date, Math.Min(24,tbuf.Length));

                    tbuf = new UTF8Encoding().GetBytes(CreatorExe);
                    Array.Copy(tbuf, hdr.CreatorExe, Math.Min(32, tbuf.Length));
                    tbuf = new UTF8Encoding().GetBytes(Comment);
                    Array.Copy(tbuf, hdr.Comment, Math.Min(32, tbuf.Length));
                    tbuf = new UTF8Encoding().GetBytes(Copyright);
                    Array.Copy(tbuf, hdr.Copyright, Math.Min(32, tbuf.Length));
                    hdr.CompressType = (UInt16)CompressType;

                    strOut.AddRawField(xtag.Header, hdr);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }

            public bool Read(RawStream str)
            {
                object obj;
                bool result = false;
                string tstr;
                try
                {
                    PFile.RMHeader hdr = PFile.RMHeaderCreate();
                    obj = str.GetNextStructureValue(hdr.GetType());
                    if (obj == null) return false;
                    hdr = (PFile.RMHeader)obj;

                    if (hdr.Tag != xval.RMHeaderTag)
                        return false;

                    Version = (float)(hdr.Version) / 1000;
                    if (Version < 1.0f)
                        return false;

                    tstr = new UTF8Encoding().GetString(hdr.Date, 0, TrimEndZero(ref hdr.Date));
                    Date = SafeDate.Parse(tstr);

                    CreatorExe = new UTF8Encoding().GetString(hdr.CreatorExe, 0, TrimEndZero(ref hdr.CreatorExe));
                    Copyright = new UTF8Encoding().GetString(hdr.Copyright, 0, TrimEndZero(ref hdr.Copyright));
                    Comment = new UTF8Encoding().GetString(hdr.Comment, 0, TrimEndZero(ref hdr.Comment));
                    result = true;
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    result = false;
                }
                return result;
            }
        }
        public class CPerfInfo
        {
            public float Version {set; get;} // not saved, just a working variable from CRMHeader

            public string RiderName {set; get;}
            public string Gender { set; get; }
            public Int32 Age { set; get; }
            public float Height { set; get; }
            public float Weight { set; get; }
            public float HeartRate { set; get; }
            public float Upper_HeartRate { set; get; }
            public float Lower_HeartRate { set; get; }

            public string CourseName { set; get; }
            public UInt32 CourseType { set; get; }
            public double Distance { set; get; }
            public float RFDrag { set; get; }
            public float RFMeas { set; get; }
            public float Watts_Factor { set; get; }
            public float FTP { set; get; }

            public Int32 PerfCount { set; get; }	    // Number of performance points.
            public UInt64 TimeMS { set; get; }       // Total time in Milliseconds
            public Int64 CourseOffset { set; get; }  // File Offset where the course is saved

            // added for header version - v1.02
            public UInt32 DeviceType {set; get;}
            public UInt32 DeviceVersion { set; get; }

            public Int32 PowerAnT { set; get; }
            public Int32 PowerFTP { set; get; }
            public Int32 DragFactor { set; get; }

            public AppModes Mode { set; get; }
            public string ModeStr
            {
				get { return Unit.GetAppModeString(Mode); }
            }
            public Int16 RawCalibrationValue { set; get; }
            public int HrAnT { set; get; }
            public int HrMin { set; get; }
            public int HrMax { set; get; }
            public int HrZone1 { set; get; }
            public int HrZone2 { set; get; }
            public int HrZone3 { set; get; }
            public int HrZone4 { set; get; }
            public int HrZone5 { set; get; }

            public void Write(RawStream strOut)
            {
                PFile.PerfInfo info = PFile.PerfInfoCreate();
                PFile.PerfInfoExt1 infoext = PFile.PerfInfoExt1Create();
                byte[] tbuf;
                try
                {
                    tbuf = new UTF8Encoding().GetBytes(RiderName);
                    Array.Copy(tbuf, info.RiderName, Math.Min(80, tbuf.Length));

                    tbuf = new UTF8Encoding().GetBytes(Gender);
                    if(tbuf.Length > 0) 
                        info.Gender = tbuf[0];

                    tbuf = new UTF8Encoding().GetBytes(CourseName);
                    Array.Copy(tbuf, info.CourseName, Math.Min(80, tbuf.Length));

                    info.Age = (Int16)Age;
                    info.Height = (Int16)Height;
                    info.Weight = (Int16)Weight;

                    info.HeartRate = (byte)(0.5f + HeartRate);
                    info.Upper_HeartRate = (byte)(0.5f + Upper_HeartRate);
                    info.Lower_HeartRate = (byte)(0.5f + Lower_HeartRate);
                    info.CourseType = (Int16)CourseType;

                    info.Distance = Distance;
                    info.RFDrag = RFDrag;
                    info.RFMeas = RFMeas;
                    info.Watts_Factor = Watts_Factor;
                    info.FTP = FTP;
                    info.PerfCount = PerfCount;
                    info.TimeMS = TimeMS;
                    info.CourseOffset = CourseOffset;

                    // These are added for header version - v1.02
                    infoext.DeviceType = (UInt16)DeviceType;
                    infoext.DeviceVersion = (UInt16)DeviceVersion;
                    infoext.Mode = (UInt16)Mode;
                    infoext.RawCalibrationValue = RawCalibrationValue;
                    infoext.DragFactor = (Int16)DragFactor;
                    infoext.PowerAnT = (UInt16)PowerAnT;
                    infoext.PowerFTP = (UInt16)PowerFTP;
                    infoext.HrAnT = (byte)HrAnT;
                    infoext.HrMin = (byte)HrMin;
                    infoext.HrMax = (byte)HrMax;
                    infoext.HrZone1 = (byte)HrZone1;
                    infoext.HrZone2 = (byte)HrZone2;
                    infoext.HrZone3 = (byte)HrZone3;
                    infoext.HrZone4 = (byte)HrZone4;
                    infoext.HrZone5 = (byte)HrZone5;

                    // the order of the following statements matters
                    strOut.AddRawField(xtag.Info, info);

                    // Added v1.02
                    strOut.AddRawField(xtag.InfoExt1, infoext);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }

            public bool Read(RawStream str, float version)
            {
                object obj;
                bool result = true;
                Version = version;
                try
                {
                    PFile.PerfInfo info = PFile.PerfInfoCreate();
                    PFile.PerfInfoExt1 infoext = PFile.PerfInfoExt1Create();

                    obj = str.GetNextStructureValue(info.GetType());
                    if (obj == null) return false;
                    info = (PFile.PerfInfo)obj;
                    if (info.Tag != xval.RMPInfoTag)
                        return false;

                    RiderName = new UTF8Encoding().GetString(info.RiderName, 0, TrimEndZero(ref info.RiderName));
                    CourseName = new UTF8Encoding().GetString(info.CourseName, 0, TrimEndZero(ref info.CourseName));
                    Gender = ((char)(info.Gender)).ToString();
                    Age = info.Age;
                    Height = info.Height;
                    Weight = info.Weight;
                    HeartRate = info.HeartRate;
                    Upper_HeartRate = info.Upper_HeartRate;
                    Lower_HeartRate = info.Lower_HeartRate;
                    Distance = info.Distance;
                    RFDrag = info.RFDrag;
                    RFMeas = info.RFMeas;
                    Watts_Factor = info.Watts_Factor;
                    FTP = info.FTP;
                    PerfCount = info.PerfCount;
                    TimeMS = info.TimeMS;
                    CourseOffset = info.CourseOffset;

                    // Added v1.02
                    if (Version > 1.01f)
                    {
                        obj = str.GetNextStructureValue(infoext.GetType());
                        if (obj == null) return false;
                        infoext = (PFile.PerfInfoExt1)obj;

                        if (infoext.Tag != xval.RMPInfoExtTag)
                            return false;

                        DeviceType = infoext.DeviceType;
                        DeviceVersion = infoext.DeviceVersion;
						try
						{
							Mode = (AppModes)infoext.Mode;
						}
						catch { Mode = AppModes.PowerTraining; } // If an error...this should catch everything.

                        RawCalibrationValue = infoext.RawCalibrationValue;
                        DragFactor = infoext.DragFactor;
                        PowerAnT = infoext.PowerAnT;
                        PowerFTP = infoext.PowerFTP;
                        HrAnT = infoext.HrAnT;
                        HrMin = infoext.HrMin;
                        HrMax = infoext.HrMax;
                        HrZone1 = infoext.HrZone1;
                        HrZone2 = infoext.HrZone2;
                        HrZone3 = infoext.HrZone3;
                        HrZone4 = infoext.HrZone4;
                        HrZone5 = infoext.HrZone5;
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    result = false;
                }
                return result;
            }
        };

        public class CPerfRawData
        {
            public float Version { set; get; }

            public UInt16 GroupFlags { set; get; }

            public UInt32 TimeMS { set; get; }
            public float Distance { set; get; }
            public float Speed { set; get; }
            public float Wind { set; get; }
            public UInt16 Watts { set; get; }
            public Int16 Grade { set; get; }

            public byte HeartRate { set; get; }
            public byte Cadence { set; get; }
            public byte PulsePower { set; get; }
            public byte DragFactor { set; get; }
            public byte SSLeftATA { set; get; }
            public byte SSRightATA { set; get; }
            public byte SSRight { set; get; }
            public byte SSLeft { set; get; }
            public byte SSLeftSplit { set; get; }
            public byte SSRightSplit { set; get; }
            public byte SS { set; get; }
            //public byte RawSpinScan { set; get; }
            public Int16 FrontGear { set; get; }
            public Int16 RearGear { set; get; }
            public Int16 GearInches { set; get; }
            public UInt16 Calories { set; get; }
            public float Watts_Load { set; get; }
            //public float LeftPower { set; get; }
            //public float RightPower { set; get; }
            public float PercentAT { set; get; }
            //public float CadenceTiming { set; get; }
            public float TSS { set; get; }
            public float IF { set; get; }
            public float NP { set; get; }
            public byte[] Bars { set; get; }

            // Added v1.02
            [Flags]
            enum bflags : ushort
            {
                fDrafting = (1 << 0),
                fDisconnected = (1 << 0)
            }
            private bflags m_flags1; // Disconnected
            private bflags m_flags2; // Drafting

            public bool Disconnected
            {
                get
                {
                    return ((m_flags1 & bflags.fDisconnected) != 0);
                }
                set
                {
                    if(value)
                        m_flags1 |= bflags.fDisconnected;
                    else
                        m_flags1 &= ~(bflags.fDisconnected);
                }
            }
            public bool Drafting
            {
                get
                {
                    return ((m_flags2 & bflags.fDrafting) != 0);
                }
                set
                {
                    if (value)
                        m_flags2 |= bflags.fDrafting;
                    else
                        m_flags2 &= ~(bflags.fDrafting);
                }
            }
            public Int16 RawCalibrationValue;
            //public bool HardwareStatus;

            public CPerfRawData()
            {
                Bars = new byte[24];
                Reset();
            }

            public void Reset()
            {
                GroupFlags = 0;

                TimeMS = 0;
                Distance = 0;
                Speed = 0;
                Wind = 0;
                Watts = 0;
                Grade = 0;
                HeartRate = 0;
                Cadence = 0;
                PulsePower = 0;
                DragFactor = 0;
                SSLeftATA = 0;
                SSRightATA = 0;
                SSRight = 0;
                SSLeft = 0;
                SSLeftSplit = 0;
                SSRightSplit = 0;
                SS = 0;
                //RawSpinScan = 0;
                FrontGear = 0;
                RearGear = 0;
                GearInches = 0;
                Calories = 0;
                Watts_Load = 0;
                //LeftPower = 0;
                //RightPower = 0;
                PercentAT = 0;
                //public float CadenceTiming = 0;
                TSS = 0;
                IF = 0;
                NP = 0;
                //Drafting = 0; // Need to add drafting.
                for (int i = 0; i < 24; i++)
                    Bars[i] = 0;

                Drafting = false;
                Disconnected = false;
                RawCalibrationValue = -200;
            }

            // Convert form PerfData
            public void Convert(ref PerfFrame.PerfData pd, StatFlags changedflags)
            {
                RawStatFlags changed = RawStatFlags.Zero;

                // Time should always change
                TimeMS = (UInt32)(pd.TimeAcc * 1000);
                if ((changedflags & gVar.AllGroups) != StatFlags.Zero)
                {
                    // Change the values that actually changed
                    if ((changedflags & gVar.Group1) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group1;
                        if ((changedflags & StatFlags.Distance) != StatFlags.Zero) { Distance = (float)(pd.Distance * ConvertConst.MetersToKilometers); }
                        if ((changedflags & StatFlags.Watts) != StatFlags.Zero)
                        {
                            int watts = (int)(0.5f + pd.Watts);
                            Watts = (UInt16)(watts > 0xffff ? 0xffff : watts);
                        }
                        if ((changedflags & StatFlags.Watts_Load) != StatFlags.Zero) { Watts_Load = pd.Watts_Load; }
                    }
                    if ((changedflags & gVar.Group2) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group2;
                        if ((changedflags & StatFlags.Speed) != StatFlags.Zero) { Speed = (float)(pd.Speed * ConvertConst.MetersPerSecondToKPH); }
                        if ((changedflags & StatFlags.Cadence) != StatFlags.Zero)
                        {
                            int rpm = (int)(0.5f + pd.Cadence);
                            Cadence = (byte)(rpm > 255 ? 255 : rpm);
                        }
                    }
                    if ((changedflags & gVar.Group3) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group3;
                        if ((changedflags & StatFlags.Grade) != StatFlags.Zero) { Grade = (Int16)(0.5f + pd.Grade * 100); }
                    }
                    if ((changedflags & gVar.Group4) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group4;
                        if ((changedflags & StatFlags.Wind) != StatFlags.Zero) { Wind = (float)(pd.Wind * ConvertConst.MetersPerSecondToKPH); }
                    }
                    if ((changedflags & gVar.Group5) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group5;
                        if ((changedflags & StatFlags.HeartRate) != StatFlags.Zero)
                        {
                            int pulse = (int)(0.5f + pd.HeartRate);
                            HeartRate = (byte)(pulse > 255 ? 255 : pulse);
                        }
                    }
                    if ((changedflags & gVar.Group6) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group6;
                        if ((changedflags & StatFlags.Calories) != StatFlags.Zero) { Calories = (UInt16)(0.5f + pd.Calories); }
                    }
                    if ((changedflags & gVar.Group7) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group7;
                        if ((changedflags & StatFlags.PulsePower) != StatFlags.Zero) { PulsePower = (byte)(0.5f + pd.PulsePower); }
                        if ((changedflags & StatFlags.DragFactor) != StatFlags.Zero) { DragFactor = (byte)(0.5f + pd.DragFactor); }

                        // Added v1.02
                        if ((changedflags & StatFlags.Calibration) != StatFlags.Zero) { RawCalibrationValue = pd.RawCalibrationValue; }
                    }
                    if ((changedflags & gVar.Group8) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group8;
                        if ((changedflags & StatFlags.FrontGear) != StatFlags.Zero) { FrontGear = (Int16)pd.FrontGear; }
                        if ((changedflags & StatFlags.RearGear) != StatFlags.Zero) { RearGear = (Int16)pd.RearGear; }
                        if ((changedflags & StatFlags.GearInches) != StatFlags.Zero) { GearInches = (Int16)pd.GearInches; }
                    }
                    if ((changedflags & gVar.Group9) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group9;
                        if ((changedflags & StatFlags.TSS) != StatFlags.Zero) { TSS = pd.TSS; }
                        if ((changedflags & StatFlags.IF) != StatFlags.Zero) { IF = pd.IF; }
                        if ((changedflags & StatFlags.NP) != StatFlags.Zero) { NP = pd.NP; }
                    }
                    if ((changedflags & gVar.Group10) != StatFlags.Zero)
                    {
                        changed |= RawStatFlags.Group10;
                        if ((changedflags & StatFlags.SS) != StatFlags.Zero) { SS = (byte)(0.5f + pd.SS); }
                        if ((changedflags & StatFlags.SSLeftATA) != StatFlags.Zero) { SSLeftATA = (byte)(0.5f + pd.SSLeftATA); }
                        if ((changedflags & StatFlags.SSRightATA) != StatFlags.Zero) { SSRightATA = (byte)(0.5f + pd.SSRightATA); }
                        if ((changedflags & StatFlags.SSRight) != StatFlags.Zero) { SSRight = (byte)(0.5f + pd.SSRight); }
                        if ((changedflags & StatFlags.SSLeft) != StatFlags.Zero) { SSLeft = (byte)(0.5f + pd.SSLeft); }
                        if ((changedflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { SSLeftSplit = (byte)(0.5f + pd.SSLeftSplit); }
                        if ((changedflags & StatFlags.SSRightSplit) != StatFlags.Zero) { SSRightSplit = (byte)(0.5f + pd.SSRightSplit); }
                        if ((changedflags & StatFlags.Bars) != StatFlags.Zero)
                        {
                            for (int i = 0; i < 24; i++)
                            {
                                float f = 255.0f * (pd.Bars[i] / 20.0f);							// 255 = 20 lbs
                                int kk = (int)((f > 0) ? (0.5f + f) : (-0.5f + f)); //iround(f)
                                if (kk > 255)
                                {
                                    kk = 255;
                                }
                                else if (kk < 0)
                                {
                                    kk = 0;
                                }
                                Bars[i] = (byte)kk;			// stored as 0-20 lbs, spin[] is unsigned char
                            }
                        }
                    }

                    // Added v1.02
                    if ((changedflags & gVar.Group11) != StatFlags.Zero)
                    {
                        if ((changedflags & StatFlags.Disconnected) != StatFlags.Zero) { Disconnected = pd.Disconnected; }
                    }
                    // Added v1.02
                    if ((changedflags & gVar.Group12) != StatFlags.Zero)
                    {
                        if ((changedflags & StatFlags.Drafting) != StatFlags.Zero) { Drafting = pd.Drafting; }
                    }

                }
                GroupFlags = (UInt16)changed;
            }

            public void ToPerfData(ref PerfFrame.PerfData npd)
            {
                RawStatFlags changedflags = (RawStatFlags)GroupFlags;
                StatFlags changed = StatFlags.Zero;

                npd.TimeMS = TimeMS;
                npd.TimeAcc = 0.001 * TimeMS;
                changed |= StatFlags.Time;

                if ((changedflags & RawStatFlags.Mask) != RawStatFlags.Zero)
                {
                    // Change the values that actually changed
                    if ((changedflags & RawStatFlags.Group1) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group1;
                        npd.Distance = (double)Distance * ConvertConst.KilometersToMeters;
                        npd.Watts = Watts;
                        npd.Watts_Load = Watts_Load;
                        npd.Watts_Wkg = Watts / 4;
                    }

                    if ((changedflags & RawStatFlags.Group2) != RawStatFlags.Zero)  {
                        changed |= gVar.Group2;
                        npd.Speed = (float)(Speed * ConvertConst.KPHToMetersPerSecond);
                        npd.Cadence = Cadence;
                    }

                    if ((changedflags & RawStatFlags.Group3) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group3;
                        npd.Grade = (float)Grade / 100.0f;
                    }
                    if ((changedflags & RawStatFlags.Group4) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group4;
                        npd.Wind = (float)(Wind * ConvertConst.KPHToMetersPerSecond);
                    }
                    if ((changedflags & RawStatFlags.Group5) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group5;
                        npd.HeartRate = HeartRate;
                    }
                    if ((changedflags & RawStatFlags.Group6) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group6;
                        npd.Calories = Calories;
                    }
                    if ((changedflags & RawStatFlags.Group7) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group7;
                        npd.PulsePower = PulsePower;
                        npd.DragFactor = DragFactor;
                        npd.RawCalibrationValue = RawCalibrationValue; // added v1.02
                    }
                    if ((changedflags & RawStatFlags.Group8) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group8;
                        npd.FrontGear = FrontGear;
                        npd.RearGear = RearGear;
                        npd.GearInches = GearInches;
                    }
                    if ((changedflags & RawStatFlags.Group9) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group9;
                        npd.TSS = TSS;
                        npd.IF = IF;
                        npd.NP = NP;
                    }
					bool dobaravg = false;
                    if ((changedflags & RawStatFlags.Group10) != RawStatFlags.Zero)
                    {
                        changed |= gVar.Group10;
                        npd.SS = SS;
                        npd.SSLeftATA = SSLeftATA;
                        npd.SSRightATA = SSRightATA;
                        npd.SSRight = SSRight;			// right spinscan
                        npd.SSLeft = SSLeft;				// left spinscan
                        npd.SSLeftSplit = SSLeftSplit;		// left split
                        npd.SSRightSplit = SSRightSplit;		// right split.
						float val;
                        for (int i = 0; i < 24; i++)
                        {
                            val = npd.Bars[i] = ((float)Bars[i] / 255.0f) * 20.0f;
							if (!dobaravg && val > 0.02)
								dobaravg = true;
                        }
                    }
					if (dobaravg)
						changed |= StatFlags.Bars_Shown;
					else
						changed &= ~StatFlags.Bars_Shown;

                    // Added v1.02
                    if ((changedflags & RawStatFlags.Group11) != RawStatFlags.Zero)
                    {
                        npd.Disconnected = Disconnected; // added v1.02
                    }
                    if ((changedflags & RawStatFlags.Group12) != RawStatFlags.Zero)
                    {
                        npd.Drafting = Drafting; // added v1.02
                    }

                }

                npd.StatFlags = (UInt64)changed; // (UInt64)(gVar.PerformanceFlags & changed);
            }

				public void Write(RawStream str)  {
                RawStatFlags changedflags = (RawStatFlags)GroupFlags;

						try  {
                    str.AddRawField(xtag.TimeMS, TimeMS);
                    str.AddRawField("GroupFlags", GroupFlags);

                    if ((changedflags & RawStatFlags.Mask) != RawStatFlags.Zero)
                    {
                        // Change the values that actually changed
                        if ((changedflags & RawStatFlags.Group1) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.Distance, Distance);
                            str.AddRawField(xtag.Watts, Watts);
                            str.AddRawField(xtag.Watts_Load, Watts_Load);
                        }
                        if ((changedflags & RawStatFlags.Group2) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.Speed, Speed);
                            str.AddRawField(xtag.Cadence, Cadence);
                        }
                        if ((changedflags & RawStatFlags.Group3) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.Grade, Grade);
                        }
                        if ((changedflags & RawStatFlags.Group4) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.Wind, Wind);
                        }
                        if ((changedflags & RawStatFlags.Group5) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.HeartRate, HeartRate);
                        }
                        if ((changedflags & RawStatFlags.Group6) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.Calories, Calories);
                        }
                        if ((changedflags & RawStatFlags.Group7) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.PulsePower, PulsePower);
                            str.AddRawField(xtag.DragFactor, DragFactor);
                            str.AddRawField(xtag.Calibration, RawCalibrationValue); // added v1.02
                        }
                        if ((changedflags & RawStatFlags.Group8) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.FrontGear, FrontGear);
                            str.AddRawField(xtag.RearGear, RearGear);
                            str.AddRawField(xtag.GearInches, GearInches);
                        }
                        if ((changedflags & RawStatFlags.Group9) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.TSS, TSS);
                            str.AddRawField(xtag.IF, IF);
                            str.AddRawField(xtag.NP, NP);
                        }
                        if ((changedflags & RawStatFlags.Group10) != RawStatFlags.Zero)
                        {
                            str.AddRawField(xtag.SS, SS);
                            str.AddRawField(xtag.SSLeftATA, SSLeftATA);
                            str.AddRawField(xtag.SSRightATA, SSRightATA);
                            str.AddRawField(xtag.SSRight, SSRight);
                            str.AddRawField(xtag.SSLeft, SSLeft);
                            str.AddRawField(xtag.SSLeftSplit, SSLeftSplit);
                            str.AddRawField(xtag.SSRightSplit, SSRightSplit);
                            for (int i = 0; i < 24; i++)
                                str.AddRawField(xtag.Bars, Bars[i]);
                        }

                        // added v1.02 - for boolean 
                        if ((changedflags & RawStatFlags.Group11) != RawStatFlags.Zero)
                        {
                            // Disconnected;
                            str.AddRawField(xtag.Flags1, m_flags1); // added v1.02
                        }
                        // added v1.02 - for boolean 
                        if ((changedflags & RawStatFlags.Group12) != RawStatFlags.Zero)
                        {
                            //Drafting;
                            str.AddRawField(xtag.Flags2, m_flags2); // added v1.02
                        }

                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }

				}				// Write(RawStream str)

            public bool Read(RawStream str, float version)
            {

                RawStatFlags changedflags = RawStatFlags.Zero;
                object obj;
                bool result = true;
                Version = version;

                try
                {
                    obj = str.GetNextStructureValue(TimeMS.GetType());
                    if (obj != null)
                        TimeMS = (UInt32)obj;
                    obj = str.GetNextStructureValue(GroupFlags.GetType());
                    if (obj != null)
                    {
                        GroupFlags = (UInt16)obj;
                        changedflags = (RawStatFlags)GroupFlags;
                    }

                    if ((changedflags & RawStatFlags.Mask) != RawStatFlags.Zero)
                    {
                        // Change the values that actually changed
                        if ((changedflags & RawStatFlags.Group1) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(Distance.GetType());
                            if (obj != null)
                                Distance = (float)obj;
                            obj = str.GetNextStructureValue(Watts.GetType());
                            if (obj != null)
                                Watts = (UInt16)obj;
                            obj = str.GetNextStructureValue(Watts_Load.GetType());
                            if (obj != null)
                                Watts_Load = (float)obj;
                        }
                        if ((changedflags & RawStatFlags.Group2) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(Speed.GetType());
                            if (obj != null)
                                Speed = (float)obj;
                            obj = str.GetNextStructureValue(Cadence.GetType());
                            if (obj != null)
                                Cadence = (byte)obj;
                        }
                        if ((changedflags & RawStatFlags.Group3) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(Grade.GetType());
                            if (obj != null)
                                Grade = (Int16)obj;
                        }
                        if ((changedflags & RawStatFlags.Group4) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(Wind.GetType());
                            if (obj != null)
                                Wind = (float)obj;
                        }
                        if ((changedflags & RawStatFlags.Group5) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(HeartRate.GetType());
                            if (obj != null)
                                HeartRate = (byte)obj;
                        }
                        if ((changedflags & RawStatFlags.Group6) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(Calories.GetType());
                            if (obj != null)
                                Calories = (UInt16)obj;
                        }
                        if ((changedflags & RawStatFlags.Group7) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(PulsePower.GetType());
                            if (obj != null)
                                PulsePower = (byte)obj;
                            obj = str.GetNextStructureValue(DragFactor.GetType());
                            if (obj != null)
                                DragFactor = (byte)obj;

                            // Added v1.02
                            if (Version > 1.01f)
                            {
                                obj = str.GetNextStructureValue(RawCalibrationValue.GetType());
                                if (obj != null)
                                    RawCalibrationValue = (Int16)obj;
                            }
                        }
                        if ((changedflags & RawStatFlags.Group8) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(FrontGear.GetType());
                            if (obj != null)
                                FrontGear = (Int16)obj;
                            obj = str.GetNextStructureValue(RearGear.GetType());
                            if (obj != null)
                                RearGear = (Int16)obj;
                            obj = str.GetNextStructureValue(GearInches.GetType());
                            if (obj != null)
                                GearInches = (Int16)obj;
                        }
                        if ((changedflags & RawStatFlags.Group9) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(TSS.GetType());
                            if (obj != null)
                                TSS = (float)obj;
                            obj = str.GetNextStructureValue(IF.GetType());
                            if (obj != null)
                                IF = (float)obj;
                            obj = str.GetNextStructureValue(NP.GetType());
                            if (obj != null)
                                NP = (float)obj;
                        }
                        if ((changedflags & RawStatFlags.Group10) != RawStatFlags.Zero)
                        {
                            obj = str.GetNextStructureValue(SS.GetType());
                            if (obj != null)
                                SS = (byte)obj;
                            obj = str.GetNextStructureValue(SSLeftATA.GetType());
                            if (obj != null)
                                SSLeftATA = (byte)obj;
                            obj = str.GetNextStructureValue(SSRightATA.GetType());
                            if (obj != null)
                                SSRightATA = (byte)obj;
                            obj = str.GetNextStructureValue(SSRight.GetType());
                            if (obj != null)
                                SSRight = (byte)obj;
                            obj = str.GetNextStructureValue(SSLeft.GetType());
                            if (obj != null)
                                SSLeft = (byte)obj;
                            obj = str.GetNextStructureValue(SSLeftSplit.GetType());
                            if (obj != null)
                                SSLeftSplit = (byte)obj;
                            obj = str.GetNextStructureValue(SSRightSplit.GetType());
                            if (obj != null)
                                SSRightSplit = (byte)obj;
                            for (int i = 0; i < 24; i++)
                            {
                                obj = str.GetNextStructureValue(Bars[i].GetType());
                                if (obj != null)
                                    Bars[i] = (byte)obj;
                            }
                        }

                        // Added v1.02
                        if (Version > 1.01f)
                        {
                            if ((changedflags & RawStatFlags.Group11) != RawStatFlags.Zero)
                            {
                                // Disconnected
                                obj = str.GetNextStructureValue(m_flags1.GetType());
                                if (obj != null)
                                    m_flags1 = (bflags)obj;

                            }
                            if ((changedflags & RawStatFlags.Group12) != RawStatFlags.Zero)
                            {
                                // Drafting
                                obj = str.GetNextStructureValue(m_flags2.GetType());
                                if (obj != null)
                                    m_flags2 = (bflags)obj;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    result = false;
                }
                return result;
            }
        };
        public class CCourse
        {
            public float Version;
            public Course cCourse = new Course();

            public void Write(RawStream strOut)
            {
                Write(strOut, false);
            }

            public void Write(RawStream strOut, bool headeronly)
            {
                byte[] tbuf;
                try
                {
                    PFile.CourseHeader hdr = PFile.CourseHeaderCreate();

                    CourseAttributes t =
                        (cCourse.Looped ? CourseAttributes.Looped : CourseAttributes.Zero) |
                        (cCourse.Mirror ? CourseAttributes.Mirror : CourseAttributes.Zero) |
                        (cCourse.Modified ? CourseAttributes.Modified : CourseAttributes.Zero) |
                        //(course.OutAndBack ? CourseAttributes.OutAndBack : CourseAttributes.Zero) |
                        (cCourse.Reverse ? CourseAttributes.Reverse : CourseAttributes.Zero);
                    hdr.Attributes = (byte)t;


                    tbuf = new UTF8Encoding().GetBytes(cCourse.Name);
                    Array.Copy(tbuf, hdr.Name, Math.Min(80, tbuf.Length));
                    tbuf = new UTF8Encoding().GetBytes(cCourse.Description);
                    Array.Copy(tbuf, hdr.Description, Math.Min(128, tbuf.Length));
                    tbuf = new UTF8Encoding().GetBytes(cCourse.FileName);
                    Array.Copy(tbuf, hdr.FileName, Math.Min(80, tbuf.Length));

                    hdr.Length = float.Parse(cCourse.StringLength);
                    hdr.Laps = cCourse.Laps;
                    hdr.StartAt = cCourse.StartAt;
                    hdr.EndAt = cCourse.EndAt;
                    hdr.Type = (byte)cCourse.Type;

                    hdr.XUnits = (byte)cCourse.XUnits;
                    hdr.YUnits = (byte)cCourse.YUnits;
                    tbuf = new UTF8Encoding().GetBytes(cCourse.OriginalHash);
                    Array.Copy(tbuf, hdr.OriginalHash, Math.Min(32, tbuf.Length));

                    tbuf = new UTF8Encoding().GetBytes(cCourse.CourseHash);
                    Array.Copy(tbuf, hdr.CourseHash, Math.Min(32, tbuf.Length));
                    tbuf = new UTF8Encoding().GetBytes(cCourse.HeaderHash);
                    Array.Copy(tbuf, hdr.HeaderHash, Math.Min(32, tbuf.Length));


                    hdr.Count = cCourse.Segments.Count;
                    strOut.AddRawField(xtag.Header, hdr);

                    if (headeronly)
                        return; 

                    CourseType ct = cCourse.Type;
                    if ((ct & CourseType.Video) == CourseType.Video)
                        ct = CourseType.Video;
                    else if ((ct & CourseType.ThreeD) == CourseType.ThreeD)
                        ct = CourseType.ThreeD;
                    switch (ct)
					{
						case CourseType.Video:
							{
								PFile.CourseDataRCV data = new PFile.CourseDataRCV();
								for (LinkedListNode<Course.Segment> n = cCourse.Segments.First; n != null; n = n.Next)
								{
									Course.GPSSegment val = (Course.GPSSegment)n.Value;
									data.gd = val.GPSData;
									data.Length = val.Length;
									strOut.AddRawField(xtag.Data, data);
								}
							}
							break;
						case CourseType.ThreeD:
							{
								PFile.CourseData3D_v2 data = new PFile.CourseData3D_v2();
								for (LinkedListNode<Course.Segment> n = cCourse.Segments.First; n != null; n = n.Next)
								{
									Course.PysicalSegment val = (Course.PysicalSegment)n.Value;
									Course.SmoothSegment ss = val as Course.SmoothSegment;

									data.Length = val.Length;
									data.Grade = val.Grade;
									data.Wind = val.Wind;
									data.Rotation = val.Rotation;
									data.Smooth = ss != null;
									data.Divisions = ss != null ? ss.Divisions : 1;

									strOut.AddRawField(xtag.Data, data);
								}
							}
							break;
						default:
							{
								PFile.CourseDataAny data = new PFile.CourseDataAny();
								for (LinkedListNode<Course.Segment> n = cCourse.Segments.First; n != null; n = n.Next)
								{
									data.XUnits = n.Value.Length;
									if (cCourse.YUnits == CourseYUnits.Grade)
									{
										Course.PysicalSegment seg = n.Value as Course.PysicalSegment;
										data.StartYUnits = seg.Change;
										data.EndYUnits = seg.Wind;
									}
									else
									{
										Course.WattsSegment wseg = n.Value as Course.WattsSegment;
										data.StartYUnits = wseg.StartY;
										data.EndYUnits = wseg.EndY;
									}
									strOut.AddRawField(xtag.Data, data);
								}
							}
							break;
					}

                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }

            public bool Read(RawStream strIn, float version)
            {
                bool result = false;
                try
                {							   
					Version = version;
                    if(cCourse.LoadRMPCourse(strIn, version))
                        result = true;
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    result = false;
                }
                return result;
            }
        }
    }

    /*
    public struct RMPHeader
    {
        public string CreatorExe; // 32 program that created
        public DateTime Date;  // date created
        public float Version;    // version of the this format
        public string Comment; // 32 description of this file
        public string Copyright;   // 32 RacerMate copyright
        public Int32 CompressType; // different compression type
    }
    */

#if D3DHOST
#else
    public class Performance : RM1.IStatsEx
    {
        public PerfFrame.PerfData cur = PerfFrame.PerfDataCreate();
        PerfFrame.PerfData next;
        PerfFrame.PerfData prev;
        StatFlags m_valid;
        float m_splitTime;
        Int64 m_ticks;
        double m_curTime;
        float m_factor;

        public Performance() {}

        public Performance(double time, ref PerfFrame.PerfData p, ref PerfFrame.PerfData n, StatFlags valid)
        {
            Change(time, ref p, ref n, valid);
        }

        public void Change(double time, ref PerfFrame.PerfData p, ref PerfFrame.PerfData n, StatFlags valid)
        {
            prev = p;
            next = n;
            m_ticks = 0;
            m_splitTime = (float)(n.TimeAcc - p.TimeAcc);
            m_factor = 0;
            if (time < n.TimeAcc && time > p.TimeAcc)
            {
                m_factor = (float)((n.TimeAcc - time) / (n.TimeAcc - p.TimeAcc));
            }
            m_curTime = time;
            m_valid = valid;
            interpolate(valid);
        }

        public double interpolate(double p, double n)
        {
            if (p != n && m_factor > 0)
                return (n - (m_factor * (n - p)));
            else
                return n;
        }

        public float interpolate(float p, float n)
        {
            if (p != n && m_factor > 0)
                return (n - (m_factor * (n - p)));
            else
                return n;
        }

        public void interpolate(StatFlags validflags)
        {
            
            if ((validflags & StatFlags.Time) != StatFlags.Zero) { cur.TimeAcc = interpolate(prev.TimeAcc, next.TimeAcc); }//prev.TimeAcc; }
            cur.TimeMS = (UInt64)(cur.TimeAcc * 1000);
            if ((validflags & StatFlags.LapTime) != StatFlags.Zero) { cur.LapTime = prev.LapTime; }
            if ((validflags & StatFlags.Lap) != StatFlags.Zero) { cur.Lap = prev.Lap; }
            if ((validflags & StatFlags.Distance) != StatFlags.Zero) { cur.Distance = interpolate(prev.Distance,next.Distance); }
            if ((validflags & StatFlags.Lead) != StatFlags.Zero) { cur.Lead = prev.Lead; }
            if ((validflags & StatFlags.Grade) != StatFlags.Zero) { cur.Grade = interpolate(prev.Grade,next.Grade); }
            if ((validflags & StatFlags.Wind) != StatFlags.Zero) { cur.Wind = interpolate(prev.Wind,next.Wind); }
            if ((validflags & StatFlags.Speed) != StatFlags.Zero) { cur.Speed = interpolate(prev.Speed,next.Speed); }
            if ((validflags & StatFlags.Speed_Avg) != StatFlags.Zero) { cur.Speed_Avg = interpolate(prev.Speed_Avg,next.Speed_Avg); }
            if ((validflags & StatFlags.Speed_Max) != StatFlags.Zero) { cur.Speed_Max = interpolate(prev.Speed_Max,next.Speed_Max); }
            if ((validflags & StatFlags.Watts) != StatFlags.Zero) { cur.Watts = interpolate(prev.Watts,next.Watts); }
            if ((validflags & StatFlags.Watts_Avg) != StatFlags.Zero) { cur.Watts_Avg = interpolate(prev.Watts_Avg,next.Watts_Avg); }
            if ((validflags & StatFlags.Watts_Max) != StatFlags.Zero) { cur.Watts_Max = interpolate(prev.Watts_Max,next.Watts_Max); }
            if ((validflags & StatFlags.Watts_Wkg) != StatFlags.Zero) { cur.Watts_Wkg = interpolate(prev.Watts_Wkg,next.Watts_Wkg); }
            if ((validflags & StatFlags.Watts_Load) != StatFlags.Zero) { cur.Watts_Load = interpolate(prev.Watts_Load,next.Watts_Load); }
            if ((validflags & StatFlags.HeartRate) != StatFlags.Zero) { cur.HeartRate = interpolate(prev.HeartRate,next.HeartRate); }
            if ((validflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) { cur.HeartRate_Avg = interpolate(prev.HeartRate_Avg,next.HeartRate_Avg); }
            if ((validflags & StatFlags.HeartRate_Max) != StatFlags.Zero) { cur.HeartRate_Max = interpolate(prev.HeartRate_Max,next.HeartRate_Max); }
            if ((validflags & StatFlags.Cadence) != StatFlags.Zero) { cur.Cadence = interpolate(prev.Cadence,next.Cadence); }
            if ((validflags & StatFlags.Cadence_Avg) != StatFlags.Zero) { cur.Cadence_Avg = interpolate(prev.Cadence_Avg,next.Cadence_Avg); }
            if ((validflags & StatFlags.Cadence_Max) != StatFlags.Zero) { cur.Cadence_Max = interpolate(prev.Cadence_Max,next.Cadence_Max); }
            if ((validflags & StatFlags.Calories) != StatFlags.Zero) { cur.Calories = interpolate(prev.Calories,next.Calories); }
            if ((validflags & StatFlags.PulsePower) != StatFlags.Zero) { cur.PulsePower = interpolate(prev.PulsePower,next.PulsePower); }
            if ((validflags & StatFlags.DragFactor) != StatFlags.Zero) { cur.DragFactor = interpolate(prev.DragFactor,next.DragFactor); }
            if ((validflags & StatFlags.SS) != StatFlags.Zero) { cur.SS = interpolate(prev.SS,next.SS); }
            if ((validflags & StatFlags.SSLeft) != StatFlags.Zero) { cur.SSLeft = interpolate(prev.SSLeft,next.SSLeft); }
            if ((validflags & StatFlags.SSRight) != StatFlags.Zero) { cur.SSRight = interpolate(prev.SSRight,next.SSRight); }
            if ((validflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { cur.SSLeftSplit = interpolate(prev.SSLeftSplit,next.SSLeftSplit); }
            if ((validflags & StatFlags.SSRightSplit) != StatFlags.Zero) { cur.SSRightSplit = interpolate(prev.SSRightSplit,next.SSRightSplit); }
            if ((validflags & StatFlags.SSLeftATA) != StatFlags.Zero) { cur.SSLeftATA = interpolate(prev.SSLeftATA,next.SSLeftATA); }
            if ((validflags & StatFlags.SSRightATA) != StatFlags.Zero) { cur.SSRightATA = interpolate(prev.SSRightATA,next.SSRightATA); }
            if ((validflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) { cur.SSLeft_Avg = interpolate(prev.SSLeft_Avg,next.SSLeft_Avg); }
            if ((validflags & StatFlags.SSRight_Avg) != StatFlags.Zero) { cur.SSRight_Avg = interpolate(prev.SSRight_Avg,next.SSRight_Avg); }
            //if ((validflags & StatFlags.RightPower) != StatFlags.Zero) { cur.RightPower = interpolate(prev.RightPower,next.RightPower); }
            if ((validflags & StatFlags.PercentAT) != StatFlags.Zero) { cur.PercentAT = interpolate(prev.PercentAT,next.PercentAT); }
            if ((validflags & StatFlags.FrontGear) != StatFlags.Zero) { cur.FrontGear = (int)interpolate((float)prev.FrontGear, (float)next.FrontGear); }
            if ((validflags & StatFlags.RearGear) != StatFlags.Zero) { cur.RearGear = (int)interpolate((float)prev.RearGear, (float)next.RearGear); }
            if ((validflags & StatFlags.GearInches) != StatFlags.Zero) { cur.GearInches = prev.GearInches; }
            //if ((validflags & StatFlags.RawSpinScan) != StatFlags.Zero) { cur.RawSpinScan = prev.RawSpinScan; }
            //if ((validflags & StatFlags.CadenceTiming) != StatFlags.Zero) { cur.CadenceTiming = interpolate(prev.CadenceTiming,next.CadenceTiming); }
            if ((validflags & StatFlags.TSS) != StatFlags.Zero) { cur.TSS = interpolate(prev.TSS,next.TSS); }
            if ((validflags & StatFlags.IF) != StatFlags.Zero) { cur.IF = interpolate(prev.IF,next.IF); }
            if ((validflags & StatFlags.NP) != StatFlags.Zero) { cur.NP = interpolate(prev.NP, next.NP); }
            if ((validflags & StatFlags.Bars_Shown) != StatFlags.Zero) { cur.Bars_Shown = prev.Bars_Shown; }
            if ((validflags & StatFlags.Bars) != StatFlags.Zero)
            {
                for (int i = 0; i < 24; i++)
                {
                    cur.Bars[i] = interpolate(prev.Bars[i], next.Bars[i]);
                }
            }
            if ((validflags & StatFlags.Bars_Avg) != StatFlags.Zero)
            {
                for (int i = 0; i < 24; i++)
                {
                    cur.AverageBars[i] = interpolate(prev.AverageBars[i], next.AverageBars[i]);
                }
            }
            if ((validflags & StatFlags.CourseScreenX) != StatFlags.Zero) { cur.CourseScreenX = prev.CourseScreenX; }
            if ((validflags & StatFlags.RiderName) != StatFlags.Zero) { cur.RiderName = prev.RiderName; }
            if ((validflags & StatFlags.Course) != StatFlags.Zero) { cur.Course = prev.Course; }

            // Added v1.02
            if ((validflags & StatFlags.Calibration) != StatFlags.Zero) { cur.RawCalibrationValue = (Int16)(0.5f + interpolate((float)prev.RawCalibrationValue, (float)next.RawCalibrationValue)); }
            if ((validflags & StatFlags.Disconnected) != StatFlags.Zero) { cur.Disconnected = prev.Disconnected; }
            if ((validflags & StatFlags.Drafting) != StatFlags.Zero) { cur.Drafting = prev.Drafting; }
        }


        // RM1.IStats
        // =============================================================
        public bool Metric { get { return true; } }
        public Int64 Ticks { get { return m_ticks; } }
        public float SplitTime { get { return m_splitTime; } }
        public float Speed { get { return cur.Speed; } }
        public double SpeedDisplay { get { return cur.Speed * ConvertConst.MetersPerSecondToMPHOrKPH; } }  // this is fake and only is here since LineGraph had a bug on Speed vs SpeedDisplay
        public float Cadence { get { return cur.Cadence; } }
        public float HeartRate { get { return cur.HeartRate; } }
        public float Watts { get { return cur.Watts; } }
        public float SS { get { return cur.SS; } }
        public float SSLeft { get { return cur.SSLeft; } }
        public float SSRight { get { return cur.SSRight; } }
        public float SSLeftSplit { get { return cur.SSLeftSplit; } }
        public float SSRightSplit { get { return cur.SSRightSplit; } }
        public float Calories { get { return cur.Calories; } }
        public float PulsePower { get { return cur.PulsePower; } }
        public float DragFactor { get { return cur.DragFactor; } }
        public float NP { get { return cur.NP; } }
        public float IF { get { return cur.IF; } }
        public float TSS { get { return cur.TSS; } }
        public float[] Bars { get { return cur.Bars; } }
        public float[] AverageBars { get { return cur.AverageBars; } }
        public int FrontGear { get { return cur.FrontGear; } }	// Velotron only -1 if not valid
        public int RearGear { get { return cur.RearGear; } }	// Velotron only -1 if not valie
        public int GearInches { get { return cur.GearInches; } } // Velotron only - We should be able to caluclate this for non-computrainers.
        public float Grade { get { return cur.Grade; } set { } }
        public float Watts_Load { get { return cur.Watts_Load; } set {} }

		public float Wind { get { return cur.Wind; } set { } }
		public bool Drafting { get { return false; /* cur.Drafting; */ } set { } }

		#pragma warning disable 67
        public event RM1.IStatsEvent OnUpdate;
		#pragma warning restore 67

        // RM1.IStatsEx
        // =============================================================
        public double Time { get { return cur.TimeAcc; } }
        public float Speed_Avg { get { return cur.Speed_Avg; } }
        public float Speed_Max { get { return cur.Speed_Max; } }
        public float Watts_Avg { get { return cur.Watts_Avg; } }
        public float Watts_Max { get { return cur.Watts_Max; } }
        public float HeartRate_Avg { get { return cur.HeartRate_Avg; } }
        public float HeartRate_Max { get { return cur.HeartRate_Max; } }
        public float Cadence_Avg { get { return cur.Cadence_Avg; } }
        public float Cadence_Max { get { return cur.Cadence_Max; } }

        public float SSLeftATA { get { return cur.SSLeftATA; } }
        public float SSRightATA { get { return cur.SSRightATA; } }
		public float SSLeft_Avg { get { return cur.SSLeft_Avg; } }
		public float SSRight_Avg { get { return cur.SSRight_Avg; } }
		public float SS_Avg { get { return (cur.SSLeft_Avg + cur.SSRight) * 0.5f; } }

        public String GearingString { get { return ""; } }
    }

    public class PerfComparer : IComparer<PerfFrame.PerfData>
    {
        public int Compare(PerfFrame.PerfData x, PerfFrame.PerfData y)
        {
            return(x.TimeMS < y.TimeMS ? -1 : (x.TimeMS > y.TimeMS ? 1 : 0));
        }
    }

    public class PerfComparerDistance : IComparer<PerfFrame.PerfData>
    {
        public int Compare(PerfFrame.PerfData x, PerfFrame.PerfData y)
        {
            return(x.Distance < y.Distance ? -1 : (x.Distance > y.Distance ? 1 : 0));
        }
    }


    static public class gVar
    {
        static public char[] bytezero = new char[] { '\0' };
        static public StatFlags AllFlags = StatFlags.Mask;
        static public StatFlags PerformanceFlags =
            StatFlags.Time |
            StatFlags.Distance |
            StatFlags.Speed |
            StatFlags.Wind |
            StatFlags.Watts |
            StatFlags.Grade |
            StatFlags.HeartRate |
            StatFlags.Cadence |
            StatFlags.PulsePower |
            StatFlags.DragFactor |
            StatFlags.SSLeftATA |
            StatFlags.SSRightATA |
            StatFlags.SSRight |
            StatFlags.SSLeft |
            StatFlags.SSLeftSplit |
            StatFlags.SSRightSplit |
            StatFlags.SS |
            //StatFlags.RawSpinScan |
            StatFlags.FrontGear |
            StatFlags.RearGear |
            StatFlags.GearInches |
            StatFlags.Calories |
            StatFlags.Watts_Load |
            //StatFlags.LeftPower |
            //StatFlags.RightPower |
            StatFlags.PercentAT |
            //StatFlags.CadenceTiming |
            StatFlags.TSS |
            StatFlags.IF |
            StatFlags.NP |
            StatFlags.Bars |

            StatFlags.Drafting | // added v1.02
            StatFlags.Disconnected | // added v1.02
            StatFlags.Calibration; // added v1.02

        // Raw Performance constants
        // Always changing
        static public StatFlags Group1 = StatFlags.Distance | StatFlags.Watts | StatFlags.Watts_Load;

        static public StatFlags Group2 = StatFlags.Speed | StatFlags.Cadence; 
        static public StatFlags Group3 = StatFlags.Grade; 
        static public StatFlags Group4 = StatFlags.Wind; 
        static public StatFlags Group5 = StatFlags.HeartRate; 
        static public StatFlags Group6 = StatFlags.Calories;

        // Very seldom change 
        static public StatFlags Group7 = StatFlags.PulsePower | StatFlags.DragFactor | StatFlags.Calibration;

        static public StatFlags Group8 = StatFlags.FrontGear | StatFlags.RearGear | StatFlags.GearInches;
        static public StatFlags Group9 = StatFlags.TSS | StatFlags.IF | StatFlags.NP; 
        static public StatFlags Group10 = StatFlags.SS | StatFlags.SSLeft | StatFlags.SSRight | StatFlags.SSLeftSplit | StatFlags.SSRightSplit | StatFlags.SSLeftATA | StatFlags.SSRightATA | StatFlags.Bars;

        // group seldom changes, mainly for boolean values
        static public StatFlags Group11 = StatFlags.Disconnected; // added v1.02
        // group changes often, mainly for boolean values
        static public StatFlags Group12 = StatFlags.Drafting;  // added v1.02

        static public StatFlags AllGroups = Group1 | Group2 | Group3 | Group4 | Group5 | Group6 | Group7 | Group8 | Group9 | Group10 | Group11 | Group12;
    }

	public class Perf
	{
		private int tick = 0;

		// Keeping track of loaded performances for debug testing.
		public static List<String> LoadedSets = new List<String>();
		public static int ms_KeyCnt = 0;
		String m_LoadedKey = null;
		void removeKey()
		{
			if (m_LoadedKey != null)
			{
				Perf.LoadedSets.Remove(m_LoadedKey);
				m_LoadedKey = null;
			}
		}
		void setKey(String filename, int size)
		{
			String old = m_LoadedKey == null ? "" : "\n\t-" + m_LoadedKey;
			removeKey();
			m_LoadedKey = filename + " Size:" + size + " Cnt:" + (++ms_KeyCnt) + old;
			Perf.LoadedSets.Add(m_LoadedKey);
		}
		// =======================================================
		~Perf()
		{
			removeKey(); // When it is flushed from memory we can remove the key... or when we get reloaded.
		}

        //PerfFile LoadedPerf;
        // Loaded frames data
        public PerfFrame.RMX LoadedRMX = new PerfFrame.RMX { Header = new PerfFile.CRMHeader(), Info = new PerfFile.CPerfInfo(), course = new Course() };

		public CourseInfo CourseInfo { get; protected set; }

        // current running frames
        StatFlags PerfFlags = gVar.AllFlags;


        LinkedListNode<PerfFrame.PerfData> m_curFrame = null;
			private uint framecnt = 0;			// todo: check usage

        public PerfFrame.PerfData LastPD = PerfFrame.PerfDataCreate(); // for keeping keyframe
        //public PerfFrame.PerfRawData LastPRD = PerfFrame.PerfRawDataCreate(); // for keeping keyframe
        public PerfFile.CPerfRawData LastCPRD = new PerfFile.CPerfRawData(); // for keeping keyframe

        string m_loadedfilename = "";
			public string m_filename = "";
        DateTime m_runDate = DateTime.Now;
        RawStream m_rawStrm = new RawStream();

        // current loaded frames
        //public bool Metric = true;
        public List<PerfFrame.PerfData> LoadedFrames = new List<PerfFrame.PerfData>();

        // Create a frame interval
        //Int32 KeyFrameVal = 10000;


        // working variables
        public PerfFrame.PerfData m_datapdAcc = PerfFrame.PerfDataCreate();
        public PerfFrame.PerfData m_datapdAvg = PerfFrame.PerfDataCreate();
        public PerfFrame.PerfData m_datapdMax = PerfFrame.PerfDataCreate();
        public PerfFrame.PerfData m_datapdMin = PerfFrame.PerfDataCreate();

        public int m_lastLap;
        public double m_LapDistance;
        public double m_lastLapTime;
        public List<float> m_LapTimes = new List<float>();

        public bool started = false;

        //RacerMatePaths.PerformancesFullPath
        public List<PerformanceFileEntry> GetPerfList(string dir)
        {
            List<PerformanceFileEntry> list = new List<PerformanceFileEntry>();
            string[] files = Directory.GetFiles(dir, "*.RMP");
			foreach (string filename in files)
			{
                LoadHeaderRawTemps(filename);
                PerformanceFileEntry entry = new PerformanceFileEntry()
                {
                    Date = LoadedRMX.Header.Date,
                    CourseName = LoadedRMX.Info.CourseName,
                    CourseType = (CourseType)LoadedRMX.Info.CourseType,
                    Ridername = LoadedRMX.Info.RiderName,
                    TimeMS = LoadedRMX.Info.TimeMS,
                    FileName = filename
                };
                list.Add(entry);
                Debug.WriteLine(filename);
			}
            return list;
        }

        // Get the from the current running frames list
        public List<PerfFrame.PerfData> GetLoadedList()
        {
            return LoadedFrames;
        }

			public Performance GetLoadedFrame(double timeacc)  {
            return GetLoadedFrame(timeacc, gVar.PerformanceFlags, ref m_curFrame);
        }
			public Performance GetLoadedFrame(double timeacc, StatFlags neededFlags)  {
            return GetLoadedFrame(timeacc, neededFlags, ref m_curFrame);
        }
			public Performance GetLoadedFrame(double timeacc, StatFlags neededFlags, ref LinkedListNode<PerfFrame.PerfData> cur)  {
            Performance workPerformance = new Performance();
            return GetLoadedFrame(ref workPerformance, timeacc, neededFlags, ref cur);
        }
			public Performance GetLoadedFrame(ref Performance performance, double timeacc)  {
            return GetLoadedFrame(ref performance, timeacc, gVar.PerformanceFlags, ref m_curFrame);
        }
			public Performance GetLoadedFrame(ref Performance performance, double timeacc, StatFlags neededFlags)  {
            return GetLoadedFrame(ref performance, timeacc, neededFlags, ref m_curFrame);
        }

		static PerfFrame.PerfData ms_TempFrame;
		public double FindDistanceTime(double distance)
		{
			ms_TempFrame.Distance = distance;
			PerfComparerDistance cp = new PerfComparerDistance();
			int fnd = LoadedFrames.BinarySearch(ms_TempFrame, cp);
			int next,prev;
			if (fnd < 0)
			{
				next = ~fnd;
                next = Math.Min(LoadedFrames.Count - 1, next); // safety checks
                prev = next > 0 ? next - 1 : 0;
			}
			else
			{
				prev = fnd > 0 ? fnd - 1 : 0;
				next = fnd;
			}
			double dp = LoadedFrames.ElementAt<PerfFrame.PerfData>(prev).Distance;
			double dn = LoadedFrames.ElementAt<PerfFrame.PerfData>(next).Distance;
			double tp = LoadedFrames.ElementAt<PerfFrame.PerfData>(prev).TimeAcc;
			if (dp == dn)
				return tp;
			double tn = LoadedFrames.ElementAt<PerfFrame.PerfData>(next).TimeAcc;
			
			return tp + (tn - tp) * ((dn - distance) / (dp - dn));
		}


        // Get the from a loaded frames linklist
        //public Performance GetLoadedFrame(double timeacc)
        public Performance GetLoadedFrame(ref Performance performance, double timeacc, StatFlags neededFlags, ref LinkedListNode<PerfFrame.PerfData> cur)
        {
            if (timeacc < 0 || LoadedFrames.Count <= 0)
                return null;
            PerfFrame.PerfData tprev;
            PerfFrame.PerfData t = new PerfFrame.PerfData {TimeMS = (UInt64)(timeacc * 1000)};
            PerfComparer cp = new PerfComparer();
            int fnd = LoadedFrames.BinarySearch(t, cp);
            if (fnd < 0)
            {
                int next = ~fnd;
                next = Math.Min(LoadedFrames.Count - 1, next); // safety checks
                int prev = next > 0 ? next - 1 : 0;
                // add interpolation here
                tprev = LoadedFrames.ElementAt<PerfFrame.PerfData>(prev);
                t = LoadedFrames.ElementAt<PerfFrame.PerfData>(next);
                
            }
            else
            {
                int prev = fnd > 0 ? fnd - 1 : 0;
                tprev = LoadedFrames.ElementAt<PerfFrame.PerfData>(prev);
                t = LoadedFrames.ElementAt<PerfFrame.PerfData>(fnd);
            }
            performance.Change(timeacc, ref tprev, ref t, neededFlags);
            return performance;
        }


		int m_RecalcStartIdx;
		double m_RecalcStartAt;
		double m_RecalcEndAt;
		public void Recalc(double startat, double endat)
		{
			if (startat != m_RecalcStartAt || endat != m_RecalcEndAt)
				ClearRecalc();

			// Find the start frame
			PerfFrame.PerfData t = new PerfFrame.PerfData { TimeMS = (UInt64)(startat * 1000) };
			PerfComparer cp = new PerfComparer();
			int fnd = LoadedFrames.BinarySearch(t, cp);
			if (fnd < 0)
				fnd = ~fnd;	// We always want this one or the next in series anyway.
			fnd = fnd < 1 ? 0:fnd - 1; // Go one back or stay at zero
			m_RecalcStartIdx = fnd;
			m_RecalcStartAt = startat;
			m_RecalcEndAt = endat;

			// Deal with this frame special like
			t = LoadedFrames[fnd];
			float speed_max = t.Speed;
			float cadence_max = t.Cadence;
			float heartrate_max = t.HeartRate;
			float watts_max = t.Watts;

			double last = t.TimeAcc, split, tacc = 0;
			double speed_acc = 0;
			double cadence_acc = 0;
			double heartrate_acc = 0;
			double watts_acc = 0;

			t.Speed_Avg = t.Speed_Max = speed_max;
			t.Cadence_Avg = t.Cadence_Max = cadence_max;
			t.HeartRate_Avg = t.HeartRate_Max = heartrate_max;
			t.Watts_Avg = t.Watts_Max = watts_max;

			LoadedFrames[fnd] = t;
			int i;
			int cnt = LoadedFrames.Count;
			for (i = fnd + 1; i < cnt; i++)
			{
				t = LoadedFrames[i];
				t.Speed_Max = t.Speed > speed_max ? (speed_max = t.Speed):speed_max;
				t.Cadence_Max = t.Cadence > cadence_max ? (cadence_max = t.Cadence):cadence_max;
				t.HeartRate_Max = t.HeartRate > heartrate_max ? (heartrate_max = t.HeartRate):heartrate_max;
				t.Watts_Max = t.Watts > watts_max ? (watts_max = t.Watts):watts_max;

				split = t.TimeAcc - last;
				last = t.TimeAcc;
				tacc += split;
				t.Speed_Avg = (float)((speed_acc += t.Speed * split) / tacc);
				t.Cadence_Avg = (float)((cadence_acc += t.Cadence * split) / tacc);
				t.HeartRate_Avg = (float)((heartrate_acc += t.HeartRate * split) / tacc);
				t.Watts_Avg = (float)((watts_acc += t.Watts * split) / tacc);

				LoadedFrames[i] = t;
				if (t.TimeAcc >= endat)
					break;
			}
		}
		public void ClearRecalc()
		{
			if (m_RecalcStartAt == 0 && m_RecalcEndAt == 0)
				return;
			int fnd = m_RecalcStartIdx - 1;
			if (fnd < 0)
				fnd = 0;

			PerfFrame.PerfData t;

			// Deal with this frame special like
			t = LoadedFrames[fnd];
			float speed_max = t.Speed_Max;
			float cadence_max = t.Cadence_Max;
			float heartrate_max = t.HeartRate_Max;
			float watts_max = t.Watts_Max;

			double last = t.TimeAcc, split, tacc = 0;
			double speed_acc = t.Speed_Avg * last;
			double cadence_acc = t.Cadence_Avg * last;
			double heartrate_acc = t.HeartRate_Avg * last;
			double watts_acc = t.Watts_Avg * last;

			int i;
			int cnt = LoadedFrames.Count;
			for (i = fnd + 1; i < cnt; i++)
			{
				t = LoadedFrames[i];
				t.Speed_Max = t.Speed > speed_max ? (speed_max = t.Speed) : speed_max;
				t.Cadence_Max = t.Cadence > cadence_max ? (cadence_max = t.Cadence) : cadence_max;
				t.HeartRate_Max = t.HeartRate > heartrate_max ? (heartrate_max = t.HeartRate) : heartrate_max;
				t.Watts_Max = t.Watts > watts_max ? (watts_max = t.Watts) : watts_max;

				split = t.TimeAcc - last;
				last = t.TimeAcc;
				tacc += split;
				t.Speed_Avg = (float)((speed_acc += t.Speed * split) / tacc);
				t.Cadence_Avg = (float)((cadence_acc += t.Cadence * split) / tacc);
				t.HeartRate_Avg = (float)((heartrate_acc += t.HeartRate * split) / tacc);
				t.Watts_Avg = (float)((watts_acc += t.Watts * split) / tacc);

				LoadedFrames[i] = t;
			}


		}


			bool saveRawAsync = true;
			//bool saveRawAsync = false;				// tlm 20160221

			bool test = true;								// saves perfs in curframes list (RAM)
			//bool test = false;

#if DEBUG
			//long snapshotcalls = 0;
			int bp = 0;
#endif

			/*************************************************************************************
				only called once
			*************************************************************************************/

			public void SnapShotStartFile(Statistics statistics, Unit thisUnit) {
				if (saveRawAsync)  {
						try   {
                    m_runDate = DateTime.Now;
                    int iRider = thisUnit.Number;

                    string rawname = string.Format(@"Last-Rider{0}.tmp", iRider + 1);
                    string rawfilename = RacerMatePaths.PerformancesFullPath + "\\" + rawname;
                    m_rawStrm.OpenRawFileOut(rawfilename);
                    
                    PFile.RMHeader hdr = PFile.RMHeaderCreate();
                    PFile.PerfInfo info = PFile.PerfInfoCreate();
                    // Add v1.02 - added an extension header
                    PFile.PerfInfoExt1 infoext = PFile.PerfInfoExt1Create();
                    
                    // Calculate headers sizes to skip
                    int pos = Marshal.SizeOf(hdr) + Marshal.SizeOf(info) + Marshal.SizeOf(infoext);

                    // Skip header size in the file
                    m_rawStrm.SetCurRawFieldOutPos(pos);
						}			// try
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    m_rawStrm.CloseRawFileOut();
						}			// catch
				}					// if (saveRawAsync)
			}						// SnapShotStartFile()




			/*************************************************************************************
				called every 33 ms?
			*************************************************************************************/

			public void SnapShot(Statistics statistics, Unit thisUnit) {
            SnapShot(statistics, thisUnit, false);
        }

			/*************************************************************************************
				called every 33 ms
			*************************************************************************************/

			public void SnapShot(Statistics statistics, Unit thisUnit, bool start)  {
				tick++;

				if (tick >= 1800) {
					tick = 0;									// gets here once per second
				}

				if (started)  {
					if (test)  {
						//xxx
                    int iRider = thisUnit.Number;

                    StatFlags sf;
                    // OR options in snap flag with update flag
						if (start)  {
                        sf = PerfFlags;
                    }
						else  {
                        sf = statistics.PerfChanged & PerfFlags;
                    }

                    //sf |= StatFlags.Calibration; // ****test*****

                    // gets snapshot of the changed data to be added to the list
						PerfFrame.UpdateLastByFlags(ref LastPD, sf, statistics, 1);					// <<

						PerfFrame.PerfData npd = PerfFrame.PerfDataCreate();							// <<<<<<<<<<<<<<<
                    npd = LastPD;

						if (start)  {
							framecnt = 1;
                        SnapShotStartFile(statistics, thisUnit);
                    }
						else  {
							framecnt++;
                    }

						if (saveRawAsync)  {
                        //PerfFrame.PerfDataToPerfRawData(ref LastPRD, ref npd, sf);
                        //m_rawStrm.AddRawField(xtag.PerfRawData, LastPRD);
                        LastCPRD.Convert(ref npd, sf);
                        LastCPRD.Write(m_rawStrm);
                    }
					}							//if (test)  {
				}								//if (started)  {

				return;
			}									// SnapShot



			/*************************************************************************************
				called from SavePerformance(void)
			*************************************************************************************/

			public PerfFrame.RMX GetRMXHeader(Statistics statistics, Unit thisUnit, float version) {
				uint cnt = framecnt;
            int iRider = thisUnit.Number;

            Rider rider = thisUnit.Rider;

            Course course = statistics.Course;
            int hrmin = 60;
            int hrmax = 170;
            int hravg = (hrmin + hrmax) / 2;
            // Flushes the data file - blocking
            PerfFile.CPerfInfo pih = new PerfFile.CPerfInfo
            {
                Version = version,
                RiderName = statistics.RiderName,
                Gender = "",
                Age = 0,
                Height = 0,
                Weight = 0,
                HeartRate = statistics.HeartRate,
                Upper_HeartRate = statistics.HeartRate_Max,
                Lower_HeartRate = statistics.HeartRate_Avg,
                CourseName = xval.None,
                CourseType = 0,
                Distance = 0,
                RFDrag = 0,
                RFMeas = 0,
                Watts_Factor = 0,
                FTP = 0,									// functional threshold power
                PerfCount = (int)cnt,	    // Number of performance points.
                TimeMS = (UInt64)(statistics.Time * 1000),

                CourseOffset = 0, //m_rawStrm.GetCurRawFieldOutPos(),

                DragFactor = 0,
                DeviceType = 0,
                DeviceVersion = 0,
                PowerAnT = 0,
                PowerFTP = 0,
                Mode = Unit.AppMode,
                RawCalibrationValue = statistics.RawCalibrationValue, //- 200,
                HrMin = hrmin,
                HrMax = hrmax,
                HrZone1 = (int)(hravg * 0.50f),
                HrZone2 = (int)(hravg * 0.60f),
                HrZone3 = (int)(hravg * 0.70f),
                HrZone4 = (int)(hravg * 0.80f),
                HrZone5 = (int)(hravg * 0.90f),
                HrAnT = (int)(hravg * 0.90f)

            };
            if (thisUnit.Trainer != null)
            {
                //pih.CalibrationValue = thisUnit.Trainer.CalibrationValue * (thisUnit.Trainer.IsCalibrated ? 1 : -1);
                pih.DeviceType = (uint) thisUnit.Trainer.Type;
                pih.DeviceVersion = (uint) thisUnit.Trainer.VersionNum;
            }
            if (course != null)
            {
                pih.CourseType = (uint)course.Type;
                pih.CourseName = course.Name;
                pih.Distance = float.Parse(course.StringLength);
            }
            if (rider != null)
            {
                //pih.Key = rider.DatabaseKey;
                pih.RiderName = rider.FullName;
                pih.Gender = rider.Gender;
                pih.Weight = (float)rider.WeightRider;
                pih.Age = rider.Age;
                pih.RFDrag = (float)rider.DragFactor;
                pih.RFMeas = 0;
                pih.Watts_Factor = 0;
                pih.FTP = (float)rider.PowerFTP;        
                pih.PowerAnT = rider.PowerAnT;
                pih.PowerFTP = rider.PowerFTP;
                pih.HrAnT = rider.HrAnT;
                pih.HrMin = rider.HrMin;
                pih.HrMax = rider.HrMax;
                pih.HrZone1 = rider.HrZone1;
                pih.HrZone2 = rider.HrZone2;
                pih.HrZone3 = rider.HrZone3;
                pih.HrZone4 = rider.HrZone4;
                pih.HrZone5 = rider.HrZone5;
            }

            PerfFile.CRMHeader rmh = new PerfFile.CRMHeader
            {
                CreatorExe = xval.RacerMateOne, // 32 program that created
                Date = m_runDate,  // date created
                Version = version,    // version of the this format
                Comment = "Performance File", // 32 description of this file
                Copyright = xval.Copyright,   // 32 RacerMate copyright
                CompressType = 0 // different compression type
            };

            PerfFile.CCourse cc = new PerfFile.CCourse
            {
                cCourse = statistics.Course
            };

            PerfFrame.RMX rmx = new PerfFrame.RMX
            {
                Header = rmh,
                Info = pih,
                course = statistics.Course
            };
            return rmx;
			}														// GetRMXHeader()

			/*************************************************************************************

			*************************************************************************************/

			public String SnapShotEnd(Statistics statistics, Unit thisUnit)  {
			String ans = null;

				if (started)  {
                int iRider = thisUnit.Number;
                Rider rider = thisUnit.Rider;

                // save last data 
                SnapShot(statistics, thisUnit);

					if (test)  {
						//xxx
                    //uint cnt = (uint)statistics.PerfRunning.Count;
						uint cnt = framecnt;

						if (cnt <= 10) {
							return ans;
						}

						try  {
                            DateTime start = DateTime.Now;
                            PerfFrame.RMX rmx = GetRMXHeader(statistics, thisUnit, xval.Version);

                            Course tcourse = Unit.AppMode == AppModes.RCV ? statistics.Course.VideoCourse:statistics.Course;
                            // the RCV performance files do not save the courses in same way as the 
                            // 3d. It affects the examination of the preformance in the PowerTraining Review mode.
                          
                            if (tcourse == null)
								tcourse = statistics.Course;

								PerfFile.CCourse cc = new PerfFile.CCourse { cCourse = tcourse	};

                            string strTime = xval.SecondsToTimeStringLabel(statistics.Time);
                            string name = string.Format(@"{0:yyyy-MM-dd@HH-mm-ss}_{1}-{2}_{3}_{4}_{5}",
                                m_runDate, iRider + 1, rmx.Info.RiderName, rmx.Info.CourseType, rmx.Info.CourseName, strTime);
                            string filename = RacerMatePaths.PerformancesFullPath + "\\" + name + ".rmp";
                            m_filename = filename;

                            // Flushes the data file - blocking
                            rmx.Info.CourseOffset = m_rawStrm.GetCurRawFieldOutPos();

                            if (cc.cCourse != null)
                                cc.Write(m_rawStrm);

                            // Flushes the data file - blocking
                            m_rawStrm.SetCurRawFieldOutPos(0);
                            rmx.Header.Write(m_rawStrm);
                            rmx.Info.Write(m_rawStrm);

                            // To be compared end of Header sizes with the skipped one when the file was created
                            Int64 pos = m_rawStrm.GetCurRawFieldOutPos();

                            // Close the header file - blocking
                            m_rawStrm.CloseRawFileOut();

                            // rename data file
                            string rawdataname = string.Format(@"Last-Rider{0}.tmp", iRider + 1);
                            string rawdatafilename = RacerMatePaths.PerformancesFullPath + "\\" + rawdataname;
                            string newrawdataname = string.Format(@"{0}.rmp", name);
                            string newrawdatafilename = RacerMatePaths.PerformancesFullPath + "\\" + newrawdataname;

								if (File.Exists(rawdatafilename))  {
								File.Move(rawdatafilename, newrawdatafilename);
								ans = newrawdatafilename;
							}

                            DateTime end = DateTime.Now;
                            //Debug.WriteLine(string.Format(@"Done {0} SnapShotEnd", end - start));

                            //**** Test for writing and reading ******
                            //ExportCSVFromFile(null, newrawdatafilename, gVar.PerformanceFlags);
                            //ExportCSVFromFile(null, rawheaderfilename, StatFlags.Mask);
                        }
						catch (Exception exc)  {
                            Debug.WriteLine(exc.Message);
                            m_rawStrm.CloseRawFileOut();
                        }
					}					// if (test)
				}						// if (started)
			return ans; // Return the filename.
			}							// SnapShotEnd()



			/*************************************************************************************

			*************************************************************************************/

        public void calcValue(ref double accumVal, ref double minVal, ref double maxVal, double n, double timeAcc)
        {
            accumVal += n;
            if (timeAcc > 5.0)
            {
                minVal = Math.Min(minVal, n);
                maxVal = Math.Max(maxVal, n);
            }
            else
            {
                maxVal = minVal = n;
            }
        }
        public void calcValue(ref float accumVal, ref float minVal, ref float maxVal, float n, double timeAcc)
        {
            accumVal += n;
            if (timeAcc > 5.0)
            {
                minVal = Math.Min(minVal, n);
                maxVal = Math.Max(maxVal, n);
            }
            else
            {
                maxVal = minVal = n;
            }
        }
        public void calcValue(ref int accumVal, ref int minVal, ref int maxVal, int n, double timeAcc)
        {
            accumVal += n;
            if (timeAcc > 5.0)
            {
                minVal = Math.Min(minVal, n);
                maxVal = Math.Max(maxVal, n);
            }
            else
            {
                maxVal = minVal = n;
            }
        }
        public void calcAvg(ref PerfFrame.PerfData pd, uint count)
        {
            //float div = (float)pd.TimeAcc;
            uint div = count;
            if (m_lastLap > 0)
                pd.LapTime /= m_lastLap;
            else
                pd.LapTime = 0.0f;
            //pd.Distance /= div;
            pd.Grade /= div;
            pd.Wind /= div;
            pd.Speed /= div;
            pd.Speed_Avg /= div;
            pd.Speed_Max /= div;
            pd.Watts /= div;
            pd.Watts_Avg /= div;
            pd.Watts_Max /= div;
            pd.Watts_Wkg /= div;
            pd.Watts_Load /= div;
            pd.HeartRate /= div;
            pd.HeartRate_Avg /= div;
            pd.HeartRate_Max /= div;
            pd.Cadence /= div;
            pd.Cadence_Avg /= div;
            pd.Cadence_Max /= div;
            pd.Calories /= div;
            pd.PulsePower /= div;
            pd.DragFactor /= div;
            pd.SS /= div;
            pd.SSLeft /= div;
            pd.SSRight /= div;
            pd.SSLeftSplit /= div;
            pd.SSRightSplit /= div;
            pd.SSLeftATA /= div;
            pd.SSRightATA /= div;
            pd.SSLeft_Avg /= div;
            pd.SSRight_Avg /= div;
            //pd.LeftPower /= div;
            //pd.RightPower /= div;
            //pd.PercentAT /= div;
            pd.FrontGear = (int)((float)pd.FrontGear / div);
            pd.RearGear = (int)((float)pd.RearGear / div);
            //pd.CadenceTiming /= div;
            pd.TSS /= div;
            pd.IF /= div;
            pd.NP /= div;
            for (int i = 0; i < 24; i++)
            {
                pd.Bars[i] /= div;
            }
            for (int i = 0; i < 24; i++)
            {
                pd.AverageBars[i] /= div;
            }
            //pd.CourseScreenX /= div;
        }

        public void CalcReport(ref PerfFrame.PerfData pd, ref PerfFrame.PerfData datapdAcc, ref PerfFrame.PerfData datapdMin, ref PerfFrame.PerfData datapdMax, float timeElapse, bool bStart)
        {
            if (bStart)
            {
                datapdAcc = datapdMin = datapdMax = pd;
                m_lastLap = 1;
                m_lastLapTime = 0.0;
                m_LapTimes.Clear();
            }
            else
            {
                datapdAcc.TimeAcc = datapdMin.TimeAcc = datapdMax.TimeAcc = pd.TimeAcc;
                datapdAcc.TimeMS = datapdMin.TimeMS = datapdMax.TimeMS = pd.TimeMS;
                datapdAcc.Lap = datapdMin.Lap = datapdMax.Lap = pd.Lap;
                if (pd.Distance > (m_LapDistance * m_lastLap))
                {
                    float lapTime = (float)(pd.TimeAcc - m_lastLapTime);
                    m_lastLapTime = pd.TimeAcc;
                    m_LapTimes.Add(lapTime);
                    calcValue(ref datapdAcc.LapTime, ref datapdMin.LapTime, ref datapdMax.LapTime, lapTime, pd.TimeAcc);
                    m_lastLap++;
                }

                datapdAcc.Distance = datapdMin.Distance = datapdMax.Distance = pd.Distance;
                //calcValue(ref datapdAcc.Distance, ref datapdMin.Distance, ref datapdMax.Distance, pd.Distance, pd.TimeAcc);
                datapdAcc.Lead = datapdMin.Lead = datapdMax.Lead = pd.Lead;
                calcValue(ref datapdAcc.Grade, ref datapdMin.Grade, ref datapdMax.Grade, pd.Grade, pd.TimeAcc);
                calcValue(ref datapdAcc.Wind, ref datapdMin.Wind, ref datapdMax.Wind, pd.Wind, pd.TimeAcc);
                calcValue(ref datapdAcc.Speed, ref datapdMin.Speed, ref datapdMax.Speed, pd.Speed, pd.TimeAcc);
                calcValue(ref datapdAcc.Speed_Avg, ref datapdMin.Speed_Avg, ref datapdMax.Speed_Avg, pd.Speed_Avg, pd.TimeAcc);
                calcValue(ref datapdAcc.Speed_Max, ref datapdMin.Speed_Max, ref datapdMax.Speed_Max, pd.Speed_Max, pd.TimeAcc);
                calcValue(ref datapdAcc.Watts, ref datapdMin.Watts, ref datapdMax.Watts, pd.Watts, pd.TimeAcc);
                calcValue(ref datapdAcc.Watts_Avg, ref datapdMin.Watts_Avg, ref datapdMax.Watts_Avg, pd.Watts_Avg, pd.TimeAcc);
                calcValue(ref datapdAcc.Watts_Max, ref datapdMin.Watts_Max, ref datapdMax.Watts_Max, pd.Watts_Max, pd.TimeAcc);
                calcValue(ref datapdAcc.Watts_Wkg, ref datapdMin.Watts_Wkg, ref datapdMax.Watts_Wkg, pd.Watts_Wkg, pd.TimeAcc);
                calcValue(ref datapdAcc.Watts_Load, ref datapdMin.Watts_Load, ref datapdMax.Watts_Load, pd.Watts_Load, pd.TimeAcc);
                // set Lowest Export values for HeartRate to 40
                calcValue(ref datapdAcc.HeartRate, ref datapdMin.HeartRate, ref datapdMax.HeartRate, ((pd.HeartRate < 40) ? (datapdMax.HeartRate < 40 ? 0 : 40) : pd.HeartRate), pd.TimeAcc);
                calcValue(ref datapdAcc.HeartRate_Avg, ref datapdMin.HeartRate_Avg, ref datapdMax.HeartRate_Avg, ((pd.HeartRate_Avg < 40) ? (datapdMax.HeartRate_Avg < 40 ? 0 : 40) : pd.HeartRate_Avg), pd.TimeAcc);
                calcValue(ref datapdAcc.HeartRate_Max, ref datapdMin.HeartRate_Max, ref datapdMax.HeartRate_Max, ((pd.HeartRate_Max < 40) ? (datapdMax.HeartRate_Max < 40 ? 0 : 40) : pd.HeartRate_Max), pd.TimeAcc);
                // set Lowest Export values for Cadence to 20
                calcValue(ref datapdAcc.Cadence, ref datapdMin.Cadence, ref datapdMax.Cadence, ((pd.Cadence < 20) ? (datapdMax.Cadence < 20 ? 0 : 20) : pd.Cadence), pd.TimeAcc);
                calcValue(ref datapdAcc.Cadence_Avg, ref datapdMin.Cadence_Avg, ref datapdMax.Cadence_Avg, ((pd.Cadence_Avg < 20) ? (datapdMax.Cadence_Avg < 20 ? 0 : 20) : pd.Cadence_Avg), pd.TimeAcc);
                calcValue(ref datapdAcc.Cadence_Max, ref datapdMin.Cadence_Max, ref datapdMax.Cadence_Max, ((pd.Cadence_Max < 20) ? (datapdMax.Cadence_Max < 20 ? 0 : 20) : pd.Cadence_Max), pd.TimeAcc);

                calcValue(ref datapdAcc.Calories, ref datapdMin.Calories, ref datapdMax.Calories, pd.Calories, pd.TimeAcc);
                calcValue(ref datapdAcc.PulsePower, ref datapdMin.PulsePower, ref datapdMax.PulsePower, pd.PulsePower, pd.TimeAcc);
                calcValue(ref datapdAcc.DragFactor, ref datapdMin.DragFactor, ref datapdMax.DragFactor, pd.DragFactor, pd.TimeAcc);
                calcValue(ref datapdAcc.SS, ref datapdMin.SS, ref datapdMax.SS, pd.SS, pd.TimeAcc);
                calcValue(ref datapdAcc.SSLeft, ref datapdMin.SSLeft, ref datapdMax.SSLeft, pd.SSLeft, pd.TimeAcc);
                calcValue(ref datapdAcc.SSRight, ref datapdMin.SSRight, ref datapdMax.SSRight, pd.SSRight, pd.TimeAcc);
                calcValue(ref datapdAcc.SSLeftSplit, ref datapdMin.SSLeftSplit, ref datapdMax.SSLeftSplit, pd.SSLeftSplit, pd.TimeAcc);
                calcValue(ref datapdAcc.SSRightSplit, ref datapdMin.SSRightSplit, ref datapdMax.SSRightSplit, pd.SSRightSplit, pd.TimeAcc);
                calcValue(ref datapdAcc.SSLeftATA, ref datapdMin.SSLeftATA, ref datapdMax.SSLeftATA, pd.SSLeftATA, pd.TimeAcc);
                calcValue(ref datapdAcc.SSRightATA, ref datapdMin.SSRightATA, ref datapdMax.SSRightATA, pd.SSRightATA, pd.TimeAcc);
                calcValue(ref datapdAcc.SSLeft_Avg, ref datapdMin.SSLeft_Avg, ref datapdMax.SSLeft_Avg, pd.SSLeft_Avg, pd.TimeAcc);
                calcValue(ref datapdAcc.SSRight_Avg, ref datapdMin.SSRight_Avg, ref datapdMax.SSRight_Avg, pd.SSRight_Avg, pd.TimeAcc);
                //calcValue(ref datapdAvg.LeftPower, ref datapdMin.LeftPower, ref datapdMax.LeftPower, pd.LeftPower, pd.TimeAcc);
                //calcValue(ref datapdAvg.RightPower, ref datapdMin.RightPower, ref datapdMax.RightPower, pd.RightPower, pd.TimeAcc);
                //calcValue(ref datapdAcc.PercentAT, ref datapdMin.PercentAT, ref datapdMax.PercentAT, pd.PercentAT, pd.TimeAcc);
                calcValue(ref datapdAcc.FrontGear, ref datapdMin.FrontGear, ref datapdMax.FrontGear, pd.FrontGear, pd.TimeAcc);
                calcValue(ref datapdAcc.RearGear, ref datapdMin.RearGear, ref datapdMax.RearGear, pd.RearGear, pd.TimeAcc);
                datapdAcc.GearInches = datapdMin.GearInches = datapdMax.GearInches = pd.GearInches;
                //calcValue(ref datapdAcc.CadenceTiming, ref datapdMin.CadenceTiming, ref datapdMax.CadenceTiming, pd.CadenceTiming, pd.TimeAcc);
                calcValue(ref datapdAcc.TSS, ref datapdMin.TSS, ref datapdMax.TSS, pd.TSS, pd.TimeAcc);
                calcValue(ref datapdAcc.IF, ref datapdMin.IF, ref datapdMax.IF, pd.IF, pd.TimeAcc);
                calcValue(ref datapdAcc.NP, ref datapdMin.NP, ref datapdMax.NP, pd.NP, pd.TimeAcc);
                datapdAcc.Bars_Shown = datapdMin.Bars_Shown = datapdMax.Bars_Shown = pd.Bars_Shown;
                for (int i = 0; i < 24; i++)
                {
                    calcValue(ref datapdAcc.Bars[i], ref datapdMin.Bars[i], ref datapdMax.Bars[i], pd.Bars[i], pd.TimeAcc);
                }
                for (int i = 0; i < 24; i++)
                {
                    calcValue(ref datapdAcc.AverageBars[i], ref datapdMin.AverageBars[i], ref datapdMax.AverageBars[i], pd.AverageBars[i], pd.TimeAcc);
                }
                //calcValue(ref datapdAvg.CourseScreenX, ref datapdMin.CourseScreenX, ref datapdMax.CourseScreenX, pd.CourseScreenX, pd.TimeAcc);
                //calcValue(ref datapdAcc.RawSpinScan, ref datapdMin.RawSpinScan, ref datapdMax.RawSpinScan, pd.RawSpinScan, pd.TimeAcc);
                datapdAcc.RiderName = datapdMin.RiderName = datapdMax.RiderName = pd.RiderName;
                //datapdAvg.Course = datapdMin.Course = datapdMax.Course = pd.Course;

                // Added v1.02
                datapdAcc.RawCalibrationValue = datapdMin.RawCalibrationValue = datapdMax.RawCalibrationValue = pd.RawCalibrationValue;
                datapdAcc.Drafting = datapdMin.Drafting = datapdMax.Drafting = pd.Drafting;
                datapdAcc.Disconnected = datapdMin.Disconnected = datapdMax.Disconnected = pd.Disconnected;
            }
        }

			/**********************************************************************************************

			**********************************************************************************************/

			public void SaveReport(BackgroundWorker bw, Statistics statistics, Unit thisUnit, StatFlags exportFlags)  {
				uint cnt = framecnt;
				//PerfData pd = new PerfData(unit);
				if (cnt <= 10)  {
					return;
				}

                int iRider = thisUnit.Number;
                Rider rider = thisUnit.Rider;

                //int i = 0;
                exportFlags |= StatFlags.Time; // always include the time
                DateTime start = DateTime.Now;

                PerfFrame.RMX rmx = GetRMXHeader(statistics, thisUnit, xval.Version);
                
                DateTime start2 = DateTime.Now;

                PerfFrame.PerfData datapd = PerfFrame.PerfDataCreate();
                bool bStart = true;
                float timeElapse = 0;
                uint reportCnt = 0;
                m_LapDistance = rmx.course.TotalX;
                // Initialize
                CalcReport(ref datapd, ref m_datapdAcc, ref m_datapdMin, ref m_datapdMax, timeElapse, bStart);
                bStart = false;

                m_datapdAvg = m_datapdAcc;

				if (reportCnt > 1)  {
					if ((1.0 + datapd.Distance) > (m_LapDistance * m_lastLap))  {
                        float lapTime = (float)(datapd.TimeAcc - m_lastLapTime);
                        m_lastLapTime = datapd.TimeAcc;
                        m_LapTimes.Add(lapTime);
                        calcValue(ref m_datapdAcc.LapTime, ref m_datapdMin.LapTime, ref m_datapdMax.LapTime, lapTime, datapd.TimeAcc);
                    }
                    else
                        m_lastLap--;
                    m_datapdAvg.LapTime = m_datapdAcc.LapTime;
                    calcAvg(ref m_datapdAvg, reportCnt);
                }

                DateTime start3 = DateTime.Now;
                string name = Path.GetFileNameWithoutExtension(m_filename);
                string reportfilename = RacerMatePaths.ReportsFullPath + "\\" + name + ".html";

                ReportStream reportStrm = null;
                reportStrm = new ReportStream();

				try  {
                    reportStrm.OpenReportFileOut(reportfilename);
                    //reportStrm.DoReport(ref rmx, ref m_datapdAcc, ref m_datapdAvg, ref m_datapdMin, ref m_datapdMax, exportFlags);
                    reportStrm.DoReport(ref rmx, this, exportFlags);
                }
				catch (Exception exc)  {
                    Debug.WriteLine(exc.Message);
                }

                reportStrm.CloseReportFileOut();

                DateTime end = DateTime.Now;
                Debug.WriteLine(string.Format(@"Done Report-{0} Calc-{1} SaveReport", end - start, start2 - start3));

				return;
			}				// SaveReport()

			/**********************************************************************************************

			**********************************************************************************************/
			/*
			public void ExportPWX(BackgroundWorker bw, Statistics statistics, Unit thisUnit, StatFlags exportFlags) {
				return;
			}							// void ExportPWX()
			*/

			/**********************************************************************************************

			**********************************************************************************************/
			/*
			public void ExportCSV(BackgroundWorker bw, Statistics statistics, Unit thisUnit, StatFlags exportFlags) {
				_ExportCSV(bw, statistics, thisUnit, exportFlags, "");
				return;
        }
			*/

			/**********************************************************************************************

			**********************************************************************************************/
			/*
			void _ExportCSV(BackgroundWorker bw, Statistics statistics, Unit thisUnit, StatFlags exportFlags, string label) {
				return;
			}						// void _ExportCSV()
			*/

			/**********************************************************************************************

			**********************************************************************************************/
                 
			public void ExportPWXFromFile(BackgroundWorker bw, string filename, StatFlags exportFlags) {
				if (LoadRawTemps(bw, filename)) {
					ExportPWXFromLoadedFile(bw, exportFlags);
				}
			}

			/**********************************************************************************************

			**********************************************************************************************/

			public void ExportPWXFromLoadedFile(BackgroundWorker bw, StatFlags exportFlags) {
				string ext = Path.GetExtension(m_loadedfilename);
				string name = Path.GetFileNameWithoutExtension(m_loadedfilename);
                string exportfilename = RacerMatePaths.ExportsFullPath + "\\" + name + ".pwx";
				//string exportfilename = m_loadedfilename.Replace(ext, ".pwx");

                //[#YYYY-MM-DD#]@[#HH-MM-SS#]_ [#LAST#][#FIRST#]_[#MODE#]_[#COURSE32#] _([#HH:MM#]).RMP
                XmlStream xmlStrm = null;
                xmlStrm = new XmlStream();

				try  {
					PerfFrame.PerfData datapd = PerfFrame.PerfDataCreate();
					datapd = LoadedFrames.Last<PerfFrame.PerfData>();
					double lastTime = datapd.TimeAcc;

                    ulong interval = (ulong)RM1_Settings.General.RateIndex;
                    if (interval == 0)
                        interval = 1;
					// round up the duration to next interval, to make sure it is higher than the last data entry
                    ulong duration = (ulong)(((lastTime + interval) * 1000) / (interval * 1000));
                    string atts = "";

                    xmlStrm.Format = true;
                    xmlStrm.OpenXmlFileOut(exportfilename);


                    xmlStrm.AddXAttrib(ref atts, "xmlns", "http://www.peaksware.com/PWX/1/0");
                    xmlStrm.AddXAttrib(ref atts, "xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    xmlStrm.AddXAttrib(ref atts, "xsi:schemaLocation", "http://www.peaksware.com/PWX/1/0 http://www.peaksware.com/PWX/1/0/pwx.xsd");
                    xmlStrm.AddXAttrib(ref atts, "version", "1.0");
                    xmlStrm.AddXAttrib(ref atts, "creator", "RacerMateOne");

                    xmlStrm.AddXElementStart("pwx", atts, 1);

                    xmlStrm.AddXElementStart("workout", 1);

                    xmlStrm.AddXElementStart("athlete", 1);
					xmlStrm.AddXField("name", LoadedRMX.Info.RiderName);
                    xmlStrm.AddXElementEnd("athlete", -1);

                    xmlStrm.AddXField("sportType", "Bike");

					xmlStrm.AddXElementStart("device", XUtil.XAttr("id", ""));
                    xmlStrm.ModifyXLevel(1);

                    xmlStrm.AddXField("make", "RacerMate");
                    xmlStrm.AddXField("model", "Computrainer");
                    xmlStrm.AddXField("stopdetectionsetting", 1.0f);

                    xmlStrm.AddXElementEnd("device", -1);

					xmlStrm.AddXField("time", string.Format("{0:s}", LoadedRMX.Header.Date));

                    xmlStrm.AddXElementStart("summarydata", 1);

                    xmlStrm.AddXField("beginning", 0);
                    xmlStrm.AddXField("duration", duration);

                    xmlStrm.AddXElementEnd("summarydata", -1);
                    /*
                    float lastHeartRate = 0;
                    float lastCadence = 0;
                    double lastDistance = 0;
                    */
                    double ApproxAltitude = 0;
                    double prev_distance = 0;
					Boolean isgradebased = (LoadedRMX.course.YUnits == CourseYUnits.Grade);

					for (double timeAcc = 0; timeAcc < (lastTime + interval); timeAcc += interval)  {
                        if (bw != null && bw.CancellationPending)
                            throw new Exception("BackgroundWorker cancelled.");

						Performance t = GetLoadedFrame(timeAcc);
                        datapd = t.cur;
                        /*
						// Added a method to catch bad HeartRate and Cadence by using last valid data
                        double delta = datapd.Distance - lastDistance;
                        lastDistance = datapd.Distance;
                        lastHeartRate = ((datapd.HeartRate < 40) ? lastHeartRate : datapd.HeartRate);
                        lastCadence = ((datapd.Cadence < 20) ? ((delta > 0 && lastCadence > 20) ? lastCadence : 20) : datapd.Cadence);
                        */
                        xmlStrm.AddXElementStart("sample", 1);

                        xmlStrm.AddXField("timeoffset", (Int64)(0.5f + datapd.TimeAcc));
                        xmlStrm.AddXField("hr", (Byte)(0.5f + datapd.HeartRate));
                        xmlStrm.AddXField("spd", string.Format("{0:F5}", datapd.Speed + 0.000005));
                        xmlStrm.AddXField("pwr", (Int32)(0.5f + datapd.Watts));
                        xmlStrm.AddXField("cad", (Byte)(0.5f + datapd.Cadence));
                        xmlStrm.AddXField("dist", string.Format("{0:F5}", datapd.Distance + 0.000005));
                        double delta_distance = datapd.Distance - prev_distance;
						//if (datapd.Grade != null)  //just in case its not initialized
						if (isgradebased)  {
                            ApproxAltitude += (delta_distance * datapd.Grade / 100);
                            xmlStrm.AddXField("alt", string.Format("{0:F5}", ApproxAltitude + 0.000005));
                        }

						prev_distance = datapd.Distance;
                        xmlStrm.AddXElementEnd("sample", -1);
                    }

                    xmlStrm.AddXElementEnd("workout", -1);
                    xmlStrm.AddXElementEnd("pwx", -1);
                }
				catch (Exception exc)  {
						Log.WriteLine(exc.Message);
                }
                if (xmlStrm != null)
                    xmlStrm.CloseXmlFileOut();
			}					// ExportPWXFromLoadedFile()

			/**********************************************************************************************

			**********************************************************************************************/

			public void ExportCSVFromFile(BackgroundWorker bw, string filename, StatFlags exportFlags)  {
				if (LoadRawTemps(bw, filename)) {
					ExportCSVFromLoadedFile(bw, exportFlags);
				}
        }

			/**********************************************************************************************

			**********************************************************************************************/

			public void ExportCSVFromLoadedFile(BackgroundWorker bw, StatFlags exportFlags)  {
				string ext = Path.GetExtension(m_loadedfilename);
				string name = Path.GetFileNameWithoutExtension(m_loadedfilename);
				string exportfilename = RacerMatePaths.ExportsFullPath + "\\" + name + ".csv";
				//string exportfilename = m_loadedfilename.Replace(ext, ".csv");

                exportFlags |= StatFlags.Time; // always include the time
                //exportFlags |= StatFlags.Calibration; // **** test *****


                //[#YYYY-MM-DD#]@[#HH-MM-SS#]_ [#LAST#][#FIRST#]_[#MODE#]_[#COURSE32#] _([#HH:MM#]).csv
                CSVStream csvStrm = null;
                csvStrm = new CSVStream();

				try  {
                int interval = RM1_Settings.General.RateIndex;
                csvStrm.SetSeparator(RM1_Settings.General.Delimiter.ToString());

                csvStrm.OpenCSVFileOut(exportfilename);

                //csvStrm.AddObject(rmx.Header, xtag.Header);
                //PerfFrame.PerfInfoWriteCSV(ref csvStrm, ref rmx.Info);
                //csvStrm.AddXElementStart(xtag.Data + " " + xtag.Count + "=\"" + cnt + "\" " + xtag.DataFlags + "=\"" + exportFlags + "\" ", 1);

                PerfFrame.PerfData datapd = PerfFrame.PerfDataCreate();
                csvStrm.SetHeader(true);
                PerfFrame.WriteCSVByFlags(ref csvStrm, exportFlags, ref datapd);
                csvStrm.SetHeader(false);

					if (interval == 0)  {
						foreach (PerfFrame.PerfData n in LoadedFrames)  {
                        if (bw != null && bw.CancellationPending)
                            throw new Exception("BackgroundWorker cancelled.");

                        PerfFrame.UpdateRawLastByFlags(ref datapd, n, (StatFlags)n.StatFlags);
                        PerfFrame.WriteCSVByFlags(ref csvStrm, exportFlags, ref datapd);
                    }
                }
					else  {
                    datapd = LoadedFrames.Last<PerfFrame.PerfData>();
                    double lastTime = datapd.TimeAcc;

						for (double timeAcc = 0; timeAcc < (lastTime + interval); timeAcc += interval)  {
                        if (bw != null && bw.CancellationPending)
                            throw new Exception("BackgroundWorker cancelled.");

                        Performance t = GetLoadedFrame(timeAcc);
                        datapd = t.cur;
                        PerfFrame.WriteCSVByFlags(ref csvStrm, exportFlags, ref datapd);
                    }
                }

                //if (course != null)
                //    PerfFrame.SaveXCourse(ref csvStrm, Path.GetFileName(filename), course);
            }
				catch (Exception exc)  {
                Debug.WriteLine(exc.Message);
            }
            csvStrm.CloseCSVFileOut();
			}								// ExportCSVFromLoadedFile()

			/**********************************************************************************************

			**********************************************************************************************/

			public void SaveReportFromFile(BackgroundWorker bw, string filename, StatFlags exportFlags)  {
				if (LoadRawTemps(bw, filename)) {
                SaveReportFromLoadedFile(bw, exportFlags);
        }
			}

			/**********************************************************************************************

			**********************************************************************************************/

			public void SaveReportFromLoadedFile(BackgroundWorker bw, StatFlags exportFlags)  {
            //ReportCols = reportCols;
            //ExportFlags = ReportCols.StatFlags;

            PerfFrame.PerfData datapd = PerfFrame.PerfDataCreate();
            bool bStart = true;
            double lastTimeAcc = 0;
            float timeElapse = 0;
            uint reportCnt = 0;
            m_LapDistance = LoadedRMX.course.TotalX;
            // Initialize
            CalcReport(ref datapd, ref m_datapdAcc, ref m_datapdMin, ref m_datapdMax, timeElapse, bStart);
            bStart = false;

				foreach (PerfFrame.PerfData n in LoadedFrames)  {
                PerfFrame.UpdateRawLastByFlags(ref datapd, n, (StatFlags)n.StatFlags);
                // Only process if there are more than 5 seconds worth of data
                //if (datapd.TimeAcc > 5.0f)
                {
                    /*
                    if (bStart)
                    {
                        reportCnt = 1;
                        timeElapse = 0;
                        lastTimeAcc = datapd.TimeAcc;
                        CalcReport(ref datapd, ref m_datapdAcc, ref m_datapdMin, ref m_datapdMax, timeElapse, bStart);
                        bStart = false;
                    }
                    else
                     * */
                    {
                        reportCnt++;
                        timeElapse = (float)(datapd.TimeAcc - lastTimeAcc);
                        CalcReport(ref datapd, ref m_datapdAcc, ref m_datapdMin, ref m_datapdMax, timeElapse, bStart);
                    }
                }
            }

            m_datapdAvg = m_datapdAcc;

				if (reportCnt > 1)  {
					if ((1.0 + datapd.Distance) > (m_LapDistance * m_lastLap))  {
                    float lapTime = (float)(datapd.TimeAcc - m_lastLapTime);
                    m_lastLapTime = datapd.TimeAcc;
                    m_LapTimes.Add(lapTime);
                    calcValue(ref m_datapdAcc.LapTime, ref m_datapdMin.LapTime, ref m_datapdMax.LapTime, lapTime, datapd.TimeAcc);
                }
                else
                    m_lastLap--;
                m_datapdAvg.LapTime = m_datapdAcc.LapTime;

                calcAvg(ref m_datapdAvg, reportCnt);
            }


            string name = Path.GetFileNameWithoutExtension(m_loadedfilename);
            string reportfilename = RacerMatePaths.ReportsFullPath + "\\" + name + ".html";

            ReportStream reportStrm = null;
            reportStrm = new ReportStream();

				try  {
                Rider rider = new Rider();
                reportStrm.OpenReportFileOut(reportfilename);
                //reportStrm.DoReport(ref LoadedRMX, ref m_datapdAcc, ref m_datapdAvg, ref m_datapdMin, ref m_datapdMax, exportFlags);
                reportStrm.DoReport(ref LoadedRMX, this, exportFlags);
            }
				catch (Exception exc)  {
                Debug.WriteLine(exc.Message);
            }
            reportStrm.CloseReportFileOut();
			}									// SaveReportFromLoadedFile()

			/**********************************************************************************************

			**********************************************************************************************/

			public bool LoadRawTemps(string rawhdrfilename)  {
            return LoadRawTemps(null, rawhdrfilename);
        }

			/**********************************************************************************************

			**********************************************************************************************/

			public bool LoadRawTemps(BackgroundWorker bw, string filename)   {
            try
            {
                m_loadedfilename = filename;
                if (!File.Exists(m_loadedfilename))
                    return false;

                int count = 0, initcount = 0;
                m_rawStrm.OpenRawFileIn(filename);

                if (!LoadedRMX.Header.Read(m_rawStrm))
                    return false;
                if(!LoadedRMX.Info.Read(m_rawStrm,LoadedRMX.Header.Version))
                    return false;

                initcount = LoadedRMX.Info.PerfCount;
                count = 0;
                PerfFrame.PerfData datapd = PerfFrame.PerfDataCreate();
                PerfFile.CPerfRawData datacprd = new PerfFile.CPerfRawData();

				PerfAverages average = new PerfAverages();
				double lasttime = 0;
                if (initcount > 0)
                {
					// Make sure the percentcount will never get to zero if bw is null
					int percentcount = bw == null ? initcount+10:initcount / 100;
					if (percentcount < 100)
						percentcount = 100;	// For really short files... don't do that many
					int pcnt = percentcount;

                    for (count = 0; count < initcount; count++)
                    {
						if (pcnt-- <= 0)
						{
							pcnt = percentcount;
							if (bw.CancellationPending)
							{
								m_rawStrm.CloseRawFileIn();
								return false;	// canceled... exit.
							}
							bw.ReportProgress(count * 100 / initcount);
						}
                        //if (count >= initcount)
                        //    break;
                        if (bw != null && bw.CancellationPending)
                            throw new Exception("BackgroundWorker cancelled.");

                        datacprd.Read(m_rawStrm,LoadedRMX.Header.Version);
                        PerfFrame.PerfData npd = PerfFrame.PerfDataCreate();
                        datacprd.ToPerfData(ref datapd);
						PerfFrame.CopyTo(ref npd, ref datapd);
						npd.StatFlags |= (ulong)(average.UpdateAverages(ref npd, npd.TimeAcc - lasttime ));
                        PerfFrame.UpdateRawLastByFlags(ref datapd, npd, (StatFlags)npd.StatFlags);
                        if (count == 0)
                        {
                            LoadedFrames.Clear();
                            LoadedFrames.Capacity = initcount;
                        }
                        LoadedFrames.Add(npd);
						lasttime = npd.TimeAcc;
                    }
                }
                if (!LoadedRMX.course.LoadRMPCourse(m_rawStrm, LoadedRMX.Header.Version))
                    return false;
				CourseInfo = new CourseInfo(LoadedRMX.course);
				setKey(filename,initcount); // Too keep track of what performances are loaded and in memory.
			}
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                m_rawStrm.CloseRawFileIn();
                return false;
            }

            m_rawStrm.CloseRawFileIn();
            return true;
        }

        public bool LoadHeaderRawTemps(string rawhdrfilename)
        {
            return LoadHeaderRawTemps(null, rawhdrfilename);
        }

		static Course ms_TempCourse;
        public bool LoadHeaderRawTemps(BackgroundWorker bw, string filename)
        {
            try
            {
                m_rawStrm.OpenRawFileIn(filename);
                if (!LoadedRMX.Header.Read(m_rawStrm))
                    return false;
                if (!LoadedRMX.Info.Read(m_rawStrm, LoadedRMX.Header.Version))
                    return false;
				m_rawStrm.SetCurRawFieldInPos(LoadedRMX.Info.CourseOffset);
				if (ms_TempCourse == null)
					ms_TempCourse = new Course();
				ms_TempCourse.hdrVersion = LoadedRMX.Header.Version;
                if (!ms_TempCourse.LoadRMPCourse(m_rawStrm, false, true, LoadedRMX.Header.Version))
					return false;
				Course c;
				if (!Courses.TrackDB.TryGetValue(ms_TempCourse.CourseHash, out c))
				{
					c = new Course();
					m_rawStrm.SetCurRawFieldInPos(LoadedRMX.Info.CourseOffset);
                    if (!c.LoadRMPCourse(m_rawStrm, true, false, LoadedRMX.Header.Version))
						return false;
					c.FileName = filename;
					Courses.TrackDB[c.CourseHash] = c;
					CourseInfo = new CourseInfo(c);
				}
				else
				{
					CourseInfo = new CourseInfo(ms_TempCourse);
					CourseInfo.FileName = filename;
				}
			}
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                m_rawStrm.CloseRawFileIn();
                return false;
            }
			

            m_rawStrm.CloseRawFileIn();
            return true;
        }

/*
        public bool LoadPerfFromXML(string filename)
        {
            return LoadPerfFromXML(null, filename);
        }

        public bool LoadPerfFromXML(BackgroundWorker bw, string filename)
        {
            bool result = true;
            try
            {
                XDocument xdoc = XDocument.Load(filename);
                XElement rootNode = xdoc.Root; // xtag.RMX

                XElement node = rootNode.Element(xtag.Header);
                PerfFile.CRMHeader hdr = new PerfFile.CRMHeader
                {
                    Version = float.Parse(node.Element(xtag.Version).Value),
                    Date = DateTime.Parse(node.Element(xtag.Date).Value),
                    CreatorExe = node.Element(xtag.CreatorExe).Value,
                    Comment = node.Element(xtag.Comment).Value,
                    Copyright = node.Element(xtag.Copyright).Value,
                    CompressType = Int32.Parse(node.Element(xtag.CompressType).Value)
                };
                LoadedRMX.Header = hdr;

                XElement ele = rootNode.Element(xtag.Info);
                PerfFile.CPerfInfo Info = new PerfFile.CPerfInfo();
                PerfFrame.PerfInfoFromXML(ref Info, ele);
                LoadedRMX.Info = Info;

                XElement eleA = rootNode.Element(xtag.Data);
                int count = int.Parse(eleA.Attribute(xtag.Count).Value);
                StatFlags DataFlags = (StatFlags)UInt64.Parse(eleA.Attribute(xtag.DataFlags).Value);
                //LoadedFlags = DataFlags;

                IEnumerable<XElement> eleArr = eleA.Elements();
                int i = 0;
                PerfFrame.PerfData datapd = PerfFrame.PerfDataCreate();
                foreach (XElement el in eleArr)
                {
                    if (bw != null && bw.CancellationPending)
                        throw new Exception("BackgroundWorker cancelled.");

                    PerfFrame.PerfData npd = PerfFrame.PerfDataFromXML(ref datapd, DataFlags, el);
                    if (i == 0)
                        LoadedFrames.Clear();
                    LoadedFrames.Add(npd);
                    i++;
                }

                LoadedRMX.course = new Course();
                LoadedRMX.course.LoadXCourse(ref rootNode);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                result = false;
            }
            return result;
        }
        public static RMX LoadRMX(string filename)
        {
            RMX rmx = new RMX();
            try
            {
                XElement node = null;

                StreamReader strm = null;
                strm = new StreamReader(filename);
                XmlTextReader reader = new XmlTextReader(strm);
                reader.ReadStartElement(xtag.RMX);
                node = LoadXElement(ref reader, xtag.Header);
                if (node != null)
                {
                    rmx.Header = new RMPHeader
                    {
                        Version = Int32.Parse(node.Element(xtag.Version).Value),
                        Date = DateTime.Parse(node.Element(xtag.Date).Value),
                        CreatorExe = node.Element(xtag.CreatorExe).Value,
                        Comment = node.Element(xtag.Comment).Value,
                        Copyright = node.Element(xtag.Copyright).Value,
                        CompressType = Int32.Parse(node.Element(xtag.CompressType).Value)
                    };
                }
                node = LoadXElement(ref reader, xtag.Info);
                if (node != null)
                {
                    rmx.Info = new PerfInfo();
                    PerfInfoFromXML(ref rmx.Info, node);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
            return rmx;
        }
         */
#endif
    }
    public static class PerfFrame
    {
#if D3DHOST
#else

        /*
        // Must match StatFlags
        //
        public enum SFI : ulong // StatFlagsIndex
        {
            //Zero = 0L,
            Time = 0, // = (1L << 0),		// Total time of the race.
            LapTime, // = (1L << 1),		// Current running total of the lap
            Lap, // = (1L << 2),
            Distance, // = (1L << 3),		// Total Distance run. 
            Lead, // = (1L << 4),		// Race order has changed
            Grade, // = (1L << 5),
            Wind, // = (1L << 6),

            Speed, // = (1L << 7),
            Speed_Avg, // = (1L << 8),
            Speed_Max, // = (1L << 9),

            Watts, // = (1L << 10),
            Watts_Avg, // = (1L << 11),
            Watts_Max, // = (1L << 12),
            Watts_Wkg, // = (1L << 13),
            Watts_Load, // = (1L << 14),

            HeartRate, // = (1L << 15),
            HeartRate_Avg, // = (1L << 16),
            HeartRate_Max, // = (1L << 17),

            Cadence, // = (1L << 18),
            Cadence_Avg, // = (1L << 19),
            Cadence_Max, // = (1L << 20),

            Calories, // = (1L << 21),
            PulsePower, // = (1L << 22),
            DragFactor, // = (1L << 23),
            SS, // = (1L << 24),
            SSLeft, // = (1L << 25),
            SSRight, // = (1L << 26),
            SSLeftSplit, // = (1L << 27),
            SSRightSplit, // = (1L << 28),
            SSLeftATA, // = (1L << 29),
            SSRightATA, // = (1L << 30),
            SSLeft_Avg, // = (1L << 31),
            SSRight_Avg, // = (1L << 32),

            LeftPower, // = (1L << 33),
            RightPower, // = (1L << 34),
            PercentAT, // = (1L << 35),
            FrontGear, // = (1L << 36),
            RearGear, // = (1L << 37),
            GearInches, // = (1L << 38),
            RawSpinScan, // = (1L << 39),
            CadenceTiming, // = (1L << 40),
            TSS, // = (1L << 41),
            IF, // = (1L << 42),
            NP, // = (1L << 43),

            Bars, // = (1L << 44),
            Bars_Shown, // = (1L << 45),
            Bars_Avg, // = (1L << 46),

            CourseScreenX, // = (1L << 47),

            RiderName, // = (1L << 48),

            Course, // = (1L << 49),

            Order, // = (1L << 50), // Global flag only
            HardwareStatus, // = (1L << 51), // Something changed in the hardware connected to that unit.

            Max, // = (1L << 52)

            TimeMS = 1024,
            SS_Stats, //SS | SSLeft | SSRight | SSLeftSplit | SSRightSplit | SSLeftATA | SSRightATA | SSLeft_Avg | SSRight_Avg,
            Gear, // = FrontGear | RearGear,
            TSS_IF_NP, // = TSS | IF | NP,

            Header,
            InfoFlags,
            Info,
            DataFlags,
            Data,

            //MagicNum,
            //SecMagicNum,
            //HeaderSize,
            CreatorExe,
            Date,
            Version,
            Comment,
            Copyright,
            CompressType

        };
        */
        public struct PerfData
        {
            // 
            // always set for data 
            public UInt64 StatFlags;
            public UInt64 TimeMS;
            public Int32 KeyFrame; // not set for info
            // data that may not be set
            public double TimeAcc;
            public double Distance;
            public Int32 Lap;
            public float LapTime;
            public float Lead;
            public float Grade;
            public float Wind;
            public float Speed;
            public float Speed_Avg;
            public float Speed_Max;
            public float Watts;
            public float Watts_Avg;
            public float Watts_Max;
            public float Watts_Wkg;
            public float Watts_Load;
            public float HeartRate;
            public float HeartRate_Avg;
            public float HeartRate_Max;
            public float Cadence;
            public float Cadence_Avg;
            public float Cadence_Max;
            public float Calories;
            public float PulsePower;
            public float DragFactor;
            public float SS;
            public float SSLeft;
            public float SSRight;
            public float SSLeftSplit;
            public float SSRightSplit;
            public float SSLeftATA;
            public float SSRightATA;
            public float SSLeft_Avg;
            public float SSRight_Avg;
            public float LeftPower;
            public float RightPower;
            public float PercentAT;
            public Int32 FrontGear;
            public Int32 RearGear;
            public Int32 GearInches;
            public float RawSpinScan;
            public float CadenceTiming;
            public float TSS;
            public float IF;
            public float NP;
            public Int32 Bars_Shown; // bool
            public float[] Bars; // 24
            public float[] AverageBars; // 24

            public Int32 CourseScreenX;
            public string RiderName;
            public string Course;
            //
            public float SS_Stats; // = SS | SSLeft | SSRight | SSLeftSplit | SSRightSplit | SSLeftATA | SSRightATA | SSLeft_Avg | SSRight_Avg,
            public float Gear; // = FrontGear | RearGear,
            public float TSS_IF_NP; // = TSS | IF | NP,

            // added for header version - v1.02
            public bool Drafting;
            public bool Disconnected;
            public Int16 RawCalibrationValue;
            //public bool HardwareStatus;
        };

			/*************************************************************************************************

			*************************************************************************************************/

			static public PerfData PerfDataCreate()  {
				//xxx
            return new PerfData { StatFlags = (UInt64)StatFlags.Mask, Bars = new float[24], AverageBars = new float[24] };
			}								// PerfDataCreate()


		static public void CopyTo(ref PerfData pnew, ref PerfData pold)
		{
			float[] bars = pnew.Bars;
			float[] abars = pnew.AverageBars;
			pnew = pold;
			Array.Copy(pold.Bars, bars, 24);
			Array.Copy(pold.AverageBars, abars, 24);
			pnew.Bars = bars;
			pnew.AverageBars = abars;
		}

        public struct RMX
        {
            public PerfFile.CRMHeader Header;
            public PerfFile.CPerfInfo Info;
            //public RMPHeader Header;
            //public PerfInfo Info;
            public UInt64 DataFlags;
            public PerfData[] Data;
            public Course course;
        }
        // static Random _r = new Random(); // **** test *****

			/*************************************************************************************************

			*************************************************************************************************/

			public static void UpdateLastByFlags(ref PerfData pd, StatFlags changedflags, Statistics statistics, Int32 keyframe)  {
            double convertSpeed = 1.0;
            //if (!statistics.Metric)
            //    convertSpeed = ConvertConst.MilesToKilometers;


            pd.StatFlags = (UInt64)changedflags;
            pd.TimeMS = (UInt64)(statistics.Time * 1000);
            //pd.KeyFrame = 0;

            if ((changedflags & StatFlags.Time) != StatFlags.Zero) { pd.TimeAcc = statistics.Time; }
            if ((changedflags & StatFlags.LapTime) != StatFlags.Zero) { pd.LapTime = (float)statistics.LapTime; }
            if ((changedflags & StatFlags.Lap) != StatFlags.Zero) { pd.Lap = statistics.Lap; }
            if ((changedflags & StatFlags.Distance) != StatFlags.Zero) { pd.Distance = statistics.Distance / convertSpeed; }
            //if ((changedflags & StatFlags.Lead) != StatFlags.Zero) { pd.Lead = statistics.Lead); }
            if ((changedflags & StatFlags.Grade) != StatFlags.Zero) { pd.Grade = statistics.Grade; }
            if ((changedflags & StatFlags.Wind) != StatFlags.Zero) { pd.Wind = statistics.Wind; }
            if ((changedflags & StatFlags.Speed) != StatFlags.Zero) {
					pd.Speed = (float)(statistics.Speed / convertSpeed);
				}
            if ((changedflags & StatFlags.Speed_Avg) != StatFlags.Zero) { pd.Speed_Avg = (float)(statistics.Speed_Avg / convertSpeed); }
            if ((changedflags & StatFlags.Speed_Max) != StatFlags.Zero) { pd.Speed_Max = (float)(statistics.Speed_Max / convertSpeed); }
            if ((changedflags & StatFlags.Watts) != StatFlags.Zero) { pd.Watts = statistics.Watts; }
            if ((changedflags & StatFlags.Watts_Avg) != StatFlags.Zero) { pd.Watts_Avg = statistics.Watts_Avg; }
            if ((changedflags & StatFlags.Watts_Max) != StatFlags.Zero) { pd.Watts_Max = statistics.Watts_Max; }
            //if ((changedflags & StatFlags.Watts_Wkg) != StatFlags.Zero) { pd.Watts_Wkg = statistics.Watts_Wkg; }
            if ((changedflags & StatFlags.Watts_Load) != StatFlags.Zero) { pd.Watts_Load = statistics.Watts_Load; }
            if ((changedflags & StatFlags.HeartRate) != StatFlags.Zero) { pd.HeartRate = statistics.HeartRate; }
            if ((changedflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) { pd.HeartRate_Avg = statistics.HeartRate_Avg; }
            if ((changedflags & StatFlags.HeartRate_Max) != StatFlags.Zero) { pd.HeartRate_Max = statistics.HeartRate_Max; }
            if ((changedflags & StatFlags.Cadence) != StatFlags.Zero) { pd.Cadence = statistics.Cadence; }
            if ((changedflags & StatFlags.Cadence_Avg) != StatFlags.Zero) { pd.Cadence_Avg = statistics.Cadence_Avg; }
            if ((changedflags & StatFlags.Cadence_Max) != StatFlags.Zero) { pd.Cadence_Max = statistics.Cadence_Max; }
            if ((changedflags & StatFlags.Calories) != StatFlags.Zero) { pd.Calories = statistics.Calories; }
            if ((changedflags & StatFlags.PulsePower) != StatFlags.Zero) { pd.PulsePower = statistics.PulsePower; }
            if ((changedflags & StatFlags.DragFactor) != StatFlags.Zero) { pd.DragFactor = statistics.DragFactor; }
            if ((changedflags & StatFlags.SS) != StatFlags.Zero) { pd.SS = statistics.SS; }
            if ((changedflags & StatFlags.SSLeft) != StatFlags.Zero) { pd.SSLeft = statistics.SSLeft; }
            if ((changedflags & StatFlags.SSRight) != StatFlags.Zero) { pd.SSRight = statistics.SSRight; }
            if ((changedflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { pd.SSLeftSplit = statistics.SSLeftSplit; }
            if ((changedflags & StatFlags.SSRightSplit) != StatFlags.Zero) { pd.SSRightSplit = statistics.SSRightSplit; }
            if ((changedflags & StatFlags.SSLeftATA) != StatFlags.Zero) { pd.SSLeftATA = statistics.SSLeftATA; }
            if ((changedflags & StatFlags.SSRightATA) != StatFlags.Zero) { pd.SSRightATA = statistics.SSRightATA; }
            if ((changedflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) { pd.SSLeft_Avg = statistics.SSLeft_Avg; }
            if ((changedflags & StatFlags.SSRight_Avg) != StatFlags.Zero) { pd.SSRight_Avg = statistics.SSRight_Avg; }
            //if ((changedflags & StatFlags.LeftPower) != StatFlags.Zero) { pd.LeftPower = statistics.LeftPower; }
            //if ((changedflags & StatFlags.RightPower) != StatFlags.Zero) { pd.RightPower = statistics.RightPower; }
            //if ((changedflags & StatFlags.PercentAT) != StatFlags.Zero) { pd.PercentAT = statistics.PercentAT; }
            if ((changedflags & StatFlags.FrontGear) != StatFlags.Zero) { pd.FrontGear = statistics.FrontGear; }
            if ((changedflags & StatFlags.RearGear) != StatFlags.Zero) { pd.RearGear = statistics.RearGear; }
            if ((changedflags & StatFlags.GearInches) != StatFlags.Zero) { pd.GearInches = statistics.GearInches; }
            //if ((changedflags & StatFlags.RawSpinScan) != StatFlags.Zero) { pd.RawSpinScan = statistics.RawSpinScan; }
            //if ((changedflags & StatFlags.CadenceTiming) != StatFlags.Zero) { pd.CadenceTiming = statistics.CadenceTiming; }
            if ((changedflags & StatFlags.TSS) != StatFlags.Zero) { pd.TSS = statistics.TSS; }
            if ((changedflags & StatFlags.IF) != StatFlags.Zero) { pd.IF = statistics.IF; }
            if ((changedflags & StatFlags.NP) != StatFlags.Zero) { pd.NP = statistics.NP; }
            if ((changedflags & StatFlags.Bars_Shown) != StatFlags.Zero) { pd.Bars_Shown = (statistics.Bars_Shown ? 1 : 0); }
				if ((changedflags & StatFlags.Bars) != StatFlags.Zero)  {
                pd.Bars = statistics.Bars; 
            }
            if ((changedflags & StatFlags.Bars_Avg) != StatFlags.Zero) { 
                pd.AverageBars = statistics.AverageBars; 
            }
            //if ((changedflags & StatFlags.CourseScreenX) != StatFlags.Zero) { pd.CourseScreenX = statistics.CourseScreenX; }
            if ((changedflags & StatFlags.RiderName) != StatFlags.Zero) { pd.RiderName = statistics.RiderName; }
            //if ((changedflags & StatFlags.Course) != StatFlags.Zero) { pd.Course = statistics.Course; }

            // Added v1.02
            if ((changedflags & StatFlags.Disconnected) != StatFlags.Zero) { pd.Disconnected = statistics.Disconnected; }
            if ((changedflags & StatFlags.Drafting) != StatFlags.Zero) { pd.Drafting = statistics.Drafting; }
            if ((changedflags & StatFlags.Calibration) != StatFlags.Zero) { pd.RawCalibrationValue = statistics.RawCalibrationValue; }

            //if ((changedflags & StatFlags.Calibration) != StatFlags.Zero) { pd.RawCalibrationValue = (Int16)_r.Next(500); } // **** test *******
            //if ((changedflags & StatFlags.HardwareStatus) != StatFlags.Zero) { pd.HardwareStatus = statistics.HardwareStatus; }

				return;
			}								// UpdateLastByFlags()

#if EXPORTXML
        public static void PerfInfoWriteXML(ref XmlStream xmlStrm, ref PerfFile.CPerfInfo pih)
        {
            xmlStrm.AddXElementStart(xtag.Info, 1);

            xmlStrm.AddXField(xtag.RiderName, pih.RiderName);
            xmlStrm.AddXField(xtag.Gender, pih.Gender);
            xmlStrm.AddXField(xtag.Age, pih.Age);
            xmlStrm.AddXField(xtag.Height, pih.Height);
            xmlStrm.AddXField(xtag.Weight, pih.Weight);
            xmlStrm.AddXField(xtag.HeartRate, pih.HeartRate);
            xmlStrm.AddXField(xtag.Upper_HeartRate, pih.Upper_HeartRate);
            xmlStrm.AddXField(xtag.Lower_HeartRate, pih.Lower_HeartRate);
            xmlStrm.AddXField(xtag.CourseName, pih.CourseName);
            xmlStrm.AddXField(xtag.CourseType, pih.CourseType);
            xmlStrm.AddXField(xtag.RFDrag, pih.RFDrag);
            xmlStrm.AddXField(xtag.RFMeas, pih.RFMeas);
            xmlStrm.AddXField(xtag.Watts_Factor, pih.Watts_Factor);
            xmlStrm.AddXField(xtag.FTP, pih.FTP);
            xmlStrm.AddXField(xtag.PerfCount, pih.PerfCount);
            xmlStrm.AddXField(xtag.TimeMS, pih.TimeMS);

            xmlStrm.AddXElementEnd(xtag.Info, -1);
        }

        public static void PerfInfoFromXML(ref PerfFile.CPerfInfo pih, XElement ele)
        {
            pih.RiderName = ele.Element(xtag.RiderName).Value;
            pih.Gender = ele.Element(xtag.Gender).Value;
            pih.Age = Int32.Parse(ele.Element(xtag.Age).Value);
            pih.Height = float.Parse(ele.Element(xtag.Height).Value);
            pih.Weight = float.Parse(ele.Element(xtag.Weight).Value);
            pih.HeartRate = float.Parse(ele.Element(xtag.HeartRate).Value);
            pih.Upper_HeartRate = float.Parse(ele.Element(xtag.Upper_HeartRate).Value);
            pih.Lower_HeartRate = float.Parse(ele.Element(xtag.Lower_HeartRate).Value);
            pih.CourseName = ele.Element(xtag.CourseName).Value;
            pih.CourseType = UInt32.Parse(ele.Element(xtag.CourseType).Value);
            pih.RFDrag = float.Parse(ele.Element(xtag.RFDrag).Value);
            pih.RFMeas = float.Parse(ele.Element(xtag.RFMeas).Value);
            pih.Watts_Factor = float.Parse(ele.Element(xtag.Watts_Factor).Value);
            pih.FTP = float.Parse(ele.Element(xtag.FTP).Value);
            pih.PerfCount = Int32.Parse(ele.Element(xtag.PerfCount).Value);
            pih.TimeMS = UInt64.Parse(ele.Element(xtag.TimeMS).Value);
        }
        public static void WriteXMLByFlags(ref XmlStream xmlStrm, StatFlags flags, ref PerfData pd, Int32 keyframe, string tagname)
        {
            StatFlags changedflags = (StatFlags)pd.StatFlags;
            if (tagname != null)
            {
                if (keyframe > 0)
                {
                    xmlStrm.AddXElementStart("" + tagname + " " + xtag.KeyFrame + "=\"" + "true" + "\"", 1);
                    changedflags |= flags;
                }
                else
                    xmlStrm.AddXElementStart(tagname, 1);
            }
            else
            {
                changedflags |= flags;
            }



            xmlStrm.AddXField(xtag.StatFlags, pd.StatFlags);
            xmlStrm.AddXField(xtag.TimeMS, pd.TimeMS);

            if ((changedflags & StatFlags.Time) != StatFlags.Zero) { xmlStrm.AddXField(xtag.TimeAcc, pd.TimeAcc); }
            if ((changedflags & StatFlags.LapTime) != StatFlags.Zero) { xmlStrm.AddXField(xtag.LapTime, pd.LapTime); }
            if ((changedflags & StatFlags.Lap) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Lap, pd.Lap); }
            if ((changedflags & StatFlags.Distance) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Distance, pd.Distance); }
            if ((changedflags & StatFlags.Lead) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Lead, pd.Lead); }
            if ((changedflags & StatFlags.Grade) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Grade, pd.Grade); }
            if ((changedflags & StatFlags.Wind) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Wind, pd.Wind); }
            if ((changedflags & StatFlags.Speed) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Speed, pd.Speed); }
            if ((changedflags & StatFlags.Speed_Avg) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Speed_Avg, pd.Speed_Avg); }
            if ((changedflags & StatFlags.Speed_Max) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Speed_Max, pd.Speed_Max); }
            if ((changedflags & StatFlags.Watts) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Watts, pd.Watts); }
            if ((changedflags & StatFlags.Watts_Avg) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Watts_Avg, pd.Watts_Avg); }
            if ((changedflags & StatFlags.Watts_Max) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Watts_Max, pd.Watts_Max); }
            if ((changedflags & StatFlags.Watts_Wkg) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Watts_Wkg, pd.Watts_Wkg); }
            if ((changedflags & StatFlags.Watts_Load) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Watts_Load, pd.Watts_Load); }
            if ((changedflags & StatFlags.HeartRate) != StatFlags.Zero) { xmlStrm.AddXField(xtag.HeartRate, pd.HeartRate); }
            if ((changedflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) { xmlStrm.AddXField(xtag.HeartRate_Avg, pd.HeartRate_Avg); }
            if ((changedflags & StatFlags.HeartRate_Max) != StatFlags.Zero) { xmlStrm.AddXField(xtag.HeartRate_Max, pd.HeartRate_Max); }
            if ((changedflags & StatFlags.Cadence) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Cadence, pd.Cadence); }
            if ((changedflags & StatFlags.Cadence_Avg) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Cadence_Avg, pd.Cadence_Avg); }
            if ((changedflags & StatFlags.Cadence_Max) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Cadence_Max, pd.Cadence_Max); }
            if ((changedflags & StatFlags.Calories) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Calories, pd.Calories); }
            if ((changedflags & StatFlags.PulsePower) != StatFlags.Zero) { xmlStrm.AddXField(xtag.PulsePower, pd.PulsePower); }
            if ((changedflags & StatFlags.DragFactor) != StatFlags.Zero) { xmlStrm.AddXField(xtag.DragFactor, pd.DragFactor); }
            if ((changedflags & StatFlags.SS) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SS, pd.SS); }
            if ((changedflags & StatFlags.SSLeft) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SSLeft, pd.SSLeft); }
            if ((changedflags & StatFlags.SSRight) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SSRight, pd.SSRight); }
            if ((changedflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SSLeftSplit, pd.SSLeftSplit); }
            if ((changedflags & StatFlags.SSRightSplit) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SSRightSplit, pd.SSRightSplit); }
            if ((changedflags & StatFlags.SSLeftATA) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SSLeftATA, pd.SSLeftATA); }
            if ((changedflags & StatFlags.SSRightATA) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SSRightATA, pd.SSRightATA); }
            if ((changedflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SSLeft_Avg, pd.SSLeft_Avg); }
            if ((changedflags & StatFlags.SSRight_Avg) != StatFlags.Zero) { xmlStrm.AddXField(xtag.SSRight_Avg, pd.SSRight_Avg); }
            //if ((changedflags & StatFlags.LeftPower) != StatFlags.Zero) { xmlStrm.AddXField(xtag.LeftPower, pd.LeftPower); }
            //if ((changedflags & StatFlags.RightPower) != StatFlags.Zero) { xmlStrm.AddXField(xtag.RightPower, pd.RightPower); }
            //if ((changedflags & StatFlags.PercentAT) != StatFlags.Zero) { xmlStrm.AddXField(xtag.PercentAT, pd.PercentAT); }
            if ((changedflags & StatFlags.FrontGear) != StatFlags.Zero) { xmlStrm.AddXField(xtag.FrontGear, pd.FrontGear); }
            if ((changedflags & StatFlags.RearGear) != StatFlags.Zero) { xmlStrm.AddXField(xtag.RearGear, pd.RearGear); }
            if ((changedflags & StatFlags.GearInches) != StatFlags.Zero) { xmlStrm.AddXField(xtag.GearInches, pd.GearInches); }
            //if ((changedflags & StatFlags.RawSpinScan) != StatFlags.Zero) { xmlStrm.AddXField(xtag.RawSpinScan, pd.RawSpinScan); }
            //if ((changedflags & StatFlags.CadenceTiming) != StatFlags.Zero) { xmlStrm.AddXField(xtag.CadenceTiming, pd.CadenceTiming); }
            if ((changedflags & StatFlags.TSS) != StatFlags.Zero) { xmlStrm.AddXField(xtag.TSS, pd.TSS); }
            if ((changedflags & StatFlags.IF) != StatFlags.Zero) { xmlStrm.AddXField(xtag.IF, pd.IF); }
            if ((changedflags & StatFlags.NP) != StatFlags.Zero) { xmlStrm.AddXField(xtag.NP, pd.NP); }
            if ((changedflags & StatFlags.Bars) != StatFlags.Zero)
            {
                xmlStrm.AddXElementStart(xtag.Bars + " " + xtag.Count + "=\"" + pd.Bars.Length + "\"");
                xmlStrm.AddXValue(pd.Bars, null);
                xmlStrm.AddXElementEnd(xtag.Bars);
            }
            if ((changedflags & StatFlags.Bars_Shown) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Bars_Shown, pd.Bars_Shown); }
            if ((changedflags & StatFlags.Bars_Avg) != StatFlags.Zero)
            {
                xmlStrm.AddXElementStart(xtag.AverageBars + " " + xtag.Count + "=\"" + pd.AverageBars.Length + "\"");
                xmlStrm.AddXValue(pd.AverageBars, null);
                xmlStrm.AddXElementEnd(xtag.AverageBars);
            }
            if ((changedflags & StatFlags.CourseScreenX) != StatFlags.Zero) { xmlStrm.AddXField(xtag.CourseScreenX, pd.CourseScreenX); }
            if ((changedflags & StatFlags.RiderName) != StatFlags.Zero) { xmlStrm.AddXField(xtag.RiderName, pd.RiderName); }
            //if ((changedflags & StatFlags.Course) != StatFlags.Zero) { xmlStrm.AddXField(xtag.Course, pd.Course); }

            if (tagname != null)
                xmlStrm.AddXElementEnd(tagname, -1);
        }

        public static void WriteXMLAttrByFlags(ref XmlStream xmlStrm, StatFlags flags, ref PerfData pd, Int32 keyframe, string tagname)
        {
            StatFlags changedflags = (StatFlags)pd.StatFlags;
            string atts = "";
            if (tagname == null)
                return;

            if (keyframe > 0)
            {
                xmlStrm.AddXAttrib(ref atts, xtag.KeyFrame, "true");
                changedflags |= flags;
            }

            xmlStrm.AddXAttrib(ref atts, xtag.StatFlags, pd.StatFlags);
            xmlStrm.AddXAttrib(ref atts, xtag.TimeMS, pd.TimeMS);

            if ((changedflags & StatFlags.Time) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.TimeAcc, pd.TimeAcc); }
            if ((changedflags & StatFlags.LapTime) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.LapTime, pd.LapTime); }
            if ((changedflags & StatFlags.Lap) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Lap, pd.Lap); }
            if ((changedflags & StatFlags.Distance) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Distance, pd.Distance); }
            if ((changedflags & StatFlags.Lead) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Lead, pd.Lead); }
            if ((changedflags & StatFlags.Grade) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Grade, pd.Grade); }
            if ((changedflags & StatFlags.Wind) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Wind, pd.Wind); }
            if ((changedflags & StatFlags.Speed) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Speed, pd.Speed); }
            if ((changedflags & StatFlags.Speed_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Speed_Avg, pd.Speed_Avg); }
            if ((changedflags & StatFlags.Speed_Max) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Speed_Max, pd.Speed_Max); }
            if ((changedflags & StatFlags.Watts) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Watts, pd.Watts); }
            if ((changedflags & StatFlags.Watts_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Watts_Avg, pd.Watts_Avg); }
            if ((changedflags & StatFlags.Watts_Max) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Watts_Max, pd.Watts_Max); }
            if ((changedflags & StatFlags.Watts_Wkg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Watts_Wkg, pd.Watts_Wkg); }
            if ((changedflags & StatFlags.Watts_Load) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Watts_Load, pd.Watts_Load); }
            if ((changedflags & StatFlags.HeartRate) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.HeartRate, pd.HeartRate); }
            if ((changedflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.HeartRate_Avg, pd.HeartRate_Avg); }
            if ((changedflags & StatFlags.HeartRate_Max) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.HeartRate_Max, pd.HeartRate_Max); }
            if ((changedflags & StatFlags.Cadence) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Cadence, pd.Cadence); }
            if ((changedflags & StatFlags.Cadence_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Cadence_Avg, pd.Cadence_Avg); }
            if ((changedflags & StatFlags.Cadence_Max) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Cadence_Max, pd.Cadence_Max); }
            if ((changedflags & StatFlags.Calories) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Calories, pd.Calories); }
            if ((changedflags & StatFlags.PulsePower) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.PulsePower, pd.PulsePower); }
            if ((changedflags & StatFlags.DragFactor) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.DragFactor, pd.DragFactor); }
            if ((changedflags & StatFlags.SS) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SS, pd.SS); }
            if ((changedflags & StatFlags.SSLeft) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SSLeft, pd.SSLeft); }
            if ((changedflags & StatFlags.SSRight) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SSRight, pd.SSRight); }
            if ((changedflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SSLeftSplit, pd.SSLeftSplit); }
            if ((changedflags & StatFlags.SSRightSplit) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SSRightSplit, pd.SSRightSplit); }
            if ((changedflags & StatFlags.SSLeftATA) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SSLeftATA, pd.SSLeftATA); }
            if ((changedflags & StatFlags.SSRightATA) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SSRightATA, pd.SSRightATA); }
            if ((changedflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SSLeft_Avg, pd.SSLeft_Avg); }
            if ((changedflags & StatFlags.SSRight_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.SSRight_Avg, pd.SSRight_Avg); }
            //if ((changedflags & StatFlags.LeftPower) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.LeftPower, pd.LeftPower); }
            //if ((changedflags & StatFlags.RightPower) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.RightPower, pd.RightPower); }
            //if ((changedflags & StatFlags.PercentAT) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.PercentAT, pd.PercentAT); }
            if ((changedflags & StatFlags.FrontGear) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.FrontGear, pd.FrontGear); }
            if ((changedflags & StatFlags.RearGear) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.RearGear, pd.RearGear); }
            if ((changedflags & StatFlags.GearInches) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.GearInches, pd.GearInches); }
            //if ((changedflags & StatFlags.RawSpinScan) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.RawSpinScan, pd.RawSpinScan); }
            //if ((changedflags & StatFlags.CadenceTiming) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.CadenceTiming, pd.CadenceTiming); }
            if ((changedflags & StatFlags.TSS) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.TSS, pd.TSS); }
            if ((changedflags & StatFlags.IF) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.IF, pd.IF); }
            if ((changedflags & StatFlags.NP) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.NP, pd.NP); }

            if ((changedflags & StatFlags.Bars_Shown) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Bars_Shown, pd.Bars_Shown); }
            if ((changedflags & StatFlags.Bars) != StatFlags.Zero)
            {
                string arrBars = "";
                int icnt = 0;
                foreach (float arg in pd.Bars)
                {
                    if (icnt == 0)
                        arrBars = arrBars + arg;
                    else
                        arrBars = arrBars + "," + arg;
                    icnt++;
                }
                xmlStrm.AddXAttrib(ref atts, xstag.Bars, arrBars);
            }
            if ((changedflags & StatFlags.Bars_Avg) != StatFlags.Zero)
            {
                string arrBars = "";
                int icnt = 0;
                foreach (float arg in pd.AverageBars)
                {
                    if (icnt == 0)
                        arrBars = arrBars + arg;
                    else
                        arrBars = arrBars + "," + arg;
                    icnt++;
                }
                xmlStrm.AddXAttrib(ref atts, xstag.AverageBars, arrBars);
            }
            //if ((changedflags & StatFlags.CourseScreenX) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.CourseScreenX, pd.CourseScreenX); }
            //if ((changedflags & StatFlags.RiderName) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.RiderName, pd.RiderName); }
            //if ((changedflags & StatFlags.Course) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xtag.Course, pd.Course); }

            xmlStrm.AddXElement(tagname, atts);
        }

        public static void WriteXMLAttrsByFlags(ref XmlStream xmlStrm, StatFlags flags, ref PerfData pd, Int32 keyframe, string tagname)
        {
            StatFlags changedflags = (StatFlags)pd.StatFlags;
            string atts = "";
            if (tagname == null)
                return;

            if (keyframe > 0)
            {
                xmlStrm.AddXAttrib(ref atts, xstag.KeyFrame, "true");
                changedflags |= flags;
            }

            xmlStrm.AddXAttrib(ref atts, xstag.StatFlags, pd.StatFlags);
            xmlStrm.AddXAttrib(ref atts, xstag.TimeMS, pd.TimeMS);

            if ((changedflags & StatFlags.Time) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.TimeAcc, pd.TimeAcc); }
            if ((changedflags & StatFlags.LapTime) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.LapTime, pd.LapTime); }
            if ((changedflags & StatFlags.Lap) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Lap, pd.Lap); }
            if ((changedflags & StatFlags.Distance) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Distance, pd.Distance); }
            if ((changedflags & StatFlags.Lead) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Lead, pd.Lead); }
            if ((changedflags & StatFlags.Grade) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Grade, pd.Grade); }
            if ((changedflags & StatFlags.Wind) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Wind, pd.Wind); }
            if ((changedflags & StatFlags.Speed) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Speed, pd.Speed); }
            if ((changedflags & StatFlags.Speed_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Speed_Avg, pd.Speed_Avg); }
            if ((changedflags & StatFlags.Speed_Max) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Speed_Max, pd.Speed_Max); }
            if ((changedflags & StatFlags.Watts) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Watts, pd.Watts); }
            if ((changedflags & StatFlags.Watts_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Watts_Avg, pd.Watts_Avg); }
            if ((changedflags & StatFlags.Watts_Max) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Watts_Max, pd.Watts_Max); }
            if ((changedflags & StatFlags.Watts_Wkg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Watts_Wkg, pd.Watts_Wkg); }
            if ((changedflags & StatFlags.Watts_Load) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Watts_Load, pd.Watts_Load); }
            if ((changedflags & StatFlags.HeartRate) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.HeartRate, pd.HeartRate); }
            if ((changedflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.HeartRate_Avg, pd.HeartRate_Avg); }
            if ((changedflags & StatFlags.HeartRate_Max) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.HeartRate_Max, pd.HeartRate_Max); }
            if ((changedflags & StatFlags.Cadence) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Cadence, pd.Cadence); }
            if ((changedflags & StatFlags.Cadence_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Cadence_Avg, pd.Cadence_Avg); }
            if ((changedflags & StatFlags.Cadence_Max) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Cadence_Max, pd.Cadence_Max); }
            if ((changedflags & StatFlags.Calories) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Calories, pd.Calories); }
            if ((changedflags & StatFlags.PulsePower) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.PulsePower, pd.PulsePower); }
            if ((changedflags & StatFlags.DragFactor) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.DragFactor, pd.DragFactor); }
            if ((changedflags & StatFlags.SS) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SS, pd.SS); }
            if ((changedflags & StatFlags.SSLeft) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SSLeft, pd.SSLeft); }
            if ((changedflags & StatFlags.SSRight) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SSRight, pd.SSRight); }
            if ((changedflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SSLeftSplit, pd.SSLeftSplit); }
            if ((changedflags & StatFlags.SSRightSplit) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SSRightSplit, pd.SSRightSplit); }
            if ((changedflags & StatFlags.SSLeftATA) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SSLeftATA, pd.SSLeftATA); }
            if ((changedflags & StatFlags.SSRightATA) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SSRightATA, pd.SSRightATA); }
            if ((changedflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SSLeft_Avg, pd.SSLeft_Avg); }
            if ((changedflags & StatFlags.SSRight_Avg) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.SSRight_Avg, pd.SSRight_Avg); }
            //if ((changedflags & StatFlags.LeftPower) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.LeftPower, pd.LeftPower); }
            //if ((changedflags & StatFlags.RightPower) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.RightPower, pd.RightPower); }
            //if ((changedflags & StatFlags.PercentAT) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.PercentAT, pd.PercentAT); }
            if ((changedflags & StatFlags.FrontGear) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.FrontGear, pd.FrontGear); }
            if ((changedflags & StatFlags.RearGear) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.RearGear, pd.RearGear); }
            if ((changedflags & StatFlags.GearInches) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.GearInches, pd.GearInches); }
            //if ((changedflags & StatFlags.RawSpinScan) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.RawSpinScan, pd.RawSpinScan); }
            //if ((changedflags & StatFlags.CadenceTiming) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.CadenceTiming, pd.CadenceTiming); }
            if ((changedflags & StatFlags.TSS) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.TSS, pd.TSS); }
            if ((changedflags & StatFlags.IF) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.IF, pd.IF); }
            if ((changedflags & StatFlags.NP) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.NP, pd.NP); }

            if ((changedflags & StatFlags.Bars_Shown) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Bars_Shown, pd.Bars_Shown); }
            if ((changedflags & StatFlags.Bars) != StatFlags.Zero)
            {
                string arrBars = "";
                int icnt = 0;
                foreach (float arg in pd.Bars)
                {
                    if(icnt==0)
                        arrBars = arrBars + arg;
                    else
                        arrBars = arrBars + "," + arg;
                    icnt++;
                }
                xmlStrm.AddXAttrib(ref atts, xstag.Bars, arrBars);
            }
            if ((changedflags & StatFlags.Bars_Avg) != StatFlags.Zero)
            {
                string arrBars = "";
                int icnt = 0;
                foreach (float arg in pd.AverageBars)
                {
                    if (icnt == 0)
                        arrBars = arrBars + arg;
                    else
                        arrBars = arrBars + "," + arg;
                    icnt++;
                }
                xmlStrm.AddXAttrib(ref atts, xstag.AverageBars, arrBars);
            }
            //if ((changedflags & StatFlags.CourseScreenX) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.CourseScreenX, pd.CourseScreenX); }
            //if ((changedflags & StatFlags.RiderName) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.RiderName, pd.RiderName); }
            //if ((changedflags & StatFlags.Course) != StatFlags.Zero) { xmlStrm.AddXAttrib(ref atts, xstag.Course, pd.Course); }

            xmlStrm.AddXElement(tagname, atts);
        }

        public static PerfData PerfDataAttrFromXML(ref PerfData pd, StatFlags InfoFlags, XElement ele)
        {
            StatFlags validflags = StatFlags.Zero;
            if (!ele.HasAttributes)
                return pd;
            if ("true" == ele.Attribute(xtag.KeyFrame).Value)
                validflags = InfoFlags;
            else
                validflags = InfoFlags & (StatFlags)pd.StatFlags;

            pd.StatFlags = UInt64.Parse(ele.Attribute(xtag.StatFlags).Value);
            pd.TimeMS = UInt64.Parse(ele.Attribute(xtag.TimeMS).Value);


            if ((validflags & StatFlags.Time) != StatFlags.Zero) pd.TimeAcc = double.Parse(ele.Attribute(xtag.TimeAcc).Value);
            if ((validflags & StatFlags.LapTime) != StatFlags.Zero) pd.LapTime = float.Parse(ele.Attribute(xtag.LapTime).Value);
            if ((validflags & StatFlags.Lap) != StatFlags.Zero) pd.Lap = int.Parse(ele.Attribute(xtag.Lap).Value);
            if ((validflags & StatFlags.Distance) != StatFlags.Zero) pd.Distance = Double.Parse(ele.Attribute(xtag.Distance).Value);
            if ((validflags & StatFlags.Lead) != StatFlags.Zero) pd.Lead = float.Parse(ele.Attribute(xtag.Lead).Value);
            if ((validflags & StatFlags.Grade) != StatFlags.Zero) pd.Grade = float.Parse(ele.Attribute(xtag.Grade).Value);
            if ((validflags & StatFlags.Wind) != StatFlags.Zero) pd.Wind = float.Parse(ele.Attribute(xtag.Wind).Value);
            if ((validflags & StatFlags.Speed) != StatFlags.Zero) pd.Speed = float.Parse(ele.Attribute(xtag.Speed).Value);
            if ((validflags & StatFlags.Speed_Avg) != StatFlags.Zero) pd.Speed_Avg = float.Parse(ele.Attribute(xtag.Speed_Avg).Value);
            if ((validflags & StatFlags.Speed_Max) != StatFlags.Zero) pd.Speed_Max = float.Parse(ele.Attribute(xtag.Speed_Max).Value);
            if ((validflags & StatFlags.Watts) != StatFlags.Zero) pd.Watts = float.Parse(ele.Attribute(xtag.Watts).Value);
            if ((validflags & StatFlags.Watts_Avg) != StatFlags.Zero) pd.Watts_Avg = float.Parse(ele.Attribute(xtag.Watts_Avg).Value);
            if ((validflags & StatFlags.Watts_Max) != StatFlags.Zero) pd.Watts_Max = float.Parse(ele.Attribute(xtag.Watts_Max).Value);
            if ((validflags & StatFlags.Watts_Wkg) != StatFlags.Zero) pd.Watts_Wkg = float.Parse(ele.Attribute(xtag.Watts_Wkg).Value);
            if ((validflags & StatFlags.Watts_Load) != StatFlags.Zero) pd.Watts_Load = float.Parse(ele.Attribute(xtag.Watts_Load).Value);
            if ((validflags & StatFlags.HeartRate) != StatFlags.Zero) pd.HeartRate = float.Parse(ele.Attribute(xtag.HeartRate).Value);
            if ((validflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) pd.HeartRate_Avg = float.Parse(ele.Attribute(xtag.HeartRate_Avg).Value);
            if ((validflags & StatFlags.HeartRate_Max) != StatFlags.Zero) pd.HeartRate_Max = float.Parse(ele.Attribute(xtag.HeartRate_Max).Value);
            if ((validflags & StatFlags.Cadence) != StatFlags.Zero) pd.Cadence = float.Parse(ele.Attribute(xtag.Cadence).Value);
            if ((validflags & StatFlags.Cadence_Avg) != StatFlags.Zero) pd.Cadence_Avg = float.Parse(ele.Attribute(xtag.Cadence_Avg).Value);
            if ((validflags & StatFlags.Cadence_Max) != StatFlags.Zero) pd.Cadence_Max = float.Parse(ele.Attribute(xtag.Cadence_Max).Value);
            if ((validflags & StatFlags.Calories) != StatFlags.Zero) pd.Calories = float.Parse(ele.Attribute(xtag.Calories).Value);
            if ((validflags & StatFlags.PulsePower) != StatFlags.Zero) pd.PulsePower = float.Parse(ele.Attribute(xtag.PulsePower).Value);
            if ((validflags & StatFlags.DragFactor) != StatFlags.Zero) pd.DragFactor = float.Parse(ele.Attribute(xtag.DragFactor).Value);
            if ((validflags & StatFlags.SS) != StatFlags.Zero) pd.SS = float.Parse(ele.Attribute(xtag.SS).Value);
            if ((validflags & StatFlags.SSLeft) != StatFlags.Zero) pd.SSLeft = float.Parse(ele.Attribute(xtag.SSLeft).Value);
            if ((validflags & StatFlags.SSRight) != StatFlags.Zero) pd.SSRight = float.Parse(ele.Attribute(xtag.SSRight).Value);
            if ((validflags & StatFlags.SSLeftSplit) != StatFlags.Zero) pd.SSLeftSplit = float.Parse(ele.Attribute(xtag.SSLeftSplit).Value);
            if ((validflags & StatFlags.SSRightSplit) != StatFlags.Zero) pd.SSRightSplit = float.Parse(ele.Attribute(xtag.SSRightSplit).Value);
            if ((validflags & StatFlags.SSLeftATA) != StatFlags.Zero) pd.SSLeftATA = float.Parse(ele.Attribute(xtag.SSLeftATA).Value);
            if ((validflags & StatFlags.SSRightATA) != StatFlags.Zero) pd.SSRightATA = float.Parse(ele.Attribute(xtag.SSRightATA).Value);
            if ((validflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) pd.SSLeft_Avg = float.Parse(ele.Attribute(xtag.SSLeft_Avg).Value);
            if ((validflags & StatFlags.SSRight_Avg) != StatFlags.Zero) pd.SSRight_Avg = float.Parse(ele.Attribute(xtag.SSRight_Avg).Value);
            //if ((validflags & StatFlags.LeftPower) != StatFlags.Zero)  pd.LeftPower = float.Parse(ele.Attribute(xtag.LeftPower).Value);
            //if ((validflags & StatFlags.RightPower) != StatFlags.Zero)  pd.RightPower = float.Parse(ele.Attribute(xtag.RightPower).Value);
            //if ((validflags & StatFlags.PercentAT) != StatFlags.Zero)  pd.PercentAT = float.Parse(ele.Attribute(xtag.PercentAT).Value);
            if ((validflags & StatFlags.FrontGear) != StatFlags.Zero) pd.FrontGear = int.Parse(ele.Attribute(xtag.FrontGear).Value);
            if ((validflags & StatFlags.RearGear) != StatFlags.Zero) pd.RearGear = int.Parse(ele.Attribute(xtag.RearGear).Value);
            if ((validflags & StatFlags.GearInches) != StatFlags.Zero) pd.GearInches = int.Parse(ele.Attribute(xtag.GearInches).Value);
            //if ((validflags & StatFlags.RawSpinScan) != StatFlags.Zero)  pd.RawSpinScan = float.Parse(ele.Attribute(xtag.RawSpinScan).Value);
            //if ((validflags & StatFlags.CadenceTiming) != StatFlags.Zero) pd.CadenceTiming = float.Parse(ele.Attribute(xtag.CadenceTiming).Value);
            if ((validflags & StatFlags.TSS) != StatFlags.Zero) pd.TSS = float.Parse(ele.Attribute(xtag.TSS).Value);
            if ((validflags & StatFlags.IF) != StatFlags.Zero) pd.IF = float.Parse(ele.Attribute(xtag.IF).Value);
            if ((validflags & StatFlags.NP) != StatFlags.Zero) pd.NP = float.Parse(ele.Attribute(xtag.NP).Value);

            if ((validflags & StatFlags.Bars) != StatFlags.Zero)
            {
                for (int i = 0; i <= 24; i++)
                {
                    pd.Bars[i] = float.Parse(ele.Attribute(xtag.Bars + i).Value);
                }
            }

            if ((validflags & StatFlags.Bars_Shown) != StatFlags.Zero) pd.Bars_Shown = int.Parse(ele.Attribute(xtag.Bars_Shown).Value);

            if ((validflags & StatFlags.Bars_Avg) != StatFlags.Zero)
            {
                for (int i = 0; i <= 24; i++)
                {
                    pd.Bars[i] = float.Parse(ele.Attribute(xtag.AverageBars + i).Value);
                }
            }

            if ((validflags & StatFlags.CourseScreenX) != StatFlags.Zero) pd.CourseScreenX = int.Parse(ele.Attribute(xtag.CourseScreenX).Value);
            if ((validflags & StatFlags.RiderName) != StatFlags.Zero) pd.RiderName = ele.Attribute(xtag.RiderName).Value;
            //if ((validflags & StatFlags.Course) != StatFlags.Zero)  pd. = Course;

            return pd;
        }

        public static PerfData PerfDataFromXML(ref PerfData pd, StatFlags InfoFlags, XElement ele)
        {

            pd.StatFlags = UInt64.Parse(ele.Element(xtag.StatFlags).Value);
            pd.TimeMS = UInt64.Parse(ele.Element(xtag.TimeMS).Value);

            StatFlags validflags = StatFlags.Zero;
            if (ele.HasAttributes && ("true" == ele.Attribute(xtag.KeyFrame).Value))
                validflags = InfoFlags;
            else
                validflags = InfoFlags & (StatFlags)pd.StatFlags;

            if ((validflags & StatFlags.Time) != StatFlags.Zero) pd.TimeAcc = double.Parse(ele.Element(xtag.TimeAcc).Value);
            if ((validflags & StatFlags.LapTime) != StatFlags.Zero) pd.LapTime = float.Parse(ele.Element(xtag.LapTime).Value);
            if ((validflags & StatFlags.Lap) != StatFlags.Zero) pd.Lap = int.Parse(ele.Element(xtag.Lap).Value);
            if ((validflags & StatFlags.Distance) != StatFlags.Zero) pd.Distance = Double.Parse(ele.Element(xtag.Distance).Value);
            if ((validflags & StatFlags.Lead) != StatFlags.Zero) pd.Lead = float.Parse(ele.Element(xtag.Lead).Value);
            if ((validflags & StatFlags.Grade) != StatFlags.Zero) pd.Grade = float.Parse(ele.Element(xtag.Grade).Value);
            if ((validflags & StatFlags.Wind) != StatFlags.Zero) pd.Wind = float.Parse(ele.Element(xtag.Wind).Value);
            if ((validflags & StatFlags.Speed) != StatFlags.Zero) pd.Speed = float.Parse(ele.Element(xtag.Speed).Value);
            if ((validflags & StatFlags.Speed_Avg) != StatFlags.Zero) pd.Speed_Avg = float.Parse(ele.Element(xtag.Speed_Avg).Value);
            if ((validflags & StatFlags.Speed_Max) != StatFlags.Zero) pd.Speed_Max = float.Parse(ele.Element(xtag.Speed_Max).Value);
            if ((validflags & StatFlags.Watts) != StatFlags.Zero) pd.Watts = float.Parse(ele.Element(xtag.Watts).Value);
            if ((validflags & StatFlags.Watts_Avg) != StatFlags.Zero) pd.Watts_Avg = float.Parse(ele.Element(xtag.Watts_Avg).Value);
            if ((validflags & StatFlags.Watts_Max) != StatFlags.Zero) pd.Watts_Max = float.Parse(ele.Element(xtag.Watts_Max).Value);
            if ((validflags & StatFlags.Watts_Wkg) != StatFlags.Zero) pd.Watts_Wkg = float.Parse(ele.Element(xtag.Watts_Wkg).Value);
            if ((validflags & StatFlags.Watts_Load) != StatFlags.Zero) pd.Watts_Load = float.Parse(ele.Element(xtag.Watts_Load).Value);
            if ((validflags & StatFlags.HeartRate) != StatFlags.Zero) pd.HeartRate = float.Parse(ele.Element(xtag.HeartRate).Value);
            if ((validflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) pd.HeartRate_Avg = float.Parse(ele.Element(xtag.HeartRate_Avg).Value);
            if ((validflags & StatFlags.HeartRate_Max) != StatFlags.Zero) pd.HeartRate_Max = float.Parse(ele.Element(xtag.HeartRate_Max).Value);
            if ((validflags & StatFlags.Cadence) != StatFlags.Zero) pd.Cadence = float.Parse(ele.Element(xtag.Cadence).Value);
            if ((validflags & StatFlags.Cadence_Avg) != StatFlags.Zero) pd.Cadence_Avg = float.Parse(ele.Element(xtag.Cadence_Avg).Value);
            if ((validflags & StatFlags.Cadence_Max) != StatFlags.Zero) pd.Cadence_Max = float.Parse(ele.Element(xtag.Cadence_Max).Value);
            if ((validflags & StatFlags.Calories) != StatFlags.Zero) pd.Calories = float.Parse(ele.Element(xtag.Calories).Value);
            if ((validflags & StatFlags.PulsePower) != StatFlags.Zero) pd.PulsePower = float.Parse(ele.Element(xtag.PulsePower).Value);
            if ((validflags & StatFlags.DragFactor) != StatFlags.Zero) pd.DragFactor = float.Parse(ele.Element(xtag.DragFactor).Value);
            if ((validflags & StatFlags.SS) != StatFlags.Zero) pd.SS = float.Parse(ele.Element(xtag.SS).Value);
            if ((validflags & StatFlags.SSLeft) != StatFlags.Zero) pd.SSLeft = float.Parse(ele.Element(xtag.SSLeft).Value);
            if ((validflags & StatFlags.SSRight) != StatFlags.Zero) pd.SSRight = float.Parse(ele.Element(xtag.SSRight).Value);
            if ((validflags & StatFlags.SSLeftSplit) != StatFlags.Zero) pd.SSLeftSplit = float.Parse(ele.Element(xtag.SSLeftSplit).Value);
            if ((validflags & StatFlags.SSRightSplit) != StatFlags.Zero) pd.SSRightSplit = float.Parse(ele.Element(xtag.SSRightSplit).Value);
            if ((validflags & StatFlags.SSLeftATA) != StatFlags.Zero) pd.SSLeftATA = float.Parse(ele.Element(xtag.SSLeftATA).Value);
            if ((validflags & StatFlags.SSRightATA) != StatFlags.Zero) pd.SSRightATA = float.Parse(ele.Element(xtag.SSRightATA).Value);
            if ((validflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) pd.SSLeft_Avg = float.Parse(ele.Element(xtag.SSLeft_Avg).Value);
            if ((validflags & StatFlags.SSRight_Avg) != StatFlags.Zero) pd.SSRight_Avg = float.Parse(ele.Element(xtag.SSRight_Avg).Value);
            //if ((validflags & StatFlags.LeftPower) != StatFlags.Zero)  pd.LeftPower = float.Parse(ele.Element(xtag.LeftPower).Value);
            //if ((validflags & StatFlags.RightPower) != StatFlags.Zero)  pd.RightPower = float.Parse(ele.Element(xtag.RightPower).Value);
            //if ((validflags & StatFlags.PercentAT) != StatFlags.Zero)  pd.PercentAT = float.Parse(ele.Element(xtag.PercentAT).Value);
            if ((validflags & StatFlags.FrontGear) != StatFlags.Zero) pd.FrontGear = int.Parse(ele.Element(xtag.FrontGear).Value);
            if ((validflags & StatFlags.RearGear) != StatFlags.Zero) pd.RearGear = int.Parse(ele.Element(xtag.RearGear).Value);
            if ((validflags & StatFlags.GearInches) != StatFlags.Zero) pd.GearInches = int.Parse(ele.Element(xtag.GearInches).Value);
            //if ((validflags & StatFlags.RawSpinScan) != StatFlags.Zero)  pd.RawSpinScan = float.Parse(ele.Element(xtag.RawSpinScan).Value);
            //if ((validflags & StatFlags.CadenceTiming) != StatFlags.Zero) pd.CadenceTiming = float.Parse(ele.Element(xtag.CadenceTiming).Value);
            if ((validflags & StatFlags.TSS) != StatFlags.Zero) pd.TSS = float.Parse(ele.Element(xtag.TSS).Value);
            if ((validflags & StatFlags.IF) != StatFlags.Zero) pd.IF = float.Parse(ele.Element(xtag.IF).Value);
            if ((validflags & StatFlags.NP) != StatFlags.Zero) pd.NP = float.Parse(ele.Element(xtag.NP).Value);

            if ((validflags & StatFlags.Bars) != StatFlags.Zero)
            {
                XElement eleA = ele.Element(xtag.Bars);
                IEnumerable<XElement> eleArr = eleA.Elements();
                int count = int.Parse(eleA.Attribute(xtag.Count).Value);
                int i = 0;
                foreach (XElement el in eleArr)
                {
                    pd.Bars[i++] = float.Parse(el.Value);
                    if (i >= count)
                        break;
                }
            }

            if ((validflags & StatFlags.Bars_Shown) != StatFlags.Zero) pd.Bars_Shown = int.Parse(ele.Element(xtag.Bars_Shown).Value);

            if ((validflags & StatFlags.Bars_Avg) != StatFlags.Zero)
            {
                XElement eleA = ele.Element(xtag.AverageBars);
                IEnumerable<XElement> eleArr = eleA.Elements();
                int count = int.Parse(eleA.Attribute(xtag.Count).Value);
                int i = 0;
                foreach (XElement el in eleArr)
                {
                    pd.AverageBars[i++] = float.Parse(el.Value);
                    if (i >= count)
                        break;
                }
            }

            if ((validflags & StatFlags.CourseScreenX) != StatFlags.Zero) pd.CourseScreenX = int.Parse(ele.Element(xtag.CourseScreenX).Value);
            if ((validflags & StatFlags.RiderName) != StatFlags.Zero) pd.RiderName = ele.Element(xtag.RiderName).Value;
            //if ((validflags & StatFlags.Course) != StatFlags.Zero)  pd. = Course;



            return pd;
        }
#endif
        // LoadRMX
        /*
        public static RMX LoadRMX(string filename)
        {
            RMX rmx = new RMX();
            try
            {
                XElement node = null;

                StreamReader strm = null;
                strm = new StreamReader(filename);
                XmlTextReader reader = new XmlTextReader(strm);
                reader.ReadStartElement(xtag.RMX);
                node = LoadXElement(ref reader, xtag.Header);
                if (node != null)
                {
                    rmx.Header = new RMPHeader
                    {
                        Version = Int32.Parse(node.Element(xtag.Version).Value),
                        Date = DateTime.Parse(node.Element(xtag.Date).Value),
                        CreatorExe = node.Element(xtag.CreatorExe).Value,
                        Comment = node.Element(xtag.Comment).Value,
                        Copyright = node.Element(xtag.Copyright).Value,
                        CompressType = Int32.Parse(node.Element(xtag.CompressType).Value)
                    };
                }
                node = LoadXElement(ref reader, xtag.Info);
                if (node != null)
                {
                    rmx.Info = new PerfInfo();
                    PerfInfoFromXML(ref rmx.Info, node);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
            return rmx;
        }
        */

			public static void WriteCSVByFlags(ref CSVStream csvStrm, StatFlags includeflags, ref PerfData pd)  {
				//StatFlags changeflags = (StatFlags)pd.StatFlags;
				float f;							// tlm2014
			

            try  {
                csvStrm.SetFirst(true);
                //csvStrm.AddCSVField(xtag.StatFlags, pd.StatFlags);
                csvStrm.AddCSVField(xtag.TimeMS, pd.TimeMS);
                //csvStrm.SetFirst(false);

                if ((includeflags & StatFlags.Time) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.TimeAcc, pd.TimeAcc); }
                if ((includeflags & StatFlags.LapTime) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.LapTime, pd.LapTime); }
                if ((includeflags & StatFlags.Lap) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Lap, pd.Lap); }
					 if ((includeflags & StatFlags.Grade) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Grade, pd.Grade); }



					// tlm2014 +++++++++++++++++++++++++++++++
                if ((includeflags & StatFlags.Distance) != StatFlags.Zero)  {
						 f = (float) (ConvertConst.MetersToMilesOrKilometers * pd.Distance);
						 csvStrm.AddCSVField(xtag.Distance, f);
						 //csvStrm.AddCSVField(xtag.Distance, pd.Distance);
					 }

                if ((includeflags & StatFlags.Lead) != StatFlags.Zero) {
						 f = (float) ConvertConst.MetersToMetersOrFeet * pd.Lead;
						 csvStrm.AddCSVField(xtag.Lead, pd.Lead);
					 }
					
                if ((includeflags & StatFlags.Wind) != StatFlags.Zero) {
						 f = (float) ConvertConst.MetersToMilesOrKilometers * pd.Wind;
						 csvStrm.AddCSVField(xtag.Wind, f);
						 //csvStrm.AddCSVField(xtag.Wind, pd.Wind);
					 }

                if ((includeflags & StatFlags.Speed) != StatFlags.Zero) {
						 f = (float)ConvertConst.MetersPerSecondToMPHOrKPH * pd.Speed;
						 csvStrm.AddCSVField(xtag.Speed, f);
						 //csvStrm.AddCSVField(xtag.Speed, pd.Speed);
					 }

                if ((includeflags & StatFlags.Speed_Avg) != StatFlags.Zero) {
						 f = (float)ConvertConst.MetersPerSecondToMPHOrKPH * pd.Speed_Avg;
						 csvStrm.AddCSVField(xtag.Speed_Avg, f);
						 //csvStrm.AddCSVField(xtag.Speed_Avg, pd.Speed_Avg);
					 }

                if ((includeflags & StatFlags.Speed_Max) != StatFlags.Zero) {
						 f = (float)ConvertConst.MetersPerSecondToMPHOrKPH * pd.Speed_Max;
						 csvStrm.AddCSVField(xtag.Speed_Max, f);
						 //csvStrm.AddCSVField(xtag.Speed_Max, pd.Speed_Max);
					 }
					 // tlm2014 --------------------------------


                if ((includeflags & StatFlags.Watts) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Watts, pd.Watts); }
                if ((includeflags & StatFlags.Watts_Avg) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Watts_Avg, pd.Watts_Avg); }
                if ((includeflags & StatFlags.Watts_Max) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Watts_Max, pd.Watts_Max); }
                if ((includeflags & StatFlags.Watts_Wkg) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Watts_Wkg, pd.Watts_Wkg); }
                if ((includeflags & StatFlags.Watts_Load) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Watts_Load, pd.Watts_Load); }
                if ((includeflags & StatFlags.HeartRate) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.HeartRate, pd.HeartRate); }
                if ((includeflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.HeartRate_Avg, pd.HeartRate_Avg); }
                if ((includeflags & StatFlags.HeartRate_Max) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.HeartRate_Max, pd.HeartRate_Max); }
                if ((includeflags & StatFlags.Cadence) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Cadence, pd.Cadence); }
                if ((includeflags & StatFlags.Cadence_Avg) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Cadence_Avg, pd.Cadence_Avg); }
                if ((includeflags & StatFlags.Cadence_Max) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Cadence_Max, pd.Cadence_Max); }
                if ((includeflags & StatFlags.Calories) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Calories, pd.Calories); }
                if ((includeflags & StatFlags.PulsePower) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.PulsePower, pd.PulsePower); }
                if ((includeflags & StatFlags.DragFactor) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.DragFactor, pd.DragFactor); }
                if ((includeflags & StatFlags.SS) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SS, pd.SS); }
                if ((includeflags & StatFlags.SSLeft) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SSLeft, pd.SSLeft); }
                if ((includeflags & StatFlags.SSRight) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SSRight, pd.SSRight); }
                if ((includeflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SSLeftSplit, pd.SSLeftSplit); }
                if ((includeflags & StatFlags.SSRightSplit) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SSRightSplit, pd.SSRightSplit); }
                if ((includeflags & StatFlags.SSLeftATA) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SSLeftATA, pd.SSLeftATA); }
                if ((includeflags & StatFlags.SSRightATA) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SSRightATA, pd.SSRightATA); }
                if ((includeflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SSLeft_Avg, pd.SSLeft_Avg); }
                if ((includeflags & StatFlags.SSRight_Avg) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.SSRight_Avg, pd.SSRight_Avg); }
                //if ((changedflags & StatFlags.LeftPower) != StatFlags.Zero) { xmlStrm.AddCSVField(xtag.LeftPower, pd.LeftPower); }
                //if ((changedflags & StatFlags.RightPower) != StatFlags.Zero) { xmlStrm.AddCSVField(xtag.RightPower, pd.RightPower); }
                //if ((changedflags & StatFlags.PercentAT) != StatFlags.Zero) { xmlStrm.AddCSVField(xtag.PercentAT, pd.PercentAT); }
                if ((includeflags & StatFlags.FrontGear) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.FrontGear, pd.FrontGear); }
                if ((includeflags & StatFlags.RearGear) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.RearGear, pd.RearGear); }
                if ((includeflags & StatFlags.GearInches) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.GearInches, pd.GearInches); }
                //if ((changedflags & StatFlags.RawSpinScan) != StatFlags.Zero) { xmlStrm.AddCSVField(xtag.RawSpinScan, pd.RawSpinScan); }
                //if ((includeflags & StatFlags.CadenceTiming) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.CadenceTiming, pd.CadenceTiming); }
                if ((includeflags & StatFlags.TSS) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.TSS, pd.TSS); }
                if ((includeflags & StatFlags.IF) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.IF, pd.IF); }
                if ((includeflags & StatFlags.NP) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.NP, pd.NP); }
                if ((includeflags & StatFlags.Bars_Shown) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Bars_Shown, pd.Bars_Shown); }
                if ((includeflags & StatFlags.Bars) != StatFlags.Zero)
                {
                    float fval = 0;
                    for (int i = 0; i < 24; i++)
                    {
                        if(i < pd.Bars.Length)
                            fval = pd.Bars[i];
                        else
                            fval = 0;
                        csvStrm.AddCSVField(xtag.Bars + i, fval);
                    }
                }
                if ((includeflags & StatFlags.Bars_Avg) != StatFlags.Zero)
                {
                    float fval = 0;
                    for (int i = 0; i < 24; i++)
                    {
                        if (i < pd.AverageBars.Length)
                            fval = pd.AverageBars[i];
                        else
                            fval = 0;
                        csvStrm.AddCSVField(xtag.AverageBars + i, fval);
                    }
                }
                if ((includeflags & StatFlags.CourseScreenX) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.CourseScreenX, pd.CourseScreenX); }
                //if ((includeflags & StatFlags.RiderName) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.RiderName, pd.RiderName != null ? pd.RiderName : ""); }
                //if ((changedflags & StatFlags.Course) != StatFlags.Zero) { xmlStrm.AddCSVField(xtag.Course, pd.Course); }

                // Added v1.02
                if ((includeflags & StatFlags.Disconnected) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Disconnected, pd.Disconnected); }
                if ((includeflags & StatFlags.Drafting) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Drafting, pd.Drafting); }
                if ((includeflags & StatFlags.Calibration) != StatFlags.Zero) { csvStrm.AddCSVField(xtag.Calibration, pd.RawCalibrationValue); }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
            csvStrm.EndLine();
        }

        public static PerfData UpdateRawLastByFlags(ref PerfData pd, PerfData pdnew, StatFlags changedflags)
        {
            pd.StatFlags = (UInt64)changedflags;
            pd.TimeMS = (UInt64)(pdnew.TimeAcc * 1000);

            if ((changedflags & StatFlags.Time) != StatFlags.Zero) { pd.TimeAcc = pdnew.TimeAcc; }
            if ((changedflags & StatFlags.LapTime) != StatFlags.Zero) { pd.LapTime = pdnew.LapTime; }
            if ((changedflags & StatFlags.Lap) != StatFlags.Zero) { pd.Lap = pdnew.Lap; }
            if ((changedflags & StatFlags.Distance) != StatFlags.Zero) { pd.Distance = pdnew.Distance; }
            if ((changedflags & StatFlags.Lead) != StatFlags.Zero) { pd.Lead = pdnew.Lead; }
            if ((changedflags & StatFlags.Grade) != StatFlags.Zero) { pd.Grade = pdnew.Grade; }
            if ((changedflags & StatFlags.Wind) != StatFlags.Zero) { pd.Wind = pdnew.Wind; }
            if ((changedflags & StatFlags.Speed) != StatFlags.Zero) {
					pd.Speed = pdnew.Speed;
				}
            if ((changedflags & StatFlags.Speed_Avg) != StatFlags.Zero) { pd.Speed_Avg = pdnew.Speed_Avg; }
            if ((changedflags & StatFlags.Speed_Max) != StatFlags.Zero) { pd.Speed_Max = pdnew.Speed_Max; }
            if ((changedflags & StatFlags.Watts) != StatFlags.Zero) { pd.Watts = pdnew.Watts; }
            if ((changedflags & StatFlags.Watts_Avg) != StatFlags.Zero) { pd.Watts_Avg = pdnew.Watts_Avg; }
            if ((changedflags & StatFlags.Watts_Max) != StatFlags.Zero) { pd.Watts_Max = pdnew.Watts_Max; }
            if ((changedflags & StatFlags.Watts_Wkg) != StatFlags.Zero)
            {
                pd.Watts_Wkg = pdnew.Watts_Wkg;
            }
            if ((changedflags & StatFlags.Watts_Load) != StatFlags.Zero) { pd.Watts_Load = pdnew.Watts_Load; }
            if ((changedflags & StatFlags.HeartRate) != StatFlags.Zero) { pd.HeartRate = pdnew.HeartRate; }
            if ((changedflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) { pd.HeartRate_Avg = pdnew.HeartRate_Avg; }
            if ((changedflags & StatFlags.HeartRate_Max) != StatFlags.Zero) { pd.HeartRate_Max = pdnew.HeartRate_Max; }
            if ((changedflags & StatFlags.Cadence) != StatFlags.Zero) { pd.Cadence = pdnew.Cadence; }
            if ((changedflags & StatFlags.Cadence_Avg) != StatFlags.Zero) { pd.Cadence_Avg = pdnew.Cadence_Avg; }
            if ((changedflags & StatFlags.Cadence_Max) != StatFlags.Zero) { pd.Cadence_Max = pdnew.Cadence_Max; }
            if ((changedflags & StatFlags.Calories) != StatFlags.Zero) { pd.Calories = pdnew.Calories; }
            if ((changedflags & StatFlags.PulsePower) != StatFlags.Zero) { pd.PulsePower = pdnew.PulsePower; }
            if ((changedflags & StatFlags.DragFactor) != StatFlags.Zero) { pd.DragFactor = pdnew.DragFactor; }
            if ((changedflags & StatFlags.SS) != StatFlags.Zero) { pd.SS = pdnew.SS; }
            if ((changedflags & StatFlags.SSLeft) != StatFlags.Zero) { pd.SSLeft = pdnew.SSLeft; }
            if ((changedflags & StatFlags.SSRight) != StatFlags.Zero) { pd.SSRight = pdnew.SSRight; }
            if ((changedflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { pd.SSLeftSplit = pdnew.SSLeftSplit; }
            if ((changedflags & StatFlags.SSRightSplit) != StatFlags.Zero) { pd.SSRightSplit = pdnew.SSRightSplit; }
            if ((changedflags & StatFlags.SSLeftATA) != StatFlags.Zero) { pd.SSLeftATA = pdnew.SSLeftATA; }
            if ((changedflags & StatFlags.SSRightATA) != StatFlags.Zero) { pd.SSRightATA = pdnew.SSRightATA; }
            if ((changedflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) { pd.SSLeft_Avg = pdnew.SSLeft_Avg; }
            if ((changedflags & StatFlags.SSRight_Avg) != StatFlags.Zero) { pd.SSRight_Avg = pdnew.SSRight_Avg; }
            //if ((changedflags & StatFlags.LeftPower) != StatFlags.Zero) { pd.LeftPower = pdnew.LeftPower; }
            //if ((changedflags & StatFlags.RightPower) != StatFlags.Zero) { pd.RightPower = pdnew.RightPower; }
            //if ((changedflags & StatFlags.PercentAT) != StatFlags.Zero) { pd.PercentAT = pdnew.PercentAT; }
            if ((changedflags & StatFlags.FrontGear) != StatFlags.Zero) { pd.FrontGear = pdnew.FrontGear; }
            if ((changedflags & StatFlags.RearGear) != StatFlags.Zero) { pd.RearGear = pdnew.RearGear; }
            if ((changedflags & StatFlags.GearInches) != StatFlags.Zero) { pd.GearInches = pdnew.GearInches; }
            //if ((changedflags & StatFlags.RawSpinScan) != StatFlags.Zero) { pd.RawSpinScan = pdnew.RawSpinScan; }
            //if ((changedflags & StatFlags.CadenceTiming) != StatFlags.Zero) { pd.CadenceTiming = pdnew.CadenceTiming; }
            if ((changedflags & StatFlags.TSS) != StatFlags.Zero) { pd.TSS = pdnew.TSS; }
            if ((changedflags & StatFlags.IF) != StatFlags.Zero) { pd.IF = pdnew.IF; }
            if ((changedflags & StatFlags.NP) != StatFlags.Zero) { pd.NP = pdnew.NP; }
            if ((changedflags & StatFlags.Bars) != StatFlags.Zero) { Array.Copy( pdnew.Bars,pd.Bars,24); }
            if ((changedflags & StatFlags.Bars_Shown) != StatFlags.Zero) { pd.Bars_Shown = pdnew.Bars_Shown; }
            if ((changedflags & StatFlags.Bars_Avg) != StatFlags.Zero) { Array.Copy(pdnew.AverageBars, pd.AverageBars, 24); }
            if ((changedflags & StatFlags.CourseScreenX) != StatFlags.Zero) { pd.CourseScreenX = pdnew.CourseScreenX; }
            if ((changedflags & StatFlags.RiderName) != StatFlags.Zero) { pd.RiderName = pdnew.RiderName; }
            if ((changedflags & StatFlags.Course) != StatFlags.Zero) { pd.Course = pdnew.Course; }

            // Added v1.02
            if ((changedflags & StatFlags.Disconnected) != StatFlags.Zero) { pd.Disconnected = pdnew.Disconnected; }
            if ((changedflags & StatFlags.Drafting) != StatFlags.Zero) { pd.Drafting = pdnew.Drafting; }
            if ((changedflags & StatFlags.Calibration) != StatFlags.Zero) { pd.RawCalibrationValue = pdnew.RawCalibrationValue; }
            return pd;
        }
#endif
        public static XElement LoadXElement(ref XmlTextReader reader, string nodename)
        {
            XElement node = null;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        if (reader.Name == nodename)
                            node = XElement.ReadFrom(reader) as XElement;
                        break;
                }
                if (node != null)
                    break;
            }
            return node;
        }
        // LoadRMPHeader
        /*
        public static RMPHeader LoadRMPHeader(string filename)
        {
            RMPHeader Header = new RMPHeader();
            try
            {
                XElement node = null;

                StreamReader strm = null;
                strm = new StreamReader(filename);
                XmlTextReader reader = new XmlTextReader(strm);
                reader.ReadStartElement(xtag.RMX);
                node = LoadXElement(ref reader, xtag.Header);
                if (node != null)
                {
                    Header = new RMPHeader
                    {
                        Version = Int32.Parse(node.Element(xtag.Version).Value),
                        Date = DateTime.Parse(node.Element(xtag.Date).Value),
                        CreatorExe = node.Element(xtag.CreatorExe).Value,
                        Comment = node.Element(xtag.Comment).Value,
                        Copyright = node.Element(xtag.Copyright).Value,
                        CompressType = Int32.Parse(node.Element(xtag.CompressType).Value)
                    };
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
            return Header;
        }
        */
        public static void SaveXCourse(ref XmlStream xmlStrm, string saveFilename, Course course)
        {
            CourseAttributes t =
                (course.Looped ? CourseAttributes.Looped : CourseAttributes.Zero) |
                (course.Mirror ? CourseAttributes.Mirror : CourseAttributes.Zero) |
                (course.Modified ? CourseAttributes.Modified : CourseAttributes.Zero) |
                //(course.OutAndBack ? CourseAttributes.OutAndBack : CourseAttributes.Zero) |
                (course.Reverse ? CourseAttributes.Reverse : CourseAttributes.Zero);
            byte direction = (byte)t;

            xmlStrm.AddXElementStart(xtag.Course, 1);

            string name = Path.GetFileNameWithoutExtension(saveFilename);
            xmlStrm.AddXElement(xtag.Info,
                XUtil.XAttr(xtag.Name, XMLEscape(name)),
                XUtil.XAttr(xtag.Description, XMLEscape(course.Description)),
                XUtil.XAttr(xtag.FileName, XMLEscape(saveFilename)),
                XUtil.XAttr(xtag.Type, course.Type.ToString()),
                XUtil.XAttr(xtag.Looped, course.Looped),
                XUtil.XAttr(xtag.Length, course.StringLength),
                XUtil.XAttr(xtag.Laps, course.Laps),
                XUtil.XAttr(xtag.StartAt, course.StartAt),
                XUtil.XAttr(xtag.EndAt, course.EndAt),
                XUtil.XAttr(xtag.Mirror, course.Mirror),
                XUtil.XAttr(xtag.Reverse, course.Reverse),
                XUtil.XAttr(xtag.Modified, course.Modified),
                XUtil.XAttr(xtag.XUnits, course.XUnits.ToString()),
                XUtil.XAttr(xtag.YUnits, course.YUnits.ToString()),
                XUtil.XAttr(xtag.OriginalHash, course.OriginalHash),
                XUtil.XAttr(xtag.CourseHash, course.CourseHash),
                XUtil.XAttr(xtag.HeaderHash, course.HeaderHash)
                );

            CourseType ct = course.Type;
			if ((ct & CourseType.Video) == CourseType.Video)
				ct = CourseType.Video;
            else if ((ct & CourseType.ThreeD) == CourseType.ThreeD)
                ct = CourseType.ThreeD;
            switch (ct)
            {
                case CourseType.Video:
                    xmlStrm.AddXElementStart(xtag.RCVType + " " + xtag.Count + "=\"" + course.Segments.Count + "\"", 1);
                    for (LinkedListNode<Course.Segment> n = course.Segments.First; n != null; n = n.Next)
                    {
                        xmlStrm.AddXElementStart(xtag.Val, 1);

                        Course.GPSSegment val = (Course.GPSSegment)n.Value;

                        xmlStrm.AddXElementStart(xtag.GPSData, 1);
                        GPSData gd = val.GPSData;

                        xmlStrm.AddXField("frame", gd.frame);
                        xmlStrm.AddXField("real", gd.real);
                        xmlStrm.AddXField("seconds", gd.seconds);
                        xmlStrm.AddXField("lat", gd.lat);
                        xmlStrm.AddXField("lon", gd.lon);
                        xmlStrm.AddXField("unfiltered_elev", gd.unfiltered_elev);
                        xmlStrm.AddXField("filtered_elev", gd.filtered_elev);
                        xmlStrm.AddXField("manelev", gd.manelev);
                        xmlStrm.AddXField("accum_meters1", gd.accum_meters1);
                        xmlStrm.AddXField("accum_meters2", gd.accum_meters2);
                        xmlStrm.AddXField("section_meters1", gd.section_meters1);
                        xmlStrm.AddXField("section_meters2", gd.section_meters2);
                        xmlStrm.AddXField("pg", gd.pg);
                        xmlStrm.AddXField("mps1", gd.mps1);
                        xmlStrm.AddXField("mph1", gd.mph1);
                        xmlStrm.AddXField("mps2", gd.mps2);
                        xmlStrm.AddXField("mph2", gd.mph2);
                        xmlStrm.AddXField("faz", gd.faz);
                        xmlStrm.AddXField("seconds_offset", gd.seconds_offset);
                        xmlStrm.AddXField("x", gd.x);
                        xmlStrm.AddXField("y", gd.y);
                        xmlStrm.AddXField("z", gd.z);

                        xmlStrm.AddXElementEnd(xtag.GPSData, -1);

                        xmlStrm.AddXField(xtag.Length, val.Length);
                        xmlStrm.AddXField(xtag.Grade, val.Grade);
                        xmlStrm.AddXField(xtag.Wind, val.Wind);

                        xmlStrm.AddXElementEnd(xtag.Val, -1);
                    }
                    xmlStrm.AddXElementEnd(xtag.RCVType);
                    break;
                case CourseType.ThreeD:
                    xmlStrm.AddXElementStart(xtag.ThreeDType + " " + xtag.Count + "=\"" + course.Segments.Count + "\"", 1);
                    for (LinkedListNode<Course.Segment> n = course.Segments.First; n != null; n = n.Next)
                    {

                        Course.PysicalSegment val = (Course.PysicalSegment)n.Value;
						Course.SmoothSegment ss = val as Course.SmoothSegment;
						if (ss != null)
						{
							string attr = "";
							xmlStrm.AddXAttrib(ref attr, xtag.Smooth, "true");
							xmlStrm.AddXElementStart(xtag.Val, attr, 1);
							xmlStrm.AddXField(xtag.Divisions, ss.Divisions.ToString());
						}
						else
							xmlStrm.AddXElementStart(xtag.Val, 1);
                        xmlStrm.AddXField(xtag.Length, val.Length);
                        xmlStrm.AddXField(xtag.Grade, val.Grade);
                        xmlStrm.AddXField(xtag.Wind, val.Wind);
                        xmlStrm.AddXField(xtag.Rotation, val.Rotation);

                        xmlStrm.AddXElementEnd(xtag.Val, -1);
                    }
                    xmlStrm.AddXElementEnd(xtag.ThreeDType, -1);
                    break;
				default:
					if (course.XUnits == CourseXUnits.Distance && course.YUnits == CourseYUnits.Grade)
					{
						// Save this coures as a Distance course, rather than the generic type.
						xmlStrm.AddXElementStart(xtag.DistanceType + " " + xtag.Count + "=\"" + course.Segments.Count + "\"", 1);
						for (LinkedListNode<Course.Segment> n = course.Segments.First; n != null; n = n.Next)
						{
							xmlStrm.AddXElementStart(xtag.Val, 1);

							Course.PysicalSegment val = (Course.PysicalSegment)n.Value;
							xmlStrm.AddXField(xtag.Length, val.Length);
							xmlStrm.AddXField(xtag.Grade, val.Grade);
							xmlStrm.AddXField(xtag.Wind, val.Wind);

							xmlStrm.AddXElementEnd(xtag.Val, -1);
						}
						xmlStrm.AddXElementEnd(xtag.DistanceType, -1);
					}
					else
					{
						xmlStrm.AddXElementStart(xtag.Segments + " " + xtag.Count + "=\"" + course.Segments.Count + "\"", 1);
						for (LinkedListNode<Course.Segment> n = course.Segments.First; n != null; n = n.Next)
						{
							xmlStrm.AddXElementStart(xtag.Val, 1);
							if (course.XUnits == CourseXUnits.Distance)
								xmlStrm.AddXField(xtag.Length, n.Value.Length);
							else
								xmlStrm.AddXField(xtag.Minutes, n.Value.Length / 60);

							if (course.YUnits == CourseYUnits.Grade)
							{
								xmlStrm.AddXField(xtag.Length, n.Value.Length);
								xmlStrm.AddXField(xtag.Grade, n.Value.GradeAt(1));
							}
							else if (course.YUnits == CourseYUnits.Watts)
							{
								xmlStrm.AddXField(xtag.StartWatts, Math.Round(n.Value.StartY,0));
								xmlStrm.AddXField(xtag.EndWatts, Math.Round(n.Value.EndY,0));
							}
							else
							{
								xmlStrm.AddXField(xtag.StartAt, n.Value.StartY);
								xmlStrm.AddXField(xtag.EndAt, Math.Round(n.Value.EndY,3));
							}
							xmlStrm.AddXElementEnd(xtag.Val, -1);
						}
						xmlStrm.AddXElementEnd(xtag.Segments, -1);
					}
					break;
            }

            xmlStrm.AddXElementEnd(xtag.Course, -1);
        }

        public static string XMLEscape(string str)
        {
            return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        public static bool SaveRMXCourse(string filename, Course course, string comment)
        {
            bool result = true;
            //RMPHeader rmxh = new RMPHeader
            PerfFile.CRMHeader rmxh = new PerfFile.CRMHeader
            {
                CreatorExe = xval.RacerMateOne, // 32 program that created
                Date = DateTime.Now,  // date created
                Version = xval.Version,    // version of this format
                Comment = XMLEscape(comment), // 32 description of this file
                Copyright = xval.Copyright,   // 32 RacerMate copyright
                CompressType = 0 // different compression type
            };
            XmlStream xmlStrm = null;
            xmlStrm = new XmlStream();
            try
            {
                xmlStrm.Format = true;
                xmlStrm.OpenXmlFileOut(filename);

                xmlStrm.AddXElementStart(xtag.RMX, 1);

                xmlStrm.AddObject(rmxh, xtag.Header);
                if (course != null)
                {
                    SaveXCourse(ref xmlStrm, Path.GetFileName(filename), course);
                }

                xmlStrm.AddXElementEnd(xtag.RMX, -1);
            }

            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                result = true;
            }
            xmlStrm.CloseXmlFileOut();

            return result;
        }

    }

#if D3DHOST
#else
    public class HashOutStream
    {
        byte [] outHash = null;
        //private MemoryStream bw = null;
        //private CryptoStream inHash = null;
        private HashAlgorithm hash = null;
        private string hashAlgorithm = null;

        public static string ComputeHash(string strToHash)
        {
            HashOutStream hash = new HashOutStream();
            string results = null;
            hash.OpenHashOut("");
            hash.Insert(strToHash);
            results = hash.GetHashOut();
            hash.CloseHashOut();
            return results;
        }

        public string GetHashOut()
        {
            FlushBufferOut();
            if (outHash == null)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < outHash.Length; i++)
            {
                sb.Append(outHash[i].ToString("X2"));
            }

            //Debug.WriteLine(sb.ToString());

            return sb.ToString();
        }

        public void CloseHashOut()  {
            FlushBufferOut();
            //if (bw != null)
            //    bw = null;
#if DEBUG
				Debug.WriteLine("PerfFrame.cs CloseHashOut(), GC.Collect()");
#endif
            GC.Collect();
        }

        public bool Insert(string instring)
        {
            return Insert(new UTF8Encoding().GetBytes(instring));
        }
        public bool Insert(byte[] inBytes)
        {
            try
            {
                if(Init())
                {
                    hash.TransformBlock(inBytes, 0, inBytes.Length, inBytes, 0);
                    //bw.Write(inBytes, 0, inBytes.Length);
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseHashOut();
                return false;
            }
            return true;
        }

        public bool Init()
        {
            try
            {
                //if (bw == null)
                //    bw = new MemoryStream();
                if (hash == null)
                {
                    // Make sure hashing algorithm name is specified.
                    if (hashAlgorithm == null)
                        hashAlgorithm = "";
                    
                    // Initialize appropriate hashing algorithm class.
                    switch (hashAlgorithm.ToUpper())
                    {
                        case "SHA1":
                            hash = new SHA1Managed();
                            break;

                        case "SHA256":
                            hash = new SHA256Managed();
                            break;

                        case "SHA384":
                            hash = new SHA384Managed();
                            break;

                        case "SHA512":
                            hash = new SHA512Managed();
                            break;

                        default:
                            hash = new MD5CryptoServiceProvider();
                            break;
                    }
                    hash.Initialize();
                }
                //if (inHash == null)
                //    inHash = new CryptoStream(bw, hash, CryptoStreamMode.Write);
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseHashOut();
                return false;
            }
            return true;
        }
        public bool OpenHashOut(string hashName)
        {
            try
            {
                CloseHashOut();
                hashAlgorithm = hashName;
                if(!Init())
                    return false;
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseHashOut();
                return false;
            }
            return true;
        }
        // flush the crypto buffer
        public bool FlushBufferOut()
        {
            //if (inHash != null)
            //{
            //    inHash.FlushFinalBlock();
            //    outHash = hash.Hash;
            //    inHash.Close();
            //    inHash = null;
            //}
            if (hash != null)
            {
                hash.TransformFinalBlock(new byte[0], 0, 0);
                outHash = hash.Hash;
                hash = null;
            }
            return true;
        }
    }




    public class RawStream  {
        FileStream outfile = null;
        FileStream infile = null;
        //private BinaryWriter bw = null;
        private BinaryReader br = null;

        private object _obj = null;
        private System.Type _type = null;

        MemoryStream memstream = null;

        IAsyncResult asyncResult = null;

        // Maintain state information to be passed to 
        // EndWriteCallback and EndReadCallback.

        class RawState  {
            // fStream is used to read and write to the file.
            FileStream fStream;
            public RawState(FileStream fStream)  {
                this.fStream = fStream;
            }

            public FileStream FStream  {
                get {
                    return fStream;
                }
            }
        }

        /******************************************************************************************
        When BeginWrite is finished writing data to the file, the
        EndWriteCallback method is called to end the asynchronous
        write operation
        ******************************************************************************************/

        static void EndWriteCallback(IAsyncResult asyncResult)  {
            //Debug.WriteLine("done a write");
            RawState tempState = (RawState)asyncResult.AsyncState;
            FileStream fStream = tempState.FStream;
            fStream.EndWrite(asyncResult);
        }

        public Int64 GetCurRawFieldOutPos()
        {
            FlushBufferOut();
            return outfile.Position;
        }

        public void SetCurRawFieldOutPos(Int64 pos)
        {
            FlushBufferOut();
            outfile.Seek(pos, SeekOrigin.Begin);
        }

        public Int64 GetCurRawFieldInPos()
        {
            return infile.Position;
        }

        public void SetCurRawFieldInPos(Int64 pos)
        {
            infile.Seek(pos, SeekOrigin.Begin);
        }

        // Always flush before seeking and after writing
        public void AddRawFieldAt(Int64 pos, string tag, object obj)
        {
            FlushBufferOut();
            outfile.Seek(pos,SeekOrigin.Begin);
            AddRawField(ref tag, ref obj, true);
        }

        // default flush
			public void AddRawField(string tag, object obj)  {
            AddRawField(ref tag, ref obj, false);
        }

        // flush optional
        public void AddRawField(string tag, object obj, bool bflush)
        {
            AddRawField(ref tag, ref obj, bflush);
        }

			public void AddRawBytes(string tag, ref byte[] val, int len, bool bFlush)  {
            AddRawBytes(ref val, len, bFlush);
        }


			/*******************************************************************************
				tags

					news = 0  TimeMilliseconds
					news = 1  Bars
					news = 2  Watts
					news = 3  Watts
					news = 4  Speed
					news = 5  Watts_Load
					news = 6  Bars
					news = 7  Bars
					news = 8  Watts
					news = 9  Watts
					news = 10  SpinScanRight
					news = 11  Header
					news = 12  Data
					news = 13  Header
					PerfFrame::CoseRawFileOut(), GC.Collect()

			*******************************************************************************/

			void AddRawField(ref string tag, ref object obj, bool bFlush) {
				if (memstream == null)
                {
#if DO_ALLOC
					memstream = new MemoryStream();
#else
                    memstream = new MemoryStream(4 * 1024);
#endif
				}

				if (memstream != null)  {
				byte[] val = StructToByteArray(ref obj);
				AddRawBytes(ref val, val.Length, bFlush);
			}
			}							// AddRawField()


			/*******************************************************************************

			*******************************************************************************/

			void AddRawBytes(ref byte[] val, int len, bool bFlush)  {
				if (memstream == null)  {
					// never gets here
					memstream = new MemoryStream();
				}

				if (memstream != null)  {
					if (val.Length < len)  {
						// never gets here for perf files because len = val.Length
						memstream.Write(val, 0, val.Length);
						// pad with zeros
						byte[] temp = new byte[len - val.Length];
						for (int i = val.Length; i < len; i++) {
							temp[i] = 0;
						}
						memstream.Write(temp, 0, temp.Length);				// write perf to memory
					}
					else  {
						memstream.Write(val, 0, len);								// write perf to memory
					}

					// buffer it until 4k, then write it

					//if (memstream.Length > (1024 * 4)) {
					if (memstream.Position > (1024 * 4)) {
					//if (bw.Length > (1024 * 32)) {
						bFlush = true;
					}

        if (bFlush && outfile != null)
        {
#if DO_ALLOC
            asyncResult = outfile.BeginWrite(
                                    memstream.GetBuffer(),
                                    0,
                                    (int)memstream.Length,
                                    new AsyncCallback(EndWriteCallback),
                                    new RawState(outfile)
                                );
            memstream.Dispose();
            memstream.Close();
            memstream = null;
#else
						ba = memstream.GetBuffer();
						int datalen;
						datalen = ba.Length;							// 8192

						datalen = (int)memstream.Length;			// 4097
						asyncResult = outfile.BeginWrite(
												ba,
												0,
												datalen,
												new AsyncCallback(EndWriteCallback),
												new RawState(outfile)
							);
	

						memstream.Position = 0;
#endif
            //GC.Collect();		// didn't help
        }								// if (bFlush && outfile != null)  {
		}
		return;
	}								// AddRawBytes()


        // flush the memory buffer to outfile
        public bool FlushBufferOut()  {
            if (memstream != null && outfile != null)  {
                asyncResult = outfile.BeginWrite(memstream.GetBuffer(), 0, (int)memstream.Length, null, new RawState(outfile));
                memstream.Dispose();
                memstream.Close();
                memstream = null;
                outfile.EndWrite(asyncResult);
            }
            return true;
        }

        public void GetRawFieldAt(Int64 pos, string tag, ref object obj)
        {
            obj = GetNextStructureValue(obj.GetType());
        }

        public void GetRawField(string tag, ref object obj)
        {
            obj = GetNextStructureValue(obj.GetType());
        }

			public void CloseRawFileOut()  {
            FlushBufferOut();
				if (outfile != null)  {
                outfile.Close();
                outfile = null;
            }
#if DEBUG
				//Debug.WriteLine("PerfFrame::CoseRawFileOut(), GC.Collect()");
#endif
				//GC.Collect();														// didn't help
        }

			public bool OpenRawFileOut(string fileName)  {							// .../Performances/Last-Rider1.tmp
            try
            {
                CloseRawFileOut();
                outfile = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseRawFileOut();
                return false;
            }
            return true;
        }

        public bool OpenRawFileIn(string fileName)
        {
            try
            {
                CloseRawFileIn();
                infile = new FileStream(fileName, FileMode.Open);
                if (infile != null)
                {
                    br = new BinaryReader(infile);
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseRawFileIn();
                return false;
            }
            return true;
        }

			public void CloseRawFileIn()  {
				if (br != null)  {
                br.Close();
                br = null;
            }
				if (infile != null)  {
                infile.Close();
                infile = null;
            }
            GC.Collect();
        }

        private byte[] StructToByteArray(ref object obj)
        {
            try
            {
                _obj = obj;
                // This function copies the structure data into a byte[] 

                //Set the buffer to the correct size 
                byte[] buffer = new byte[Marshal.SizeOf(_obj)];

                //Allocate the buffer to memory and pin it so that GC cannot use the 
                //space (Disable GC) 
                GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                // copy the struct into int byte[] mem alloc 
                Marshal.StructureToPtr(_obj, h.AddrOfPinnedObject(), false);

                h.Free(); //Allow GC to do its job 

                return buffer; // return the byte[]. After all that's why we are here 
                // right. 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool EOF				//End of File
        {
            get
            {
                if (infile != null)
                {
                    if (infile.Position >= infile.Length)
                        infile.Close();
                }

                return infile == null;
            }
        }

        public bool GetNextBytes(ref byte[] buffer, int len)
        {
            try
            {
                if (EOF)
                    return false;
                infile.Read(buffer, 0, len);
                if (infile.Position >= infile.Length)
                    CloseRawFileIn();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public object GetNextStructureValue(System.Type type)
        {
            _type = type;
            byte[] buffer = new byte[Marshal.SizeOf(_type)];

            object obj = null;
            try
            {
                /*
                if (EOF)
                    return null;
                infile.Read(buffer, 0, buffer.Length);
                if (infile.Position >= infile.Length)
                    CloseRawFileIn();
                */

                if (!GetNextBytes(ref buffer, buffer.Length))
                    return null;

                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                obj = (object)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), _type);
                handle.Free();

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

		}												// class RawStream



    public class CSVStream
    {
        FileStream outfile = null;
        private StringBuilder build = null;
        int CSVType = 1;
        string separator = ",";
        bool bFirst = true;
        bool bHeader = true;

        public void SetFirst(bool bFlag) { bFirst = bFlag; }
        public void SetHeader(bool bFlag) { bHeader = bFlag; }
        public void SetSeparator(string sep) { separator = sep; }

        public void EndLine()
        {
            build.Append(Environment.NewLine);
        }

        public void AddCSVField(string tag, object val)
        {
            ToColumnOut(tag, val, null);
        }

        public bool ToColumnOut(string fieldName, object objValue, string typeName)
        {
            string sep = separator;
            string val;
            switch (CSVType)
            {
                case 1: // Normal
                    {
                        sep = separator;
                        if (bFirst)
                        {
                            sep = "";
                            bFirst = false;
                        }
                        build.Append(sep);
                        if (bHeader)
                        {
                            build.Append(EscapeCSVQuotedStr(fieldName));
                        }
                        else
                        {
                            /*
                            if (typeName == "Char")
                            {
                                objValue = EncodeCharacter((char)objValue);
                            }
                             */
                            val = objValue.ToString();
                            build.Append(EscapeCSVQuotedStr(val));
                        }
                    }
                    break;
                default: // Pair values
                    {
                        /*
                        if (typeName == "Char")
                        {
                            objValue = EncodeCharacter((char)objValue);
                        }
                        */
                        val = objValue.ToString();
                        build.Append(@" """ + fieldName + @""" " + EscapeQuotedStr(val) + Environment.NewLine);
                    }
                    break;
            }
            return true;
        }

        public bool OpenCSVFileOut(string fileName)
        {
            try
            {
                outfile = new FileStream(fileName, FileMode.Create);
                if (outfile != null)
                {
                    build = new StringBuilder();
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseCSVFileOut();
            }
            return false;
        }

        public void CloseCSVFileOut()
        {
            FlushCSVBuild();
            if (outfile != null)
            {
                outfile.Close();
                outfile = null;
            }
        }
        //
        //
        public bool FlushCSVBuild()
        {
            MemoryStream strm = null;
            try
            {
                string str = build.ToString();
                build.Remove(0, build.Length);
                //convert string to stream
                if (str != null)
                {
                    strm = new MemoryStream();
                    if (strm != null)
                    {
                        byte[] strmByte = null;
                        strmByte = StreamConverter.ConvertStringToByteArray(str);
                        if (strmByte != null)
                        {
                            strm.Write(strmByte, 0, str.Length);
                        }
                        byte[] bytes = null;
                        bytes = strm.ToArray();
                        if (bytes != null)
                        {
                            outfile.Write(bytes, 0, bytes.Length);
                        }
                        strm.Close();
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                if (strm != null)
                {
                    strm.Close();
                }
            }
            return false;
        }
        public string EscapeQuotedStr(string value)
        {
            string s = @"""""";
            if (value != null && 0 < value.Length)
            {
                value = value.Replace("\\", "\\\\");
                value = value.Replace("\"", "\\\"");
                value = value.Replace("\r\n", "\\\n");
                s = @"""" + value + @"""";
            }
            return s;
        }

        public string EscapeCSVQuotedStr(string value)
        {
            string s = "";
            if (value != null && 0 < value.Length)
            {
                value = value.Replace("\\", "\\\\");
                value = value.Replace("\"", "\"\"");
                s = @"""" + value + @"""";
            }
            return s;
        }
    }
    public class ReportStream
    {
        PerfFrame.RMX m_rmx;
        //Rider m_rider;
        public PerfFrame.PerfData m_datapdAcc = PerfFrame.PerfDataCreate();
        public PerfFrame.PerfData m_datapdAvg = PerfFrame.PerfDataCreate();
        public PerfFrame.PerfData m_datapdMax = PerfFrame.PerfDataCreate();
        public PerfFrame.PerfData m_datapdMin = PerfFrame.PerfDataCreate();

        StatFlags[] Items = {
            StatFlags.Speed, // = (1L << 7),
            StatFlags.Watts, // = (1L << 10),
            StatFlags.Watts_Wkg, // = (1L << 13),
            StatFlags.Watts_Load, // = (1L << 14),
            StatFlags.PercentAT, // = (1L << 35),
            StatFlags.HeartRate, // = (1L << 15),
            StatFlags.PulsePower, // = (1L << 22),
            StatFlags.Cadence, // = (1L << 18),
            StatFlags.Distance, // = (1L << 3),		// Total Distance run. 
            StatFlags.Grade, // = (1L << 5),
            StatFlags.Wind, // = (1L << 6),
            StatFlags.DragFactor, // = (1L << 23),
            StatFlags.TSS, // = (1L << 41),
            StatFlags.Calories, // = (1L << 21),
            StatFlags.SS, // = (1L << 24),
            StatFlags.SSLeft, // = (1L << 25),
            StatFlags.SSRight, // = (1L << 26),
            StatFlags.SSLeftATA, // = (1L << 29),
            StatFlags.SSRightATA, // = (1L << 30),
            //StatFlags.LeftPower, // = (1L << 33),
            //StatFlags.RightPower, // = (1L << 34),
            //StatFlags.RawSpinScan, // = (1L << 39),
            //StatFlags.CadenceTiming, // = (1L << 40),
            StatFlags.FrontGear, // = (1L << 36),
            StatFlags.RearGear, // = (1L << 37),
            StatFlags.GearInches // = (1L << 38),
        };
        string DistanceUnits(bool bCaps) { return RM1_Settings.General.Metric ? (bCaps ? "Kilometers" : "km") : (bCaps ? "Miles" : "miles"); }
        string SpeedUnits(bool bCaps) { return RM1_Settings.General.Metric ? (bCaps ? "KPH" : "kph") : (bCaps ? "MPH" : "mph"); }
        string WeightUnits(bool bCaps) { return RM1_Settings.General.Metric ? (bCaps ? "KGS" : "kgs") : (bCaps ? "LBS" : "lbs"); }
        string Units(bool bCaps) { return RM1_Settings.General.Metric ? (bCaps ? "Metric" : "metric") : (bCaps ? "English" : "english"); }
        double ConvertConstLBSToReportUnits() { return RM1_Settings.General.Metric ? ConvertConst.LBStoKGS : 1.0; }
        string AltUnits() { return RM1_Settings.General.Metric ? "m":"ft"; }
      
        FileStream outfile = null;
        private StringBuilder build = null;

        public bool OpenReportFileOut(string fileName)
        {
            try
            {
                outfile = new FileStream(fileName, FileMode.Create);
                if (outfile != null)
                {
                    build = new StringBuilder();
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseReportFileOut();
            }
            return false;
        }

        public void CloseReportFileOut()
        {
            FlushReportBuild();
            if (outfile != null)
            {
                outfile.Close();
                outfile = null;
            }
        }

        public bool FlushReportBuild()
        {
            MemoryStream strm = null;
            try
            {
                string str = build.ToString();
                build.Remove(0, build.Length);
                //convert string to stream
                if (str != null)
                {
                    strm = new MemoryStream();
                    if (strm != null)
                    {
                        byte[] strmByte = null;
                        strmByte = StreamConverter.ConvertStringToByteArray(str);
                        if (strmByte != null)
                        {
                            strm.Write(strmByte, 0, str.Length);
                        }
                        byte[] bytes = null;
                        bytes = strm.ToArray();
                        if (bytes != null)
                        {
                            outfile.Write(bytes, 0, bytes.Length);
                        }
                        strm.Close();
                        return true;
                    }
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                if (strm != null)
                {
                    strm.Close();
                }
            }
            return false;
        }

        bool GetEntry(int idx, ref string item, ref string itempresent, ref string min, ref string avg, ref string max, StatFlags includeflags)
        {
            bool res = false;
            try
            {
                StatFlags entryflag = Items[idx];
                switch (entryflag)
                {
                    case StatFlags.Speed: // = (1L << 7),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.Speed; min = string.Format("{0:F1}", m_datapdMin.Speed * ConvertConst.MetersPerSecondToMPHOrKPH); avg = string.Format("{0:F1}", m_datapdAvg.Speed * ConvertConst.MetersPerSecondToMPHOrKPH); max = string.Format("{0:F1}", m_datapdMax.Speed * ConvertConst.MetersPerSecondToMPHOrKPH); res = true;
                        }
                        break;
                    case StatFlags.Watts: // = (1L << 10),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.Watts; min = string.Format("{0:F1}", m_datapdMin.Watts); avg = string.Format("{0:F1}", m_datapdAvg.Watts); max = string.Format("{0:F1}", m_datapdMax.Watts); res = true;
                        }
                        break;
                    case StatFlags.Watts_Wkg: // = (1L << 13),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            
                            item = xtag.Watts_Wkg; itempresent = "Watts/kg";
                            if (m_rmx.Info.Weight > 0)
                            {
                                min = string.Format("{0:F1}", m_datapdMin.Watts / (m_rmx.Info.Weight * ConvertConst.LBStoKGS ));
                                avg = string.Format("{0:F1}", m_datapdAvg.Watts / (m_rmx.Info.Weight * ConvertConst.LBStoKGS ));
                                max = string.Format("{0:F1}", m_datapdMax.Watts / (m_rmx.Info.Weight * ConvertConst.LBStoKGS ));
                            }
                            else
                            {
                                min = max = avg = "kgs=0";
                            }
                            res = true;
                        }
                        break;
                    case StatFlags.Watts_Load: // = (1L << 14),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.Watts_Load; min = string.Format("{0:F1}", m_datapdMin.Watts_Load); avg = string.Format("{0:F1}", m_datapdAvg.Watts_Load); max = string.Format("{0:F1}", m_datapdMax.Watts_Load); res = true;
                        }
                        break;
                    case StatFlags.PercentAT: // = (1L << 35),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.PercentAT; min = string.Format("{0:F1}%", m_datapdMin.PercentAT); avg = string.Format("{0:F1}%", m_datapdAvg.PercentAT); max = string.Format("{0:F1}%", m_datapdMax.PercentAT); res = true;
                        }
                        break;
                    case StatFlags.HeartRate: // = (1L << 15),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.HeartRate; min = string.Format("{0:F1}", m_datapdMin.HeartRate); avg = string.Format("{0:F1}", m_datapdAvg.HeartRate); max = string.Format("{0:F1}", m_datapdMax.HeartRate); res = true;
                        }
                        break;
                    case StatFlags.PulsePower: // = (1L << 22),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.PulsePower; min = string.Format("{0:F1}", m_datapdMin.PulsePower); avg = string.Format("{0:F1}", m_datapdAvg.PulsePower); max = string.Format("{0:F1}", m_datapdMax.PulsePower); res = true;
                        }
                        break;
                    case StatFlags.Cadence: // = (1L << 18),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.Cadence; min = string.Format("{0:F1}", m_datapdMin.Cadence); avg = string.Format("{0:F1}", m_datapdAvg.Cadence); max = string.Format("{0:F1}", m_datapdMax.Cadence); res = true;
                        }
                        break;
                    case StatFlags.Distance: // = (1L << 3),		// Total Distance run. 
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.Distance; min = string.Format("{0:F1}", m_datapdMin.Distance * ConvertConst.MetersToMilesOrKilometers); avg = string.Format("{0:F1}", m_datapdAvg.Distance * ConvertConst.MetersToMilesOrKilometers); max = string.Format("{0:F1}", m_datapdMax.Distance * ConvertConst.MetersToMilesOrKilometers); res = true;
                        }
                        break;
                    case StatFlags.Grade: // = (1L << 5),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.Grade; min = string.Format("{0:F1}", m_datapdMin.Grade); avg = string.Format("{0:F1}", m_datapdAvg.Grade); max = string.Format("{0:F1}", m_datapdMax.Grade); res = true;
                        }
                        break;
                    case StatFlags.Wind: // = (1L << 6),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.Wind; min = string.Format("{0:F1}", m_datapdMin.Wind * ConvertConst.MetersToMilesOrKilometers); avg = string.Format("{0:F1}", m_datapdAvg.Wind * ConvertConst.MetersToMilesOrKilometers); max = string.Format("{0:F1}", m_datapdMax.Wind * ConvertConst.MetersToMilesOrKilometers); res = true;
                        }
                        break;
                    case StatFlags.DragFactor: // = (1L << 23),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = xtag.DragFactor; itempresent = "Drag Factor"; min = string.Format("{0:F1}", m_datapdMin.DragFactor); avg = string.Format("{0:F1}", m_datapdAvg.DragFactor); max = string.Format("{0:F1}", m_datapdMax.DragFactor); res = true;
                        }
                        break;
                    case StatFlags.TSS: // = (1L << 41),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = xtag.TSS; itempresent = "Training Stress Score"; min = string.Format("{0:F1}", m_datapdMin.TSS); avg = string.Format("{0:F1}", m_datapdAvg.TSS); max = string.Format("{0:F1}", m_datapdMax.TSS); res = true;
                        }
                        break;
                    case StatFlags.Calories: // = (1L << 21),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.Calories; min = string.Format("{0:F1}", m_datapdMin.Calories); avg = string.Format("{0:F1}", m_datapdAvg.Calories); max = string.Format("{0:F1}", m_datapdMax.Calories); res = true;
                        }
                        break;
                    case StatFlags.SS: // = (1L << 24),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.SS; min = string.Format("{0:F1}", m_datapdMin.SS); avg = string.Format("{0:F1}", m_datapdAvg.SS); max = string.Format("{0:F1}", m_datapdMax.SS); res = true;
                        }
                        break;
                    case StatFlags.SSLeft: // = (1L << 25),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = itempresent = xtag.SSLeft; itempresent = "SpinScan Left";  min = string.Format("{0:F1}", m_datapdMin.SSLeft); avg = string.Format("{0:F1}", m_datapdAvg.SSLeft); max = string.Format("{0:F1}", m_datapdMax.SSLeft); res = true;
                        }
                        break;
                    case StatFlags.SSRight: // = (1L << 26),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = xtag.SSRight; itempresent = "SpinScan Right"; min = string.Format("{0:F1}", m_datapdMin.SSRight); avg = string.Format("{0:F1}", m_datapdAvg.SSRight); max = string.Format("{0:F1}", m_datapdMax.SSRight); res = true;
                        }
                        break;
                    case StatFlags.SSLeftATA: // = (1L << 29),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item =  xtag.SSLeftATA; itempresent = "SS Left Ave Torque Angle"; min = string.Format("{0:F1}", m_datapdMin.SSLeftATA); avg = string.Format("{0:F1}", m_datapdAvg.SSLeftATA); max = string.Format("{0:F1}", m_datapdMax.SSLeftATA); res = true;
                        }
                        break;
                    case StatFlags.SSRightATA: // = (1L << 30),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = xtag.SSRightATA; itempresent = "SS Right Ave Torque Angle"; min = string.Format("{0:F1}", m_datapdMin.SSRightATA); avg = string.Format("{0:F1}", m_datapdAvg.SSRightATA); max = string.Format("{0:F1}", m_datapdMax.SSRightATA); res = true;
                        }
                        break;
                    //case StatFlags.LeftPower: // = (1L << 33),
                    //case StatFlags.RightPower: // = (1L << 34),
                    //case StatFlags.RawSpinScan: // = (1L << 39),
                    //case StatFlags.CadenceTiming: // = (1L << 40),
                    //    if ((includeflags & entryflag) != StatFlags.Zero)
                    //    {
                    //        item = xtag.CadenceTiming; min = string.Format("{0:F1}", m_datapdMin.CadenceTiming); avg = string.Format("{0:F1}", m_datapdAvg.CadenceTiming); max = string.Format("{0:F1}", m_datapdMax.CadenceTiming); res = true;
                    //    }
                    //    break;

                    case StatFlags.FrontGear: // = (1L << 36),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item  = xtag.FrontGear; itempresent = "Front Gear";  min = string.Format("{0:F1}", m_datapdMin.FrontGear); avg = string.Format("{0:F1}", m_datapdAvg.FrontGear); max = string.Format("{0:F1}", m_datapdMax.FrontGear); res = true;
                        }
                        break;
                    case StatFlags.RearGear: // = (1L << 37),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = xtag.RearGear; itempresent = "Rear Gear"; min = string.Format("{0:F1}", m_datapdMin.RearGear); avg = string.Format("{0:F1}", m_datapdAvg.RearGear); max = string.Format("{0:F1}", m_datapdMax.RearGear); res = true;
                        }
                        break;
                    case StatFlags.GearInches: // = (1L << 38),
                        if ((includeflags & entryflag) != StatFlags.Zero)
                        {
                            item = xtag.GearInches; itempresent = "Gear Inches";  min = string.Format("{0:F1}", m_datapdMin.GearInches); avg = string.Format("{0:F1}", m_datapdAvg.GearInches); max = string.Format("{0:F1}", m_datapdMax.GearInches); res = true;
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
            return res;
        }

        //public void DoReport(ref PerfFrame.RMX rmx, ref PerfFrame.PerfData datapdAcc, ref PerfFrame.PerfData datapdAvg, ref PerfFrame.PerfData datapdMin, ref PerfFrame.PerfData datapdMax, StatFlags selected)
        public void DoReport(ref PerfFrame.RMX rmx, Perf perf, StatFlags selected)
        {

            m_rmx = rmx;
            //m_rider = rider;
            m_datapdAcc = perf.m_datapdAcc;
            m_datapdAvg = perf.m_datapdAvg;
            m_datapdMax = perf.m_datapdMax;
            m_datapdMin = perf.m_datapdMin;

            string datarows = "";
            string item = "", itempresent = "", min = "", avg = "", max = "";
            for (int i = 0; i < Items.Length; i++)
            {
                if (GetEntry(i, ref item, ref itempresent,ref min, ref avg, ref max, selected))
                    datarows += PutReportDataRow(item, itempresent, min, avg, max);
            }


            string laprows = "";
            int cnt = perf.m_LapTimes.Count;
            if (cnt > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    if(0 == (i % 5))
                    {
                        if (i > 0)
                            laprows += string.Format(@"	</tr>{0}", Environment.NewLine);
                        laprows += string.Format(@"	<tr>{0}", Environment.NewLine);
                    }
                    double laptime = perf.m_LapTimes[i];
                    laprows += string.Format(@"		<td align=""center""><font size=""2"" face=""Verdana"">{0}@ {1:F1}</font></td>{2}",
                        i + 1, Statistics.SecondsToTimeString(laptime), Environment.NewLine);
                }
            }

            build.Append(PutReport(
                PutHead("PERFORMANCE REPORT " + m_rmx.Info.CourseName) +
                PutBody(
                    PutReportHeader() +
                    PutReportData(
                        PutReportDataRowLabel("Item", "Min", "Avg", "Max") +
                        datarows
                        ) +
                    PutReportLaps(laprows)
                    ) +
                PutReportFooter()
            ));
        }

        string PutReport(string content)
        {
            return string.Format(
@"<html>
{0}
</html>
", content);

        }
        string PutHead(string title)
        {
            return string.Format(
@"<head>
<meta http-equiv=""Content-Language"" content=""en-us"">
<meta http-equiv=""Content-Type"" content=""text/html; charset=windows-1252"">
<title>{0}</title>
</head>", title);
        }
        string PutReportFooter()
        {
            return string.Format(
@"
<br>
<table width=""650"" border=""0"" cellspacing=""1"" cellpadding=""0"">	<tr>
<td><img src=""aboutyou.png"" align=""left"" height=""40""></td><td><img src=""urllogosmall.png"" align=""right"" height=""20""></td>	</tr>
</table>");
        }

        string PutBody(string content)
        {
            return string.Format(
@"<body>
{0}
</body>", content);
        }

        string PutReportHeader()
        {
            string head1 = string.Format(
@"<table border=""0"" width=""650"" cellspacing=""1"">
<tr>
<td><img src=""tourlogosmall.png"" align=""left"" width=""400"" height=""100""></td><td>
<p class=MsoTitle align=""center""><font face=""Verdana""><b>PERFORMANCE REPORT</b><br>
{0}<br>
</font><span style=""font-weight: 400""><font face=""Verdana"" size=""2"">
<br>
Date/Time: {1}</font></p>
</td>
</tr>
</table>
<hr align=""left"" width=""650"">
<table border=""0"" width=""650"">
<tr>
<td align=""center"" width=""179"">&nbsp;</td>
		<td align=""center"" width=""264""><b><font size=""2"" face=""Verdana"">Rider
		Data</font></b></td>
		<td align=""center"">&nbsp;</td>
	</tr>
	<tr>
		<td align=""center"" width=""179"">&nbsp;</td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">Athlete: 
		{2}</font></td>
		<td align=""center"">&nbsp;</td>
	</tr>
	<tr>
		<td align=""center"" width=""179""><font size=""2"" face=""Verdana"">Age: {3}</font></td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">Weight: {4:F1} {22}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">Gender: {5}</font></td>
	</tr>
	<tr>
		<td align=""center"" width=""179""><font size=""2"" face=""Verdana"">Aet (Watts): {6:F1}</font></td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">FTP (Watts): {7:F1}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">DragFactor: {8:F1}</font></td>
	</tr>
	<tr>
		<td align=""center"" width=""179""><font size=""2"" face=""Verdana"">HR-Resting: {9:F1}</font></td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">HR-Maximum: {10:F1}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">HR-Anaerobic: {11:F1}</font></td>
	</tr>
	<tr>
		<td align=""center"" width=""179""><font size=""2"" face=""Verdana"">Zone 1 (50-60%) </font></td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">{12:F1}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">{13:F1}</font></td>
	</tr>
	<tr>
		<td align=""center"" width=""179""><font size=""2"" face=""Verdana"">Zone 2 (60-70%) </font></td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">{14:F1}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">{15:F1}</font></td>
	</tr>
	<tr>
		<td align=""center"" width=""179""><font size=""2"" face=""Verdana"">Zone 3 (70-80%) </font></td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">{16:F1}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">{17:F1}</font></td>
	</tr>
	<tr>
		<td align=""center"" width=""179""><font size=""2"" face=""Verdana"">Zone 4 (80-90%) </font></td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">{18:F1}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">{19:F1}</font></td>
	</tr>
	<tr>
		<td align=""center"" width=""179""><font size=""2"" face=""Verdana"">Zone 5 (90-100%) </font></td>
		<td align=""center"" width=""264""><font size=""2"" face=""Verdana"">{20:F1}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">{21:F1}</font></td>
	</tr>
</table>",
         m_rmx.Info.CourseName, // 0
         m_rmx.Header.Date, // 1
         m_rmx.Info.RiderName, // 2
         m_rmx.Info.Age, // 3
         m_rmx.Info.Weight * ConvertConstLBSToReportUnits(), // 4
         m_rmx.Info.Gender, // 5
         m_rmx.Info.PowerAnT, // 6
         m_rmx.Info.PowerFTP, // 7
         m_rmx.Info.DragFactor, // 8
         m_rmx.Info.HrMin, // 9
         m_rmx.Info.HrMax, // 10
         m_rmx.Info.HrAnT, // 11
         m_rmx.Info.HrZone1, // 12
         m_rmx.Info.HrZone2 - 1, // 13
         m_rmx.Info.HrZone2, // 14
         m_rmx.Info.HrZone3 - 1, // 15
         m_rmx.Info.HrZone3, // 16
         m_rmx.Info.HrZone4 - 1, // 17
         m_rmx.Info.HrZone4, // 18
         m_rmx.Info.HrZone5 - 1, // 19
         m_rmx.Info.HrZone5, // 20
         m_rmx.Info.HrMax, // 21
         WeightUnits(false)); // 22

            string head2 = string.Format(
@"<hr align=""left"" width=""650"">
<table border=""0"" width=""650"">
	<tr>
		<td align=""center"" width=""228"">&nbsp;</td>
		<td align=""center"" width=""173""><b><font size=""2"" face=""Verdana"">Course&nbsp;Data</font></b></td>
		<td align=""center"">&nbsp;</td>
	</tr>
	<tr>
		<td align=""center"" width=""228""><font size=""2"" face=""Verdana"">Name: {0}</font></td>
		<td align=""center"" width=""173""><font size=""2"" face=""Verdana"">Distance: 
		{1:F1} {12}</font></td>
	</tr>
	<tr>
		<td align=""center"" width=""228""><font size=""2"" face=""Verdana"">Units: 
		{2}</font></td>
		<td align=""center"" width=""173""><font size=""2"" face=""Verdana"">Laps: {3:F1}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">Lap Length: {4:F1} {12}</font></td>
	</tr>
	<tr>
		<td width=""169"" align=""center""><font face=""Verdana"" size=""2"">Min Grade: {5:F1}%</font></td>
		<td width=""165"" align=""center""><font face=""Verdana"" size=""2"">Avg Grade: {6:F1}%</font></td>
		<td align=""center""><font face=""Verdana"" size=""2"">Max Grade: {7:F1}%</font></td>
	</tr>
	<tr>
		<td width=""169"" align=""center""><font face=""Verdana"" size=""2"">Min Wind: {8:F1} {13}</font></td>
		<td width=""165"" align=""center""><font face=""Verdana"" size=""2"">Avg Wind: {9:F1} {13}</font></td>
		<td align=""center""><font face=""Verdana"" size=""2"">Max Wind: {10:F1} {13}</font></td>
	</tr>
<tr>
<td align=""center"" width=""228"">&nbsp;</td>
<td align=""center"" width=""173""><font face=""Verdana"" size=""2"">Total Climbing: {11:F1} {14}</font></td>
<td align=""center"" width=""228"">&nbsp;</td>
</tr>
</table>",
         m_rmx.Info.CourseName, // 0
         m_rmx.course.Laps * m_rmx.course.TotalX * ConvertConst.MetersToMilesOrKilometers, // 1
         Units(true), // 2
         m_rmx.course.StringLaps,  // 3
         m_rmx.course.TotalX * ConvertConst.MetersToMilesOrKilometers, // 4 
         m_datapdMin.Grade, // 5
         m_datapdAvg.Grade, // 6
         m_datapdMax.Grade, // 7
         m_datapdMin.Wind * ConvertConst.MetersPerSecondToMPHOrKPH, // 8
         m_datapdAvg.Wind * ConvertConst.MetersPerSecondToMPHOrKPH, // 9
         m_datapdMax.Wind * ConvertConst.MetersPerSecondToMPHOrKPH, // 10
         m_rmx.course.Laps * m_rmx.course.Alt, // 11
         DistanceUnits(true), // 12
         SpeedUnits(false), // 13
         AltUnits());//14

// for head3
//    <tr>
//        <td align=""center"" colspan=""3"" ><b><font size=""2"" face=""Verdana"">( {7} )</font></b></td>
//    </tr>
            string head3 = string.Format(
@"<hr align=""left"" width=""650"">
<table border=""0"" width=""650"">
    <tr>
        <td align=""center"" width=""193"">&nbsp;</td>
        <td align=""center""><b><font size=""2"" face=""Verdana"">Performance&nbsp;Statistics</font></b></td>
        <td align=""center"" width=""203"">&nbsp;</td>
    </tr>
    <tr>
        <td align=""center"" width=""193"">&nbsp;<font size=""2"" face=""Verdana"">Total&nbsp;{5}&nbsp;Ridden:&nbsp;{0:F1}</font></td>
        <td align=""center""><font size=""2"" face=""Verdana"">Device: {6}</font></td>
        <td align=""center"" width=""203""><font size=""2"" face=""Verdana"">Lap&nbsp;Avg:&nbsp;{1:F1}</font></td>
	</tr>
	<tr>
        <td align=""center"" width=""193""><font size=""2"" face=""Verdana"">Rolling&nbsp;Calibration: {2:F2}</font></td>
		<td align=""center""><font size=""2"" face=""Verdana"">Finish&nbsp;Time:&nbsp;{3:F1}</font></td>
		<td align=""center"" width=""203""><font size=""2"" face=""Verdana"">Calories:&nbsp;{4:F1}</font></td>
	</tr>
<tr>
        <td align=""center"" width=""193""><font size=""2"" face=""Verdana""></td>
		<td align=""center""><font size=""2"" face=""Verdana"">Actual Climbing:&nbsp;{8:F1} {9}</font></td>
		<td align=""center"" width=""203""><font size=""2"" face=""Verdana""></td>
	</tr>
</table>",
         m_datapdAcc.Distance * ConvertConst.MetersToMilesOrKilometers, // 0
         m_datapdAvg.LapTime < 0.1 ? Statistics.SecondsToTimeString(m_datapdAcc.TimeAcc) : Statistics.SecondsToTimeString(m_datapdAvg.LapTime + 0.05f), // 1
         (Math.Abs(m_rmx.Info.RawCalibrationValue) / 100.0) + 0.005f, // 2
         Statistics.SecondsToTimeString(m_datapdAcc.TimeAcc), // 3
         m_datapdMax.Calories, // 4
         DistanceUnits(true), // 5
         m_rmx.Info.DeviceType == (uint)RM1.DeviceType.COMPUTRAINER ? "CompuTrainer" : 
            m_rmx.Info.DeviceType == (uint)RM1.DeviceType.VELOTRON ? "Velotron" : "Unknown", // 6
         m_rmx.Info.ModeStr, // 7
         m_rmx.course.GetAccomplishedClimbingInMeters(m_datapdAcc.Distance)* ConvertConst.MetersToMetersOrFeet,
         AltUnits());//9
            string spacing = string.Format(
@"<table border=""1"" width=""650"" cellspacing=""1"">
</table>");
            return (head1 + head2 + head3 + spacing);
        }

        string PutReportData(string content)
        {
            return string.Format(
@"<table border=""1"" width=""650"" cellspacing=""1"">
{0}
</table>", content);
        }

        string PutReportDataRowLabel(string item, string min, string avg, string max)
        {
            return string.Format(
@"	<tr>
<th valign=""top""><p align=""center"">
<font face=""Verdana"" size=""2""><b>Item</b></font></th><th valign=""top""><p align=""center"">
<font face=""Verdana"" size=""2""><b>Min</b></font></th><th valign=""top""><p align=""center"">
<font face=""Verdana"" size=""2""><b>Avg</b></font></th><th valign=""top""><p align=""center"">
<font face=""Verdana"" size=""2""><b>Max</b></font></th>	</tr>", item, min, avg, max);
        }


        string PutReportDataRow(string item, string itempresent, string min, string avg, string max)
        {
            return string.Format(
@"	<tr>
		<td align=""center"" width=""152""><font face=""Verdana"" size=""2"">{0}</font></td>
		<td align=""center"" width=""169""><font face=""Verdana"" size=""2"">{1}</font></td>
		<td align=""center"" width=""165""><font face=""Verdana"" size=""2"">{2}</font></td>
		<td align=""center""><font face=""Verdana"" size=""2"">{3}</font></td>
	</tr>", itempresent, min, avg, max);
        }


        string PutReportLaps(string content)
        {
            return string.Format(
@"<hr align=""left"" width=""650"">
<table border=""0"" width=""650"">
    <tr>
        <td align=""center"" width=""193"">&nbsp;</td>
        <td align=""center""><b><font size=""2"" face=""Verdana"">Lap&nbsp;Times</font></b></td>
        <td align=""center"" width=""203"">&nbsp;</td>
    </tr>
</table>
<table border=""1"" width=""650"" cellspacing=""1"">
{0}
</table>", content);
        }

    }
#endif

class StreamConverter
    {
        public StreamConverter()
        {
        }

        public static byte[] ConvertStringToByteArray(string strReport)
        {
            byte[] bytes = null;

            if (strReport != null)
            {
                int iLen = strReport.Length;

                bytes = new byte[iLen];
                if (bytes != null)
                {
                    for (int x = 0; x < iLen; x++)
                    {
                        bytes[x] = (byte)strReport[x];
                    }
                }
            }

            return (bytes);
        }
    }

#if THREADTESTING
    public class RawStream2
    {
        FileStream outfile = null;
        FileStream infile = null;
        private BinaryWriter bw = null;
        private BinaryReader br = null;
        //int RawType = 1;
        private object _obj = null;
        private System.Type _type = null;

        public void AddRawField(string tag, object obj)
        {
            byte[] val = StructToByteArray(obj);
            bw.Write(val);
        }

        public void GetRawField(string tag, ref object obj)
        {
            obj = GetNextStructureValue(obj.GetType());
        }

        public void CloseRawFileOut()
        {
            FlushRawOut();
            if (bw != null)
            {
                bw.Close();
                bw = null;
            }
            if (outfile != null)
            {
                outfile.Close();
                outfile = null;
            }
            GC.Collect();
        }

        public bool OpenRawFileOut(string fileName)
        {
            try
            {
                CloseRawFileOut();
                outfile = new FileStream(fileName, FileMode.Create);
                if (outfile != null)
                {
                    bw = new BinaryWriter(outfile);
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseRawFileOut();
            }
            return false;
        }

        //
        //
        public bool FlushRawOut()
        {
            if (bw != null)
            {
                bw.Flush();
                return true;
            }
            return false;
        }

        public bool OpenRawFileIn(string fileName)
        {
            try
            {
                CloseRawFileIn();
                infile = new FileStream(fileName, FileMode.Open);
                if (infile != null)
                {
                    br = new BinaryReader(infile);
                }
            }
            catch (Exception exc)
            {
                string msg = exc.Message;
                CloseRawFileIn();
            }
            return false;
        }

        public void CloseRawFileIn()
        {
            if (br != null)
            {
                br.Close();
                br = null;
            }
            if (infile != null)
            {
                infile.Close();
                infile = null;
            }
            GC.Collect();
        }

        private byte[] StructToByteArray(object obj)
        {
            try
            {
                _obj = obj;
                // This function copies the structure data into a byte[] 

                //Set the buffer to the correct size 
                byte[] buffer = new byte[Marshal.SizeOf(_obj)];

                //Allocate the buffer to memory and pin it so that GC cannot use the 
                //space (Disable GC) 
                GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                // copy the struct into int byte[] mem alloc 
                Marshal.StructureToPtr(_obj, h.AddrOfPinnedObject(), false);

                h.Free(); //Allow GC to do its job 

                return buffer; // return the byte[]. After all that's why we are here 
                // right. 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool EOF				//End of File
        {
            get
            {
                if (infile != null)
                {
                    if (infile.Position >= infile.Length)
                        infile.Close();
                }

                return infile == null;
            }
        }
        public object GetNextStructureValue(System.Type type)
        {
            _type = type;
            byte[] buffer = new byte[Marshal.SizeOf(_type)];

            object obj = null;
            try
            {
                if (EOF)
                    return null;
                infile.Read(buffer, 0, buffer.Length);
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                obj = (object)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), _type);
                handle.Free();
                if (infile.Position >= infile.Length)
                    CloseRawFileIn();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
    public static class PerfThread
    {
        static LinkedList<RawStream2> ms_List = new LinkedList<RawStream2>();

        static Thread ms_Thread;
        private static readonly AutoResetEvent ms_WaitEvent = new AutoResetEvent(false);

        public static void Wake()
        {
            ms_WaitEvent.Set();
        }
        public static void End()
        {
            ms_Thread = null;
            Wake();
        }
        public static void Add(ref RawStream2 obj)
        {
            ms_List.AddLast(obj);
        }

        public static void Start()
        {
            try
            {
                if (ms_Thread == null)
                {
                    ms_Thread = new Thread(new ThreadStart(ThreadLoop));
                    ms_Thread.Start();
                }
            }
            catch { }
        }

        private static void ThreadLoop()
        {
            while (true)
            {
                ms_WaitEvent.WaitOne();
                try
                {
                    for (LinkedListNode<RawStream2> n = ms_List.First; n != null; n = n.Next)
                    {
                        n.Value.FlushRawOut();
                    }
                }
                catch { }
                if (ms_Thread == null)
                    break;
            }
        }
    }
#endif

	public class PerfInfo
	{
		static Perf ms_Perf;

		public DateTime Date	{ get; protected set; }
		public String CourseName { get; protected set; }
		public CourseType CourseType { get; protected set; }
		public String Ridername { get; protected set; }
		public double TimeMS { get; protected set; }
		public String FileName { get; protected set; }
		public String CourseHash { get; protected set; }
		public CourseInfo CourseInfo { get; protected set; }

		PerfInfo()
		{
		}
		bool Init(Perf perf)
		{
			return true;
		}
		public static PerfInfo Load( String filename )
		{
			if (ms_Perf == null)
				ms_Perf = new Perf();

			if (!ms_Perf.LoadHeaderRawTemps(filename))
				return null;

			PerfInfo pinfo = new PerfInfo();
			if (!pinfo.Init(ms_Perf))
				return null;
		
			pinfo.Date = ms_Perf.LoadedRMX.Header.Date;
            pinfo.CourseName = ms_Perf.LoadedRMX.Info.CourseName;
            pinfo.CourseType = (CourseType)ms_Perf.LoadedRMX.Info.CourseType;
            pinfo.Ridername = ms_Perf.LoadedRMX.Info.RiderName;
			pinfo.TimeMS = ms_Perf.LoadedRMX.Info.TimeMS;
			pinfo.CourseInfo = ms_Perf.CourseInfo;

			return pinfo;
		}
	}

	public static class Performances
	{
		public delegate void PerfInfoAdded(PerfInfo perfinfo);
		public static event PerfInfoAdded OnPerfInfoAdded;

		public static List<PerfInfo> AllPerfInfo { get; private set; }


		public static Dictionary<String, PerfInfo> FileDB { get; private set; }
		static Performances()
		{
			FileDB = new Dictionary<string, PerfInfo>();
			AllPerfInfo = new List<PerfInfo>(); 
		}
		static BackgroundWorker ms_PerformanceBackground;
		public static void Scan()
		{
			if (ms_PerformanceBackground != null)
				return;
			BackgroundWorker bw = new BackgroundWorker();
			ms_PerformanceBackground = bw;
			bw.DoWork += new DoWorkEventHandler(bw_Performance_ScanWork);

			bw.WorkerReportsProgress = true;
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_Performance_RunWorkerCompleted);
			bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
			bw.RunWorkerAsync(RacerMatePaths.PerformancesFullPath);
		}
		private static void bw_Performance_ScanWork(object sender, DoWorkEventArgs e)
		{
            App.SetDefaultCulture();

            BackgroundWorker worker = sender as BackgroundWorker;
			String dir = (String)e.Argument;

			int cnt = 0;
			string[] files = Directory.GetFiles(dir, "*.RMP");
			int total = files.Length;

			foreach (string filename in files)
			{
				PerfInfo pinfo;
				String lkey = filename.ToLower();
				if (!FileDB.TryGetValue(lkey, out pinfo))
				{
					// OK we don't have it yet... let try loading up the performance.
					pinfo = PerfInfo.Load(filename);
					if (pinfo != null)
					{
						FileDB[lkey] = pinfo;
						worker.ReportProgress(cnt * 100 / total, pinfo);
					}
					else
						FileDB[lkey] = null;
				}
			}
		}
		private static void bw_Performance_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ms_PerformanceBackground = null;
		}
		private static void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			PerfInfo pinfo = (PerfInfo)e.UserState;
			AllPerfInfo.Add(pinfo);
			if (OnPerfInfoAdded != null)
			{
				OnPerfInfoAdded(pinfo);
			}
		}
	}

}

/* example for cloning structs with arrays by using array container
class ArrayContainer
{
    public byte[] Array1 { get; set; }
    public byte[] Array2 { get; set; }

    public ArrayContainer DeepCopy()
    {
        ArrayContainer result = new ArrayContainer();
        foreach (var property in this.GetType().GetProperties())
        {
            var oldData = property.GetValue(this, null) as byte[];
            if (oldData != null)
            {
                // Copy data with .ToArray() actually copies data.
                property.SetValue(result, oldData.ToArray(), null);
            }
        }

        return result;
    }
}
ArrayContainer container = new ArrayContainer();
container.Array1 = new byte[] { 1 };
container.Array2 = new byte[] { 2 };
ArrayContainer copy = container.DeepCopy();
copy.Array1[0] = 3;
Debug.WriteLine("{0}, {1}, {2}, {3}", container.Array1[0], container.Array2[0], copy.Array1[0], copy.Array2[0]);
*/