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
	/// Interaction logic for CourseDisplayEx.xaml
	/// </summary>
	public partial class CourseDisplayEx : UserControl
	{
		//===============================================================
		public static DependencyProperty DisableProperty = DependencyProperty.Register("Disable", typeof(bool), typeof(CourseDisplayEx),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDisableChanged)));

		public bool Disable
		{
			get { return (bool)this.GetValue(DisableProperty); }
			set { this.SetValue(DisableProperty, value); }
		}
		private static void OnDisableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CourseDisplayEx)d).DisableChanged();
		}
		//===============================================================
		public static DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(float), typeof(CourseDisplayEx),
			new FrameworkPropertyMetadata(0.0f, new PropertyChangedCallback(OnDistanceChanged)));
		public float Distance
		{
			get { return (float)this.GetValue(DistanceProperty); }
			set { this.SetValue(DistanceProperty, value); }
		}
		private static void OnDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CourseDisplayEx)d).OnDistance();
		}

		//==============================================================
		public static DependencyProperty RoundedEdgeProperty = DependencyProperty.Register("RoundedEdge", typeof(Brush), typeof(CourseDisplayEx),
			new FrameworkPropertyMetadata(Brushes.Transparent));
		public Brush RoundedEdge
		{
			get { return (Brush)this.GetValue(RoundedEdgeProperty); }
			set { this.SetValue(RoundedEdgeProperty, value); }
		}
		//===============================================================
		public static DependencyProperty LayerUnitProperty = DependencyProperty.Register("LayerUnit", typeof(double), typeof(CourseDisplayEx),
			new FrameworkPropertyMetadata((100.0 / 5280) * ConvertConst.MilesToMeters, new PropertyChangedCallback(OnLayerUnitChanged)));
		public double LayerUnit
		{
			get { return (double)this.GetValue(LayerUnitProperty); }
			set { this.SetValue(LayerUnitProperty, value); }
		}
		private static void OnLayerUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CourseDisplayEx)d).LayerUnitChanged();
		}
		void LayerUnitChanged()
		{
			if (m_bLoaded)
				RedrawCourse();
		}
		//==============================================================
		protected Course m_Course;
		public Course Course
		{
			get { return m_Course; }
			set
			{
				m_Course = value;
				Disable = m_Course == null;
				m_BikeLoc = m_Course != null ? new Course.Location(m_Course, 0) : null;
				if (m_BikeIcon != null)
					m_BikeIcon.Visibility = m_Course == null ? Visibility.Collapsed : Visibility.Visible;
				RedrawCourse();
				DisableChanged();
			}
		}
		//==============================================================
		//=========================================================================================================
		protected Unit m_Unit;
		protected StatFlags m_UnitFlags = StatFlags.LapDistance | StatFlags.Course;
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
		}
		void UpdateOn()
		{ 
			if (m_bUpdateOn || m_Unit == null || !m_bLoaded)
				return;
			m_bUpdateOn = true;
			Unit.AddNotify(m_Unit, m_UnitFlags, new Unit.NotifyEvent(OnUnitUpdate));
			OnUnitUpdate(m_Unit, m_UnitFlags);
		}
		void OnUnitUpdate(Unit unit, StatFlags changed)
		{
			if (!m_bLoaded)
				return;
			if ((changed & StatFlags.Course) != StatFlags.Zero)
			{
				Course = null;
				Course = Unit.Course;
			}
			if ((changed & StatFlags.LapDistance) != StatFlags.Zero)
			{
				Distance = (float)unit.Statistics.LapDistance;
			}
		}
		//=========================================================================================================



		protected Course.Location m_BikeLoc;

		bool m_bLoaded = false;

		public CourseDisplayEx()
		{
			InitializeComponent();
		}


		private void CourseDisplayEx_Loaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;

			m_LocBarWidth = LocBar.Width;
			m_bLoaded = true;
			RedrawCourse();
			DisableChanged();
			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				UpdateOn();
			});
		}
		private void CourseDisplayEx_Unloaded(object sender, RoutedEventArgs e)
		{
			if (m_bInRedraw)
				AppWin.RemoveRenderUpdate(new AppWin.RenderUpdate(DoRedraw));
			UpdateOff();
		}

		bool m_bInRedraw;
		bool DoRedraw(double seconds, double split)
		{
			double w = CourseBox.ActualWidth;
			double h = CourseBox.ActualHeight;
			if (w == 0 && h == 0)
				return false; // Keep doing this.
			RedrawCourse();
			m_bInRedraw = false;
			return true; // Done - remove itself
		}

		public void RedrawCourse()
		{
			if (!m_bLoaded && IsVisible)
				return;
			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			double w = CourseBox.ActualWidth;
			double h = CourseBox.ActualHeight;

			if (w == 0 && h == 0)
			{
				if (!m_bInRedraw)
				{
					m_bInRedraw = true;
					AppWin.AddRenderUpdate(new AppWin.RenderUpdate(DoRedraw), 0);
				}
				return;
				
			}

			CoursePath.Width = w;
			CoursePath.Height = h;
			LocPath.Width = w;
			LocPath.Height = h;
			if (m_Course != null && m_Course.Segments.Count > 0)
			{
				double miny = m_Course.MinY;
				double maxy = m_Course.MaxY;
				double diff = maxy - miny;


				using (StreamGeometryContext ctx = geometry.Open())
				{
					ctx.BeginFigure(new Point(0, h), true, true);
					Point cp = m_Course.Segments.First.Value.StartPoint;
					Point sp = new Point(cp.X * w, cp.Y * h);
					ctx.LineTo(sp, true, false);
					foreach (Course.Segment s in m_Course.Segments)
					{
						Course.SmoothSegment ss = s as Course.SmoothSegment;
						if (ss != null)
						{
							sp.X = ss.StartPoint.X * w;
							sp.Y = ss.StartPoint.Y * h;
							cp.X = ss.EndPoint.X * w;
							cp.Y = ss.EndPoint.Y * h;
							double ww = (cp.X - sp.X);
							if (ww > ss.Divisions)
								ww = ss.Divisions;
							if (ww > 1)
							{
								double xadd = (cp.X - sp.X) / ww;
								double xnadd = 1.0 / ww;
								double xn = xnadd;
								for (sp.X += xadd; xn < 1.0; sp.X += xadd, xn += xnadd)
								{
									sp.Y = ss.GetNormalizedY(xn) * h;
									ctx.LineTo(sp, true, false);
								}
							}
							ctx.LineTo(cp, true, false);
						}
						else
						{
							if (Math.Abs(s.StartPoint.Y - cp.Y) > 0.00001)
							{
								sp.X = s.StartPoint.X * w;
								sp.Y = s.StartPoint.Y * h;
								ctx.LineTo(sp, true, false);
							}
							cp = s.EndPoint;
							sp.X = cp.X * w;
							sp.Y = cp.Y * h;
							ctx.LineTo(sp, true, false);
						}
					}
					ctx.LineTo(new Point(w, h), true, false);
				}
			}
			geometry.Freeze();
			CoursePath.Data = geometry;
			LocPath.Data = geometry;
			OnDistance();
		}

		double m_LocBarWidth;

		public double BikeX;
		public double BikeY;

		public double Change
		{
			get
			{
				return m_BikeLoc == null || m_BikeLoc.Segment == null ? 0.0 : m_BikeLoc.Segment.Change;
			}
		}
		public double Wind
		{
			get
			{
				if (m_BikeLoc != null && m_BikeLoc.Segment != null)
				{
					Course.PysicalSegment ps = m_BikeLoc.Segment as Course.PysicalSegment;
					if (ps != null)
						return ps.Wind;
				}
				return 0.0;
			}
		}
		public double Y
		{
			get
			{
				if (m_BikeLoc != null && m_BikeLoc.Segment != null)
				{
					return m_BikeLoc.Y;
				}
				return 0.0;
			}
		}

		double m_BikeOffsetX;
		double m_BikeOffsetY;

		public double BikeOffsetX { get { return m_BikeOffsetX; } set { m_BikeOffsetX = value; OnDistance(); } }
		public double BikeOffsetY { get { return m_BikeOffsetY; } set { m_BikeOffsetY = value; OnDistance(); } }


		Image m_BikeIcon;
		Point m_BikeBase;
		public Image BikeIcon
		{
			get { return m_BikeIcon; }
			set
			{
				m_BikeIcon = value;
				if (m_BikeIcon != null)
				{
					m_BikeBase.X = m_BikeIcon.Margin.Left;
					m_BikeBase.Y = m_BikeIcon.Margin.Top;
					m_BikeIcon.Visibility = m_Course == null ? Visibility.Collapsed : Visibility.Visible;
				}
				OnDistance();
			}
		}

		public void OnDistance()
		{
			if (!m_bLoaded || m_Course == null)
				return;

			double distance = (float)Distance;
			// ActualWidth;
			double actualwidth = CourseBox.ActualWidth;
			double n = actualwidth * distance / m_Course.TotalX;
			BikeX = n;

			double w = m_LocBarWidth;
			n -= m_LocBarWidth * 0.5;
			if (n < 0)
			{
				w += n;
				n = 0;
			}
			if ((n + w) > actualwidth)
			{
				w = actualwidth - n;
			}
			if (w <= 0)
			{
				LocBar.Visibility = Visibility.Collapsed;
				return;
			}
			LocBar.Visibility = Visibility.Visible;
			LocBar.Margin = new Thickness(n, 0, 0, 0);
			LocBar.Width = w;
			LocPath.Margin = new Thickness(-n, 0, 0, 0);

			TopBar.Visibility = Visibility.Visible;
			TopBar.Margin = new Thickness(n, 0, 0, 0);
			TopBar.Width = w;


			m_BikeLoc.Normalized = distance / m_Course.TotalX;

			BikeY = m_BikeLoc.NormalizedY * CourseBox.ActualHeight + CourseBox.Margin.Top;

			if (m_BikeIcon != null)
			{
				double ww = BikeIcon.ActualWidth;
				double hh = BikeIcon.ActualHeight;
				m_BikeIcon.Margin = new Thickness(m_BikeBase.X + BikeX + BikeOffsetX, m_BikeBase.Y + BikeY + BikeOffsetY, 0, 0);
			}
		}

		private void DisableChanged()
		{
			if (!m_bLoaded)
				return;

			if (Disable)
			{
				Disable_Box.Visibility = Visibility.Visible;
				LocBar.Visibility = Visibility.Visible;
				TopBar.Visibility = Visibility.Visible;
			}
			else
			{
				Disable_Box.Visibility = Visibility.Collapsed;
				LocBar.Visibility = Visibility.Collapsed;
				TopBar.Visibility = Visibility.Collapsed;
			}
		}


	}
}
