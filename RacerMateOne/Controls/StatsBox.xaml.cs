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
	/// Interaction logic for StatsBox.xaml
	/// </summary>
	public partial class StatsBox : BaseUnit
	{
		/*
		Speed
		Speed_Avg
		Speed_Max
		Distance
		Distance_Avg
		Distance_Max
		Watts
		Watts_Avg
		Watts_Max
		Watts_Wkg
		TSS
		Cadence
		Cadence_Avg
		Cadence_Max
		HeartRate
		HeartRate_Avg
		HeartRate_Max
		Calories
		PulsePower
		Grade
		Wind
		Gear
		Gearing
		*/

		const StatFlags c_BaseStatFlags = StatFlags.Speed | StatFlags.Speed_Avg | StatFlags.Speed_Max | StatFlags.Distance |
				StatFlags.Watts | StatFlags.Watts_Avg | StatFlags.Watts_Max | StatFlags.Watts_Wkg | StatFlags.TSS_IF_NP |
				StatFlags.Cadence | StatFlags.Cadence_Avg | StatFlags.Cadence_Max |
				StatFlags.HeartRate | StatFlags.HeartRate_Avg | StatFlags.HeartRate_Max | StatFlags.HeartRateAlarm |
				StatFlags.Calories | StatFlags.PulsePower |
				StatFlags.Grade | StatFlags.Wind |
				StatFlags.GearInches | StatFlags.Gearing;

		/****************************************************************************************************************

		****************************************************************************************************************/

		public StatsBox()  {
#if DEBUG_LOG_ENABLED
			Log.WriteLine("StatsBox.xaml.cs, StatsBox() constructor");
#endif
			InitializeComponent();
			Background = Brushes.Transparent;
			StatFlags = c_BaseStatFlags;
		}								// StatsBox()

		/****************************************************************************************************************

		****************************************************************************************************************/

		protected override void BaseUnit_Loaded(object sender, RoutedEventArgs e)  {
			SpeedLabel.Content = ConvertConst.TextMPHorKPH;
			base.BaseUnit_Loaded(sender, e);
			if (m_Unit != null)
				OnUnitFlagsChanged(m_Unit, m_StatFlags);
		}										// BaseUnit_Loaded()

		/****************************************************************************************************************

		****************************************************************************************************************/

		protected override void OnUnitChanged()  {
			GearLine.Visibility = Unit != null && Unit.IsVelotron ? Visibility.Visible : Visibility.Collapsed;
			base.OnUnitChanged();
		}														// OnUnitChanged()

		/****************************************************************************************************************

		****************************************************************************************************************/

		protected override void OnUnitFlagsChanged(Unit unit, StatFlags changed) {
			if (!m_bInit || Unit == null) {
				return;
			}

			Statistics s = unit.Statistics;

			if ((changed & StatFlags.Speed) != StatFlags.Zero) {
				Speed.Content = s.Speed_String;
			}

			if ((changed & StatFlags.Speed_Avg) != StatFlags.Zero)
				Speed_Avg.Content = s.Speed_Avg_String;

			if ((changed & StatFlags.Speed_Max) != StatFlags.Zero)
				Speed_Max.Content = s.Speed_Max_String;

			if ((changed & StatFlags.Distance) != StatFlags.Zero)
				Distance.Content = s.Distance_String;

			if ((changed & StatFlags.Watts) != StatFlags.Zero)
				Watts.Content = s.Watts_String;

			if ((changed & StatFlags.Watts_Avg) != StatFlags.Zero)
				Watts_Avg.Content = s.Watts_Avg_String;

			if ((changed & StatFlags.Watts_Max) != StatFlags.Zero)
				Watts_Max.Content = s.Watts_Max_String;

			if ((changed & StatFlags.Watts_Wkg) != StatFlags.Zero)
				Watts_Wkg.Content = s.Watts_Wkg_String;

			if ((changed & StatFlags.TSS_IF_NP) != StatFlags.Zero)
				TSS.Content = String.Format("{0:0.#}/{1:0.#}/{2:0.#}", unit.Statistics.TSS, unit.Statistics.IF, unit.Statistics.NP);

			if ((changed & StatFlags.Cadence) != StatFlags.Zero)
				Cadence.Content = s.Cadence_String;

			if ((changed & StatFlags.Cadence_Avg) != StatFlags.Zero)
				Cadence_Avg.Content = s.Cadence_Avg_String;

			if ((changed & StatFlags.Cadence_Max) != StatFlags.Zero)
				Cadence_Max.Content = s.Cadence_Max_String;

			if ((changed & StatFlags.HeartRate) != StatFlags.Zero)
				HeartRate.Content = s.HeartRate_String;

			if ((changed & StatFlags.HeartRateAlarm) != StatFlags.Zero)
				HeartRate.Foreground = s.HeartRateFlash ? Brushes.Red : Brushes.White;

			if ((changed & StatFlags.HeartRate_Avg) != StatFlags.Zero)
				HeartRate_Avg.Content = s.HeartRate_Avg_String;

			if ((changed & StatFlags.HeartRate_Max) != StatFlags.Zero)
				HeartRate_Max.Content = s.HeartRate_Max_String;

			if ((changed & StatFlags.Calories) != StatFlags.Zero)
				Calories.Content = s.Calories_String;

			if ((changed & StatFlags.PulsePower) != StatFlags.Zero)
				PulsePower.Content = s.PulsePower_String;

			if ((changed & StatFlags.Grade) != StatFlags.Zero)
				Grade.Content = s.Grade_String;

			if ((changed & StatFlags.Wind) != StatFlags.Zero)
				Wind.Content = s.Wind_String;

			if ((changed & StatFlags.GearInches) != StatFlags.Zero)  {
				Gear.Content = s.Gear_String;
			}

			if ((changed & StatFlags.Gearing) != StatFlags.Zero)  {
				Gearing.Content = s.Gearing_String;
			}
		}								// OnUnitFlagsChanged()

		/****************************************************************************************************************

		****************************************************************************************************************/

		protected override void OnStatFlagsChanged()  {
			StatFlags s = StatFlags;
			if (s != c_BaseStatFlags)
				StatFlags = c_BaseStatFlags;
			base.OnStatFlagsChanged();
		}									// OnStatFlagsChanged()
	}
}
