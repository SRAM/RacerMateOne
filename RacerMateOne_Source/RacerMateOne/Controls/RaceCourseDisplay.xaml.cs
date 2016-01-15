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
	/// Interaction logic for RaceCourseDisplay.xaml
	/// </summary>
	public partial class RaceCourseDisplay : UserControl
	{
		public const int ms_Height = 18;

		StatFlags m_StatFlags = StatFlags.RiderName | StatFlags.Distance | StatFlags.LapDistance | StatFlags.Lap | StatFlags.Drafting | StatFlags.Order | StatFlags.Finished | StatFlags.Lead;

		//============================================
		// Resources
		//============================================
		static bool ms_ResourcesInit = false;

		static Brush[] ms_UpLine = new Brush[8];
		static Brush[] ms_DownLine = new Brush[8];
		static Brush[] ms_HLine = new Brush[8];
		static Brush[] ms_CircleBrush = new Brush[8];

		static Style ms_Style_NameLabel;
		static Style ms_Style_NameLabelPacer;
		static Style ms_Style_NameText;

		static Style ms_Style_Circle;
		static Style ms_Style_CircleText;

		static Style ms_Style_VLine;
		static Style ms_Style_HLine;

		static Style ms_Style_BBox;
		static Style ms_Style_BBoxLabel;

		static DoubleCollection ms_Dashed;

		static Brush ms_Name_Std;
		static Brush ms_Name_Bot;
		static Brush ms_Name_Std_Draft;
		static Brush ms_Name_Bot_Draft;
		static Brush ms_Name_Active;

		static void InitResources(RaceCourseDisplay ca)
		{
			if (ms_ResourcesInit)
				return;
			for(int i=0;i<8;i++)
			{
				int num = i+1;
				ms_HLine[i] = (Brush)ca.FindResource("H_" + num);
				ms_UpLine[i] = (Brush)ca.FindResource("Up_" + num);
				ms_DownLine[i] = (Brush)ca.FindResource("Down_" + num);
				ms_CircleBrush[i] = (Brush)ca.FindResource("Circle_" + num);
			}

			ms_Style_NameLabel = (Style)ca.FindResource("NameLabel");
			ms_Style_NameLabelPacer = (Style)ca.FindResource("NameLabelPacer");
			ms_Style_NameText = (Style)ca.FindResource("NameText");

			ms_Style_Circle = (Style)ca.FindResource("Circle");
			ms_Style_CircleText = (Style)ca.FindResource("CircleText");

			ms_Name_Std = (Brush)ca.FindResource("Name_Std");
			ms_Name_Bot = (Brush)ca.FindResource("Name_Bot");
			ms_Name_Std_Draft = (Brush)ca.FindResource("Name_Std_Draft");
			ms_Name_Bot_Draft = (Brush)ca.FindResource("Name_Bot_Draft");
			ms_Name_Active = (Brush)ca.FindResource("Name_Active");

			ms_Dashed = new DoubleCollection();
			ms_Dashed.Add(3);
			ms_Dashed.Add(2);

			ms_Style_VLine = (Style)ca.FindResource("VLine");
			ms_Style_HLine = (Style)ca.FindResource("HLine");

			ms_Style_BBox = (Style)ca.FindResource("BBox");
			ms_Style_BBoxLabel = (Style)ca.FindResource("BBoxLabel");

			ms_ResourcesInit = true;
		}

		//============================================

		class Node  {
			const double c_LeftSide = 60;
			Unit m_Unit;
			RaceCourseDisplay m_Parent;
			Label m_NameLabel;
			TextBlock m_NameText;

			Label m_PlaceLabel;
			Label m_LapLabel;

			Grid m_Circle;

			Line m_HLine;
			Line m_VLine;

			Border m_BBox;
			Label m_BBoxLabel;

			Thickness m_BBoxLoc;

			int m_Num;

			public Course.Location m_Loc;

			// Off,Left,Right,Message
			// Must be off to go to the waiting state.

		
			enum States
			{
				Off,
				Message,
				Left,
				Right
			}
			States m_State = States.Off;
			States m_Side = States.Off;

			bool m_Fade = false;
			double m_CurFade = 0;

			Ellipse m_Ellipse;
			Label m_EllipseLabel;
			


			public Node(RaceCourseDisplay cd, int num)
			{
				m_Num = num;
				m_Parent = cd;
				m_Unit = null;
				m_NameLabel = new Label();
				//m_NameLabel.Background = Brushes.Green;

				m_NameLabel.Style = RaceCourseDisplay.ms_Style_NameLabel;

				m_NameText = new TextBlock();
				m_NameText.Style = RaceCourseDisplay.ms_Style_NameText;
				m_NameLabel.Content = m_NameText;
				m_Parent.Names.Children.Add(m_NameLabel);

				m_PlaceLabel = new Label();
				m_PlaceLabel.Style = RaceCourseDisplay.ms_Style_NameLabel;
				m_PlaceLabel.Content = "1";
				m_Parent.Place.Children.Add(m_PlaceLabel);

				m_LapLabel = new Label();
				m_LapLabel.Style = RaceCourseDisplay.ms_Style_NameLabel;
				m_PlaceLabel.Content = "1";
				m_Parent.LapsColumn.Children.Add(m_LapLabel);


				m_Circle = new Grid();
				//m_Circle.Background = Brushes.Red;
				m_Circle.Height = RaceCourseDisplay.ms_Height;
				Ellipse e = new Ellipse();
				e.Style = RaceCourseDisplay.ms_Style_Circle;
				e.Fill = ms_CircleBrush[num];
				m_Ellipse = e;
				m_Circle.Children.Add(e);
				Label l = new Label();
				l.Style = RaceCourseDisplay.ms_Style_CircleText;
				l.Content = (num+1).ToString();
				m_EllipseLabel = l;
				m_Circle.Children.Add(l);

				m_HLine = new Line();
				m_HLine.Style = RaceCourseDisplay.ms_Style_HLine;
				m_HLine.Stroke = RaceCourseDisplay.ms_HLine[num];
				m_Parent.Layer_HLine.Children.Add(m_HLine);

				m_VLine = new Line();
				m_VLine.Style = RaceCourseDisplay.ms_Style_VLine;
				m_VLine.Stroke = RaceCourseDisplay.ms_UpLine[num];
				m_Parent.Layer_VLine.Children.Add(m_VLine);

				m_BBox = new Border();
				m_BBox.Opacity = 0;
				m_BBox.Style = RaceCourseDisplay.ms_Style_BBox;
				m_Parent.Layer_Text.Children.Add(m_BBox);

				m_BBoxLabel = new Label();
				m_BBoxLabel.Style = RaceCourseDisplay.ms_Style_BBoxLabel;
				m_BBox.Child = m_BBoxLabel;

				m_Parent.Position.Children.Add(m_Circle);

				m_BBoxLoc = new Thickness(0, 0, 0, 0);
				
				m_Loc = new Course.Location(m_Parent.m_Course, 0);
				m_Active = false;

				m_NameLabel.Foreground = RaceCourseDisplay.ms_Name_Std;
			}
			bool m_Active;
			public bool Active
			{
				get { return m_Active; }
				set
				{
					if (m_Active == value)
						return;
					m_Active = value;
					StartEveryFrameUpdate();
					UpdateActive();
				}
			}


			object m_Message;
			public object Message
			{
				get { return m_Message; }
				set
				{
					if (value == m_Message)
						return;
					m_Message = value;
					if (m_State != States.Message)
					{
						m_Fade = false;
					}
					else
						m_BBoxLabel.Content = m_Message;
					Update(StatFlags.LapDistance);
					StartEveryFrameUpdate();
				}
			}

			bool m_bEveryFrameUpdate;
			void StartEveryFrameUpdate()
			{
				if (!m_bEveryFrameUpdate)
				{
					AppWin.AddRenderUpdate(new AppWin.RenderUpdate(EveryFrameUpdate), 0);
					m_bEveryFrameUpdate = true;
				}
			}
			const double c_FadeSpeed = 2;
			public bool EveryFrameUpdate(double seconds, double split)
			{
				if (m_Fade)
				{
					if (m_CurFade < 1)
					{
						m_CurFade += split * c_FadeSpeed;
						if (m_CurFade > 1)
							m_CurFade = 1;
						m_BBox.Opacity = m_CurFade;
					}
					if (m_CurFade == 1)
					{
						m_bEveryFrameUpdate = false;
						return true;
					}
				}
				else
				{
					if (m_CurFade > 0)
					{
						m_CurFade -= split * c_FadeSpeed;
						if (m_CurFade < 0)
							m_CurFade = 0;
						m_BBox.Opacity = m_CurFade;
					}
					if (m_CurFade == 0)
					{
						// See what mode we should be in.
						if (m_Message != null)
						{
							m_State = States.Message;
							m_BBoxLabel.Content = m_Message;
							m_Fade = true;
							Update(StatFlags.LapDistance);
						}
						else if (m_Side != States.Off)
						{
							m_State = m_Side;
							m_Fade = true;
							Update(StatFlags.LapDistance);
						}
						else
						{
							m_State = States.Off;
							m_bEveryFrameUpdate = false;
							return true;
						}
					}
				}
				return false; // Do this again.	
			}


			void UpdateActive()
			{
				if (Unit != null)
					m_NameLabel.Foreground = m_Active ? RaceCourseDisplay.ms_Name_Active : 
						Unit.IsBot ? Unit.Statistics.Drafting ?  RaceCourseDisplay.ms_Name_Bot_Draft:RaceCourseDisplay.ms_Name_Bot :
						Unit.Statistics.Drafting ? RaceCourseDisplay.ms_Name_Std_Draft : RaceCourseDisplay.ms_Name_Std;
			}
			public Unit Unit
			{
				get { return m_Unit; }
				set
				{
					m_Unit = value;
					Visibility v = value == null ? Visibility.Collapsed : Visibility.Visible;
					m_HLine.Visibility = m_PlaceLabel.Visibility = m_Circle.Visibility = m_NameLabel.Visibility = v;
					m_LapLabel.Visibility = value != null && Unit.Laps > 1 ? Visibility.Visible : Visibility.Collapsed;
					if (value != null)
					{
						int num = value.Number;
						m_Ellipse.Fill = ms_CircleBrush[num];
						m_EllipseLabel.Content = (num + 1).ToString();

						m_HLine.StrokeDashArray = Unit.IsBot ? RaceCourseDisplay.ms_Dashed : null;
						double y = (m_Parent.CourseBox.ActualHeight - (RaceCourseDisplay.ms_Height * Unit.RaceUnit.Count)) + ms_Height * m_Num + RaceCourseDisplay.ms_Height * 0.5;
						m_VLine.Y1 = m_HLine.Y1 = m_HLine.Y2 = y;
						m_BBoxLoc.Top = y - 8;
						// Show everything and do a full update
						Update(m_Parent.m_StatFlags);
						StartEveryFrameUpdate();
					}
				}
			}

			/*****************************************************************************************************

			*****************************************************************************************************/

			public void Update(StatFlags flags)  {
				if (Unit == null) {
					return;
				}

				if ((flags & StatFlags.RiderName) != StatFlags.Zero)  {
					m_NameText.Text = Unit.Statistics.RiderName;
					m_NameLabel.Style = Unit.IsBot ? RaceCourseDisplay.ms_Style_NameLabelPacer : RaceCourseDisplay.ms_Style_NameLabel;
					UpdateActive();
				}
				else if ((flags & StatFlags.Drafting) != StatFlags.Zero) {
					UpdateActive();
				}

				if ((flags & StatFlags.Order) != StatFlags.Zero)  {
					int order = Unit.Order;
					m_PlaceLabel.Content = order == 0 ? "-" : order.ToString();
				}

				if ((flags & StatFlags.Lap ) != StatFlags.Zero)  {
					int lap = Unit.Statistics.Lap;
					m_LapLabel.Content = lap > Unit.Laps ? Unit.Laps.ToString():lap.ToString();
				}

				if ((flags & (StatFlags.Distance | StatFlags.LapDistance | StatFlags.Drafting)) != StatFlags.Zero)  {
					double d,totalx = m_Parent.m_TotalX;
					d = Unit.Statistics.LapDistance;
					m_Loc.Normalized = d / totalx;
					double x = m_Parent.m_Width * d / totalx;
					m_VLine.X1 = m_VLine.X2 = m_HLine.X2 = x;
					double th = m_Parent.Layer_VLine.ActualHeight;
					double h = m_Loc.NormalizedY * m_Parent.m_Height;
					double y = (th - m_Parent.m_Height) + h;
					if (y >= th)  {
						y = th - 1;
					}
					m_VLine.Y2 = y;
					m_VLine.Stroke = m_VLine.Y1 < m_VLine.Y2 ? RaceCourseDisplay.ms_UpLine[m_Num] : RaceCourseDisplay.ms_DownLine[m_Num];

					String s;
					double w = 40;
					bool off = false;
					Brush color = RaceCourseDisplay.ms_Name_Std;
					
					if (m_Unit.Statistics.Finished)  {
						switch (m_Unit.Order)  {
							case 1:
								s = "1st";
								break;
							case 2:
								s = "2nd";
								break;
							case 3:
								s = "3rd";
								break;
							case 4:
								s = "4th";
								break;
							case 5:
								s = "5th";
								break;
							case 6:
								s = "6th";
								break;
							case 7:
								s = "7th";
								break;
							case 8:
								s = "8th";
								break;
							default:
								s = "Finished";
								w = 50;
								break;
						}
					}										// if (m_Unit.Statistics.Finished)
					else  {
						if (Unit.Statistics.Drafting) {
							s = "D " + m_Unit.Statistics.DistanceLeadString;
							color = RaceCourseDisplay.ms_Name_Std_Draft;
							w = 48;
						}
						else {
							s = m_Unit.Statistics.DistanceLeadString;
						}

						if (s == "-")  {
							if (m_Unit.Statistics.Distance > 0) {
								w = 50;
								//xxx
								s = "Leading";
							}
							else {
								off = true;
							}
						}
					}									// if (!m_Unit.Statistics.Finished)

					m_Side = m_Message != null ? States.Message:off ? States.Off:x > w + c_LeftSide ? States.Left : States.Right; 

					if (m_Side != m_State)  {
						m_Fade = false; // Fade out
						StartEveryFrameUpdate();
					}

					if (m_State == States.Right || m_State == States.Left)  {
						m_BBoxLoc.Left = m_State == States.Left ? x - (w+c_LeftSide) : x + 60;
						m_BBox.Width = w;
						m_BBox.Margin = m_BBoxLoc;
						m_BBoxLabel.Content = s;
						m_BBox.HorizontalAlignment = HorizontalAlignment.Left;
					}
					else if (m_State == States.Message)  {
						m_BBoxLoc.Left = 0;
						m_BBox.Width = 300;
						m_BBox.Margin = m_BBoxLoc;
						m_BBox.HorizontalAlignment = HorizontalAlignment.Center;
					}
					m_BBoxLabel.Foreground = color;
				}
				return;
			}										// public void Update(StatFlags flags)

		}											// class Node


		List<Node> m_StoreNodes = new List<Node>();
		List<Node> m_Nodes = new List<Node>();

		//============================================
		public static DependencyProperty AltProperty = DependencyProperty.Register("Alt", typeof(bool), typeof(RaceCourseDisplay),
			new FrameworkPropertyMetadata(true, new PropertyChangedCallback(_AltChanged)));
		public bool Alt
		{
			get { return (bool)this.GetValue(AltProperty); }
			set { this.SetValue(AltProperty, value); }
		}
		private static void _AltChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RaceCourseDisplay)d).OnAltChanged(false);
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
				TextBrush = Brushes.White;
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
				TextBrush = AppWin.StdBrush_Dark;
			}
		}
		//==========================================
		public RaceCourseDisplay()
		{
			InitializeComponent();
			InitResources(this);
		}





		private Course m_Course;
		double m_TotalX;
		public static SolidColorBrush ms_Other_Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99000000"));
		public Course Course
		{
			get { return m_Course; }
			set
			{
				if (m_Course == value)
					return;
				m_Course = value;
				if (m_Course != null)
				{
					foreach (Node n in m_StoreNodes)
					{
						n.m_Loc = new Course.Location(m_Course, 0);
					}
					m_TotalX = m_Course.TotalX;
				}
				LoadUnits();
			}
		}
		bool m_bLoaded;

		private void RaceCourseDisplay_Loaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;

			m_bLoaded = true;
			OnAltChanged(true);
			LoadUnits();
		}


		bool m_bUpdating = false;
		void UpdateOn()
		{
			if (!m_bLoaded || m_bUpdating)
				return;
			m_bUpdating = true;
			Unit.AddNotify(null, m_StatFlags, new Unit.NotifyEvent(Update));
		}
		void UpdateOff()
		{
			if (m_bUpdating)
			{
				m_bUpdating = false;
				Unit.RemoveNotify(null, m_StatFlags, new Unit.NotifyEvent(Update));
			}
		}
		public void Update(Unit unit, StatFlags flags)
		{
			foreach (Node n in m_Nodes)
			{
				n.Update(n.Unit.Statistics.Changed);
			}
		}

		public void UpdateAll(Unit unit, StatFlags flags)
		{
			if (!m_bLoaded || !IsVisible || m_Course == null)
				return;
			foreach (Node n in m_Nodes)
			{
				n.Update(flags);
			}
		}




		private Node m_CurActiveNode;
		private int m_CurActive; // The currently displayed active unit.
		private int CurActive
		{
			get { return m_CurActive; }
			set
			{
				m_CurActive = value;
				Node n = value >= 0 && value < m_Nodes.Count ? m_Nodes[value] : null;
				if (m_CurActiveNode == n)
					return;
				if (m_CurActiveNode != null)
					m_CurActiveNode.Active = false;
				m_CurActiveNode = n;
				n.Active = true;				
			}
		}

		Brush OffBrush = Brushes.White;
		Brush ActiveBrush = Brushes.Red;
		Brush TextBrush = Brushes.White;

		double m_Width = 1;
		double m_Height = 1;
		double m_Offset = 1;
		double m_Adjust = 0;

		public void RedrawCourse()
		{
			if (!m_bLoaded)
				return;
			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;

			double rcwidth = Unit.Laps > 1 ?  (Unit.Laps >= 100 ? 27:Unit.Laps >= 10 ? 18:9) + LapsColumn.Margin.Left:0.0;

			double w = ActualWidth - (NameBox.ActualWidth + CourseBox.Margin.Left + CourseBox.Margin.Right + rcwidth);
			double h = CoursePath.Height;
			//VLine.Margin = HLine.Margin = new Thickness(NameBox.ActualWidth,0,0,0);
			m_Adjust = h - NameBox.ActualHeight;
			CoursePath.Width = w;
			CoursePath.Height = h;
			CoursePath.Margin = new Thickness(0, 0, rcwidth, 0);
			m_Width = w;
			m_Height = h;
			m_Offset = ActualHeight - h;

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
			UpdateAll(null,m_StatFlags);

			//Debug.WriteLine(String.Format("{0}: {1:F0}, {2:F0}",Unit.Active.Count,CalcHeight, ActualHeight));
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
				foreach(Node n in m_Nodes)
					n.Active = m_ActiveUnit == n.Unit;
			}
		}

		public void SetMessage(Unit unit, object msg)
		{
			if (unit == null)
				return;
			foreach (Node n in m_Nodes)
			{
				if (n.Unit == unit)
				{
					n.Message = msg;
					break;
				}
			}
		}


		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (m_bLoaded)
			{
				LoadUnits();
				Update(null,m_StatFlags);
			}
		}

		private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible)
				RedoOn();
			else
				UpdateOff();
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			m_Course = null; //
			UpdateOff();
		}

		//====================================================================
		public double CalcHeight
		{
			get
			{
				int num = Unit.RaceUnit.Count;
				double h = num * ms_Height + CourseBox.Margin.Bottom + CourseBox.Margin.Top + Names.Margin.Bottom + Names.Margin.Top;
				return h < MinHeight ? MinHeight : h;
			}
		}

		bool m_bRedo;

		public void LoadUnits()
		{
			if (!m_bLoaded || !IsVisible || m_Course == null)
				return;

			while (m_StoreNodes.Count < Unit.RaceUnit.Count)
				m_StoreNodes.Add( new Node( this, m_StoreNodes.Count ) );

			int t = 0;
			int num = m_Nodes.Count();
			m_Nodes.Clear();
			foreach (Unit unit in Unit.RaceUnit)
			{
				Node n = m_StoreNodes[t++];
				m_Nodes.Add(n);
				n.Unit = unit;
			}
			for (int i = m_Nodes.Count; i < num; i++)
				m_StoreNodes[i].Unit = null;

			RedoOn();

		}

		public void RedoOn()
		{
			if (!m_bRedo)
			{
				m_bRedo = true;
				AppWin.AddRenderUpdate(new AppWin.RenderUpdate(LoadUnitRenderUpdate), 0);
			}
		}

		/// <summary>
		/// Do until we have valid stuff or we are not running any more.
		/// </summary>
		/// <param name="seconds"></param>
		/// <param name="split"></param>
		/// <returns></returns>
		bool LoadUnitRenderUpdate(double seconds, double split)
		{
			if (!m_bLoaded || !IsVisible || m_Course == null)
			{
				UpdateOff();
				m_bRedo = false;
				return true; // Close out.
			}
			if (ActualWidth == 0 || CourseBox.ActualHeight == 0)
				return false; // We need to do this again next render frame.

			// OK We should be able to redraw the course and have everything work fine.
			RedrawCourse();

			UpdateOn();
			UpdateAll(null, m_StatFlags);

			m_bRedo = false;
			return true;
		}

	}
}
