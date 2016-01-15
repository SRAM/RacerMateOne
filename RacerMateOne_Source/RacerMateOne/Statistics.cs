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



namespace RacerMateOne
{
	public enum RaceStates : int
	{
		Unknown = 0,
		Loading,
		PreRace,
		Countdown,
		Racing,
		AllFinished,
		AllSaved,

		Max
	}

    // **** WARNING: changing the values of enum variables here will break Performance File format. ****
    // **** Please only add new ones. Do NOT exchange or reuse values. ****
	[Flags]
	public enum StatFlags : ulong
	{
		Zero			= 0L,

		Time			= (1L << 0),		// Total time of the race.
		LapTime			= (1L << 1),		// Current running total of the lap
		Lap				= (1L << 2),
		Distance		= (1L << 3),		// Total Distance run. 
		Lead			= (1L << 4),		// Race order has changed
		Grade			= (1L << 5),
		Wind			= (1L << 6),

		Speed			= (1L << 7),
		Speed_Avg		= (1L << 8),
		Speed_Max		= (1L << 9),

		Watts			= (1L << 10),
		Watts_Avg		= (1L << 11),
		Watts_Max		= (1L << 12),
		Watts_Wkg		= (1L << 13),
		Watts_Load		= (1L << 14),

		HeartRate		= (1L << 15),
		HeartRate_Avg	= (1L << 16),
		HeartRate_Max	= (1L << 17),

		Cadence			= (1L << 18),
		Cadence_Avg		= (1L << 19),
		Cadence_Max		= (1L << 20),

		Calories		= (1L << 21),
		PulsePower		= (1L << 22),
		DragFactor		= (1L << 23),
		SS				= (1L << 24),
		SSLeft			= (1L << 25),
		SSRight			= (1L << 26),
		SSLeftSplit		= (1L << 27),
		SSRightSplit	= (1L << 28),
		SSLeftATA		= (1L << 29),
		SSRightATA		= (1L << 30),
		SSLeft_Avg		= (1L << 31),
		SSRight_Avg		= (1L << 32),
		SS_Avg = SSLeft_Avg | SSRight_Avg,
		SS_Stats = SS | SSLeft | SSRight | SSLeftSplit | SSRightSplit | SSLeftATA | SSRightATA | SSLeft_Avg | SSRight_Avg,

		HardwareStatusFlags	= (1L << 33),		// Realtime not stored.
		LapDistance		= (1L << 34),		// Used for the course display.
		
		PercentAT		= (1L << 35),
		
		FrontGear		= (1L << 36),
		RearGear		= (1L << 37),
		Gearing			= FrontGear | RearGear,
		GearInches		= (1L << 38),
		VelotronOnly	= FrontGear | RearGear | GearInches,
		
		Drafting		= (1L << 39),
	
		CadenceTiming	= (1L << 40),
		TSS				= (1L << 41),
		IF				= (1L << 42),
		NP				= (1L << 43),
		TSS_IF_NP		= TSS | IF | NP,

		Bars			= (1L << 44),
		Bars_Shown		= (1L << 45),
		Bars_Avg		= (1L << 46),

		CourseScreenX	= (1L << 47),

		RiderName		= (1L << 48),

		Course			= (1L << 49),

		Order			= (1L << 50), // Global flag only
		HardwareStatus  = (1L << 51), // Something changed in the hardware connected to that unit.

		Finished		= (1L << 52), // Rider finished the race
		TrackDistance	= (1L << 53),

		Disconnected	= (1L << 54), // True if the unit has been disconnected.

		Calibration		= (1L << 55), // Need to read this from the trainer.

		HeartRateAlarm	= (1L << 56),

		Max				= (1L << 57),
		Mask			= Max - 1
	};


	public class Statistics: RM1.IStatsEx
	{
		public static bool Unregistered = false;

		public const Int64 c_CadenceOverrideTime = (Int64)(ConvertConst.SecondToHundredNanosecond * 3);

		public static StatFlags AllChanged = StatFlags.Zero;

		public StatFlags Changed = StatFlags.Zero;
		public StatFlags LastChanged = StatFlags.Zero;
		public StatFlags PerfChanged = StatFlags.Zero;

		public bool HasStarted { get; protected set; }


		static protected double ms_Timer;
		static protected int ms_TimerIntervalCount;
		static public double MasterTimer
		{
			get { return ms_Timer; }
			set
			{
				if (ms_Timer != value)
				{
					ms_Timer = value;
					ms_TimerIntervalCount = 0;
					AllChanged |= StatFlags.Time;
				}
			}
		}
		static public string MasterTimerString { get { return SecondsToTimeString(ms_Timer); } }
		

		public void SetChangedFlags(StatFlags sf)
		{
			m_Mux.WaitOne();
			Changed |= sf;
			PerfChanged |= sf;
			AllChanged |= sf;
			m_Mux.ReleaseMutex();
		}

		public static String SecondsToTimeString(double s)
		{
			double h = Math.Floor(s / 3600.0);
			s -= h * 3600;
			double m = Math.Floor(s / 60);
			s = Math.Round(s - m * 60,1);
			if (s >= 60) s = 59.9;
			
			return String.Format("{0:0}:{1:00}:{2:00.0}", h, m, s);
		}


		public class Average
		{
			public double m_Acc = 0.0;
			private double m_Total = 0.0;
			private float m_Avg = 0.0f;
			public double Min = 0.0;
			public int RoundTo = 1;

			public void Reset() { m_Total = m_Acc = 0.0; m_Avg = 0.0f; }
			public bool Add(double amount, double timeslice)
			{
				if (amount > Min)
				{
					m_Acc += amount * timeslice;
					m_Total += timeslice;
					float n = (float)Math.Round(m_Acc / m_Total, RoundTo);
					if (n != m_Avg)
					{
						m_Avg = n;
						return true;
					}
				}
				return false;
			}
			public void Force(double v)
			{
				m_Avg = (float)v;
				m_Acc = v;
				m_Total = 1.0;
			}
			public static implicit operator double(Average avg)  // implicit digit to byte conversion operator
			{
				return (double)avg.m_Avg;
			}
			public static implicit operator float(Average avg)
			{
				return avg.m_Avg;
			}
		}

		public class Filter
		{
			// this class not used
			public static double SplitTime = 0.0;
			public static double MinValue = 0.01;

			public double Raw { get; protected set; }
			private double m_avga = 0.0;
			private double m_avgb;
			public Filter() {
				Raw = 0.0;
				m_avgb = MinValue;
			}

			public float Set( float val )
			{
				double ftemp = val + (m_avga - m_avgb) * 0.27;
				m_avga += (ftemp - m_avga) * SplitTime;
				m_avgb += (m_avga - m_avgb) * SplitTime;
				if (m_avgb < MinValue)
					m_avgb = MinValue;
				return (float)m_avgb;
			}
		}

		public Statistics()
		{
			/*
			for (int i = 0; i < RM1.BarCount; i++)
			{
				m_BarFilters[i] = new Filter();
			}
			 */
			Reset();
		}

		bool m_bForceGrade = false;
		public void ForceGrade() { m_bForceGrade = true; }
		bool m_bForceWatts = false;
		public void ForceWatts() { m_bForceWatts = true; }

		protected Mutex m_Mux = new Mutex();

		protected Int64 m_Ticks;
		protected float m_SplitTime;
		protected float m_LastSpeed;
		protected float m_Speed;
		protected float m_Cadence;

		protected float m_HeartRate;
		protected float m_Watts;
		protected float m_SS;
		protected float m_SSLeft;
		protected float m_SSRight;
		protected float m_SSLeftSplit;
		protected float m_SSRightSplit;
		protected float[] m_Bars = new float[RM1.BarCount];
		protected bool m_Bars_Shown = false;
		protected float[] m_AverageBars = new float[RM1.BarCount];
		protected double [] m_Bars_Acc = new double[RM1.BarCount];
		protected double m_Bars_TotalTime;

		protected int m_FrontGear;
		protected int m_RearGear;
		protected int m_GearInches;

		protected float[] m_LastBars = new float[RM1.BarCount];
		protected float[] m_LastAverageBars = new float[RM1.BarCount];

		//protected Filter[] m_BarFilters = new Filter[RM1.BarCount];


		protected double m_TimeAcc;

		protected Average m_Speed_Avg = new Average();
		protected float m_Speed_Max;

		protected Average m_Cadence_Avg = new Average();
		protected float m_Cadence_Max;

		protected Average m_HeartRate_Avg = new Average();
		protected float m_HeartRate_Max;

		protected Average m_Watts_Avg = new Average();
		protected float m_Watts_Max;

		protected float m_SSLeftATA = 90.0f;
		protected float m_SSRightATA = 90.0f;

		

		protected float m_Grade = 0.0f;
		protected float m_Watts_Load = 0.0f;
		protected float m_Wind = 0.0f;
		protected bool m_Drafting = false;
		protected bool m_tempDrafting = false;

