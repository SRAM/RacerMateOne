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
using System.Windows.Interop;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.ComponentModel;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for BarSpinScan.xaml
	/// </summary>
	public partial class BarSpinScan : UserControl
	{
		private static bool? _isInDesignMode;
		public static bool IsInDesignMode
		{
			get
			{
				if (!_isInDesignMode.HasValue)
				{
#if SILVERLIGHT
					_isInDesignMode = DesignerProperties.IsInDesignTool;
#else
					var prop = DesignerProperties.IsInDesignModeProperty;
					_isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
#endif
				}
				return _isInDesignMode.Value;
			}
		}
		//=========================================================================================================
		public static DependencyProperty ShowLabelsProperty = DependencyProperty.Register("ShowLabels", typeof(bool), typeof(BarSpinScan),
			new FrameworkPropertyMetadata(true, new PropertyChangedCallback(_ShowLabelsChanged)));
		public bool ShowLabels
		{
			get { return (bool)this.GetValue(ShowLabelsProperty); }
			set
			{
				// Lock in these bars by copying over the stuff.
				this.SetValue(ShowLabelsProperty, value);
			}
		}
		private static void _ShowLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BarSpinScan)d).OnShowLabelsChanged();
		}
		//=========================================================================================================
		public static DependencyProperty SSLeftATAProperty = DependencyProperty.Register("SSLeftATA", typeof(float), typeof(BarSpinScan),
			new FrameworkPropertyMetadata(50.0f, new PropertyChangedCallback(_ATAChanged)));
		public float SSLeftATA
		{
			get { return (float)this.GetValue(SSLeftATAProperty); }
			set
			{
				// Lock in these bars by copying over the stuff.
				this.SetValue(SSLeftATAProperty, value);
			}
		}
		public static DependencyProperty SSRightATAProperty = DependencyProperty.Register("SSRightATA", typeof(float), typeof(BarSpinScan),
			new FrameworkPropertyMetadata(50.0f, new PropertyChangedCallback(_ATAChanged)));
		public float SSRightATA
		{
			get { return (float)this.GetValue(SSRightATAProperty); }
			set
			{
				// Lock in these bars by copying over the stuff.
				this.SetValue(SSRightATAProperty, value);
			}
		}
		private static void _ATAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BarSpinScan)d).OnATAChanged(false);
		}

		//=========================================================================================================
		public static float[] m_DefBars;
		public static float[] m_AvgDefBars;

		public static DependencyProperty BarsProperty = DependencyProperty.Register("Bars", typeof(float[]), typeof(BarSpinScan),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(_BarsChanged)));
		public float[] Bars
		{
			get { return (float[])this.GetValue(BarsProperty); }
			set
			{
				// Lock in these bars by copying over the stuff.
				this.SetValue(BarsProperty, value == null ? m_DefBars:value.Clone());
			}
		}
		private static void _BarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BarSpinScan)d).OnBarsChanged();
		}
		//=========================================================================================================
		public static DependencyProperty AvgBarsProperty = DependencyProperty.Register("AvgBars", typeof(float[]), typeof(BarSpinScan),
			new FrameworkPropertyMetadata(null, new PropertyChangedCallback(_AvgBarsChanged)));
		public float[] AvgBars
		{
			get { return (float[])this.GetValue(AvgBarsProperty); }
			set
			{
				// Lock in these AvgBars by copying over the stuff.
				this.SetValue(AvgBarsProperty, value == null ? m_DefBars:value.Clone());
			}
		}
		private static void _AvgBarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BarSpinScan)d).OnAvgBarsChanged();
		}
		//=========================================================================================================
		public static DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(BarSpinScan),
			new FrameworkPropertyMetadata(10.0, new PropertyChangedCallback(_ScaleChanged)));
		public double Scale
		{
			get { return (double)this.GetValue(ScaleProperty); }
			set
			{
				// Lock in these bars by copying over the stuff.
				this.SetValue(ScaleProperty, value);
			}
		}
		private static void _ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BarSpinScan)d).OnScaleChanged();
		}
		//=========================================================================================================
		public static DependencyProperty MaxForceProperty = DependencyProperty.Register("MaxForce", typeof(double), typeof(BarSpinScan),
			new FrameworkPropertyMetadata(10.0));
		public double MaxForce
		{
			get { return (double)this.GetValue(MaxForceProperty); }
			protected set
			{
				// Lock in these bars by copying over the stuff.
				this.SetValue(MaxForceProperty, value);
			}
		}
		//=========================================================================================================
		protected Unit m_Unit;
		protected StatFlags m_UnitFlags = StatFlags.Bars | StatFlags.Bars_Avg | StatFlags.SSLeftATA | StatFlags.SSRightATA;
		public Unit Unit
		{
			get { return m_Unit; }
			set
			{
				if (m_Unit == value)
					return;
				UpdateOff();
				m_Unit = value;
				UpdateOn();
			}
		}
		bool m_bUpdateOn = false;
		void UpdateOff()
		{
			if (!m_bUpdateOn)
				return;
			m_bUpdateOn = false;
			Unit.RemoveNotify(m_Unit, m_UnitFlags, new Unit.NotifyEvent(OnUnitUpdate));
			Bars = null;
			SSRightATA = SSLeftATA = 50.0f;
		}
		void UpdateOn()
		{
			if (m_bUpdateOn || m_Unit == null)
				return;
			m_bUpdateOn = true;
			Unit.AddNotify(m_Unit, m_UnitFlags, new Unit.NotifyEvent(OnUnitUpdate));
			OnUnitUpdate(m_Unit, m_UnitFlags);
		}
		void OnUnitUpdate(Unit unit, StatFlags changed)
		{
			if (!m_bInit)
				return;
			if ((changed & StatFlags.Bars) != StatFlags.Zero)
			{
				bool bshown = UpdateBars(m_BarPaths, m_Unit.Statistics.Bars);
				if (bshown != m_bCurShown)
				{
					m_bCurShown = bshown;
					LeftLine.Visibility = RightLine.Visibility = bshown ? Visibility.Visible : Visibility.Hidden;
				}
			}
			if (AvgBarsOn && (changed & StatFlags.Bars_Avg) != StatFlags.Zero)
				UpdateBars(m_AvgBarPaths, m_Unit.Statistics.AverageBars);
			if ((changed & (StatFlags.SSLeftATA | StatFlags.SSRightATA)) != StatFlags.Zero)
			{
				SSLeftATA = m_Unit.Statistics.SSLeftATA;
				SSRightATA = m_Unit.Statistics.SSRightATA;
			}
		}

		public bool AvgBarsOn;

		static BarSpinScan()
		{
			m_DefBars = new float[RM1.BarCount];
			m_AvgDefBars = new float[RM1.BarCount];
			for (int i = 0; i < RM1.BarCount; i++)
			{
				double r = 2.0 * Math.PI * i / RM1.BarCount;
				m_DefBars[i] = (float)(5.0 * (Math.Sin(r) + 1.0));

				r = 2.0 * Math.PI * (i + (1 / RM1.BarCount)) / RM1.BarCount;
				m_AvgDefBars[i] = (float)(5.0 * (Math.Sin(r) + 1.0));
			}
		}



		private Path[] m_BarPaths = new Path[RM1.BarCount];
		private Path[] m_AvgBarPaths = new Path[RM1.BarCount];
		private bool m_bInit = false;

		#pragma warning disable 414
		private bool m_bInBarChange = false;
		#pragma warning restore 414

		private double m_ATALineDeg = 0.0;
		private double m_ATAAdd = 0.0;



		public BarSpinScan()
		{
			InitializeComponent();
			if (IsInDesignMode)
			{
				Bars = m_DefBars;
				AvgBars = m_AvgDefBars;
			}
		}

		private void BarSpinScan_Loaded(object sender, RoutedEventArgs e)
		{
			int cnt = 0;
			foreach (Object o in Base.Children)
			{
				Grid grid = o as Grid;
				if (grid == null)
					continue;
				m_BarPaths[cnt] = (Path)grid.Children[1];
				m_AvgBarPaths[cnt] = (Path)grid.Children[0];
				cnt++;
			}
			m_ATAAdd = Base.ActualWidth * 0.5;
			m_ATALineDeg = m_ATAAdd / 180.0f;

			m_CurScale = Scale;
			MaxForce = Scale;
			m_bInit = true;
			bool bshown = UpdateBars(m_BarPaths, Bars);
			if (bshown != m_bCurShown)
			{
				m_bCurShown = bshown;
				LeftLine.Visibility = RightLine.Visibility = bshown ? Visibility.Visible : Visibility.Hidden;
			}
			UpdateBars(m_AvgBarPaths, AvgBars);
			OnShowLabelsChanged();
		}

		private void OnBarsChanged()
		{
			bool bshown = UpdateBars(m_BarPaths, Bars);
			if (bshown != m_bCurShown)
			{
				m_bCurShown = bshown;
				LeftLine.Visibility = RightLine.Visibility = bshown ? Visibility.Visible : Visibility.Hidden;
			}
		}
		private void OnAvgBarsChanged()
		{
			UpdateBars(m_AvgBarPaths, AvgBars);
		}

		private bool m_bCurShown = true;

		private bool UpdateBars(Path[] arr, float[] bars)
		{
			if (!m_bInit)
				return false;
			if (bars == null || bars.Length < RM1.BarCount)
			{
				for (int i = 0; i < RM1.BarCount; i++)
					arr[i].Visibility = Visibility.Hidden;
				return false;
			}

			double scale = 200.0 / Scale;
			double left, center, right;
			center = 200 - bars[RM1.BarCount - 1] * scale;
			right = 200 - bars[0] * scale;
			double max = 0,nb;
			bool bshown = false;
			for (int i = 0; i < RM1.BarCount; i++)
			{
				nb = bars[i];
				if (nb > max)
					max = nb;
				left = center;
				center = right;
				right = 200 - (i + 1 >= RM1.BarCount ? bars[0] : bars[i + 1]) * scale;
				if (redoBar(arr[i], left, center, right))
					bshown = true;
			}

			double nscale = (Math.Round(max / 5.0) + 1) * 5.0;
			if (nscale < 10.0)
				nscale = 10.0;

			if (m_CurScale != nscale)
			{
				// 10000000
				if (nscale < m_CurScale)
				{
					Int64 ticks = System.DateTime.Now.Ticks;
					if (m_HoldScaleTime == 0)
						m_HoldScaleTime = ticks + 10000000; // Must exist that way for at least a second.
					if (ticks < m_HoldScaleTime)
						nscale = m_CurScale;
				}
				else
					m_HoldScaleTime = 0;
				if (nscale != m_CurScale)
				{
					m_HoldScaleTime = 0;
					m_CurScale = nscale;
					m_bInBarChange = true;
					ChangeScaleAnim.To = nscale;
					ChangeScale.Begin();
					m_bInBarChange = false;
				}
				MaxForce = m_CurScale;
			}
			return bshown;
		}

		double m_CurScale;
		Int64 m_HoldScaleTime;


		private const double mc_sub = 6.0 / 18.0;
		private const double mc_height = 200.0;
		private const double mc_center = 6.0;
		private const double mc_right = 12.0;
		private const double mc_killheight = 199.2;
		private bool redoBar(Path path, double left, double center, double right)
		{
			left = center + (left - center) * mc_sub;
			right = center + (right - center) * mc_sub;
			bool ans;
			if (left > mc_killheight && center > mc_killheight && right > mc_killheight)
			{
				//path.Visibility = Visibility.Hidden;
				//return false;
				center = mc_killheight;
				ans = false;
			}
			else
				ans = true;


			path.Visibility = Visibility.Visible;
			// Start by dealing with the bars.
			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(new Point(0,mc_height), true /* is filled */, true /* is closed */);
				ctx.LineTo(new Point(0, center), true, false);
				ctx.LineTo(new Point(mc_right, center), true, false);
				//ctx.LineTo(new Point(0, left), true, false);
				//ctx.QuadraticBezierTo(new Point(mc_center, center), new Point(mc_right, right), true, false);
				ctx.LineTo(new Point(mc_right, mc_height), true, false);
			}
			geometry.Freeze();
			path.Data = geometry;
			return ans;
		}


		private void OnScaleChanged()
		{
			if (!m_bInit)
				return;
			double scale = Scale;
		}

		float m_CurRightATA = -1;
		float m_CurLeftATA = -1;

		private void OnATAChanged(bool force)
		{
			if (!m_bInit)
				return;

			float l_ata = SSLeftATA;
			float r_ata = SSRightATA;
			if (r_ata != m_CurRightATA)
			{
				m_CurRightATA = r_ata;

				RightLine.X1 = RightLine.X2 = m_ATALineDeg * r_ata + m_ATAAdd;
			}
			if (l_ata != m_CurLeftATA)
			{
				m_CurLeftATA = l_ata;
				LeftLine.X1 = LeftLine.X2 = m_ATALineDeg * l_ata;
			}

		}
		bool m_CurShowLabels = true;
		private void OnShowLabelsChanged()
		{
			if (!m_bInit)
				return;
			bool show = ShowLabels;
			if (show != m_CurShowLabels)
			{
				m_CurShowLabels = show;
				Visibility v = show ? Visibility.Visible : Visibility.Collapsed;
				Numbers.Visibility = v;
			}
		}

	}
}
