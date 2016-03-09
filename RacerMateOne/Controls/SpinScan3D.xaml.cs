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
	/// Interaction logic for SpinScan3D.xaml
	/// </summary>
	public partial class SpinScan3D : BaseUnit
	{
		public SpinScan3D()
		{
			InitializeComponent();

			b_bars.UpdateSourceTrigger = b_l_ata.UpdateSourceTrigger = b_r_ata.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
		}
		bool m_Polar = true;
		public bool Polar
		{
			get { return m_Polar; }
			set
			{
				if (m_Polar == value)
					return;
				m_Polar = value;
				OnUnitChanged();
			}
		}
		Binding b_bars = new Binding("Bars");
		Binding b_l_ata = new Binding("SSLeftATA");
		Binding b_r_ata = new Binding("SSLeftATA");



		protected override void OnUnitChanged()
		{
			if (!m_bInit)
				return;

			if (m_Unit != null)
				b_bars.Source = b_r_ata.Source = b_l_ata.Source = m_Unit.Statistics;
			if (m_Polar)
			{
				PolarSS.Visibility = Visibility.Visible;
				PolarSS.ShowLabels = false;
				PolarSS.Unit = m_Unit;
				BarSS.Visibility = Visibility.Hidden;
				BarSS.Unit = null;
			}
			else
			{
				PolarSS.Visibility = Visibility.Hidden;
				PolarSS.Unit = null;
				BarSS.Visibility = Visibility.Visible;
				BarSS.ShowLabels = false;
				BarSS.Unit = m_Unit;
			}
		
		}

	}
}
