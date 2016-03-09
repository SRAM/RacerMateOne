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
	public partial class SpinScanBox : BaseUnit
	{
		//=====================================================
		public static DependencyProperty ShowBoxProperty = DependencyProperty.Register("ShowBox", typeof(bool), typeof(SpinScanBox),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(_ShowBoxChanged)));
		public bool ShowBox
		{
			get { return (bool)this.GetValue(ShowBoxProperty); }
			set
			{
				// Lock in these bars by copying over the stuff.
				this.SetValue(ShowBoxProperty, value);
			}
		}
		private static void _ShowBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { ((SpinScanBox)d).OnShowBoxChanged(); }
		//=====================================================

		public SpinScanBox()
		{
			InitializeComponent();
			this.StatFlags = StatFlags.SS_Stats;
		}

		protected override void BaseUnit_Loaded(object sender, RoutedEventArgs e)
		{
			base.BaseUnit_Loaded(sender, e);
			OnShowBoxChanged();
		}

		bool m_bShowBox = false;
		void OnShowBoxChanged()
		{
			if (!m_bInit)
				return;
			bool showbox = ShowBox;
			if (showbox == m_bShowBox)
				return;
			m_bShowBox = showbox;
			if (showbox)
			{
				BackBox.Visibility = Visibility.Visible;
				SPanel.Margin = new Thickness(0, 4, 3, 4);
			}
			else
			{
				BackBox.Visibility = Visibility.Collapsed;
				SPanel.Margin = new Thickness(0, 0, 0, 0);
			}
		}

		protected override void OnUnitFlagsChanged(Unit unit, StatFlags changed)
		{
			if (!m_bInit)
				return;
			Statistics s = unit.Statistics;
			if ((changed & StatFlags.SSLeft) != StatFlags.Zero)
				d_SSLeft.Content = String.Format("{0:F0}", s.SSLeft);
			if ((changed & StatFlags.SSRight) != StatFlags.Zero)
				d_SSRight.Content = String.Format("{0:F0}", s.SSRight);
			if ((changed & StatFlags.SS) != StatFlags.Zero)
				d_SSAvg.Content = String.Format("{0:F0}", s.SS);
			if ((changed & StatFlags.SSLeftSplit) != StatFlags.Zero)
				d_SSLeftWatts.Content = String.Format("{0:F0}", s.SSLeftSplit);
			if ((changed & StatFlags.SSRightSplit) != StatFlags.Zero)
				d_SSRightWatts.Content = String.Format("{0:F0}", s.SSRightSplit);
			if ((changed & StatFlags.SSLeft_Avg) != StatFlags.Zero)
				d_SSLeft_Avg.Content = String.Format("{0:F0}", s.SSLeft_Avg);
			if ((changed & StatFlags.SSRight_Avg) != StatFlags.Zero)
				d_SSRight_Avg.Content = String.Format("{0:F0}", s.SSRight_Avg);
			if ((changed & StatFlags.SSLeftATA) != StatFlags.Zero)
				d_SSLeftATA.Content = String.Format("{0:F0}", s.SSLeftATA);
			if ((changed & StatFlags.SSRightATA) != StatFlags.Zero)
				d_SSRightATA.Content = String.Format("{0:F0}", s.SSRightATA);
			if ((changed & StatFlags.SS_Avg) != StatFlags.Zero)
				d_SSAvgATA.Content = String.Format("{0:F0}", s.SS_Avg);
		}
	}
}
