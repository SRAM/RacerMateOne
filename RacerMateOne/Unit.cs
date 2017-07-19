using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
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

namespace RacerMateOne
{
	[Flags]
	public enum ActiveUnits
	{
		Zero = 0,
		Unit1 = (1 << 0),
		Unit2 = (1 << 1),
		Unit3 = (1 << 2),
		Unit4 = (1 << 3),
		Unit5 = (1 << 4),
		Unit6 = (1 << 5),
		Unit7 = (1 << 6),
		Unit8 = (1 << 7)
	}

	public enum AppModes: ushort
	{
		ThreeD = 0,
		RCV,
		PowerTraining,
		Spinscan,

		Unknown = 0xffff
	};


	public class UnitSave
	{
		public struct Node
		{
			public String RiderKey;
			public String BotKey;
			public bool IsActive;
			public bool IsDemoUnit;
			public int ControlUnitNumber;
		}
		public Node[] Nodes = new Node[8];
		public UnitSave()
		{
			int i;
			for (i = 0; i < 8; i++)
			{
				Unit unit = Unit.Units[i];
				Nodes[i].RiderKey = unit.Rider == null ? null : unit.Rider.DatabaseKey;
				Nodes[i].BotKey = unit.Bot == null ? null : unit.Bot.Key;
				Nodes[i].IsActive = unit.IsActive;
				Nodes[i].IsDemoUnit = unit.IsDemoUnit;
				Nodes[i].ControlUnitNumber = unit.Bot == null ? -1 : unit.Bot.ControlUnit == null ? -1 : unit.Bot.ControlUnit.Number;
			}
		}
		public void Restore()
		{
			int i;
			for (i = 0; i < 8; i++)
				RestoreNode(i);
		}
		public void RestoreNode(int i)
		{
			Unit unit = Unit.Units[i];
			unit.Rider = Nodes[i].RiderKey == null ? null : Riders.FindRiderByKey(Nodes[i].RiderKey);
			unit.Bot = Nodes[i].BotKey == null ? null : Bot.Create(Nodes[i].BotKey);
			unit.IsActive = Nodes[i].IsActive;
			unit.IsDemoUnit = Nodes[i].IsDemoUnit;
			if (unit.Bot != null && Nodes[i].ControlUnitNumber >= 0)
				unit.Bot.ControlUnit = Unit.Units[Nodes[i].ControlUnitNumber];
		}
	};

	public class ActiveUnitSave
	{
		bool[] m_Active = new bool[8];
		public void Save()
		{
			for (int i = 0; i < 8; i++)
				m_Active[i] = Unit.Units[i].IsActive;
		}
		public void Restore()
		{
			for (int i = 0; i < 8; i++)
				Unit.Units[i].IsActive = m_Active[i];
		}
		public void SetActive(int num, bool active)
		{
			if (num >= 8 || num < 0)
				return;
			m_Active[num] = active;
		}
		public bool GetActive(int num) { return num < 0 || num >= 8 ? false : m_Active[num]; }

		public ActiveUnitSave()
		{
			Save();
		}
	}


	public class Unit : INotifyPropertyChanged, IComparable
	{
        // TODO - Will need to fix this
        private static AppModes ms_AppMode = AppModes.Unknown; // Application that is actually running, "ThreeD","RCV","PowerTraining","Spinscan"
		public static AppModes AppMode
		{
			get { return ms_AppMode; }
			set { ms_AppMode = value; }
		}

		protected static string[] ms_AppModeStr = new string[]
        {
            "3D Cycling",
            "Real Course Video",
            "Power",
            "SpinScan"
        };

		public static String AppModeStr
		{
			get
			{
				return (UInt16)ms_AppMode >= ms_AppModeStr.Length ? "Unknown" : ms_AppModeStr[(UInt16)ms_AppMode];
			}
		}
		public static String GetAppModeString(AppModes mode)
		{
			return (UInt16)mode >= ms_AppModeStr.Length ? "Unknown" : ms_AppModeStr[(UInt16)mode];
		}


        //==========================================================================================
		public delegate void RiderChanged(Unit unit, Rider rider);
		public event RiderChanged OnRiderChanged;
		//==========================================================================================
		public delegate void NotifyEvent(Unit unit, StatFlags flags);
		public class NotifyNode
		{
			public String NotifyID;
			public Unit unit;
			public StatFlags mask;
			public event NotifyEvent ev;
			public NotifyNode( String nid, Unit u, StatFlags m )  { NotifyID = nid; unit = u; mask = m; }
			public bool Empty { get { return ev == null; } }
			public bool Force;
			public void Check()
			{
				if (ev != null)
				{
					StatFlags f = (unit == null ? Statistics.AllChanged : unit.m_Statistics.Changed) & mask;
					if (Force || f != StatFlags.Zero)
					{
						Force = false;
						ev(unit, f);
					}
				}
			}
		}
		private static Dictionary<String,NotifyNode>	ms_NotifyMap = new Dictionary<String,NotifyNode>(); 
		private static List<String> ms_NotifyRemove = new List<String>();

