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
	/// Interaction logic for PolarSpinScan.xaml
	/// </summary>
	public partial class PolarSpinScan : UserControl
	{
		//=========================================================================================================
		public static DependencyProperty ShowLabelsProperty = DependencyProperty.Register("ShowLabels", typeof(bool), typeof(PolarSpinScan),
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
			((PolarSpinScan)d).OnShowLabelsChanged(false);
		}
		//=========================================================================================================
		public static DependencyProperty AltSkinProperty = DependencyProperty.Register("AltSkin", typeof(bool), typeof(PolarSpinScan),
			new FrameworkPropertyMetadata(false, new PropertyChangedCallback(_AltSkinChanged)));
		public bool AltSkin
		{
			get { return (bool)this.GetValue(AltSkinProperty); }
			set
			{
				// Lock in these bars by copying over the stuff.
				this.SetValue(AltSkinProperty, value);
			}
		}
		private static void _AltSkinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((PolarSpinScan)d).OnAltSkinChanged();
		}

		//=========================================================================================================
		public static DependencyProperty SSLeftATAProperty = DependencyProperty.Register("SSLeftATA", typeof(float), typeof(PolarSpinScan),
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
		public static DependencyProperty SSRightATAProperty = DependencyProperty.Register("SSRightATA", typeof(float), typeof(PolarSpinScan),
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
			((PolarSpinScan)d).OnATAChanged(false);
		}
	

		//=========================================================================================================
		static float[] ms_ZeroBars = new float[RM1.BarCount];
		public static DependencyProperty BarsProperty = DependencyProperty.Register("Bars", typeof(float[]), typeof(PolarSpinScan),
			new FrameworkPropertyMetadata(new float[RM1.BarCount], new PropertyChangedCallback(_BarsChanged)));
		public float [] Bars
		{
			get { return (float [])this.GetValue(BarsProperty); }
			set 
			{ 
				// Lock in these bars by copying over the stuff.
				this.SetValue(BarsProperty, value == null ? ms_ZeroBars:value.Clone()); 
			}
		}
		private static void _BarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((PolarSpinScan)d).OnBarsChanged();
		}
		//=========================================================================================================
		public static DependencyProperty AvgBarsProperty = DependencyProperty.Register("AvgBars", typeof(float[]), typeof(PolarSpinScan),
			new FrameworkPropertyMetadata(new float[RM1.BarCount], new PropertyChangedCallback(_AvgBarsChanged)));
		public float[] AvgBars
		{
			get { return (float[])this.GetValue(AvgBarsProperty); }
			set
			{
				// Lock in these AvgBars by copying over the stuff.
				this.SetValue(AvgBarsProperty, value == null ? ms_ZeroBars:value.Clone());
			}
		}
		private static void _AvgBarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((PolarSpinScan)d).OnAvgBarsChanged();
		}
		//=========================================================================================================
		public static DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(PolarSpinScan),
			new FrameworkPropertyMetadata(10.0, new PropertyChangedCallback(_ScaleChanged)));
		public double Scale
		{
			get { return (double)this.GetValue(ScaleProperty); }
			set
			{
				// Lock in these bars by copying over the stuff.
				//this.SetValue(ScaleProperty, value);
			}
		}
		private static void _ScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((PolarSpinScan)d).OnScaleChanged();
		}
		//=========================================================================================================
		public static DependencyProperty MaxForceProperty = DependencyProperty.Register("MaxForce", typeof(double), typeof(PolarSpinScan),
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
		String m_NotifyString;

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
			m_NotifyString = Unit.AddNotify(m_Unit, m_UnitFlags, new Unit.NotifyEvent(OnUnitUpdate));
			OnUnitUpdate(m_Unit,m_UnitFlags);
		}
		void OnUnitUpdate(Unit unit, StatFlags changed)
		{
			if (!m_bInit)
				return;
			if ((changed & StatFlags.Bars) != StatFlags.Zero)
			{
				bool bshown = UpdateBars(d_Path, m_Unit.Statistics.Bars);
				if (bshown != m_bCurBarShown)
				{
					m_bCurBarShown = bshown;
					m_LeftLine.Visibility = m_RightLine.Visibility = bshown ? Visibility.Visible : Visibility.Hidden;
				}
			}
			if (AvgBarsOn && (changed & StatFlags.Bars_Avg) != StatFlags.Zero)
				UpdateBars(d_AvgPath, m_Unit.Statistics.AverageBars);
			if ((changed & (StatFlags.SSLeftATA | StatFlags.SSRightATA)) != StatFlags.Zero)
			{
				SSLeftATA = m_Unit.Statistics.SSLeftATA;
				SSRightATA = m_Unit.Statistics.SSRightATA;
			}
			if (m_bRedoScale)
				RedoScale();
		}

		public bool AvgBarsOn;
			
		bool m_bDelay;
		void DelayedUpdate()
		{
			if (!m_bDelay)
			{
				m_bDelay = true;
				Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
				{
					OnBarsChanged();
					OnAvgBarsChanged();
					m_bDelay = false;
				});
			}
		}


		static double[] xarr = new double[RM1.BarCount * 2];
		static double[] yarr = new double[RM1.BarCount * 2];


		static PolarSpinScan()
		{
			double radians;
			int h = RM1.BarCount;
			double offset = Math.PI / (h * 2);
			for (int i = 0; i < h; i++)
			{
				radians = offset+(Math.PI * i / h) + Math.PI;
				xarr[i] = Math.Sin(radians);
				xarr[i + h] = -xarr[i];
				yarr[i] = -Math.Cos(radians);
				yarr[i + h] = -yarr[i];
			}
		}


		public PolarSpinScan()
		{
			InitializeComponent();
		}

		private Line m_RightLine;
		private Line m_LeftLine;
		private bool m_bInit;

		// private Label[] m_DebugLabels = new Label[RM1.BarCount];

		LineSegment[] m_LineSegmentArr = new LineSegment[RM1.BarCount];
		Point[] m_PointArr = new Point[RM1.BarCount];
		PathGeometry m_PathGeo = new PathGeometry();
		PathFigure m_PathFigure = new PathFigure();

		double m_Radius;
		double m_cx;
		double m_cy;
		List<Ellipse> m_Circles = new List<Ellipse>();

		List<UIElement> m_Labels = new List<UIElement>();

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			Line line;
			int i;

			m_Labels.Add(t_BottomZero);
			m_Labels.Add(t_TopZero);

			m_PathGeo.FillRule = FillRule.Nonzero;
			// Create geometry
			for (i = 0; i < RM1.BarCount; i++)
			{
				Point pt = new Point();
				m_PointArr[0] = pt;
				LineSegment ln = new LineSegment();
				ln.Point = pt;
				m_LineSegmentArr[i] = ln;
				if (i == 0)
					m_PathFigure.StartPoint = pt;
				else
					m_PathFigure.Segments.Add(ln);
			}
			m_PathFigure.Segments.Add(m_LineSegmentArr[0]);
			m_PathGeo.Figures.Add(m_PathFigure);
			m_CurScale = Scale;
			MaxForce = Scale;

			Style style = (Style)FindResource("OffDeg");
			Brush br = (Brush)this.FindResource("Brush_OffDeg");
			Style linestyle = (Style)FindResource("DegLine");
			Style lightlinestyle = (Style)FindResource("LtDegLine");
			double ww = Base.ActualWidth;
			double hh = Base.ActualHeight;
			double ccx = ww / 2.0;
			double ccy = hh / 2.0;
			double textradius =  Math.Min(ccx,ccy) - 13.0;
			double cx, radians, cy;
			double cradius = BaseCircle.ActualWidth * 0.5;
			double s1, c1;
			m_Radius = cradius;
			m_cx = ccx;
			m_cy = ccy;
			double sin, cos;
			String txt;
			double arc = Math.PI / 6;
			// Create the numbers around the edge.
			for (i = 0; i < 6; i++)
			{
				radians = Math.PI * i / 6;
				cos = Math.Cos(radians);
				sin = Math.Sin(radians);
				if (i != 0)
				{
					txt = (180 * i / 6).ToString();
					cx = ccx + sin * textradius;
					cy = ccy - cos * textradius;
					Label label = new Label();
					label.Style = style;
					label.Content = txt;
					label.Margin = new Thickness(cx - 25, cy - 10, 0, 0);
					Base.Children.Add(label);
					m_Labels.Add(label);

					cx = ccx - sin * textradius;
					cy = ccy + cos * textradius;
					if (i == 4)
						cx -= 5; // Fug for this one number
					else if (i == 6)
						cx -= 1;
					label = new Label();
					label.Style = style;
					label.Content = txt;
					label.Margin = new Thickness(cx - 25, cy - 10, 0, 0);
					Base.Children.Add(label);
					m_Labels.Add(label);
				}

				line = new Line();
				line.X1 = ccx + sin * cradius;
				line.Y1 = ccy - cos * cradius;
				line.X2 = ccx - sin * cradius;
				line.Y2 = ccy + cos * cradius;
				line.Style = linestyle;
				EBase.Children.Add(line);

				c1 = cos;
				s1 = sin;

				radians = Math.PI * (i * 2 + 1) / 12 ;
				cos = Math.Cos(radians);
				sin = Math.Sin(radians);

				line = new Line();
				line.X1 = ccx + sin * cradius;
				line.Y1 = ccy - cos * cradius;
				line.X2 = ccx - sin * cradius;
				line.Y2 = ccy + cos * cradius;
				line.Style = lightlinestyle;
				EBase.Children.Add(line);


				radians = Math.PI * (i+1) / 6;
				cos = Math.Cos(radians);
				sin = Math.Sin(radians);
				Path p;
				StreamGeometry geometry;
				Brush bcolor = (Brush)this.FindResource("BarColor" + (i + 1));				
				p = new Path();
				p.Stroke = bcolor;
				p.Fill = Brushes.Transparent;
				p.StrokeThickness = 12;
				geometry = new StreamGeometry();
				using (StreamGeometryContext ctx = geometry.Open())
				{
					ctx.BeginFigure(new Point( ccx,ccy ), false /* is filled */, false /* is closed */);
					ctx.LineTo(new Point(ccx + s1 * cradius, ccy - c1 * cradius), false, false);
					ctx.ArcTo( new Point( ccx + sin * cradius, ccy - cos * cradius ),new Size(cradius,cradius),arc,false,SweepDirection.Clockwise,true,false);
					ctx.LineTo(new Point(ccx - s1 * cradius, ccy + c1 * cradius), false, false);
					ctx.ArcTo(new Point(ccx - sin * cradius, ccy + cos * cradius), new Size(cradius, cradius), arc, false, SweepDirection.Clockwise, true, false);
				}
				geometry.Freeze();
				p.Data = geometry;
				ColorBase.Children.Add(p);

			}


			m_RightLine = new Line();
			m_LeftLine = new Line();
			m_LeftLine.X1 = m_RightLine.X1 = m_LeftLine.X2 = m_RightLine.X2 = ccx;
			m_LeftLine.Y1 = m_RightLine.Y1 = m_LeftLine.Y2 = m_RightLine.Y2 = ccy;
			m_RightLine.Style = m_LeftLine.Style = (Style)FindResource("SSLine");
			Base.Children.Add(m_RightLine);
			Base.Children.Add(m_LeftLine);

			Scale = 10.0;

			/*
			Style debugstyle = (Style)FindResource("Debug");
			for (i = 0; i < RM1.BarCount; i++)
			{
				m_DebugLabels[i] = new Label();
				m_DebugLabels[i].Style = debugstyle;
				//m_DebugLabels[i].Content = "";
				DebugText.Children.Add(m_DebugLabels[i]);
			}
			 */
			m_bInit = true;
			OnScaleChanged();
			OnATAChanged( true );
			OnShowLabelsChanged( true );
			OnAltSkinChanged();
			if (m_Unit != null)
				OnUnitUpdate(m_Unit, m_UnitFlags);
		}

		//bool m_bInBarChange = false;

		bool m_bCurBarShown = true;
		void OnBarsChanged()
		{
			if (!m_bInit)
				return;
			bool bshown = UpdateBars(d_Path, Bars);
			if (bshown != m_bCurBarShown)
			{
				m_bCurBarShown = bshown;
				m_LeftLine.Visibility = m_RightLine.Visibility = bshown ? Visibility.Visible : Visibility.Hidden;
			}
		}
		void OnAvgBarsChanged()
		{
			UpdateBars(d_AvgPath, AvgBars);
		}

		double m_CurScale;
		Int64 m_HoldScaleTime;

		double m_Time;
		Int64 m_LastTicks;

		bool UpdateBars( Path elem, float[] bars )
		{
			if (!m_bInit || bars == null || bars.Length < 24)
				return false;
			
			Int64 ticks = System.DateTime.Now.Ticks;
			double lastframe = (ticks - m_LastTicks) * 0.0000001f;
			m_LastTicks = ticks;

			if (lastframe > 0.5)
				lastframe = 0.5;
			m_Time = m_Time * 0.9 + lastframe * 0.1;
			if (m_Time == 0.0)
				m_Time = 0.001;
			double fps = 1.0 / m_Time;
			FPS.Text = String.Format("{0:F1}", fps);

			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			bool shown = false; 
			double max;
			double scale = Scale;
			using (StreamGeometryContext ctx = geometry.Open())
			{
				double mul = m_Radius / scale;
				int i, c;
				double b = bars[0];
				double nb, hb;

				max = b;
				if (b > 0.08)
					shown = true;
				b *= mul;
				double bstart = b;
				ctx.BeginFigure(new Point(m_cx + xarr[0] * b, m_cy + yarr[0] * b), true /* is filled */, true /* is closed */);
				for (i = 1, c = 1; i < RM1.BarCount; i++, b = nb)
				{
					nb = bars[i];
					if (nb > max)
						max = nb;
					if (nb > 0.08)
						shown = true;
					nb *= mul;
					hb = (b + nb) * 0.5;
					if (nb > m_Radius)
						nb = m_Radius;
					if (hb > m_Radius)
						hb = m_Radius;
					ctx.LineTo(new Point(m_cx + xarr[c] * hb, m_cy + yarr[c] * hb), true, false);
					c++;
					ctx.LineTo(new Point(m_cx + xarr[c] * nb, m_cy + yarr[c] * nb), true, false);
					c++;
				}
				hb = (b + bstart) * 0.5f;
				ctx.LineTo(new Point(m_cx + xarr[c] * hb, m_cy + yarr[c] * hb), true, false);
			}
			geometry.Freeze();
			if (shown)
			{
				elem.Data = geometry;
				elem.Visibility = Visibility.Visible;
			}
			else
				elem.Visibility = Visibility.Hidden;


			double nscale = (Math.Round(max / 5.0) + 1) * 5.0;
			if (m_DebugScale > nscale)
				nscale = m_DebugScale;
			if (nscale < 10.0)
				nscale = 10.0;

			if (m_CurScale != nscale)
			{
				// 10000000
				if (nscale < m_CurScale)
				{
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
					//m_bInBarChange = true;
					ChangeScaleAnim.To = nscale;
					ChangeScale.Begin();
					//m_bInBarChange = false;
				}
				MaxForce = m_CurScale;
			}

			/*
			String s = "";
			for (int j = 0; j < 24; j++)
				s += String.Format("{0:F3},", bars[j]);
			Log.WriteLine(s);
			*/
			return shown;
		}

		void OnScaleChanged()
		{
			if (!m_bInit)
				return;
			m_bRedoScale = true;
			Unit.ForceNotify(m_NotifyString);
		}

		bool m_bRedoScale;
		void RedoScale()
		{
			m_bRedoScale = false;
			double scale = Scale;
			debug_Scale.Text = scale.ToString();
			double mv = m_Radius * 5.0 / scale;
			double acc = m_cx - mv;
			int cnt = 0;
			Ellipse circle;
			while (acc > 28.0)
			{
				if (cnt == m_Circles.Count)
				{
					circle = new Ellipse();
					circle.Style = (Style)FindResource(
						AltSkin ? m_Circles.Count == 0 ? "CenterCircleStyle_Alt":"CircleStyle_Alt":m_Circles.Count == 0 ? "CenterCircleStyle":"CircleStyle");
					m_Circles.Add(circle);
					CircleBase.Children.Insert(0,circle);
				}
				else
					circle = m_Circles[cnt];
				circle.Margin = new Thickness(acc);
				circle.Visibility = Visibility.Visible;
				cnt++;
				acc -= mv;
			}
			while (cnt < m_Circles.Count)
			{
				m_Circles[cnt].Visibility = Visibility.Hidden;
				cnt++;
			}
		}

		private void ChangeScaleAnim_Completed(object sender, EventArgs e)
		{

		}

		float m_DebugScale = 0.0f;
		private void DebugP_Click(object sender, RoutedEventArgs e)
		{
			if (m_DebugScale < 50.0f)
				m_DebugScale += 2.5f;
			OnBarsChanged();
		}

		private void DebugM_Click(object sender, RoutedEventArgs e)
		{
			if (m_DebugScale > 0.0f)
				m_DebugScale -= 2.5f;
			OnBarsChanged();
		}

		double DegToRadians( double d )
		{
			return d * Math.PI / 180.0;
		}

		float m_CurLeftATA = -1.0f;
		float m_CurRightATA = -1.0f;
		private void OnATAChanged(bool force)
		{
			if (!m_bInit)
				return;
			double radians;
			float l_ata = SSLeftATA;
			float r_ata = SSRightATA;
			if (r_ata != m_CurRightATA)
			{
				m_CurRightATA = r_ata;
				radians = DegToRadians(r_ata) + Math.PI;
				m_RightLine.X2 = m_Radius * Math.Sin(radians) + m_cx;
				m_RightLine.Y2 = m_Radius * -Math.Cos(radians) + m_cy;
			}
			if (l_ata != m_CurLeftATA)
			{
				m_CurLeftATA = l_ata;
				radians = DegToRadians(l_ata) + Math.PI;
				m_LeftLine.X2 = m_Radius * -Math.Sin(radians) + m_cx;
				m_LeftLine.Y2 = m_Radius * Math.Cos(radians) + m_cy;
			}

		}

		bool m_CurShowLabels = true;
		private void OnShowLabelsChanged( bool force )
		{
			if (!m_bInit)
				return;
			bool show = ShowLabels;
			if (show != m_CurShowLabels || force)
			{
				m_CurShowLabels = show;
				Visibility v = show ? Visibility.Visible : Visibility.Hidden;
				foreach(UIElement e in m_Labels)
					e.Visibility = v;
			}
		}

		bool m_CurAltSkin = false;
		private void OnAltSkinChanged()
		{
			if (!m_bInit)
				return;
			bool altskin = AltSkin;
			if (altskin != m_CurAltSkin)
			{
				m_CurAltSkin = altskin;
				for (int i = 0; i < m_Circles.Count; i++)
				{
					Ellipse circle = m_Circles[i];
					circle.Style = (Style)FindResource(
						AltSkin ? i == 0 ? "CenterCircleStyle_Alt" : "CircleStyle_Alt" : i == 0 ? "CenterCircleStyle" : "CircleStyle");
				}
			}
			Visibility v = altskin ? Visibility.Hidden : Visibility.Visible;
			EBase.Visibility = v;
			//CircleBase.Visibility = v;
			ColorBase.Visibility = v;
			EdgeCircle.Visibility = v;

			BaseCircle.Style = (Style)FindResource(altskin ? "Border3D" : "CircleStyle");
			d_Path.Style = (Style)FindResource(AltSkin ? "PathStyle_Alt" : "PathStyle");
		}

	}
}
