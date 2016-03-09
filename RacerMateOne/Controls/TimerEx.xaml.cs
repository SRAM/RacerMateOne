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
	/// Interaction logic for TimerEx.xaml
	/// </summary>
	public partial class TimerEx : BaseUnit
	{
		public TimerEx()
		{
			InitializeComponent();
			if (!AppWin.IsInDesignMode)
				Background = Brushes.Transparent;
			StatFlags = StatFlags.Time | StatFlags.Lap | StatFlags.RiderName | StatFlags.Finished;
		}

		protected override void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			base.BaseUnit_Loaded(sender, e);
			if (m_Unit != null)
				OnUnitFlagsChanged(m_Unit, m_StatFlags);
		}

		bool m_bShowFinal = true;
		public bool ShowFinal
		{
			get { return m_bShowFinal; }
			set
			{
				if (value == m_bShowFinal)
					return;
				m_bShowFinal = value;
				OnUnitFlagsChanged(Unit, m_StatFlags);
			}
		}

		// Before the race
		// just show the name.

		// Durring race
		// LapTime & Last shown if lap > 1 
		// Best shown if best is not last lap
		// if lap > 2 change label to Last/Best otheriwse label is just Last

		// After the race
		// LapTime is total time and shown, add best label.
		// Last and best are shown as above.

		bool m_bShowLapTimer;
		bool m_bCurFinsihed;
		protected override void OnUnitFlagsChanged(Unit unit, StatFlags changed)
		{
			if (!m_bInit || Unit == null)
				return;
			if (Unit.Statistics.Finished != m_bCurFinsihed)
			{
				m_bCurFinsihed = Unit.Statistics.Finished;
				s_Final.Visibility = m_bCurFinsihed ? Visibility.Visible:Visibility.Collapsed;
				changed |= StatFlags.Lap | StatFlags.Time;
			}
			if ((changed & StatFlags.Lap) != StatFlags.Zero)
			{
				// s_Lap, s_Last, s_Best
				// Determine what is shown.
				int lap = Unit.Statistics.Lap;
				int best = Unit.Statistics.BestLap;
				m_bShowLapTimer = true;
				if (lap <= 1 && !m_bCurFinsihed)
				{
					// First lap doesn't get anyting
					s_Lap.Visibility = s_Best.Visibility = s_Last.Visibility = Visibility.Collapsed; 
					m_bShowLapTimer = false;
				}
				else if (m_bCurFinsihed && Unit.Laps == 1)
				{
					// Fi we are finished and laps is one...
					s_Best.Visibility = s_Last.Visibility = Visibility.Collapsed;
					s_Lap.Visibility = m_bShowFinal ? Visibility.Visible : Visibility.Collapsed;
				}
				else
				{
					s_Lap.Visibility = s_Last.Visibility = Visibility.Visible;
					LastLap.Content = Unit.Statistics.LastLapTimeString;
					if (lap > 2 || m_bCurFinsihed)
					{
						s_Best.Visibility = Visibility.Visible;
						BestLabel.Content = String.Format("Best lap {0}", best);
						BestLap.Content = Unit.Statistics.BestLapTimeString;
					}
					else
						s_Best.Visibility = Visibility.Collapsed;
				}
			}
			if ((changed & StatFlags.RiderName) != StatFlags.Zero)
			{
				RiderName.Content = unit.Statistics.RiderName;
			}
			if ((changed & (StatFlags.Time | StatFlags.Lap)) != StatFlags.Zero && m_bShowLapTimer)
			{
				LapTimer.Content = m_bCurFinsihed ? Unit.Statistics.TimerString : Unit.Statistics.LapTimeString;
			}
		}

	}
}