		private static String NotifyID(Unit unit, StatFlags flags)
		{
			return (unit == null ? "n" : unit.Number.ToString()) + "-" + String.Format("{0:x16}", (UInt64)flags);
		}

		static bool ms_Force;
		public static void ForceNotify(String nid)
		{
			NotifyNode n;
			if (nid != null && ms_NotifyMap.TryGetValue(nid, out n))
			{
				n.Force = true;
				ms_Force = true;
			}
		}

		public static String AddNotify(Unit unit, StatFlags mask, NotifyEvent ev)
		{
			if (ev == null)
				return "";
			NotifyNode n;
			String nid = NotifyID(unit,mask);
			if (!ms_NotifyMap.TryGetValue(nid, out n))
			{
				ms_NotifyMap[nid] = n = new NotifyNode(nid, unit, mask);
			}
			if (ms_NotifyRemove.Count > 0)
				ms_NotifyRemove.Remove(nid);
			n.ev += ev;
			return nid;
		}
		public static void RemoveNotify( Unit unit, StatFlags mask, NotifyEvent ev)
		{
			if (ev == null)
				return;
			NotifyNode n;
			String nid = NotifyID(unit,mask);
			if (ms_NotifyMap.TryGetValue( nid, out n ))
			{
				n.ev -= ev;
				if (n.Empty)
					ms_NotifyRemove.Add( nid );
			}
		}

		private int m_Order = 0;
		public int Order 
		{
			get { return m_bActive ? m_Order : 0; }
			protected set
			{
				if (m_Order != value)
				{
					m_Order = value;
					m_Statistics.Changed |= StatFlags.Order;
					Statistics.AllChanged |= StatFlags.Order;
				}
			}
		}

		TrainerUserConfigurable m_TC;
		public TrainerUserConfigurable TC 
		{
			get 
			{
				if (m_TC == null)
					m_TC = RM1_Settings.SavedTrainersList[Number];
				return m_TC; 
			}
			protected set 
			{
				m_TC = value;
			}
		}

		// INotifyPropertyChanged
		// =============================================================
		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		// IComparable
		// =============================================================
		public int CompareTo(object obj)
		{
			Unit unit = obj as Unit;
			if (unit != null)
			{
				double d = m_Statistics.Distance - unit.m_Statistics.Distance;
				if (d == 0)
					d = m_Statistics.Time - unit.m_Statistics.Time;

				return d < 0 ? 1 : d > 0 ? -1 : 0;
			}
			throw new ArgumentException("Object is not a Unit");
		}
		// =============================================================

		private Rider m_Rider;
		public Rider Rider
		{
			get { return m_Rider; }
			set
			{
				if (m_Rider != value)
				{
					if (value != null)
					{
						foreach (Unit u in Units)
						{
							if (u.m_Rider == value)
							{
								// Swap the two riders.... Make sure to clear out this rider so we don't go into an infinite loop
								Rider savedrider = m_Rider;
								m_Rider = null;
								u.Rider = savedrider;
								break;
							}
						}
					}
					m_Rider = value;
					if (m_Trainer != null)
						m_Trainer.Rider = m_Rider;
					m_Statistics.Rider = m_Rider;
					if (TC != null)
						TC.PreviousRiderKey = m_Rider == null ? "" : m_Rider.DatabaseKey;
					OnPropertyChanged("Rider");
					if (OnRiderChanged != null)
						OnRiderChanged(this, m_Rider);
				}
			}
		}