		protected double m_SSLeft_Acc;
		protected double m_SSRight_Acc;
		protected float m_SSLeft_Avg;
		protected float m_SSRight_Avg;

		protected float m_LastCalories;
		protected float m_TotalCalories;
		protected bool m_bInitCalories;

		protected float m_PulsePower;
		protected float m_NP;
		protected float m_IF;
		protected float m_TSS;

		protected double m_Distance;


		public RM1.StatusFlags HardwareStatusFlags
		{
			get
			{
				return m_Trainer == null ? RM1.StatusFlags.Zero:m_Trainer.StatusFlags;
			}
		}

		protected float m_Lead;
		public float Lead
		{
			get { return m_Lead; }
			set
			{
				if (value == m_Lead || (float.IsNaN(value) && float.IsNaN(m_Lead)))
					return;
				m_Lead = value;
				Changed |= StatFlags.Lead;
				PerfChanged |= StatFlags.Lead;
				AllChanged |= StatFlags.Lead;
			}
		}
		public String DistanceLeadString
		{
			get
			{
				return float.IsNaN(m_Lead) ? "-":RM1_Settings.General.Metric ?
					m_Lead < 1000.0 ? String.Format("{0:F0}m", m_Lead) : String.Format("{0:0.##}", m_Lead * ConvertConst.MetersToKilometers) :
					m_Lead < UseAsFeet ? String.Format("{0:F0}'", m_Lead * ConvertConst.MetersToFeet) :
										  String.Format("{0:0.##}", m_Lead * ConvertConst.MetersToMiles);
			}
		}

		public String DistanceLeadStringDisplay  {
			get  {
				return float.IsNaN(m_Lead) ? m_Distance <= 0 ? "-":"Leading" : RM1_Settings.General.Metric ?
					m_Lead < 1000.0 ? String.Format("{0:F0}m", m_Lead) : String.Format("{0:0.##}", m_Lead * ConvertConst.MetersToKilometers) :
					m_Lead < UseAsFeet ? String.Format("{0:F0}'", m_Lead * ConvertConst.MetersToFeet) :
										  String.Format("{0:0.##}", m_Lead * ConvertConst.MetersToMiles);
			}
		}


		public bool Drafting
		{
			get { return m_tempDrafting; }
			set
			{
				m_tempDrafting = Unit.AllowDrafting && m_State == State.Running ? value : false;
			}
		}

		public bool Disconnected
		{
			get
			{
				return (m_Trainer != null) && (m_Bot == null) ? m_Trainer.NoCommunication:false;
			}
		}

				


		public enum State
		{
			Stopped,
			Running,
			Paused
		};
		State m_State;
		public State CurrentState { get { return m_State; } }

		// Basic info
		private DateTime m_Date = new DateTime();
		public DateTime Date { get {return m_Date; } }
		//private TimeSpan m_TimeSpan = new TimeSpan();
		public String TimerString
		{
			get
			{
				return SecondsToTimeString(m_TimeAcc);
			}
		}
		public double LapTime { get { return m_TimeAcc - m_LastLapTotalTime; } }
		public string LapTimeString
		{
			get
			{
				return SecondsToTimeString(LapTime);
			}
		}
		
		double m_BestLapTime;
		int m_BestLap;
		public int BestLap { get { return m_BestLap; } }
		public double BestLapTime { get { return m_BestLapTime; } }
		public string BestLapTimeString { get { return SecondsToTimeString(m_BestLapTime); } }



		protected double m_LastLapTotalTime;
		protected double m_LastLapTime;
		public double LastLapTime { get { return m_LastLapTime; } } 
		public string LastLapTimeString { get { return SecondsToTimeString( m_LastLapTime); } }

		protected List<double> m_LapTimes = new List<double>();

		public object GetFromFlag(StatFlags flag)
		{
			switch (flag)
			{
				case StatFlags.Time: return m_TimeAcc;
				case StatFlags.LapTime: return LapTime;
				case StatFlags.Lap: return m_Lap;
				case StatFlags.Distance: return m_Distance;
				case StatFlags.Lead: return m_Lead;
				case StatFlags.Grade: return m_Grade;
				case StatFlags.Wind: return m_Wind;
				case StatFlags.Speed: return m_Speed;
				case StatFlags.Speed_Avg: return Speed_Avg;
				case StatFlags.Speed_Max: return m_Speed_Max;
				case StatFlags.Watts: return m_Watts;
				case StatFlags.Watts_Avg: return Watts_Avg;
				case StatFlags.Watts_Max: return m_Watts_Max;
				case StatFlags.Watts_Wkg: return Watts_Wkg;
				case StatFlags.Watts_Load: return m_Watts_Load;
				case StatFlags.HeartRate: return m_HeartRate;
				case StatFlags.HeartRate_Avg: return HeartRate_Avg;
				case StatFlags.HeartRate_Max: return m_HeartRate_Max;
				case StatFlags.Cadence: return m_Cadence;
				case StatFlags.Cadence_Avg: return Cadence_Avg;
				case StatFlags.Cadence_Max: return m_Cadence_Max;
				case StatFlags.Calories: return m_TotalCalories;
				case StatFlags.PulsePower: return m_PulsePower;
				case StatFlags.DragFactor: return DragFactor;
				case StatFlags.SS: return m_SS;
				case StatFlags.SSLeft: return m_SSLeft;
				case StatFlags.SSRight: return m_SSRight;
				case StatFlags.SSLeftSplit: return m_SSLeftSplit;
				case StatFlags.SSRightSplit: return m_SSRightSplit;
				case StatFlags.SSLeftATA: return m_SSLeftATA;
				case StatFlags.SSRightATA: return m_SSRightATA;
				case StatFlags.SS_Avg: return SS_Avg;
				case StatFlags.SSLeft_Avg: return m_SSLeft_Avg;
				case StatFlags.SSRight_Avg: return m_SSRight_Avg;
				//case StatFlags.PercentAT: return m_PercentAT;
				case StatFlags.FrontGear: return m_FrontGear;
				case StatFlags.RearGear: return m_RearGear;
				case StatFlags.GearInches: return m_GearInches;
				//case StatFlags.RawSpinScan: return m_RawSpinScan;
				//case StatFlags.CadenceTiming: return m_CadenceTiming;
				case StatFlags.TSS: return m_TSS;
				case StatFlags.IF: return m_IF;
				case StatFlags.NP: return m_NP;
				case StatFlags.Bars: return m_Bars;
				case StatFlags.Bars_Shown: return m_Bars_Shown;
				case StatFlags.Bars_Avg: return m_AverageBars;
				case StatFlags.CourseScreenX: return m_CourseScreenX;
				case StatFlags.RiderName: return RiderName;
				case StatFlags.Course: return Course;
				case StatFlags.Finished: return m_Finished;
			}
			return null;
		}
		public object GetFromFlagDisplay(StatFlags flag)
		{
			switch (flag)
			{
				case StatFlags.Speed: return m_Speed * ConvertConst.MetersPerSecondToMPHOrKPH;
				case StatFlags.Distance: return m_Distance * ConvertConst.MetersToMilesOrKilometers;
				case StatFlags.Wind: return m_Wind * ConvertConst.MetersPerSecondToMPHOrKPH;
				case StatFlags.Speed_Avg: return m_Speed_Avg * ConvertConst.MetersPerSecondToMPHOrKPH;
				case StatFlags.Speed_Max: return m_Speed_Max * ConvertConst.MetersPerSecondToMPHOrKPH;
			}
			return GetFromFlag(flag);
		}


		// RM1.IStats
		// =============================================================
		public bool Metric
		{
			get { return true; }
			set
			{
			}
		}
		public String DistanceAbbr { get { return RM1_Settings.General.Metric ? "km" : "mi"; } }


		public Int64 Ticks { get { return m_Ticks; } }
		public float SplitTime { get { return m_SplitTime; } }

		public float Speed { get { return m_Speed; } }

		public float Cadence { get { return m_Cadence; } }

		protected Int64 m_CadenceOverrideTime;
		public float Cadence3D
		{
			get
			{
				if (m_Cadence > 0 || m_Speed < ConvertConst.mps_mph_2)
					return m_Cadence;
				if (m_Trainer != null && m_Bot != null && m_Trainer.Type == RM1.DeviceType.VELOTRON)
					return m_Cadence;
				return m_Speed > m_LastSpeed || Unit.LastTime > m_CadenceOverrideTime ?
					m_Speed > ConvertConst.mps_mph_15 ? 90:(float)(90 * Math.Sqrt(m_Speed / ConvertConst.mps_mph_15))
					:0.0f;
			}
		}



		public float HeartRate { get { return m_HeartRate; } }


		public float Watts { get { return m_Watts; } }

		public float SS { get { return m_SS; } }
		public float SSLeft { get { return m_SSLeft; } }
		public float SSRight { get { return m_SSRight; } }
		public float SSLeftSplit { get { return m_SSLeftSplit; } }
		public float SSRightSplit { get { return m_SSRightSplit; } }

