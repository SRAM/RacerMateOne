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
	/// Interaction logic for CoursePickerLine.xaml
	/// </summary>
	public partial class CoursePickerLine : UserControl
	{
		//==============================================================
		public static readonly RoutedEvent ClickEvent =
			EventManager.RegisterRoutedEvent(
			"Click", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CoursePickerLine));

		public static bool UseRegistration;
		public int TValue;

		public event RoutedEventHandler Click
		{
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}
		bool m_bClickCheck = false;
		bool m_bIn = false;
		private void btn_MouseDown(object sender, MouseButtonEventArgs e)
		{
			m_bClickCheck = true;
		}
		private void btn_MouseLeave(object sender, MouseEventArgs e)
		{
			m_bIn = false;
		}
		private void btn_MouseEnter(object sender, MouseEventArgs e)
		{
			m_bIn = true;
		}
		private void btn_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (m_bIn && m_bClickCheck)
			{
				RoutedEventArgs args = new RoutedEventArgs(ClickEvent);
				RaiseEvent(args);
			}
			m_bClickCheck = false;
		}
		//=============================================================================

		Course m_Course;
		public Course Course { get { return m_Course; } }
		public CoursePickerLine(Course c)
		{
			InitializeComponent();
			m_Course = c;
		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			CourseName.Text = m_Course.Name;

			Course oc = m_Course.OriginalCourse == null ? m_Course : m_Course.OriginalCourse;
			Unregistered.Visibility = ((oc.Type & CourseType.Video) != CourseType.Zero) && !oc.Registered && UseRegistration ? Visibility.Visible:Visibility.Collapsed;			


			//nca+++ 3Sep17: never show RCV needs registration msg
			//Unregistered.Visibility = ((oc.Type & CourseType.Video) != CourseType.Zero) && !oc.Registered && UseRegistration ? Visibility.Visible:Visibility.Collapsed;
			Unregistered.Visibility = Visibility.Collapsed;
			//nca---


			Laps.Text = m_Course.StringLaps;
			//Length.Text = m_Course.StringLength;
            Length.Text = m_Course.DisplayedLengthText();
            Altitude.Text = m_Course.StringAscentBounded();

            if (m_Course.PerformanceHeader != null)
            {
                Laps.Text = "";
                //Length.Text = m_Course.StringLength;
                Length.Text = "";
                Altitude.Text = "";
                g_SecondLine.Visibility = Visibility.Visible;
                SecondLine.Text = String.Format("{0}, {1} ( {2} )",
                    m_Course.PerformanceInfo.RiderName,
                    m_Course.PerformanceHeader.Date.ToString(),
                    Statistics.SecondsToTimeString((double)m_Course.PerformanceInfo.TimeMS / 1000));
            }
            else
            {
                Laps.Text = m_Course.StringLaps;
                //Length.Text = m_Course.StringLength;
                Length.Text = m_Course.DisplayedLengthText();
                Altitude.Text = m_Course.StringAscentBounded();
			
            }
		}
	}
}