		// =============================================================
		public int RaceUnitNumber { get; protected set; }
		private bool m_bActive = false;
		public bool IsActive
		{
			get { return m_bActive; }
			set 
			{
				if (m_bActive == value)
					return;
				ms_Version++;	// Every time we change this we will need to reupdate some numbers on the next get.
				m_bActive = value;
				if (value)
				{
					ms_Active.Add(this);
					CheckOrder();
					m_Statistics.Lead = float.NaN; // No lead yet.. we will do it eventually

					ms_RaceUnit.Clear();
					for (int i = 0; i < MaxUnits; i++) // Race units keep the order.
					{
						if (Units[i].m_bActive)
						{
							Units[i].RaceUnitNumber = ms_RaceUnit.Count;
							ms_RaceUnit.Add(Units[i]);
						}
						else
							Units[i].RaceUnitNumber = -1;
					}
					m_Statistics.Course = ms_Course;
					if (ms_State == Statistics.State.Running)
						m_Statistics.Start();
					else if (ms_State == Statistics.State.Paused)
					{
						m_Statistics.Start();
						m_Statistics.Pause();
					}
					else
					{
						m_Statistics.Stop();
					}
				}
				else
				{
					ms_Active.Remove(this);
					ms_RaceUnit.Remove(this);
				}
			}
		}
		// =============================================================
		private RM1.DeviceType m_DeviceType = RM1.DeviceType.DOES_NOT_EXIST;
		public RM1.DeviceType DeviceType 
		{ 
			get { return m_DeviceType; } 
			protected set 
			{
				if (value != m_DeviceType)
				{
					m_DeviceType = value;
					OnPropertyChanged("DeviceType");
					OnPropertyChanged("Device");
				}
			}
		}
		/// <summary>
		/// Returns the device type string, but will not change if we have an inactive trainer hooked up to this unit.
		/// </summary>
		public String Device
		{
			get { return m_DeviceType == RM1.DeviceType.COMPUTRAINER ? "Computrainer":m_DeviceType == RM1.DeviceType.VELOTRON ? "Velotron":""; }
		}
		// =============================================================
		public bool IsVelotron { get; protected set; }
		//==========================================================================================
		bool m_IsDemoUnit = false;
		public bool IsDemoUnit
		{
			get { return m_IsDemoUnit; }
			set
			{
				if (m_IsDemoUnit == value)
					return;
				m_IsDemoUnit = value;
				ms_Version++;
			}
		}

		public bool IsRiderUnit
		{
			get { return (m_Rider != null && m_Bot == null) || (m_IsDemoUnit && m_Bot != null); }
		}


		public object RaceData; // Should only be changed by the race screen ... used as a holding area
		//==========================================================================================

		private RM1.Trainer m_Trainer;
		public RM1.Trainer Trainer
		{
			get { return m_Trainer; }
			set
			{
				if (m_Trainer == value)
					return;
				if (value != null)
				{
					foreach (Unit u in Units)
					{
						if (u.m_Trainer == value)
						{
							// Swap this.
							u.m_Trainer = m_Trainer;
							u.OnPropertyChanged("Trainer");
							break;
						}
					}
					IsVelotron = value.Type == RM1.DeviceType.VELOTRON;
				}
				else
					IsVelotron = false;
				m_Trainer = value;
				if (m_Trainer != null)
					m_Trainer.Rider = m_Rider;
				m_Statistics.Attach(m_Trainer);
                m_Statistics.Course = Course; // added course loaded

                OnPropertyChanged("Trainer");
				m_Statistics.Changed |= StatFlags.HardwareStatus | StatFlags.RiderName;
				Statistics.AllChanged |= StatFlags.HardwareStatus | StatFlags.RiderName;
			}
		}
		// =============================================================
		Bot m_Bot;
		public Bot Bot
		{
			get { return m_Bot; }
			set
			{
				if (m_Bot == value)
					return;
				ms_Version++;
				if (m_Bot != null)
				{
					m_Bot.Unit = null;
					m_Statistics.Attach((Bot)null);
				}
				m_Bot = value;
				if (m_Bot != null)
				{
					m_Bot.Unit = this;
					m_Statistics.Attach(m_Bot);
				}
				if (TC != null)
					TC.BotKey = (Bot != null ? Bot.Key:"");
				m_Statistics.Changed |= StatFlags.HardwareStatus | StatFlags.RiderName;
				Statistics.AllChanged |= StatFlags.HardwareStatus | StatFlags.RiderName;
			}
		}


		public bool IsPerson
		{
			get { return (m_Rider != null && m_Trainer != null && m_Bot == null); }
		}

		public bool IsBot
		{
			get { return (m_Bot != null && !IsDemoUnit); }
		}
		// =============================================================
		
		/// <summary>
		/// Forces a rider into that position.   Will create a rider if nessary to fill the area.
		/// </summary>
		/**************************************************************************************************

		**************************************************************************************************/

		public void ForceRider()
		{
			            
            if (Rider != null)
				return;
			foreach (Rider rider in Riders.RidersList)
			{
				if (!RiderInUse(rider))
				{
					Rider = rider;
					return;
				}
			}
			Rider r = new Rider();
			r.NickName = "Temporary Rider";
			Rider = r;
		}
		// =============================================================
		private bool m_MasterPad = false;
		public bool MasterPad
		{
			get { return m_MasterPad; }
			set
			{
				if (m_MasterPad == value)
					return;
				m_MasterPad = value;
				ms_Version++;
			}
		}

		// =============================================================
		public int iRider = -1;

		// =============================================================
		private Statistics m_Statistics = new Statistics();
		public Statistics Statistics { get { return m_Statistics; } }


		public int Number { get; protected set; }
		// =============================================================
		/**************************************************************************************************

		**************************************************************************************************/