		public float[] Bars { get { return m_Bars; } }
		public float[] AverageBars { get { return m_AverageBars; } }

		public bool Bars_Shown { get { return m_Bars_Shown; } }


		public int FrontGear { get { return m_FrontGear; } }	// Velotron only -1 if not valid
		public int RearGear { get { return m_RearGear; } }	// Velotron only -1 if not valie

		public float Calories { get { return m_TotalCalories; } }
		public String CaloriesString
		{
			get
			{
				return String.Format(m_TotalCalories < 1000 ? "{0:0.0}" : "{0:F0}", m_TotalCalories);
			}
		}
		public float PulsePower { get { return m_PulsePower; } }
		public float NP { get { return m_NP; } }
		public float IF { get { return m_IF; } }
		public float TSS { get { return m_TSS; } }

		public int GearInches { get { return m_GearInches; } }

		bool m_Finished;
		public bool Finished { get { return m_Finished; } } 

		public event RM1.IStatsEvent OnUpdate;

		public String Test { get { return "AAA"; } set { OnPropertyChanged("Test"); } }


		public float Grade 
		{ 
			get { return m_Grade; }
			set
			{
			}
		}
		public float Wind
		{
			get { return m_Wind; }
			set
			{
			}
		}

		float m_PercentAT;
		public float PercentAT
		{
			get { return m_PercentAT; }
			set
			{
			}
		}

		public float Watts_Load 
		{ 
			get { return m_Watts_Load; }
			set
			{
			}
		}
		public double NormalizedX { get { return m_Loc == null ? 0.0f : m_Loc.Normalized; } }
		public double NormalizedY { get { return m_Loc == null ? 0.0f : m_Loc.NormalizedY; } }

		public String RiderName 
		{ 
			get 
			{
				return m_Bot != null ? m_Bot.DisplayName : m_Rider != null ? m_Rider.ToString() : "Unknown";
			}
		}

		public float DragFactor 
		{ 
			get 
			{
				return m_Rider != null ? m_Rider.DragFactor : 0;
			} 
		}

		public String SavedFileName = null;


		//------------------------------------------------------
		
		
		
		Int16 m_HoldCalibrationValue = -200;
		Int16 m_HardwareCalibrationValue = -200;	// Used when the HOLD is not in effect.

		public Int16 RawCalibrationValue
		{
			get { return m_Hold ? m_HoldCalibrationValue:m_HardwareCalibrationValue; }
		}
		public void SetCalibrationValue(bool iscalibrated, int cvalue, bool hardware)
		{
			if (!iscalibrated && cvalue == 0)
				cvalue = 1;
			Int16 v = (Int16)(iscalibrated ? cvalue : -cvalue);
			Int16 org = hardware ? m_HardwareCalibrationValue:m_HoldCalibrationValue;

			if (v != org)
			{
				if (hardware)
					m_HardwareCalibrationValue = v;
				else
					m_HoldCalibrationValue = v;
				Changed |= StatFlags.Calibration;
				PerfChanged |= StatFlags.Calibration;
				Statistics.AllChanged |= StatFlags.Calibration;
			}
		}
		public bool IsCalibrated
		{
			get { return RawCalibrationValue >= 0; }
		}
		public int CalibrationValue
		{
			get { Int16 v = RawCalibrationValue; return v < 0 ? -v : v; }
		}
		public String CalibrationString
		{
			get 
			{ 
				if (m_Hold || m_Trainer == null)
				{
					return String.Format("{0:F2}",CalibrationValue / 100.0);
				}
				else
				{
					if (m_Trainer.Type == RM1.DeviceType.COMPUTRAINER)
					{
						return String.Format("{0:F2}",CalibrationValue / 100.0);
					}
				}
				return "";
			}
		}





		// RM1.IStatsEx
		// =============================================================
		public double Time { get { return m_TimeAcc; } }
		
		public void Reset()
		{
			m_Mux.WaitOne();
			Stop();
			try
			{
				m_bForceGrade = m_bForceWatts = true;
				SavedFileName = null;
				StatFlags f = StatFlags.Mask;
				HasStarted = false;
				m_TimeAcc = 0.0;
				m_Speed_Avg.Reset();
				m_Speed_Max = 0;
				m_Cadence_Avg.Reset();
				m_Cadence_Max = 0;
				m_HeartRate_Avg.Reset();
				m_HeartRate_Max = 0;

				m_Watts_Avg.Reset();
				m_Watts_Max = 0;


				m_SSLeft_Acc = m_SSRight_Acc = m_SSLeft_Avg = m_SSRight_Avg = 0;
				m_Distance = 0.0;
				m_TrackDistance = 0.0;
				m_LapDistance = 0.0;
				m_Lap = 1;
				m_LapTimes.Clear();
				m_LastLapTotalTime = 0.0;
				m_LastLapTime = 0.0;
				m_BestLapTime = 0.0;
				m_BestLap = 0;

				m_Bars_TotalTime = 0.000000001;
				for (int i = 0; i < RM1.BarCount; i++)
				{
					m_Bars_Acc[i] = 0.0;
					m_AverageBars[i] = 0.0f;
					m_LastAverageBars[i] = 0.0f;
				}
				m_Bars_Shown = false;
				m_Grade = RM1_Settings.General.IdleGrade;
				m_Watts_Load = RM1_Settings.General.IdleWatts;
				m_Finished = false;	// When we reset this gets set back to false

				m_TotalCalories = 0;

				UpdateLoc(ref f);
				Changed |= f;
				PerfChanged |= f;
				AllChanged |= f;
				m_bForceGrade = m_bForceWatts = true;
			}
			catch
			{
			}

			m_Mux.ReleaseMutex();
		}
		public void Start()
		{
			m_Mux.WaitOne();
			m_bForceGrade = m_bForceWatts = true;
			Stop();
			Reset();
			HasStarted = true;
			m_State = State.Running;

			StatFlags f = StatFlags.Zero;
			UpdateLoc(ref f );

            Unit thisUnit = null;
            if (m_Bot != null)
                thisUnit = m_Bot.Unit;
            else if (m_Trainer != null)
                thisUnit = Unit.GetUnit(m_Trainer);

            // Enable saving reports, export and/or performances now that it started 
            // Only demo unit or person allowed to save a performance file
            if (App.AllowPerfSave && thisUnit != null && (thisUnit.IsActive && (thisUnit.IsDemoUnit || thisUnit.IsPerson)))
            {
                PerfContainer.started = true;
                // Create the first snapshot entry
                bool bFirstSnapShot = true;
                PerfContainer.SnapShot(this, thisUnit, bFirstSnapShot);
            }
			Changed |= StatFlags.Mask;
			PerfChanged |= StatFlags.Mask;
			AllChanged |= StatFlags.Mask;
			LastChanged = StatFlags.Mask;
			m_bInitCalories = true;

			if (m_Trainer != null)
			{
				m_Trainer.SetPaused(false, true);
				m_Trainer.Reset_trnr_Averages();
			}
            m_Mux.ReleaseMutex();
		}

		public void Stop()
		{
			m_Mux.WaitOne();
			UnPause();
			m_bForceGrade = m_bForceWatts = true;
			m_State = State.Stopped;
			m_Finished = true;

            Unit thisUnit = null;
            if (m_Bot != null)
                thisUnit = m_Bot.Unit;
            else if (m_Trainer != null)
                thisUnit = Unit.GetUnit(m_Trainer);

            // Only demo unit or person allowed to save a performance file
            if (thisUnit != null && (thisUnit.IsActive && (thisUnit.IsDemoUnit || thisUnit.IsPerson)) && SavedFileName == null)
            {
                // Save performance 
                SavedFileName = PerfContainer.SnapShotEnd(this, thisUnit);
            }
            // Always Disable saving until started again next time 
            PerfContainer.started = false;


			StatFlags f = StatFlags.Zero;
			UpdateLoc(ref f);
			Changed |= f;
			PerfChanged |= f;
			AllChanged |= f;

            m_Mux.ReleaseMutex();
		}

		public void Pause()
		{
			m_Mux.WaitOne();
			if (m_State == State.Running)
				m_State = State.Paused;
			if (m_Trainer != null && m_State == State.Paused)
				m_Trainer.SetPaused( true, true );
			m_Mux.ReleaseMutex();
		}



		private double m_StopTime;
		public double StopTime
		{
			get { return m_StopTime; }
			set 
			{
				if (value > 0)
					m_StopDistance = 0;
				m_StopTime = value;
				if (value > 0 && m_StopTime >= m_TimeAcc)
					Stop();
			}
		}
		private double m_StopDistance;
		public double StopDistance
		{
			get { return m_StopDistance; }
			set 
			{
				if (value > 0)
					m_StopTime = 0;
				m_StopDistance = value;
				if (value > 0 && m_StopDistance <= m_Distance)
					Stop();
			}
		}

