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

namespace RacerMateOne.Controls
{
	/// <summary>
	/// Interaction logic for CountDown.xaml
	/// </summary>
	public partial class CountDown : UserControl
	{
		//===============================================
		public static readonly RoutedEvent GoEvent =
			EventManager.RegisterRoutedEvent(
			"Go", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CountDown));

		public event RoutedEventHandler Go
		{
			add { AddHandler(GoEvent, value); }
			remove { RemoveHandler(GoEvent, value); }
		}
		//===============================================

		public CountDown()
		{
			InitializeComponent();
			Visibility = Visibility.Collapsed;
		}

		bool m_Started;
		bool m_NeedStart;
		int m_Count;
		int m_MaxCount;

		public bool Start()
		{
			if (m_Started)
				return false;

			Opacity = 0.0;
			Visibility = Visibility.Visible;
			m_NeedStart = m_Started = true;
			m_Count = 0;
			CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);

			m_MaxCount = MainGrid.Children.Count;

			return true;
		}

		private int m_CurNum = -1;
		private UIElement m_Cur;
		private int Num
		{
			get { return m_CurNum; }
			set
			{
				int v = value < -1 ? -1 : value >= m_MaxCount ? -1 : value;
				if (v == m_CurNum)
					return;
				if (m_CurNum >= 0)
					m_Cur.Visibility = Visibility.Hidden;
				m_CurNum = v;
				if (v >= 0)
				{
					m_Cur = (UIElement)MainGrid.Children[m_CurNum];
					m_Cur.Visibility = Visibility.Visible;
				}
				else
					m_Cur = null;
			}
		}

		TimeSpan m_LastRenderTime;
		Int64 m_StartTime;
		double m_Elapsed;
		void CompositionTarget_Rendering(object sender, EventArgs e)
		{
			RenderingEventArgs args = (RenderingEventArgs)e;
			if (m_LastRenderTime == args.RenderingTime)
				return;

			m_LastRenderTime = args.RenderingTime;
			if (m_Count == 0)
			{
				// Set up the system timer.
				m_StartTime = DateTime.Now.Ticks;
				m_Elapsed = 0.0;
			}
			else
				m_Elapsed = ConvertConst.HundredNanosecondToSecond * (DateTime.Now.Ticks - m_StartTime);
			m_Count++;
			// 0.5 - Full 
			// 0.5 to 1.5 Fade out
			// 1.5 to 2 blank

			// Figure out what number and value to use.
			Num = ((int)m_Elapsed) / 2;
			if (Num >= 3)
			{
				if ((m_Started && !m_NeedStart) || Num > 3)
				{
					RaiseEvent(new RoutedEventArgs(GoEvent));
					m_Started = false;
				}
				if (m_NeedStart)			// This will make sure GO is displayed at least once when started.
					m_NeedStart = false;
			}


			if (m_Cur == null)
			{
				// We are done... turn everything off.
				Visibility = Visibility.Collapsed;
				CompositionTarget.Rendering -= new EventHandler(CompositionTarget_Rendering);
			}
			else
			{
				double cycle = m_Elapsed - Num * 2.0;
				Opacity = cycle < 0.5 ? 1.0 : cycle > 1.5 ? 0.0 : 1.5 - cycle;
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
		}
	}
}