		private Unit(int num) 
		{
			Number = num;
			RaceUnitNumber = -1;
		}
		// =============================================================

		// =============================================================
		// Static section.
		// =============================================================
		/**************************************************************************************************

		**************************************************************************************************/

		static Unit()
		{
			Units = new Unit[MaxUnits];
			for (int i = 0; i < MaxUnits; i++)
				Units[i] = new Unit(i);
			RM1.OnUpdate += new RM1.UpdateEvent(Update); // Always deal with this.
			HasStarted = false;
		}
		// =============================================================
		/**************************************************************************************************

		**************************************************************************************************/

		public static void OnPropertyChangedAll(string name)
		{
			PropertyChangedEventArgs pa = null;
			for (int i = 0; i < MaxUnits; i++)
			{
				if (Units[i].PropertyChanged != null)
				{
					if (pa == null)
						pa = new PropertyChangedEventArgs(name);
					Units[i].PropertyChanged(Units[i], pa);
				}
			}
		}



		public const int MaxUnits = 8;
		public static Unit[] Units { get; private set; }

		private static int ms_Version = 0;

		private static List<Unit> ms_Active = new List<Unit>();
		public static List<Unit> Active { get { return ms_Active; } }

		/// <summary>
		/// All the active units.
		/// </summary>
		public static List<Unit> RaceUnit { get { return ms_RaceUnit; } }
		private static List<Unit> ms_RaceUnit = new List<Unit>();

		private static Unit ms_FirstMasterUnit;
		public static Unit FirstMasterUnit { get { fixOtherLists(); return ms_FirstMasterUnit; } }


		private static int ms_OtherListVersion = -1;

		/**************************************************************************************************

		**************************************************************************************************/

		private static void fixOtherLists()
		{
			if (ms_Version == ms_OtherListVersion)
				return;
			ms_OtherListVersion = ms_Version;
			ms_BotUnits.Clear();
			ms_RiderUnits.Clear();
			ms_FirstMasterUnit = null;
			foreach (Unit unit in RaceUnit)
			{
				if (unit.Bot == null || unit.Bot.IsRider || unit.IsDemoUnit)
				{
					if (ms_FirstMasterUnit == null)
						ms_FirstMasterUnit = unit;
					ms_RiderUnits.Add(unit);
				}
				else
					ms_BotUnits.Add(unit);
			}
			if (ms_FirstMasterUnit == null)
			{
				foreach (Unit unit in Units)
				{
					if (unit.Trainer != null)
					{
						ms_FirstMasterUnit = unit;
						break;
					}
				}
			}
		}



		
		/// <summary>
		/// Active Human riders
		/// </summary>
		public static List<Unit> RiderUnits { get { fixOtherLists();  return ms_RiderUnits; } }
		private static List<Unit> ms_RiderUnits = new List<Unit>();

		/// <summary>
		/// Active Bots
		/// </summary>
		public static List<Unit> BotUnits { get { fixOtherLists();  return ms_BotUnits; } }
		private static List<Unit> ms_BotUnits = new List<Unit>(); 


		public static Unit Selected;	// Used for working on one unit and also for the single unit courses.

		private static Course ms_Course;
		public static Course Course
		{
			get { return ms_Course; }
			set
			{
				if (ms_Course != value)
				{
					ms_Version++;
					// Go through all the units and attach the course 
					ms_Course = value;
					ms_Laps = Course.Laps;

					foreach (Unit unit in Units)
						unit.m_Statistics.Course = value;
					RedoLaps();
					OnPropertyChangedAll("Course");
				}
			}
		}
		private static int ms_Laps = 1;
		public static int Laps
		{
			get { return ms_Laps; }
			set
			{
				int v = value < 1 ? 1:value;
				if (ms_Laps != v)
				{
					ms_Laps = v;
					RedoLaps();
					OnPropertyChangedAll("Laps");
				}
			}
		}
		static bool m_Manual = false;
		public static bool Manual		// Manual mode
		{
			get { return m_Manual; }
			set
			{
				if (m_Manual == value)
					return;
				m_Manual = value;
				RedoLaps();
			}
		}

		/**************************************************************************************************

		**************************************************************************************************/

		private static void RedoLaps()
		{
			if (ms_Course == null)
				return;
			double d = ms_Course.TotalX * ms_Laps;
			if (m_Manual)
			{
				// Make sure we don't stop.
				foreach (Unit unit in Units)
				{
					unit.m_Statistics.StopTime = 0;
					unit.m_Statistics.StopDistance = 0;
				}
			}
			else if (ms_Course.XUnits == CourseXUnits.Time)
			{
				foreach (Unit unit in Units)
					unit.m_Statistics.StopTime = d;
			}
			else
			{
				foreach (Unit unit in Units)
					unit.m_Statistics.StopDistance = d;
			}
			foreach (Unit unit in Units)
				unit.m_Statistics.NumberOfLaps = ms_Laps;
		}