		public void UnPause()
		{
			m_Mux.WaitOne();
			if (m_State == State.Paused)
				m_State = State.Running;
			if (m_Trainer != null)
				m_Trainer.SetPaused(false, true);
			m_Mux.ReleaseMutex();
		}
		public float Speed_Avg { get { return (float)(m_Speed_Avg); } }
		public float Speed_Max { get { return (float)(m_Speed_Max); } }

		public float Watts_Avg { get { return m_Watts_Avg; } }
		public float Watts_Max { get { return m_Watts_Max; } }

		float m_WkgConv;
		public float Watts_Wkg
		{
			get
			{
				return (float)(m_Watts * m_WkgConv);
			}
		}
		public void SetWkgConv()
		{
			float w = (float)(160.0 * ConvertConst.LBStoKGS);
			if (m_Bot != null && m_Bot.ControlUnit != null && m_Bot.ControlUnit.Rider != null)
				w = m_Bot.ControlUnit.Rider.WeightBikeKGS + m_Bot.ControlUnit.Rider.WeightRiderKGS;
			else if (m_Rider != null)
				w = m_Rider.WeightBikeKGS + m_Rider.WeightRiderKGS;

			if (w <= 1) w = 1;
			m_WkgConv = 1 / w;
		}

		public float HeartRate_Avg { get { return m_HeartRate_Avg; } }
		public float HeartRate_Max { get { return m_HeartRate_Max; } }

		public float Cadence_Avg { get { return m_Cadence_Avg; } }
		public float Cadence_Max { get { return m_Cadence_Max; } }

		public float SSLeftATA { get { return m_SSLeftATA; } }
		public float SSRightATA { get { return m_SSRightATA; } }

		public float SSLeft_Avg { get { return m_SSLeft_Avg; } }
		public float SSRight_Avg { get { return m_SSRight_Avg; } }
		public float SS_Avg { get { return (m_SSLeft_Avg + m_SSLeft_Avg) * 0.5f; } }


		public double Distance { get { return m_Distance; } }
		public double TrackDistanceSub(double subsplit)
		{
			return m_TrackDistance + m_Speed * subsplit;
		}
		private double m_TrackDistance;
		public double TrackDistance {get { return m_TrackDistance; } }

		public double DebugDistance
		{
			set
			{
				double total = m_Course.TotalX * Unit.Laps;
				m_Distance = value > total ? total:value;
				m_TrackDistance = total;
				m_LapDistance = value;
				m_Lap = 1;
				if (Course != null)
				{
					m_Lap = 1;
					while(m_LapDistance >= Course.TotalX)
					{
						m_Lap++;
						m_LapDistance -= Course.TotalX;
					}
				}
				if (m_Finished != value >= total)
				{
					m_Finished = value >= total;
					Changed |= StatFlags.Finished;
					PerfChanged |= StatFlags.Finished;
					AllChanged |= StatFlags.Finished;
				}
				if (m_Finished)
					m_LapDistance = Course.TotalX;
				Changed |= StatFlags.Distance | StatFlags.Lap;
				PerfChanged |= StatFlags.Distance | StatFlags.Lap;
				AllChanged |= StatFlags.Distance | StatFlags.Lap;
			}
		}

		public List<double> LapTimes
		{
			get { return m_LapTimes; }
		}

		protected double m_LapDistance;
		public double LapDistance
		{
			get { return m_LapDistance; }
		}
		protected int m_Lap;
		public int Lap
		{
			get { return m_Lap; }
		}


		protected int m_NumberOfLaps = 1;
		public int NumberOfLaps
		{
			get { return m_NumberOfLaps; }
			set
			{
				int v = value < 1 ? 1 : value;
				if (m_NumberOfLaps != v)
				{
					m_NumberOfLaps = v;
					OnPropertyChanged("NumberOfLaps");
				}
			}
		}





		public String GearingString   {
			// xxx tlm20150414
			get { return FrontGear + "/" + RearGear; }
			/*
			get {
				return "99/97";
			}
			*/
		}


		public double DistanceDisplay { get { return m_Distance * ConvertConst.MetersToMilesOrKilometers; } }
		public double SpeedDisplay { get { return m_Speed * ConvertConst.MetersPerSecondToMPHOrKPH; } }
		public double Speed_Max_Display { get { return m_Speed_Max * ConvertConst.MetersPerSecondToMPHOrKPH; } }
		public double Speed_Avg_Display { get { return m_Speed_Avg * ConvertConst.MetersPerSecondToMPHOrKPH; } }
		public double WindDisplay { get { return m_Wind * ConvertConst.MetersPerSecondToMPHOrKPH; } }

		const double UseAsFeet = ConvertConst.FeetToMeters * 1056;
		public String DistanceDisplayString
		{
			get
			{
				return RM1_Settings.General.Metric ?
					m_Distance < 1000.0 ? String.Format("{0:F0}m", m_Distance) : String.Format("{0:F2}", m_Distance * ConvertConst.MetersToKilometers) :
					m_Distance < UseAsFeet ? String.Format("{0:F0}'", m_Distance * ConvertConst.MetersToFeet) :
										  String.Format("{0:F2}", m_Distance * ConvertConst.MetersToMiles);
			}
		}

		// INotifyPropertyChanged
		// =============================================================
		public event PropertyChangedEventHandler PropertyChanged;
		[Conditional("STATS_PROPERTYCHANGED")] 
		public void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
		// =========================================================================
		protected Rider m_Rider;
		public Rider Rider
		{
			get { return m_Rider; }
			set
			{
				if (value == Rider)
					return;
				m_Rider = value;
				if (m_Trainer != null)
					m_Trainer.Rider = value;
				SetWkgConv();
			}
		}
		// =========================================================================
		protected int m_CourseScreenX;
		protected Course m_Course;
		protected Course.Location m_Loc;
		protected bool m_bTimeBased;
		protected bool m_bLoadBased;

		public Course Course
		{
			get { return m_Course; }
			set 
			{
				m_Mux.WaitOne();
				if (m_Course != value)
				{
					Changed |= StatFlags.Course;
					PerfChanged |= StatFlags.Course;
					AllChanged |= StatFlags.Course;
				}

				Reset();
				m_Course = value;
				if (value != null)
				{
					m_Loc = new Course.Location(Course, 0.0f);
					m_bTimeBased = m_Course.XUnits == CourseXUnits.Time;
					m_bLoadBased = m_Course.YUnits != CourseYUnits.Grade;
				}
				else
				{
					m_Loc = null;
				}
				m_bForceGrade = m_bForceWatts = true;
				m_Mux.ReleaseMutex();
			}
		}

		//
		//
		// =========================================================================
		RM1.Trainer m_Trainer;
		Bot m_Bot;

		public void Attach(RM1.Trainer trainer)
		{
			if (m_Trainer == trainer)
				return;
			if (m_Trainer != null)
			{
				m_Trainer.SetPaused(false, true);
				m_Trainer.Reset_trnr_Averages();
				m_Trainer.SetGrade(0, true);
			}
			m_Trainer = trainer;
			
			if (m_Trainer != null)
			{
				SetCalibrationValue(m_Trainer.IsCalibrated, m_Trainer.CalibrationValue, true);
				m_Trainer.SetPaused(false, true);
				m_Trainer.Reset_trnr_Averages();
				m_Trainer.SetGrade(0, true);
			}
			else
				SetCalibrationValue(false, 200, true );
		}
		public void Attach(Bot bot)
		{
			if (m_Bot == bot)
				return;
			m_Bot = bot;
			SetWkgConv();
		}



		double m_OverrideDistance;
		bool m_bOverrideDistance;
		public double OverrideDistance
		{
			set
			{
				m_OverrideDistance = value;
				m_bOverrideDistance = true;
			}
		}


		bool m_HeartRateFlash;
		public bool HeartRateFlash 
		{
			get { return m_HeartRateFlash; }
		}
		/// <summary>
		/// Based off of time.  This will set the HeartRateFlash on/off and also return HeartRateAlarm or 0 
		/// </summary>
		public void SetHRAlarm() // Updates the alarm based on time.   
		{
			int hr = (int)m_HeartRate;
			bool state = false;
			if (hr > 0 && m_Rider != null)
			{
				int min = m_Rider.HrAlarmMin;
				int max = m_Rider.HrAlarmMax;
				if (min != 0 && hr < min)
					state = (Unit.AdvTime % 2) > 1;
				else if (max != 0 && hr > max)
					state = (Unit.AdvTime % 1) > 0.5;
			}
			if (m_HeartRateFlash != state)
			{
				m_HeartRateFlash = state;
				Changed |= StatFlags.HeartRateAlarm;
				PerfChanged |= StatFlags.HeartRateAlarm;
				AllChanged |= StatFlags.HeartRateAlarm;
			}
		}

