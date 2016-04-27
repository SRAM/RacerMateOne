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
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.IO;

#if DEBUG
using System.Diagnostics;				// Needed for process invocation
#endif

namespace RacerMateOne
{
	public interface IBotInfo
	{
		Bot Create(String key);
		TextBlock Info();
		String DefaultKey { get; }
	}

    public class PerformanceFileEntry
    {
        public DateTime Date;
        public string CourseName;
        public CourseType CourseType;
        public string Ridername;
        public double TimeMS;
        public string FileName;

        public TextBlock Info()
        {
            TextBlock tb = new TextBlock();
            TextBlock t = new TextBlock();
            InlineCollection tbi = tb.Inlines;
            tbi.Add(Ridername + "  " + CourseName);
            t = new TextBlock();
            t.Foreground = Brushes.Gray;
            tbi.Add(new LineBreak());
            string timeinfo = string.Format("{0} ( {1} )", Date.ToString(), Statistics.SecondsToTimeString((double)TimeMS / 1000)); 
            tbi.Add(timeinfo);
            return tb;
        }
    }

    public class Bot : RM1.IStats, IRiderModel
	{
		static Bot()
		{
            Bots.Add(WattsBot.gInfo);
			Bots.Add(FixedSpeedBot.gInfo);
			Bots.Add(DelayBot.gInfo);
			Bots.Add(SpeedBot.gInfo);
			Bots.Add(StdWattsBot.gInfo);
			Bots.Add(HRBot.gInfo);
			//Bots.Add(AIBot.gInfo);
			Bots.Add(FTPBot.gInfo);
			Bots.Add(AnTBot.gInfo);
			OtherBots.Add(PerformanceBot.gInfo);
        }


		public bool Ready = true;

//        public virtual RM1.IStats Stats { get { return this; } }
        public bool IsPacer { get; protected set; }
		protected Bot()
		{
			IsPacer = true;
			IsRider = false;
		}
		protected String m_BaseName = "Unknown";
		protected String BaseName
		{
			set
			{
				if (m_BaseName == value)
					return;
				m_BaseName = value;
				if (m_Unit != null)
					m_Unit.Statistics.SetChangedFlags(StatFlags.RiderName);
			}
		}

		public bool IsRider { get; protected set; }

		public virtual String DisplayName { get { return Name; } } 

		protected String m_Name = null;
		public virtual String Name 
		{
			get { return m_Name != null ? m_Name : m_BaseName; }
			set { m_Name = value; }
		}
		public String Key { get; protected set; }
		public IBotInfo Info { get; protected set; }

		public virtual Object UnitContent
		{
			get { return Name; }
		}

		public virtual object DisplayText
		{
			get { return Name; }
		}



		public delegate void KeyChangedEvent( Bot bot );
		public event KeyChangedEvent KeyChanged;
		public void DoKeyChangeEvent()
		{
			if (KeyChanged != null)
				KeyChanged(this);
		}

		public virtual UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}
		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}

		public virtual void Adjust(int dir) { }

		static public readonly List<IBotInfo> Bots = new List<IBotInfo>();
		static public readonly List<IBotInfo> OtherBots = new List<IBotInfo>();
		static protected String[] PreParse(String key)
		{
			String[] ss = key.Split(',');
			for (int i = 0; i < ss.Length; i++) ss[i] = ss[i].Trim();
			return ss;
		}
		public static Bot Create(String botkey)
		{
			if (botkey == null || botkey == "")
				return null;
			foreach (IBotInfo binfo in Bots)
			{
				Bot bot = binfo.Create(botkey);
				if (bot != null)
					return bot;
			}
			foreach (IBotInfo binfo in OtherBots)
			{
				Bot bot = binfo.Create(botkey);
				if (bot != null)
					return bot;
			}
			return null;
		}

		public virtual void Edit( UIElement calling_control )
		{
		}



		static float[] ms_BlankBars = new float[RM1.BarCount];
		//==============================================
		public virtual bool Metric { get { return true; } }

		public virtual Int64 Ticks { get { return ms_LastTicks; } }
		public virtual float SplitTime { get { return ms_SplitTime; } }

		protected float fval = 0.0f;

#if DEBUG
		/*
		protected long totdeltas = 0;
		protected long tickcnt = 0;
		protected double avgdelta = 0.0;
		protected long now = 0;
		protected long delta;
		protected long lastnow = DateTime.Now.Ticks;
		*/
