
#pragma warning disable 414

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
using System.Diagnostics;

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for CoursePicker.xaml
	/// </summary>
	public partial class CoursePicker : UserControl
	{
		//====================================================================
		public static readonly RoutedEvent CourseSelectedEvent =
			EventManager.RegisterRoutedEvent(
			"CourseSelected", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CoursePicker));

		public event RoutedEventHandler CourseSelected
		{
			add { AddHandler(CourseSelectedEvent, value); }
			remove { RemoveHandler(CourseSelectedEvent, value); }
		}

		//====================================================================
		public CoursePicker()
		{
			InitializeComponent();
		}
		protected void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			m_PathStyle = (Style)FindResource("PType");
			Courses.OnCourseAdded += new Courses.CourseAdded(Courses_OnCourseAdded);
		}
		protected void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			Courses.OnCourseAdded -= new Courses.CourseAdded(Courses_OnCourseAdded);
			m_bWaiting = false;
            Course testagainst = m_SCourse;
            //Debug.WriteLine(" Closing the CoursePicker, I think I am wiping out " + m_CourseList.Count  + " courses." + Environment.NewLine);
            //m_CourseList.RemoveAll(item => item != testagainst);
                  
			AppWin.RemoveRenderUpdate(new AppWin.RenderUpdate(SortCheck));
           // Debug.WriteLine("unloading the control, making list null");
           
		}
		Dictionary<Course, CoursePickerLine> m_Lines = new Dictionary<Course, CoursePickerLine>();
		int m_TValue = 1;

		Style m_PathStyle;

		public void SetList(List<Course> list)
		{
			d_CoursePicker.Children.Clear();
			m_LineC = 0;
			m_TValue++; // 
			if (list == null)
				return;

			foreach (Course c in list)
				addLine(c);
			Course sc = m_SCourse;
			m_SCourse = null; // Force redo
			SelectedCourse = sc;
		}
		List<Path> m_LineList = new List<Path>();

		int m_LineC;


		void addLine(Course c)
		{
			if (m_BlockRedo)
			{
				m_RedoBlocked = true;
				return;
			}
			CoursePickerLine cline;
			if (!m_Lines.TryGetValue(c, out cline))
			{
				cline = new CoursePickerLine(c);
				cline.Margin = new Thickness(0, 1.75, 0, 1.75);
				cline.Click += new RoutedEventHandler(cline_Click);
				m_Lines[c] = cline;
			}
			if (cline.TValue != m_TValue) // Make sure we only add each course once! 
			{
				m_CurSort = -1;
				if (d_CoursePicker.Children.Count > 0)
				{
					Path p;
					if (m_LineC == m_LineList.Count)
					{
						p = new Path();
						p.Margin = new Thickness(3, 0, 5, 0);
						p.Style = m_PathStyle;
						m_LineList.Add(p);
					}
					else
						p = m_LineList[m_LineC];
					m_LineC++;
					d_CoursePicker.Children.Add(p);
				}
				d_CoursePicker.Children.Add(cline);
				cline.TValue = m_TValue;
			}
		}



		private void cline_Click(object sender, RoutedEventArgs e)
		{
			Controls.CoursePickerLine cline = sender as Controls.CoursePickerLine;
			//if (cline.Course != m_SCourse)
			{
				SelectedCourse = cline.Course;
				RoutedEventArgs args = new RoutedEventArgs(CourseSelectedEvent);
				RaiseEvent(args);
			}
		}


		Controls.CoursePickerLine m_SelectedCourseLine;
		Course m_SCourse;
		public Course SelectedCourse
		{
			get { return m_SCourse; }
			set
			{
				if (m_SCourse != value)
				{
					Controls.CoursePickerLine cline;
					if (value != null && m_Lines.TryGetValue(value, out cline) && cline.TValue == m_TValue)
					{
						if (m_SelectedCourseLine != null)
							m_SelectedCourseLine.Background = Brushes.White;
						m_SelectedCourseLine = cline;
						cline.BringIntoView();
						cline.Background = AppWin.StdBrush_Selected;
					}
					else if (m_SelectedCourseLine != null)
					{
						m_SelectedCourseLine.Background = Brushes.White;
						m_SelectedCourseLine = null;
					}
           			m_SCourse = value;
                  //  Debug.WriteLine(Environment.NewLine + "           CoursePicke has selected new course = " + (m_SCourse != null ? System.IO.Path.GetFileNameWithoutExtension(m_SCourse.FileName):" not selected")); 
		
				}
			}
		}

		//===============================================================
		protected CourseFilter m_Filter;
		public CourseFilter Filter
		{
			get { return m_Filter; }
			set
			{
				if (m_Filter == value)
					return;
				m_Filter = value;
				RedoList();
			}
		}


		//===============================================================
		protected int m_SortColumn;

		public int SortColumn
		{
			get { return m_SortColumn; }
			set
			{
				int v = value < 0 ? 0 : value > 3 ? 3 : value;
				if (v == m_SortColumn)
					return;
				m_SortColumn = v;
				DoSort(true);	// Force a sort.
			}
		}

		protected bool m_Reverse;
		public bool Reverse
		{
			get { return m_Reverse; }
			set
			{
				if (value == m_Reverse)
					return;
				m_Reverse = value;
				DoSort(true);
			}
		}

		public void Set(CourseFilter filter, int column, bool reverse)
		{
			if (m_Reverse == reverse && m_SortColumn == column && filter == m_Filter)
				return;
			m_Reverse = reverse;
			m_SortColumn = column;
			m_Filter = filter;
			DoSort(true);
		}
		//===============================================================
		public List<Course> List
		{
			get { return m_CourseList; }
		}

		public bool Shown(Course course)
		{
			CoursePickerLine cline;
			return m_Lines.TryGetValue(course, out cline) && cline.TValue == m_TValue;
		}

		void Courses_OnCourseAdded(Course course)
		{
            int replaceindex = -1;
            //Debug.WriteLine("Courses added to picker control " + System.IO.Path.GetFileName(course.FileName));
            if (Filter != null && Filter.InFilter(course))
			{
                //foreach (Course cc in m_CourseList)
                //{
                //    if (String.Compare(course.FileName, cc.FileName, true) == 0)
                //    {
                //        replaceindex = m_CourseList.IndexOf(cc); 
                     
                //        break;
                //    }
                //}
                if (replaceindex >= 0)
                {
                    //bool setthisasselected = false;
                    //if (m_CourseList.IndexOf(SelectedCourse) == replaceindex)
                    //    setthisasselected = true;
                    //m_CourseList.Insert(replaceindex, course);
                    //m_CourseList.RemoveAt(replaceindex + 1);
                    //if (setthisasselected)
                    //{
                    //    CoursePickerLine Cline = m_SelectedCourseLine;
                    //    m_Lines[course] = Cline;
                    //    SelectedCourse = course;
                    //}
                }
                else
                {
                    m_CourseList.Add(course);
                    if (!DoSort())
                        addLine(course);
                }
			}
		}



		List<Course> m_CourseList = new List<Course>();
		int m_CurSort = -1;
		bool m_CurReverse = false;
		bool m_bWaiting = false;
		double m_SortWait;

		bool m_BlockRedo = false;
		bool m_RedoBlocked = false;
		public bool BlockRedo
		{
			get { return m_BlockRedo; }
			set
			{
				if (value == m_BlockRedo)
					return;
				bool redo = m_BlockRedo && m_RedoBlocked;
				m_BlockRedo = value;
				m_RedoBlocked = false;
				t_Loading.Visibility = m_BlockRedo ? Visibility.Visible : Visibility.Hidden;
				if (!m_BlockRedo && redo)
					RedoList();
			}
		}


		public void RedoList()
		{
			if (m_BlockRedo)
			{
				m_RedoBlocked = true;
				return;
			}
            //Debug.WriteLine(string.Format("{0} - RedoList", DateTime.Now));
            m_CourseList.Clear();
			if (m_Filter != null)
			{
                foreach (Course c in Courses.AllCourses)
                {
                    if (m_Filter.InFilter(c))
                    {
                        m_CourseList.Add(c);
                    }
                }
			}
			m_SortWait = 0;
			m_CurSort = -1;
			DoSort();
		}
		public bool DoSort() { return DoSort(false); }
		public bool DoSort(bool force)
		{
            //Debug.WriteLine(string.Format("{0} - DoSort force={1}", DateTime.Now, force));
            bool changed = false;
			if (m_CurSort != m_SortColumn)
			{
				if (force || AppWin.LastRenderSeconds >= m_SortWait)
				{
					// Yes we can... Ok do it.
					switch (m_SortColumn)
					{
						case 0: m_CourseList.Sort(cmpColumn_0); break;
						case 1: m_CourseList.Sort(cmpColumn_1); break;
						case 2: m_CourseList.Sort(cmpColumn_2); break;

						case 3: m_CourseList.Sort(cmpColumn_3); break;
					}
					m_SortWait = AppWin.LastRenderSeconds + 5.0; // Once we sorted it .. we souldn't autodo it for a little bit.
					m_CurSort = m_SortColumn;
					m_CurReverse = false;
					changed = true;
					// Cancel any waiting.
					if (m_bWaiting)
					{
						m_bWaiting = false;
						AppWin.RemoveRenderUpdate(new AppWin.RenderUpdate(SortCheck));
					}
				}
				else
				{
					if (!m_bWaiting)
					{
						m_bWaiting = true;
						AppWin.AddRenderUpdate(new AppWin.RenderUpdate(SortCheck), 0);
					}
                    //Debug.WriteLine(string.Format("{0} - DoSort false", DateTime.Now));
                    return false; // Can't do anything now.
				}
			}
			if (m_CurReverse != m_Reverse)
			{
				m_CourseList.Reverse();
				m_CurReverse = m_Reverse;
				changed = true;
			}
			if (changed)
			{
				//Debug.WriteLine("SetList");
				SetList(m_CourseList);
                //Debug.WriteLine(string.Format("{0} - DoSort true", DateTime.Now));
                return true;
			}
            //Debug.WriteLine(string.Format("{0} - DoSort false", DateTime.Now));
            return false;
		}
		public bool SortCheck(double seconds, double split)
		{
            if (Course.ms_lowPri)
                m_SortWait = AppWin.LastRenderSeconds + 5.0; // Once we sorted it .. we souldn't autodo it for a little bit.

            if (seconds >= m_SortWait)
			{
				m_bWaiting = false; // Will auto cancel after this... so don't stop the wait.
				DoSort(true);
				return true;
			}
			return false;
		}

		//===============================================
		private static int cmpColumn_0(Course a, Course b)
		{
			if (a.PerformanceHeader != null && b.PerformanceHeader != null)
			{
				if (a.PerformanceHeader.Date > b.PerformanceHeader.Date)
					return -1;
				if (a.PerformanceHeader.Date < b.PerformanceHeader.Date)
					return 1;
				int ans = String.Compare(a.PerformanceInfo.RiderName, b.PerformanceInfo.RiderName, true);
				if (ans != 0)
					return ans;
			}
			return String.Compare(a.Name.ToString(), b.Name.ToString(), true);
		}
		private static int cmpColumn_1(Course a, Course b)
		{
			if (a.Laps == b.Laps)
				return String.Compare(a.Name.ToString(), b.Name.ToString(), true);
			return a.Laps < b.Laps ? -1 : 1;
		}
		private static int cmpColumn_2(Course a, Course b)
		{
            if (Math.Round(a.MetricDistanceOrTimeWithLaps, 2) == Math.Round(b.MetricDistanceOrTimeWithLaps, 2))
				return String.Compare(a.Name.ToString(), b.Name.ToString(), true);
            return a.MetricDistanceOrTimeWithLaps < b.MetricDistanceOrTimeWithLaps ? -1 : 1;
		}
		private static int cmpColumn_3(Course a, Course b)
		{
			if (Math.Round(a.AscentMetricOrMaxPowerWithLaps, 2) == Math.Round(b.AscentMetricOrMaxPowerWithLaps, 2))
				return String.Compare(a.Name.ToString(), b.Name.ToString(), true);
            return a.AscentMetricOrMaxPowerWithLaps < b.AscentMetricOrMaxPowerWithLaps ? -1 : 1;
		}

	}
}