		protected bool m_Hold;
		public bool Hold
		{
			get { return m_Hold; }
			set
			{
				m_Mux.WaitOne();
				if (!m_Hold && value)
				{
					m_HoldCalibrationValue = m_HardwareCalibrationValue;
				}
				m_Hold = value;
				m_Mux.ReleaseMutex();
			}
		}
		public void LoadPerfFrame(ref PerfFrame.PerfData cur, Course course)
		{
			StatFlags c = StatFlags.Zero;
			m_Mux.WaitOne();
			try
			{
				if (m_TimeAcc != cur.TimeAcc) { m_TimeAcc = cur.TimeAcc; c |= StatFlags.Time; }
				//if (m_LapTime != cur.LapTime) { m_LapTime = cur.LapTime; c |= StatFlags.LapTime; } // if ((validflags & StatFlags.LapTime) != StatFlags.Zero) { cur.LapTime = prev.LapTime; }
				//if (m_Lap != cur.Lap) { m_Lap = cur.Lap; c |= StatFlags.Lap; } //  if ((validflags & StatFlags.Lap) != StatFlags.Zero) { cur.Lap = prev.Lap; }
				if (m_Distance != cur.Distance) 
				{ 
					m_Distance = cur.Distance;
					c |= StatFlags.Distance; 
					if (course.XUnits == CourseXUnits.Distance)
					{
						// Calculate the lap and lap distance based on the distance provided.
						int lap = (int)Math.Floor(m_Distance / course.TotalX) + 1;
						double lapd;
						if (lap > course.Laps)
						{
							lapd = course.TotalX;
							lap = course.Laps;
						}
						else
							lapd = m_Distance - course.TotalX * (lap-1);
						if (m_Lap != lap) { m_Lap = lap; c |= StatFlags.Lap; }
						if (m_LapDistance != lapd)
						{
							m_LapDistance = lapd;
							c |= StatFlags.LapDistance;
						}
					}
				}
				if (m_Lead != cur.Lead) { m_Lead = cur.Lead; c |= StatFlags.Lead; } //  if ((validflags & StatFlags.Lead) != StatFlags.Zero) { cur.Lead = prev.Lead; }
				if (m_Grade != cur.Grade) { m_Grade = cur.Grade; c |= StatFlags.Grade; } //  if ((validflags & StatFlags.Grade) != StatFlags.Zero) { cur.Grade = interpolate(prev.Grade, next.Grade); }
				if (m_Wind != cur.Wind) { m_Wind = cur.Wind; c |= StatFlags.Wind; } //  if ((validflags & StatFlags.Wind) != StatFlags.Zero) { cur.Wind = interpolate(prev.Wind, next.Wind); }

				if (m_Speed != cur.Speed) {
					m_Speed = cur.Speed;
					c |= StatFlags.Speed;
				} //  if ((validflags & StatFlags.Speed) != StatFlags.Zero) { cur.Speed = interpolate(prev.Speed, next.Speed); }

				if (m_Speed_Avg != cur.Speed_Avg) { m_Speed_Avg.Force(cur.Speed_Avg); c |= StatFlags.Speed_Avg; } //  if ((validflags & StatFlags.Speed_Avg) != StatFlags.Zero) { cur.Speed_Avg = interpolate(prev.Speed_Avg, next.Speed_Avg); }
				if (m_Speed_Max != cur.Speed_Max) { m_Speed_Max = cur.Speed_Max; c |= StatFlags.Speed_Max; } //  if ((validflags & StatFlags.Speed_Max) != StatFlags.Zero) { cur.Speed_Max = interpolate(prev.Speed_Max, next.Speed_Max); }
				if (m_Watts != cur.Watts) { m_Watts = cur.Watts; c |= StatFlags.Watts; } //  if ((validflags & StatFlags.Watts) != StatFlags.Zero) { cur.Watts = interpolate(prev.Watts, next.Watts); }
				if (m_Watts_Avg != cur.Watts_Avg) { m_Watts_Avg.Force(cur.Watts_Avg); c |= StatFlags.Watts_Avg; } //  if ((validflags & StatFlags.Watts_Avg) != StatFlags.Zero) { cur.Watts_Avg = interpolate(prev.Watts_Avg, next.Watts_Avg); }
				if (m_Watts_Max != cur.Watts_Max) { m_Watts_Max = cur.Watts_Max; c |= StatFlags.Watts_Max; } //  if ((validflags & StatFlags.Watts_Max) != StatFlags.Zero) { cur.Watts_Max = interpolate(prev.Watts_Max, next.Watts_Max); }
				//if (m_Watts_Wkg != cur.Watts_Wkg) { m_Watts_Wk = cur.Watts_Wkg; c |= StatFlags.Watts_Wkg; } //  if ((validflags & StatFlags.Watts_Wkg) != StatFlags.Zero) { cur.Watts_Wkg = interpolate(prev.Watts_Wkg, next.Watts_Wkg); }
				if (m_Watts_Load != cur.Watts_Load) { m_Watts_Load = cur.Watts_Load; c |= StatFlags.Watts_Load; } //  if ((validflags & StatFlags.Watts_Load) != StatFlags.Zero) { cur.Watts_Load = interpolate(prev.Watts_Load, next.Watts_Load); }

				//  if ((validflags & StatFlags.HeartRate) != StatFlags.Zero) { cur.HeartRate = interpolate(prev.HeartRate, next.HeartRate); }
				if (m_HeartRate != cur.HeartRate) {
					m_HeartRate = cur.HeartRate;
					c |= StatFlags.HeartRate;
				}
				if (m_HeartRate_Avg != cur.HeartRate_Avg) { m_HeartRate_Avg.Force( cur.HeartRate_Avg ); c |= StatFlags.HeartRate_Avg; } //  if ((validflags & StatFlags.HeartRate_Avg) != StatFlags.Zero) { cur.HeartRate_Avg = interpolate(prev.HeartRate_Avg, next.HeartRate_Avg); }
				if (m_HeartRate_Max != cur.HeartRate_Max) { m_HeartRate_Max = cur.HeartRate_Max; c |= StatFlags.HeartRate_Max; } //  if ((validflags & StatFlags.HeartRate_Max) != StatFlags.Zero) { cur.HeartRate_Max = interpolate(prev.HeartRate_Max, next.HeartRate_Max); }
				if (m_Cadence != cur.Cadence) { m_Cadence = cur.Cadence; c |= StatFlags.Cadence; } //  if ((validflags & StatFlags.Cadence) != StatFlags.Zero) { cur.Cadence = interpolate(prev.Cadence, next.Cadence); }
				if (m_Cadence_Avg != cur.Cadence_Avg) { m_Cadence_Avg.Force( cur.Cadence_Avg ); c |= StatFlags.Cadence_Avg; } //  if ((validflags & StatFlags.Cadence_Avg) != StatFlags.Zero) { cur.Cadence_Avg = interpolate(prev.Cadence_Avg, next.Cadence_Avg); }
				if (m_Cadence_Max != cur.Cadence_Max) { m_Cadence_Max = cur.Cadence_Max; c |= StatFlags.Cadence_Max; } //  if ((validflags & StatFlags.Cadence_Max) != StatFlags.Zero) { cur.Cadence_Max = interpolate(prev.Cadence_Max, next.Cadence_Max); }
				if (m_TotalCalories != cur.Calories) { m_TotalCalories = cur.Calories; c |= StatFlags.Calories; } //  if ((validflags & StatFlags.Calories) != StatFlags.Zero) { cur.Calories = interpolate(prev.Calories, next.Calories); }
				if (m_PulsePower != cur.PulsePower) { m_PulsePower = cur.PulsePower; c |= StatFlags.PulsePower; } //  if ((validflags & StatFlags.PulsePower) != StatFlags.Zero) { cur.PulsePower = interpolate(prev.PulsePower, next.PulsePower); }
				//if (m_DragFactor != cur.DragFactor) { m_DragFactor = cur.DragFactor; c |= StatFlags.DragFactor; } //  if ((validflags & StatFlags.DragFactor) != StatFlags.Zero) { cur.DragFactor = interpolate(prev.DragFactor, next.DragFactor); }
				if (m_SS != cur.SS) { m_SS = cur.SS; c |= StatFlags.SS; } //  if ((validflags & StatFlags.SS) != StatFlags.Zero) { cur.SS = interpolate(prev.SS, next.SS); }
				if (m_SSLeft != cur.SSLeft) { m_SSLeft = cur.SSLeft; c |= StatFlags.SSLeft; } //  if ((validflags & StatFlags.SSLeft) != StatFlags.Zero) { cur.SSLeft = interpolate(prev.SSLeft, next.SSLeft); }
				if (m_SSRight != cur.SSRight) { m_SSRight = cur.SSRight; c |= StatFlags.SSRight; } //  if ((validflags & StatFlags.SSRight) != StatFlags.Zero) { cur.SSRight = interpolate(prev.SSRight, next.SSRight); }
				if (m_SSLeftSplit != cur.SSLeftSplit) { m_SSLeftSplit = cur.SSLeftSplit; c |= StatFlags.SSLeftSplit; } //  if ((validflags & StatFlags.SSLeftSplit) != StatFlags.Zero) { cur.SSLeftSplit = interpolate(prev.SSLeftSplit, next.SSLeftSplit); }
				if (m_SSRightSplit != cur.SSRightSplit) { m_SSRightSplit = cur.SSRightSplit; c |= StatFlags.SSRightSplit; } //  if ((validflags & StatFlags.SSRightSplit) != StatFlags.Zero) { cur.SSRightSplit = interpolate(prev.SSRightSplit, next.SSRightSplit); }
				if (m_SSLeftATA != cur.SSLeftATA) { m_SSLeftATA = cur.SSLeftATA; c |= StatFlags.SSLeftATA; } //  if ((validflags & StatFlags.SSLeftATA) != StatFlags.Zero) { cur.SSLeftATA = interpolate(prev.SSLeftATA, next.SSLeftATA); }
				if (m_SSRightATA != cur.SSRightATA) { m_SSRightATA = cur.SSRightATA; c |= StatFlags.SSRightATA; } //  if ((validflags & StatFlags.SSRightATA) != StatFlags.Zero) { cur.SSRightATA = interpolate(prev.SSRightATA, next.SSRightATA); }
				if (m_SSLeft_Avg != cur.SSLeft_Avg) { m_SSLeft_Avg = cur.SSLeft_Avg; c |= StatFlags.SSLeft_Avg; } //  if ((validflags & StatFlags.SSLeft_Avg) != StatFlags.Zero) { cur.SSLeft_Avg = interpolate(prev.SSLeft_Avg, next.SSLeft_Avg); }
				if (m_SSRight_Avg != cur.SSRight_Avg) { m_SSRight_Avg = cur.SSRight_Avg; c |= StatFlags.SSRight_Avg; } //  if ((validflags & StatFlags.SSRight_Avg) != StatFlags.Zero) { cur.SSRight_Avg = interpolate(prev.SSRight_Avg, next.SSRight_Avg); }
				if (m_PercentAT != cur.PercentAT) { m_PercentAT = cur.PercentAT; c |= StatFlags.PercentAT; } //  if ((validflags & StatFlags.PercentAT) != StatFlags.Zero) { cur.PercentAT = interpolate(prev.PercentAT, next.PercentAT); }
				if (m_FrontGear != cur.FrontGear) { m_FrontGear = cur.FrontGear; c |= StatFlags.FrontGear; } //  if ((validflags & StatFlags.FrontGear) != StatFlags.Zero) { cur.FrontGear = (int)interpolate((float)prev.FrontGear, (float)next.FrontGear); }
				if (m_RearGear != cur.RearGear) { m_RearGear = cur.RearGear; c |= StatFlags.RearGear; } //  if ((validflags & StatFlags.RearGear) != StatFlags.Zero) { cur.RearGear = (int)interpolate((float)prev.RearGear, (float)next.RearGear); }
				if (m_GearInches != cur.GearInches) { m_GearInches = cur.GearInches; c |= StatFlags.GearInches; } //  if ((validflags & StatFlags.GearInches) != StatFlags.Zero) { cur.GearInches = prev.GearInches; }
				//if ((validflags & StatFlags.RawSpinScan) != StatFlags.Zero) { cur.RawSpinScan = prev.RawSpinScan; }
				//if ((validflags & StatFlags.CadenceTiming) != StatFlags.Zero) { cur.CadenceTiming = interpolate(prev.CadenceTiming,next.CadenceTiming); }
				if (m_TSS != cur.TSS) { m_TSS = cur.TSS; c |= StatFlags.TSS; } //  if ((validflags & StatFlags.TSS) != StatFlags.Zero) { cur.TSS = interpolate(prev.TSS, next.TSS); }
				if (m_IF != cur.IF) { m_IF = cur.IF; c |= StatFlags.IF; } //  if ((validflags & StatFlags.IF) != StatFlags.Zero) { cur.IF = interpolate(prev.IF, next.IF); }
				if (m_NP != cur.NP) { m_NP = cur.NP; c |= StatFlags.NP; } //  if ((validflags & StatFlags.NP) != StatFlags.Zero) { cur.NP = interpolate(prev.NP, next.NP); }

				if (m_HoldCalibrationValue != cur.RawCalibrationValue) { m_HoldCalibrationValue = (Int16)cur.RawCalibrationValue; c |= StatFlags.Calibration; }
				//if (m_Bars_Shown != cur.Bars_Shown) { m_Bars_Shown = cur.Bars_Shown; c |= StatFlags.Bars_Shown; } //  if ((validflags & StatFlags.Bars_Shown) != StatFlags.Zero) { cur.Bars_Shown = prev.Bars_Shown; }
				int i;
				for (i = 0; i < m_Bars.Length; i++)
				{
					if (m_Bars[i] != cur.Bars[i])
					{
						for (; i < m_Bars.Length; i++)
							m_Bars[i] = cur.Bars[i];
						c |= StatFlags.Bars;
						break;
					}
				}
				for (i = 0; i < m_AverageBars.Length; i++)
				{
					if (m_AverageBars[i] != cur.AverageBars[i])
					{
						for (; i < m_Bars.Length; i++)
							m_AverageBars[i] = cur.AverageBars[i];
						c |= StatFlags.Bars_Avg;
						break;
					}
				}

                // Will - uncomment these and fix as needed
                // Added v1.02
                //if (m_Calibration != cur.Calibration) { m_Calibration = cur.Calibration; c |= StatFlags.Calibration; }
                //if (m_Drafting != cur.Drafting) { m_Drafting = cur.Drafting; c |= StatFlags.Drafting; }
                //if (m_Disconnected != cur.Disconnected) { m_Disconnected = cur.Disconnected; c |= StatFlags.Disconnected; }

				// HeartRate alarm flag

				Changed |= c;
				PerfChanged |= c;
				AllChanged |= c;
				LastChanged = c;

			}
			catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
			m_Mux.ReleaseMutex();
		}


