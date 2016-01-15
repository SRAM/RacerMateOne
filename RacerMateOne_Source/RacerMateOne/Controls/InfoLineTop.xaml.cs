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
	/// Interaction logic for InfoLineTop.xaml
	/// </summary>
	public partial class InfoLineTop : BaseUnit
	{
		//===============================================================
		public static DependencyProperty SmallProperty = DependencyProperty.Register("Small", typeof(bool), typeof(InfoLineTop),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnSmallChanged)));

		public bool Small
		{
			get { return (bool)this.GetValue(SmallProperty); }
			set { this.SetValue(SmallProperty, value); }
		}
		private static void OnSmallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((InfoLineTop)d).SmallChanged();
		}
		void SmallChanged()
		{
			bool small = Small;
			if (!m_bInit || m_CurSmall == small)
				return;
			m_CurSmall = small;
			p_AltDrag.Visibility = small ? Visibility.Visible : Visibility.Collapsed;
			l_Length.Visibility = l_Length_label.Visibility = p_Center.Visibility = !small ? Visibility.Visible : Visibility.Collapsed;
			if (m_Unit != null)
				OnUnitFlagsChanged(m_Unit, m_StatFlags);
		}

		//===============================================================
		StatFlags m_ValidDisplayFlags = StatFlags.TSS_IF_NP | StatFlags.DragFactor;
		StatFlags m_DisplayFlags;
		public StatFlags DisplayFlags
		{
			get { return m_DisplayFlags; }
			set
			{
				StatFlags v = value & m_ValidDisplayFlags;
				if (v == m_DisplayFlags)
					return;
				m_DisplayFlags = v;
				RedoDisplayFlags();
			}
		}



		public InfoLineTop()
		{
			InitializeComponent();
			StatFlags = StatFlags.TSS_IF_NP | StatFlags.DragFactor | StatFlags.Lap | StatFlags.Course;
			m_DisplayFlags = StatFlags;
		}
		bool m_CurSmall = false;

		protected override void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			base.BaseUnit_Loaded(sender, e);
			SmallChanged();
			RedoDisplayFlags();
			if (m_Unit != null)
				OnUnitFlagsChanged(m_Unit, m_StatFlags);
		}

		protected override void OnUnitFlagsChanged(Unit unit, StatFlags changed)
		{
			if (!m_bInit)
				return;
			Statistics s = unit.Statistics;
			if ((changed & StatFlags.TSS) != StatFlags.Zero)
				l_TSS.Content = String.Format("{0:F1}",s.TSS );
			if ((changed & StatFlags.IF) != StatFlags.Zero)
				l_IF.Content = String.Format("{0:F1}",s.IF );
			if ((changed & StatFlags.NP) != StatFlags.Zero)
				l_NP.Content = String.Format("{0:F1}",s.NP );
			if ((changed & StatFlags.DragFactor) != StatFlags.Zero)
			{
				if (m_CurSmall)
					l_DragFactorAlt.Content = String.Format("{0:F0}%", s.DragFactor);
				else
					l_DragFactor.Content = String.Format("{0:F0}%", s.DragFactor);
			}
			if ((changed & StatFlags.Lap) != StatFlags.Zero)
			{
				if (Unit.Laps <= 1)
					p_Laps.Visibility = Visibility.Collapsed;
				else
				{
					p_Laps.Visibility = Visibility.Visible;
					l_Laps.Content = String.Format("{0}/{1}",s.Lap,Unit.Laps);
				}
			}
			if (!m_CurSmall && (changed & StatFlags.Course) != StatFlags.Zero)
			{
				l_Length.Content = String.Format("{0:F1}{1}",
					s.Course == null ? 0.0 : s.Course.TotalX * ConvertConst.MetersToMilesOrKilometers, s.DistanceAbbr);
			}
		}

		void RedoDisplayFlags()
		{
			p_Left.Visibility = (m_DisplayFlags & StatFlags.TSS_IF_NP) == StatFlags.TSS_IF_NP ? Visibility.Visible:Visibility.Collapsed;
			p_Center.Visibility = (m_DisplayFlags & StatFlags.DragFactor) == StatFlags.DragFactor ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
