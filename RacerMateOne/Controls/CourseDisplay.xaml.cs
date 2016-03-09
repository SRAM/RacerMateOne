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
using System.Diagnostics;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for CourseDisplay.xaml
	/// </summary>
	public partial class CourseDisplay : UserControl
	{
		//===============================================================
		public static DependencyProperty DisableProperty = DependencyProperty.Register("Disable", typeof(bool), typeof(CourseDisplay),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDisableChanged)));

		public bool Disable
		{
			get { return (bool)this.GetValue(DisableProperty); }
			set { this.SetValue(DisableProperty, value); }
		}
		private static void OnDisableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CourseDisplay)d).DisableChanged();
		}
		//===============================================================
		public static DependencyProperty DistanceProperty = DependencyProperty.Register("Distance", typeof(double), typeof(CourseDisplay),
			new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnDistanceChanged)));
		public double Distance
		{
			get { return (double)this.GetValue(DistanceProperty); }
			set { this.SetValue(DistanceProperty, value); }
		}
		private static void OnDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CourseDisplay)d).OnDistance();
		}

		//==============================================================
		public static DependencyProperty RoundedEdgeProperty = DependencyProperty.Register("RoundedEdge", typeof(Brush), typeof(CourseDisplay),
			new FrameworkPropertyMetadata(Brushes.Transparent));
		public Brush RoundedEdge
		{
			get { return (Brush)this.GetValue(RoundedEdgeProperty); }
			set { this.SetValue(RoundedEdgeProperty, value); }
		}
		//===============================================================
		public static DependencyProperty LayerUnitProperty = DependencyProperty.Register("LayerUnit", typeof(double), typeof(CourseDisplay),
			new FrameworkPropertyMetadata((100.0 / 5280) * ConvertConst.MilesToMeters,new PropertyChangedCallback(OnLayerUnitChanged)));
		public double LayerUnit
		{
			get { return (double)this.GetValue(LayerUnitProperty); }
			set { this.SetValue(LayerUnitProperty, value); }
		}
		private static void OnLayerUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CourseDisplay)d).LayerUnitChanged();
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
				m_BikeLoc = m_Course != null ? new Course.Location(m_Course, 0):null;
				if (m_BikeIcon != null)
					m_BikeIcon.Visibility = m_Course == null ? Visibility.Collapsed:Visibility.Visible;
				RedrawCourse();
				DisableChanged();
			}
		}
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

		public CourseDisplay()
		{
			InitializeComponent();
		}


		private void courseDisplay_Loaded(object sender, RoutedEventArgs e)
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
		private void CourseDisplay_Unloaded(object sender, RoutedEventArgs e)
		{
			UpdateOff();
		}
        static double currentdiff = 0.1;
		public void RedrawCourse()
		{
			
            if (!m_bLoaded)
				return;
			double controlwidth = AppWin.Instance.ActualWidth;	// Use this for the width...more than we need but close.

			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			if (m_Course != null && m_Course.Segments.Count > 0)
			{
               // Debug.WriteLine("redraw ");
                double miny = m_Course.MinY;
				double maxy = m_Course.MaxY;
				double diff = maxy - miny;
                //if (currentdiff != diff) Debug.WriteLine("diff =" + diff);
                currentdiff = diff;

				FillBrush.Viewport = new Rect(0, 0, 0.25, LayerUnit / diff);

				using (StreamGeometryContext ctx = geometry.Open())
				{
					/*
					ctx.BeginFigure(new Point(0, 0), true, true );
					Point cp = m_Course.Segments.First.Value.StartPoint;
					ctx.LineTo(cp, true, false);
					foreach (Course.Segment s in m_Course.Segments)
					{
						if (Math.Abs(s.StartPoint.Y - cp.Y) > 0.00001)
							ctx.LineTo(s.StartPoint, true, false);
						cp = s.EndPoint;
						ctx.LineTo(cp, true, false);
					}
					ctx.LineTo(new Point(1, 0), true, false);
					 */

					ctx.BeginFigure(new Point(0, 0), true /* is filled */, true /* is closed */);
					Point cp = m_Course.Segments.First.Value.StartPoint;
					ctx.LineTo(cp, true, false);
					foreach (Course.Segment s in m_Course.Segments)
					{
						Course.SmoothSegment ss = s as Course.SmoothSegment;
						if (ss != null)
						{
							// We have a smooth segment... We can do this one of two ways... add a number of segments 
							Point sp = ss.StartPoint;
							cp = ss.EndPoint;
							double w = (cp.X - sp.X) * controlwidth;
							if (w > ss.Divisions)
								w = ss.Divisions;
							if (w > 1)
							{
								double xadd = (cp.X - sp.X) / w;
								double xnadd = 1.0 / w;
								double xn = xnadd;
								for (sp.X += xadd; xn < 1.0; sp.X += xadd, xn += xnadd)
								{
									sp.Y = ss.GetNormalizedY(xn);
									ctx.LineTo(sp, true, false);
								}
							}
							ctx.LineTo(cp, true, false);
						}
						else
						{
							if (Math.Abs(s.StartPoint.Y - cp.Y) > 0.00001)
								ctx.LineTo(s.StartPoint, true, false);
							cp = s.EndPoint;
							ctx.LineTo(cp, true, false);
						}
					}
					ctx.LineTo(new Point(1, 0), true, false);

				}
			}
			geometry.Freeze();
			CoursePath.Data = geometry;
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

		public double BikeOffsetX { get { return m_BikeOffsetX; }  set { m_BikeOffsetX = value; OnDistance(); } }
		public double BikeOffsetY { get { return m_BikeOffsetY; }  set { m_BikeOffsetY = value; OnDistance(); } }


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
					m_BikeIcon.Visibility = m_Course == null ? Visibility.Collapsed:Visibility.Visible;
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
			double actualwidth = ActualWidth;
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

			TopBar.Visibility = Visibility.Visible;
			TopBar.Margin = new Thickness(n, 0, 0, 0);
			LocBar.Width = w;

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
		//==========================================================
		bool m_Interactive;
		public bool Interactive
		{
			get { return m_Interactive; }
			set
			{
				if (m_Interactive == value)
					return;
				m_Interactive = value;
				if (value)
				{
					MouseDown += new MouseButtonEventHandler(CourseDisplay_MouseDown);
					MouseUp += new MouseButtonEventHandler(CourseDisplay_MouseUp);
					MouseMove += new MouseEventHandler(CourseDisplay_MouseMove);
				}
				else
				{
					MouseDown -= new MouseButtonEventHandler(CourseDisplay_MouseDown);
					MouseUp -= new MouseButtonEventHandler(CourseDisplay_MouseUp);
					MouseMove -= new MouseEventHandler(CourseDisplay_MouseMove);
				}
			}
		}


		public void DMouseDown(MouseButtonEventArgs e)
		{
			if (!m_bMouseDown)
			{
				CaptureMouse();
				m_bMouseDown = true;
				SetPos(e.GetPosition(this).X);
			}
		}
		public void DMouseUp(MouseButtonEventArgs e)
		{
			if (m_bMouseDown)
			{
				ReleaseMouseCapture();
				SetPos(e.GetPosition(this).X);
				m_bMouseDown = false;
			}
		}

		void CourseDisplay_MouseUp(object sender, MouseButtonEventArgs e)
		{
			DMouseUp(e);
		}

		void CourseDisplay_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_bMouseDown)
			{
				SetPos(e.GetPosition(this).X);
			}
		}

		void SetPos(double x)
		{
			double w = ActualWidth;
			if (w > 0 && m_Course != null)
			{
				float distance = (float)(m_Course.TotalX * (x / w));
				if (Distance == distance)
					return;
				Distance = distance;
				RoutedEventArgs args = new RoutedEventArgs(DistanceChangedEvent, distance);
				RaiseEvent(args);
			}
		}

		bool m_bMouseDown;
		void CourseDisplay_MouseDown(object sender, MouseButtonEventArgs e)
		{
			DMouseDown(e);
		}

		public static readonly RoutedEvent DistanceChangedEvent =
			EventManager.RegisterRoutedEvent(
			"DistanceChanged", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CourseDisplay));

		public event RoutedEventHandler DistanceChanged
		{
			add { AddHandler(DistanceChangedEvent, value); }
			remove { RemoveHandler(DistanceChangedEvent, value); }
		}
	}
}