		/**************************************************************************************************

		**************************************************************************************************/

		public static void LoadFromSettings()
		{
			foreach (TrainerUserConfigurable tc in RM1_Settings.SavedTrainersList)
			{
				if (tc.PositionIndex > MaxUnits || tc.PositionIndex < 1)
					continue;

				Unit unit = Units[tc.PositionIndex - 1];
				unit.TC = tc;
				unit.IsActive = tc.Active;
				unit.Trainer = tc.CurrentTrainer;
				unit.Rider = Riders.FindRiderByKey(tc.PreviousRiderKey);
				unit.Bot = Bot.Create(tc.BotKey);
			}
			OneActive = Units[RM1_Settings.General.TrainingRiderNumber];
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static void SaveToSettings()
		{
			foreach (Unit unit in Units)
			{
				if (unit.TC != null)
				{
					TrainerUserConfigurable tc = unit.TC;
					tc.Active = unit.IsActive;
					tc.PreviouslyDiscovered = unit.Trainer == null ? 0 : 1;
					tc.SavedPortName = unit.Trainer == null ? string.Empty : unit.Trainer.PortName;
					tc.PreviousRiderKey = unit.Rider == null ? "" : unit.Rider.DatabaseKey;
					tc.BotKey = unit.Bot == null ? "" : unit.Bot.Key;
				}
			}
			RM1_Settings.General.TrainingRiderNumber = OneActive.Number;
		}

		/**************************************************************************************************

		**************************************************************************************************/

		static void CheckOrder()
		{
			int i, len = Active.Count;
			if (len <= 1)
				return;

			for (i = 1; i < len; i++)
			{
				if (ms_Active[i].m_Statistics.Distance > ms_Active[i - 1].m_Statistics.Distance)
					break;
			}
			if (i < len)
			{
				// We need to resort.
				ms_Active.Sort();
				Statistics.AllChanged |= StatFlags.Order;
				i=1;
				foreach (Unit unit in ms_Active)
					unit.Order = i++;					
			}
		}

		public static Int64 LastTime { get; protected set; }

		// NOTE RM1.ms_Mux is locked here 
		/**************************************************************************************************
			called every 33 ms
		**************************************************************************************************/

		public static void IntervalUpdate(Int64 lasttime, double splittime)  {
			LastTime = lasttime;

			foreach (Unit unit in Active)  {
				if (unit.m_Bot != null)
					unit.Statistics.UpdateStats(unit.m_Bot, splittime);
				else if (unit.m_Trainer != null)
					unit.Statistics.UpdateStats(unit.m_Trainer, splittime);
			}
		}

		public static double AdvTime;	// Continuasly updates.

		/**************************************************************************************************
			Happens on the main thread... This happens as soon as we can get it to happen on the main
			thread.  Marsheld down
		**************************************************************************************************/

		public static void Update(double splittime)  {
			AdvTime += splittime;

			if (OnUpdate != null) {
				OnUpdate(splittime);
			}

			int acount = Active.Count;

			if (acount > 1 && ((Statistics.AllChanged & StatFlags.Distance) != StatFlags.Zero))  {
				// See if the order has changed
				CheckOrder(); // This could set the StatFlags.Order flag
			}

			Unit lastu = null;
			float d = float.NaN;

			foreach(Unit unit in Active)  {
				unit.Statistics.SetHRAlarm();			// Turn on and off the HR alarm if on.

				if (lastu == null) {
					unit.Statistics.Lead = float.NaN;
				}
				else  {
					float diff = (float)(lastu.Statistics.Distance - unit.Statistics.Distance);
					if (diff <= 0.0f) {
						diff = d;
					}
					else {
						d = diff;
					}
					unit.Statistics.Lead = diff;
				}
				lastu = unit;
			}										// foreach(Unit unit in Active)  {

			// Send out notifications... Poor mans approch to satistics handling.  Less binding involved.

			if (Statistics.AllChanged != StatFlags.Zero || ms_Force)  {
				foreach (KeyValuePair<String, NotifyNode> k in ms_NotifyMap) {
					k.Value.Check();
				}
				ms_Force = false;
			}

			if (ms_NotifyRemove.Count > 0)  {
				foreach (String n in ms_NotifyRemove) {
					ms_NotifyMap.Remove(n);
				}
				ms_NotifyRemove.Clear();
			}

			foreach (Unit unit in Units) {
				unit.m_Statistics.Changed = StatFlags.Zero;
			}
			Statistics.AllChanged = StatFlags.Zero;
		}													// Update()


		/**************************************************************************************************
			For all starting and stopping... Make sure we are out of the update loop before chaning the
			settings so all timers will match.
		**************************************************************************************************/

		private static Statistics.State ms_State;
		public static Statistics.State State { get { return ms_State; } }

		public static void ClearCalibration()  {
			Log.WriteLine("Unit.cs, ClearCalibration()");

			foreach (Unit unit in Units)  {
				if (unit.Trainer != null)
					unit.Trainer.CalibrateMode = false;
			}
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static void Start()  {
			Log.WriteLine("Unit.cs, Start()");

			if (ms_State == Statistics.State.Stopped)  {
				RM1.Trainer.LockStats();
				try  {
					Reset();

					Log.WriteLine("Unit.cs, Start() continuing...");

					ms_State = Statistics.State.Running;

					foreach (Unit unit in Active)  {
						unit.m_Statistics.Start();
						if (unit.Trainer != null)  {
							unit.Trainer.Reset_trnr_Averages();
							unit.Trainer.Paused = false;
						}
					}
					ReloadRiders();
				}
				catch (Exception ex)  {
					RM1.MutexException(ex);
				}
				RM1.Trainer.UnlockStats();

				HasStarted = true;
				Controls.Render3D.Start();
			}
		}
		/**************************************************************************************************

		**************************************************************************************************/

		public static void Stop()  {
			Log.WriteLine("Unit.cs, Unit::Stop()");

			if (ms_State == Statistics.State.Running || ms_State == Statistics.State.Paused)  {
				RM1.Trainer.LockStats();
				try  {
					ms_State = Statistics.State.Stopped;
					foreach (Unit unit in Active)  {
						unit.m_Statistics.Stop();
					}
				}
				catch (Exception ex) {
					RM1.MutexException(ex);
				}
				RM1.Trainer.UnlockStats();

				Controls.Render3D.Stop();
			}
		}
		/**************************************************************************************************

		**************************************************************************************************/

		public static void Pause()
		{
			if (ms_State == Statistics.State.Running)
			{
				RM1.Trainer.LockStats();
				try
				{
					ms_State = Statistics.State.Paused;
					foreach (Unit unit in Active)
					{
						unit.m_Statistics.Pause();
						if (unit.Trainer != null)
							unit.Trainer.SetPaused(true, true);
					}
				}
				catch (Exception ex) { RM1.MutexException(ex); }

				RM1.Trainer.UnlockStats();
				Controls.Render3D.Pause();
			}
		}
		public static void UnPause()
		{
			if (ms_State == Statistics.State.Paused)
			{
				RM1.Trainer.LockStats();
				try
				{
					ms_State = Statistics.State.Running;
					foreach (Unit unit in Active)
					{
						unit.m_Statistics.UnPause();
						if (unit.Trainer != null)
							unit.Trainer.SetPaused(false, true);
					}
				}
				catch (Exception ex) { RM1.MutexException(ex); }
				RM1.Trainer.UnlockStats();

				Controls.Render3D.UnPause();
			}
		}
		/**************************************************************************************************

		**************************************************************************************************/

		public static void Reset()
		{
			RM1.Trainer.LockStats();
			try
			{
				Statistics.MasterTimer = 0.0;
				Stop();
				foreach (Unit unit in Units)
				{
					unit.m_Statistics.Reset();
					if (unit.Trainer != null)
					{
						unit.Trainer.Reset_trnr_Averages();
						unit.Trainer.SetPaused(false, true);
					}
				}
				ReloadRiders();
			}
			catch (Exception ex) { RM1.MutexException(ex); }
			RM1.Trainer.UnlockStats();

			HasStarted = false;
			Controls.Render3D.Reset();
		}

		public static void DeleteRecentPerformance()
		{
			foreach (Unit unit in Units)
			{
				if (unit.Statistics.SavedFileName != null)
				{
					try { System.IO.File.Delete(unit.Statistics.SavedFileName); }
					catch { Log.Debug("In DeleteRecentPerformance, Couldn't delete \"" + unit.Statistics.SavedFileName + "\""); }
					unit.Statistics.SavedFileName = null;
				}
			}
		}



		public class OverviewClass : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;
			public void OnPropertyChanged(string name)
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}
		public static OverviewClass Overview = new OverviewClass();

		public static event RM1.UpdateEvent OnUpdate;

		public static Unit GetUnit(RM1.Trainer trainer)
		{
			foreach (Unit u in Units)
			{
				if (trainer == u.m_Trainer)
					return u;
			}
			return null;
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static bool FixRiderList()
		{
            //Debug.WriteLine("in fix rider list starting with " + RiderUnits.Count);
			if (RiderUnits.Count > 0)
				return true;
			// Find one with a rider.
			foreach (Unit unit in Units)
			{
				if (unit.IsRiderUnit)
				{
					unit.IsActive = true;
					SaveToSettings();
					return true;
				}
			}
			foreach (Unit unit in Units)
			{
				if (unit.Trainer != null)
				{
					unit.Rider = Riders.RidersList.First();
					unit.Bot = null;
					unit.IsActive = true;
					SaveToSettings();
					return true;
				}
			}
			Units[0].Rider = Riders.RidersList.First();
			Units[0].Bot = null;
			Units[0].IsActive = true;
			SaveToSettings();
			return true;
		}



		/**************************************************************************************************

		**************************************************************************************************/

		public static void AtLeastOneActive()
		{
           // Debug.WriteLine("in at least one active " );
            foreach (Unit unit in Units)
			{
				if (unit.Rider != null && unit.Bot == null && unit.Trainer != null && unit.IsActive)
					return;
			}
			foreach (Unit unit in Units)
			{
				if (unit.Rider != null && unit.Bot == null && unit.Trainer != null)
				{
					unit.IsActive = true;
					return;
				}
			}
			foreach (Unit unit in Units)
			{
				if (unit.Rider != null && unit.Bot == null)
				{
					unit.IsActive = true;
					return;
				}
			}
			Rider trider = null;
			foreach (Unit unit in Units)
			{
				if (unit.Rider != null)
				{
					trider = unit.Rider;
					break;
				}
			}
			if (trider == null)
				trider = Riders.DefaultRider;

			foreach (Unit unit in Units)
			{
				if (unit.Trainer != null && unit.Bot == null)
				{
					unit.Rider = trider;
					unit.IsActive = true;
					return;
				}
			}
			foreach (Unit unit in Units)
			{
				if (unit.Bot == null)
				{
					unit.Rider = trider;
					unit.IsActive = true;
					return;
				}
			}
			foreach (Unit unit in Units)
			{
				if (unit.Trainer != null)
				{
					unit.Rider = trider;
					unit.Bot = null;
					unit.IsActive = true;
					return;
				}
			}
			Units[0].Rider = trider;
			Units[0].Bot = null;
			Units[0].IsActive = true;
			
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static bool AnyVelotron
		{
			get
			{
				foreach (RM1.Trainer tc in RM1.ValidTrainers)
				{
					if (tc.Type == RM1.DeviceType.VELOTRON && tc.IsConnected)
						return true;
				}
				return false;
			}
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static void DeactivateAll()
		{
			foreach (Unit unit in Units)
				unit.IsActive = false;
		}
		/**************************************************************************************************

		**************************************************************************************************/

		public static void ClearAll()
		{
			foreach (Unit unit in Units)
			{
				unit.IsActive = false;
				unit.Bot = null;
			}		
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static bool RiderInUse(Rider rider)
		{
			if (rider == null)
				return false;

			foreach (Unit unit in Units)
			{
				if (rider == unit.Rider)
					return true;
			}
			return false;
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static Unit TrainerUnit(RM1.Trainer tc)
		{
			foreach( Unit unit in Units )
			{
				if (unit.Trainer == tc)
					return unit;
			}
			return null;
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static void AllocateHardware(bool redoall)
		{
			if (redoall)
			{
				foreach (Unit u in Units)
				{
					u.Trainer = null;
				}
			}
			foreach (RM1.Trainer tc in RM1.ValidTrainers)
			{
				Unit unit = TrainerUnit(tc);
				if (unit == null)
				{
					foreach (Unit u in Units)
					{
						if (u.Trainer == null)
						{
							u.Trainer = tc;
							break;
						}
					}
				}
			}
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static void AllocateBots()
		{
			int rider = 0;
			List<Unit> rlist = new List<Unit>();
			foreach (Unit unit in RiderUnits)
			{
				// Must be a real rider.
				if (unit.Bot == null)
					rlist.Add(unit);
			}
			foreach (Unit unit in BotUnits)
			{
				if (unit.Bot != null)
				{
					Unit u = rlist.Count > 0 ? rlist[rider++] : null;
					if (rider >= rlist.Count)
						rider = 0;
					unit.Bot.ControlUnit = u;
				}
			}
		}

		static bool ms_AllowDrafting = true;
		public static bool AllowDrafting
		{
			get { return ms_AllowDrafting; }
			set
			{
				if (value == ms_AllowDrafting)
					return;
				if (ms_AllowDrafting) // do this before so the off drafting will take
				{
					foreach (Unit unit in Units)
					{
						unit.Statistics.Drafting = false;
					}
				}
				ms_AllowDrafting = value;
			}
		}

		static Unit m_OneActive = null;
		/**************************************************************************************************

		**************************************************************************************************/

		public static Unit OneActive
		{
			get
			{
				// Verify that the one active one is a valid one active.
				if (m_OneActive == null || !m_OneActive.IsRiderUnit)
					m_OneActive = null;
				if (m_OneActive == null)
				{
					// We need an active unit.
					foreach(Unit unit in Units)
					{
						if (unit.IsRiderUnit)
						{
							m_OneActive = unit;
							break;
						}
					}
				}
                if (m_OneActive == null)
                {
                    Debug.WriteLine("found null active, taking first entry");
                    m_OneActive = Units[0];
                }
				return m_OneActive;
			}
			set
			{
				Unit unit = value;
				if (unit != null && !unit.IsRiderUnit)
					unit = null;
				m_OneActive = unit;
			}
		}
		/**************************************************************************************************

		**************************************************************************************************/

		public static void ActivateOneActive()
		{
			foreach (Unit unit in Units)
			{
				if (unit.IsActive)
				{
					OneActive = unit;
					break;
				}
			}
			Unit au = OneActive;
			foreach (Unit unit in Units)
			{
				unit.IsActive = unit == au;
			}
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static void ClearPerformanceBots(String savehash)
		{
			foreach (Unit unit in Units)
			{
				PerformanceBot pb = unit.Bot as PerformanceBot;
				if (pb != null && pb.Course.HeaderHash != savehash)
				{
					unit.Bot = null;
				}
			}
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static void ClearPerformanceBots(Course course, bool videomode)
		{
			if (course == null)
				return;
			if (videomode)
			{
				course = course.VideoCourse;
				if (course == null)
				{
					// Just clear all the performance bots out.
					ClearPerformanceBots( "" );
					return;
				}
			}
			PerformanceBot pfirst = null;

			foreach (Unit unit in Units)
			{
				PerformanceBot pb = unit.Bot as PerformanceBot;
				if (pb == null)
					continue;

				if (videomode)
				{
					if (pb.Course.VideoCourse != course || (pfirst != null && pfirst.Course.CourseHash != pb.Course.CourseHash))
						unit.Bot = null;
				}
				else if (pb.Course.CourseHash != course.CourseHash)
					unit.Bot = null;
				pfirst = pb;
			}
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static Course PerformanceCourse(bool videomode)
		{
			Course course = null;
			foreach (Unit unit in Units)
			{
				PerformanceBot pb = unit.Bot as PerformanceBot;
				if (pb == null || pb.Course == null)
					continue;
				Course tc;
				if (videomode)
				{
					tc = pb.Course.VideoCourse;
					if (tc == null)
					{
						unit.Bot = null;
						continue;
					}
				}
				else
					tc = pb.Course;

				if (course == null)
				{
					course = tc;
					if (videomode)
					{
						course.StartAt = pb.Course.StartAt;
						course.Laps = pb.Course.Laps;
					}
				}
				else
				{
					if (tc == null || tc.HeaderHash != course.HeaderHash)
						unit.Bot = null;
				}
			}
			return course;
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static ActiveUnits ActiveUnits
		{
			get
			{
				int i = 1;
				ActiveUnits au = ActiveUnits.Zero;
				foreach (Unit unit in Units)
				{
					if (unit.IsActive)
						au |= (ActiveUnits)i;
					i += i;
				}
				return au;
			}
			set
			{
				int i = 1;
				foreach (Unit unit in Units)
				{
					bool active = (value & (ActiveUnits)i) != ActiveUnits.Zero;
					if (active && unit.Rider == null && unit.Bot == null)
						active = false;
					unit.IsActive = active;
					i += i;
				}
                //Debug.WriteLine("about to call atleastoneactive from ActiveUnits");
			
                AtLeastOneActive();
                //Debug.WriteLine("return atleastoneactive from ActiveUnits");
			
            }
		}

		/**************************************************************************************************

		**************************************************************************************************/

		public static void UpdateTrainerData(bool clearserial)  {
			foreach (Unit unit in Units)  {
				if (unit.Trainer != null)  {
#if DEBUG
					Log.WriteLine("Unit.cs, UpdateTrainerData()");
#endif
					unit.TC.RememberedDeviceType = unit.Trainer.TypeString == "Unknown" ?
						(unit.TC.RememberedDeviceType == "Unknown" ? "Computrainer" : unit.TC.RememberedDeviceType) :
						unit.Trainer.TypeString;
					unit.TC.Active = true;
					unit.TC.SavedPortName = unit.Trainer.PortName;
				}
				else
				{
					unit.TC.RememberedDeviceType = "Unknown";
					if (clearserial)
						unit.TC.SavedPortName = string.Empty;
				}
			}
		}
		public static void UpdateTrainerData() { UpdateTrainerData(false); }


		public static bool HasStarted { get; protected set; }

		/**************************************************************************************************

		**************************************************************************************************/

		public static void ReloadRiders()
		{
			foreach (Unit unit in Units)
			{
				if (unit.m_Trainer != null)
					unit.m_Trainer.Rider = unit.m_Rider;
			}
		}


	}

}
