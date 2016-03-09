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
	/// Interaction logic for StatsLine.xaml
	/// </summary>
	public partial class StatsLine : UserControl
	{
		//======================================================================
		public static DependencyProperty StatFlagsProperty = DependencyProperty.Register("StatFlags", typeof(StatFlags), typeof(StatsLine),
			new FrameworkPropertyMetadata(StatFlags.Zero, new PropertyChangedCallback(_StatFlagsChanged)));
		public StatFlags StatFlags
		{
			get { return (StatFlags)this.GetValue(StatFlagsProperty); }
			set { this.SetValue(StatFlagsProperty, value); }
		}
		private static void _StatFlagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((StatsLine)d).OnStatFlagsChanged();
		}
		protected StatFlags m_StatFlags;
		void OnStatFlagsChanged()
		{
			StatFlags flags = StatFlags;
			if (m_StatFlags == flags)
				return;
			ShutdownUpdate();
			m_StatFlags = flags;
			RedoControl();
			StartUpdate();
		}
		//======================================================================
		public static DependencyProperty DivFlagsProperty = DependencyProperty.Register("DivFlags", typeof(StatFlags), typeof(StatsLine),
			new FrameworkPropertyMetadata(StatFlags.Zero, new PropertyChangedCallback(_DivFlagsChanged)));
		public StatFlags DivFlags
		{
			get { return (StatFlags)this.GetValue(DivFlagsProperty); }
			set { this.SetValue(DivFlagsProperty, value); }
		}
		private static void _DivFlagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((StatsLine)d).RedoDivFlags();
		}
		//======================================================================
		public static DependencyProperty LinesProperty = DependencyProperty.Register("Lines", typeof(int), typeof(StatsLine),
			new FrameworkPropertyMetadata(1, new PropertyChangedCallback(_LineChanged)));
		public int Lines
		{
			get { return (int)this.GetValue(LinesProperty); }
			set { this.SetValue(LinesProperty, value); }
		}
		private static void _LineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((StatsLine)d).OnLineChanged();
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
			public StackPanel Panel;
			public StatFlags Flag;
			public String FormatString;
			public List<Label> LabelList = new List<Label>();
			public Line Div;
			public ItemNode(StackPanel panel, StatFlags flag, String fstring)
			{
				Panel = panel;
				Flag = flag;
				FormatString = fstring;
			}
		}
		List<ItemNode> m_ItemList = new List<ItemNode>();
		List<ItemNode> m_ItemActive = new List<ItemNode>();

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
		public StatsLine()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (RM1_Settings.General.Metric)
			{
				((Label)(Speed.Children[0])).Content = "KPH";
			}

			m_ItemList.Add(new ItemNode( RiderName, StatFlags.RiderName, "{0}" ));
			m_ItemList.Add(new ItemNode( Speed, StatFlags.Speed, "{0:F1}" ));
			m_ItemList.Add(new ItemNode( Speed_Avg, StatFlags.Speed_Avg, "{0:F1}" ));
			m_ItemList.Add(new ItemNode( Speed_Max, StatFlags.Speed_Max, "{0:F1}" ));

			m_ItemList.Add(new ItemNode( Watts, StatFlags.Watts, "{0:F1}" ));
			m_ItemList.Add(new ItemNode( Watts_Avg, StatFlags.Watts_Avg, "{0:F1}" ));
			m_ItemList.Add(new ItemNode( Watts_Max, StatFlags.Watts_Max, "{0:F1}" ));
			
			m_ItemList.Add(new ItemNode( HeartRate, StatFlags.HeartRate, "{0:F0}" ));
			m_ItemList.Add(new ItemNode( HeartRate_Avg, StatFlags.HeartRate_Avg, "{0:F0}" ));
			m_ItemList.Add(new ItemNode( HeartRate_Max, StatFlags.HeartRate_Max, "{0:F0}" ));

			m_ItemList.Add(new ItemNode( Cadence, StatFlags.Cadence, "{0:F0}" ));
			m_ItemList.Add(new ItemNode( Cadence_Avg, StatFlags.Cadence_Avg, "{0:F0}" ));
			m_ItemList.Add(new ItemNode( Cadence_Max, StatFlags.Cadence_Max, "{0:F0}" ));

			m_ItemList.Add(new ItemNode( Distance, StatFlags.Distance, "{0:F1}" ));
			m_ItemList.Add(new ItemNode( Calories, StatFlags.Calories, "{0:F1}" ));
			m_ItemList.Add(new ItemNode( Grade, StatFlags.Grade, "{0:F1}" ));


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
					UpdateLine(unit, m_CurUpdateFlags);
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
				if ((f & n.Flag) != StatFlags.Zero)
				{
					m_ItemActive.Add(n);
					if (n.Panel.Children.Count != lines)
					{
						while (n.Panel.Children.Count < lines)
						{
							label = new Label();
							label.Content = "xx";
							if ((n.Flag & (StatFlags.HeartRate | StatFlags.RiderName)) != StatFlags.Zero)
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
				if ((f & n.Flag) != StatFlags.Zero)
					vcount++;
				if ((divflags & n.Flag) != StatFlags.Zero && vcount > 0)
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

			int num, len = m_Units.Length;
			for (num = 0; num < len; num++)
				if (m_Units[num] == unit)
					break;
			if (num >= len || unit == null)
				return;

			foreach (ItemNode n in m_ItemActive)
			{
				if ((n.Flag & flags) != StatFlags.Zero)
				{
					n.LabelList[num].Content = String.Format(n.FormatString, unit.Statistics.GetFromFlagDisplay(n.Flag));
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
