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
	/// Interaction logic for RaceCourseDisplayOld.xaml
	/// </summary>
	public partial class RaceCourseDisplayOld : UserControl
	{
		//============================================
		public static DependencyProperty AltProperty = DependencyProperty.Register("Alt", typeof(bool), typeof(RaceCourseDisplayOld),
			new FrameworkPropertyMetadata(true, new PropertyChangedCallback(_AltChanged)));
		public bool Alt
		{
			get { return (bool)this.GetValue(AltProperty); }
			set { this.SetValue(AltProperty, value); }
		}
		private static void _AltChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RaceCourseDisplayOld)d).OnAltChanged(false);
		}
		//============================================

		private Course m_Course;
		private Course.Location[] m_Locs = new Course.Location[8];

		public static SolidColorBrush ms_Other_Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99000000"));

		public Course Course
		{
			get { return m_Course; }
			set
			{
				if (m_Course == value)
					return;
				m_Course = value;
				for (int i = 0; i < 8; i++)
					m_Locs[i] = m_Course == null ? null:new Course.Location(m_Course, 0);

				RedrawCourse();
			}
		}

		void OnAltChanged(bool force)
		{
			if (!m_bLoaded)
				return;
			bool alt = Alt;
			Brush bkbrush;
			Brush fgbrush;
			if (alt)
			{
				CoursePath.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99000000"));
				CoursePath.StrokeThickness = 1;
				CoursePath.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99999999"));
				bkbrush = Brushes.White;
				fgbrush = Brushes.Black;
				OffBrush = Brushes.White;
				ActiveBrush = Brushes.Red;
			}
			else
			{
				CoursePath.Fill = FillBrush;
				CoursePath.StrokeThickness = 0;
				CoursePath.Stroke = Brushes.Transparent;
				bkbrush = AppWin.StdBrush_Background;
				fgbrush = Brushes.White;
				OffBrush = bkbrush;
				ActiveBrush = Brushes.Red;
			}
			foreach( Grid g in m_Markers )
			{
				((Ellipse)g.Children[0]).Fill = bkbrush;
				((Label)g.Children[1]).Foreground = fgbrush;
			}
			int active = CurActive;
			CurActive = -1;
			CurActive = active; // Reset the active colors.
		}	


		Grid[] m_Markers = new Grid[8];
		Rectangle[] m_Rects = new Rectangle[8];

		bool m_bLoaded;
		public RaceCourseDisplayOld()
		{
			InitializeComponent();
			m_Markers[0] = r1;
			m_Markers[1] = r2;
			m_Markers[2] = r3;
			m_Markers[3] = r4;
			m_Markers[4] = r5;
			m_Markers[5] = r6;
			m_Markers[6] = r7;
			m_Markers[7] = r8;

			m_Rects[0] = b1;
			m_Rects[1] = b2;
			m_Rects[2] = b3;
			m_Rects[3] = b4;
			m_Rects[4] = b5;
			m_Rects[5] = b6;
			m_Rects[6] = b7;
			m_Rects[7] = b8;
		}
		private void RaceCourseDisplayOld_Loaded(object sender, RoutedEventArgs e)
		{
			m_bLoaded = true;
			OnAltChanged(true);
			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				if (IsVisible)
					UpdateOn();
				RedrawCourse();
			});

		}

		bool m_bUpdating = false;
		void UpdateOn()
		{
			if (!m_bLoaded || m_bUpdating)
				return;
			m_bUpdating = true;
			Unit.OnUpdate += new RM1.UpdateEvent(Update);
		}
		void UpdateOff()
		{
			if (m_bUpdating)
			{
				m_bUpdating = false;
				Unit.OnUpdate -= new RM1.UpdateEvent(Update);
			}
		}

		private int m_CurActive; // The currently displayed active unit.
		private int CurActive
		{
			get { return m_CurActive; }
			set
			{
				if (m_CurActive == value)
					return;
				if (m_CurActive >= 0)
					m_Rects[m_CurActive].Fill = ((Ellipse)(m_Markers[m_CurActive].Children[0])).Fill = OffBrush;
				m_CurActive = value;
				if (m_CurActive >= 0)
					m_Rects[m_CurActive].Fill = ((Ellipse)(m_Markers[m_CurActive].Children[0])).Fill = ActiveBrush;
			}
		}

		Brush OffBrush = Brushes.White;
		Brush ActiveBrush = Brushes.Red;





		private int m_CurCount;
		private static double[] ms_dxtemp = new double[8];
		private void Update(double splittime)
		{
			int i;
			int cnt = Course != null ? Unit.Active.Count:0;
			if (cnt != m_CurCount)
			{
				for (i = 0; i < cnt; i++)
					m_Rects[i].Visibility = m_Markers[i].Visibility = Visibility.Visible;
				for (; i < 8; i++)
					m_Rects[i].Visibility = m_Markers[i].Visibility = Visibility.Hidden;
				m_CurCount = cnt;
			}
			if (cnt == 0)
				return;
			double w = CourseBox.ActualWidth;
			double h = CourseBox.ActualHeight;
			double m = w / Course.TotalX;
			double dx,lastx,x,y,d;
			double skip = 8;
			double maxy = h - cnt * skip;
			if (maxy < 8)
			{
				skip = (h - 8) / cnt;
				maxy = h - cnt * skip;
			}
			lastx = y = 0;
			int active = -1;
			Unit unit;
			for (i = 0; i < cnt; i++)
			{
				unit = Unit.Active[i];
				if (unit == m_ActiveUnit)
					active = i;
				d = unit.Statistics.LapDistance;
				x = d * m;
				if (i > 0)
				{
					dx = Math.Abs(lastx - x);				
					if (dx < 16)
					{
						y += 16 - dx;
						if (y > maxy + i * skip)
							y = maxy + i * skip;
					}
					else
						y = 0;
				}
				m_Locs[i].Normalized = x / w;

				m_Markers[i].Margin = new Thickness(x, y, 0, 0);
				m_Rects[i].Margin = new Thickness(x + (8 - 2.5), 0, 0, 2);
				m_Rects[i].Height = (1.0 - m_Locs[i].NormalizedY) * h;
				lastx = x;
			}
			CurActive = active;
		}


		public void RedrawCourse()
		{
			if (!m_bLoaded)
				return;
			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			double w = CourseBox.ActualWidth;
			double h = CourseBox.ActualHeight;
			CoursePath.Width = w;
			CoursePath.Height = h;
			if (m_Course != null && m_Course.Segments.Count > 0)
			{
				double miny = m_Course.MinY;
				double maxy = m_Course.MaxY;
				double diff = maxy - miny;


				using (StreamGeometryContext ctx = geometry.Open())
				{
					ctx.BeginFigure(new Point(0, h), true /* is filled */, true /* is closed */);
					Point cp = m_Course.Segments.First.Value.StartPoint;
					Point sp = new Point(cp.X * w, cp.Y * h);
					ctx.LineTo(sp, true, false);
					foreach (Course.Segment s in m_Course.Segments)
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
					ctx.LineTo(new Point(w, h), true, false);
				}
			}
			geometry.Freeze();
			CoursePath.Data = geometry;

		}

		Unit m_ActiveUnit;
		public Unit ActiveUnit
		{
			get { return m_ActiveUnit; }
			set
			{
				if (m_ActiveUnit == value)
					return;
				m_ActiveUnit = value;
				int active = -1;
				int i,cnt = Course != null ? Unit.Active.Count : 0;
				for (i = 0; i < cnt; i++)
				{
					if (Unit.Active[i] == m_ActiveUnit)
					{
						active = i;
						break;
					}
				}
				CurActive = active;
			}
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (m_bLoaded)
			{
				RedrawCourse();
				Update(0);
			}
		}

		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
				UpdateOn();
			else
				UpdateOff();
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			UpdateOff();
		}
	}
}
