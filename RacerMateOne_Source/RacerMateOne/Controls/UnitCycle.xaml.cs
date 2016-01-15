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
using System.Windows.Threading;
using System.Threading;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for UnitCycle.xaml
	/// </summary>
	public partial class UnitCycle : UserControl
	{
		//==============================================================
		public static readonly RoutedEvent OnUnitChangedEvent =
			EventManager.RegisterRoutedEvent(
			"OnUnitChanged", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(UnitCycle));

		public event RoutedEventHandler OnUnitChanged
		{
			add { AddHandler(OnUnitChangedEvent, value); }
			remove { RemoveHandler(OnUnitChangedEvent, value); }
		}

		public int CycleTime = 5;


		List<Label> m_Names = new List<Label>();

		DispatcherTimer m_Timer = new DispatcherTimer();

		public UnitCycle()
		{
			InitializeComponent();
			m_Timer.Tick += new EventHandler(TimerTick);
			m_Timer.Interval = new TimeSpan(0, 0, CycleTime);
			
		}

		private void TimerTick(object sender, EventArgs e)
		{
			int v = m_Active + 1;
			if (v >= Unit.Active.Count)
				v = 0;
			ActiveUnit = v;
		}

		public Unit Unit { get { return Unit.RaceUnit[m_Active]; } }

		int m_Active = 0;
		public int ActiveUnit
		{
			get { return m_Active; }
			set
			{
				int v = value < 0 ? 0 : value >= m_Names.Count ? m_Names.Count - 1 : value;
				if (m_Active == v)
					return;
				m_Names[m_Active].Foreground = Brushes.White;
				m_Names[v].Foreground = Brushes.Red;
				m_Active = v;
				if (m_UpdateStarted)
				{
					m_Timer.Stop();
					m_Timer.Start();
					RoutedEventArgs args = new RoutedEventArgs(OnUnitChangedEvent);
					RaiseEvent(args);
				}
				AdjustPointer();
			}
		}
		public void ForceUnit(Unit unit)
		{
			int cnt = 0;
			foreach (Unit u in Unit.RaceUnit)
			{
				if (u == unit)
				{
					ActiveUnit = cnt;
					break;
				}
				cnt++;
			}
		}



		StatFlags m_StatFlags = StatFlags.Order | StatFlags.RiderName;

		bool m_bInit = false;
		bool m_UpdateStarted = false;

		void UpdateStart()
		{
			if (IsVisible && m_bInit && !m_UpdateStarted && !AppWin.IsInDesignMode)
			{
				m_UpdateStarted = true;
				Unit.AddNotify(null, m_StatFlags, new Unit.NotifyEvent(OnUpdateUnit));
				OnUpdateUnit(null, m_StatFlags);
				m_Timer.Start();
			}
		}
		void UpdateStop()
		{
			if (m_UpdateStarted)
			{
				Unit.RemoveNotify(null, m_StatFlags, new Unit.NotifyEvent(OnUpdateUnit));
				m_UpdateStarted = false;
				m_Timer.Stop();
			}
		}


		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			m_bInit = true;
			UpdateStart();
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			UpdateStop();
		}

		void OnUpdateUnit( Unit unit_n, StatFlags changed )
		{
			if (!m_bInit)
				return ;
			if ((changed & StatFlags.RiderName) != StatFlags.Zero)
			{
				NameLine.Children.Clear();
				m_Names.Clear();
				foreach(Unit unit in Unit.RaceUnit)
				{
					Label label = new Label();
					label.Tag = unit;
					m_Names.Add(label);
					NameLine.Children.Add(label);
				}
				changed |= StatFlags.Order;
			}
			if ((changed & StatFlags.Order) != StatFlags.Zero)
			{
				foreach(Label label in m_Names)
				{
					Unit unit = (Unit)label.Tag;
					TextBlock tb = new TextBlock();
					InlineCollection tbi = tb.Inlines;
					TextBlock sub = new TextBlock();
					sub.MaxWidth = 100;
					sub.TextTrimming = TextTrimming.CharacterEllipsis;
					sub.Text = unit.Statistics.RiderName;
					sub.Margin = new Thickness(0,0,8,0);
					tbi.Add(sub);
					tbi.Add(unit.Order.ToString());
					label.Content = tb;
				}
				m_Names[m_Active].Foreground = Brushes.Red;
				Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
				{
					RoutedEventArgs args = new RoutedEventArgs(OnUnitChangedEvent);
					RaiseEvent(args);
					AdjustPointer();
				});
			}
		}
		void AdjustPointer()
		{
			Label active = m_Names[m_Active];
			Point pt = active.TransformToAncestor(MainGrid).Transform(new Point(active.ActualWidth/2.0, 0));
			Dest.To = pt.X - 40;
			AnimatePointer.Begin();

		}

		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
				UpdateStart();
			else
				UpdateStop();
		}


	}
}
