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
	/// Interaction logic for Timer.xaml
	/// </summary>
	public partial class Timer : BaseUnit
	{
	//===============================================================
		public static DependencyProperty ShowMainTimerProperty = DependencyProperty.Register("ShowMainTimer", typeof(bool), typeof(Timer),
				new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowMainTimerChanged)));

		public bool ShowMainTimer
		{
			get { return (bool)this.GetValue(ShowMainTimerProperty); }
			set { this.SetValue(ShowMainTimerProperty, value); }
		}
		private static void OnShowMainTimerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((Timer)d).ShowMainTimerChanged(false);
		}
		bool m_CurShowMainTimer;

		void ShowMainTimerChanged(bool force)
		{
			bool show = ShowMainTimer;
			if (!force && (!m_bInit || m_CurShowMainTimer == show))
				return;
			m_CurShowMainTimer = show;
			MainTimer.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
			if (m_Unit != null)
				OnUnitFlagsChanged(m_Unit, m_StatFlags);
		}

		//===============================================================


		public Timer()
		{
			InitializeComponent();
			if (!AppWin.IsInDesignMode)
				Background = Brushes.Transparent;
			StatFlags = StatFlags.Time | StatFlags.Lap | StatFlags.RiderName;
		}

		Label[] m_Laps = new Label[9];


		protected override void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			m_Laps[0] = Lap1;
			m_Laps[1] = Lap2;
			m_Laps[2] = Lap3;
			m_Laps[3] = Lap4;
			m_Laps[4] = Lap5;
			m_Laps[5] = Lap6;
			m_Laps[6] = Lap7;
			m_Laps[7] = Lap8;
			m_Laps[8] = Lap9;
			for (int i = 0; i < 9; i++)
				m_Laps[i].Visibility = Visibility.Collapsed;
			base.BaseUnit_Loaded(sender,e);

			ShowMainTimerChanged(true);
			if (m_Unit != null)
				OnUnitFlagsChanged(m_Unit, m_StatFlags);

		}
		
		int m_CurLaps = -1;
		protected override void  OnUnitChanged()
		{
			m_CurLaps = 0;
 			 base.OnUnitChanged();
		}

		protected override void OnUnitFlagsChanged(Unit unit, StatFlags changed)
		{
			if (!m_bInit)
				return;
			int claps = Unit.Laps;
			if ((ShowMainTimer || claps > 1) && (changed & StatFlags.Time) != StatFlags.Zero)
			{
				MainTimer.Content = unit.Statistics.TimerString;
				if (claps != m_CurLaps)
				{
					m_CurLaps = claps;
					LapTimer.Visibility = claps > 1 ? Visibility.Visible:Visibility.Collapsed;
				}
				if (claps > 1)
					LapTimer.Content = unit.Statistics.LapTimeString;
			}

			if ((changed & StatFlags.Lap) != StatFlags.Zero)
			{
				// Redo laps.
				List<double> laps = unit.Statistics.LapTimes;
				int n = 0;
				for (int i = laps.Count - 1; i >= 0; i--)
				{
					m_Laps[n].Visibility = Visibility.Visible;
					m_Laps[n].Content = Statistics.SecondsToTimeString(laps[i]);
					n++;
					if (n >= 9)
						break;
				}
				for (; n < 9; n++)
					m_Laps[n].Visibility = Visibility.Collapsed;
			}
			if ((changed & StatFlags.RiderName) != StatFlags.Zero)
				RiderName.Content = unit.Statistics.RiderName;
		}

		public Brush RiderNameColor
		{
			get { return RiderName.Foreground; }
			set
			{
				RiderName.Foreground = value;
			}
		}

	}
}
