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
	/// Interaction logic for CountDown_Old.xaml
	/// </summary>
	public partial class CountDown_Old : UserControl
	{
		//===============================================
		public static readonly RoutedEvent GoEvent =
			EventManager.RegisterRoutedEvent(
			"Go", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(CountDown_Old));

		public event RoutedEventHandler Go
		{
			add { AddHandler(GoEvent, value); }
			remove { RemoveHandler(GoEvent, value); }
		}
		//===============================================

		public enum States
		{
			Off,
			C3,
			C2,
			C1,
			GO,
		}
		public States State = States.Off;

		public CountDown_Old()
		{
			InitializeComponent();
			if (!AppWin.IsInDesignMode)
			{
				Visibility = Visibility.Collapsed;
			}
			State = States.Off;

			m_Timer.Tick += new EventHandler(timer_Tick);
			m_Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
		}
		private DispatcherTimer m_Timer = new DispatcherTimer();
		private void timer_Tick(object sender, EventArgs e)
		{
			UpdateState();
		}


		private void UpdateState()
		{
			FadeOutGo.Stop();
			FadeOut.Stop();

			switch (State)
			{
				case States.Off:
					Visibility = Visibility.Collapsed;
					break;
				case States.C3:
					State = States.C2;
					t_Num.Content = "3";
					t_Go.Visibility = Visibility.Collapsed;
					t_Num.Visibility = Visibility.Visible;
					Visibility = Visibility.Visible;
					MainBox.Opacity = 1.0;
					FadeOut.Begin();
					break;
				case States.C2:
					State = States.C1;
					t_Num.Content = "2";
					t_Go.Visibility = Visibility.Collapsed;
					t_Num.Visibility = Visibility.Visible;
					Visibility = Visibility.Visible;
					MainBox.Opacity = 1.0;
					FadeOut.Begin();
					break;
				case States.C1:
					State = States.GO;
					t_Num.Content = "1";
					t_Go.Visibility = Visibility.Collapsed;
					t_Num.Visibility = Visibility.Visible;
					Visibility = Visibility.Visible;
					MainBox.Opacity = 1.0;
					FadeOut.Begin();
					break;
				case States.GO:
					t_Go.Visibility = Visibility.Visible;
					t_Num.Visibility = Visibility.Collapsed;
					State = States.Off;
					MainBox.Opacity = 1.0;
					FadeOutGo.Begin();
					m_Timer.Stop();

					RaiseEvent(new RoutedEventArgs(GoEvent));
					break;
			}
		}


		public bool Start()
		{
			if (State != States.Off)
				return false;
			State = States.C3;
			UpdateState();
			m_Timer.Start();
			return true;
		}

		private void FadeOutGo_Completed(object sender, EventArgs e)
		{
			UpdateState();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
		}
	}
}