		//============================================================================
		/// <summary>
		/// SHOULD ONLY BE CALLED FROM UNIT.
		/// </summary>
		/// <param name="istats"></param>
		/// <param name="splittime"></param>
		public void UpdateStats( RM1.IStats istats, double splittime )
		{
			StatFlags f = StatFlags.Zero;
			m_Mux.WaitOne();
			if (Hold)
			{
				m_Mux.ReleaseMutex();
				return;
			}
	
			try
			{
				m_Ticks = istats.Ticks;
				m_SplitTime = istats.SplitTime;
				m_LastSpeed = m_Speed;

				if (m_Speed != istats.Speed)   {					// istats.Speed is in meters per second!!!
					m_Speed = istats.Speed; 
					f |= StatFlags.Speed;  
					OnPropertyChanged("Speed"); 
				}

				if (m_Cadence != istats.Cadence)
				{
					m_Cadence = istats.Cadence;
					if (m_Cadence == 0)
						m_CadenceOverrideTime = Unit.LastTime + c_CadenceOverrideTime;
					f |= StatFlags.Cadence;
					OnPropertyChanged("Cadence");
				}
				if (m_HeartRate != istats.HeartRate) 
				{ 
					m_HeartRate = istats.HeartRate;
					f |= StatFlags.HeartRate;
					OnPropertyChanged("HeartRate");
				}
				if (m_Watts != istats.Watts) 
				{ 
					m_Watts = istats.Watts;
					f |= StatFlags.Watts | StatFlags.Watts_Wkg;
					OnPropertyChanged("Watts");
				}
				if (m_SS != istats.SS) 
				{ 
					m_SS = istats.SS;
					f |= StatFlags.SS;
					OnPropertyChanged("SS"); 
				}
				if (m_SSLeft != istats.SSLeft) 
				{ 
					m_SSLeft = istats.SSLeft;
					f |= StatFlags.SSLeft;
					OnPropertyChanged("SSLeft"); 
				}
				if (m_SSRight != istats.SSRight) 
				{ 
					m_SSRight = istats.SSRight;
					f |= StatFlags.SSRight;
					OnPropertyChanged("SSRight"); 
				}
				if (m_SSLeftSplit != istats.SSLeftSplit) 
				{ 
					m_SSLeftSplit = istats.SSLeftSplit;
					f |= StatFlags.SSLeftSplit;
					OnPropertyChanged("SSLeftSplit"); 
				}
				if (m_SSRightSplit != istats.SSRightSplit) 
				{ 
					m_SSRightSplit = istats.SSRightSplit; 
					f |= StatFlags.SSRightSplit;
					OnPropertyChanged("SSRightSplit"); 
				}

				// xxx

				bool chgear = false;
				if (m_FrontGear != istats.FrontGear)  { 
					chgear = true;  
					m_FrontGear = istats.FrontGear;
#if DEBUG
					if (m_FrontGear == 56) {
						//int bp = 88;
						//bp = 0;
					}
#endif
					f |= StatFlags.FrontGear;
					OnPropertyChanged("FrontGear"); 
				}

				if (m_RearGear != istats.RearGear)  { 
					chgear = true;  
					m_RearGear = istats.RearGear;
					f |= StatFlags.RearGear;
					OnPropertyChanged("RearGear"); 
				}





				if (m_PulsePower != istats.PulsePower)  { 
					m_PulsePower = istats.PulsePower;
					f |= StatFlags.PulsePower;
					OnPropertyChanged("PulsePower"); 
				}
				if (m_NP != istats.NP) 
				{ 
					m_NP = istats.NP;
					f |= StatFlags.NP;
					OnPropertyChanged("NP"); 
				}
				if (m_IF != istats.IF) 
				{ 
					m_IF = istats.IF; 
					f |= StatFlags.IF;
					OnPropertyChanged("IF"); 
				}
				if (m_TSS != istats.TSS) { m_TSS = istats.TSS; f |= StatFlags.TSS; OnPropertyChanged("TSS"); }

				if (chgear)  {
					// xxx tlm20150414
					OnPropertyChanged("GearingString");
					int tt = istats.GearInches;
					f |= StatFlags.GearInches;
					if (tt != m_GearInches) {
						m_GearInches = tt;
						OnPropertyChanged("GearInches");
					}
				}

				// Drafting
				if (m_Drafting != m_tempDrafting)
				{
					m_Drafting = m_tempDrafting;
					if (m_Trainer != null)
						m_Trainer.Drafting = m_Drafting;
					f |= StatFlags.Drafting;
					OnPropertyChanged("Drafting");
				}


				// Bars must be check individually.
				int i;
				bool dobaravg = false;
				float val;
				float[] last;
				bool changed;

				last = m_Bars;
				m_Bars = m_LastBars;
				m_LastBars = last;


				Filter.SplitTime = m_SplitTime;

				for (i = 0,changed = false; i < RM1.BarCount; i++)  {
					val = istats.Bars[i];

					if (!dobaravg && val > 0.02)  {
						dobaravg = true;
					}

					//val = m_BarFilters[i].Set(val);

					if (!changed && m_Bars[i] != val)  {
						m_Bars[i] = val;
						changed = true;
					}
					else  {
						m_Bars[i] = val;
					}
				}


				if (dobaravg != m_Bars_Shown)
				{
					f |= StatFlags.Bars_Shown;
					m_Bars_Shown = dobaravg;
					OnPropertyChanged("Bars_Shown");
				}

				if (changed)
				{
					f |= StatFlags.Bars;
					OnPropertyChanged("Bars");

					double top, bottom;
					float ata;
					top = 0.0f;
					bottom = 0.0f;
					for (i = 0; i < RM1.HalfBarCount; i++)
					{
						top += m_Bars[i] * RM1.BarRadians[i];
						bottom += m_Bars[i];
					}

					ata = bottom != 0.0f ? (float)Math.Round(top * 180.0 / (bottom * Math.PI), 2) : 90.0f;

					// tlm20130508 swapped left/right code block here

					if (m_SSLeftATA != ata)
					{
						m_SSLeftATA = ata;
						f |= StatFlags.SSLeftATA;
						OnPropertyChanged("SSLeftATA");
					}

					top = 0.0f;
					bottom = 0.0f;
					for (; i < RM1.BarCount; i++)
					{
						top += m_Bars[i] * RM1.BarRadians[i];
						bottom += m_Bars[i];
					}
					ata = bottom != 0.0f ? (float)Math.Round(top * 180.0 / (bottom * Math.PI), 2) : 90.0f;

					// tlm20130508 swapped left/right code block here

					if (m_SSRightATA != ata)
					{
						m_SSRightATA = ata;
						f |= StatFlags.SSRightATA;
						OnPropertyChanged("SSRightATA");
					}
		
				}

				float d;
				double traveled = m_LastSpeed * m_SplitTime;
				if (m_bOverrideDistance)
				{
					m_bOverrideDistance = false;
					traveled = m_OverrideDistance - Distance;
				}
				if (m_State == State.Running)
				{
					// Calories is special... 
					if (m_bInitCalories)
					{
						m_bInitCalories = false;
						m_LastCalories = istats.Calories;
					}
					else
					{
						float c = istats.Calories;
						d = c - m_LastCalories;
						m_LastCalories = c;
						if (d < 0)
							m_bInitCalories = true;
						else if (d > 0)
						{
							m_TotalCalories += d;
							f |= StatFlags.Calories;
						}
						OnPropertyChanged("Calories");
					}

					// Collect statistics.
					m_TimeAcc += m_SplitTime;
					if (ms_TimerIntervalCount != RM1.Trainer.IntervalCount)
					{
						ms_TimerIntervalCount = RM1.Trainer.IntervalCount;
						ms_Timer = m_TimeAcc;
					}
					if (m_StopTime > 0 && m_TimeAcc >= m_StopTime)
					{
						m_SplitTime = (float)(m_TimeAcc - m_StopTime);
						m_TimeAcc = m_StopTime;
						Stop();
						f |= StatFlags.Finished;
					}

					// Distance
					if (traveled > 0.0)
					{
						m_Distance += traveled;
						m_TrackDistance += traveled;
						if (m_StopDistance > 0 && m_Distance >= m_StopDistance)
						{
							// Figure out a new time and distance... and then stop the race.
							m_Distance -= traveled;
							m_TimeAcc -= m_SplitTime;
							m_SplitTime = (float)((m_StopDistance - m_Distance) / (m_Speed * RM1.iSecondsPerHour));
							m_Distance = m_StopDistance;
							traveled = m_Speed * m_SplitTime * RM1.iSecondsPerHour;
							if (m_LapDistance + traveled < m_Course.TotalX)
								traveled = m_Course.TotalX - m_LapDistance;
							if (traveled <= 0.0)
								traveled = 0.00001;
							Stop();
							f |= StatFlags.Finished;
						}
					}
					f |= StatFlags.Time;
					OnPropertyChanged("TimerString");
					if (traveled > 0.0)
					{
						f |= StatFlags.Distance | StatFlags.TrackDistance;
						OnPropertyChanged("Distance");
					}

					if (m_bTimeBased)
						traveled = m_SplitTime;

					if (traveled > 0.0)
					{
						OnPropertyChanged("LapDistance");
						m_LapDistance += traveled;
						if (m_Course != null && m_Course.Manual)
						{
							// Advance the course to the new lapdistance + 1
							m_Course.ManualAdvance(m_LapDistance + 0.1);
						}

						f |= StatFlags.LapDistance;
						if (m_Course != null && m_LapDistance >= m_Course.TotalX)
						{
							do
							{
								// Figure out the exact lap time.
								double laptime_totaltime = m_bTimeBased ? m_Course.TotalX * m_Lap:(m_TimeAcc - (m_SplitTime * (m_LapDistance - m_Course.TotalX) / traveled));
								double lp = laptime_totaltime - m_LastLapTotalTime;
								m_LapTimes.Add(lp);
								m_LastLapTotalTime = laptime_totaltime;
								m_LastLapTime = lp;
								m_LapDistance -= m_Course.TotalX;
								if (m_BestLap == 0 || lp < m_BestLapTime)
								{
									m_BestLap = m_Lap;
									m_BestLapTime = lp;
								}
								m_Lap++;
							} while (m_LapDistance >= m_Course.TotalX);
							f |= StatFlags.Lap;
							OnPropertyChanged("Lap");
							if (m_Finished)
								m_LapDistance = m_Course.TotalX; 
						}
						UpdateLoc(ref f);
					}
					// Speed
					if (m_Speed_Avg.Add(m_Speed, m_SplitTime))
					{
						f |= StatFlags.Speed_Avg;
						OnPropertyChanged("Speed_Avg");
					}
					if (m_Speed > m_Speed_Max)
					{
						f |= StatFlags.Speed_Max;
						m_Speed_Max = m_Speed; OnPropertyChanged("Speed_Max");
					}


					// Cadence
					if (m_Cadence_Avg.Add(m_Cadence, m_SplitTime))
					{
						f |= StatFlags.Cadence_Avg;
						OnPropertyChanged("Cadence_Avg");
					}
					if (m_Cadence > m_Cadence_Max)
					{
						m_Cadence_Max = m_Cadence;
						f |= StatFlags.Cadence_Max;
						OnPropertyChanged("Cadence_Max");
					}

					// Heartrate
					if (m_HeartRate_Avg.Add(m_HeartRate, m_SplitTime))
					{
						f |= StatFlags.HeartRate_Avg;
						OnPropertyChanged("HeartRate_Avg");
					}
					if (m_HeartRate > m_HeartRate_Max)
					{
						m_HeartRate_Max = m_HeartRate;
						f |= StatFlags.HeartRate_Max;
						OnPropertyChanged("HeartRate_Max");
					}

					// Watts
					if (m_Watts_Avg.Add(m_Watts, m_SplitTime))
					{
						f |= StatFlags.Watts_Avg;
						OnPropertyChanged("Watts_Avg");
					}
					if (m_Watts > m_Watts_Max)
					{
						m_Watts_Max = m_Watts;
						f |= StatFlags.Watts_Max;
						OnPropertyChanged("Watts_Max");
					}

					// SS Averages
					m_SSLeft_Acc += m_SSLeft * m_SplitTime;
					m_SSRight_Acc += m_SSRight * m_SplitTime;
					d = (float)Math.Round(m_SSLeft_Acc / m_TimeAcc, 1);
					if (d != m_SSLeft_Avg)
					{
						m_SSLeft_Avg = d;
						f |= StatFlags.SSLeft_Avg;
						OnPropertyChanged("SSLeft_Avg");
					}
					d = (float)Math.Round(m_SSRight_Acc / m_TimeAcc, 1);
					if (d != m_SSRight_Avg)
					{
						m_SSRight_Avg = d;
						f |= StatFlags.SSRight_Avg;
						OnPropertyChanged("SSRight_Avg");
					}
					if ((f & StatFlags.SS_Avg) != StatFlags.Zero)
						OnPropertyChanged("SS_Avg");

					// SS AverageBars
					if (dobaravg)
					{
						dobaravg = false;
						last = m_AverageBars;
						m_AverageBars = m_LastAverageBars;
						m_LastAverageBars = last;

						m_Bars_TotalTime += m_SplitTime;
						if (m_Bars_TotalTime > 0.0)
						{
							for (i = 0, changed = false; i < RM1.BarCount; i++)
							{
								m_Bars_Acc[i] += istats.Bars[i] * m_SplitTime;
								if (!changed)
								{
									d = (float)Math.Round(m_Bars_Acc[i] / m_Bars_TotalTime, 2);
									if (d != m_AverageBars[i])
									{
										changed = true;
										m_AverageBars[i] = d;
									}
								}
								else
									m_AverageBars[i] = (float)Math.Round(m_Bars_Acc[i] / m_Bars_TotalTime, 2);
							}
							if (changed)
							{
								f |= StatFlags.Bars_Avg;
								OnPropertyChanged("AverageBars");
							}
						}
					}
				}
				else if (m_Finished && traveled > 0.0)
				{
					m_TrackDistance += traveled;
					f |= StatFlags.TrackDistance;
				}
				f |= StatFlags.Time;
				OnPropertyChanged("Time");	// When all is done we do the time... this make sure everything else is updated properly.
			}
			catch { }
			Changed |= f;
			PerfChanged |= Changed; // All changed flags for this loop get added to the PerfChanged
			AllChanged |= f;
			LastChanged = f;

            if (m_State == State.Running || (((f & StatFlags.Finished) != StatFlags.Zero) && m_State == State.Stopped))
            {
                // If snapshot was started, then take a snapshot here
				// Note - we also need to take one last snapshot if we finished the race
                Unit thisUnit = null;
                if (m_Bot != null)
                    thisUnit = m_Bot.Unit;
                else if (m_Trainer != null)
                    thisUnit = Unit.GetUnit(m_Trainer);


                // Only demo unit or person allowed to save a performance file
                if (thisUnit != null && (thisUnit.IsActive && (thisUnit.IsDemoUnit || thisUnit.IsPerson)))
                {
                    PerfContainer.SnapShot(this, thisUnit);
					PerfChanged = StatFlags.Zero; // After a snapshot we can clear out the perf flags.
                }
            }


            m_Mux.ReleaseMutex();

			if (OnUpdate != null)
				OnUpdate(this, m_SplitTime);
		}


