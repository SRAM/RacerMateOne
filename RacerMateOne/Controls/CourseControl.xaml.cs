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
	/// Interaction logic for Course.xaml
	/// </summary>
	public partial class CourseControl : UserControl
	{
		//===============================================================
		public static DependencyProperty DisableProperty = DependencyProperty.Register("Disable", typeof(bool), typeof(CourseControl),
				new FrameworkPropertyMetadata(false,new PropertyChangedCallback(OnDisableChanged)));

		public bool Disable
		{
			get { return (bool)this.GetValue(DisableProperty); }
			set { this.SetValue(DisableProperty, value); }
		}
		private static void OnDisableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CourseControl)d).DisableChanged();
		}
		//===============================================================

		public static readonly RoutedEvent NeedSaveChangedEvent =
			EventManager.RegisterRoutedEvent(
			"NeedSaveChanged", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CourseControl));

		public event RoutedEventHandler NeedSaveChanged
		{
			add { AddHandler(NeedSaveChangedEvent, value); }
			remove { RemoveHandler(NeedSaveChangedEvent, value); }
		}

        public bool NeedSave
        {
            //get { return m_Course != null && m_Course.Changed; }
            get { return m_CurNeedSave; }
        }
		public bool m_CurNeedSave=false;
		public bool m_NeedSaveForce = false;

		//===============================================================

		public static readonly RoutedEvent StartChangedEvent =
			EventManager.RegisterRoutedEvent(
			"StartChanged", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CourseControl));

		public event RoutedEventHandler StartChanged
		{
			add { AddHandler(StartChangedEvent, value); }
			remove { RemoveHandler(StartChangedEvent, value); }
		}
		//===============================================================

		public static readonly RoutedEvent FlagsMovedEvent =
			EventManager.RegisterRoutedEvent(
			"FlagsMoved", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CourseControl));

		public event RoutedEventHandler FlagsMoved
		{
			add { AddHandler(FlagsMovedEvent, value); }
			remove { RemoveHandler(FlagsMovedEvent, value); }
		}

		//===============================================================
		public int Laps
		{
			get { return CurrentCourse != null ? CurrentCourse.Laps : 1; }
			set
			{
				if (CurrentCourse != null)
				{
					CurrentCourse.Laps = value;
					RedrawText();
                    //Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
                    //{
                    //    RedrawText();
                    //});
				}
			}
		}
		//===============================================================

		bool m_bReverse;
		public bool Reverse
		{
			get { return m_bReverse; }
			set
			{
				m_bReverse = value;
				FlagsChanged();
			}
		}
		bool m_bMirror;
		public bool Mirror
		{
			get { return m_bMirror; }
			set
			{
				m_bMirror = value;
				FlagsChanged();
			}
		}
		public void SetFlags(bool mirror, bool reverse)
		{
			m_bMirror = mirror;
			m_bReverse = reverse;
			FlagsChanged();
		}

		bool m_bZoom;
		public bool Zoom
		{
			get { return m_bZoom; }
			set
			{
                if (m_bZoom == value) 
                    return;
                m_bZoom = value;
				FlagsChanged();
			}
		}
		//===============================================================

		public static DependencyProperty RoundedEdgeProperty = DependencyProperty.Register("RoundedEdge", typeof(Brush), typeof(CourseControl));
		public Brush RoundedEdge
		{
			get { return (Brush)this.GetValue(RoundedEdgeProperty); }
			set { this.SetValue(RoundedEdgeProperty, value); }
		}
		//===============================================================
		public static DependencyProperty LayerUnitProperty = DependencyProperty.Register("LayerUnit", typeof(double), typeof(CourseControl),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLayerUnitChanged)));
		public double LayerUnit
		{
			get { return (double)this.GetValue(LayerUnitProperty); }
			set { this.SetValue(LayerUnitProperty, value); }
		}
		private static void OnLayerUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((CourseControl)d).LayerUnitChanged();
		}
		void LayerUnitChanged()
		{
			RedrawCourse();
		}


		//===============================================================
		protected RacerMateOne.Course m_BaseCourse = null;
		protected RacerMateOne.Course m_Course = null;
		public Course CurrentCourse
		{
			get { return m_BaseCourse; }
			set
			{
				if (m_BaseCourse == value)
					return;
               // Debug.WriteLine("---Course Control CurrentCourse is being set");
				m_BaseCourse = value;
				m_Course = null;
				m_NeedSaveForce = true;
				FlagsChanged();
				Save();
				DisableChanged();
			}
		}
		//===============================================================
        // the modified course is what is ultimately loaded into a ride, and the m_BaseCourse is what we send it with attributes.
        //It's kinda fucked up.

		public Course ModifiedCourse
		{
			get
			{
                //Debug.WriteLine(" some access to the Modified course occured, Reverse= " + Reverse.ToString() + " Mirror = " + Mirror.ToString() + " Zoom = " + true.ToString() + " zStartAt =" + zStartAt + " zEndAt= " + zEndAt); 
                return new Course( m_BaseCourse,Reverse,Mirror,true,zStartAt,zEndAt );
			}
		}
		//===============================================================

		public CourseControl()
		{
			InitializeComponent();
			this.SetValue(LayerUnitProperty, (100.0 / 5280) * ConvertConst.MilesToMeters);


		}


		public void Save()
		{
            //Debug.WriteLine(Environment.NewLine + Environment.NewLine + "in coursecontrol Save m_bZoom= " + m_bZoom.ToString());
            //return;
            if (m_BaseCourse != null)
			{
				m_BaseCourse.StartAt = m_bZoom ? zStartAt:m_BaseCourse.TotalX * StartFlag.Value;
				m_BaseCourse.EndAt = m_bZoom ? zEndAt:m_BaseCourse.TotalX * EndFlag.Value;
				m_BaseCourse.Mirror = Mirror;
				m_BaseCourse.Reverse = Reverse;
                //Debug.WriteLine("Saving a course: Course = " + System.IO.Path.GetFileNameWithoutExtension(m_BaseCourse.FileName) + Environment.NewLine
                //    + "Start = " + m_BaseCourse.StartAt + " EndAt = " + m_BaseCourse.EndAt + Environment.NewLine + 
                //    "Mirror= " + Mirror + " Reverse = " + Reverse + Environment.NewLine + 
                //    " m_BaseCourse.Changed = " + m_BaseCourse.Changed.ToString() + " NeedSaveForce = " + m_NeedSaveForce.ToString());
                //Debug.WriteLine( Environment.NewLine + Environment.NewLine);
               
                //Debug.WriteLine("Original course: Course = " + System.IO.Path.GetFileNameWithoutExtension(m_BaseCourse.FileName) + Environment.NewLine
                //                  + "OStart = " + m_BaseCourse.OriginalStartAt + " OEndAt = " + m_BaseCourse.OriginalEndAt + Environment.NewLine +
                //                  "OMirror= " + m_BaseCourse.OriginalMirror + " Reverse = " + m_BaseCourse.OriginalReverse + Environment.NewLine);
                //Debug.WriteLine("Done coursecontrol Save" + Environment.NewLine + Environment.NewLine);
  

				if (m_CurNeedSave != m_BaseCourse.Changed || m_NeedSaveForce)
				{
                    //Debug.WriteLine("Raising Nedsavechanged: m_CurNeedSasved was : " + m_CurNeedSave + " m_basecourse.changed was " + m_BaseCourse.Changed + " m_needSaveForce was " + m_NeedSaveForce);
                    m_NeedSaveForce = false;
                    //Debug.WriteLine("checking m_BaseCourse.Changed");
					m_CurNeedSave = m_BaseCourse.Changed;
                    //m_Course.Changed = m_BaseCourse.Changed;
                  //  Debug.WriteLine("------------" + Environment.NewLine + " Raising the event NeedSaveChangedEvent" + Environment.NewLine + " --------");
					RoutedEventArgs args = new RoutedEventArgs(NeedSaveChangedEvent);
           		    RaiseEvent(args);
				}
			}
		}

		public void SetCourse( Course course, bool reverse, bool mirror, bool zoom, double startat, double endat )
		{
			adjusting = true;
            //Debug.WriteLine("inside Setcourse (parm parm)");
			try
			{
                // this may be teh killer bit here. We are setting these equal to input parms
                // because only teh Staging window calls this to set teh course upon clicking one.
                // it prevents recomputing the start and end points so that the file is interprted as start end POST profile transformation.
                m_CurMirror = mirror;
                m_CurReverse = reverse;
                m_CurZoom = zoom;
				m_bReverse = reverse;
				m_bMirror = mirror;
				m_bZoom = zoom;
				if (course != null)
				{
					StartFlag.Value = 0;
					EndFlag.Value = 1;
                    StartFlag.Value = startat / course.TotalX;
                    EndFlag.Value = endat / course.TotalX;
                    
                    //FlagsChanged();
				}
			}
			catch {
                Debug.WriteLine("Exception in setcourse(parm parm..)"); 
            }
			adjusting = false;
			CurrentCourse = course;
		}

		private bool adjusting = false;
		private void StartFlag_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (adjusting)
				return;
			adjusting = true;
			double m = m_Course != null ? m_Course.MinCourseXNormalized : 0.001;
			double min = StartFlag.Value + m;
			if (min > 1)
				min = 1;
			if (StartFlag.Value > EndFlag.Value - m)
			{
				double t = EndFlag.Value - m;
				if (t < 0)
					t = 0;
				StartFlag.Value = t;
			}
			double left = Math.Floor(CourseGrid.ActualWidth * min);
			if (left > CourseGrid.ActualWidth - 10)
				left = CourseGrid.ActualWidth - 10;
			if (EndFlag.Margin.Left != left)
			{
				EndFlag.Margin = new Thickness(left, EndFlag.Margin.Top, EndFlag.Margin.Right, EndFlag.Margin.Bottom);
				EndFlag.Minimum = min;
			}
			//if (m_Course != null)
			//	m_Course.StartAt = m_Course.TotalX * StartFlag.Value;
			Save();
			RedrawText();
			adjusting = false;

			RoutedEventArgs args = new RoutedEventArgs(StartChangedEvent);
			RaiseEvent(args);
		}

		private void EndFlag_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (adjusting)
				return;
			adjusting = true;
			double m = m_Course != null ? m_Course.MinCourseXNormalized : 0.001;
			double max = EndFlag.Value - m;
			if (max < 0)
				max = 0;
			if (EndFlag.Value < StartFlag.Value + m)
			{
				double t = StartFlag.Value + m;
				if (t > 1)
					t = 1;
				EndFlag.Value = t;
			}
			double right = Math.Floor(CourseGrid.ActualWidth * (1.0 - max));
			if (right < 1)
				right = 1;
			if (StartFlag.Margin.Right != right)
			{
				StartFlag.Margin = new Thickness(StartFlag.Margin.Left, StartFlag.Margin.Top, right, StartFlag.Margin.Bottom);
				StartFlag.Maximum = max;
			}
            //if (m_Course != null)
            //    m_Course.EndAt = m_Course.TotalX * EndFlag.Value;
			Save();
			RedrawText();
			adjusting = false;
		}

		protected RacerMateOne.Course.Location m_StartLoc;
		protected RacerMateOne.Course.Location m_EndLoc;
		protected RacerMateOne.Course.Location m_MouseLoc;
		protected TextBlock[] m_TBInfo;

		public RacerMateOne.Course.Location StartLoc
		{
			get { return m_StartLoc; }
		}


		void SetCourse()
		{
			if (m_HoverLeft == null || m_HoverRight == null)
				return;
			if (m_Course != null)
			{
				//StartFlag.Value = m_Course.StartAt / m_Course.TotalX;
				//EndFlag.Value = (m_Course.EndAt <= 0 ? m_Course.TotalX : m_Course.EndAt) / m_Course.TotalX;
				//Mirror = m_Course.Mirror;
				//Reverse = m_Course.Reverse;
                
				m_StartLoc = new Course.Location(m_Course, StartFlag.Value);
				m_EndLoc = new Course.Location(m_Course, EndFlag.Value);
				
				m_MouseLoc = new Course.Location(m_Course, 0);
                m_MouseLoc.Start = m_StartLoc;
                m_MouseLoc.End = m_EndLoc;
                MidText.Visibility = LeftText.Visibility = RightText.Visibility = Visibility.Visible;
				m_HoverLeft.Children.Clear();
				m_HoverRight.Children.Clear();
				m_TBInfo = new TextBlock[m_Course.InfoLabels.Count];
				int cnt = 0;
				foreach (String s in m_Course.InfoLabels)
				{
					TextBlock t = new TextBlock();
					t.FontSize = 12;
					t.Foreground = Brushes.Black;
					t.FontWeight = FontWeights.Bold;
					t.Text = s;
					m_HoverLeft.Children.Add(t);

					t = new TextBlock();
					t.FontSize = 12;
					t.Foreground = Brushes.Black;
					t.Text = "";
					m_HoverRight.Children.Add(t);
					m_TBInfo[cnt++] = t;
				}
				//m_Course.StartAt = StartFlag.Value * m_Course.TotalX;
				//m_Course.EndAt = EndFlag.Value * m_Course.TotalX;
			}
			else
			{
				m_TBInfo = null;
				m_StartLoc = m_EndLoc = m_MouseLoc = null;
				MidText.Visibility = LeftText.Visibility = RightText.Visibility = Visibility.Collapsed;
			}

			RedrawCourse();
		}

		double zStartAt
		{
			get 
			{ 
				return (m_CurZoom ? m_CurZoomStartAt:0) + StartFlag.Value * (m_Course != null ? m_Course.TotalX : 0); 
			}
		}
		double zEndAt
		{
			get 
			{ 
				return (m_CurZoom ? m_CurZoomStartAt:0) + EndFlag.Value * (m_Course != null ? m_Course.TotalX : 1); 
			}
		}

		public void RedrawCourse()
		{
			if (AppWin.IsInDesignMode)
				return;

			double controlwidth = AppWin.Instance.ActualWidth;	// Use this for the width...more than we need but close.

			StreamGeometry geometry = new StreamGeometry();
			geometry.FillRule = FillRule.EvenOdd;
			if (m_Course != null && m_Course.Segments.Count > 0)
			{
				double miny = m_Course.MinY;
				double maxy = m_Course.MaxY;
				double diff = maxy - miny;

				FillBrush.Viewport = new Rect( 0,0, 0.25, LayerUnit / diff );

				using (StreamGeometryContext ctx = geometry.Open())
				{
					ctx.BeginFigure(new Point(0,1), true /* is filled */, true /* is closed */);
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
								for (sp.X += xadd; xn < 1.0;sp.X += xadd,xn += xnadd)
								{
									sp.Y = ss.GetNormalizedY( xn );
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
					ctx.LineTo(new Point(1, 1),true,false);
				}				
			}
			geometry.Freeze();
			CoursePath.Data = geometry;
			RedrawText();
		}

		public void RedrawText()
		{
			if (m_Course == null)
				return;
			m_StartLoc.Normalized = StartFlag.Value;
			m_EndLoc.Normalized = EndFlag.Value;

			LeftText.Text = m_StartLoc.XText;
			RightText.Text = m_EndLoc.XText;
			InlineCollection ic = MidText.Inlines;
			ic.Clear();

			m_Course.LengthFillTextBlock(MidText,m_StartLoc, m_EndLoc, Laps);
          //  Debug.WriteLine("the text will be: " + MidText.Text + " m_starLoc was " + m_StartLoc.Normalized  + " m_EndLoc was " + m_EndLoc.Normalized);
			//MidTextRight.Text = m_Course.SectionLengthText(m_StartLoc, m_EndLoc);

			Point cp = MidText.TranslatePoint(new Point(0, 0), this);
			double cw = MidText.ActualWidth;

			double x,w;
			w = LeftText.ActualWidth;
			x = ActualWidth * StartFlag.Value - w;
			if (x + w > cp.X - 10)
				x = cp.X - 10 - w;
			if (x < 0)
				x = 0;
			LeftText.Margin = new Thickness(x, LeftText.Margin.Top, 0, 0);

			w = RightText.ActualWidth;
			x = ActualWidth * EndFlag.Value;

			if (x < cp.X + cw + 10)
				x = cp.X + cw + 10;
			if (x + w > ActualWidth)
				x = ActualWidth - w;
			RightText.Margin = new Thickness(x, RightText.Margin.Top, 0, 0);
		}



		private StackPanel m_HoverLeft;
		private StackPanel m_HoverRight;
		private void HoverLeft_Loaded(object sender, RoutedEventArgs e) { m_HoverLeft = sender as StackPanel; if (m_Course != null) SetCourse(); }
		private void HoverRight_Loaded(object sender, RoutedEventArgs e) { m_HoverRight = sender as StackPanel; if (m_Course != null) SetCourse();  }

		protected bool m_bHoverBoxOn = false;
		protected bool m_bLoaded = false;
		private void course_Loaded(object sender, RoutedEventArgs e)
		{
			if (AppWin.IsInDesignMode)
				return;

			Dispatcher.BeginInvoke(DispatcherPriority.Render, (ThreadStart)delegate()
			{
				m_bLoaded = true;
				FlagsChanged();
				DisableChanged();
				double sf = StartFlag.Value;
				double ef = EndFlag.Value;
				if (sf != 0 || ef != 1)
				{
					StartFlag.Value = 0;
					EndFlag.Value = 1;
					StartFlag.Value = sf;
					EndFlag.Value = ef;
				}
			});
		}

		private void course_PreviewMouseMove(object sender, MouseEventArgs e)
		{
			Point m;
			if (m_Drag != 0)
			{
				if (m_Drag < 0)
					m = new Point(StartFlag.Value * CourseArea.ActualWidth, 1);
				else
					m = new Point(EndFlag.Value * CourseArea.ActualWidth, 1);
			}
			else
				m = e.GetPosition(CourseArea);


			if (!Disable && m_Course != null && m.X > 0 && m.Y > 0 && m.X < CourseArea.ActualWidth  && m.Y < CourseArea.ActualHeight)
			{
				if (m.X < 0)
					m.X = 0;
				else if (m.X > CourseArea.ActualWidth)
					m.X = CourseArea.ActualWidth;

				double n = m.X / CourseArea.ActualWidth;
				double ex = !m_CurZoom && (n < StartFlag.Value || n > EndFlag.Value) && m_Drag == 0 ? 0.5 : 1.0;


				if (!m_bHoverBoxOn)
				{
					m_bHoverBoxOn = true;

					hoverBox.Visibility = Visibility.Visible;
					hoverBoxFade.Stop();
					hoverBoxFadeAnimation.To = ex;
					hoverBoxFade.Begin();
				}
				else if (ex != hoverBoxFadeAnimation.To)
				{
					hoverBoxFade.Stop();
					hoverBoxFadeAnimation.To = ex;
					hoverBoxFade.Begin();
				}


				m_MouseLoc.Normalized = n;
				Point mpos = e.GetPosition(this);

				hoverBox.ArrowSlant = 0.3;
				hoverBox.ArrowHeight = m_MouseLoc.NormalizedY * CourseArea.ActualHeight + 10.0;
				hoverBox.SetPoint(m.X, CourseArea.Margin.Top + m_MouseLoc.NormalizedY * CourseArea.ActualHeight, this.ActualWidth - 36);

				List<String> infodata = m_MouseLoc.InfoData;
                m_MouseLoc.Start = m_StartLoc;
                m_MouseLoc.End = m_EndLoc;
				int cnt = 0;
				foreach (String s in infodata)
				{
					m_TBInfo[cnt++].Text = s;
				}
			}
			else if (m_bHoverBoxOn)
			{
				m_bHoverBoxOn = false;

				hoverBoxFade.Stop();
				hoverBoxFadeAnimation.To = 0.0;
				hoverBoxFade.Begin();
			}


		}

		private void course_MouseEnter(object sender, MouseEventArgs e)
		{

		}

		private void course_MouseLeave(object sender, MouseEventArgs e)
		{
			if (m_bHoverBoxOn)
			{
				m_bHoverBoxOn = false;
				hoverBoxFade.Stop();
				hoverBoxFadeAnimation.To = 0.0;
				hoverBoxFade.Begin();
			}
		}

		private void DisableChanged()
		{
			if (!m_bLoaded)
				return;

			if (Disable || m_BaseCourse == null)
			{
				Disable_Box.Visibility = Visibility.Visible;
				LeftText.Visibility = Visibility.Hidden;
				RightText.Visibility = Visibility.Hidden;
				MidText.Visibility = Visibility.Hidden;
			}
			else
			{
				Disable_Box.Visibility = Visibility.Collapsed;
				LeftText.Visibility = Visibility.Visible;
				RightText.Visibility = Visibility.Visible;
				MidText.Visibility = Visibility.Visible;
			}
            //Debug.WriteLine("calling ShowBarsChanged");
			ShowBarsChanged();
		}

		public double LastStartValue { get; protected set; }
		public double LastEndValue { get; protected set; }

		int m_Drag = 0;
		private void StartFlag_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			LastStartValue = StartFlag.Value;
			LastEndValue = EndFlag.Value;
			m_Drag = -1;
		}

		private void StartFlag_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			RoutedEventArgs args = new RoutedEventArgs(FlagsMovedEvent);
			RaiseEvent(args);	
			m_Drag = 0;
		}

		private void EndFlag_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
		{
			LastStartValue = StartFlag.Value;
			LastEndValue = EndFlag.Value;
			m_Drag = 1;
		}

		private void EndFlag_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			RoutedEventArgs args = new RoutedEventArgs(FlagsMovedEvent);
			RaiseEvent(args);
			m_Drag = 0;
		}

		private void hoverBoxFadeAnimation_Completed(object sender, EventArgs e)
		{
			if (hoverBox.Opacity <= 0.0)
				hoverBox.Visibility = Visibility.Collapsed;
		}

		bool m_CurMirror;
		bool m_CurReverse;
		bool m_CurZoom;
		double m_CurZoomStartAt;
		double m_CurZoomEndAt;
        int reversalcount = 0;

		private void FlagsChanged()
		{
            
            //Debug.WriteLine("Inside FlagsChanged() Mirror = "+ Mirror + " Reverse=" + Reverse + " Zoom=" + Zoom);
            if (!m_bLoaded)
				return;
			bool mirror = Mirror;
			bool reverse = Reverse;
			bool zoom = Zoom;
            //Debug.WriteLine("m_CurReverse = " + m_CurReverse + " m_CurMirror = " + m_CurMirror + " m_CurZoom = " + m_CurZoom );
            //Debug.WriteLine("Reverse      = " + Reverse +      " Mirror      = " + Mirror +      "      Zoom = " + Zoom);

			if (m_BaseCourse == null && m_Course != null)
			{
                Debug.WriteLine("Inside FlagsChanged() mBaseCourse is null and m_course is not, setting a null course??");
                m_Course = null;
				SetCourse(); // Close down stuff.
			}
			else if (m_BaseCourse != null && (m_Course == null || mirror != m_CurMirror || reverse != m_CurReverse || zoom != m_CurZoom))
			{
                //Debug.WriteLine("In Flagschanged and adjusting Start and end values, Start = " + m_BaseCourse.StartAt + " End = " + m_BaseCourse.EndAt);
                double sv,ev;
				if (m_CurMirror != mirror)
                
				{
					if (mirror)
					{
						sv = StartFlag.Value; ev = EndFlag.Value;
						if (sv != 0.0 || ev != 1.0)
						{
                          //  Debug.WriteLine(" I am changing the start and end flags due to mirror");
                          //  Debug.WriteLine(" Before: StartFlag = " + StartFlag.Value + " EndFlag = " + EndFlag.Value);
                            StartFlag.Value = (StartFlag.Value + (m_CurReverse ? 1.0 : 0)) / 2.0;
                            EndFlag.Value = (EndFlag.Value + (m_CurReverse ? 1.0 : 0)) / 2.0;
                            //StartFlag.Value = (StartFlag.Value + (m_bReverse ? 1.0 : 0)) / 2.0;
                            //EndFlag.Value = (EndFlag.Value + (m_bReverse ? 1.0 : 0)) / 2.0;
						}
					}
					else
					{
						sv = StartFlag.Value * 2.0; ev = EndFlag.Value * 2.0;
						if (sv < 1.0 && ev > 1.0)
							ev = 1.0;
						else if (sv >= 1.0)
						{
							ev = (1.0 - StartFlag.Value) * 2.0;
							sv = (1.0 - EndFlag.Value) * 2.0;
							if (ev > 1.0)
								ev = 1.0;
						}
						StartFlag.Value = sv;
						EndFlag.Value = ev;
                       // Debug.WriteLine(" After: StartFlag = " + StartFlag.Value + " EndFlag = " + EndFlag.Value);
                    
                    }
				}

                if (m_CurReverse != reverse && !mirror)
                    {
                    reversalcount += 1;
                    //Debug.WriteLine(" I am reversing the start and end flags due to reversal" + reversalcount );
                   // Debug.WriteLine(" Before: StartFlag = " + StartFlag.Value + " EndFlag = " + EndFlag.Value);
                    ev = 1.0 - StartFlag.Value;
					sv = 1.0 - EndFlag.Value;
                    
					StartFlag.Value = sv;
					EndFlag.Value = ev;
                    
                    //Debug.WriteLine(" After: StartFlag = " + StartFlag.Value + " EndFlag = " + EndFlag.Value);
				}
				if (m_CurZoom != zoom)
				{
                    //Debug.WriteLine("I'm treating Zoom");
                    if (zoom)
					{
                        //Debug.WriteLine(Environment.NewLine + "Zooming IN m_BaseCourse.TotalX = " + m_BaseCourse.TotalX + " zStartAt= " + zStartAt + " zEndAt= " + zEndAt);
                        m_CurZoomStartAt = zStartAt;
						m_CurZoomEndAt = zEndAt;
						StartFlag.Value = 0;
						EndFlag.Value = 1.0;
					}
                    else if (m_BaseCourse != null)
                    {  //Debug.WriteLine(Environment.NewLine + "Zooming OUT m_BaseCourse.TotalX = " + m_BaseCourse.TotalX + " zStartAt= " + zStartAt + " zEndAt= " + zEndAt );
                        if (!Mirror || (zStartAt == 0 && zEndAt == 2 * m_BaseCourse.TotalX))
                        {
                            //Debug.WriteLine("making normal zoomout adjustment");
                            StartFlag.Value = zStartAt / m_BaseCourse.TotalX;
                            EndFlag.Value = zEndAt / m_BaseCourse.TotalX;
                        }
                        else
                        {
                            //Debug.WriteLine("making special adjustment");
                            StartFlag.Value = (zStartAt / (m_BaseCourse.TotalX * 2));
                            EndFlag.Value = (zEndAt / (m_BaseCourse.TotalX * 2));
                        }
                    }
				}
			
				m_CurMirror = mirror; 
				m_CurReverse = reverse;
				m_CurZoom = zoom;
				if (Mirror || Reverse || Zoom)
				{
                  //  Debug.WriteLine("calling new course");
                  //  adjusting = true;
                    m_Course = new Course(m_BaseCourse, Reverse, Mirror, Zoom, m_CurZoomStartAt, m_CurZoomEndAt);
                    SetCourse();
                  //  adjusting = false;
				}
				else
				{
					if (m_BaseCourse != m_Course)
					{
                     //   adjusting = true; 
                        m_Course = m_BaseCourse;
						SetCourse();
                    //    adjusting = false;
					}
				}
			}
			Save();
			ShowBarsChanged();
		}

		private void ShowBarsChanged()
		{
			if (!m_bLoaded)
				return;
			Visibility showbars = m_Course != null && !Disable ? Visibility.Visible : Visibility.Hidden;
			StartFlag.Visibility = showbars;
			EndFlag.Visibility = showbars;
		}

		//============================================================
		bool m_Interactive;
		public bool Interactive
		{
			get { return m_Interactive; }
			set
			{
				if (m_Interactive == value)
					return;
				m_Interactive = true;
				d_Bottom.Visibility = m_Interactive ? Visibility.Hidden : Visibility.Visible;
				d_Loc.Visibility = m_Interactive ? Visibility.Visible : Visibility.Collapsed;
			}
		}


	}
}
