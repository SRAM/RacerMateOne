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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for StatsLineSmall.xaml
	/// </summary>
	public partial class StatsLineSmall : UserControl
	{
		static Brush ms_LineDefault = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88FFFFFF"));
		public static DependencyProperty LineProperty = DependencyProperty.Register("Line", typeof(Brush), typeof(StatsLineSmall),
				new FrameworkPropertyMetadata(ms_LineDefault));
		public Brush Line
		{
			get { return (Brush)this.GetValue(LineProperty); }
			set { this.SetValue(LineProperty, value); }
		}

		//======================================================================
		public static DependencyProperty StatFlagsProperty = DependencyProperty.Register("StatFlags", typeof(StatFlags), typeof(StatsLineSmall),
			new FrameworkPropertyMetadata(StatFlags.Zero, new PropertyChangedCallback(_StatFlagsChanged)));
		public StatFlags StatFlags
		{
			get { return (StatFlags)this.GetValue(StatFlagsProperty); }
			set { this.SetValue(StatFlagsProperty, value); }
		}
		private static void _StatFlagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((StatsLineSmall)d).OnStatFlagsChanged();
		}
		protected StatFlags m_StatFlags;
		void OnStatFlagsChanged()
		{
			StatFlags flags = StatFlags | StatFlags.Disconnected;
			if (m_StatFlags == flags)
				return;
			if (!Unit.AnyVelotron && !AppWin.IsInDesignMode)
				flags &= ~StatFlags.VelotronOnly;
			if ((flags & StatFlags.Lead) != StatFlags.Zero)
				flags |= StatFlags.Drafting;
			if ((flags & StatFlags.HeartRate) != StatFlags.Zero)
				flags |= StatFlags.HeartRateAlarm;

			ShutdownUpdate();
			m_StatFlags = flags;
			RedoControl();
			RedoDivFlags();
			StartUpdate();
		}
		//======================================================================
		public static DependencyProperty DivFlagsProperty = DependencyProperty.Register("DivFlags", typeof(StatFlags), typeof(StatsLineSmall),
			new FrameworkPropertyMetadata(
				StatFlags.Speed_Max | StatFlags.Lead | StatFlags.TSS_IF_NP | StatFlags.Cadence_Max | StatFlags.PulsePower | StatFlags.Gearing,
				new PropertyChangedCallback(_DivFlagsChanged)));
		public StatFlags DivFlags
		{
			get { return (StatFlags)this.GetValue(DivFlagsProperty); }
			set { this.SetValue(DivFlagsProperty, value); }
		}
		private static void _DivFlagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((StatsLineSmall)d).RedoDivFlags();
		}
		//======================================================================
		public static DependencyProperty LinesProperty = DependencyProperty.Register("Lines", typeof(int), typeof(StatsLineSmall),
			new FrameworkPropertyMetadata(1, new PropertyChangedCallback(_LineChanged)));
		public int Lines
		{
			get { return (int)this.GetValue(LinesProperty); }
			set { this.SetValue(LinesProperty, value); }
		}
		private static void _LineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((StatsLineSmall)d).OnLineChanged();
		}
		void OnLineChanged()
		{
			int lines = Lines;
			if (lines == m_Units.Length)
				return;
			ShutdownUpdate();
			Array.Resize<Unit>(ref m_Units, lines);
			StartUpdate();
		}
		//======================================================================
		public Unit Unit
		{
			get { return m_Units.Length > 0 ? m_Units[0] : null; }
			set
			{
				if (m_Units.Length <= 0)
					return;
				SetUnit(0, value);
			}
		}
		//======================================================================
		class ItemNode
		{
			public static bool ms_bDisconnected;
			public StackPanel Panel;
			public StatFlags Flag;
			public StatFlags ShowFlag;
			public String FormatString;
			public List<Label> LabelList = new List<Label>();
			public Line Div;
			public ItemNode(StackPanel panel, StatFlags flag, String fstring)
			{
				Panel = panel;
				ShowFlag = Flag = flag;
				FormatString = fstring;
			}
			public virtual void Update(Label lab, Unit unit)
			{
				lab.Content = ms_bDisconnected ? "-" : String.Format(FormatString, unit.Statistics.GetFromFlagDisplay(Flag));
			}
		}
		class ItemNode_RiderName : ItemNode
		{
			public ItemNode_RiderName(StackPanel panel):base(panel, StatFlags.RiderName | StatFlags.Drafting, "{0}")
			{
				ShowFlag = StatFlags.RiderName;
			}
			public override void Update(Label lab, Unit unit)
			{
				TextBlock tb = new TextBlock();
				tb.Width = 100;
				tb.TextTrimming = TextTrimming.CharacterEllipsis;
				tb.Text = unit.Statistics.RiderName;
				tb.Foreground = Brushes.Red; // unit.Statistics.Drafting ? Brushes.Blue : Brushes.Red;
				lab.Content =  tb;
			}
		}
		class ItemNode_Gearing : ItemNode
		{
			public ItemNode_Gearing(StackPanel panel)
				: base(panel, StatFlags.Gearing, "{0}")
			{
			}
			public override void Update(Label lab, Unit unit)
			{
				lab.Content = ms_bDisconnected ? "-":String.Format("{0}/{1}", unit.Statistics.FrontGear, unit.Statistics.RearGear);
			}
		}
		class ItemNode_Distance : ItemNode
		{
			public ItemNode_Distance(StackPanel panel)
				: base(panel, StatFlags.Distance, "{0}")
			{
			}
			public override void Update(Label lab, Unit unit)
			{
				lab.Content = ms_bDisconnected ? "-" : unit.Statistics.DistanceDisplayString;
			}
		}
		class ItemNode_Lead : ItemNode
		{
			public ItemNode_Lead(StackPanel panel)
				: base(panel, StatFlags.Lead | StatFlags.Drafting, "{0}")
			{
			}
			public override void Update(Label lab, Unit unit)
			{
				lab.Foreground = unit.Statistics.Drafting ? AppWin.StdBrush_Drafting : Brushes.White;
				lab.Content = ms_bDisconnected ? "-" : unit.Statistics.DistanceLeadString;
			}
		}
		class ItemNode_HeartRate : ItemNode
		{
			public StatsLineSmall StatsLine;
			public ItemNode_HeartRate(StackPanel panel, StatsLineSmall line)
				: base(panel, StatFlags.HeartRate | StatFlags.HeartRateAlarm, "{0}")
			{
				StatsLine = line;
			}
			public override void Update(Label lab, Unit unit)
			{
				float hr = unit.Statistics.HeartRate;
				lab.Foreground = unit.Statistics.HeartRateFlash && StatsLine.ShowHeartRateAlarm ? Brushes.DarkRed : Brushes.Red;
				lab.Content = hr <= 0 ? "-" : hr.ToString();
			}
		}
		class ItemNode_TSS : ItemNode
		{
			public ItemNode_TSS(StackPanel panel)
				: base(panel, StatFlags.TSS_IF_NP, "{0}")
			{
			}
			public override void Update(Label lab, Unit unit)
			{
				lab.Content = ms_bDisconnected ? "-" : String.Format("{0:0.#}/{1:0.#}/{2:0.#}", unit.Statistics.TSS, unit.Statistics.IF, unit.Statistics.NP);
			}
		}
		class ItemNode_Calories : ItemNode
		{
			public ItemNode_Calories(StackPanel panel)
				: base(panel, StatFlags.Calories, "{0}")
			{
			}
			public override void Update(Label lab, Unit unit)
			{
				lab.Content = ms_bDisconnected ? "-" : unit.Statistics.CaloriesString;
			}
		}


		List<ItemNode> m_ItemList = new List<ItemNode>();
		List<ItemNode> m_ItemActive = new List<ItemNode>();

		public bool ShowHeartRateAlarm = true;

		Unit[] m_Units = new Unit[1];

		StatFlags m_Force = StatFlags.Zero; // Next time through Show force an update of these fields.

		public void SetUnit(int line, Unit unit)
		{
			if (line >= m_Units.Length || line < 0)
				return;
			if (m_Units[line] == unit)
				return;
			ShutdownUpdate();
			m_Units[line] = unit;
			StartUpdate();
			m_Force = StatFlags;
		}



		bool m_bInit;
		public StatsLineSmall()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (RM1_Settings.General.Metric)
			{
				((Label)(Speed.Children[0])).Content = "KPH";
			}

			m_ItemList.Add(new ItemNode_RiderName(RiderName));
			m_ItemList.Add(new ItemNode(Speed, StatFlags.Speed, "{0:0.0}"));
			m_ItemList.Add(new ItemNode(Speed_Avg, StatFlags.Speed_Avg, "{0:0.0}"));
			m_ItemList.Add(new ItemNode(Speed_Max, StatFlags.Speed_Max, "{0:0.0}"));

			m_ItemList.Add(new ItemNode(Watts, StatFlags.Watts, "{0:F0}"));
			m_ItemList.Add(new ItemNode(Watts_Avg, StatFlags.Watts_Avg, "{0:F0}"));
			m_ItemList.Add(new ItemNode(Watts_Max, StatFlags.Watts_Max, "{0:F0}"));
			m_ItemList.Add(new ItemNode(Watts_Wkg, StatFlags.Watts_Wkg, "{0:0.0}"));
			m_ItemList.Add(new ItemNode(DragFactor, StatFlags.DragFactor, "{0:F0}"));
			m_ItemList.Add(new ItemNode_TSS(TSS));

			m_ItemList.Add(new ItemNode_HeartRate(HeartRate,this));
			m_ItemList.Add(new ItemNode(HeartRate_Avg, StatFlags.HeartRate_Avg, "{0:F0}"));
			m_ItemList.Add(new ItemNode(HeartRate_Max, StatFlags.HeartRate_Max, "{0:F0}"));

			m_ItemList.Add(new ItemNode(Cadence, StatFlags.Cadence, "{0:F0}"));
			m_ItemList.Add(new ItemNode(Cadence_Avg, StatFlags.Cadence_Avg, "{0:F0}"));
			m_ItemList.Add(new ItemNode(Cadence_Max, StatFlags.Cadence_Max, "{0:F0}"));

			m_ItemList.Add(new ItemNode_Distance(Distance));
			m_ItemList.Add(new ItemNode_Calories(Calories));
			m_ItemList.Add(new ItemNode_Lead(Lead));
			m_ItemList.Add(new ItemNode(PulsePower, StatFlags.PulsePower, "{0:F0}"));
			m_ItemList.Add(new ItemNode(Grade, StatFlags.Grade, "{0:0.0}"));
			m_ItemList.Add(new ItemNode(Wind, StatFlags.Wind, "{0:F0}"));

			m_ItemList.Add(new ItemNode(GearInches, StatFlags.GearInches, "{0}"));
			m_ItemList.Add(new ItemNode_Gearing(Gearing));


			m_bInit = true;
			RedoControl();
			RedoDivFlags();
			StartUpdate();
		}


		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			ShutdownUpdate();
		}

		StatFlags m_CurUpdateFlags;
		void ShutdownUpdate()
		{
			if (!m_bInit || m_CurUpdateFlags == StatFlags.Zero)
				return;
			foreach (Unit unit in m_Units)
			{
				if (unit != null)
					Unit.RemoveNotify(unit, m_CurUpdateFlags, UpdateLine);
			}
			m_CurUpdateFlags = StatFlags.Zero;
		}

		void StartUpdate()
		{
			if (!m_bInit || m_CurUpdateFlags == m_StatFlags)
				return;

			if (m_CurUpdateFlags != StatFlags.Zero)
				ShutdownUpdate();

			m_CurUpdateFlags = m_StatFlags;
			if (m_CurUpdateFlags == StatFlags.Zero)
				return;
			foreach (Unit unit in m_Units)
			{
				if (unit != null)
				{
					Unit.AddNotify(unit, m_CurUpdateFlags, UpdateLine);
					UpdateLine(unit, m_StatFlags);
				}
			}
		}


		private void RedoControl()
		{
			if (!m_bInit)
				return;
			int lines = Lines + 1;
			m_ItemActive.Clear();
			StatFlags f = m_StatFlags;
			Label label;
			foreach (ItemNode n in m_ItemList)
			{
				if ((f & n.ShowFlag) != StatFlags.Zero)
				{
					m_ItemActive.Add(n);
					if (n.Panel.Children.Count != lines)
					{
						while (n.Panel.Children.Count < lines)
						{
							label = new Label();
							label.Content = "xx";
							Label header = (Label)(n.Panel.Children[0]);
							label.Width = header.Width;
							if ((n.ShowFlag & (StatFlags.HeartRate | StatFlags.RiderName)) != StatFlags.Zero)
								label.Foreground = Brushes.Red;
							n.LabelList.Add(label);
							n.Panel.Children.Add(label);
						}
						while (n.Panel.Children.Count > lines)
						{
							label = n.LabelList.Last();
							n.LabelList.Remove(label);
							n.Panel.Children.Remove(label);
						}
					}
					n.Panel.Visibility = Visibility.Visible;
				}
				else
					n.Panel.Visibility = Visibility.Collapsed;
			}
			m_Force = f;
		}

		private void RedoDivFlags()
		{
			if (!m_bInit)
				return;
			StatFlags f = m_StatFlags;
			StatFlags divflags = DivFlags;
			int vcount = 0;
			ItemNode lastdiv = null;
			foreach (ItemNode n in m_ItemList)
			{
				if ((f & n.ShowFlag) != StatFlags.Zero) // Item is visible... We can have a div.
					vcount++;
				if ((divflags & n.ShowFlag) != StatFlags.Zero && vcount > 0)
				{
					if (n.Div == null)
					{
						n.Div = new Line();
						Panel.Children.Insert(Panel.Children.IndexOf(n.Panel) + 1, n.Div);
					}
					else
						n.Div.Visibility = Visibility.Visible;
					lastdiv = n;
					vcount = 0;
				}
				else if (n.Div != null)
					n.Div.Visibility = Visibility.Collapsed;
			}
			if (vcount == 0 && lastdiv != null)
				lastdiv.Div.Visibility = Visibility.Collapsed;
		}

		void UpdateLine(Unit unit, StatFlags flags)
		{
			if (!m_bInit)
				return;

			if ((flags & StatFlags.Disconnected) != StatFlags.Zero)
			{
				flags = ~StatFlags.Zero;
			}
			ItemNode.ms_bDisconnected = unit.Statistics.Disconnected;


			int num, len = m_Units.Length;
			for (num = 0; num < len; num++)
				if (m_Units[num] == unit)
					break;
			if (num >= len || unit == null)
				return;
			foreach (ItemNode n in m_ItemActive)
			{
				if ((n.Flag & flags) != StatFlags.Zero && n.LabelList.Count > num)
				{
					n.Update(n.LabelList[num],unit);
				}
			}
		}
	}

	/*
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

	LeftPower		= (1L << 33),
	RightPower		= (1L << 34),
	PercentAT		= (1L << 35),
	FrontGear		= (1L << 36),
	RearGear		= (1L << 37),
	Gear			= FrontGear | RearGear,
	GearInches		= (1L << 38),
	RawSpinScan		= (1L << 39),
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

	Max				= (1L << 49),
	Mask			= Max - 1
	*/
}