		void UpdateLoc(ref StatFlags f)
		{
			if (m_Loc != null)
			{
				m_Loc.X = (m_Course.XUnits == CourseXUnits.Distance ? m_LapDistance : LapTime);

				if (m_Course.YUnits == CourseYUnits.Grade)
				{
					float g,w;
					if (m_State == State.Running)
					{
						g = m_Loc.Grade * 100;
						Course.PysicalSegment ps = (Course.PysicalSegment)m_Loc.Segment;
						w = ps == null ? 0:ps.Wind;
					}
					else
					{
						g = RM1_Settings.General.IdleGrade;
						w = 0.0F;
					}
					if (m_Grade != g || m_bForceGrade)
					{
						m_Grade = g;
						f |= StatFlags.Grade;
						if (m_Trainer != null)
							m_Trainer.SetGrade( Unregistered ? 0.0f:g, true);
						if (m_Bot != null)
							m_Bot.Grade = g;
						OnPropertyChanged("Grade");
					}
					if (m_Wind != w || m_bForceGrade)
					{
						m_Wind = w;
						f |= StatFlags.Wind;
						if (m_Trainer != null)
							m_Trainer.SetWind( w, true );
						if (m_Bot != null)
							m_Bot.Wind = w;
						OnPropertyChanged("Wind");
					}
					if (m_bForceGrade)
						m_bForceGrade = false;
				}
				else
				{
					float w = m_State != State.Running ? (float)RM1_Settings.General.IdleWatts:(float)m_Loc.Y;
					if (Course.YUnits == CourseYUnits.PercentAT)
					{
						if (m_State != State.Running)
							w = RM1_Settings.General.IdlePercentAT;

						m_PercentAT = w;
						// We need to find the percentage of the at from the rider.
						w = (float)(m_Rider == null ? 200.0:m_Rider.PowerAeT * w * 0.01);
					}
					if (m_Watts_Load != w || m_bForceWatts)
					{
						m_Watts_Load = w;
						f |= StatFlags.Watts_Load;
						if (m_Trainer != null)
							m_Trainer.SetWattsLoad(w,true);
						if (m_Bot != null)
							m_Bot.Watts_Load = w;
						OnPropertyChanged("Watts_Load");
						if (m_bForceWatts)
							m_bForceWatts = false;
					}
				}
				int nn = (int)(m_Loc.Normalized * 2048);
				if (nn != m_CourseScreenX)
				{
					f |= StatFlags.CourseScreenX;
					m_CourseScreenX = nn;
				}
			}
		}
        
