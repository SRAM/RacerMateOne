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
	/// Interaction logic for StatsArea.xaml
	/// </summary>
	public partial class StatsArea : BaseUnit
	{
		public enum Modes
		{
			Simple,
			Polar,
			Bar,
			Stats,
			Time
		};
		//=========================================================================================================
		public static DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(Modes), typeof(StatsArea),
			new FrameworkPropertyMetadata(Modes.Stats, new PropertyChangedCallback(_ModeChanged)));
		public Modes Mode
		{
			get { return (Modes)this.GetValue(ModeProperty); }
			set { this.SetValue(ModeProperty, value); }
		}
		private static void _ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((StatsArea)d).OnModeChanged(false);
		}
		//=========================================================================================================
		StatFlags m_StatsAreaFlags = StatFlags.Distance | StatFlags.Cadence | StatFlags.Grade | StatFlags.RiderName |
						StatFlags.Speed | StatFlags.Watts | StatFlags.HeartRate |
						StatFlags.Bars | StatFlags.Gearing;

		public StatFlags StatsAreaFlags  {
			get { return m_StatsAreaFlags; }
			set {
				if (m_StatsAreaFlags == value)
					return;
				m_StatsAreaFlags = value;
				OnModeChanged(true);
			}
		}


		//=========================================================================================================
		public Render3DView View;

		/*********************************************************************************************************

		*********************************************************************************************************/

		public StatsArea() {

#if DEBUG_LOG_ENABLED
			Log.WriteLine("StatsArea.xaml.cs, StatsArea::StatsArea()");
#endif

			InitializeComponent();
			StatFlags = StatFlags.Distance | StatFlags.Cadence | StatFlags.Grade | StatFlags.RiderName | 
						StatFlags.Speed | StatFlags.Watts | StatFlags.HeartRate |
						StatFlags.Bars | StatFlags.Gearing;
		}

		/*********************************************************************************************************

		*********************************************************************************************************/

		protected override void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			SpeedHeader.Content = ConvertConst.TextMPHorKPH;
			base.BaseUnit_Loaded(sender,e);
			OnModeChanged(true);
		}

		/*********************************************************************************************************

		*********************************************************************************************************/

		protected override void BaseUnit_Unloaded(object sender, RoutedEventArgs e)
		{
			base.BaseUnit_Unloaded(sender, e);
		}
		
		Modes m_CurMode = Modes.Stats;

		/*********************************************************************************************************

		*********************************************************************************************************/

		void OnModeChanged(bool force)
		{
			if (!m_bInit)
				return;
			Modes mode = Mode;
			if (!force && mode == m_CurMode)
				return;
			switch (m_CurMode)
			{
				case Modes.Polar:
				case Modes.Bar:
					SpinScanDisplay.Visibility = Visibility.Hidden;
					Background = Brushes.Transparent;
					break;
				case Modes.Simple:
					Background = Brushes.Transparent;
					StatsView.Visibility = Visibility.Collapsed;
					break;
				case Modes.Stats:
					Gearing.Visibility = TopPanel.Visibility = BottomPanel.Visibility = Visibility.Collapsed;
					break;
				case Modes.Time:
					TimerBox.Visibility = Visibility.Collapsed;
					RiderName.Visibility = Visibility.Visible;
					break;
			}
			m_CurMode = mode;
			Visibility v = Visibility.Hidden;
			switch (m_CurMode)
			{
				case Modes.Polar:
					Background = AppWin.StdBrush_Background;
					SpinScanDisplay.Visibility = Visibility.Visible;
					SSPolar.Visibility = Visibility.Visible;
					SSBar.Visibility = Visibility.Hidden;
					break;
				case Modes.Bar:
					Background = AppWin.StdBrush_Background;
					SpinScanDisplay.Visibility = Visibility.Visible;
					SSPolar.Visibility = Visibility.Hidden;
					SSBar.Visibility = Visibility.Visible;
					break;
				case Modes.Simple:
					Background = AppWin.StdBrush_Background;
					StatsView.Visibility = Visibility.Visible;
					//StatsView.StatFlags = m_StatsAreaFlags;
					break;
				case Modes.Stats:
					v = Visibility.Visible;
					if (Unit.IsVelotron) {
						Gearing.Visibility = Visibility.Visible;
					}
					TopPanel.Visibility = BottomPanel.Visibility = Visibility.Visible;
					break;

				case Modes.Time:
					v = Visibility.Visible;
					TimerBox.Visibility = Visibility.Visible;
					RiderName.Visibility = Visibility.Collapsed;
					RedoTimerSize();
					break;
			}
			if (View != null)
				View.Visibility = v;
			SetStatFlags();
			OnUnitFlagsChanged(Unit, m_StatFlags);
		}

		/*********************************************************************************************************

		*********************************************************************************************************/

		void SetStatFlags()
		{
			StatFlags s = StatFlags.Zero;
			Unit unit = Unit;
			if (unit != null)  {
				bool velotron = unit.IsVelotron;
				if (velotron) {
					s |= StatFlags.Gearing | StatFlags.GearInches;
				}

				switch (m_CurMode)  {
					case Modes.Stats:
						s |= StatFlags.RiderName | StatFlags.Speed | StatFlags.Distance | StatFlags.Lead | StatFlags.Grade |
							StatFlags.Watts | StatFlags.Watts_Avg | StatFlags.Cadence | StatFlags.HeartRate;
						break;
				}
				if ((s & StatFlags.Lead) != StatFlags.Zero)
					s |= StatFlags.Drafting;
			}
			StatFlags = s;
		}

		/*********************************************************************************************************

		*********************************************************************************************************/

		protected override void OnUnitChanged()
		{
			Unit unit = Unit;
			SSPolar.AvgBarsOn = true;
			SSBar.AvgBarsOn = true;
			TimerControl.Unit = SSPolar.Unit = SSBar.Unit = SSBox.Unit = unit;

			//Gear.Visibility = unit != null && unit.IsVelotron ? Visibility.Visible : Visibility.Collapsed;
			StatsView.Unit = unit;
			SetStatFlags();
		}

		/*********************************************************************************************************

		*********************************************************************************************************/

		protected override void OnUnitFlagsChanged(Unit unit, StatFlags changed)
		{
			if (!m_bInit)
				return;
			Statistics s = unit.Statistics;

			if ((changed & StatFlags.Disconnected) != StatFlags.Zero)  {
				if (unit.Statistics.Disconnected)  {
					Distance.Content = Speed.Content = Lead.Content = Grade.Content = "-";
					Watts.Content = Watts_Avg.Content = Cadence.Content = HeartRate.Content = "-";
					Gearing.Content = "-";
					return;
				}
				else
					changed = m_StatFlags;	// Everything updated.
			}

			if ((changed & StatFlags.Distance) != StatFlags.Zero)
				Distance.Content = s.DistanceDisplayString;
			if ((changed & StatFlags.Speed) != StatFlags.Zero)
				Speed.Content = s.Speed_String;
			if ((changed & (StatFlags.Lead | StatFlags.Drafting)) != StatFlags.Zero)
			{
				Lead.Content = s.DistanceLeadString;
				Lead.Foreground = s.Drafting ? AppWin.StdBrush_Drafting : Brushes.White;
			}
			if ((changed & StatFlags.Grade) != StatFlags.Zero)
				Grade.Content = String.Format("{0:0.0}", s.Grade);

			if ((changed & StatFlags.Watts) != StatFlags.Zero)
				Watts.Content = String.Format("{0:F0}", s.Watts);
			if ((changed & StatFlags.Watts_Avg) != StatFlags.Zero)
				Watts_Avg.Content = String.Format( "{0:F0}", s.Watts_Avg );
			if ((changed & StatFlags.Cadence) != StatFlags.Zero)
				Cadence.Content = String.Format("{0:F0}", s.Cadence);
			if ((changed & StatFlags.HeartRate) != StatFlags.Zero)
				HeartRate.Content = String.Format("{0:F0}", s.HeartRate);

			if ((changed & StatFlags.Gearing) != StatFlags.Zero) {
				Gearing.Content = s.GearingString;
			}
		
			if ((changed & StatFlags.RiderName) != StatFlags.Zero)
				RiderName.Content = s.RiderName;
		}

		/*********************************************************************************************************

		*********************************************************************************************************/

		private void BaseUnit_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			RedoTimerSize();
		}

		/*********************************************************************************************************

		*********************************************************************************************************/

		void RedoTimerSize()
		{
			if (!m_bInit)
				return;
			double w,h;
			if ((w = ActualWidth) > 0 && (h = ActualHeight) > 0)
			{
				Point uloc = this.TransformToAncestor(AppWin.Instance).Transform(new Point(0, 0));
				Point bloc = this.TransformToAncestor(AppWin.Instance).Transform(new Point(w, h));
				w = bloc.X - uloc.X;
				h = bloc.Y - uloc.Y;
				if (w < 200)
				{
					h = h * 200 / w;
					w = 200;
				}
				TimerBoxGrid.Width = w;
				TimerBoxGrid.Height = h;
			}
		}
	}
}