#endif

		protected float m_Speed = 0.0f;
		public virtual float Speed { get { return m_Speed; } }

		protected float m_Cadence = 0.0f;
        public virtual float Cadence { get { return m_Cadence; } }
		public virtual float HeartRate { get { return 0.0f; } }
		public virtual float Watts { get { return 0.0f; } }

		public virtual float SS { get { return 0.0f; } }
		public virtual float SSLeft { get { return 0.0f; } }
		public virtual float SSRight { get { return 0.0f; } }
		public virtual float SSLeftSplit { get { return 0.0f; } }
		public virtual float SSRightSplit { get { return 0.0f; } }

		public virtual float Calories { get { return 0.0f; } }
		public virtual float PulsePower { get { return 0.0f; } }
		public virtual float NP { get { return 0.0f; } }
		public virtual float IF { get { return 0.0f; } }
		public virtual float TSS { get { return 0.0f; } }

		public virtual float[] Bars { get { return ms_BlankBars; } }
		public virtual float[] AverageBars { get { return ms_BlankBars; } }

		public virtual int FrontGear { get { return 0; } }	// Velotron only -1 if not valid
		public virtual int RearGear { get { return 0; } }	// Velotron only -1 if not valie
		public virtual int GearInches { get { return 0; } } // Velotron only - We should be able to caluclate this for non-computrainers.

		protected float m_Grade;
		public virtual float Grade 
		{
			get { return m_Grade; }
			set { m_Grade = value; }
		}
		protected float m_Watts_Load;
		public virtual float Watts_Load
		{
			get { return m_Watts_Load; }
			set { m_Watts_Load = value; }
		}

		protected float m_Wind;
		public virtual float Wind
		{
			get { return m_Wind; }
			set { m_Wind = Wind; }
		}

		protected bool m_Drafting;
		public virtual bool Drafting
		{
			get { return m_Drafting; }
			set { m_Drafting = Drafting; }
		}


		public event RM1.IStatsEvent OnUpdate;
		//=================================================
		int m_IntervalCount = 0;

		//=================================================
		public virtual bool NeedsControlUnit { get { return false; } }
		public Unit ControlUnit
		{
			get { return m_ControlUnit; }
			set
			{
				if (m_ControlUnit == value)
					return;
				m_ControlUnit = value;
				ControlUnitChanged();
			}
		}
		protected Unit m_ControlUnit;
		protected virtual void ControlUnitChanged()
		{
		}



		//=================================================
		protected Unit m_Unit;
		public Unit Unit
		{
			get { return m_Unit; }
			set
			{
				if (m_Unit == value)
					return;
				RM1.Trainer.LockStats();
				try
				{
					m_Unit = value;
					if (m_Unit != null)
					{
						if (ms_BotList.Count == 0)
						{
							RM1.OnInterval += new RM1.IntervalEvent(OnInterval);
							RM1.Trainer.IncreaseStartCount();
						}
						ms_BotList.AddLast(this);

					}
					else
					{
						int n = ms_BotList.Count;
						ms_BotList.Remove(this);
						if (n > 0 && ms_BotList.Count == 0)
						{
							RM1.OnInterval -= new RM1.IntervalEvent(OnInterval);
							RM1.Trainer.DecreaseStartCount();
						}
					}
				}
				catch (Exception ex) { RM1.MutexException(ex); }
				RM1.Trainer.UnlockStats();
			}
		}

		protected virtual void Update()
		{
			m_IntervalCount++;
			if (OnUpdate != null)
				OnUpdate(this, null);
		}

		//========================
		protected static LinkedList<Bot> ms_BotList = new LinkedList<Bot>();
		protected static Int64 ms_LastTicks;
		protected static float ms_SplitTime;

		protected static void OnInterval(Int64 lastticks, float splittime)
		{
			ms_LastTicks = lastticks;
			ms_SplitTime = splittime;
			foreach (Bot bot in ms_BotList)
				bot.Update();
		}
		//========================
		protected IRiderModel m_OverrideModel = null;
		public IRiderModel OverrideModel
		{
			get { return m_OverrideModel; }
			set
			{
				m_OverrideModel = value;
			}
		}
		public virtual RiderModels Model { get { return m_OverrideModel != null ? m_OverrideModel.Model : RiderModels.Chrome_Male_Triathlon; } }
		public virtual Color GetMaterialColor(RiderMaterials m)
		{
			return m_OverrideModel != null ? m_OverrideModel.GetMaterialColor(m) : Colors.White;
		}

		public virtual float HrMax
		{
			get
			{
				return m_ControlUnit != null && m_ControlUnit.Rider != null ? m_ControlUnit.Rider.HrMax : 170;
			}
		}
	}


	public class DemoBot : Bot
	{
		public DemoBot()
		{
			BaseName = "Demo";
			Key = "Demo";
			Info = gInfo;
			DestSpeed = 0.0;
		}
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				Bot bot = null;
				try
				{
					bot = new DemoBot();
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Demo");
				return tb;
			}
			public String DefaultKey { get { return "Demo"; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();
		public static double StdGearInches = 70;  // For caluclating RPM - uses this gear ratio.
		public double m_DestSpeed;
		public double m_DestRPM;
		public double DestSpeed
		{
			get { return m_DestSpeed; }
			set
			{
				m_DestSpeed = value < 0 ? 0 : value;

				double t = m_DestSpeed * ConvertConst.MetersPerSecondToMPH;
				if (t > 0)
				{
					m_DestRPM = 80 * Math.Sqrt(t / 10);
				}
				else
					m_DestRPM = 0;
			}
		}
		/// <summary>
		/// Round to the nerest MPH or KPH value.
		/// </summary>
		public double DestSpeedInt
		{
			set
			{
				double v;
				v = value * (RM1_Settings.General.Metric ? ConvertConst.MetersPerSecondToKPH : ConvertConst.MetersPerSecondToMPH);
				v = Math.Round(v);
				v = v * (RM1_Settings.General.Metric ? ConvertConst.KPHToMetersPerSecond : ConvertConst.MPHToMetersPerSecond);
				DestSpeed = v;
			}
		}
		public override void Adjust(int dir)
		{
			DestSpeed += dir * (RM1_Settings.General.Metric ? ConvertConst.KPHToMetersPerSecond : ConvertConst.MPHToMetersPerSecond);
		}


		protected override void Update()
		{
			m_Speed = (float)m_DestSpeed;
			m_Cadence = (float)(m_Cadence * 0.9 + m_DestRPM * 0.1);
			base.Update();
		}
	}

	#region OLD DelayBot
	/*
	public class DelayBotOld : Bot
	{
		int m_Delay;
		DelayBotOld(int delay)
		{
			Delay = delay;

			BaseName = String.Format("Pacer Time {0}s", m_Delay);
			Key = String.Format("DelayBot,{0}", m_Delay);
			Info = gInfo;
		}

		public int Delay
		{
			get { return m_Delay; }
			set
			{
				int v = value < 1 ? 1 : value > 6 ? 6 : value;
				if (m_Delay != v)
				{
					m_Delay = v;
					BaseName = String.Format("Pacer Time, {0}s", m_Delay);
					Key = String.Format("DelayBot,{0}", m_Delay);
					m_DisplayName = String.Format("Delay {0}s", m_Delay);
					DoKeyChangeEvent();
				}
			}
		}

		String m_DisplayName;
		public override String DisplayName { get { return m_DisplayName; } }


		public override void Adjust(int dir)
		{
			Delay += dir;
		}

		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0,0,0,0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}

		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}

		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_DelayBot dlg = new Dialogs.Edit_DelayBot();
			//dlg.Bot = this;
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
		}

		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss.Length < 1 || ss[0] != "DelayBot")
					return null;
				try
				{
					bot = new DelayBotOld(ss.Length < 2 ? 3:(int)Convert.ToInt32(ss[1]));
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Smart Pacer - Delay ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				tbi.Add("(1 to 6 seconds)");
				return tb;
			}
			public String DefaultKey { get { return "DelayBot"; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();
        Rider m_CurRider;

        StatFlags neededFlags = StatFlags.Speed | StatFlags.Cadence | StatFlags.Grade | StatFlags.Watts_Load;
        Performance workPerformance = new Performance();

        protected override void Update()
        {
            if (m_Unit.Rider != m_CurRider)
            {
                m_CurRider = m_Unit.Rider;
            }
            if (Unit.Units.Length > 1)
            {
                // todo - needs to add picking the unit to shadow from.
                Unit delay = Unit.Units[0];
                RM1.IStats t = (RM1.IStats)delay.Statistics.PerfContainer.GetRunningFrame(ref workPerformance, m_Unit.Statistics.Time - m_Delay, neededFlags);
                if (t != null)
                {
                    m_Speed = t.Speed;
                    m_Cadence = t.Cadence;
                    m_Grade = t.Grade;
                    m_Watts_Load = t.Watts_Load;
                }
            }
            switch (Unit.State)
            {
                case Statistics.State.Stopped: m_Speed = 0.0f; break;
                case Statistics.State.Paused: return;	// Actually we don't need to do anything here.
            }
            base.Update();
        }

	}
	*/
	#endregion

	public class PerformanceBot : Bot
	{
		class BotModel : IRiderModel
		{
			Bot m_Parent;
            private String m_gender = "M";
			public RiderModels Model
			{
				get 
				{
					Unit unit = m_Parent.Unit;
                    if (unit != null && unit.Rider != null)
                        return unit.Rider.Model;
                    else
                        return m_gender == "F" ? RiderModels.Female_Triathlon : RiderModels.Male_Triathlon;
					//return unit != null && unit.Rider != null ? unit.Rider.Model : RiderModels.Male_Triathlon; 
				}
			}
			public Color GetMaterialColor(RiderMaterials material)
			{
				return Colors.White;
			}
			public BotModel(Bot parent, String gender)
            { 
                m_Parent = parent;
                m_gender = gender.ToUpper() == "F" ? "F" : "M"; 
            }
		}


        
        private String m_PerfBotGender = "M";

		const String c_BotName = "Performance";
		int m_Delay;
		public PerformanceBot(string filename, int percent, Course course)
		{
			if (filename == null)
				filename = "";
			Info = gInfo;
			m_Course = course;
			m_Delay = 0;
			m_LoadedPerformanceFile = filename;
			Percent = percent; // Deals with the update names.
			Ready = false;
			IsRider = true;
            if (course != null && course.PerformanceInfo.Gender != null)
            {
                m_PerfBotGender = course.PerformanceInfo.Gender.ToUpper() == "F" ? "F" : "M";
            }
            else
                m_PerfBotGender = "M";

            m_OverrideModel = new BotModel(this, m_PerfBotGender);
        }


		public override Object UnitContent
		{
			get
			{
				TextBlock tb = new TextBlock();
				TextBlock t;
				InlineCollection tbi = tb.Inlines;
				t = new TextBlock();
				t.Text = "Perf ";
				tbi.Add(t);

				t = new TextBlock();
				t.FontSize = 12;
				t.Text = String.Format("{0}%,",Percent);
				tbi.Add( t );

				t = new TextBlock();
				t.FontSize = 12;
				t.Width = 80;
				t.TextTrimming = TextTrimming.CharacterEllipsis;
				t.FontSize = 12;
				t.Text = Course.PerformanceInfo.RiderName;
				tbi.Add(t);

				tb.ToolTip = Course.PerformanceHeader.Date.ToString();
				return tb;

			}
		}


		Course m_Course;
		public Course Course
		{
			get
			{
				if (m_Course == null)
				{
					if (!Courses.FileDB.TryGetValue(m_LoadedPerformanceFile.ToLower(), out m_Course ))
					{
						Course c = new Course();
						if (c.Load(LoadedPerfFile))
							m_Course = c;
					}
				}
				return m_Course;
			}
		}

		void UpdateNames()
		{
			if (Course != null & Course.PerformanceInfo != null)
				m_DisplayName =  String.Format("{1} %{0}",Course.PerformanceInfo.RiderName,m_Percent);
			else
				m_DisplayName = String.Format("P {0}", m_Percent ); 
			BaseName = String.Format("Performance {0}%", m_Percent);
			Key = c_BotName + "," + LoadedPerfFile;
			if (m_Percent != 0)
				Key += String.Format("{0}", m_Percent);
		}
		String m_DisplayName;
		public override String DisplayName { get { return m_DisplayName; } }

		int m_Percent;
		float m_SpeedAdj;
		public int Percent
		{
			get
			{
				return m_Percent;
			}
			set
			{
				int v = value < -30 ? -30 : value > 30 ? 30 : value;
				m_Percent = v;
				m_SpeedAdj = (float)((v / 100.0) + 1.0);
				UpdateNames();
			}
		}

		public static double StartDistance;
		double m_CurStartDistance;
		double m_TimeOffset;


		public override void Adjust(int dir)
		{
			Percent += dir;
		}

		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}

		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}
		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_PerformanceBot dlg = new Dialogs.Edit_PerformanceBot();
			dlg.Bot = this;
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
			DoKeyChangeEvent();
		}
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				PerformanceBot bot = null;
				if (ss.Length < 2 || String.Compare(ss[0],c_BotName,true) != 0)
					return null;
				try
				{
					string filename = null;
					int percent = 0;
					if (ss.Length > 1 && ss[1].Length > 0)
						filename = ss[1];
					if (ss.Length > 2)
						percent = (int)Convert.ToInt32(ss[2]);
					bot = new PerformanceBot(filename,percent, null);
					if (bot.Course == null)
						bot = null;
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Perf ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				tbi.Add("(-30% to 30%)");
				return tb;
			}
			public String DefaultKey { get { return c_BotName; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();
		Rider m_CurRider;

		String m_LoadedPerformanceFile = null;
		public String LoadedPerfFile
		{
			get { return m_LoadedPerformanceFile; }
			set
			{
				if (value != m_LoadedPerformanceFile)
				{
					m_LoadedPerformanceFile = value;
					if (m_LoadedPerformanceFile == null)
						m_LoadedPerformanceFile = "";
					UpdateNames();
					m_Perf = null;	// Force reload and free the other one.
				}
			}
		}

		StatFlags neededFlags = gVar.PerformanceFlags;
		Performance prd = new Performance();

		Perf m_Perf;

		/// <summary>
		/// Forces the performance to be reloaded.   Use a background worker to load stuff.
		/// </summary>
		/// <param name="bw"></param>
		public void LoadPerformance(BackgroundWorker bw)
		{
			if (m_Perf == null)
			{
				m_Perf = new Perf();
				m_Perf.LoadRawTemps(bw, m_LoadedPerformanceFile);
			}
		}
		public int EstLoadPerformance()
		{
			if (m_Perf == null)
			{
				try
				{
					FileInfo finfo = new FileInfo(m_LoadedPerformanceFile);
					return (int)finfo.Length;
				}
				catch { }
			}
			return 0;
		}


		protected override void Update()
		{
			if (!Ready)
				return;
			if (m_Unit.Rider != m_CurRider)
			{
				m_CurRider = m_Unit.Rider;
			}
			Perf perf = m_Perf;
			if (perf == null)
			{
				LoadPerformance(null);
                perf = m_Perf; // reassigns after to give perf a valid value
			}
			if (m_CurStartDistance != StartDistance)
			{
				m_CurStartDistance = StartDistance;
				m_TimeOffset = perf.FindDistanceTime(m_CurStartDistance);
			}


			if (perf.GetLoadedList().Count > 0 && (m_Unit.Statistics.Time - m_Delay + m_TimeOffset) > 0)  {
				perf.GetLoadedFrame(ref prd, m_Unit.Statistics.Time * m_SpeedAdj - m_Delay + m_TimeOffset, neededFlags);

				m_Speed = prd.Speed * m_SpeedAdj;
				m_Cadence = prd.Cadence * m_SpeedAdj;
				m_Grade = prd.Grade;
				m_Watts_Load = prd.Watts_Load;

				//m_Distance = prd.Distance;
				m_Wind = prd.Wind;
				m_Watts = prd.Watts * m_SpeedAdj;

				m_HeartRate = prd.HeartRate;
				m_SSLeftATA = prd.SSLeftATA;
				m_SSRightATA = prd.SSRightATA;
				m_SSRight = prd.SSRight;			// right spinscan
				m_SSLeft = prd.SSLeft;				// left spinscan
				m_SSLeftSplit = prd.SSLeftSplit;		// left split
				m_SSRightSplit = prd.SSRightSplit;		// right split.
				m_PulsePower = prd.PulsePower;
				m_DragFactor = prd.DragFactor;
				m_SS = prd.SS;
				//m_RawSpinScan = prd.RawSpinScan;

				m_Calories = prd.Calories;

				m_FrontGear = prd.FrontGear;
				m_RearGear = prd.RearGear;
				m_GearInches = prd.GearInches;

				m_Watts_Load = prd.Watts_Load;
				//m_LeftPower = prd.LeftPower;
				//m_RightPower = prd.RightPower;
				//m_PercentAT = prd.PercentAT;
				m_TSS = prd.TSS;
				m_IF = prd.IF;
				m_NP = prd.NP;

				// prd.CadenceTiming = pd.CadenceTiming;
				// Need to add drafting.
				// prd.Drafting = pd.Drafting;
				//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
				for (int i = 0; i < 24; i++)
				{
					m_Bars[i] = prd.Bars[i];
				}
				m_Unit.Statistics.OverrideDistance = prd.cur.Distance - StartDistance;
			}

			switch (Unit.State)
			{
				case Statistics.State.Stopped: m_Speed = 0.0f; break;
				case Statistics.State.Paused: return;	// Actually we don't need to do anything here.
			}
			base.Update();
		}

		protected double m_TimeAcc;
		//protected double m_Distance;
		//protected float m_Grade;
		//protected float m_Wind;
		//protected float m_Speed;
		protected float m_Watts;
		//protected float m_Watts_Load;
		protected float m_HeartRate;
		//protected float m_Cadence;
		protected float m_Calories;
		protected float m_PulsePower;
		protected float m_DragFactor;
		protected float m_SS;
		protected float m_SSLeft;
		protected float m_SSRight;
		protected float m_SSLeftSplit;
		protected float m_SSRightSplit;
		protected float m_SSLeftATA;
		protected float m_SSRightATA;
		//protected float m_PercentAT;
		protected int m_FrontGear;
		protected int m_RearGear;
		protected int m_GearInches;
		//protected float m_RawSpinScan;
		//protected float m_CadenceTiming;
		protected float m_TSS;
		protected float m_IF;
		protected float m_NP;
		//protected int m_Bars_Shown; // bool
		protected float[] m_Bars = new float[24]; // 24
		//protected float[] m_AverageBars; // 24
		//protected bool m_Drafting;

		public override float Speed { get { return m_Speed; } }
		public override float Cadence { get { return m_Cadence; } }
		public override float HeartRate { get { return m_HeartRate; } }
		public override float Watts { get { return m_Watts; } }
		public override float SS { get { return m_SS; } }
		public override float SSLeft { get { return m_SSLeft; } }
		public override float SSRight { get { return m_SSRight; } }
		public override float SSLeftSplit { get { return m_SSLeftSplit; } }
		public override float SSRightSplit { get { return m_SSRightSplit; } }
		public override float Calories { get { return m_Calories; } }
		public override float PulsePower { get { return m_PulsePower; } }
		public override float NP { get { return m_NP; } }
		public override float IF { get { return m_IF; } }
		public override float TSS { get { return m_TSS; } }
		public override float[] Bars { get { return m_Bars; } }
		public override int FrontGear { get { return m_FrontGear; } }	// Velotron only -1 if not valid
		public override int RearGear { get { return m_RearGear; } }	// Velotron only -1 if not valie
		public override int GearInches { get { return m_GearInches; } } // Velotron only - We should be able to caluclate this for non-computrainers.
		public override float Grade { get { return m_Grade; } set { } }
		public override float Watts_Load { get { return m_Watts_Load; } set { } }
		public override float Wind { get { return m_Wind; } set { } }
		//public override bool Drafting { get { return false; /* m_Drafting; */ } set { } }
		//public override double Distance { get { return m_Distance; } }
		//public override float Grade { get { return m_Grade; } }
		//public override float Wind { get { return m_Wind; } }
		//public override float SSLeftATA { get { return m_SSLeftATA; } }
		//public override float SSRightATA { get { return m_SSRightATA; } }
	}



	public class WattsBot : Bot
	{
		int m_Watts;
		public WattsBot(int watts)
		{
			BotWatts = watts;
			Info = gInfo;
		}

		public int BotWatts
		{
			get { return m_Watts; }
			set
			{
				int v = value < 1 ? 1 : value > 1000 ? 1000 : value;
				if (m_Watts != v)
				{
					m_Watts = v;
					BaseName = String.Format("Pacer, {0} watts", m_Watts);
					Key = String.Format("WattsBot,{0}", m_Watts);
					m_DisplayName = String.Format("Pacer {0}w", m_Watts);
					DoKeyChangeEvent();
				}
			}
		}

		String m_DisplayName;
		public override String DisplayName { get { return m_DisplayName; } } 


		public override void Adjust(int dir)
		{
			BotWatts += dir * 10;
		}


		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}

		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}

		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_WattsBot dlg = new Dialogs.Edit_WattsBot();
			dlg.Bot = this;
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
		}

		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss.Length < 1 || ss[0] != "WattsBot")
					return null;
				try
				{
					bot = new WattsBot(ss.Length < 2 ? 150 : (int)Convert.ToInt32(ss[1]));
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Fixed Watts ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				tbi.Add("(1 to 1K watts)");
				return tb;
			}
			public String DefaultKey { get { return "WattsBot"; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();

		//=============================
		// Stats overrides
		public override float Watts { get { return BotWatts; } }
		//=============================
		
		float m_Weight = 201;
		
		#pragma warning disable 414
		float m_Height = (6 * 12 + 1);
		#pragma warning restore 414


		protected override void Update()
		{
			/*
			if (m_Unit.Rider != m_CurRider)
			{
				m_CurRider = m_Unit.Rider;
				if (m_CurRider != null)
				{
					m_Weight = 201; //  m_CurRider.WeightRiderLBS + m_CurRider.WeightBikeLBS;
					m_Height = (6 * 12 + 1);
				}
				else
				{
					m_Weight = 201;
					m_Height = (6 * 12 + 1);
				}
			}
			 */

			float destspeed = WattsToSpeed(m_Watts);					// this is mph / 160
			if (destspeed < 0.0f)
				destspeed = 0.0f;
			switch(Unit.State)
			{
				case Statistics.State.Stopped: destspeed = 0.0f; break;
				case Statistics.State.Paused: return;	// Actually we don't need to do anything here.
			}

			// Accelerate the bike by x 
			m_MPH = destspeed;
			float ts = m_MPH * 20.0f;
			m_Speed = (float)(m_MPH * ConvertConst.MPHToMetersPerSecond);
			m_Cadence = 30;
			base.Update();
		}

		float m_MPH = 0.0f;

		float m_drag_aerodynamic = 8.0f;
		float m_drag_tire = 0.006f;

		protected float WattsToSpeed( float watts )
		{
			float k_thrust = watts * 375 / 746;
			float k_accel = ms_SplitTime * 22.0f;
			float k_winddrag = m_drag_aerodynamic / 900.0f;
			float grade_drag = (Grade * 0.01f + m_drag_tire) * m_Weight;


			//Jim: we need the rf_drag from our opponent's computrainer. Is this it?
			// It sets the minimum drag the pacer sees when going slow or downhill.

			//	minimum_drag = (float)data[RED][RF_DRAG].raw / 256.0f;

			// It might be cleaner to copy the computrainer's rfdrag to the pacer's first
			// and then use the pacer's rfdrag
			//data[BLUE][RF_DRAG].raw = data[RED][RF_DRAG].raw;
			//minimum_drag = (float)data[BLUE][RF_DRAG].raw / 256.0f;

			float minimum_drag = 0.0f;

			if (minimum_drag < 0.5f)  
			{
				minimum_drag = 2.0f;
			}

			if (m_Weight > 1.0)  
			{
				k_accel /= m_Weight;
			}

			float newspeed = m_MPH;
			int i;
			float ftemp;
			for (i=0; i<4; i++)  
			{
				if (newspeed < 1.0f)  
				{
					newspeed = 1.0f;
				}
				ftemp = (newspeed + 0.0f /*wind*/) * Math.Abs(newspeed + 0.0f /*ds->decoder->wind*/) * k_winddrag + grade_drag;

				if (ftemp < minimum_drag)  
				{
					ftemp = minimum_drag;
				}
				ftemp = (k_thrust / newspeed - ftemp) * k_accel;

				if (ftemp > 1.0f)  
				{
					ftemp = 1.0f;
				}

				if (ftemp < -1.0f)  
				{
					ftemp = -1.0f;
				}

				newspeed = m_MPH + ftemp;
			}

			if (newspeed < 0.0f) 
			{
				newspeed = 0.0f;
			}
			return newspeed;
		}
	}


	public class FixedSpeedBot : Bot
	{
		const String c_KeyName = "FixedSpeed";
		const float c_MinBestSpeed = 5;
		const float c_MinBestSpeedMetric = 8;
		const float c_MaxBestSpeed = 50;
		const float c_MaxBestSpeedMetric = 80;
		float m_MPHorKPH;
		float MPHorKPH
		{
			get { return m_MPHorKPH; }
			set
			{
				float min = RM1_Settings.General.Metric ? c_MinBestSpeedMetric : c_MinBestSpeed;
				float max = RM1_Settings.General.Metric ? c_MaxBestSpeedMetric : c_MaxBestSpeed;
				m_MPHorKPH = value < min ? min : value > max ? max : value;
			}
		}

		public override bool NeedsControlUnit { get { return true; } }

		bool m_Metric;
		double m_BestSpeed;
		FixedSpeedBot(double mph_or_kph, bool metric)
		{

			m_Metric = metric;
			MPHorKPH = (float)Math.Round(mph_or_kph,1);
			UpdateNames();
		}
		void UpdateNames()
		{
			if (m_Metric != RM1_Settings.General.Metric)
			{
				m_MPHorKPH = (float)Math.Round((m_MPHorKPH * (m_Metric ? ConvertConst.KilometersToMiles : ConvertConst.MilesToKilometers)), 1);
				m_Metric = RM1_Settings.General.Metric;
			}
			BaseName = String.Format("Pacer - Fixed speed, {0:0.#}{1}", m_MPHorKPH, m_Metric ? "kph" : "mph");
			Key = String.Format("{0},{1:0.#},{2}", c_KeyName,Math.Round(m_MPHorKPH,1), m_Metric ? "kph" : "mph");
			m_BestSpeed = m_MPHorKPH * ConvertConst.MPHOrKPHToMetersPerSecond;
			m_DisplayName = string.Format("Speed {0:0.#}{1}", m_MPHorKPH, m_Metric ? "kph" : "mph");
		}
		public override void Adjust(int dir)
		{
			m_MPHorKPH += dir * 0.1f;
			UpdateNames();
		}
		String m_DisplayName;
		public override String DisplayName { get { return m_DisplayName; } }

		protected double m_MaxAcc = ConvertConst.MPHToMetersPerSecond * 1.5; 

		protected override void Update()
		{
			float destspeed = Unit.State == Statistics.State.Stopped ? 0.0f:Unit.State == Statistics.State.Paused ? m_Speed:(float)m_BestSpeed;
			if (m_Speed > destspeed)
			{
				m_Speed -= (float)(ms_SplitTime * m_MaxAcc);
				if (m_Speed < destspeed)
					m_Speed = (float)destspeed;
			}
			else if (m_Speed < destspeed)
			{
				m_Speed += (float)(ms_SplitTime * m_MaxAcc);
				if (m_Speed > destspeed)
					m_Speed = (float)destspeed;
			}

			double t = m_Speed * ConvertConst.MetersPerSecondToMPH;
			if (t > 0)
			{
				t = 80 * Math.Sqrt(t / 10);
			}
			else
				t = 0;
			m_Cadence = (float)t;
			base.Update();
		}

		//================
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss.Length < 1 || ss[0] != c_KeyName)
					return null;
				try
				{
					bot = new FixedSpeedBot(ss.Length < 2 ? 15 : Convert.ToDouble(ss[1]),
							ss.Length < 3 ? false : String.Compare(ss[2], "kph") == 0);
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Fixed speed ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				float min = RM1_Settings.General.Metric ? c_MinBestSpeedMetric : c_MinBestSpeed;
				float max = RM1_Settings.General.Metric ? c_MaxBestSpeedMetric : c_MaxBestSpeed;
				tbi.Add(String.Format("({0} to {1} {2})", min, max, ConvertConst.TextMPHorKPH));
				return tb;
			}
			public String DefaultKey { get { return c_KeyName; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();

		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}

		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}


		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_Bot dlg = new Dialogs.Edit_Bot();
			int min = (int)(RM1_Settings.General.Metric ? c_MinBestSpeedMetric : c_MinBestSpeed);
			int max = (int)(RM1_Settings.General.Metric ? c_MaxBestSpeedMetric : c_MaxBestSpeed);

			dlg.Set(min, max, ConvertConst.TextMPHorKPH, m_MPHorKPH);
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
			m_MPHorKPH = dlg.Value;
			m_MPHorKPH = m_MPHorKPH < 1 ? 1 : m_MPHorKPH > c_MaxBestSpeed ? c_MaxBestSpeed : m_MPHorKPH;
			UpdateNames();
			DoKeyChangeEvent();
		}

	}



	//===========================================================
	/// <summary>
	/// When numbers are matched the bike will move into a position in front of the rider.  
	/// When faster moves behind when slower moves in front
	/// </summary>
	public class MatchBot : Bot
	{
		protected MatchBot()
		{
		}

		public override RiderModels Model 
		{ 
			get 
			{ 
				return m_OverrideModel != null ? m_OverrideModel.Model : 
					m_ControlUnit != null && m_ControlUnit.Rider != null && m_ControlUnit.Rider.Gender != "M" ?
					RiderModels.Gold_Female_Triathlon:RiderModels.Gold_Male_Triathlon; 
			} 
		}



		/// <summary>
		/// Distance to move away from the other rider.
		/// </summary>
		protected double m_Radius = 50.0;
		/// <summary>
		/// How many MPH can this go faster or slower to match the speed
		/// </summary>
		protected double m_MaxDiffinput = ConvertConst.MPHToMetersPerSecond * 10;

		/// <summary>
		/// Maximum MPH change per second.
		/// </summary>
		protected double m_MaxAcc = ConvertConst.MPHToMetersPerSecond * 1.5; 


		/// <summary>
		/// How much infront of the bike it should be at 0 speed.
		/// </summary>
		protected double m_InFront = 3.0;

		protected double m_DestPos;

		protected virtual void UpdateDisplayName() {}
		protected String m_DisplayName;
		public override String DisplayName { get { return m_DisplayName; } } 


		/// <summary>
		/// Normalized difference.
		/// </summary>
		protected double Diff
		{
			get { return m_Diff; }
			set
			{
				double v = value > 1.0 ? 1.0: value < -1.0 ? -1.0: value;
				m_Diff = v;
				// Figure out where
				m_RelativeDest = m_InFront + m_Radius * m_Diff;
			}
		}
		protected double m_Diff;
		protected double m_RelativeDest;

		public override bool NeedsControlUnit { get { return true; } }
		protected override void ControlUnitChanged()
		{
			UpdateDisplayName();
		}

		protected double m_MaxCatchupTime = 10;
		protected double m_MinCatchupSpeedDiff = ConvertConst.MPHToMetersPerSecond * 2;
		protected double m_MinSpeed = ConvertConst.MPHToMetersPerSecond * 5;
		protected override void Update()
		{
			double catchuptime;
			double destspeed;
			double orgdestspeed;
			if (m_ControlUnit == null)
			{
				orgdestspeed = destspeed = 0; // Control the speed down to zero.
			}
			else
			{
				double dif = (m_ControlUnit.Statistics.Distance + m_RelativeDest) - m_Unit.Statistics.Distance;
				orgdestspeed = destspeed = m_ControlUnit.Statistics.Speed; // Max distance
				if (dif > 0)
				{
					catchuptime = dif / (destspeed + m_MaxDiffinput); // At max speed how many seconds will it take.
					double tt = m_MaxDiffinput * (catchuptime > m_MaxCatchupTime ? 1 : (catchuptime / m_MaxCatchupTime));
					destspeed += tt < m_MinCatchupSpeedDiff ? m_MinCatchupSpeedDiff : tt;
				}
				else if (dif < 0)
				{
					// How long at the current speed is it going to take the other bike to catch up to it if this one is not moving.;
					if (destspeed > 0)
					{
						catchuptime = -dif / destspeed;
						destspeed = (catchuptime > m_MaxCatchupTime ? 0 : (m_MaxCatchupTime - catchuptime) / m_MaxCatchupTime) * destspeed;
					}
				}
			}
			// Now adjust the speed to the destspeed via accelleration deacceleration
			catchuptime = ms_SplitTime * m_MaxAcc;
			if (m_Speed > destspeed)
			{
				m_Speed -= (float)(ms_SplitTime * m_MaxAcc);
				if (m_Speed < destspeed)
					m_Speed = (float)destspeed;
			}
			else if (m_Speed < destspeed)
			{
				m_Speed += (float)(ms_SplitTime * m_MaxAcc);
				if (m_Speed > destspeed)
					m_Speed = (float)destspeed;
			}
			if (m_Speed < m_MinSpeed && m_Speed < orgdestspeed)
			{
				m_Speed = (float)(orgdestspeed < m_MinSpeed ? orgdestspeed : m_MinSpeed);
			}


			double t = m_Speed * ConvertConst.MetersPerSecondToMPH;
			if (t > 0)
			{
				t = 80 * Math.Sqrt(t / 10);
			}
			else
				t = 0;
			m_Cadence = (float)t;

			base.Update();
		}
	}


	/// <summary>
	/// Sets a destination speed.  If user is going that speed the bike moves into place.
	///   - If faster moves behind.
	///   - If slower will move ahead.
	/// </summary>
	public class SpeedBot : MatchBot
	{
		const float c_MinBestSpeed = 5;
		const float c_MinBestSpeedMetric = 8;
		const float c_MaxBestSpeed = 50;
		const float c_MaxBestSpeedMetric = 80;
		float m_MPHorKPH;
		float MPHorKPH
		{
			get { return m_MPHorKPH; }
			set
			{
				float min = RM1_Settings.General.Metric ? c_MinBestSpeedMetric : c_MinBestSpeed;
				float max = RM1_Settings.General.Metric ? c_MaxBestSpeedMetric : c_MaxBestSpeed;
				m_MPHorKPH = value < min ? min : value > max ? max : value;
			}
		}

		bool m_Metric;
		double m_BestSpeed;
		SpeedBot(double mph_or_kph, bool metric)
		{
			m_Metric = metric;
			MPHorKPH = (float)Math.Round(mph_or_kph,1);
			UpdateNames();
		}
		void UpdateNames()
		{
			if (m_Metric != RM1_Settings.General.Metric)
			{
				m_MPHorKPH = (float)Math.Round((m_MPHorKPH * (m_Metric ? ConvertConst.KilometersToMiles : ConvertConst.MilesToKilometers)),1);
				m_Metric = RM1_Settings.General.Metric;
			}
			BaseName = String.Format("Smart Pacer - Speed, {0:0.#}{1}", m_MPHorKPH, m_Metric ? "kph" : "mph");
			Key = String.Format("SpeedBot,{0:0.#},{1}", Math.Round(m_MPHorKPH,1), m_Metric ? "kph" : "mph");
			m_BestSpeed = m_MPHorKPH * ConvertConst.MPHOrKPHToMetersPerSecond;

			UpdateDisplayName();
		}

		protected override void UpdateDisplayName()
		{
			m_DisplayName = String.Format("SP {0:0.#}{1}", m_MPHorKPH, m_Metric ? "kph" : "mph");
			base.UpdateDisplayName();
		}

		public override void Adjust(int dir)
		{
			m_MPHorKPH += dir * 0.1f;
			UpdateNames();
		}


		protected override void Update()
		{
			if (m_ControlUnit == null)
				Diff = 0;
			else
			{
				double diff = m_BestSpeed - m_ControlUnit.Statistics.Speed;
				if (diff > ConvertConst.MPHToMetersPerSecond)
					diff -= ConvertConst.MPHToMetersPerSecond;
				else if (diff < -ConvertConst.MPHToMetersPerSecond)
					diff += ConvertConst.MPHToMetersPerSecond;
				else 
					diff = 0;
				Diff = diff / (10 * ConvertConst.MPHToMetersPerSecond);
			}
			base.Update();
		}

		//================
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss.Length < 1 || ss[0] != "SpeedBot")
					return null;
				try
				{
					bot = new SpeedBot(ss.Length < 2 ? 15 : Convert.ToDouble(ss[1]),
							ss.Length < 3 ? false : String.Compare(ss[2], "kph") == 0);
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Smart Pacer - Speed ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				float min = RM1_Settings.General.Metric ? c_MinBestSpeedMetric : c_MinBestSpeed;
				float max = RM1_Settings.General.Metric ? c_MaxBestSpeedMetric : c_MaxBestSpeed;
				tbi.Add(String.Format("({0} to {1} {2})", min, max, ConvertConst.TextMPHorKPH));
				return tb;
			}
			public String DefaultKey { get { return "SpeedBot"; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();

		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}

		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}


		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_Bot dlg = new Dialogs.Edit_Bot();
			int min = (int)(RM1_Settings.General.Metric ? c_MinBestSpeedMetric : c_MinBestSpeed);
			int max = (int)(RM1_Settings.General.Metric ? c_MaxBestSpeedMetric : c_MaxBestSpeed);

			dlg.Set(min, max, ConvertConst.TextMPHorKPH, m_MPHorKPH);
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
			m_MPHorKPH = dlg.Value;
			m_MPHorKPH = m_MPHorKPH < 1 ? 1 : m_MPHorKPH > c_MaxBestSpeed ? c_MaxBestSpeed : m_MPHorKPH;
			UpdateNames();
			DoKeyChangeEvent();
		}

	}

	public class StdWattsBot : MatchBot
	{
		const float c_MinBestWatts = 1;
		const float c_MaxBestWatts = 1000;
		float m_DWatts;
		float DWatts
		{
			get { return m_DWatts; }
			set
			{
				m_DWatts = value < c_MinBestWatts ? c_MinBestWatts: value > c_MaxBestWatts ? c_MaxBestWatts : value;
			}
		}
		StdWattsBot(int watts)
		{
			DWatts = watts;
			UpdateNames();
		}
		void UpdateNames()
		{
			BaseName = String.Format("Smart Pacer - Watts, {0}", m_DWatts );
			Key = String.Format("StdWattsBot,{0}", Math.Round(m_DWatts) );
			UpdateDisplayName();
		}

		protected override void UpdateDisplayName()
		{
			m_DisplayName = String.Format("SP {0:0.#}w", m_DWatts );
			base.UpdateDisplayName();
		}

		public override void Adjust(int dir)
		{
			m_DWatts += dir;
			UpdateNames();
		}

		public override float Watts { get { return m_DWatts; } }
		protected override void Update()
		{
			if (m_ControlUnit == null)
				Diff = 0;
			else
			{
				double diff = m_DWatts - m_ControlUnit.Statistics.Watts;
				if (diff > 2)
					diff -= 2;
				else if (diff < -2)
					diff += 2;
				else
					diff = 0;
				Diff = diff / 20;
			}
			base.Update();
		}

		//================
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss.Length < 1 || ss[0] != "StdWattsBot")
					return null;
				try
				{
					bot = new StdWattsBot(ss.Length < 2 ? 100 : (int)Convert.ToInt32(ss[1]));
							
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Smart Pacer - Watts ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				tbi.Add(String.Format("(1 to 1K Watts)"));
				return tb;
			}
			public String DefaultKey { get { return "StdWattsBot"; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();

		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}

		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}


		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_Bot dlg = new Dialogs.Edit_Bot();
			dlg.Set((int)c_MinBestWatts, (int)c_MaxBestWatts, "Watts", (int)m_DWatts, false);
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
			DWatts = dlg.Value;
			UpdateNames();
			DoKeyChangeEvent();
		}

	}

	public class HRBot : MatchBot
	{
		HRBot(int lower, int upper)
		{
			SetPercentages( lower, upper );
		}
		HRBot(int zone)
		{
			SetZone(zone);
		}
		const int c_MinPercentage = 50;
		const int c_DefaultMinGap = 5;
		const double c_OutOfRange = 5.0;
		public const string c_KeyName = "HeartrateBot";

		int m_MinGap = c_DefaultMinGap;

		int m_Zone;
		public int Zone { get { return m_Zone; } }

		int m_LowerPercent;
		int m_UpperPercent;
		public int LowerPercent { get { return m_LowerPercent; } }
		public int UpperPercent { get { return m_UpperPercent; } }

		float m_PercentV;
		public override float HeartRate { get { return (float)(m_ControlUnit == null || m_ControlUnit.Rider == null ? 0 : m_ControlUnit.Rider.PowerFTP * m_PercentV); } }
		
		void setPercentages(int lower, int upper)
		{
			if (lower > upper)
			{
				int t = lower;
				lower = upper;
				upper = t;
			}
			lower = lower < c_MinPercentage ? c_MinPercentage : lower > 100 - m_MinGap ? 100 - m_MinGap:lower;
			upper = upper < c_MinPercentage + m_MinGap ? c_MinPercentage + m_MinGap : upper > 100 ? 100:upper;
			m_PercentV = (float)((lower + upper) / 2.0);
			if (upper - lower < m_MinGap)
			{
				if (lower + m_MinGap > 100)
				{
					upper = 100;
					lower = 100 - m_MinGap;
				}
				else if (upper - m_MinGap < c_MinPercentage)
				{
					lower = c_MinPercentage;
					upper = lower + m_MinGap;
				}
				else
					upper = lower + m_MinGap;
			}
			m_LowerPercent = lower;
			m_UpperPercent = upper;
			UpdateNames();
			DoKeyChangeEvent();
		}

		public void SetPercentages(int lower, int upper)
		{
			m_Zone = 0;
			setPercentages(lower, upper);
		}
		public void SetZone(int zone)
		{
			m_Zone = zone < 1 ? 1 : zone > 5 ? 5 : zone;
			switch (zone)
			{
				case 1: setPercentages(50, 60); break;
				case 2: setPercentages(60, 70); break;
				case 3: setPercentages(70, 80); break;
				case 4: setPercentages(80, 90); break;
				case 5: setPercentages(90, 100); break;
			}
		}

		void UpdateNames()
		{
			BaseName = m_Zone != 0 ?
				String.Format("HR Pacer, Zone {0}", m_Zone) :
				String.Format("HR Pacer, {0}%-{1}%", m_LowerPercent, m_UpperPercent);
			Key = m_Zone != 0 ?
				String.Format("{0},Zone,{1}", c_KeyName,m_Zone) :
				String.Format("{0},{1},{2}", c_KeyName,m_LowerPercent, m_UpperPercent);
			UpdateDisplayName();
		}

		public override object DisplayText
		{
			get
			{
				if (m_ControlUnit == null || m_ControlUnit.Rider == null)
					return Name;
				Rider rider = m_ControlUnit.Rider;
				String hrz;
				switch(m_Zone)
				{
					case 1: hrz = rider.Zone1;break;
					case 2: hrz = rider.Zone2;break;
					case 3: hrz = rider.Zone3;break;
					case 4: hrz = rider.Zone4;break;
					case 5: hrz = rider.Zone5;break;
					default:
						hrz = rider.HeartRateText(m_LowerPercent,m_UpperPercent);
						break;
				}
				return Name + ", "+hrz;
			}
		}



		protected override void UpdateDisplayName()
		{
			m_DisplayName = m_Zone != 0 ?
				String.Format("HR Zone {0}", m_Zone) :
				String.Format("HR {0}%-{1}%", m_LowerPercent, m_UpperPercent);
			base.UpdateDisplayName();
		}
		public override void Adjust(int dir)
		{
			if (m_Zone == 0)
			{
				m_MinGap = m_UpperPercent - m_LowerPercent;
				SetPercentages(m_LowerPercent + dir * m_MinGap, m_UpperPercent + dir * m_MinGap);
				m_MinGap = c_DefaultMinGap;
			}
			else
				SetZone( m_Zone + dir );
		}

		protected override void Update()
		{
			if (m_ControlUnit == null)
				Diff = 0;
			else
			{
				double hr = m_ControlUnit.Rider != null ? m_ControlUnit.Rider.HeartRatePercentage(m_ControlUnit.Statistics.HeartRate) : 75.0;
				if (hr >= m_LowerPercent && hr <= m_UpperPercent)
					Diff = 0;
				else if (hr < m_LowerPercent)
					Diff = (double)(m_LowerPercent - hr) / c_OutOfRange;
				else
					Diff = (double)(m_UpperPercent - hr) / c_OutOfRange;
			}
			base.Update();
		}

		//================
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss[0] != c_KeyName)
					return null;
				try
				{
					if (ss.Length < 3)
						bot = new HRBot(3);
					else if (ss[1] == "Zone")
						bot = new HRBot((int)Convert.ToInt32(ss[2]));
					else
						bot = new HRBot((int)Convert.ToInt32(ss[1]), (int)Convert.ToInt32(ss[2]));
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Smart Pacer - Heartrate ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				tbi.Add(String.Format("(Zone or Percentage)"));
				return tb;
			}
			public String DefaultKey { get { return c_KeyName; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();

		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}
		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}


		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_HRBot dlg = new Dialogs.Edit_HRBot();
			dlg.Bot = this;
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
			DoKeyChangeEvent();
		}


	}

	public class AIBot : MatchBot
	{
		public const String c_KeyName = "AIPacer";
		[Flags]
		public enum AIFlags : uint
		{
			Zero = 0,
			Frequent = (1 << 0),
			Agressive = (1 << 1)
		};

		AIBot(String frequency, String strength )
		{
			SetAttack( frequency, strength );
		}

		String m_Frequency;
		String m_Strength;

		public String Frequency { get { return m_Frequency; } }
		public String Strength { get { return m_Strength; } }

		String m_FrequencyAbbr;
		String m_StrengthAbbr;

		AIFlags m_AIFlags;


		public void SetAttack(String frequency, String strength)
		{
			AIFlags ai = AIFlags.Zero;
			if (String.Compare(frequency, "Frequent", true) == 0)
			{
				m_Frequency = "Frequent";
				ai |= AIFlags.Frequent;
				m_FrequencyAbbr = "Fq";
			}
			else
			{
				m_Frequency = "Infrequent";
				m_FrequencyAbbr = "InFq";
			}
			if (String.Compare(strength, "Agressive", true) == 0)
			{
				m_Strength = "Agressive";
				ai |= AIFlags.Agressive;
				m_StrengthAbbr = "Agr";
			}
			else
			{
				m_Strength = "Gental";
				m_StrengthAbbr = "Gntl";
			}
			m_AIFlags = ai;
			UpdateNames();
			DoKeyChangeEvent();
		}

		void UpdateNames()
		{
			BaseName = String.Format("AI Pacer, {0}/{1}", m_Frequency,m_Strength);
			Key = String.Format("{0},{1},{2}", c_KeyName, m_Frequency,m_Strength);
			UpdateDisplayName();
		}

		protected override void UpdateDisplayName()
		{
			m_DisplayName = String.Format("AI {0}/{1}", m_FrequencyAbbr, m_StrengthAbbr);
			base.UpdateDisplayName();
		}


		public override void Adjust(int dir)
		{
			int t = (int)m_AIFlags;
			t += dir;
			t = t < 0 ? 3 : t > 3 ? 0 : t;
			m_AIFlags = (AIFlags)t;
			SetAttack((m_AIFlags & AIFlags.Frequent) != AIFlags.Zero ? "Frequent" : "Infrequent",
					   (m_AIFlags & AIFlags.Agressive) != AIFlags.Zero ? "Agressive" : "General");
		}
		protected override void Update()
		{
			Diff = 0;
			base.Update();
		}

		//================
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss.Length < 1 || ss[0] != c_KeyName)
					return null;
				if (ss.Length < 3)
					bot = new AIBot("Infrequent", "Agressive");
				else
					bot = new AIBot(ss[1], ss[2]);
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("AI Pacer");
				return tb;
			}
			public String DefaultKey { get { return c_KeyName; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();

		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}
		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}
		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_AIBot dlg = new Dialogs.Edit_AIBot();
			dlg.Bot = this;
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
			DoKeyChangeEvent();
		}


	}

	//===========================================================
	public class PercentBot : MatchBot
	{
		protected PercentBot()
		{
		}

		protected virtual void UpdateNames()
		{
			DoKeyChangeEvent();
		}


		protected int m_AdjustStep = 5;
		protected int m_MinPercent = 50;
		protected int m_MaxPercent = 100;

		public int MinPercent { get { return m_MinPercent; } }
		public int MaxPercent { get { return m_MaxPercent; } }

		protected double m_PercentV;
		protected int m_Percent;
		public virtual int Percent
		{
			get { return m_Percent; }
			set
			{
				m_Percent = value < m_MinPercent ? m_MinPercent : value > m_MaxPercent ? m_MaxPercent : value;
				m_PercentV = m_Percent / 100.0;
				UpdateNames();
			}
		}
		public override void Adjust(int dir)
		{
			Percent += dir * m_AdjustStep;
		}

		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_PercentBot dlg = new Dialogs.Edit_PercentBot();
			dlg.Bot = this;
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
		}

	}

	public class FTPBot : PercentBot
	{
		public const string c_KeyName = "FTPBot";
		FTPBot( int percent )
		{
			m_MaxPercent = 200;
			Percent = (int)(Math.Round(percent / 5.0) * 5);
		}
		protected override void UpdateNames()
		{
			BaseName = String.Format("FTP {0}%", Percent);
			Key = String.Format("{0},{1}", c_KeyName, Percent);
			UpdateDisplayName();
			base.UpdateNames();
		}
		protected override void UpdateDisplayName()
		{
			m_DisplayName = String.Format("FTP {0}%,{1}w", Percent,Watts);
			base.UpdateDisplayName();
		}

		public override object DisplayText
		{
			get
			{
				return String.Format("FTP {0}%,{1} Watts", Percent, Watts);
			}
		}

		protected override void Update()
		{
			Diff = m_ControlUnit == null || m_ControlUnit.Rider == null ? 0:(m_ControlUnit.Rider.PowerFTP * m_PercentV - m_ControlUnit.Statistics.Watts) / 20;
			base.Update();
		}

		public override float Watts { get { return (float)(m_ControlUnit == null || m_ControlUnit.Rider == null ? 0 : m_ControlUnit.Rider.PowerFTP * m_PercentV); } }

		//--------
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss[0] != c_KeyName)
					return null;
				try
				{
					if (ss.Length < 2)
						bot = new FTPBot(75);
					else 
						bot = new FTPBot((int)Convert.ToInt32(ss[1]));
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Smart Pacer - FTP ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				tbi.Add(String.Format("(50% to 200%)"));
				return tb;
			}
			public String DefaultKey { get { return c_KeyName; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();
	}

	public class AnTBot : PercentBot
	{
		public const string c_KeyName = "AnTBot";
		AnTBot(int percent)
		{
			m_MaxPercent = 150;
			Percent = (int)(Math.Round(percent / 5.0) * 5);
		}
		protected override void UpdateNames()
		{
			BaseName = String.Format("AnT {0}%", Percent);
			Key = String.Format("{0},{1}", c_KeyName, Percent);
			UpdateDisplayName();
			base.UpdateNames();
		}
		protected override void UpdateDisplayName()
		{
			m_DisplayName = String.Format("AnT {0}%,{1}w", Percent, Watts);
			base.UpdateDisplayName();
		}

		public override object DisplayText
		{
			get
			{
				return String.Format("AnT {0}%,{1} Watts", Percent, Watts);
			}
		}

		protected override void Update()
		{
			Diff = m_ControlUnit == null || m_ControlUnit.Rider == null ? 0 : (m_ControlUnit.Rider.PowerAnT * m_PercentV - m_ControlUnit.Statistics.Watts) / 20;
			base.Update();
		}

		public override float Watts { get { return (float)(m_ControlUnit == null || m_ControlUnit.Rider == null ? 0 : m_ControlUnit.Rider.PowerAnT * m_PercentV); }}


		//--------
		class BotInfo : IBotInfo
		{
			public Bot Create(String key)
			{
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss[0] != c_KeyName)
					return null;
				try
				{
					if (ss.Length < 2)
						bot = new AnTBot(100);
					else
						bot = new AnTBot((int)Convert.ToInt32(ss[1]));
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Smart Pacer - AnT ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				tbi.Add(String.Format("(50% to 150%)"));
				return tb;
			}
			public String DefaultKey { get { return c_KeyName; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();

	}


	public class DelayBot : MatchBot  {
		const int c_MinSeconds = 1;
		const int c_MaxSeconds = 6;
		int m_Delay;
		double old_rider_mps = 0.0;

		public int Delay  {
			get {
				return m_Delay;
			}
			set  {
				m_Delay = value < c_MinSeconds ? c_MinSeconds: value > c_MaxSeconds ? c_MaxSeconds : value;
				m_MaxCatchupTime = m_Delay;
			}
		}

		DelayBot(int delay)  {
			m_MaxAcc = ConvertConst.MPHToMetersPerSecond * 10;
			m_MaxDiffinput = ConvertConst.MPHToMetersPerSecond * 20;
			Delay = delay;
			UpdateNames();
		}

		void UpdateNames()  {
			BaseName = String.Format("Smart Pacer - Delay, {0}s", m_Delay);
			Key = String.Format("DelayBot,{0}", m_Delay);
			UpdateDisplayName();
		}

		protected override void UpdateDisplayName()  {
			m_DisplayName = String.Format("Delay {0}s", m_Delay );
			base.UpdateDisplayName();
		}

		public override void Adjust(int dir)  {
			Delay += dir;
			UpdateNames();
		}

		StatFlags m_neededFlags = StatFlags.Speed | StatFlags.Cadence | StatFlags.Grade | StatFlags.Watts_Load;
		Performance m_workPerformance = new Performance();
		double m_Time = 0.0;
		const double c_MinRange = ConvertConst.MPHToMetersPerSecond * -1;
		const double c_MaxRange = ConvertConst.MPHToMetersPerSecond * 1;

		// tlm+++
		//float a = 0.0f;							// experimenting
		//float speed = 0.0f;						// experimenting
		// tlm---


		/*****************************************************************
			xxx
			copy saved in bot1.cs
			called every 33 ms = 30 hz
		*****************************************************************/


		protected override void Update()  {
			if (m_ControlUnit == null)  {
				Diff = 0;				// sets m_RelativeDest
				return;
			}

#if SAVE_IN_RAM
//#if xSAVE_IN_RAM
				RM1.IStats t = m_ControlUnit.Statistics.PerfContainer.GetRunningFrame(ref m_workPerformance, m_ControlUnit.Statistics.Time - m_Delay, m_neededFlags);
				if (t == null) {
					// Until we get t values we need to move this to zero.
					m_Diff = 0;
					m_RelativeDest = 0.0;
					base.Update();	// Just deal with the bot.
					return;
				}

				double diff = t.Speed - m_ControlUnit.Statistics.Speed;
				double distdif = (m_ControlUnit.Statistics.Distance + m_RelativeDest) - m_Unit.Statistics.Distance;

				if (diff < c_MinRange || diff > c_MaxRange) {
					m_Time = m_Delay;
					m_Speed = t.Speed; // We are done... the bot takes over.
					return;
				}

				m_Time -= ms_SplitTime;

				if (m_Time > 0) {
					m_Speed = t.Speed; // We are done... the bot takes over.
					return;
				}

				Diff = diff / (10 * ConvertConst.MPHToMetersPerSecond);
				base.Update();	// Bot acks like a smart pacer... when at the same speed.
#else

			float rider_mps = (float)m_ControlUnit.Statistics.Speed;						// in meters per second

			if (m_ControlUnit.Statistics.Distance < 0.000001f) {
				m_Diff = 0.0;
				//m_RelativeDest = 0.0;
				old_rider_mps = rider_mps;
				base.Update();					// Just deal with the bot
				return;
			}


#if DEBUG
			int bp = 0;
			float fmph = rider_mps * (float)ConvertConst.MetersPerSecondToMPH;
			float fmeters = (float)m_Unit.Statistics.Distance;								// in meters
			float ffeet = fmeters * (float)ConvertConst.MetersToFeet;
			float fmiles = fmeters * (float)ConvertConst.MetersToMiles;
#endif
			
			if (m_Delay <= 0) {
				m_Speed = rider_mps;
				return;
		}

			old_rider_mps += (rider_mps - old_rider_mps) / (m_Delay * 30.0f * 0.66f);					// old_speed = low pass filtered (current rider speed)
			double mps_diff = old_rider_mps - rider_mps;															// old rider speed - current rider speed (in meters per second)

			if (mps_diff < c_MinRange || mps_diff > c_MaxRange) {							// < 1 mph or > 1 mph?
				m_Time = m_Delay;
				m_Speed = (float)old_rider_mps;						// We are done... the bot takes over.
				return;
			}

			m_Time -= ms_SplitTime;

			if (m_Time > 0) {
				m_Speed = (float)old_rider_mps;							// We are done... the bot takes over.
				return;
			}

			Diff = mps_diff / (10 * ConvertConst.MPHToMetersPerSecond);				// normalized difference, sets m_RelativeDest

			base.Update();																			// call matchbot, "Bot acks like a smart pacer... when at the same speed"
#endif

		}										// Update()


		/*****************************************************************

		*****************************************************************/

		class BotInfo : IBotInfo  {
			public Bot Create(String key)  {
				String[] ss = Bot.PreParse(key);
				Bot bot = null;
				if (ss.Length < 1 || ss[0] != "DelayBot")
					return null;
				try
				{
					bot = new DelayBot(ss.Length < 2 ? 3 : (int)Convert.ToInt32(ss[1]));
							
				}
				catch { }
				return bot;
			}
			public TextBlock Info()
			{
				TextBlock tb = new TextBlock();
				TextBlock t = new TextBlock();
				InlineCollection tbi = tb.Inlines;
				tbi.Add("Smart Pacer - Delay ");
				t = new TextBlock();
				t.Foreground = Brushes.Gray;
				tbi.Add("(1 to 6 seconds)");
				return tb;
			}
			public String DefaultKey { get { return "DelayBot"; } }
		}
		public static readonly IBotInfo gInfo = new BotInfo();

		public override UIElement EditArea()
		{
			Controls.PlainButton pb = new Controls.PlainButton();
			pb.Text = "Edit";
			pb.Padding = new Thickness(0, 0, 0, 0);
			pb.FontSize = 16;
			pb.VerticalContentAlignment = VerticalAlignment.Center;
			pb.HorizontalAlignment = HorizontalAlignment.Center;
			pb.Width = 62;
			pb.Click += new RoutedEventHandler(pb_Click);
			return pb;
		}

		void pb_Click(object sender, RoutedEventArgs e)
		{
			Edit((UIElement)sender);
		}


		public override void Edit(UIElement calling_control)
		{
			Dialogs.Edit_DelayBot dlg = new Dialogs.Edit_DelayBot();
			dlg.Bot = this;
			dlg.Owner = AppWin.Instance;
			dlg.ShowDialog(); //shows as modal
			UpdateNames();
			DoKeyChangeEvent();
		}

	}
}