        // for 3DP 
        //public LinkedList<PerfFrame.PerfPoint> Perfs = new LinkedList<PerfFrame.PerfPoint>();
        
        // for RM1 XML
        // use for running a performance if rider is using this
        public Perf PerfContainer = new Perf();
		
		// Formated outputs.
		public String Speed_String { get { return String.Format("{0:0.0}", m_Speed * ConvertConst.MetersPerSecondToMPHOrKPH); } }
		public String Speed_Avg_String { get { return String.Format("{0:0.0}", (double)m_Speed_Avg * ConvertConst.MetersPerSecondToMPHOrKPH); } }
		public String Speed_Max_String { get { return String.Format("{0:0.0}", m_Speed_Max * ConvertConst.MetersPerSecondToMPHOrKPH); } }
		public String Distance_String { get { return DistanceDisplayString;  } }
		public String Watts_String { get { return String.Format("{0:F0}",m_Watts); } }
		public String Watts_Avg_String { get { return String.Format("{0:F0}", (double)m_Watts_Avg); } }
		public String Watts_Max_String { get { return String.Format("{0:F0}",m_Watts_Max); } }
		public String Watts_Wkg_String { get { return String.Format("{0:0.0}",Watts_Wkg); } }
		public String TSS_String { get { return String.Format("{0:0.#}/{1:0.#}/{2:0.#}", m_TSS, m_IF, m_NP); } }
		public String Cadence_String { get { return String.Format("{0:F0}",m_Cadence); } }
		public String Cadence_Avg_String { get { return String.Format("{0:F0}", (double)m_Cadence_Avg); } }
		public String Cadence_Max_String { get { return String.Format("{0:F0}",m_Cadence_Max); } }
		public String HeartRate_String { get { return m_HeartRate <= 0 ? "-":String.Format("{0:F0}",m_HeartRate); } }
		public String HeartRate_Avg_String { get { return String.Format("{0:F0}", (double)m_HeartRate_Avg); } }
		public String HeartRate_Max_String { get { return String.Format("{0:F0}",m_HeartRate_Max); } }
		public String Calories_String { get { return String.Format(m_TotalCalories < 1000 ? "{0:0.0}" : "{0:F0}", m_TotalCalories); } }
		public String PulsePower_String { get { return String.Format("{0:F0}",m_PulsePower); } }
		public String Grade_String { get { return String.Format("{0:0.0}",m_Grade); } }
		public String Wind_String { get { return String.Format("{0:0.0}",m_Wind * ConvertConst.MetersPerSecondToMPHOrKPH); } }
		public String Gear_String { get { return String.Format("{0}",GearInches); } }
		public String Gearing_String {
			get {
				return GearingString;
			}
		}
	}
}
